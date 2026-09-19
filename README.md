<p align="center"><img width="25%" src="https://raw.githubusercontent.com/UMapx/UMapx.Imaging.Webp/main/docs/umapxnet_big.png" /></p>
<p align="center">UMapx sub-library for interacting with webp images</p>

# Installation
Install **UMapx.Imaging.Webp** to your project using [NuGet](https://www.nuget.org/packages/UMapx.Imaging.Webp/) package manager.

```c#
using UMapx.Imaging;

byte[] lossless = bitmap.ToWebp();
byte[] lossy = bitmap.ToWebp(quality: 75, speed: 4);
using var decoded = lossless.FromWebp();
```
To get started with **UMapx.Imaging.Webp** try simple [example](https://github.com/UMapx/UMapx.Imaging.Webp/tree/main/examples).
The example targets .NET 8 for Windows. Run it from the repository root with
`dotnet run --project examples/UMapx.Imaging.Webp.Example.csproj -c Release`.
Sample images are copied to its output directory; converted images are written
to the `results` folder beside the executable.

The library targets **.NET Standard 2.0**. It uses `System.Drawing` and ships
native libwebp 1.6.0 for **Windows x86 and x64**. Native DLLs are included as
NuGet runtime assets and selected when the consuming application is built or
published. The native builds use the Windows Universal C Runtime and SSE2.
Linux, macOS and native ARM64 processes are not supported.

Supported bitmap formats are `Format24bppRgb` and `Format32bppArgb`, up to
16383 pixels in each dimension. `ToWebp()` uses lossless compression; RGB values
under fully transparent pixels may change. The quality overload uses lossy
compression with quality 0–100 and effort 0–9 (higher effort is slower).
Invalid or incomplete WebP data throws `InvalidDataException`; animated WebP
is not supported. Dispose decoded bitmaps when finished. Concurrent calls must
use separate input bitmaps.

Build the solution with `dotnet build UMapx.Imaging.Webp.sln -c Release`.
The xUnit tests use the same test SDK, runner and coverage collector versions as
UMapx. Run `dotnet test UMapx.Imaging.Webp.sln -c Release` for the default
x64 tests against the source project. Tests are also discoverable in Visual Studio.
Run `./tools/test.ps1` in PowerShell to build and test the actual NuGet package
on x86 and x64 using .NET 8, with TRX reports under `tests/obj/package-tests`.
The script requires .NET SDK 8 or later and .NET 8
runtimes for x64 and x86. The library itself remains on .NET Standard 2.0.
Optional .NET Framework 4.8 compatibility checks can be run with
`./tools/test.ps1 -Frameworks net48,net8.0-windows`; these also require the
.NET Framework 4.8 targeting pack.
For coverage, run `dotnet test UMapx.Imaging.Webp.sln -c Release -p:GeneratePackageOnBuild=false -p:DebugType=portable -p:DebugSymbols=true --collect "XPlat Code Coverage"`.

Native build sources, checksums and rebuilding instructions are recorded in
`libwebp/README.md`, included in the NuGet package.

# License
The managed library is MIT licensed. The bundled libwebp and SharpYUV code is
covered by the notices in the `libwebp/` directory inside the
NuGet package.
