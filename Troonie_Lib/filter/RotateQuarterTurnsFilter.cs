using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Troonie_Lib
{
    /// <summary> 90, 180 or 270 degrees clockwise rotation of an image. </summary> 
    public class RotateQuarterTurnsFilter : AbstractFilter
	{
        /// <summary>
        /// 1 --> source, 2 --> destination.
        /// </summary>
        private int w1, w2, h1, h2, stride1, stride2, off1, off2;

        /// <summary>Represents the possible degrees of rotation angle (clockwise).</summary>
        public enum RotationOrder
        {
			/// <summary>90 degrees.</summary>
			deg90,
            /// <summary>180 degrees.</summary>
            deg180,
            /// <summary>270 degrees.</summary>
            deg270
        }

        /// <summary>The clockwise rotation order of the image.</summary>
        /// <remarks><para>Default value is set to 
        /// <see cref="RGBOrder.deg90"/>.</para></remarks>
        public RotationOrder Order { get; set; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="RotateQuarterTurnsFilter"/> class.
        /// </summary>
        public RotateQuarterTurnsFilter()
		{
			SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
			Order = RotationOrder.deg90;
		}

        /// <summary>
        /// Applies the filter on the passed <paramref name="source"/> bitmap.
        /// </summary>
        /// <param name="source">The source image to process.</param>
        /// <returns>The filter result as a new bitmap.</returns>
        public override Bitmap Apply(Bitmap source)
        {
            CheckPixelFormat(source.PixelFormat);
            Bitmap destination;
            w1 = source.Width;
            h1 = source.Height;
            Rectangle rect1 = new Rectangle(0, 0, w1, h1);            

            if (Order == RotationOrder.deg180)
            {
                w2 = w1;
                h2 = h1;                
            }
            else
            {
                w2 = h1;
                h2 = w1;
            }

            Rectangle rect2 = new Rectangle(0, 0, w2, h2);
            destination = new Bitmap(w2, h2, dstPixelFormat);

            BitmapData srcData = source.LockBits(rect1, ImageLockMode.ReadOnly, source.PixelFormat);
            BitmapData dstData = destination.LockBits(rect2, ImageLockMode.ReadWrite, destination.PixelFormat);
            Process(srcData, dstData);
            destination.UnlockBits(dstData);
            source.UnlockBits(srcData);

            if (destination.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                SetColorPalette(destination);
            }

            return destination;
        }

        #region protected methods

        protected override void SetProperties (double[] filterProperties)
		{
			Order = (RotationOrder)filterProperties[0];
		}

		/// <summary>
		/// Processes the filter on the passed <paramref name="srcData"/>
		/// resulting into <paramref name="dstData"/>.
		/// </summary>
		/// <param name="srcData">The source bitmap data.</param>
		/// <param name="dstData">The destination bitmap data.</param>
		protected internal override unsafe void Process(BitmapData srcData, BitmapData dstData)
		{
            int ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
            stride1 = srcData.Stride;
            off1 = stride1 - w1 * ps;
            stride2 = dstData.Stride;
            off2 = stride2 - w2 * ps;
            byte* dst = (byte*)dstData.Scan0.ToPointer();

            switch (Order)
            {
                case RotationOrder.deg90:
                    // for each line
                    for (int y2 = 0; y2 < h2; y2++)
                    {
                        // align src pointer to correct column
                        byte* src = (byte*)srcData.Scan0.ToPointer();
                        src += stride1 * (h1 - 1);
                        src += ps * y2;

                        // for each pixel
                        for (int x2 = 0; x2 < w2; x2++, src -= stride1, dst += ps)
                        {
                            // 8 bit grayscale
                            dst[RGBA.B] = src[RGBA.B];

                            // rgb, 24 and 32 bit
                            if (ps >= 3)
                            {
                                dst[RGBA.G] = src[RGBA.G];
                                dst[RGBA.R] = src[RGBA.R];
                            }

                            // alpha, 32 bit
                            if (ps == 4)
                            {
                                dst[RGBA.A] = Use255ForAlpha ? (byte)255 : src[RGBA.A];
                            }
                        }
                        dst += off2;
                    }
                    break;

                case RotationOrder.deg270:
                    //// for each line
                    //for (int y = h - 1; y >= 0; y--)
                    //{
                    //    // align pointer to end of the line
                    //    byte* src = (byte*)srcData.Scan0.ToPointer();
                    //    src += stride * y;

                    //    // for each pixel
                    //    for (int x = 0; x < w; x++, src += ps, dst += ps)
                    //    {
                    //        // 8 bit grayscale
                    //        dst[RGBA.B] = src[RGBA.B];

                    //        // rgb, 24 and 32 bit
                    //        if (ps >= 3)
                    //        {
                    //            dst[RGBA.G] = src[RGBA.G];
                    //            dst[RGBA.R] = src[RGBA.R];
                    //        }

                    //        // alpha, 32 bit
                    //        if (ps == 4)
                    //        {
                    //            dst[RGBA.A] = Use255ForAlpha ? (byte)255 : src[RGBA.A];
                    //        }
                    //    }
                    //    dst += offset;
                    //}

                    break;

                case RotationOrder.deg180:
                    // for each line
                    for (int y = h1 - 1; y >= 0; y--)
                    {
                        // align pointer to end of the line
                        byte* src = (byte*)srcData.Scan0.ToPointer();
                        src += stride1 * y + stride1 - off1 - ps;
                        //					src += stride * y - offset;
                        // for each pixel
                        for (int x = 0; x < w1; x++, src -= ps, dst += ps)
                        {
                            // 8 bit grayscale
                            dst[RGBA.B] = src[RGBA.B];

                            // rgb, 24 and 32 bit
                            if (ps >= 3)
                            {
                                dst[RGBA.G] = src[RGBA.G];
                                dst[RGBA.R] = src[RGBA.R];
                            }

                            // alpha, 32 bit
                            if (ps == 4)
                            {
                                dst[RGBA.A] = Use255ForAlpha ? (byte)255 : src[RGBA.A];
                            }
                        }
                        dst += off2;
                    }

                    break;
            }
        }

		#endregion protected methods             
	}
}

