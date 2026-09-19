param([string]$WorkDirectory = (Join-Path $PSScriptRoot '../sources/obj/native-update'))

$ErrorActionPreference = 'Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$work = [IO.Path]::GetFullPath($WorkDirectory)
New-Item -ItemType Directory -Path $work -Force | Out-Null

function Get-VerifiedArchive($Url, $Path, $Sha256) {
    if (!(Test-Path -LiteralPath $Path)) {
        Invoke-WebRequest $Url -OutFile $Path
    }
    if ((Get-FileHash -LiteralPath $Path -Algorithm SHA256).Hash -ne $Sha256) {
        throw "Checksum mismatch: $Path"
    }
}

$sourceArchive = Join-Path $work 'libwebp-1.6.0.tar.gz'
Get-VerifiedArchive 'https://storage.googleapis.com/downloads.webmproject.org/releases/webp/libwebp-1.6.0.tar.gz' $sourceArchive 'e4ab7009bf0629fd11982d4c2aa83964cf244cffba7347ecd39019a9e38c4564'
$zigArchive = Join-Path $work 'zig.zip'
Get-VerifiedArchive 'https://ziglang.org/download/0.14.1/zig-x86_64-windows-0.14.1.zip' $zigArchive '554f5378228923ffd558eac35e21af020c73789d87afeabf4bfd16f2e6feed2c'

& tar -xf $sourceArchive -C $work
if ($LASTEXITCODE -ne 0) { throw 'Unable to extract libwebp.' }
$source = Join-Path $work 'libwebp-1.6.0'
$zig = Join-Path $work 'toolchain/zig-x86_64-windows-0.14.1/zig.exe'
if (!(Test-Path -LiteralPath $zig)) {
    Expand-Archive -LiteralPath $zigArchive -DestinationPath (Join-Path $work 'toolchain') -Force
}

$previousCache = $env:ZIG_GLOBAL_CACHE_DIR
$env:ZIG_GLOBAL_CACHE_DIR = Join-Path $work 'zig-cache'
try {
    $files = foreach ($directory in @('src/dec', 'src/enc', 'src/dsp', 'src/utils', 'sharpyuv')) {
        Get-ChildItem -LiteralPath (Join-Path $source $directory) -Filter '*.c' -File
    }
    foreach ($architecture in @('x64', 'x86')) {
        $target = if ($architecture -eq 'x64') { 'x86_64-windows-gnu' } else { 'x86-windows-gnu' }
        $output = Join-Path $work "libwebp_$architecture.dll"
        $arguments = @('-target', $target, '-mcpu=baseline', '-msse2', '-O2', '-std=c99',
            '-shared', '-static', '-DWEBP_DLL', '-DWEBP_USE_THREAD', '-U_WIN32_WINNT', '-D_WIN32_WINNT=0x0601',
            '-I', $source, '-o', $output)
        $arguments += @($files | Sort-Object FullName | ForEach-Object FullName)
        $responseFile = Join-Path $work "compile-$architecture.rsp"
        $arguments | ForEach-Object { '"' + $_.Replace('\', '/') + '"' } |
            Set-Content -LiteralPath $responseFile -Encoding ascii
        Write-Output "Building libwebp 1.6.0 for $architecture"
        & $zig cc "@$responseFile"
        if ($LASTEXITCODE -ne 0) { throw "Native build failed: $architecture" }
    }
    # Update the shipped pair only after both architectures build successfully.
    foreach ($architecture in @('x64', 'x86')) {
        Copy-Item -LiteralPath (Join-Path $work "libwebp_$architecture.dll") -Destination (Join-Path $repo "sources/libwebp_$architecture.dll")
    }
    $notices = Join-Path $repo 'third-party/libwebp'
    New-Item -ItemType Directory -Path $notices -Force | Out-Null
    foreach ($file in @('COPYING', 'PATENTS', 'AUTHORS')) {
        Copy-Item -LiteralPath (Join-Path $source $file) -Destination (Join-Path $notices $file)
    }
    $toolchain = Split-Path -Parent $zig
    Copy-Item -LiteralPath (Join-Path $toolchain 'lib/libc/mingw/COPYING') -Destination (Join-Path $notices 'MINGW-COPYING')
    Copy-Item -LiteralPath (Join-Path $toolchain 'LICENSE') -Destination (Join-Path $notices 'ZIG-LICENSE')
    Get-FileHash (Join-Path $repo 'sources/libwebp_x64.dll'), (Join-Path $repo 'sources/libwebp_x86.dll') -Algorithm SHA256
}
finally {
    $env:ZIG_GLOBAL_CACHE_DIR = $previousCache
}
