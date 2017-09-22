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
	public class SobelEdgeMarkerFilter : AbstractFilter
    {
        /// <summary>
        /// Indicating whether edges are sketched as white instead black pixel. 
        /// Default: false.       
        /// </summary>
        public bool UseWhiteEdgeColor { get; set; }

        /// <summary>
        /// The threshold value. Default: 127.
        /// </summary>
        public byte Threshold { get; set; }



        /// <summary>
        /// Initializes a new instance of the <see cref="CannyEdgeDetector"/> class.
        /// </summary>
		public SobelEdgeMarkerFilter()
        {
			SupportedSrcPixelFormat = PixelFormatFlags.All;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;    

			Threshold = 127;
        }

        #region protected methods

		protected override void SetProperties (double[] filterProperties)
		{
			UseWhiteEdgeColor = filterProperties [0] == 1 ? true : false;
			Threshold = (byte)filterProperties [3];
		}
			
		protected internal override unsafe void Process(
			BitmapData srcData, BitmapData dstData)
        {
			CopyFilter cf = new CopyFilter ();
			cf.Process (srcData, dstData);

			int ps = System.Drawing.Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
			float maxChannel;
            int w = srcData.Width;
            int h = srcData.Height;
			int stride = srcData.Stride;
			int offset = stride - w * ps;

            byte* src = (byte*)srcData.Scan0.ToPointer();
            byte* dst = (byte*)dstData.Scan0.ToPointer();
            // align pointer
            src += stride + ps;
			dst += stride + ps;

            // for each line
            for (int y = 1; y < h - 1; y++)
            {
                // for each pixel
				for (int x = 1; x < w - 1; x++, src += ps, dst += ps)
                {
                    //  SobelX       SobelY         Neighbour Pixel
                    //  1  0 -1      1  2  1        p20 p21 p22
                    //  2  0 -2      0  0  0        p10  x  p12
                    //  1  0 -1     -1 -2 -1        p00 p01 p02

					if (ps == 1 /*8 bpp*/) {
						int p20 = src [-stride - ps];
						int p21 = src [-stride];
						int p22 = src [-stride + ps];
						int p10 = src [-ps];
						int p12 = src [+ps];
						int p00 = src [stride - ps];
						int p01 = src [stride];
						int p02 = src [stride + ps];

						int sobelX = p00 + 2 * p10 + p20 - p02 - 2 * p12 - p22;
						int sobelY = p20 + 2 * p21 + p22 - p00 - 2 * p01 - p02;
						maxChannel = (float)Math.Sqrt (
							sobelX * sobelX + sobelY * sobelY);	

//						*dst = (byte)(maxChannel + 0.5);
						if (maxChannel >= Threshold) {
							*dst = (byte)(UseWhiteEdgeColor ? 255 : 0);
						}
					} else {
						
						Int3 p20 = new Int3 (src [-stride - ps + RGBA.R], 
							                         src [-stride - ps + RGBA.G], 
							                         src [-stride - ps + RGBA.B]);
						Int3 p21 = new Int3 (src [-stride + RGBA.R], 
							                         src [-stride + RGBA.G], 
							                         src [-stride + RGBA.B]);
						Int3 p22 = new Int3 (src [-stride + ps + RGBA.R], 
							                         src [-stride + ps + RGBA.G], 
							                         src [-stride + ps + RGBA.B]);
						Int3 p10 = new Int3 (src [-ps + RGBA.R], 
							                         src [-ps + RGBA.G], 
							                         src [-ps + RGBA.B]);
						Int3 p12 = new Int3 (src [+ps + RGBA.R], 
							                         src [+ps + RGBA.G], 
							                         src [+ps + RGBA.B]);
						Int3 p00 = new Int3 (src [stride - ps + RGBA.R], 
							                         src [stride - ps + RGBA.G], 
							                         src [stride - ps + RGBA.B]);
						Int3 p01 = new Int3 (src [stride + RGBA.R], 
							                         src [stride + RGBA.G], 
							                         src [stride + RGBA.B]);
						Int3 p02 = new Int3 (src [stride + ps + RGBA.R], 
							                         src [stride + ps + RGBA.G], 
							                         src [stride + ps + RGBA.B]);

						Int3 sobelX = p00 + 2 * p10 + p20 - p02 - 2 * p12 - p22;
						Int3 sobelY = p20 + 2 * p21 + p22 - p00 - 2 * p01 - p02;
						Float3 edgeSqr = Float3.Sqrt (sobelX * sobelX + sobelY * sobelY);
						maxChannel = Math.Max (Math.Max (edgeSqr.X, edgeSqr.Y), edgeSqr.Z);

						if (maxChannel >= Threshold)
						{
							dst [RGBA.R] = dst [RGBA.G] = dst [RGBA.B] = (byte)(UseWhiteEdgeColor ? 255 : 0);
						}
					}
                }
                src += 2 * ps + offset;
				dst += 2 * ps + offset;
            }
        }

        #endregion protected methods             
    }
}
