param(
    [string[]]$Frameworks = @('net8.0-windows'),
    [string[]]$DependencySources = @('https://api.nuget.org/v3/index.json')
)

$ErrorActionPreference = 'Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$runId = [Guid]::NewGuid().ToString('N')
$work = Join-Path $repo "tests/obj/package-tests/$runId"
New-Item -ItemType Directory -Path $work -Force | Out-Null

& dotnet build (Join-Path $repo 'sources/UMapx.Imaging.Webp.csproj') -c Release --nologo
if ($LASTEXITCODE -ne 0) { throw 'Library build failed.' }
[xml]$project = Get-Content (Join-Path $repo 'sources/UMapx.Imaging.Webp.csproj') -Raw
$version = @($project.Project.PropertyGroup.Version | Where-Object { $_ })[0]

# A fresh package cache plus source mapping ensures the local package is tested,
# even when the same version already exists in a global cache or on nuget.org.
$config = [xml]'<configuration><packageSources><clear /></packageSources><packageSourceMapping /></configuration>'
$sources = @((Join-Path $repo 'sources/bin/Release')) + $DependencySources
for ($i = 0; $i -lt $sources.Count; $i++) {
    $key = "source$i"
    $source = $config.CreateElement('add')
    $source.SetAttribute('key', $key)
    $source.SetAttribute('value', $sources[$i])
    [void]$config.configuration.packageSources.AppendChild($source)
    $mapping = $config.CreateElement('packageSource')
    $mapping.SetAttribute('key', $key)
    $package = $config.CreateElement('package')
    $package.SetAttribute('pattern', $(if ($i -eq 0) { 'UMapx.Imaging.Webp' } else { '*' }))
    [void]$mapping.AppendChild($package)
    [void]$config.configuration.SelectSingleNode('packageSourceMapping').AppendChild($mapping)
}
$configPath = Join-Path $work 'NuGet.config'
$config.Save($configPath)

foreach ($framework in $Frameworks) {
    foreach ($architecture in @('x64', 'x86')) {
        $output = Join-Path $repo "tests/bin/package-tests/$runId/$framework/$architecture"
        $intermediate = Join-Path $work "$framework/$architecture"
        $arguments = @('publish', (Join-Path $repo 'tests/UMapx.Imaging.Webp.Tests.csproj'),
            '-c', 'Release', '--nologo', "-p:TestFramework=$framework", "-p:TestArchitecture=$architecture",
            '-p:UsePackageUnderTest=true', "-p:PackageVersionUnderTest=$version",
            "-p:RestoreConfigFile=$configPath", "-p:RestorePackagesPath=$work/packages",
            "-p:IntermediateOutputPath=$intermediate/", "-p:OutputPath=$intermediate/build/",
            "-p:PublishDir=$output/", '-p:UseAppHost=false', '-p:SelfContained=false', '-r', "win-$architecture")
        & dotnet @arguments
        if ($LASTEXITCODE -ne 0) { throw "Package consumer build failed: $framework / $architecture" }
        & dotnet vstest (Join-Path $output 'UMapx.Imaging.Webp.Tests.dll') "/Platform:$architecture" "/TestAdapterPath:$output" `
            '/Logger:trx;LogFileName=tests.trx' "/ResultsDirectory:$work/results/$framework/$architecture"
        if ($LASTEXITCODE -ne 0) { throw "Regression tests failed: $framework / $architecture" }
    }
}
Write-Output 'All package-consumer checks passed.'
