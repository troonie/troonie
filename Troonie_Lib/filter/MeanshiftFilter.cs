using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Troonie_Lib
{
    /// <summary>
	/// Binarization filter.
    /// </summary>
	public class MeanshiftFilter : AbstractFilter
    {		
		private int h;
		private int w;
		private int processedPixelsCounter;
//		private ProgressBarForm tmpForm;

		/// <summary>The color distance.</summary>
		public float ColorDistance { get; set; }

		/// <summary>The maximum iteration shifted value until filter converges.
		/// </summary>
		public int MaximumIteration { get; set; }

		/// <summary>The minimum iteration shifted value until filter converges.
		/// </summary>
		public float MinimumShifted { get; set; }

		/// <summary>The spatial radius.</summary>
		public int SpatialRadius { get; set; }

		public MeanshiftFilter()
        {
			SupportedSrcPixelFormat = PixelFormatFlags.Color;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;

			MaximumIteration = 8;
			MinimumShifted = 16;

			ColorDistance = 32;
			SpatialRadius = 8;
	    }

        #region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
//			ColorBinarization = filterProperties[0] == 1;
//			Threshold = (byte)filterProperties [3];
		}

        protected internal override unsafe void Process(
            BitmapData srcData, BitmapData dstData)
        {
			int ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
			processedPixelsCounter = 0;
//			const int ps = 3;
			w = srcData.Width;
			h = srcData.Height;
			int stride = srcData.Stride;
			int offset = stride - w * ps;

			float squaredSpatialRadius = SpatialRadius * SpatialRadius;
			float squaredColorDistance = ColorDistance * ColorDistance;

			byte* src = (byte*)srcData.Scan0.ToPointer();
			byte* dst = (byte*)dstData.Scan0.ToPointer();

//			tmpForm = new ProgressBarForm
//			{
//				ProgressBar = { Maximum = h },
//				StartPosition = FormStartPosition.CenterScreen
//			};
//			tmpForm.Show();

			// for each line
			for (int yy = 0; yy < h; yy++)
			{
//				RefreshFormProgressBar();

				// for each pixel
				for (int xx = 0; xx < w; xx++, src += ps, dst += ps)
				{
					Float3 currentColor = 
						new Float3(src[RGBA.R], src[RGBA.G], src[RGBA.B]);

					Float2 position = new Float2(xx, yy);
					Float3 yiq = GetYIQFromRGB(currentColor);
					float shifted;
					int iteration = 0;

					do
					{
						Float2 lastPosition = position;
						Float3 lastYIQ = yiq;

						Float2 summedPosition = new Float2(0.0f, 0.0f);
						Float3 summedYIQ = new Float3(0.0f, 0.0f, 0.0f);

						int count = 0;

						// for each kernel row
						int v;
						int u;
						for (v = -SpatialRadius; v <= SpatialRadius; v++)
						{
							int t = yy + v;

							// skip row
							if (t < 0)
								continue;
							// break
							if (t >= h)
								break;

							// for each kernel column
							for (u = -SpatialRadius; u <= SpatialRadius; u++)
							{
								t = xx + u;

								// skip column
								if (t < 0)
									continue;

								if (t >= w) 
									continue;

								// DO PROCESSING JOB
								if (v*v + u*u <= squaredSpatialRadius)
								{
									Float3 nextColor =
										new Float3(src[v*stride + u*ps + RGBA.R],
											src[v*stride + u*ps + RGBA.G],
											src[v*stride + u*ps + RGBA.B]);


									Float3 currentYIQ = GetYIQFromRGB(nextColor);

									float e = GetSquaredEuclideanDistance(yiq, currentYIQ);

									if (e <= squaredColorDistance)
									{
										summedPosition.X += u;
										summedPosition.Y += v;

										summedYIQ += currentYIQ;

										count++;
									}
								}
							}
						}

						float invertCount = 1.0f / count;

						yiq = summedYIQ * invertCount;

						position = summedPosition * invertCount + 0.5f;

						shifted =
							GetSquaredEuclideanDistance(position, lastPosition) +
							GetSquaredEuclideanDistance(yiq, lastYIQ);

						iteration++;
					}
					while (shifted > MinimumShifted && iteration < MaximumIteration);

					Float3 rgb = GetRGBFromYIQ(yiq);
					rgb += 0.5f; // for rounding in next step
					dst[RGBA.R] = (byte)rgb.X;
					dst[RGBA.G] = (byte)rgb.Y;
					dst[RGBA.B] = (byte)rgb.Z;

					processedPixelsCounter++;
				}
				src += offset;
				dst += offset;
			}

//			tmpForm.Dispose();
        }

        #endregion protected methods

		#region private methods

		private static Float3 GetRGBFromYIQ(Float3 yiq)
		{
			Float3 rgb = new Float3(0, 0, 0)
			{
				X = yiq.X + 0.9563f*yiq.Y + 0.6210f*yiq.Z,
				Y = yiq.X - 0.2721f*yiq.Y - 0.6473f*yiq.Z,
				Z = yiq.X - 1.1070f*yiq.Y + 1.7046f*yiq.Z
			};

			return rgb;
		}

		private static float GetSquaredEuclideanDistance(Float3 a, Float3 b)
		{
			float x = a.X - b.X;
			float y = a.Y - b.Y;
			float z = a.Z - b.Z;

			float euclideanDistance = x * x + y * y + z * z;

			return euclideanDistance;
		}

		private static float GetSquaredEuclideanDistance(Float2 a, Float2 b)
		{
			float x = a.X - b.X;
			float y = a.Y - b.Y;

			float euclideanDistance = x * x + y * y;

			return euclideanDistance;
		}

		private static Float3 GetYIQFromRGB(Float3 rgb)
		{
			Float3 yiq = new Float3(0, 0, 0)
			{
				X = 0.2990f*rgb.X + 0.5870f*rgb.Y + 0.1140f*rgb.Z,
				Y = 0.5957f*rgb.X - 0.2744f*rgb.Y - 0.3212f*rgb.Z,
				Z = 0.2114f*rgb.X - 0.5226f*rgb.Y + 0.3111f*rgb.Z
			};

			return yiq;
			// return rgb;            
		}

//		private void RefreshFormProgressBar()
//		{
//			if (tmpForm != null)
//			{
//				tmpForm.Text = Program.L.Text[116] +
//					(int)((float)processedPixelsCounter / (w * h) * 100) + @"%";
//				tmpForm.Resultlabel.Text =
//					Program.L.Text[117] + (w*h - processedPixelsCounter); // +
//				// @";   " + Program.L.Text[118] + NumberOfSegments;
//				tmpForm.ProgressBar.PerformStep();
//				tmpForm.Refresh();
//			}
//		}

		#endregion private methods
    }
}
