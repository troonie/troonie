using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Picturez_Lib
{
    /// <summary> Oil painting filter. </summary>
    public class OilPaintingFilter : AbstractFilter
    {
        /// <summary>
        /// Window size to search for most frequent pixels' intensity.
        /// </summary>
        public int BrushSize { get; set; }

        public OilPaintingFilter()
        {
            SupportedSrcPixelFormat = PixelFormatFlags.Format24BppRgb;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource; 
            BrushSize = 5;
        }

		protected override void SetProperties (double[] filterProperties)
		{
			BrushSize = (int)filterProperties[3];
		}

        /// <summary>
        /// Processes the filter on the passed <paramref name="srcData" />
        /// resulting into <paramref name="dstData" />.
        /// </summary>
        /// <param name="srcData">The source bitmap data.</param>
        /// <param name="dstData">The destination bitmap data.</param>
		protected override internal unsafe void Process(
            BitmapData srcData, BitmapData dstData)
        {
            // pixel size
            // int ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;
            const int ps = 3;
            int w = srcData.Width;
            int h = srcData.Height;
            int offset = srcData.Stride - srcData.Width * ps;

            // brush radius
            int radius = BrushSize >> 1;

            // intensity values
            int[] intensities = new int[256];

            byte* src = (byte*)srcData.Scan0.ToPointer();
            byte* dst = (byte*)dstData.Scan0.ToPointer();

            // RGB image
            int[] red = new int[256];
            int[] green = new int[256];
            int[] blue = new int[256];

            // for each line
            for (int y = 0; y < h; y++)
            {
                // for each pixel
                for (int x = 0; x < w; x++, src += ps, dst += ps)
                {
                    // clear arrays
                    Array.Clear(intensities, 0, 256);
                    Array.Clear(red, 0, 256);
                    Array.Clear(green, 0, 256);
                    Array.Clear(blue, 0, 256);

                    // for each kernel row
                    int v;
                    int u;
                    for (v = -radius; v <= radius; v++)
                    {
                        int t = y + v;

                        // skip row
                        if (t < 0)
                            continue;
                        // break
                        if (t >= h)
                            break;

                        // for each kernel column
                        for (u = -radius; u <= radius; u++)
                        {
                            t = x + u;

                            // skip column
                            if (t < 0)
                                continue;

                            if (t >= w) 
                                continue;

                            // DO PROCESSING JOB
                            byte* p = &src[v * srcData.Stride + u * ps];

                            // grayscale value using BT709
                            byte intensity = (byte)(0.2125 * p[RGBA.R] + 
                                                    0.7154 * p[RGBA.G] + 
                                                    0.0721 * p[RGBA.B]);

                            //
                            intensities[intensity]++;
                            // red
                            red[intensity] += p[RGBA.R];
                            // green
                            green[intensity] += p[RGBA.G];
                            // blue
                            blue[intensity] += p[RGBA.B];
                        }
                    }

                    // get most frequent intensity
                    byte maxIntensity = 0;
                    u = 0;

                    for (v = 0; v < 256; v++)
                    {
                        if (intensities[v] > u)
                        {
                            maxIntensity = (byte)v;
                            u = intensities[v];
                        }
                    }

                    // set destination pixel
                    dst[RGBA.R] = (byte)(
                        red[maxIntensity] / intensities[maxIntensity]);
                    dst[RGBA.G] = (byte)(
                        green[maxIntensity] / intensities[maxIntensity]);
                    dst[RGBA.B] = (byte)(
                        blue[maxIntensity] / intensities[maxIntensity]);
                }
                src += offset;
                dst += offset;
            }
        }
    }
}
