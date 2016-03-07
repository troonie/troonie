using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using DNColorPalette = System.Drawing.Imaging.ColorPalette;

namespace Picturez_Lib
{
	/// <summary>
	/// Source code adapted form AFroge.NET (www.aforgenet.com).
	/// </summary>
	public class AforgeImage
	{
		/// <summary>
		/// [Source: AForge.Net] Loads bitmap from file without file-locking.
		/// </summary>
		/// 
		/// <param name="fileName">File name to load bitmap from.</param>
		/// 
		/// <returns>Returns loaded bitmap.</returns>
		/// 
		/// <remarks><para>The method is provided as an alternative of <see cref="System.Drawing.Image.FromStream(System.IO.Stream)"/>
		/// method to solve the issues of locked file. The standard .NET's method locks the source file until
		/// image's object is disposed, so the file can not be deleted or overwritten. This method workarounds the issue and
		/// does not lock the source file.</para>
		/// 
		/// <para>Sample usage:</para>
		/// <code>
		/// Bitmap image = FromFile( "test.jpg" );
		/// </code>
		/// </remarks>
		/// 
		public static Bitmap FromFile( string fileName )
		{
			Bitmap loadedImage = null;
			FileStream stream = null;

			try
			{
				// read image to temporary memory stream
				stream = File.OpenRead( fileName );
				MemoryStream memoryStream = new MemoryStream( );

				byte[] buffer = new byte[10000];
				while ( true )
				{
					int read = stream.Read( buffer, 0, 10000 );

					if ( read == 0 )
						break;

					memoryStream.Write( buffer, 0, read );
				}

				loadedImage = (Bitmap) Bitmap.FromStream( memoryStream );
			}
			finally
			{
				if ( stream != null )
				{
					stream.Dispose( );
				}
			}

			return loadedImage;
		}

		/// <summary>
		/// [Source: AForge.Net] Clone image.
		/// </summary>
		/// 
		/// <param name="source">Source image.</param>
		/// <param name="format">Pixel format of result image.</param>
		/// 
		/// <returns>Returns clone of the source image with specified pixel format.</returns>
		///
		/// <remarks>The original <see cref="System.Drawing.Bitmap.Clone(System.Drawing.Imaging.PixelFormat)">Bitmap.Clone()</see>
		/// does not produce the desired result - it does not create a clone with specified pixel format.
		/// More of it, the original method does not create an actual clone - it does not create a copy
		/// of the image. That is why this method was implemented to provide the functionality.</remarks> 
		///
		public static Bitmap Clone( Bitmap source, PixelFormat format )
		{
			// copy image if pixel format is the same
			if ( source.PixelFormat == format )
				return Clone( source );

			int width = source.Width;
			int height = source.Height;

			// create new image with desired pixel format
			Bitmap bitmap = new Bitmap( width, height, format );

			// draw source image on the new one using Graphics
			Graphics g = Graphics.FromImage( bitmap );
			g.DrawImage( source, 0, 0, width, height );
			g.Dispose( );

			return bitmap;
		}

		/// <summary>
		/// [Source: AForge.Net] Clone image.
		/// </summary>
		/// 
		/// <param name="source">Source image.</param>
		/// 
		/// <returns>Return clone of the source image.</returns>
		/// 
		/// <remarks>The original <see cref="System.Drawing.Bitmap.Clone(System.Drawing.Imaging.PixelFormat)">Bitmap.Clone()</see>
		/// does not produce the desired result - it does not create an actual clone (it does not create a copy
		/// of the image). That is why this method was implemented to provide the functionality.</remarks> 
		/// 
		public static Bitmap Clone( Bitmap source )
		{
			// lock source bitmap data
			BitmapData sourceData = source.LockBits(
				new Rectangle( 0, 0, source.Width, source.Height ),
				ImageLockMode.ReadOnly, source.PixelFormat );

			// create new image
			Bitmap destination = Clone( sourceData );

			// unlock source image
			source.UnlockBits( sourceData );

			//
			if (
				( source.PixelFormat == PixelFormat.Format1bppIndexed ) ||
				( source.PixelFormat == PixelFormat.Format4bppIndexed ) ||
				( source.PixelFormat == PixelFormat.Format8bppIndexed ) ||
				( source.PixelFormat == PixelFormat.Indexed ) )
			{
				DNColorPalette srcPalette = source.Palette;
				DNColorPalette dstPalette = destination.Palette;

				int n = srcPalette.Entries.Length;

				// copy pallete
				for ( int i = 0; i < n; i++ )
				{
					dstPalette.Entries[i] = srcPalette.Entries[i];
				}

				destination.Palette = dstPalette;
			}

			return destination;
		}

		/// <summary>
		/// [Source: AForge.Net] Clone image.
		/// </summary>
		/// 
		/// <param name="sourceData">Source image data.</param>
		///
		/// <returns>Clones image from source image data. The message does not clone pallete in the
		/// case if the source image has indexed pixel format.</returns>
		/// 
		public static Bitmap Clone( BitmapData sourceData )
		{
			// get source image size
			int width = sourceData.Width;
			int height = sourceData.Height;

			// create new image
			Bitmap destination = new Bitmap( width, height, sourceData.PixelFormat );

			// lock destination bitmap data
			BitmapData destinationData = destination.LockBits(
				new Rectangle( 0, 0, width, height ),
				ImageLockMode.ReadWrite, destination.PixelFormat );

			CopyUnmanagedMemory( destinationData.Scan0, sourceData.Scan0, height * sourceData.Stride );

			// unlock destination image
			destination.UnlockBits( destinationData );

			return destination;
		}

		/// <summary>
		/// [Source: AForge.Net] Copy block of unmanaged memory.
		/// </summary>
		/// 
		/// <param name="dst">Destination pointer.</param>
		/// <param name="src">Source pointer.</param>
		/// <param name="count">Memory block's length to copy.</param>
		/// 
		/// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
		/// 
		/// <remarks><para>This function is required because of the fact that .NET does
		/// not provide any way to copy unmanaged blocks, but provides only methods to
		/// copy from unmanaged memory to managed memory and vise versa.</para></remarks>
		///
		public static IntPtr CopyUnmanagedMemory( IntPtr dst, IntPtr src, int count )
		{
			unsafe
			{
				CopyUnmanagedMemory( (byte*) dst.ToPointer( ), (byte*) src.ToPointer( ), count );
			}
			return dst;
		}

		/// <summary>
		/// [Source: AForge.Net] Copy block of unmanaged memory.
		/// </summary>
		/// 
		/// <param name="dst">Destination pointer.</param>
		/// <param name="src">Source pointer.</param>
		/// <param name="count">Memory block's length to copy.</param>
		/// 
		/// <returns>Return's value of <paramref name="dst"/> - pointer to destination.</returns>
		/// 
		/// <remarks><para>This function is required because of the fact that .NET does
		/// not provide any way to copy unmanaged blocks, but provides only methods to
		/// copy from unmanaged memory to managed memory and vise versa.</para></remarks>
		/// 
		public static unsafe byte* CopyUnmanagedMemory( byte* dst, byte* src, int count )
		{
			int countUint = count >> 2;
			int countByte = count & 3;

			uint* dstUint = (uint*) dst;
			uint* srcUint = (uint*) src;

			while ( countUint-- != 0 )
			{
				*dstUint++ = *srcUint++;
			}

			byte* dstByte = (byte*) dstUint;
			byte* srcByte = (byte*) srcUint;

			while ( countByte-- != 0 )
			{
				*dstByte++ = *srcByte++;
			}
			return dst;
		}
	}
}

