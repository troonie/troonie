using System;
using IOFile = System.IO.File;

namespace Picturez_Lib
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
		/// Extends System.File.Exists(..) with comparing the format string with normal, lower and upper case (e.g. 'a.Txt', 'a.txt' and 'a.TXT').
		/// </summary>
		public bool Exists (string filename)
		{
			string format = filename.Substring (filename.LastIndexOf ('.'));
			string name = filename.Substring (0, filename.Length - format.Length);

			bool b1 = IOFile.Exists (filename);
			bool b2 = IOFile.Exists (name + format.ToLowerInvariant());
			bool b3 = IOFile.Exists (name + format.ToUpperInvariant());

			return b1 || b2 || b3;
		}
	}
}

