namespace Picturez_Lib
{    
    using System.Drawing;
    using System.Drawing.Imaging;

    public abstract class AbstractFilter
    {
        /// <summary>
        /// Applies the filter on the passed <paramref name="source"/> bitmap.
        /// </summary>
        /// <param name="source">The source image to process.</param>
        /// <returns>The filter result as a new bitmap.</returns>
        public Bitmap Apply(Bitmap source)
        {
            Bitmap destination = new Bitmap(
                source.Width, source.Height, PixelFormat.Format32bppArgb);
            Rectangle rect = new Rectangle(0, 0, source.Width, source.Height);
            BitmapData srcData = source.LockBits(
                rect, ImageLockMode.ReadWrite, source.PixelFormat);
            BitmapData dstData = destination.LockBits(
                rect, ImageLockMode.ReadWrite, destination.PixelFormat);
            Process(srcData, dstData);
            destination.UnlockBits(dstData);
            source.UnlockBits(srcData);

            if (destination.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                // -- ColorPalette.SetColorPaletteToGray(destination); --
                // get palette
                var cp = destination.Palette;
                // init palette
                for (int i = 0; i < 256; i++)
                {
                    cp.Entries[i] = Color.FromArgb(i, i, i);
                }
                // set palette back
                destination.Palette = cp;
                // END -- ColorPalette.SetColorPaletteToGray(destination); --
            }

            return destination;
        }

        protected abstract void Process(BitmapData srcData, BitmapData dstData);
    }
}
