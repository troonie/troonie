using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Troonie_Lib
{
	/// <summary>     
	/// The filter exponentiates every color channel by the specified 
	/// <see cref="Exponent"/>. The <see cref="Threshold"/> 
	/// property determines, if the exponentiate result will be used alone or 
	/// will be subtract from white color value. 
	/// The <see cref="InvertExponentiateChannels"/> property determines, if the 
	/// normal or the inverted filter will be used. </summary>  
	public class ExponentiateChannelsFilter : AbstractFilter
	{
		/// <summary>The exponent to use for exponentiating the color channels.
		/// </summary>
		/// <remarks>Default value: 1.10.</remarks>
		public double Exponent { get; set; }

		/// <summary>
		/// The threshold for determining, if a channel will be increased or 
		/// decreased.
		/// </summary>
		/// <remarks>Default value: 255 (no effect).</remarks>
		public byte Threshold { get; set; }

		public ExponentiateChannelsFilter()
		{
			SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;

			Threshold = 255;
			Exponent = 1.10;
		}

		#region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
			Exponent = filterProperties [3];
			Threshold = (byte)filterProperties [4];
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
//			Rectangle rectCompare = new Rectangle(0, 0, CompareBitmap.Width, CompareBitmap.Height);

			int w = srcData.Width;
			int h = srcData.Height;
			int stride = srcData.Stride;
			int offset = stride - w * ps;

			byte* src = (byte*)srcData.Scan0.ToPointer();
			byte* dst = (byte*)dstData.Scan0.ToPointer();

			// for each line
			for (int y = 0; y < h; y++)
			{
				// for each pixel
				for (int x = 0; x < w; x++, src += ps, dst += ps)
				{
					// 8 bit grayscale
					byte b = (byte)(Math.Min(Math.Pow(src[RGBA.B], Exponent), 255));
					dst[RGBA.B] = (byte)(src[RGBA.B] <= Threshold ? b : 255 - b);

					// rgb, 24 and 32 bit
					if (ps >= 3) {
						byte r = (byte)(Math.Min(Math.Pow(src[RGBA.R], Exponent), 255));
						byte g = (byte)(Math.Min(Math.Pow(src[RGBA.G], Exponent), 255));
						dst[RGBA.R] = (byte)(src[RGBA.R] <= Threshold ? r : 255 - r);
						dst[RGBA.G] = (byte)(src[RGBA.G] <= Threshold ? g : 255 - g);
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
	}
}