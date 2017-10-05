using System;
using System.Drawing.Imaging;
using System.Drawing;

namespace Troonie_Lib
{
	/// <summary>
	/// Discrete convolution operator with a 5x5 filter <see cref="Kernel"/>.
	/// <remarks>
	/// <para>Convolution operators transforms an input image using a 
	/// mathematical function in an output image. Thereby, the mathematical 
	/// function in the discrete range is realized by a convolution mask, also 
	/// called filter <see cref="Kernel"/>. Typical convolution operations are 
	/// blurring, sharpening and edge detection.
	/// </para>
	/// </remarks> 
	/// </summary>
	public class Convolution5X5Filter : AbstractFilter
	{
		/// <summary>Represents the pre-defined convolution filter masks.</summary>
		public enum FilterMask
		{
			GaussianBlur,
			Laplace,
			Laplace2,
			LinearBlur
		}

		/// <summary>
		/// Pre-defined convolution filter mask, known as Gaussian blur 
		/// filter (with Sigma=1.4 and Size=5).
		/// </summary>
		public static int[] GaussianBlurKernel = new[]
		{  1, 2, 2, 2, 1,
			2, 4, 5, 4, 2, 
			2, 5, 7, 5, 2, 
			2, 4, 5, 4, 2, 
			1, 2, 2, 2, 1 };

		/// <summary>
		/// Pre-defined convolution filter mask, known as Laplace filter.
		/// </summary>
		public static int []LaplaceKernel = new []
		{ 0,  0,  0,  0, 0, 
			0, -1, -1, -1, 0, 
			0, -1,  8, -1, 0, 
			0, -1, -1, -1, 0, 
			0,  0,  0,  0, 0 };

		/// <summary>
		/// Pre-defined convolution filter mask, known as other version of 
		/// Laplace filter.
		/// </summary>
		public static int[] LaplaceKernel2 = new[]
		{ 0,  0,  0,  0, 0, 
			0, -1, -2, -1, 0, 
			0, -2, 12, -2, 0, 
			0, -1, -2, -1, 0, 
			0,  0,  0,  0, 0 };

		/// <summary>
		/// Pre-defined convolution filter mask, known as linear blur filter.
		/// </summary>
		public static int[] LinearBlurKernel = new[]
		{ 1, 1, 1, 1, 1, 
			1, 1, 1, 1, 1, 
			1, 1, 1, 1, 1, 
			1, 1, 1, 1, 1, 
			1, 1, 1, 1, 1 };

		private FilterMask mask;
		/// <summary>Pre-defined convolution filter mask. Default value is set to 
		/// <see cref="FilterMask.LaplaceKernel"/>.</summary>
		public FilterMask Mask 
		{ 
			get 
			{
				return mask;
			}
			set 
			{
				mask = value;
				switch (value) {
				case FilterMask.GaussianBlur:
					Kernel = GaussianBlurKernel;
					break;
				case FilterMask.Laplace:
					Kernel = LaplaceKernel;
					break;
				case FilterMask.Laplace2:
					Kernel = LaplaceKernel2;
					break;
				case FilterMask.LinearBlur:
					Kernel = LinearBlurKernel;
					break;
				}
			}
		}

		/// <summary>The convolution 5x5 mask as a 25-elements array 
		/// (filled row by row), also called filter kernel.</summary>
		public int[] Kernel { get; set; }

		public Convolution5X5Filter()
		{
			SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;

			Mask = FilterMask.Laplace;
			// Kernel = LaplaceKernel;
		}

		#region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
			Mask = (FilterMask)filterProperties[0];
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
			const int r = 2; //radius
			byte ir, ig, ib;
			// 8 bit grayscale
			ir = ig = ib = RGBA.B;
			// rgb, 24 and 32 bit
			if (ps >= 3) {
				ig = RGBA.G;
				ir = RGBA.R;
			}
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
					int divisor = 0;
					Int3 value = new Int3();                    
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
							int kernelIndex =  v * 5 + 10 + u + 2;
							int kernelValue = Kernel[kernelIndex];

							Int3 color = new Int3(
								src[v * stride + u * ps + ir],
								src[v * stride + u * ps + ig],
								src[v * stride + u * ps + ib]);
							value += color * kernelValue;
							divisor += kernelValue;
						}
					}
					value /= Math.Max(divisor, 1);

					// check max and min values
					value.X = Math.Max(0, Math.Min(value.X, 255));
					value.Y = Math.Max(0, Math.Min(value.Y, 255));
					value.Z = Math.Max(0, Math.Min(value.Z, 255));

					dst[ir] = (byte)(value.X);
					dst[ig] = (byte)(value.Y);
					dst[ib] = (byte)(value.Z);

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