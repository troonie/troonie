using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System;

namespace Troonie_Lib
{
	[Serializable]	public class Keyword : IEquatable<Keyword>
	{
		[XmlAttribute]	public int Count { get; set; }
		[XmlText]		public string Text { get; set; }

		public Keyword() {
			this.Text = "";
			this.Count = 1;
		}

		public Keyword(string text) {
			this.Text = text;
			this.Count = 1;
		}

		public Keyword(string text, int count) {
			this.Text = text;
			this.Count = count;
		}

		#region IEquatable

		public override string ToString()
		{
			return "Count: " + Count + "   Text: " + Text;
		}

		public override bool Equals(object obj)
		{
			if (obj == null) {
				return false;
			}

			Keyword objAsKeyword = (Keyword)obj;

			if (objAsKeyword == null) {
				return false;
			} else {
				return Equals (objAsKeyword);
			}
		}

		public override int GetHashCode()
		{
			return Text.GetHashCode();
		}

		public bool Equals(Keyword other)
		{
			if (other == null) {
				return false;
			}
				
			return Text.Equals(other.Text);
		}
			
		#endregion IEquatable


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

		public class ComparerDescendingByCountAndAscendingByText : IComparer<Keyword>
		{
			public int Compare(Keyword x, Keyword y)
			{
				int comp = y.Count.CompareTo (x.Count);
				if (comp == 0) {
					comp = String.Compare(x.Text, y.Text);
				}
				return comp;
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
		private static string xmlFile; // = Constants.I.EXEPATH + "keywords.xml"; 

		public const int MAX_NUMBER_OF_KEYWORDS = 150;

        /// <summary>File path for saving converted image(s).</summary>
		public List<Keyword> Keywords { get; set; }	

		private KeywordSerializer() {
			Keywords = new List<Keyword>();
		}

		public static KeywordSerializer Load()
		{
			xmlFile = Constants.I.CONFIG.KeywordsXmlFilePath;

			if (!File.Exists (xmlFile)) {
				Save(new KeywordSerializer());
			}

			XmlSerializer serializer = new XmlSerializer(typeof(KeywordSerializer));
			StreamReader sr = null;
			KeywordSerializer c;

			try {
				sr = new StreamReader(xmlFile);
				c = (KeywordSerializer)serializer.Deserialize(sr);
			}
			catch (Exception){
				Constants.I.CONFIG.KeywordsXmlFilePath = Constants.I.EXEPATH + "keywords.xml";
				c = Load ();
			}
			finally {
				if (sr != null) {
					sr.Close ();
				}
			}

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
