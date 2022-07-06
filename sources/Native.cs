using System;
using System.Runtime.InteropServices;
using System.Security;

namespace UMapx.Imaging
{
    #region | Import libwebp functions |

    [SuppressUnmanagedCodeSecurityAttribute]
    internal sealed partial class UnsafeNativeMethods
    {

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        internal static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        private static readonly int WEBP_DECODER_ABI_VERSION = 0x0208;

        /// <summary>This function will initialize the configuration according to a predefined set of parameters (referred to by 'preset') and a given quality factor</summary>
        /// <param name="config">The WebPConfig structure</param>
        /// <param name="preset">Type of image</param>
        /// <param name="quality">Quality of compression</param>
        /// <returns>0 if error</returns>
        internal static int WebPConfigInit(ref WebPConfig config, WebPPreset preset, float quality)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPConfigInitInternal_x86(ref config, preset, quality, WEBP_DECODER_ABI_VERSION);
                case 8:
                    return WebPConfigInitInternal_x64(ref config, preset, quality, WEBP_DECODER_ABI_VERSION);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPConfigInitInternal")]
        private static extern int WebPConfigInitInternal_x86(ref WebPConfig config, WebPPreset preset, float quality, int WEBP_DECODER_ABI_VERSION);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPConfigInitInternal")]
        private static extern int WebPConfigInitInternal_x64(ref WebPConfig config, WebPPreset preset, float quality, int WEBP_DECODER_ABI_VERSION);

        /// <summary>Get info of WepP image</summary>
        /// <param name="rawWebP">Bytes[] of WebP image</param>
        /// <param name="data_size">Size of rawWebP</param>
        /// <param name="features">Features of WebP image</param>
        /// <returns>VP8StatusCode</returns>
        internal static VP8StatusCode WebPGetFeatures(IntPtr rawWebP, int data_size, ref WebPBitstreamFeatures features)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPGetFeaturesInternal_x86(rawWebP, (UIntPtr)data_size, ref features, WEBP_DECODER_ABI_VERSION);
                case 8:
                    return WebPGetFeaturesInternal_x64(rawWebP, (UIntPtr)data_size, ref features, WEBP_DECODER_ABI_VERSION);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPGetFeaturesInternal")]
        private static extern VP8StatusCode WebPGetFeaturesInternal_x86([InAttribute()] IntPtr rawWebP, UIntPtr data_size, ref WebPBitstreamFeatures features, int WEBP_DECODER_ABI_VERSION);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPGetFeaturesInternal")]
        private static extern VP8StatusCode WebPGetFeaturesInternal_x64([InAttribute()] IntPtr rawWebP, UIntPtr data_size, ref WebPBitstreamFeatures features, int WEBP_DECODER_ABI_VERSION);

        /// <summary>Activate the lossless compression mode with the desired efficiency</summary>
        /// <param name="config">The WebPConfig struct</param>
        /// <param name="level">between 0 (fastest, lowest compression) and 9 (slower, best compression)</param>
        /// <returns>0 in case of parameter error</returns>
        internal static int WebPConfigLosslessPreset(ref WebPConfig config, int level)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPConfigLosslessPreset_x86(ref config, level);
                case 8:
                    return WebPConfigLosslessPreset_x64(ref config, level);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPConfigLosslessPreset")]
        private static extern int WebPConfigLosslessPreset_x86(ref WebPConfig config, int level);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPConfigLosslessPreset")]
        private static extern int WebPConfigLosslessPreset_x64(ref WebPConfig config, int level);

        /// <summary>Check that configuration is non-NULL and all configuration parameters are within their valid ranges</summary>
        /// <param name="config">The WebPConfig structure</param>
        /// <returns>1 if configuration is OK</returns>
        internal static int WebPValidateConfig(ref WebPConfig config)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPValidateConfig_x86(ref config);
                case 8:
                    return WebPValidateConfig_x64(ref config);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPValidateConfig")]
        private static extern int WebPValidateConfig_x86(ref WebPConfig config);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPValidateConfig")]
        private static extern int WebPValidateConfig_x64(ref WebPConfig config);

        /// <summary>Initialize the WebPPicture structure checking the DLL version</summary>
        /// <param name="wpic">The WebPPicture structure</param>
        /// <returns>1 if not error</returns>
        internal static int WebPPictureInitInternal(ref WebPPicture wpic)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPPictureInitInternal_x86(ref wpic, WEBP_DECODER_ABI_VERSION);
                case 8:
                    return WebPPictureInitInternal_x64(ref wpic, WEBP_DECODER_ABI_VERSION);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureInitInternal")]
        private static extern int WebPPictureInitInternal_x86(ref WebPPicture wpic, int WEBP_DECODER_ABI_VERSION);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureInitInternal")]
        private static extern int WebPPictureInitInternal_x64(ref WebPPicture wpic, int WEBP_DECODER_ABI_VERSION);

        /// <summary>Colorspace conversion function to import RGB samples</summary>
        /// <param name="wpic">The WebPPicture structure</param>
        /// <param name="bgr">Point to BGR data</param>
        /// <param name="stride">stride of BGR data</param>
        /// <returns>Returns 0 in case of memory error.</returns>
        internal static int WebPPictureImportBGR(ref WebPPicture wpic, IntPtr bgr, int stride)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPPictureImportBGR_x86(ref wpic, bgr, stride);
                case 8:
                    return WebPPictureImportBGR_x64(ref wpic, bgr, stride);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportBGR")]
        private static extern int WebPPictureImportBGR_x86(ref WebPPicture wpic, IntPtr bgr, int stride);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportBGR")]
        private static extern int WebPPictureImportBGR_x64(ref WebPPicture wpic, IntPtr bgr, int stride);

        /// <summary>Color-space conversion function to import RGB samples</summary>
        /// <param name="wpic">The WebPPicture structure</param>
        /// <param name="bgra">Point to BGRA data</param>
        /// <param name="stride">stride of BGRA data</param>
        /// <returns>Returns 0 in case of memory error.</returns>
        internal static int WebPPictureImportBGRA(ref WebPPicture wpic, IntPtr bgra, int stride)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPPictureImportBGRA_x86(ref wpic, bgra, stride);
                case 8:
                    return WebPPictureImportBGRA_x64(ref wpic, bgra, stride);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportBGRA")]
        private static extern int WebPPictureImportBGRA_x86(ref WebPPicture wpic, IntPtr bgra, int stride);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportBGRA")]
        private static extern int WebPPictureImportBGRA_x64(ref WebPPicture wpic, IntPtr bgra, int stride);

        /// <summary>Color-space conversion function to import RGB samples</summary>
        /// <param name="wpic">The WebPPicture structure</param>
        /// <param name="bgr">Point to BGR data</param>
        /// <param name="stride">stride of BGR data</param>
        /// <returns>Returns 0 in case of memory error.</returns>
        internal static int WebPPictureImportBGRX(ref WebPPicture wpic, IntPtr bgr, int stride)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPPictureImportBGRX_x86(ref wpic, bgr, stride);
                case 8:
                    return WebPPictureImportBGRX_x64(ref wpic, bgr, stride);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportBGRX")]
        private static extern int WebPPictureImportBGRX_x86(ref WebPPicture wpic, IntPtr bgr, int stride);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureImportBGRX")]
        private static extern int WebPPictureImportBGRX_x64(ref WebPPicture wpic, IntPtr bgr, int stride);

        /// <summary>The writer type for output compress data</summary>
        /// <param name="data">Data returned</param>
        /// <param name="data_size">Size of data returned</param>
        /// <param name="wpic">Picture structure</param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int WebPMemoryWrite([In()] IntPtr data, UIntPtr data_size, ref WebPPicture wpic);
        internal static WebPMemoryWrite OnCallback;

        /// <summary>Compress to WebP format</summary>
        /// <param name="config">The configuration structure for compression parameters</param>
        /// <param name="picture">'picture' hold the source samples in both YUV(A) or ARGB input</param>
        /// <returns>Returns 0 in case of error, 1 otherwise. In case of error, picture->error_code is updated accordingly.</returns>
        internal static int WebPEncode(ref WebPConfig config, ref WebPPicture picture)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPEncode_x86(ref config, ref picture);
                case 8:
                    return WebPEncode_x64(ref config, ref picture);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncode")]
        private static extern int WebPEncode_x86(ref WebPConfig config, ref WebPPicture picture);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncode")]
        private static extern int WebPEncode_x64(ref WebPConfig config, ref WebPPicture picture);

        /// <summary>Release the memory allocated by WebPPictureAlloc() or WebPPictureImport*()
        /// Note that this function does _not_ free the memory used by the 'picture' object itself.
        /// Besides memory (which is reclaimed) all other fields of 'picture' are preserved</summary>
        /// <param name="picture">Picture structure</param>
        internal static void WebPPictureFree(ref WebPPicture picture)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    WebPPictureFree_x86(ref picture);
                    break;
                case 8:
                    WebPPictureFree_x64(ref picture);
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureFree")]
        private static extern void WebPPictureFree_x86(ref WebPPicture wpic);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureFree")]
        private static extern void WebPPictureFree_x64(ref WebPPicture wpic);

        /// <summary>Validate the WebP image header and retrieve the image height and width. Pointers *width and *height can be passed NULL if deemed irrelevant</summary>
        /// <param name="data">Pointer to WebP image data</param>
        /// <param name="data_size">This is the size of the memory block pointed to by data containing the image data</param>
        /// <param name="width">The range is limited currently from 1 to 16383</param>
        /// <param name="height">The range is limited currently from 1 to 16383</param>
        /// <returns>1 if success, otherwise error code returned in the case of (a) formatting error(s).</returns>
        internal static int WebPGetInfo(IntPtr data, int data_size, out int width, out int height)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPGetInfo_x86(data, (UIntPtr)data_size, out width, out height);
                case 8:
                    return WebPGetInfo_x64(data, (UIntPtr)data_size, out width, out height);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPGetInfo")]
        private static extern int WebPGetInfo_x86([InAttribute()] IntPtr data, UIntPtr data_size, out int width, out int height);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPGetInfo")]
        private static extern int WebPGetInfo_x64([InAttribute()] IntPtr data, UIntPtr data_size, out int width, out int height);

        /// <summary>Decode WEBP image pointed to by *data and returns BGR samples into a preallocated buffer</summary>
        /// <param name="data">Pointer to WebP image data</param>
        /// <param name="data_size">This is the size of the memory block pointed to by data containing the image data</param>
        /// <param name="output_buffer">Pointer to decoded WebP image</param>
        /// <param name="output_buffer_size">Size of allocated buffer</param>
        /// <param name="output_stride">Specifies the distance between scan lines</param>
        internal static void WebPDecodeBGRInto(IntPtr data, int data_size, IntPtr output_buffer, int output_buffer_size, int output_stride)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    if (WebPDecodeBGRInto_x86(data, (UIntPtr)data_size, output_buffer, output_buffer_size, output_stride) == null)
                        throw new InvalidOperationException("Can not decode WebP");
                    break;
                case 8:
                    if (WebPDecodeBGRInto_x64(data, (UIntPtr)data_size, output_buffer, output_buffer_size, output_stride) == null)
                        throw new InvalidOperationException("Can not decode WebP");
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPDecodeBGRInto")]
        private static extern IntPtr WebPDecodeBGRInto_x86([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, int output_buffer_size, int output_stride);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPDecodeBGRInto")]
        private static extern IntPtr WebPDecodeBGRInto_x64([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, int output_buffer_size, int output_stride);

        /// <summary>Decode WEBP image pointed to by *data and returns BGRA samples into a preallocated buffer</summary>
        /// <param name="data">Pointer to WebP image data</param>
        /// <param name="data_size">This is the size of the memory block pointed to by data containing the image data</param>
        /// <param name="output_buffer">Pointer to decoded WebP image</param>
        /// <param name="output_buffer_size">Size of allocated buffer</param>
        /// <param name="output_stride">Specifies the distance between scan lines</param>
        internal static void WebPDecodeBGRAInto(IntPtr data, int data_size, IntPtr output_buffer, int output_buffer_size, int output_stride)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    if (WebPDecodeBGRAInto_x86(data, (UIntPtr)data_size, output_buffer, output_buffer_size, output_stride) == null)
                        throw new InvalidOperationException("Can not decode WebP");
                    break;
                case 8:
                    if (WebPDecodeBGRAInto_x64(data, (UIntPtr)data_size, output_buffer, output_buffer_size, output_stride) == null)
                        throw new InvalidOperationException("Can not decode WebP");
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPDecodeBGRAInto")]
        private static extern IntPtr WebPDecodeBGRAInto_x86([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, int output_buffer_size, int output_stride);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPDecodeBGRAInto")]
        private static extern IntPtr WebPDecodeBGRAInto_x64([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, int output_buffer_size, int output_stride);

        /// <summary>Decode WEBP image pointed to by *data and returns ARGB samples into a preallocated buffer</summary>
        /// <param name="data">Pointer to WebP image data</param>
        /// <param name="data_size">This is the size of the memory block pointed to by data containing the image data</param>
        /// <param name="output_buffer">Pointer to decoded WebP image</param>
        /// <param name="output_buffer_size">Size of allocated buffer</param>
        /// <param name="output_stride">Specifies the distance between scan lines</param>
        internal static void WebPDecodeARGBInto(IntPtr data, int data_size, IntPtr output_buffer, int output_buffer_size, int output_stride)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    if (WebPDecodeARGBInto_x86(data, (UIntPtr)data_size, output_buffer, output_buffer_size, output_stride) == null)
                        throw new InvalidOperationException("Can not decode WebP");
                    break;
                case 8:
                    if (WebPDecodeARGBInto_x64(data, (UIntPtr)data_size, output_buffer, output_buffer_size, output_stride) == null)
                        throw new InvalidOperationException("Can not decode WebP");
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPDecodeARGBInto")]
        private static extern IntPtr WebPDecodeARGBInto_x86([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, int output_buffer_size, int output_stride);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPDecodeARGBInto")]
        private static extern IntPtr WebPDecodeARGBInto_x64([InAttribute()] IntPtr data, UIntPtr data_size, IntPtr output_buffer, int output_buffer_size, int output_stride);

        /// <summary>Initialize the configuration as empty. This function must always be called first, unless WebPGetFeatures() is to be called</summary>
        /// <param name="webPDecoderConfig">Configuration structure</param>
        /// <returns>False in case of mismatched version.</returns>
        internal static int WebPInitDecoderConfig(ref WebPDecoderConfig webPDecoderConfig)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPInitDecoderConfigInternal_x86(ref webPDecoderConfig, WEBP_DECODER_ABI_VERSION);
                case 8:
                    return WebPInitDecoderConfigInternal_x64(ref webPDecoderConfig, WEBP_DECODER_ABI_VERSION);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPInitDecoderConfigInternal")]
        private static extern int WebPInitDecoderConfigInternal_x86(ref WebPDecoderConfig webPDecoderConfig, int WEBP_DECODER_ABI_VERSION);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPInitDecoderConfigInternal")]
        private static extern int WebPInitDecoderConfigInternal_x64(ref WebPDecoderConfig webPDecoderConfig, int WEBP_DECODER_ABI_VERSION);

        /// <summary>Decodes the full data at once, taking configuration into account</summary>
        /// <param name="data">WebP raw data to decode</param>
        /// <param name="data_size">Size of WebP data </param>
        /// <param name="webPDecoderConfig">Configuration structure</param>
        /// <returns>VP8_STATUS_OK if the decoding was successful</returns>
        internal static VP8StatusCode WebPDecode(IntPtr data, int data_size, ref WebPDecoderConfig webPDecoderConfig)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPDecode_x86(data, (UIntPtr)data_size, ref webPDecoderConfig);
                case 8:
                    return WebPDecode_x64(data, (UIntPtr)data_size, ref webPDecoderConfig);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPDecode")]
        private static extern VP8StatusCode WebPDecode_x86(IntPtr data, UIntPtr data_size, ref WebPDecoderConfig config);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPDecode")]
        private static extern VP8StatusCode WebPDecode_x64(IntPtr data, UIntPtr data_size, ref WebPDecoderConfig config);

        /// <summary>Free any memory associated with the buffer. Must always be called last. Doesn't free the 'buffer' structure itself</summary>
        /// <param name="buffer">WebPDecBuffer</param>
        internal static void WebPFreeDecBuffer(ref WebPDecBuffer buffer)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    WebPFreeDecBuffer_x86(ref buffer);
                    break;
                case 8:
                    WebPFreeDecBuffer_x64(ref buffer);
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPFreeDecBuffer")]
        private static extern void WebPFreeDecBuffer_x86(ref WebPDecBuffer buffer);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPFreeDecBuffer")]
        private static extern void WebPFreeDecBuffer_x64(ref WebPDecBuffer buffer);

        /// <summary>Lossy encoding images</summary>
        /// <param name="bgr">Pointer to BGR image data</param>
        /// <param name="width">The range is limited currently from 1 to 16383</param>
        /// <param name="height">The range is limited currently from 1 to 16383</param>
        /// <param name="stride">Specifies the distance between scanlines</param>
        /// <param name="quality_factor">Ranges from 0 (lower quality) to 100 (highest quality). Controls the loss and quality during compression</param>
        /// <param name="output">output_buffer with WebP image</param>
        /// <returns>Size of WebP Image or 0 if an error occurred</returns>
        internal static int WebPEncodeBGR(IntPtr bgr, int width, int height, int stride, float quality_factor, out IntPtr output)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPEncodeBGR_x86(bgr, width, height, stride, quality_factor, out output);
                case 8:
                    return WebPEncodeBGR_x64(bgr, width, height, stride, quality_factor, out output);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeBGR")]
        private static extern int WebPEncodeBGR_x86([InAttribute()] IntPtr bgr, int width, int height, int stride, float quality_factor, out IntPtr output);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeBGR")]
        private static extern int WebPEncodeBGR_x64([InAttribute()] IntPtr bgr, int width, int height, int stride, float quality_factor, out IntPtr output);

        /// <summary>Lossy encoding images</summary>
        /// <param name="bgr">Pointer to BGRA image data</param>
        /// <param name="width">The range is limited currently from 1 to 16383</param>
        /// <param name="height">The range is limited currently from 1 to 16383</param>
        /// <param name="stride">Specifies the distance between scan lines</param>
        /// <param name="quality_factor">Ranges from 0 (lower quality) to 100 (highest quality). Controls the loss and quality during compression</param>
        /// <param name="output">output_buffer with WebP image</param>
        /// <returns>Size of WebP Image or 0 if an error occurred</returns>
        internal static int WebPEncodeBGRA(IntPtr bgra, int width, int height, int stride, float quality_factor, out IntPtr output)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPEncodeBGRA_x86(bgra, width, height, stride, quality_factor, out output);
                case 8:
                    return WebPEncodeBGRA_x64(bgra, width, height, stride, quality_factor, out output);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeBGRA")]
        private static extern int WebPEncodeBGRA_x86([InAttribute()] IntPtr bgra, int width, int height, int stride, float quality_factor, out IntPtr output);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeBGRA")]
        private static extern int WebPEncodeBGRA_x64([InAttribute()] IntPtr bgra, int width, int height, int stride, float quality_factor, out IntPtr output);

        /// <summary>Lossless encoding images pointed to by *data in WebP format</summary>
        /// <param name="bgr">Pointer to BGR image data</param>
        /// <param name="width">The range is limited currently from 1 to 16383</param>
        /// <param name="height">The range is limited currently from 1 to 16383</param>
        /// <param name="stride">Specifies the distance between scan lines</param>
        /// <param name="output">output_buffer with WebP image</param>
        /// <returns>Size of WebP Image or 0 if an error occurred</returns>
        internal static int WebPEncodeLosslessBGR(IntPtr bgr, int width, int height, int stride, out IntPtr output)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPEncodeLosslessBGR_x86(bgr, width, height, stride, out output);
                case 8:
                    return WebPEncodeLosslessBGR_x64(bgr, width, height, stride, out output);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeLosslessBGR")]
        private static extern int WebPEncodeLosslessBGR_x86([InAttribute()] IntPtr bgr, int width, int height, int stride, out IntPtr output);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeLosslessBGR")]
        private static extern int WebPEncodeLosslessBGR_x64([InAttribute()] IntPtr bgr, int width, int height, int stride, out IntPtr output);

        /// <summary>Lossless encoding images pointed to by *data in WebP format</summary>
        /// <param name="bgra">Pointer to BGRA image data</param>
        /// <param name="width">The range is limited currently from 1 to 16383</param>
        /// <param name="height">The range is limited currently from 1 to 16383</param>
        /// <param name="stride">Specifies the distance between scan lines</param>
        /// <param name="output">output_buffer with WebP image</param>
        /// <returns>Size of WebP Image or 0 if an error occurred</returns>
        internal static int WebPEncodeLosslessBGRA(IntPtr bgra, int width, int height, int stride, out IntPtr output)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPEncodeLosslessBGRA_x86(bgra, width, height, stride, out output);
                case 8:
                    return WebPEncodeLosslessBGRA_x64(bgra, width, height, stride, out output);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeLosslessBGRA")]
        private static extern int WebPEncodeLosslessBGRA_x86([InAttribute()] IntPtr bgra, int width, int height, int stride, out IntPtr output);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPEncodeLosslessBGRA")]
        private static extern int WebPEncodeLosslessBGRA_x64([InAttribute()] IntPtr bgra, int width, int height, int stride, out IntPtr output);

        /// <summary>Releases memory returned by the WebPEncode</summary>
        /// <param name="p">Pointer to memory</param>
        internal static void WebPFree(IntPtr p)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    WebPFree_x86(p);
                    break;
                case 8:
                    WebPFree_x64(p);
                    break;
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPFree")]
        private static extern void WebPFree_x86(IntPtr p);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPFree")]
        private static extern void WebPFree_x64(IntPtr p);

        /// <summary>Get the WebP version library</summary>
        /// <returns>8bits for each of major/minor/revision packet in integer. E.g: v2.5.7 is 0x020507</returns>
        internal static int WebPGetDecoderVersion()
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPGetDecoderVersion_x86();
                case 8:
                    return WebPGetDecoderVersion_x64();
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPGetDecoderVersion")]
        private static extern int WebPGetDecoderVersion_x86();
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPGetDecoderVersion")]
        private static extern int WebPGetDecoderVersion_x64();

        /// <summary>Compute PSNR, SSIM or LSIM distortion metric between two pictures</summary>
        /// <param name="srcPicture">Picture to measure</param>
        /// <param name="refPicture">Reference picture</param>
        /// <param name="metric_type">0 = PSNR, 1 = SSIM, 2 = LSIM</param>
        /// <param name="pResult">dB in the Y/U/V/Alpha/All order</param>
        /// <returns>False in case of error (the two pictures don't have same dimension, ...)</returns>
        internal static int WebPPictureDistortion(ref WebPPicture srcPicture, ref WebPPicture refPicture, int metric_type, IntPtr pResult)
        {
            switch (IntPtr.Size)
            {
                case 4:
                    return WebPPictureDistortion_x86(ref srcPicture, ref refPicture, metric_type, pResult);
                case 8:
                    return WebPPictureDistortion_x64(ref srcPicture, ref refPicture, metric_type, pResult);
                default:
                    throw new InvalidOperationException("Invalid platform. Can not find proper function");
            }
        }
        [DllImport("libwebp_x86.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureDistortion")]
        private static extern int WebPPictureDistortion_x86(ref WebPPicture srcPicture, ref WebPPicture refPicture, int metric_type, IntPtr pResult);
        [DllImport("libwebp_x64.dll", CallingConvention = CallingConvention.Cdecl, EntryPoint = "WebPPictureDistortion")]
        private static extern int WebPPictureDistortion_x64(ref WebPPicture srcPicture, ref WebPPicture refPicture, int metric_type, IntPtr pResult);
    }
    #endregion

    #region | Predefined |

    /// <summary>Enumerate some predefined settings for WebPConfig, depending on the type of source picture. These presets are used when calling WebPConfigPreset()</summary>
    internal enum WebPPreset
    {
        /// <summary>Default preset</summary>
        WEBP_PRESET_DEFAULT = 0,
        /// <summary>Digital picture, like portrait, inner shot</summary>
        WEBP_PRESET_PICTURE,
        /// <summary>Outdoor photograph, with natural lighting</summary>
        WEBP_PRESET_PHOTO,
        /// <summary>Hand or line drawing, with high-contrast details</summary>
        WEBP_PRESET_DRAWING,
        /// <summary>Small-sized colorful images</summary>
        WEBP_PRESET_ICON,
        /// <summary>Text-like</summary>
        WEBP_PRESET_TEXT
    };

    /// <summary>Encoding error conditions</summary>
    internal enum WebPEncodingError
    {
        /// <summary>No error</summary>
        VP8_ENC_OK = 0,
        /// <summary>Memory error allocating objects</summary>
        VP8_ENC_ERROR_OUT_OF_MEMORY,
        /// <summary>Memory error while flushing bits</summary>
        VP8_ENC_ERROR_BITSTREAM_OUT_OF_MEMORY,
        /// <summary>A pointer parameter is NULL</summary>
        VP8_ENC_ERROR_NULL_PARAMETER,
        /// <summary>Configuration is invalid</summary>
        VP8_ENC_ERROR_INVALID_CONFIGURATION,
        /// <summary>Picture has invalid width/height</summary>
        VP8_ENC_ERROR_BAD_DIMENSION,
        /// <summary>Partition is bigger than 512k</summary>
        VP8_ENC_ERROR_PARTITION0_OVERFLOW,
        /// <summary>Partition is bigger than 16M</summary>
        VP8_ENC_ERROR_PARTITION_OVERFLOW,
        /// <summary>Error while flushing bytes</summary>
        VP8_ENC_ERROR_BAD_WRITE,
        /// <summary>File is bigger than 4G</summary>
        VP8_ENC_ERROR_FILE_TOO_BIG,
        /// <summary>Abort request by user</summary>
        VP8_ENC_ERROR_USER_ABORT,
        /// <summary>List terminator. Always last</summary>
        VP8_ENC_ERROR_LAST,
    }

    /// <summary>Enumeration of the status codes</summary>
    internal enum VP8StatusCode
    {
        /// <summary>No error</summary>
        VP8_STATUS_OK = 0,
        /// <summary>Memory error allocating objects</summary>
        VP8_STATUS_OUT_OF_MEMORY,
        /// <summary>Configuration is invalid</summary>
        VP8_STATUS_INVALID_PARAM,
        VP8_STATUS_BITSTREAM_ERROR,
        /// <summary>Configuration is invalid</summary>
        VP8_STATUS_UNSUPPORTED_FEATURE,
        VP8_STATUS_SUSPENDED,
        /// <summary>Abort request by user</summary>
        VP8_STATUS_USER_ABORT,
        VP8_STATUS_NOT_ENOUGH_DATA,
    }

    /// <summary>Image characteristics hint for the underlying encoder</summary>
    internal enum WebPImageHint
    {
        /// <summary>Default preset</summary>
        WEBP_HINT_DEFAULT = 0,
        /// <summary>Digital picture, like portrait, inner shot</summary>
        WEBP_HINT_PICTURE,
        /// <summary>Outdoor photograph, with natural lighting</summary>
        WEBP_HINT_PHOTO,
        /// <summary>Discrete tone image (graph, map-tile etc)</summary>
        WEBP_HINT_GRAPH,
        /// <summary>List terminator. Always last</summary>
        WEBP_HINT_LAST
    };

    /// <summary>Describes the byte-ordering of packed samples in memory</summary>
    internal enum WEBP_CSP_MODE
    {
        /// <summary>Byte-order: R,G,B,R,G,B,..</summary>
        MODE_RGB = 0,
        /// <summary>Byte-order: R,G,B,A,R,G,B,A,..</summary>
        MODE_RGBA = 1,
        /// <summary>Byte-order: B,G,R,B,G,R,..</summary>
        MODE_BGR = 2,
        /// <summary>Byte-order: B,G,R,A,B,G,R,A,..</summary>
        MODE_BGRA = 3,
        /// <summary>Byte-order: A,R,G,B,A,R,G,B,..</summary>
        MODE_ARGB = 4,
        /// <summary>Byte-order: RGB-565: [a4 a3 a2 a1 a0 r5 r4 r3], [r2 r1 r0 g4 g3 g2 g1 g0], ...
        /// WEBP_SWAP_16BITS_CSP is defined, 
        /// Byte-order: RGB-565: [a4 a3 a2 a1 a0 b5 b4 b3], [b2 b1 b0 g4 g3 g2 g1 g0], ..</summary>
        MODE_RGBA_4444 = 5,
        /// <summary>Byte-order: RGB-565: [r4 r3 r2 r1 r0 g5 g4 g3], [g2 g1 g0 b4 b3 b2 b1 b0], ...
        /// WEBP_SWAP_16BITS_CSP is defined, 
        /// Byte-order: [b3 b2 b1 b0 a3 a2 a1 a0], [r3 r2 r1 r0 g3 g2 g1 g0], ..</summary>
        MODE_RGB_565 = 6,
        /// <summary>RGB-premultiplied transparent modes (alpha value is preserved)</summary>
        MODE_rgbA = 7,
        /// <summary>RGB-premultiplied transparent modes (alpha value is preserved)</summary>
        MODE_bgrA = 8,
        /// <summary>RGB-premultiplied transparent modes (alpha value is preserved)</summary>
        MODE_Argb = 9,
        /// <summary>RGB-premultiplied transparent modes (alpha value is preserved)</summary>
        MODE_rgbA_4444 = 10,
        /// <summary>YUV 4:2:0</summary>
        MODE_YUV = 11,
        /// <summary>YUV 4:2:0</summary>
        MODE_YUVA = 12,
        /// <summary>MODE_LAST -> 13</summary>
        MODE_LAST = 13,
    }

    /// <summary>
    /// Decoding states. State normally flows as:
    /// WEBP_HEADER->VP8_HEADER->VP8_PARTS0->VP8_DATA->DONE for a lossy image, and
    /// WEBP_HEADER->VP8L_HEADER->VP8L_DATA->DONE for a lossless image.
    /// If there is any error the decoder goes into state ERROR.
    /// </summary>
    internal enum DecState
    {
        STATE_WEBP_HEADER,  // All the data before that of the VP8/VP8L chunk.
        STATE_VP8_HEADER,   // The VP8 Frame header (within the VP8 chunk).
        STATE_VP8_PARTS0,
        STATE_VP8_DATA,
        STATE_VP8L_HEADER,
        STATE_VP8L_DATA,
        STATE_DONE,
        STATE_ERROR
    };
    #endregion

    #region | libwebp structs |

    /// <summary>Features gathered from the bit stream</summary>
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct WebPBitstreamFeatures
    {
        /// <summary>Width in pixels, as read from the bit stream</summary>
        public int Width;
        /// <summary>Height in pixels, as read from the bit stream</summary>
        public int Height;
        /// <summary>True if the bit stream contains an alpha channel</summary>
        public int Has_alpha;
        /// <summary>True if the bit stream is an animation</summary>
        public int Has_animation;
        /// <summary>0 = undefined (/mixed), 1 = lossy, 2 = lossless</summary>
        public int Format;
        /// <summary>Padding for later use</summary>
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 5, ArraySubType = UnmanagedType.U4)]
        private readonly uint[] pad;
    };

    /// <summary>Compression parameters</summary>
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct WebPConfig
    {
        /// <summary>Lossless encoding (0=lossy(default), 1=lossless)</summary>
        public int lossless;
        /// <summary>Between 0 (smallest file) and 100 (biggest)</summary>
        public float quality;
        /// <summary>Quality/speed trade-off (0=fast, 6=slower-better)</summary>
        public int method;
        /// <summary>Hint for image type (lossless only for now)</summary>
        public WebPImageHint image_hint;
        /// <summary>If non-zero, set the desired target size in bytes. Takes precedence over the 'compression' parameter</summary>
        public int target_size;
        /// <summary>If non-zero, specifies the minimal distortion to try to achieve. Takes precedence over target_size</summary>
        public float target_PSNR;
        /// <summary>Maximum number of segments to use, in [1..4]</summary>
        public int segments;
        /// <summary>Spatial Noise Shaping. 0=off, 100=maximum</summary>
        public int sns_strength;
        /// <summary>Range: [0 = off .. 100 = strongest]</summary>
        public int filter_strength;
        /// <summary>Range: [0 = off .. 7 = least sharp]</summary>
        public int filter_sharpness;
        /// <summary>Filtering type: 0 = simple, 1 = strong (only used if filter_strength > 0 or auto-filter > 0)</summary>
        public int filter_type;
        /// <summary>Auto adjust filter's strength [0 = off, 1 = on]</summary>
        public int autofilter;
        /// <summary>Algorithm for encoding the alpha plane (0 = none, 1 = compressed with WebP lossless). Default is 1</summary>
        public int alpha_compression;
        /// <summary>Predictive filtering method for alpha plane. 0: none, 1: fast, 2: best. Default if 1</summary>
        public int alpha_filtering;
        /// <summary>Between 0 (smallest size) and 100 (lossless). Default is 100</summary>
        public int alpha_quality;
        /// <summary>Number of entropy-analysis passes (in [1..10])</summary>
        public int pass;
        /// <summary>If true, export the compressed picture back. In-loop filtering is not applied</summary>
        public int show_compressed;
        /// <summary>Preprocessing filter (0=none, 1=segment-smooth, 2=pseudo-random dithering)</summary>
        public int preprocessing;
        /// <summary>Log2(number of token partitions) in [0..3] Default is set to 0 for easier progressive decoding</summary>
        public int partitions;
        /// <summary>Quality degradation allowed to fit the 512k limit on prediction modes coding (0: no degradation, 100: maximum possible degradation)</summary>
        public int partition_limit;
        /// <summary>If true, compression parameters will be remapped to better match the expected output size from JPEG compression. Generally, the output size will be similar but the degradation will be lower</summary>
        public int emulate_jpeg_size;
        /// <summary>If non-zero, try and use multi-threaded encoding</summary>
        public int thread_level;
        /// <summary>If set, reduce memory usage (but increase CPU use)</summary>
        public int low_memory;
        /// <summary>Near lossless encoding [0 = max loss .. 100 = off (default)]</summary>
        public int near_lossless;
        /// <summary>If non-zero, preserve the exact RGB values under transparent area. Otherwise, discard this invisible RGB information for better compression. The default value is 0</summary>
        public int exact;
        /// <summary>Reserved for future lossless feature</summary>
        public int delta_palettization;
        /// <summary>If needed, use sharp (and slow) RGB->YUV conversion</summary>
        public int use_sharp_yuv;
        /// <summary>Padding for later use</summary>
        private readonly int pad1;
        private readonly int pad2;
    };

    /// <summary>Main exchange structure (input samples, output bytes, statistics)</summary>
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct WebPPicture
    {
        /// <summary>Main flag for encoder selecting between ARGB or YUV input. Recommended to use ARGB input (*argb, argb_stride) for lossless, and YUV input (*y, *u, *v, etc.) for lossy</summary>
        public int use_argb;
        /// <summary>Color-space: should be YUV420 for now (=Y'CbCr). Value = 0</summary>
        public UInt32 colorspace;
        /// <summary>Width of picture (less or equal to WEBP_MAX_DIMENSION)</summary>
        public int width;
        /// <summary>Height of picture (less or equal to WEBP_MAX_DIMENSION)</summary>
        public int height;
        /// <summary>Pointer to luma plane</summary>
        public IntPtr y;
        /// <summary>Pointer to chroma U plane</summary>
        public IntPtr u;
        /// <summary>Pointer to chroma V plane</summary>
        public IntPtr v;
        /// <summary>Luma stride</summary>
        public int y_stride;
        /// <summary>Chroma stride</summary>
        public int uv_stride;
        /// <summary>Pointer to the alpha plane</summary>
        public IntPtr a;
        /// <summary>stride of the alpha plane</summary>
        public int a_stride;
        /// <summary>Padding for later use</summary>
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
        private readonly uint[] pad1;
        /// <summary>Pointer to ARGB (32 bit) plane</summary>
        public IntPtr argb;
        /// <summary>This is stride in pixels units, not bytes</summary>
        public int argb_stride;
        /// <summary>Padding for later use</summary>
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.U4)]
        private readonly uint[] pad2;
        /// <summary>Byte-emission hook, to store compressed bytes as they are ready</summary>
        public IntPtr writer;
        /// <summary>Can be used by the writer</summary>
        public IntPtr custom_ptr;
        // map for extra information (only for lossy compression mode)
        /// <summary>1: intra type, 2: segment, 3: quant, 4: intra-16 prediction mode, 5: chroma prediction mode, 6: bit cost, 7: distortion</summary>
        public int extra_info_type;
        /// <summary>If not NULL, points to an array of size ((width + 15) / 16) * ((height + 15) / 16) that will be filled with a macroblock map, depending on extra_info_type</summary>
        public IntPtr extra_info;
        /// <summary>Pointer to side statistics (updated only if not NULL)</summary>
        public IntPtr stats;
        /// <summary>Error code for the latest error encountered during encoding</summary>
        public UInt32 error_code;
        /// <summary>If not NULL, report progress during encoding</summary>
        public IntPtr progress_hook;
        /// <summary>This field is free to be set to any value and used during callbacks (like progress-report e.g.)</summary>
        public IntPtr user_data;
        /// <summary>Padding for later use</summary>
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 13, ArraySubType = UnmanagedType.U4)]
        private readonly uint[] pad3;
        /// <summary>Row chunk of memory for YUVA planes</summary>
        private readonly IntPtr memory_;
        /// <summary>Row chunk of memory for ARGB planes</summary>
        private readonly IntPtr memory_argb_;
        /// <summary>Padding for later use</summary>
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
        private readonly uint[] pad4;
    };

    /// <summary>Structure for storing auxiliary statistics (mostly for lossy encoding)</summary>
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct WebPAuxStats
    {
        /// <summary>Final size</summary>
        public int coded_size;
        /// <summary>Peak-signal-to-noise ratio for Y</summary>
        public float PSNRY;
        /// <summary>Peak-signal-to-noise ratio for U</summary>
        public float PSNRU;
        /// <summary>Peak-signal-to-noise ratio for V</summary>
        public float PSNRV;
        /// <summary>Peak-signal-to-noise ratio for All</summary>
        public float PSNRALL;
        /// <summary>Peak-signal-to-noise ratio for Alpha</summary>
        public float PSNRAlpha;
        /// <summary>Number of intra4</summary>
        public int block_count_intra4;
        /// <summary>Number of intra16</summary>
        public int block_count_intra16;
        /// <summary>Number of skipped macro-blocks</summary>
        public int block_count_skipped;
        /// <summary>Approximate number of bytes spent for header</summary>
        public int header_bytes;
        /// <summary>Approximate number of bytes spent for  mode-partition #0</summary>
        public int mode_partition_0;
        /// <summary>Approximate number of bytes spent for DC coefficients for segment 0</summary>
        public int residual_bytes_DC_segments0;
        /// <summary>Approximate number of bytes spent for AC coefficients for segment 0</summary>
        public int residual_bytes_AC_segments0;
        /// <summary>Approximate number of bytes spent for UV coefficients for segment 0</summary>
        public int residual_bytes_uv_segments0;
        /// <summary>Approximate number of bytes spent for DC coefficients for segment 1</summary>
        public int residual_bytes_DC_segments1;
        /// <summary>Approximate number of bytes spent for AC coefficients for segment 1</summary>
        public int residual_bytes_AC_segments1;
        /// <summary>Approximate number of bytes spent for UV coefficients for segment 1</summary>
        public int residual_bytes_uv_segments1;
        /// <summary>Approximate number of bytes spent for DC coefficients for segment 2</summary>
        public int residual_bytes_DC_segments2;
        /// <summary>Approximate number of bytes spent for AC coefficients for segment 2</summary>
        public int residual_bytes_AC_segments2;
        /// <summary>Approximate number of bytes spent for UV coefficients for segment 2</summary>
        public int residual_bytes_uv_segments2;
        /// <summary>Approximate number of bytes spent for DC coefficients for segment 3</summary>
        public int residual_bytes_DC_segments3;
        /// <summary>Approximate number of bytes spent for AC coefficients for segment 3</summary>
        public int residual_bytes_AC_segments3;
        /// <summary>Approximate number of bytes spent for UV coefficients for segment 3</summary>
        public int residual_bytes_uv_segments3;
        /// <summary>Number of macro-blocks in segments 0</summary>
        public int segment_size_segments0;
        /// <summary>Number of macro-blocks in segments 1</summary>
        public int segment_size_segments1;
        /// <summary>Number of macro-blocks in segments 2</summary>
        public int segment_size_segments2;
        /// <summary>Number of macro-blocks in segments 3</summary>
        public int segment_size_segments3;
        /// <summary>Quantizer values for segment 0</summary>
        public int segment_quant_segments0;
        /// <summary>Quantizer values for segment 1</summary>
        public int segment_quant_segments1;
        /// <summary>Quantizer values for segment 2</summary>
        public int segment_quant_segments2;
        /// <summary>Quantizer values for segment 3</summary>
        public int segment_quant_segments3;
        /// <summary>Filtering strength for segment 0 [0..63]</summary>
        public int segment_level_segments0;
        /// <summary>Filtering strength for segment 1 [0..63]</summary>
        public int segment_level_segments1;
        /// <summary>Filtering strength for segment 2 [0..63]</summary>
        public int segment_level_segments2;
        /// <summary>Filtering strength for segment 3 [0..63]</summary>
        public int segment_level_segments3;
        /// <summary>Size of the transparency data</summary>
        public int alpha_data_size;
        /// <summary>Size of the enhancement layer data</summary>
        public int layer_data_size;

        // lossless encoder statistics
        /// <summary>bit0:predictor bit1:cross-color transform bit2:subtract-green bit3:color indexing</summary>
        public Int32 lossless_features;
        /// <summary>Number of precision bits of histogram</summary>
        public int histogram_bits;
        /// <summary>Precision bits for transform</summary>
        public int transform_bits;
        /// <summary>Number of bits for color cache lookup</summary>
        public int cache_bits;
        /// <summary>Number of color in palette, if used</summary>
        public int palette_size;
        /// <summary>Final lossless size</summary>
        public int lossless_size;
        /// <summary>Lossless header (transform, Huffman, etc) size</summary>
        public int lossless_hdr_size;
        /// <summary>Lossless image data size</summary>
        public int lossless_data_size;
        /// <summary>Padding for later use</summary>
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.U4)]
        private readonly uint[] pad;
    };

    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct WebPDecoderConfig
    {
        /// <summary>Immutable bit stream features (optional)</summary>
        public WebPBitstreamFeatures input;
        /// <summary>Output buffer (can point to external memory)</summary>
        public WebPDecBuffer output;
        /// <summary>Decoding options</summary>
        public WebPDecoderOptions options;
    }

    /// <summary>Output buffer</summary>
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct WebPDecBuffer
    {
        /// <summary>Color space</summary>
        public WEBP_CSP_MODE colorspace;
        /// <summary>Width of image</summary>
        public int width;
        /// <summary>Height of image</summary>
        public int height;
        /// <summary>If non-zero, 'internal_memory' pointer is not used. If value is '2' or more, the external memory is considered 'slow' and multiple read/write will be avoided</summary>
        public int is_external_memory;
        /// <summary>Output buffer parameters</summary>
        public RGBA_YUVA_Buffer u;
        /// <summary>Padding for later use</summary>
        private readonly UInt32 pad1;
        /// <summary>Padding for later use</summary>
        private readonly UInt32 pad2;
        /// <summary>Padding for later use</summary>
        private readonly UInt32 pad3;
        /// <summary>Padding for later use</summary>
        private readonly UInt32 pad4;
        /// <summary>Internally allocated memory (only when is_external_memory is 0). Should not be used externally, but accessed via WebPRGBABuffer</summary>
        public IntPtr private_memory;
    }

    /// <summary>Union of buffer parameters</summary>
    [StructLayoutAttribute(LayoutKind.Explicit)]
    internal struct RGBA_YUVA_Buffer
    {
        [FieldOffsetAttribute(0)]
        public WebPRGBABuffer RGBA;

        [FieldOffsetAttribute(0)]
        public WebPYUVABuffer YUVA;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct WebPYUVABuffer
    {
        /// <summary>Pointer to luma samples</summary>
        public IntPtr y;
        /// <summary>Pointer to chroma U samples</summary>
        public IntPtr u;
        /// <summary>Pointer to chroma V samples</summary>
        public IntPtr v;
        /// <summary>Pointer to alpha samples</summary>
        public IntPtr a;
        /// <summary>Luma stride</summary>
        public int y_stride;
        /// <summary>Chroma U stride</summary>
        public int u_stride;
        /// <summary>Chroma V stride</summary>
        public int v_stride;
        /// <summary>Alpha stride</summary>
        public int a_stride;
        /// <summary>Luma plane size</summary>
        public UIntPtr y_size;
        /// <summary>Chroma plane U size</summary>
        public UIntPtr u_size;
        /// <summary>Chroma plane V size</summary>
        public UIntPtr v_size;
        /// <summary>Alpha plane size</summary>
        public UIntPtr a_size;
    }

    /// <summary>Generic structure for describing the output sample buffer</summary>
    [StructLayoutAttribute(LayoutKind.Sequential)]
    internal struct WebPRGBABuffer
    {
        /// <summary>Pointer to RGBA samples</summary>
        public IntPtr rgba;
        /// <summary>Stride in bytes from one scanline to the next</summary>
        public int stride;
        /// <summary>Total size of the RGBA buffer</summary>
        public UIntPtr size;
    }

    /// <summary>Decoding options</summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct WebPDecoderOptions
    {
        /// <summary>If true, skip the in-loop filtering</summary>
        public int bypass_filtering;
        /// <summary>If true, use faster point-wise up-sampler</summary>
        public int no_fancy_upsampling;
        /// <summary>If true, cropping is applied _first_</summary>
        public int use_cropping;
        /// <summary>Left position for cropping. Will be snapped to even values</summary>
        public int crop_left;
        /// <summary>Top position for cropping. Will be snapped to even values</summary>
        public int crop_top;
        /// <summary>Width of the cropping area</summary>
        public int crop_width;
        /// <summary>Height of the cropping area</summary>
        public int crop_height;
        /// <summary>If true, scaling is applied _afterward_</summary>
        public int use_scaling;
        /// <summary>Final width</summary>
        public int scaled_width;
        /// <summary>Final height</summary>
        public int scaled_height;
        /// <summary>If true, use multi-threaded decoding</summary>
        public int use_threads;
        /// <summary>Dithering strength (0=Off, 100=full)</summary>
        public int dithering_strength;
        /// <summary>Flip output vertically</summary>
        public int flip;
        /// <summary>Alpha dithering strength in [0..100]</summary>
        public int alpha_dithering_strength;
        /// <summary>Padding for later use</summary>
        private readonly UInt32 pad1;
        /// <summary>Padding for later use</summary>
        private readonly UInt32 pad2;
        /// <summary>Padding for later use</summary>
        private readonly UInt32 pad3;
        /// <summary>Padding for later use</summary>
        private readonly UInt32 pad4;
        /// <summary>Padding for later use</summary>
        private readonly UInt32 pad5;
    };

    #endregion
}
