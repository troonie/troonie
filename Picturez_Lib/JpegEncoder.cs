using System.Diagnostics;
using System.IO;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Picturez_Lib
{   
    /// <summary>
    /// Static functions for saving a jpeg image with specified quality.
    /// </summary>
    public static class JpegEncoder
    {
        /// <summary>
        /// Saves an image as a jpeg image, with the given quality
        /// </summary>
        /// <param name="path">Path to which the image would be saved.</param>
        /// <param name="img">The image</param>
        /// <param name="quality">An integer from 0 to 100, with 100 being the
        /// highest quality</param>
		public static void SaveJpeg (string path, Image img, byte quality, bool grayscale)
        {
            if (quality>100)
                throw new ArgumentOutOfRangeException(
					"quality", "quality must be between 0 and 100.");
            
			if (Constants.I.WINDOWS) {
				SaveWithDotNet (path, img, quality);
			} else {
				string p = path + ".tmp.bmp";
				img.Save (p, ImageFormat.Bmp);
				int error = SaveWithCjpeg (p, path, quality, grayscale);
				File.Delete (p);

				// Backup, if 'cjpeg' does not work
				if (error != 0)
					SaveWithDotNet (path, img, quality);
			}
        }

        /// <summary>
        /// Returns the image codec with the given mime type
        /// </summary>
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec
            for(int i=0; i<codecs.Length; i++)
                if(codecs[i].MimeType == mimeType)
                    return codecs[i];
            return null;
        } 

		private static void SaveWithDotNet(string path, Image img, byte quality)
		{
			//create an encoder parameter for the image quality
			EncoderParameter qualityParam = 
				new EncoderParameter(Encoder.Quality, quality);
			EncoderParameter compressionParam = 
				new EncoderParameter(Encoder.Compression, 
					(long)EncoderValue.CompressionNone);

			ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");
			EncoderParameters encoderParams = new EncoderParameters(2);
			//set the quality and compression parameters for codec
			encoderParams.Param[0] = qualityParam;
			encoderParams.Param[1] = compressionParam;

			img.Save(path, jpegCodec, encoderParams);			
		}				

		// Why: https://bugzilla.novell.com/show_bug.cgi?id=506179
		private static int SaveWithCjpeg(
			string origFilename, string destFilename, int quality, bool grayscale)
		{
			string gray = grayscale ? " -grayscale " : "";

			// -quality 10 -outfile /home/jose/c.jpg  /home/jose/b.bmp
			string args = "-quality " + quality.ToString() + gray + 
				"  -outfile " + destFilename + " " + origFilename;

			// args = "-quality 10 -outfile /home/jose/c3.jpg /home/jose/b.bmp";
			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			proc.StartInfo.FileName = "cjpeg";   
			proc.StartInfo.Arguments = args; 
			proc.StartInfo.UseShellExecute = false; 
//			proc.StartInfo.RedirectStandardOutput = true;
//			proc.StartInfo.RedirectStandardError = true;

			proc.Start();

			proc.WaitForExit();
//			StreamReader srOutput = proc.StandardOutput;
//			string standardOutput = srOutput.ReadToEnd();
//			Console.WriteLine ("Output: " + standardOutput);
//
//			StreamReader srError = proc.StandardError;
//			string standardError = srError.ReadToEnd();
//			Console.WriteLine ("Error: " + standardError);

			int exitCode = proc.ExitCode;
			proc.Close();

			return exitCode;
		}
    }
}
