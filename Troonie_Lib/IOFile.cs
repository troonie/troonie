using System;
using System.IO;

namespace Troonie_Lib
{
    /// <summary>
    /// Helper class for reading out, writing in and append (text-)files.
    /// </summary>
    public class IOFile
    {
		private static IOFile instance;
		public static IOFile I
		{
			get
			{
				if (instance == null) {
					instance = new IOFile ();
				}
				return instance;
			}
		}

        ///<summary> Returns complete content of passed file.</summary>
        public string Read(String filename)
        {
            string tContent = "";

            if (File.Exists(filename))
            {
                StreamReader tFile = new StreamReader(filename, System.Text.Encoding.Default);
                tContent = tFile.ReadToEnd();
                tFile.Close();
            }
            return tContent;
        }

        ///<summary>
		/// Appends passed lines into the file.
        ///</summary>
		public void Write(string filename, string text, bool append)
        {
			StreamWriter tFile = new StreamWriter(filename, append);
			tFile.Write(text);
            tFile.Close();
        }

        ///<summary>
		/// Reads out text in specified line.
        ///</summary>
        public string ReadLine(String filename, int line)
        {
            string tContent = "";
            float tRow = 0;
            if (File.Exists(filename))
            {
                StreamReader tFile = new StreamReader(filename, System.Text.Encoding.Default);
                while (!tFile.EndOfStream && tRow < line)
                {
                    tRow++;
                    tContent = tFile.ReadLine();
                }
                tFile.Close();
                if (tRow < line)
                    tContent = "";
            }
            return tContent;
        }

        /// <summary>
        /// Writes passed text into specified line. Text can be replaced or appended.
        /// </summary>
        public void WriteLine(String filename, int line, string text, bool replace)
        {
            string tContent = "";
			string n = Environment.NewLine;
			string[] tDelimiterstring = { Environment.NewLine };

            if (File.Exists(filename))
            {
                StreamReader tFile = new StreamReader(filename, System.Text.Encoding.Default);
                tContent = tFile.ReadToEnd();
                tFile.Close();
            }

            string[] tCols = tContent.Split(tDelimiterstring, StringSplitOptions.None);

            if (tCols.Length >= line)
            {
                if (!replace)
					tCols[line - 1] = text + Environment.NewLine + tCols[line - 1];
                else
                    tCols[line - 1] = text;

                tContent = "";
                for (int x = 0; x < tCols.Length - 1; x++)
                {
					tContent += tCols[x] + Environment.NewLine;
                }
                tContent += tCols[tCols.Length - 1];

            }
            else
            {
                for (int x = 0; x < line - tCols.Length; x++)
					tContent += Environment.NewLine;

                tContent += text;
            }


            StreamWriter tSaveFile = new StreamWriter(filename);
            tSaveFile.Write(tContent);
            tSaveFile.Close();
        }

        /// <summary>
        /// Checks, if a directory is empty.
        /// </summary>
        /// <param name="path">Path of the directory.</param>
        /// <returns>
        /// 	<c>true</c> if directory is empty, otherwise <c>false</c>.
        /// </returns>
        public bool IsDirEmpty(string path)
        {
            if (Directory.GetDirectories(path).Length == 0 &&
                Directory.GetFiles(path).Length == 0)
                return true;
            else
                return false;
        }
    }
}
