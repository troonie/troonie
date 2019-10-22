using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Troonie_Lib
{
	/// <summary>
	/// Simple contrast filter by exponentiation of channel value. 
	/// </summary>
	public class ContrastFilter : AbstractFilter
    {
		/// <summary>The pixel size</summary>
		private int ps;

		public enum Variants
		{
			exponential_Channel_Independency,
			exponential_Biggest_Channel_For_Threshold_Decision,
			linear_Channel_Independency,
			linear_Biggest_Channel_For_Threshold_Decision
		}

		public Variants Variant { get; set; }

		/// <summary>Linear value for manipulating channel result. Lightness in- or decreasing by linear addition/subtraction.</summary>
		public byte Linear { get; set; }

		/// <summary>Exponent for manipulating channel result. Lightness in- or decreasing by exponentiation.</summary>
		public float Exp { get; set; }

		/// <summary>Threshold to determine whether increasing or decreasing channel value.</summary>
		public byte Threshold { get; set; }

		public ContrastFilter ()
        {
            SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
			Exp = 1;
			Linear = 0;
			Threshold = 127;
			Variant = Variants.exponential_Channel_Independency;
		}
	    
        #region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
			Variant = (Variants)filterProperties [0];
			Threshold = (byte)filterProperties [3];
			Exp = (float)filterProperties [4];
			Linear = (byte)filterProperties [5];
		}

        protected internal override unsafe void Process(
            BitmapData srcData, BitmapData dstData)
        {
			ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
			int w = srcData.Width;
			int h = srcData.Height;
			int offset = srcData.Stride - w * ps;

			byte* src = (byte*)srcData.Scan0.ToPointer();
            byte* dst = (byte*)dstData.Scan0.ToPointer();

            // for each line
            for (int y = 0; y < h; y++)
            {
                // for each pixel
                for (int x = 0; x < w; x++, src += ps, dst += ps)
                {
					switch (Variant) 
					{
					case Variants.exponential_Channel_Independency:
						Process_Exponential_Channel_Independency (src, dst);
						break;
					default:
						Process_Linear_Channel_Independency (src, dst);
						break;

					}
					// alpha, 32 bit
					if (ps == 4) {
						dst[RGBA.A] = Use255ForAlpha ? (byte)255 : src[RGBA.A];
					}
                }
                src += offset;
                dst += offset;
            }
        }

        #endregion protected methods

		private unsafe void Process_Exponential_Channel_Independency (byte* src, byte* dst)
		{
			// 8 bit grayscale
			if (ps == 1) {
				float tmp_exp = src [RGBA.B] >= Threshold ? Exp : 2 - Exp;
				double tmp = Math.Pow (src [RGBA.B], tmp_exp) + 0.5f;
				*dst = (byte)Math.Min (tmp, 255);
			}

			// rgb, 24 and 32 bit
			if (ps >= 3) {
				// red
				float tmp_exp = src [RGBA.R] >= Threshold ? Exp : 2 - Exp;
				double tmp = Math.Pow (src [RGBA.R], tmp_exp) + 0.5f;
				dst [RGBA.R] = (byte)Math.Min (tmp, 255);

				// green
				tmp_exp = src [RGBA.G] >= Threshold ? Exp : 2 - Exp;
				tmp = Math.Pow (src [RGBA.G], tmp_exp) + 0.5f;
				dst [RGBA.G] = (byte)Math.Min (tmp, 255);

				// blue
				tmp_exp = src [RGBA.B] >= Threshold ? Exp : 2 - Exp;
				tmp = Math.Pow (src [RGBA.B], tmp_exp) + 0.5f;
				dst [RGBA.B] = (byte)Math.Min (tmp, 255);
			}
		}

		private unsafe void Process_Linear_Channel_Independency (byte* src, byte* dst)
		{
			// 8 bit grayscale
			if (ps == 1) {
				int tmp = src [RGBA.B] >= Threshold ? src [RGBA.B] + Linear : src [RGBA.B] - Linear;
				tmp = Math.Min (tmp, 255);
				tmp = Math.Max (tmp, 0);
				*dst = (byte)tmp;
			}

			// rgb, 24 and 32 bit
			if (ps >= 3) {
				// red
				int tmp = src [RGBA.R] >= Threshold ? src [RGBA.R] + Linear : src [RGBA.R] - Linear;
				tmp = Math.Min (tmp, 255);
				tmp = Math.Max (tmp, 0);
				dst [RGBA.R] = (byte)tmp;

				// green
				tmp = src [RGBA.G] >= Threshold ? src [RGBA.G] + Linear : src [RGBA.G] - Linear;
				tmp = Math.Min (tmp, 255);
				tmp = Math.Max (tmp, 0);
				dst [RGBA.G] = (byte)tmp;

				// blue
				tmp = src [RGBA.B] >= Threshold ? src [RGBA.B] + Linear : src [RGBA.B] - Linear;
				tmp = Math.Min (tmp, 255);
				tmp = Math.Max (tmp, 0);
				dst [RGBA.B] = (byte)tmp;
			}
		}
	}
}
