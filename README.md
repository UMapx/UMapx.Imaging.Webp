<p align="center"><img width="25%" src="https://raw.githubusercontent.com/UMapx/UMapx.Imaging.Webp/main/docs/umapxnet_big.png" /></p>
<p align="center">UMapx sub-library for encoding and decoding WebP images on Windows</p>

# Installation

Install **UMapx.Imaging.Webp** using [NuGet](https://www.nuget.org/packages/UMapx.Imaging.Webp/):

```shell
dotnet add package UMapx.Imaging.Webp
```

# Quick start

Encode an existing JPEG image and decode the resulting WebP data:

```csharp
using System.Drawing;
using UMapx.Imaging;

using var bitmap = new Bitmap("input.jpg");
byte[] lossless = bitmap.ToWebp();
byte[] lossy = bitmap.ToWebp(quality: 75, speed: 4);
using var decoded = lossless.FromWebp();
```

Replace `input.jpg` with the path to your image. The snippet uses C# 9 or later.
The [sample project](https://github.com/UMapx/UMapx.Imaging.Webp/tree/main/examples)
targets .NET 8 for Windows. Run it from the repository root with
`dotnet run --project examples/UMapx.Imaging.Webp.Example.csproj -c Release`.
Sample images are copied to its output directory; converted images are written
to the `results` folder beside the executable.

# Platform support

The library targets **.NET Standard 2.0**. It uses `System.Drawing` and ships
native libwebp 1.6.0 for **Windows x86 and x64**. Native DLLs are included as
NuGet runtime assets and selected when the consuming application is built or
published. The native builds use the Windows Universal C Runtime and SSE2.
Linux, macOS and native ARM64 processes are not supported.

# API behavior

Encoding accepts `Format24bppRgb` and `Format32bppArgb` bitmaps, with each
dimension from 1 to 16383 pixels. Other pixel formats or larger dimensions
throw `NotSupportedException`.

`ToWebp()` uses lossless compression; RGB values under fully transparent pixels
may change. `ToWebp(quality, speed)` uses lossy color compression, including at
quality 100. Both parameters are required: `quality` ranges from 0 to 100 and
also controls alpha-channel compression; alpha values may change below 100.
`speed` ranges from 0 to 9 and controls compression effort, with higher values
requesting more effort. Values outside these ranges throw
`ArgumentOutOfRangeException`.

`FromWebp()` returns a `Format32bppArgb` bitmap if the WebP image has an alpha
channel, or a `Format24bppRgb` bitmap otherwise. Empty, invalid or incomplete
WebP data throws `InvalidDataException`; animated WebP throws
`NotSupportedException`. Null inputs throw `ArgumentNullException`.

Only pixel data is converted; EXIF, ICC profiles and other image metadata are
not preserved. Dispose decoded bitmaps when finished. Encoding leaves ownership
of the input bitmap with the caller. Concurrent encodes must use separate input
bitmaps; do not modify input data while a conversion is running.

# Build and test

Run the following commands from the repository root on Windows with .NET SDK 8
or later and the .NET 8 x64 runtime installed.
Build the solution with `dotnet build UMapx.Imaging.Webp.sln -c Release`.
Run `dotnet test UMapx.Imaging.Webp.sln -c Release` to execute the xUnit tests
against the source project on .NET 8 x64. Tests are also discoverable in Visual Studio.
For coverage, run `dotnet test UMapx.Imaging.Webp.sln -c Release -p:GeneratePackageOnBuild=false -p:DebugType=portable -p:DebugSymbols=true --collect "XPlat Code Coverage"`.

Native source download links, checksums and build instructions are recorded in
`libwebp/README.md`, included in the NuGet package.

# License

The managed library is MIT licensed. The bundled libwebp and SharpYUV code is
covered by the notices in the `libwebp/` directory inside the
NuGet package.
