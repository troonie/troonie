using System.Diagnostics;
using System.IO;
using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Troonie_Lib
{   
    /// <summary>
    /// Static functions for saving a jpeg image with specified quality.
    /// </summary>
    public static class JpegEncoder
    {
		public static bool DoesImageWorkingWithDjpeg (string fileName) => 
		DoesImageWorkingWithDjpeg (fileName, out int exitCode, out string errorText);

		/// <summary>
		/// Checks whether a given image is working with djpeg or not.
		/// </summary>
		/// <returns><c>true</c>, if image is working with djpeg.<c>false</c> otherwise.</returns>
		/// <param name="fileName">The image filename.</param>
		/// <param name="exitCode">The Exit code.</param>
		/// <param name="errorText">The optional Error text.</param>
		public static bool DoesImageWorkingWithDjpeg (string fileName, out int exitCode, out string errorText)
		{
			bool b = false;
			FileInfo info = new FileInfo (fileName);
			string tmpFileName = info.Name.Replace (info.Extension, Constants.Extensions [TroonieImageFormat.BMP24].Item1);

			string bmpFileName = Constants.I.TEMPPATH + tmpFileName;
			string args = "-bmp -outfile \"" + bmpFileName + "\" \"" + fileName + "\"";

			// use jpeg lib for decoding jpeg files
			using (System.Diagnostics.Process proc = new System.Diagnostics.Process ()) {
				proc.StartInfo.FileName = Constants.I.WINDOWS ? (Constants.I.EXEPATH + Constants.DJPEGNAME + @".exe") : Constants.DJPEGNAME;
				proc.StartInfo.Arguments = args;
				proc.StartInfo.UseShellExecute = false;
				proc.StartInfo.CreateNoWindow = true;
				//proc.StartInfo.RedirectStandardOutput = true;
				proc.StartInfo.RedirectStandardError = true;
				proc.Start ();
				proc.WaitForExit ();
				//StreamReader srOutput = proc.StandardOutput;
				//string standardOutput = srOutput.ReadToEnd();
				//Console.WriteLine ("Output: " + standardOutput);
				//srOutput.Close();

				StreamReader srError = proc.StandardError;
				errorText = srError.ReadToEnd ();
				//Console.WriteLine ("Error: " + errorText);
				srError.Close ();

				exitCode = proc.ExitCode;
				proc.Close ();
				proc.Dispose ();
			}

			if (exitCode == 0) {
				File.Delete (bmpFileName);
				b = true;
			}

			return b;
		}

		public static bool ExistsCjpeg()
		{
			try
			{
				Process proc = new Process();
				proc.StartInfo.FileName = Constants.CJPEGNAME;
				proc.StartInfo.Arguments = "-v";
				proc.StartInfo.UseShellExecute = false; 
				proc.StartInfo.CreateNoWindow = true;
				proc.StartInfo.RedirectStandardOutput = true;
				proc.StartInfo.RedirectStandardError = true;
				proc.Start();
				proc.WaitForExit();

//				StreamReader srOutput = proc.StandardOutput;
//				string standardOutput = srOutput.ReadToEnd();
//				Console.WriteLine ("Output: " + standardOutput);
//				srOutput.Close();
//	
//				StreamReader srError = proc.StandardError;
//				string standardError = srError.ReadToEnd();
//				Console.WriteLine ("Error: " + standardError);
//				srError.Close();

//				int exitCode = proc.ExitCode;
				proc.Close();
				proc.Dispose();

				// check DJPEG
				proc = new Process();
				proc.StartInfo.FileName = Constants.DJPEGNAME;
				proc.StartInfo.Arguments = "-v";
				proc.StartInfo.UseShellExecute = false; 
				proc.StartInfo.CreateNoWindow = true;
				proc.StartInfo.RedirectStandardOutput = true;
				proc.StartInfo.RedirectStandardError = true;
				proc.Start();
				proc.WaitForExit();

				//				StreamReader srOutput = proc.StandardOutput;
				//				string standardOutput = srOutput.ReadToEnd();
				//				Console.WriteLine ("Output: " + standardOutput);
				//				srOutput.Close();

				//				StreamReader srError = proc.StandardError;
				//				string standardError = srError.ReadToEnd();
				//				Console.WriteLine ("Error: " + standardError);
				//				srError.Close();

				//				int exitCode = proc.ExitCode;
				proc.Close();
				proc.Dispose();

				return true;
			}
			catch  (Exception ) {
//				Console.WriteLine ("e = " + e.Message);
				return false;
			}
		}


//        /// <summary>
//        /// Saves an image as a jpeg image, with the given quality
//        /// </summary>
//        /// <param name="path">Path to which the image would be saved.</param>
//        /// <param name="img">The image</param>
//        /// <param name="quality">An integer from 0 to 100, with 100 being the
//        /// highest quality</param>
//		public static void SaveJpeg (string path, Image img, byte quality, bool grayscale)
//        {
//            if (quality>100)
//                throw new ArgumentOutOfRangeException(
//					"quality", "quality must be between 0 and 100.");
//            
////			if (Constants.I.WINDOWS) {
////				SaveWithDotNet (path, img, quality);
////			} else {
//				string p = path + ".tmp.bmp";
//				img.Save (p, ImageFormat.Bmp);
//				/* int error = */ SaveWithCjpeg (p, path, quality, grayscale);
//				File.Delete (p);
//
////				// Backup, if 'cjpeg' does not work
////				if (error != 0)
////					SaveWithDotNet (path, img, quality);
////			}
//        }

//		private static ImageCodecInfo GetImageCodecInfo(ImageFormat format)
//		{
//			ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
//
//			foreach (ImageCodecInfo item in codecs) {
//				if (item.FormatID == format.Guid)
//					return item;
//			}
//
//			return null;
//		} 
//
//		private static void SaveWithDotNet(string path, Image img, byte quality)
//		{
//			ImageCodecInfo jpgEncoder = GetImageCodecInfo (ImageFormat.Jpeg);
//			EncoderParameters encoderParameters = new EncoderParameters(2);
//			EncoderParameter qualityParam = new EncoderParameter (Encoder.Quality, (long)quality);
//			EncoderParameter compressionParam = new EncoderParameter (Encoder.Compression, 
//							(long)EncoderValue.CompressionNone);
//			encoderParameters.Param [0] = qualityParam;
//			encoderParameters.Param [1] = compressionParam;
//
//			img.Save(path, jpgEncoder, encoderParameters);			
//		}				

		// Why: https://bugzilla.novell.com/show_bug.cgi?id=506179
		public static bool SaveWithCjpeg(
			//			string origFilename, string destFilename, byte quality, bool grayscale)
			string path, Image img, byte quality, TroonieImageFormat f)
		{
			bool success;

			if (quality>100)
				throw new ArgumentOutOfRangeException(
					"quality", "quality must be set between 0 and 100.");

			string p = path + ".tmp.bmp";
			img.Save (p, ImageFormat.Bmp);


			//			string gray = f == TroonieImageFormat.JPEG8 ? " -grayscale " : "";
			string args = string.Empty;
			// LOSSLESS: cjpeg -rgb1 -block 1 -arithmetic
			// -quality 10 -outfile /home/jose/c.jpg  /home/jose/b.bmp
			switch (f) {
			case TroonieImageFormat.JPEG24:
				args = "-quality " + quality.ToString ();
				break;
			case TroonieImageFormat.JPEG8:
				args = "-quality " + quality.ToString () + " -grayscale";
				break;
			case TroonieImageFormat.JPEGLOSSLESS:
				args = "-rgb1 -block 1 -arithmetic";
				break;

			}

			args =	args + " -outfile \"" + path + "\" \"" + p + "\"";

			// args = "-quality 10 -outfile /home/jose/c3.jpg /home/jose/b.bmp";

			using (Process proc = new Process ()) {
				try {
					proc.StartInfo.FileName = Constants.I.WINDOWS ? (Constants.I.EXEPATH + Constants.CJPEGNAME + @".exe") : Constants.CJPEGNAME;   
					proc.StartInfo.Arguments = args; 
					proc.StartInfo.UseShellExecute = false; 
					proc.StartInfo.CreateNoWindow = true;
					//					proc.StartInfo.RedirectStandardOutput = true;
					//					proc.StartInfo.RedirectStandardError = true;
					proc.Start();
					proc.WaitForExit();
					proc.Close();
					success = true;
				}
				catch(Exception){
					success = false;
				}
				finally {
					File.Delete (p);
				}
			}

			return success;
		}
    }
}
