using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace UMapx.Imaging
{
    /// <summary>Converts Windows bitmaps to and from WebP images.</summary>
    /// <remarks>
    /// Supports Windows x86 and x64. Image metadata is not preserved.
    /// Concurrent encodes must use separate input bitmaps.
    /// </remarks>
    public static class BitmapWebp
    {
        private const int WEBP_MAX_DIMENSION = 16383;

        /// <summary>Decodes a still WebP image to a bitmap.</summary>
        /// <param name="rawWebP">Encoded WebP image.</param>
        /// <returns>
        /// A bitmap in <see cref="PixelFormat.Format32bppArgb"/> if the image has an alpha channel,
        /// or <see cref="PixelFormat.Format24bppRgb"/> otherwise. The caller must dispose it.
        /// </returns>
        /// <exception cref="ArgumentNullException">The input is null.</exception>
        /// <exception cref="InvalidDataException">The image is empty, invalid or incomplete.</exception>
        /// <exception cref="NotSupportedException">The image is animated.</exception>
        public static Bitmap FromWebp(this byte[] rawWebP)
        {
            if (rawWebP == null)
                throw new ArgumentNullException(nameof(rawWebP));
            if (rawWebP.Length == 0)
                throw new InvalidDataException("The WebP image is empty.");

            Bitmap bitmap = null;
            GCHandle pinned = GCHandle.Alloc(rawWebP, GCHandleType.Pinned);
            try
            {
                IntPtr input = pinned.AddrOfPinnedObject();
                var features = new WebPBitstreamFeatures();
                VP8StatusCode status = UnsafeNativeMethods.WebPGetFeatures(input, rawWebP.Length, ref features);
                if (status != VP8StatusCode.VP8_STATUS_OK)
                    throw new InvalidDataException("Invalid WebP image: " + status);
                if (features.Has_animation != 0)
                    throw new NotSupportedException("Animated WebP images are not supported.");

                PixelFormat format = features.Has_alpha != 0
                    ? PixelFormat.Format32bppArgb : PixelFormat.Format24bppRgb;
                bitmap = new Bitmap(features.Width, features.Height, format);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.WriteOnly, format);
                try
                {
                    int outputSize = checked(data.Stride * bitmap.Height);
                    if (format == PixelFormat.Format24bppRgb)
                        UnsafeNativeMethods.WebPDecodeBGRInto(input, rawWebP.Length, data.Scan0, outputSize, data.Stride);
                    else
                        UnsafeNativeMethods.WebPDecodeBGRAInto(input, rawWebP.Length, data.Scan0, outputSize, data.Stride);
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
                return bitmap;
            }
            catch
            {
                bitmap?.Dispose();
                throw;
            }
            finally
            {
                pinned.Free();
            }
        }

        /// <summary>Converts a bitmap to lossless WebP. RGB values of fully transparent pixels may change.</summary>
        /// <param name="bitmap">A 24bpp RGB or 32bpp ARGB bitmap, from 1 to 16383 pixels in each dimension.</param>
        /// <returns>The encoded WebP image.</returns>
        /// <remarks>The caller retains ownership of <paramref name="bitmap"/>.</remarks>
        /// <exception cref="ArgumentNullException">The bitmap is null.</exception>
        /// <exception cref="NotSupportedException">The pixel format is unsupported or a dimension exceeds 16383 pixels.</exception>
        /// <exception cref="InvalidOperationException">The WebP encoder fails.</exception>
        public static byte[] ToWebp(this Bitmap bitmap)
        {
            ValidateBitmap(bitmap);
            BitmapData data = null;
            IntPtr encoded = IntPtr.Zero;
            try
            {
                data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                    ImageLockMode.ReadOnly, bitmap.PixelFormat);
                UIntPtr nativeSize = bitmap.PixelFormat == PixelFormat.Format24bppRgb
                    ? UnsafeNativeMethods.WebPEncodeLosslessBGR(data.Scan0, bitmap.Width, bitmap.Height, data.Stride, out encoded)
                    : UnsafeNativeMethods.WebPEncodeLosslessBGRA(data.Scan0, bitmap.Width, bitmap.Height, data.Stride, out encoded);
                if (nativeSize == UIntPtr.Zero || encoded == IntPtr.Zero)
                    throw new InvalidOperationException("Unable to encode the bitmap as WebP.");
                int size = checked((int)nativeSize.ToUInt64());
                var result = new byte[size];
                Marshal.Copy(encoded, result, 0, size);
                return result;
            }
            finally
            {
                try
                {
                    if (data != null)
                        bitmap.UnlockBits(data);
                }
                finally
                {
                    if (encoded != IntPtr.Zero)
                        UnsafeNativeMethods.WebPFree(encoded);
                }
            }
        }

        /// <summary>Converts a bitmap to lossy WebP with the specified quality and compression effort.</summary>
        /// <param name="bitmap">A 24bpp RGB or 32bpp ARGB bitmap, from 1 to 16383 pixels in each dimension.</param>
        /// <param name="quality">Color and alpha quality from 0 to 100. Color compression remains lossy at 100.</param>
        /// <param name="speed">Compression effort from 0 to 9. Higher values request more effort.</param>
        /// <returns>The encoded WebP image.</returns>
        /// <remarks>Alpha values may change below quality 100. The caller retains ownership of <paramref name="bitmap"/>.</remarks>
        /// <exception cref="ArgumentNullException">The bitmap is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Quality is outside 0 to 100 or speed is outside 0 to 9.</exception>
        /// <exception cref="NotSupportedException">The pixel format is unsupported or a dimension exceeds 16383 pixels.</exception>
        /// <exception cref="InvalidOperationException">The WebP encoder fails.</exception>
        public static byte[] ToWebp(this Bitmap bitmap, int quality, int speed)
        {
            ValidateBitmap(bitmap);
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException(nameof(quality), "Quality must be between 0 and 100.");
            if (speed < 0 || speed > 9)
                throw new ArgumentOutOfRangeException(nameof(speed), "Speed must be between 0 and 9.");

            var config = new WebPConfig();
            if (UnsafeNativeMethods.WebPConfigInit(ref config, WebPPreset.WEBP_PRESET_DEFAULT, quality) == 0)
                throw new InvalidOperationException("Unable to initialize the WebP encoder.");
            config.method = Math.Min(speed, 6);
            config.autofilter = 1;
            config.pass = speed + 1;
            config.segments = 4;
            config.partitions = 3;
            config.thread_level = 1;
            config.alpha_quality = quality;
            config.alpha_filtering = 2;
            config.use_sharp_yuv = 1;
            return AdvancedEncode(bitmap, config);
        }

        private static void ValidateBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));
            if (bitmap.Width <= 0 || bitmap.Height <= 0)
                throw new ArgumentException("Bitmap contains no data.", nameof(bitmap));
            if (bitmap.Width > WEBP_MAX_DIMENSION || bitmap.Height > WEBP_MAX_DIMENSION)
                throw new NotSupportedException("Bitmap dimensions cannot exceed 16383 pixels.");
            if (bitmap.PixelFormat != PixelFormat.Format24bppRgb && bitmap.PixelFormat != PixelFormat.Format32bppArgb)
                throw new NotSupportedException("Only Format24bppRgb and Format32bppArgb are supported.");
        }

        private static byte[] AdvancedEncode(Bitmap bitmap, WebPConfig config)
        {
            if (UnsafeNativeMethods.WebPValidateConfig(ref config) != 1)
                throw new InvalidOperationException("Invalid WebP encoder configuration.");
            var picture = new WebPPicture();
            if (UnsafeNativeMethods.WebPPictureInitInternal(ref picture) != 1)
                throw new InvalidOperationException("Unable to initialize the WebP picture.");

            BitmapData data = null;
            using (var output = new MemoryStream())
            {
                var writer = new EncodedDataWriter(output);
                var callback = new UnsafeNativeMethods.WebPMemoryWrite(writer.Write);
                try
                {
                    data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                        ImageLockMode.ReadOnly, bitmap.PixelFormat);
                    picture.width = bitmap.Width;
                    picture.height = bitmap.Height;
                    picture.use_argb = 1;
                    int imported = bitmap.PixelFormat == PixelFormat.Format32bppArgb
                        ? UnsafeNativeMethods.WebPPictureImportBGRA(ref picture, data.Scan0, data.Stride)
                        : UnsafeNativeMethods.WebPPictureImportBGR(ref picture, data.Scan0, data.Stride);
                    if (imported != 1)
                        throw new InvalidOperationException("Unable to import the bitmap into WebP.");

                    picture.writer = Marshal.GetFunctionPointerForDelegate(callback);
                    if (UnsafeNativeMethods.WebPEncode(ref config, ref picture) != 1)
                        throw new InvalidOperationException("WebP encoding failed: " + (WebPEncodingError)picture.error_code, writer.Error);
                    return output.ToArray();
                }
                finally
                {
                    // Each encode owns its callback, including during concurrent encodes and GC.
                    GC.KeepAlive(callback);
                    try
                    {
                        if (data != null)
                            bitmap.UnlockBits(data);
                    }
                    finally
                    {
                        UnsafeNativeMethods.WebPPictureFree(ref picture);
                    }
                }
            }
        }

        private sealed class EncodedDataWriter
        {
            private readonly MemoryStream output;
            private readonly byte[] buffer = new byte[8192];
            internal Exception Error { get; private set; }

            internal EncodedDataWriter(MemoryStream output)
            {
                this.output = output;
            }

            internal int Write(IntPtr data, UIntPtr dataSize, ref WebPPicture picture)
            {
                try
                {
                    ulong count = dataSize.ToUInt64();
                    if (count > (ulong)(int.MaxValue - output.Length))
                        throw new IOException("The encoded WebP exceeds the maximum byte array size.");

                    int remaining = (int)count;
                    while (remaining > 0)
                    {
                        int chunk = Math.Min(remaining, buffer.Length);
                        Marshal.Copy(data, buffer, 0, chunk);
                        output.Write(buffer, 0, chunk);
                        remaining -= chunk;
                        data = IntPtr.Add(data, chunk);
                    }
                    return 1;
                }
                catch (Exception error)
                {
                    // Managed exceptions must not unwind through the native encoder.
                    Error = error;
                    return 0;
                }
            }
        }
    }
}
