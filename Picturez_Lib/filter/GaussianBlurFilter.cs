using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Picturez_Lib
{
	/// <summary>
	/// Gaussian blur filter with a <see cref="Size"/>x<see cref="Size"/> 
	/// filter kernel.
	/// </summary>
	/// <remarks>
	/// <para>Filter implements the Gaussian operator with flexibel kernel 
	/// values of a <see cref="Size"/>x<see cref="Size"/> kernel matrix.
	/// </para>
	/// <para>The Gaussian filter calculates each pixel
	/// of the result image as weighted sum of the correspond pixel and 
	/// its neighbors in the source image. The weighted sum is divided by 
	/// the divisor before putting it into result image.
	/// </para>
	/// </remarks>    
	public class GaussianBlurFilter : AbstractFilter
	{
		/// <summary>
		/// Gaussian sigma value, [0.1, 7.0].
		/// </summary>
		/// 
		/// <remarks><para>Sigma value for Gaussian function used to calculate
		/// the kernel.</para>
		/// 
		/// <para>Default value is set to <b>1.4</b>.</para>
		/// </remarks>
		/// 
		public double Sigma { get; set; }

		/// <summary>
		/// Kernel size, [3, 11].
		/// </summary>
		/// 
		/// <remarks><para>Size of Gaussian kernel.</para>
		/// 
		/// <para>Default value is set to <b>5</b>.</para>
		/// </remarks>
		/// 
		public int Size { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GaussianBlurFilter"/> class.
		/// </summary>
		public GaussianBlurFilter()
		{
			SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;
			Sigma = 1.4;
			Size = 7;
		}

		#region protected methods

		/// <summary>
		/// Processes the filter on the passed <paramref name="srcData"/>
		/// resulting into <paramref name="dstData"/>.
		/// </summary>
		/// <param name="srcData">The source bitmap data.</param>
		/// <param name="dstData">The destination bitmap data.</param>
		protected internal override unsafe void Process(BitmapData srcData, BitmapData dstData)
		{
			 int ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
//			const int ps = 3;
			int w = srcData.Width;
			int h = srcData.Height;
			int stride = srcData.Stride;
			int offset = stride - w * ps;

			byte* src = (byte*)srcData.Scan0.ToPointer();
			byte* dst = (byte*)dstData.Scan0.ToPointer();

			//radius
			int r = Size >> 1; // Size / 2;
			double minimum = Function2D(-r, -r, Sigma);

			// for each line
			for (int y = 0; y < h; y++)
			{
				// for each pixel
				for (int x = 0; x < w; x++, src += ps, dst += ps)
				{
					int divisor = 0;
//					Int3 value = new Int3();
					int valueR = 0, valueG = 0, valueB = 0, valueA = 0;
					int v;
					int u;

					// for each kernel row
					for (v = -r; v <= r; v++)
					{
						int t = y + v;

						// skip row
						if (t < 0)
							continue;
						// break
						if (t >= h)
							break;

						// for each kernel column
						for (u = -r; u <= r; u++)
						{
							t = x + u;

							// skip column
							if (t < 0)
								continue;

							if (t >= w)
								continue;

							// DO PROCESSING JOB
							double tmp = Function2D(u, v, Sigma);
							tmp = tmp / minimum;
							int inttmp = (int)tmp;

							// 8 bit grayscale
							int btmp = src [v * stride + u * ps + RGBA.B];
							valueB += btmp * inttmp;

							// rgb, 24 and 32 bit
							if (ps >= 3) {
								int gtmp = src [v * stride + u * ps + RGBA.G];
								int rtmp = src [v * stride + u * ps + RGBA.R];
								valueG += gtmp * inttmp;
								valueR += rtmp * inttmp;
							}

							// alpha, 32 bit
							if (ps == 4) {
								int atmp = src [v * stride + u * ps + RGBA.A];
								valueA += atmp * inttmp;
							}

							divisor += inttmp;
						}
					}

					// Note: Avoids zreo-dividing, eventhrough it should not happened...
					if (divisor == 0)
						divisor = 1;

					valueR /= divisor;
					valueG /= divisor;
					valueB /= divisor;
					valueA /= divisor;

					// 8 bit grayscale
					dst[RGBA.B] = (byte)(valueB);

					// rgb, 24 and 32 bit
					if (ps >= 3) {
						dst [RGBA.G] = (byte)(valueG);
						dst [RGBA.R] = (byte)(valueR);
					}

					// alpha, 32 bit
					if (ps == 4) {
						dst [RGBA.A] = (byte)(Use255ForAlpha ? 255 : valueR);
					}
				}
				src += offset;
				dst += offset;
			}
		}

		#endregion protected methods

		private double Function2D(float x, float y, double sigma)
		{
			// float PI = 3.14159265f;
			double sqrSigma = sigma * sigma; //pow(sigma, 2);

			double tmp = (x * x + y * y) / (-2 * sqrSigma);
			double z = Math.Exp(tmp);
			double n = 2 * Math.PI * sqrSigma;
			double v = z / n;
			//exp( ( x * x + y * y ) / ( -2 * sqrSigma ) ) / ( 2 * PI * sqrSigma );
			return v;
		}
	}
}

