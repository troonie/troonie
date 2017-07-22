using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System;

namespace Troonie_Lib
{
	[Serializable]	public struct Keyword
	{
		[XmlAttribute]	public int Count { get; set; }
		[XmlText]		public string Text { get; set; }

		public class ComparerAscendingByCount : IComparer<Keyword>
		{
			public int Compare(Keyword x, Keyword y)
			{
				return x.Count.CompareTo (y.Count);
			}
		}

		public class ComparerDescendingByCount : IComparer<Keyword>
		{
			public int Compare(Keyword x, Keyword y)
			{
				return y.Count.CompareTo (x.Count);
			}
		}



		public class ComparerAscendingByText : IComparer<Keyword>
		{
			public int Compare(Keyword x, Keyword y)
			{
				return String.Compare(x.Text, y.Text);
			}
		}

		public class ComparerDescendingByText : IComparer<Keyword>
		{
			public int Compare(Keyword x, Keyword y)
			{
				return String.Compare(y.Text, x.Text);
			}
		}
	}		

	/// <summary> Serializer for storing Keywords. </summary>
	public class KeywordSerializer
    {
		private static string xmlFile = Constants.I.EXEPATH + "keywords.xml"; 

        /// <summary>File path for saving converted image(s).</summary>
		public List<Keyword> Keywords { get; set; }	

		public KeywordSerializer() {
			Keywords = new List<Keyword>();
		}

		public static KeywordSerializer Load()
		{
			if (!File.Exists (xmlFile)) {
				Save(new KeywordSerializer());
			}

			XmlSerializer serializer = new XmlSerializer(typeof(KeywordSerializer));
			StreamReader sr = new StreamReader(xmlFile);
			KeywordSerializer c = (KeywordSerializer)serializer.Deserialize(sr);

			sr.Close();
			return c;
		}

		public static void Save(KeywordSerializer c)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(KeywordSerializer));
			FileStream fs = new FileStream(xmlFile, FileMode.Create); 
			serializer.Serialize(fs, c);
			fs.Close();
		}
	}
}
