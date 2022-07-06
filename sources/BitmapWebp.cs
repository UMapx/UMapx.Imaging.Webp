using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace UMapx.Imaging
{
    /// <summary>
    /// Uses to work with webp bitmap format.
    /// </summary>
    public static class BitmapWebp
    {
        #region | Consts |

        /// <summary>
        /// WEBP_MAX_DIMENSION
        /// </summary>
        private const int WEBP_MAX_DIMENSION = 16383;

        #endregion

        #region | Public Decode Functions |

        /// <summary>Decode a WebP image</summary>
        /// <param name="rawWebP">The data to uncompress</param>
        /// <returns>Bitmap</returns>
        public static Bitmap FromWebp(this byte[] rawWebP)
        {
            Bitmap bmp = null;
            BitmapData bmpData = null;
            GCHandle pinnedWebP = GCHandle.Alloc(rawWebP, GCHandleType.Pinned);

            try
            {
                //Get image width and height
                GetWebpInfo(rawWebP, out int imgWidth, out int imgHeight, out bool hasAlpha, out bool hasAnimation, out string format);

                //Create a BitmapData and Lock all pixels to be written
                if (hasAlpha)
                    bmp = new Bitmap(imgWidth, imgHeight, PixelFormat.Format32bppArgb);
                else
                    bmp = new Bitmap(imgWidth, imgHeight, PixelFormat.Format24bppRgb);
                bmpData = bmp.LockBits(new Rectangle(0, 0, imgWidth, imgHeight), ImageLockMode.WriteOnly, bmp.PixelFormat);

                //Uncompress the image
                int outputSize = bmpData.Stride * imgHeight;
                IntPtr ptrData = pinnedWebP.AddrOfPinnedObject();
                if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
                     UnsafeNativeMethods.WebPDecodeBGRInto(ptrData, rawWebP.Length, bmpData.Scan0, outputSize, bmpData.Stride);
                else
                     UnsafeNativeMethods.WebPDecodeBGRAInto(ptrData, rawWebP.Length, bmpData.Scan0, outputSize, bmpData.Stride);

                return bmp;
            }
            catch (Exception) { throw; }
            finally
            {
                //Unlock the pixels
                if (bmpData != null)
                    bmp.UnlockBits(bmpData);

                //Free memory
                if (pinnedWebP.IsAllocated)
                    pinnedWebP.Free();
            }
        }
        
        #endregion

        #region | Public Encode Functions |

        /// <summary>Lossless encoding bitmap to WebP (Simple encoding API)</summary>
        /// <param name="bmp">Bitmap</param>
        /// <returns>Compressed data</returns>
        public static byte[] ToWebp(this Bitmap bmp)
        {
            //test bmp
            if (bmp.Width == 0 || bmp.Height == 0)
                throw new ArgumentException("Bitmap contains no data.", "bmp");
            if (bmp.Width > WEBP_MAX_DIMENSION || bmp.Height > WEBP_MAX_DIMENSION)
                throw new NotSupportedException("Bitmap's dimension is too large. Max is " + WEBP_MAX_DIMENSION + "x" + WEBP_MAX_DIMENSION + " pixels.");
            if (bmp.PixelFormat != PixelFormat.Format24bppRgb && bmp.PixelFormat != PixelFormat.Format32bppArgb)
                throw new NotSupportedException("Only support Format24bppRgb and Format32bppArgb pixelFormat.");

            BitmapData bmpData = null;
            IntPtr unmanagedData = IntPtr.Zero;
            try
            {
                //Get bmp data
                bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);

                //Compress the bmp data
                int size;
                if (bmp.PixelFormat == PixelFormat.Format24bppRgb)
                    size = UnsafeNativeMethods.WebPEncodeLosslessBGR(bmpData.Scan0, bmp.Width, bmp.Height, bmpData.Stride, out unmanagedData);
                else
                    size = UnsafeNativeMethods.WebPEncodeLosslessBGRA(bmpData.Scan0, bmp.Width, bmp.Height, bmpData.Stride, out unmanagedData);

                //Copy image compress data to output array
                byte[] rawWebP = new byte[size];
                Marshal.Copy(unmanagedData, rawWebP, 0, size);

                return rawWebP;
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn WebP.EncodeLossless (Simple)"); }
            finally
            {
                //Unlock the pixels
                if (bmpData != null)
                    bmp.UnlockBits(bmpData);

                //Free memory
                if (unmanagedData != IntPtr.Zero)
                    UnsafeNativeMethods.WebPFree(unmanagedData);
            }
        }
        
        /// <summary>Lossy encoding bitmap to WebP (Advanced encoding API)</summary>
        /// <param name="bmp">Bitmap</param>
        /// <param name="quality">Between 0 (lower quality, lowest file size) and 100 (highest quality, higher file size)</param>
        /// <param name="speed">Between 0 (fastest, lowest compression) and 9 (slower, best compression)</param>
        /// <returns>Compressed data</returns>
        public static byte[] ToWebp(this Bitmap bmp, int quality, int speed)
        {
            // Initialize configuration structure
            WebPConfig config = new WebPConfig();

            // Set compression parameters
            if (UnsafeNativeMethods.WebPConfigInit(ref config, WebPPreset.WEBP_PRESET_DEFAULT, 75) == 0)
                throw new Exception("Can not configure preset");

            // Add additional tuning:
            config.method = speed;

            if (config.method > 6)
                config.method = 6;

            config.quality = quality;
            config.autofilter = 1;
            config.pass = speed + 1;
            config.segments = 4;
            config.partitions = 3;
            config.thread_level = 1;
            config.alpha_quality = quality;
            config.alpha_filtering = 2;
            config.use_sharp_yuv = 1;

            // Old version does not support preprocessing 4
            if (UnsafeNativeMethods.WebPGetDecoderVersion() > 1082)
            {
                config.preprocessing = 4;
                config.use_sharp_yuv = 1;
            }
            else
                config.preprocessing = 3;

            return AdvancedEncode(bmp, config, false);
        }

        #endregion

        #region | Private Functions |

        /// <summary>Get info of WEBP data</summary>
        /// <param name="rawWebP">The data of WebP</param>
        /// <param name="width">width of image</param>
        /// <param name="height">height of image</param>
        /// <param name="has_alpha">Image has alpha channel</param>
        /// <param name="has_animation">Image is a animation</param>
        /// <param name="format">Format of image: 0 = undefined (/mixed), 1 = lossy, 2 = lossless</param>
        private static void GetWebpInfo(this byte[] rawWebP, out int width, out int height, out bool has_alpha, out bool has_animation, out string format)
        {
            VP8StatusCode result;
            GCHandle pinnedWebP = GCHandle.Alloc(rawWebP, GCHandleType.Pinned);

            try
            {
                IntPtr ptrRawWebP = pinnedWebP.AddrOfPinnedObject();

                WebPBitstreamFeatures features = new WebPBitstreamFeatures();
                result = UnsafeNativeMethods.WebPGetFeatures(ptrRawWebP, rawWebP.Length, ref features);

                if (result != 0)
                    throw new Exception(result.ToString());

                width = features.Width;
                height = features.Height;
                if (features.Has_alpha == 1) has_alpha = true; else has_alpha = false;
                if (features.Has_animation == 1) has_animation = true; else has_animation = false;
                switch (features.Format)
                {
                    case 1:
                        format = "lossy";
                        break;
                    case 2:
                        format = "lossless";
                        break;
                    default:
                        format = "undefined";
                        break;
                }
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn WebP.GetInfo"); }
            finally
            {
                //Free memory
                if (pinnedWebP.IsAllocated)
                    pinnedWebP.Free();
            }
        }

        /// <summary>Encoding image  using Advanced encoding API</summary>
        /// <param name="bmp">Bitmap with the image</param>
        /// <param name="config">Configuration for encode</param>
        /// <param name="info">True if need encode info.</param>
        /// <returns>Compressed data</returns>
        private static byte[] AdvancedEncode(this Bitmap bmp, WebPConfig config, bool info)
        {
            byte[] rawWebP = null;
            byte[] dataWebp = null;
            WebPPicture wpic = new WebPPicture();
            BitmapData bmpData = null;
            WebPAuxStats stats = new WebPAuxStats();
            IntPtr ptrStats = IntPtr.Zero;
            GCHandle pinnedArrayHandle = new GCHandle();
            int dataWebpSize;
            try
            {
                //Validate the configuration
                if (UnsafeNativeMethods.WebPValidateConfig(ref config) != 1)
                    throw new Exception("Bad configuration parameters");

                //test bmp
                if (bmp.Width == 0 || bmp.Height == 0)
                    throw new ArgumentException("Bitmap contains no data.", "bmp");
                if (bmp.Width > WEBP_MAX_DIMENSION || bmp.Height > WEBP_MAX_DIMENSION)
                    throw new NotSupportedException("Bitmap's dimension is too large. Max is " + WEBP_MAX_DIMENSION + "x" + WEBP_MAX_DIMENSION + " pixels.");
                if (bmp.PixelFormat != PixelFormat.Format24bppRgb && bmp.PixelFormat != PixelFormat.Format32bppArgb)
                    throw new NotSupportedException("Only support Format24bppRgb and Format32bppArgb pixelFormat.");

                // Setup the input data, allocating a the bitmap, width and height
                bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
                if (UnsafeNativeMethods.WebPPictureInitInternal(ref wpic) != 1)
                    throw new Exception("Can´t initialize WebPPictureInit");

                wpic.width = (int)bmp.Width;
                wpic.height = (int)bmp.Height;
                wpic.use_argb = 1;

                if (bmp.PixelFormat == PixelFormat.Format32bppArgb)
                {
                    //Put the bitmap componets in wpic
                    int result = UnsafeNativeMethods.WebPPictureImportBGRA(ref wpic, bmpData.Scan0, bmpData.Stride);

                    if (result != 1)
                        throw new Exception("Can´t allocate memory in WebPPictureImportBGRA");

                    wpic.colorspace = (uint)WEBP_CSP_MODE.MODE_bgrA;
                    dataWebpSize = bmp.Width * bmp.Height * 32;
                    dataWebp = new byte[bmp.Width * bmp.Height * 32];                //Memory for WebP output
                }
                else
                {
                    //Put the bitmap contents in WebPPicture instance
                    int result = UnsafeNativeMethods.WebPPictureImportBGR(ref wpic, bmpData.Scan0, bmpData.Stride);

                    if (result != 1)
                        throw new Exception("Can´t allocate memory in WebPPictureImportBGR");

                    dataWebpSize = bmp.Width * bmp.Height * 24;

                }

                //Set up statistics of compression
                if (info)
                {
                    stats = new WebPAuxStats();
                    ptrStats = Marshal.AllocHGlobal(Marshal.SizeOf(stats));
                    Marshal.StructureToPtr(stats, ptrStats, false);
                    wpic.stats = ptrStats;
                }

                //Memory for WebP output
                if (dataWebpSize > 2147483591)
                    dataWebpSize = 2147483591;

                dataWebp = new byte[bmp.Width * bmp.Height * 32];
                pinnedArrayHandle = GCHandle.Alloc(dataWebp, GCHandleType.Pinned);
                IntPtr initPtr = pinnedArrayHandle.AddrOfPinnedObject();
                wpic.custom_ptr = initPtr;

                //Set up a byte-writing method (write-to-memory, in this case)
                UnsafeNativeMethods.OnCallback = new UnsafeNativeMethods.WebPMemoryWrite(MyWriter);
                wpic.writer = Marshal.GetFunctionPointerForDelegate(UnsafeNativeMethods.OnCallback);

                //compress the input samples
                if (UnsafeNativeMethods.WebPEncode(ref config, ref wpic) != 1)
                    throw new Exception("Encoding error: " + ((WebPEncodingError)wpic.error_code).ToString());

                //Remove OnCallback
                UnsafeNativeMethods.OnCallback = null;

                //Unlock the pixels
                bmp.UnlockBits(bmpData);
                bmpData = null;

                //Copy webpData to rawWebP
                int size = (int)((long)wpic.custom_ptr - (long)initPtr);
                rawWebP = new byte[size];
                Array.Copy(dataWebp, rawWebP, size);

                //Remove compression data
                pinnedArrayHandle.Free();
                dataWebp = null;

                //Show statistics
                if (info)
                {
                    stats = (WebPAuxStats)Marshal.PtrToStructure(ptrStats, typeof(WebPAuxStats));
                    Console.WriteLine("Dimension: " + wpic.width + " x " + wpic.height + " pixels\n" +
                                    "Output:    " + stats.coded_size + " bytes\n" +
                                    "PSNR Y:    " + stats.PSNRY + " db\n" +
                                    "PSNR u:    " + stats.PSNRU + " db\n" +
                                    "PSNR v:    " + stats.PSNRV + " db\n" +
                                    "PSNR ALL:  " + stats.PSNRALL + " db\n" +
                                    "Block intra4:  " + stats.block_count_intra4 + "\n" +
                                    "Block intra16: " + stats.block_count_intra16 + "\n" +
                                    "Block skipped: " + stats.block_count_skipped + "\n" +
                                    "Header size:    " + stats.header_bytes + " bytes\n" +
                                    "Mode-partition: " + stats.mode_partition_0 + " bytes\n" +
                                    "Macro-blocks 0: " + stats.segment_size_segments0 + " residuals bytes\n" +
                                    "Macro-blocks 1: " + stats.segment_size_segments1 + " residuals bytes\n" +
                                    "Macro-blocks 2: " + stats.segment_size_segments2 + " residuals bytes\n" +
                                    "Macro-blocks 3: " + stats.segment_size_segments3 + " residuals bytes\n" +
                                    "Quantizer    0: " + stats.segment_quant_segments0 + " residuals bytes\n" +
                                    "Quantizer    1: " + stats.segment_quant_segments1 + " residuals bytes\n" +
                                    "Quantizer    2: " + stats.segment_quant_segments2 + " residuals bytes\n" +
                                    "Quantizer    3: " + stats.segment_quant_segments3 + " residuals bytes\n" +
                                    "Filter level 0: " + stats.segment_level_segments0 + " residuals bytes\n" +
                                    "Filter level 1: " + stats.segment_level_segments1 + " residuals bytes\n" +
                                    "Filter level 2: " + stats.segment_level_segments2 + " residuals bytes\n" +
                                    "Filter level 3: " + stats.segment_level_segments3 + " residuals bytes\n", "Compression statistics");
                }

                return rawWebP;
            }
            catch (Exception ex) { throw new Exception(ex.Message + "\r\nIn WebP.AdvancedEncode"); }
            finally
            {
                //Free temporal compress memory
                if (pinnedArrayHandle.IsAllocated)
                {
                    pinnedArrayHandle.Free();
                }

                //Free statistics memory
                if (ptrStats != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptrStats);
                }

                //Unlock the pixels
                if (bmpData != null)
                {
                    bmp.UnlockBits(bmpData);
                }

                //Free memory
                if (wpic.argb != IntPtr.Zero)
                {
                    UnsafeNativeMethods.WebPPictureFree(ref wpic);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="data_size"></param>
        /// <param name="picture"></param>
        /// <returns></returns>
        private static int MyWriter([InAttribute()] IntPtr data, UIntPtr data_size, ref WebPPicture picture)
        {
            UnsafeNativeMethods.CopyMemory(picture.custom_ptr, data, (uint)data_size);
            picture.custom_ptr = new IntPtr(picture.custom_ptr.ToInt64() + (int)data_size);
            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="data_size"></param>
        /// <param name="picture"></param>
        /// <returns></returns>
        private delegate int MyWriterDelegate([InAttribute()] IntPtr data, UIntPtr data_size, ref WebPPicture picture);
        
        #endregion
    }
}