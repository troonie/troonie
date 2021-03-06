using System;
using System.IO;

namespace Troonie_Lib
{
	/// <summary>
	/// Extends System.File by using correct case writing of filename strings. E.g.: At Linux 'a.Txt', 'a.txt' and 'a.TXT' are three different files.
	/// </summary>
	public class FileHelper
	{
		private static FileHelper instance;
		public static FileHelper I
		{
			get
			{
				if (instance == null) {
					instance = new FileHelper ();
				}
				return instance;
			}
		}

		/// <summary>
		/// Extends System.File.Exists(..) with comparing the format string with normal, 
		/// lower and upper case (e.g. 'a.Txt', 'a.txt' and 'a.TXT').
		/// </summary>
		public bool Exists (string filename)
		{
			string format = filename.Substring (filename.LastIndexOf ('.'));
			string name = filename.Substring (0, filename.Length - format.Length);

			bool b1 = File.Exists (filename);
			bool b2 = File.Exists (name + format.ToLowerInvariant());
			bool b3 = File.Exists (name + format.ToUpperInvariant());

			return b1 || b2 || b3;
		}

		/// <summary>
		/// Renames a file. Note: <paramref name="filePath"/> includes absolute path and original 
		/// filename. <paramref name="newFilename"/> could but does not need to include the path.
		/// The new file name without path is enough.
		/// </summary>
		public bool Rename(string filePath, string newFilename)
		{
			string dir = Path.GetDirectoryName (filePath);
			string destPath = Path.Combine (dir, newFilename);
			bool success = true;

			try {
				File.Move (filePath, destPath);
			}
			catch (Exception) {
				success = false;
			}

			return success;
		}

		public string TransformStringToValidFilename(string filename, bool replaceSpacesByUnderscore)
		{
			filename = filename.Trim ();
			if (replaceSpacesByUnderscore) {
				filename = filename.Replace (' ', '_');
			}
			filename = System.Text.RegularExpressions.Regex.Replace(filename, @"[\\/:*?""<>|]", string.Empty);
			filename = filename.Replace("[", ""); // will not replaced by 'Regex.Replace(..)'
			filename = filename.Replace("]", ""); // will not replaced by 'Regex.Replace(..)'

			return filename;
		}
	}
}

