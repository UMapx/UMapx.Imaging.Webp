Bundled libwebp: 1.6.0 (including SharpYUV 0.4.2).

Upstream: https://developers.google.com/speed/webp/download

Source archive: https://storage.googleapis.com/downloads.webmproject.org/releases/webp/libwebp-1.6.0.tar.gz

Source SHA-256: `e4ab7009bf0629fd11982d4c2aa83964cf244cffba7347ecd39019a9e38c4564`

The two DLLs are compiled from the unmodified upstream source archive using
Zig 0.14.1's C compiler, with Windows threads enabled, SSE2, optimization `-O2`,
and SharpYUV linked into each DLL. There is no external SharpYUV or compiler
runtime DLL dependency; the Windows Universal C Runtime is required.
The compiler is a build tool only and is not shipped in the NuGet package.
`MINGW-COPYING` and `ZIG-LICENSE` accompany the runtime support code linked
by the compiler.

Compiler archive: https://ziglang.org/download/0.14.1/zig-x86_64-windows-0.14.1.zip

Compiler SHA-256: `554f5378228923ffd558eac35e21af020c73789d87afeabf4bfd16f2e6feed2c`

| File | Target | SHA-256 |
| --- | --- | --- |
| `sources/libwebp_x64.dll` | x86_64-windows-gnu | `359c16ac843db95fa5d7e9a31bf8c59e325efae4cd0bfb6fb212e4742c0e22a0` |
| `sources/libwebp_x86.dll` | x86-windows-gnu | `e46c0c0c02bd385c1487128c92c47a8c3de7226459304523384204abb99341ae` |

To rebuild on Windows, run `./tools/build-native.ps1` from PowerShell. It verifies
the pinned archive hashes before extracting them, builds both architectures,
then replaces the DLLs and refreshes upstream `COPYING`, `PATENTS` and `AUTHORS`.
Build products and the downloaded compiler stay under `sources/obj/native-update`.
Record fresh DLL hashes here after an intentional rebuild; build paths/toolchain
metadata may affect binary hashes. Run `./tools/test.ps1` before publishing.
