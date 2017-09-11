namespace Troonie_Lib
{
    using System;
    using System.Drawing.Imaging;

    /// <summary> Sobel edge detector. </summary>
    /// <remarks><para>The filter searches for objects' edges by applying 
    /// Sobel operator.</para>
    /// 
    /// <para>Each pixel of the result image is calculated as approximated 
    /// absolute gradient magnitude for corresponding pixel of the source image:
    /// <code lang="none">
    /// |G| = |Gx| + |Gy] ,
    /// </code>
    /// where Gx and Gy are calculate utilizing Sobel convolution kernels:
    /// <code lang="none">
    ///    Gx         Gy
    /// -1 0 +1    +1 +2 +1
    /// -2 0 +2     0  0  0
    /// -1 0 +1    -1 -2 -1
    /// </code>
    /// Using the above kernel the approximated magnitude for pixel <b>x</b> is calculate using
    /// the next equation:
    /// <code lang="none">
    /// P1 P2 P3
    /// P8  x P4
    /// P7 P6 P5
    /// 
    /// |G| = |P1 + 2P2 + P3 - P7 - 2P6 - P5| +
    ///       |P3 + 2P4 + P5 - P1 - 2P8 - P7|
    /// </code>
    /// </para>
    /// </remarks>
    public class SobelEdgeDetectorFilter : AbstractFilter
    {
        /// <summary>
        /// Indicating whether edge image is drawn as black-white image. 
        /// Default: false.
        /// Note: If true, a correct <see cref="Threshold"/> value is needed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if edge image will be  drawn as black-white image.
        /// </value>
        public bool BlackWhite { get; set; }

        /// <summary>
        /// The threshold value. Default: 0 (no threshold).
        /// </summary>
        public byte Threshold { get; set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="CannyEdgeDetector"/> class.
        /// </summary>
        public SobelEdgeDetectorFilter()
        {
			SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.Format8BppIndexed;           
        }

        #region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
			BlackWhite = filterProperties [0] == 1 ? true : false;
			Threshold = (byte)filterProperties [3];
		}

        /// <summary>
        /// Processes the filter on the passed <paramref name="srcData"/>
        /// resulting into <paramref name="dstData"/>.
        /// </summary>
        /// <param name="srcData">The source bitmap data.</param>
        /// <param name="dstData">The destination bitmap data.</param>
		protected internal override unsafe void Process(
			BitmapData srcData, BitmapData dstData)
        {
            int srcPS = System.Drawing.Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
			float maxChannel;
            int w = srcData.Width;
            int h = srcData.Height;
            int srcStride = srcData.Stride;
            int dstStride = dstData.Stride;
            int srcOffset = srcStride - w * srcPS;
            int dstOffset = dstStride - w;

            byte* src = (byte*)srcData.Scan0.ToPointer();
            byte* dst = (byte*)dstData.Scan0.ToPointer();
            // align pointer
            src += srcStride + srcPS;
            dst += dstStride + 1;

            // for each line
            for (int y = 1; y < h - 1; y++)
            {
                // for each pixel
                for (int x = 1; x < w - 1; x++, src += srcPS, dst += 1)
                {
                    //  SobelX       SobelY         Neighbour Pixel
                    //  1  0 -1      1  2  1        p20 p21 p22
                    //  2  0 -2      0  0  0        p10  x  p12
                    //  1  0 -1     -1 -2 -1        p00 p01 p02

					if (srcPS == 1 /*8 bpp*/) {
						int p20 = src [-srcStride - srcPS];
						int p21 = src [-srcStride];
						int p22 = src [-srcStride + srcPS];
						int p10 = src [-srcPS];
						int p12 = src [+srcPS];
						int p00 = src [srcStride - srcPS];
						int p01 = src [srcStride];
						int p02 = src [srcStride + srcPS];

						int sobelX = p00 + 2 * p10 + p20 - p02 - 2 * p12 - p22;
						int sobelY = p20 + 2 * p21 + p22 - p00 - 2 * p01 - p02;
						maxChannel = (float)Math.Sqrt (
							sobelX * sobelX + sobelY * sobelY);						

					} else {
						
						Int3 p20 = new Int3 (src [-srcStride - srcPS + RGBA.R], 
							                         src [-srcStride - srcPS + RGBA.G], 
							                         src [-srcStride - srcPS + RGBA.B]);
						Int3 p21 = new Int3 (src [-srcStride + RGBA.R], 
							                         src [-srcStride + RGBA.G], 
							                         src [-srcStride + RGBA.B]);
						Int3 p22 = new Int3 (src [-srcStride + srcPS + RGBA.R], 
							                         src [-srcStride + srcPS + RGBA.G], 
							                         src [-srcStride + srcPS + RGBA.B]);
						Int3 p10 = new Int3 (src [-srcPS + RGBA.R], 
							                         src [-srcPS + RGBA.G], 
							                         src [-srcPS + RGBA.B]);
						Int3 p12 = new Int3 (src [+srcPS + RGBA.R], 
							                         src [+srcPS + RGBA.G], 
							                         src [+srcPS + RGBA.B]);
						Int3 p00 = new Int3 (src [srcStride - srcPS + RGBA.R], 
							                         src [srcStride - srcPS + RGBA.G], 
							                         src [srcStride - srcPS + RGBA.B]);
						Int3 p01 = new Int3 (src [srcStride + RGBA.R], 
							                         src [srcStride + RGBA.G], 
							                         src [srcStride + RGBA.B]);
						Int3 p02 = new Int3 (src [srcStride + srcPS + RGBA.R], 
							                         src [srcStride + srcPS + RGBA.G], 
							                         src [srcStride + srcPS + RGBA.B]);

						Int3 sobelX = p00 + 2 * p10 + p20 - p02 - 2 * p12 - p22;
						Int3 sobelY = p20 + 2 * p21 + p22 - p00 - 2 * p01 - p02;
						Float3 edgeSqr = Float3.Sqrt (sobelX * sobelX + sobelY * sobelY);
						maxChannel = Math.Max (Math.Max (edgeSqr.X, edgeSqr.Y), edgeSqr.Z);
					}
                    *dst = (byte)(maxChannel + 0.5);
                    if (maxChannel < Threshold)
                    {
                        *dst = 0;
                    }
                    else if (BlackWhite)
                    {
                        *dst = 255;
                    }
                }
                src += 2 * srcPS + srcOffset;
                dst += 2 + dstOffset;
            }
        }

        #endregion protected methods             
    }
}
