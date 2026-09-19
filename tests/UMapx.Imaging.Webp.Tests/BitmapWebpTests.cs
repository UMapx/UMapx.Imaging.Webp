using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UMapx.Imaging;
using Xunit;

namespace UMapx.Imaging.Webp.Tests;

[Trait("Category", "Imaging")]
public class BitmapWebpTests
{
    [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPGetDecoderVersion")]
    private static extern int Version64();

    [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPGetDecoderVersion")]
    private static extern int Version86();

    public static IEnumerable<object[]> ImageCases()
    {
        foreach (PixelFormat format in new[] { PixelFormat.Format24bppRgb, PixelFormat.Format32bppArgb })
            foreach (int size in new[] { 1, 2, 17, 64 })
                yield return new object[] { size, format };
    }

    [Fact]
    public void TestHostUsesRequestedArchitecture()
    {
        string? expected = typeof(BitmapWebpTests).Assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .Single(attribute => attribute.Key == "TestArchitecture").Value;
        Assert.Equal(expected, IntPtr.Size == 8 ? "x64" : "x86");
    }

    [Fact]
    public void BundledNativeLibraryIsVersion160()
    {
        Assert.Equal(0x010600, IntPtr.Size == 8 ? Version64() : Version86());
    }

    [Theory, MemberData(nameof(ImageCases))]
    public void LosslessRoundtripPreservesPixels(int size, PixelFormat format)
    {
        AssertRoundtrip(size, format, lossless: true);
    }

    [Theory, MemberData(nameof(ImageCases))]
    public void LossyRoundtripPreservesDimensionsAlphaAndImageContent(int size, PixelFormat format)
    {
        AssertRoundtrip(size, format, lossless: false);
    }

    [Theory]
    [InlineData(PixelFormat.Format24bppRgb, true)]
    [InlineData(PixelFormat.Format24bppRgb, false)]
    [InlineData(PixelFormat.Format32bppArgb, true)]
    [InlineData(PixelFormat.Format32bppArgb, false)]
    public void TruncatedImageIsRejected(PixelFormat format, bool lossless)
    {
        using var bitmap = MakeBitmap(64, format);
        byte[] complete = lossless ? bitmap.ToWebp() : bitmap.ToWebp(75, 4);
        var truncated = new byte[complete.Length / 2];
        Array.Copy(complete, truncated, truncated.Length);
        Assert.Throws<InvalidDataException>(() => { using var decoded = truncated.FromWebp(); });
    }

    [Fact]
    public void NullEncodedImageIsRejected()
    {
        Assert.Throws<ArgumentNullException>("rawWebP", () => BitmapWebp.FromWebp(null!));
    }

    [Fact]
    public void EmptyEncodedImageIsRejected()
    {
        Assert.Throws<InvalidDataException>(() => Array.Empty<byte>().FromWebp());
    }

    [Fact]
    public void InvalidHeaderIsRejected()
    {
        Assert.Throws<InvalidDataException>(() => new byte[] { 1, 2, 3, 4 }.FromWebp());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void NullBitmapIsRejected(bool lossless)
    {
        Assert.Throws<ArgumentNullException>("bitmap", () =>
            lossless ? BitmapWebp.ToWebp(null!) : BitmapWebp.ToWebp(null!, 75, 4));
    }

    [Theory]
    [InlineData(-1, 4, "quality")]
    [InlineData(101, 4, "quality")]
    [InlineData(75, -1, "speed")]
    [InlineData(75, 10, "speed")]
    public void InvalidParametersLeaveBitmapReusable(int quality, int speed, string parameter)
    {
        using var bitmap = MakeBitmap(17, PixelFormat.Format32bppArgb);
        Assert.Throws<ArgumentOutOfRangeException>(parameter, () => bitmap.ToWebp(quality, speed));
        Assert.NotEmpty(bitmap.ToWebp());
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 9)]
    [InlineData(100, 0)]
    [InlineData(100, 9)]
    public void ParameterBoundariesAreSupported(int quality, int speed)
    {
        using var bitmap = MakeBitmap(17, PixelFormat.Format32bppArgb);
        using var decoded = bitmap.ToWebp(quality, speed).FromWebp();
        Assert.Equal(bitmap.Size, decoded.Size);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void UnsupportedPixelFormatIsRejected(bool lossless)
    {
        using var bitmap = new Bitmap(8, 8, PixelFormat.Format8bppIndexed);
        Assert.Throws<NotSupportedException>(() => lossless ? bitmap.ToWebp() : bitmap.ToWebp(75, 4));
    }

    [Fact]
    public void LargeOutputIsWrittenAcrossMultipleChunks()
    {
        AssertRoundtrip(256, PixelFormat.Format32bppArgb, lossless: false);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FullyTransparentPixelsRemainTransparent(bool lossless)
    {
        using var bitmap = MakeBitmap(64, PixelFormat.Format32bppArgb);
        bitmap.SetPixel(0, 0, Color.FromArgb(0, 128, 64, 32));
        byte[] bytes = lossless ? bitmap.ToWebp() : bitmap.ToWebp(75, 4);
        using var decoded = bytes.FromWebp();
        Assert.Equal(bitmap.Size, decoded.Size);
        Assert.Equal(0, decoded.GetPixel(0, 0).A);
    }

    [Fact]
    public async Task ConcurrentEncodesKeepCallbacksAliveDuringGarbageCollection()
    {
        using var stop = new CancellationTokenSource();
        Task collector = Task.Run(async () =>
        {
            while (!stop.IsCancellationRequested)
            {
                GC.Collect();
                await Task.Delay(5);
            }
        });
        try
        {
            await Task.Run(() => Parallel.For(0, 32, new ParallelOptions { MaxDegreeOfParallelism = 4 }, i =>
                AssertRoundtrip(64, i % 2 == 0 ? PixelFormat.Format24bppRgb : PixelFormat.Format32bppArgb, lossless: false)));
        }
        finally
        {
            stop.Cancel();
            await collector;
        }
    }

    private static Bitmap MakeBitmap(int size, PixelFormat format)
    {
        var bitmap = new Bitmap(size, size, format);
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                bitmap.SetPixel(x, y, Color.FromArgb(format == PixelFormat.Format32bppArgb ? 40 + (x + y) % 216 : 255,
                    x * 17 % 256, y * 29 % 256, (x * 7 + y * 11) % 256));
        return bitmap;
    }

    private static void AssertRoundtrip(int size, PixelFormat format, bool lossless)
    {
        using var bitmap = MakeBitmap(size, format);
        byte[] bytes = lossless ? bitmap.ToWebp() : bitmap.ToWebp(100, 4);
        Assert.True(bytes.Length > 12, "Empty WebP output");
        Assert.Equal("RIFF", Encoding.ASCII.GetString(bytes, 0, 4));
        Assert.Equal("WEBP", Encoding.ASCII.GetString(bytes, 8, 4));
        using var decoded = bytes.FromWebp();
        Assert.Equal(bitmap.Size, decoded.Size);
        double error = 0;
        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                Color expected = bitmap.GetPixel(x, y);
                Color actual = decoded.GetPixel(x, y);
                Assert.Equal(expected.A, actual.A);
                if (lossless) Assert.Equal(expected.ToArgb(), actual.ToArgb());
                error += Math.Abs(expected.R - actual.R) + Math.Abs(expected.G - actual.G) + Math.Abs(expected.B - actual.B);
            }
        Assert.True(error / (size * size * 3) < 45, "Decoded image differs substantially from input");
    }
}
