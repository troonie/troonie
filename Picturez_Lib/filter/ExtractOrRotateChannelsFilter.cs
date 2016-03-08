using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Picturez_Lib
{
	/// <summary> Extracts or rotates channels of an image. </summary> 
	public class ExtractOrRotateChannelsFilter : AbstractFilter
	{
		/// <summary>Represents the possible channel order of RGB pixel.</summary>
		public enum RGBOrder
		{
			/// <summary>Red channel.</summary>
			R,
			/// <summary>Green channel.</summary>
			G,
			/// <summary>Blue order.</summary>
			B,
			/// <summary>Alpha order.</summary>
			A,
			/// <summary>RGB channel order.</summary>
			RGB,
			/// <summary>RBG channel order.</summary>
			RBG,
			/// <summary>BRG channel order.</summary>
			BRG,
			/// <summary>BGR channel order.</summary>
			BGR,
			/// <summary>GBR channel order.</summary>
			GBR,
			/// <summary>GRB channel order.</summary>
			GRB
		}

		/// <summary>The rotation order of RGB channels.</summary>
		/// <remarks><para>Default value is set to 
		/// <see cref="RGBOrder.GBR"/>.</para></remarks>
		public RGBOrder Order { get; set; }

		/// <summary>
		/// Initializes a new instance of the 
		/// <see cref="ExtractOrRotateChannels"/> class.
		/// </summary>
		public ExtractOrRotateChannelsFilter()
		{
			SupportedSrcPixelFormat = PixelFormatFlags.Color;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
			Order = RGBOrder.GBR;
		}

		#region protected methods

		/// <summary>
		/// Processes the filter on the passed <paramref name="srcData"/>
		/// resulting into <paramref name="dstData"/>.
		/// </summary>
		/// <param name="srcData">The source bitmap data.</param>
		/// <param name="dstData">The destination bitmap data.</param>
		protected override unsafe void Process(BitmapData srcData, BitmapData dstData)
		{
			int ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
//			const int ps = 3;
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
					switch(Order)
					{
						case RGBOrder.R:
						dst[RGBA.R] = src[RGBA.R];
						dst[RGBA.G] = src[RGBA.R];
						dst[RGBA.B] = src[RGBA.R];
						if (ps == 4)
							dst[RGBA.A] = 255;
						break;
						case RGBOrder.G:
						dst[RGBA.R] = src[RGBA.G];
						dst[RGBA.G] = src[RGBA.G];
						dst[RGBA.B] = src[RGBA.G];
						if (ps == 4)
							dst[RGBA.A] = 255;
						break;
						case RGBOrder.B:
						dst[RGBA.R] = src[RGBA.B];
						dst[RGBA.G] = src[RGBA.B];
						dst[RGBA.B] = src[RGBA.B];
						if (ps == 4)
							dst[RGBA.A] = 255;
						break;
					case RGBOrder.A:
						dst[RGBA.R] = src[RGBA.A];
						dst[RGBA.G] = src[RGBA.A];
						dst[RGBA.B] = src[RGBA.A];
						if (ps == 4)
							dst[RGBA.A] = 255;
						break;
						case RGBOrder.RGB:
						dst[RGBA.R] = src[RGBA.R];
						dst[RGBA.G] = src[RGBA.G];
						dst[RGBA.B] = src[RGBA.B];
						if (ps == 4)
							dst[RGBA.A] = src[RGBA.A];
						break;
						case RGBOrder.RBG:
						dst[RGBA.R] = src[RGBA.R];
						dst[RGBA.G] = src[RGBA.B];
						dst[RGBA.B] = src[RGBA.G];
						if (ps == 4)
							dst[RGBA.A] = src[RGBA.A];
						break;
						case RGBOrder.BRG:
						dst[RGBA.R] = src[RGBA.B];
						dst[RGBA.G] = src[RGBA.R];
						dst[RGBA.B] = src[RGBA.G];
						if (ps == 4)
							dst[RGBA.A] = src[RGBA.A];
						break;
						case RGBOrder.BGR:
						dst[RGBA.R] = src[RGBA.B];
						dst[RGBA.G] = src[RGBA.G];
						dst[RGBA.B] = src[RGBA.R];
						if (ps == 4)
							dst[RGBA.A] = src[RGBA.A];
						break;
						case RGBOrder.GBR:
						dst[RGBA.R] = src[RGBA.G];
						dst[RGBA.G] = src[RGBA.B];
						dst[RGBA.B] = src[RGBA.R];
						if (ps == 4)
							dst[RGBA.A] = src[RGBA.A];
						break;
						case RGBOrder.GRB:
						dst[RGBA.R] = src[RGBA.G];
						dst[RGBA.G] = src[RGBA.R];
						dst[RGBA.B] = src[RGBA.B];
						if (ps == 4)
							dst[RGBA.A] = src[RGBA.A];
						break;
					}
				}
				src += offset;
				dst += offset;
			}
		}

		#endregion protected methods             
	}
}

