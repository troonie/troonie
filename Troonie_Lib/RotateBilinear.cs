// Source code fragments of image filter from AForge.NET framework
// http://code.google.com/p/aforge/
// Copyright © Andrew Kirillov, 2005-2008
// andrew.kirillov@gmail.com
//
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Troonie_Lib
{
    /// <summary>
    /// Rotate image using bilinear interpolation.
    /// </summary>
    /// 
    /// <para><note>Rotation is performed in counterclockwise direction.</note></para>
    /// 
    /// <remarks><para>The class implements image rotation filter using bilinear
    /// interpolation algorithm.</para>
    /// 
    /// <para>The filter accepts 8 bpp grayscale images and 24 bpp
    /// color images for processing.</para>
    /// </remarks>
    public class RotateBilinear
    {
//		private static RotateBilinear instance;
//		public static RotateBilinear I
//		{
//			get
//			{
//				if (instance == null) {
//					instance = new RotateBilinear ();
//				}
//				return instance;
//			}
//		}

		public bool Use255ForAlpha { get; set; }

        /// <summary>
        /// Rotation angle.
        /// </summary>
        private double angle;

        /// <summary>
        /// Rotation angle, [0, 360].
        /// </summary>
        public double Angle
        {
            get { return angle; }
            set { angle = value % 360; }
        }

        /// <summary>
        /// Keep image size or not.
        /// </summary>
        /// 
        /// <remarks><para>The property determines if source image's size will be kept
        /// as it is or not. If the value is set to <b>false</b>, then the new image will have
        /// new dimension according to rotation angle. If the value is set to
        /// <b>true</b>, then the new image will have the same size, which means that some parts
        /// of the image may be clipped because of rotation.</para>
        /// </remarks>
        /// 
        public bool KeepSize { get; set; }

        /// <summary>
        /// Fill color.
        /// </summary>
        /// 
        /// <remarks><para>The fill color is used to fill areas of destination image,
        /// which don't have corresponsing pixels in source image.</para></remarks>
        /// 
        public Color FillColor { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RotateBilinear"/> class.
        /// </summary>
        /// 
        /// <param name="angle">Rotation angle.</param>
		/// <param name="keepSize">Keep image size or not. If the value is set to <b>false</b>, 
		/// then the new image will have
		/// new dimension according to rotation angle. If the value is set to
		/// <b>true</b>, then the new image will have the same size, which means that some parts
		/// of the image may be clipped because of rotation.</param>
        /// 
        public RotateBilinear( double angle, bool keepSize )
        {
            FillColor = Color.FromArgb(0, 0, 0);
            this.angle = angle;
            KeepSize = keepSize;
        }

        /// <summary>
        /// Applies the filter on the passed <paramref name="source"/> bitmap.
        /// </summary>
        /// <param name="source">The source image to process.</param>
        /// <returns>The filter result as a new bitmap.</returns>
        public Bitmap Apply(Bitmap source)
        {
            // get new image size
			Size newSize = CalculateNewImageSize(angle, source.Width, source.Height, KeepSize);

            Bitmap destination = new Bitmap(
                newSize.Width, newSize.Height, source.PixelFormat);
            Rectangle srcRect = new Rectangle(0, 0, source.Width, source.Height);
            Rectangle dstRect = new Rectangle(0, 0, newSize.Width, newSize.Height);
            BitmapData srcData = source.LockBits(
                srcRect, ImageLockMode.ReadWrite, source.PixelFormat);
            BitmapData dstData = destination.LockBits(
                dstRect, ImageLockMode.ReadWrite, destination.PixelFormat);
            Process(srcData, dstData);
            destination.UnlockBits(dstData);
            source.UnlockBits(srcData);

			if (destination.PixelFormat == PixelFormat.Format8bppIndexed)
			{
				ColorPalette.I.SetColorPaletteToGray (destination);
			}

            return destination;
        }

        /// <summary>
        /// Calculates new image size.
        /// </summary>
        /// 
        /// <param name="source">Source image data.</param>
        /// 
        /// <returns>New image size - size of the destination image.</returns>
        /// 
		public static Size CalculateNewImageSize(double angle, int originalWidth, int originalHeight, bool keepSize)
        {
            // return same size if original image size should be kept
			if (keepSize)
				return new Size(originalWidth, originalHeight);

            // angle's sine and cosine
            double angleRad = -angle * Math.PI / 180;
            double angleCos = Math.Cos(angleRad);
            double angleSin = Math.Sin(angleRad);

            // calculate half size
			double halfWidth = (double)originalWidth / 2;
			double halfHeight = (double)originalHeight / 2;

            // rotate corners
            double cx1 = halfWidth * angleCos;
            double cy1 = halfWidth * angleSin;

            double cx2 = halfWidth * angleCos - halfHeight * angleSin;
            double cy2 = halfWidth * angleSin + halfHeight * angleCos;

            double cx3 = -halfHeight * angleSin;
            double cy3 = halfHeight * angleCos;

            // recalculate image size
            halfWidth = Math.Max(Math.Max(cx1, cx2), Math.Max(cx3, 0)) - 
                        Math.Min(Math.Min(cx1, cx2), Math.Min(cx3, 0));

            halfHeight = Math.Max(Math.Max(cy1, cy2), Math.Max(cy3, 0)) - 
                         Math.Min(Math.Min(cy1, cy2), Math.Min(cy3, 0));

            return new Size(
                (int)(halfWidth * 2 + 0.5), (int)(halfHeight * 2 + 0.5));
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        ///
        private unsafe void Process(
            BitmapData sourceData, BitmapData destinationData)
        {
            // get source image size
            int     width       = sourceData.Width;
            int     height      = sourceData.Height;
            double  halfWidth   = (double) width / 2;
            double  halfHeight  = (double) height / 2;

            // get destination image size
            int     newWidth    = destinationData.Width;
            int     newHeight   = destinationData.Height;
            double  halfNewWidth    = (double) newWidth / 2;
            double  halfNewHeight   = (double) newHeight / 2;

            // angle's sine and cosine
            double angleRad = -angle * Math.PI / 180;
			double angleCos = Math.Cos( angleRad );
			double angleSin = Math.Sin( angleRad );

            int srcStride = sourceData.Stride;
//            int dstOffset = destinationData.Stride -
//                ((destinationData.PixelFormat == PixelFormat.Format8bppIndexed) 
//                  ? newWidth : newWidth * 3 );

			int dstPs = Image.GetPixelFormatSize(destinationData.PixelFormat) / 8;
			int dstOffset = destinationData.Stride - newWidth * dstPs;

            // fill values
            byte fillR = FillColor.R;
            byte fillG = FillColor.G;
            byte fillB = FillColor.B;

            // do the job
            byte* src = (byte*) sourceData.Scan0.ToPointer( );
            byte* dst = (byte*) destinationData.Scan0.ToPointer();

            // destination pixel's coordinate relative to image center
            double cx, cy;
            // coordinates of source points
            double  ox, oy, tx, ty, dx1, dy1, dx2, dy2;
            int     ox1, oy1, ox2, oy2;
            // width and height decreased by 1
            int ymax = height - 1;
            int xmax = width - 1;
            // temporary pointers
            byte* p1, p2;

            // check pixel format
            if ( destinationData.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // grayscale
                cy = -halfNewHeight;
                for ( int y = 0; y < newHeight; y++ )
                {
                    // do some pre-calculations of source points' coordinates
                    // (calculate the part which depends on y-loop, but does not
                    // depend on x-loop)
                    tx = angleSin * cy + halfWidth;
                    ty = angleCos * cy + halfHeight;

                    cx = -halfNewWidth;
                    for ( int x = 0; x < newWidth; x++, dst++ )
                    {
                        // coordinates of source point
                        ox = tx + angleCos * cx;
                        oy = ty - angleSin * cx;

                        // top-left coordinate
                        ox1 = (int) ox;
                        oy1 = (int) oy;

                        // validate source pixel's coordinates
                        if ( ( ox1 < 0 ) || ( oy1 < 0 ) || ( ox1 >= width ) || ( oy1 >= height ) )
                        {
                            // fill destination image with filler
                            *dst = fillG;
                        }
                        else
                        {
                            // bottom-right coordinate
                            ox2 = ( ox1 == xmax ) ? ox1 : ox1 + 1;
                            oy2 = ( oy1 == ymax ) ? oy1 : oy1 + 1;

                            if ( ( dx1 = ox - ox1 ) < 0 )
                                dx1 = 0;
                            dx2 = 1.0 - dx1;

                            if ( ( dy1 = oy - oy1 ) < 0 )
                                dy1 = 0;
                            dy2 = 1.0 - dy1;

                            p1 = src + oy1 * srcStride;
                            p2 = src + oy2 * srcStride;

                            // interpolate using 4 points
                            *dst = (byte) (
                                dy2 * ( dx2 * p1[ox1] + dx1 * p1[ox2] ) +
                                dy1 * ( dx2 * p2[ox1] + dx1 * p2[ox2] ) );
                        }
                        cx++;
                    }
                    cy++;
                    dst += dstOffset;
                }
            }
            else
            {
                // RGB & ARGB
                cy = -halfNewHeight;
                for ( int y = 0; y < newHeight; y++ )
                {
                    // do some pre-calculations of source points' coordinates
                    // (calculate the part which depends on y-loop, but does not
                    // depend on x-loop)
                    tx = angleSin * cy + halfWidth;
                    ty = angleCos * cy + halfHeight;

                    cx = -halfNewWidth;
					for ( int x = 0; x < newWidth; x++, dst += dstPs )
                    {
                        // coordinates of source point
                        ox = tx + angleCos * cx;
                        oy = ty - angleSin * cx;

                        // top-left coordinate
                        ox1 = (int) ox;
                        oy1 = (int) oy;

                        // validate source pixel's coordinates
                        if ( ( ox1 < 0 ) || ( oy1 < 0 ) || ( ox1 >= width ) || ( oy1 >= height ) )
                        {
                            // fill destination image with filler
                            dst[RGBA.R] = fillR;
                            dst[RGBA.G] = fillG;
                            dst[RGBA.B] = fillB;
							if (dstPs == 4) {
								dst[RGBA.A] = 255;
							}
                        }
                        else
                        {
                            // bottom-right coordinate
                            ox2 = ( ox1 == xmax ) ? ox1 : ox1 + 1;
                            oy2 = ( oy1 == ymax ) ? oy1 : oy1 + 1;

                            if ( ( dx1 = ox - ox1 ) < 0 )
                                dx1 = 0;
                            dx2 = 1.0f - dx1;

                            if ( ( dy1 = oy - oy1 ) < 0 )
                                dy1 = 0;
                            dy2 = 1.0f - dy1;

                            // get four points
                            p1 = p2 = src + oy1 * srcStride;
							p1 += ox1 * dstPs;
							p2 += ox2 * dstPs;

                            byte* p4;
                            byte* p3 = p4 = src + oy2 * srcStride;
							p3 += ox1 * dstPs;
							p4 += ox2 * dstPs;

                            // interpolate using 4 points

                            // red
                            dst[RGBA.R] = (byte) (
                                dy2 * ( dx2 * p1[RGBA.R] + dx1 * p2[RGBA.R] ) +
                                dy1 * ( dx2 * p3[RGBA.R] + dx1 * p4[RGBA.R] ) );

                            // green
                            dst[RGBA.G] = (byte) (
                                dy2 * ( dx2 * p1[RGBA.G] + dx1 * p2[RGBA.G] ) +
                                dy1 * ( dx2 * p3[RGBA.G] + dx1 * p4[RGBA.G] ) );

                            // blue
                            dst[RGBA.B] = (byte) (
                                dy2 * ( dx2 * p1[RGBA.B] + dx1 * p2[RGBA.B] ) +
                                dy1 * ( dx2 * p3[RGBA.B] + dx1 * p4[RGBA.B] ) );

							// alpha, 32 bit
							if (dstPs == 4) {
								dst [RGBA.A] = Use255ForAlpha ? (byte)255 : (byte) (
													dy2 * ( dx2 * p1[RGBA.A] + dx1 * p2[RGBA.A] ) +
													dy1 * ( dx2 * p3[RGBA.A] + dx1 * p4[RGBA.A] ) );
							}
                        }
                        cx++;
                    }
                    cy++;
                    dst += dstOffset;
                }
            }
        }
    }
}
