using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

namespace Picturez_Lib
{
	public class Language
	{
		private static string langFileName = Constants.I.EXEPATH + "languages.csv";
		private static Language instance;
		public static Language I
		{
			get
			{
				if (instance == null)
					instance = new Language ();

				return instance;
			}
		}

		public static string AllLanguagesAsString{ get; private set; }

		private Dictionary<int, List<string>> allLanguages;

		private List<string> language;
		public List<string> L {	get	{ return language;	} }

		public int languageID;

		public int LanguageID {
			get {
				return languageID;
			}
			set {
				languageID = value >= allLanguages.Count ? 0 : value;
				allLanguages.TryGetValue (languageID, out language);
			}
		}
	
		public Language ()
		{			
			allLanguages = new Dictionary<int, List<string>> ();
			Init ();
			LanguageID = SetLanguageID ();
		}
			
		private void Init()
		{			
			string line;
			StreamReader file =	new StreamReader(langFileName);
			// read first line
			line = file.ReadLine ();
			string[] tempArray = line.Split (';', '\t');
			// number of languages
			int nr = tempArray.Length;
			for (int i = 0; i < nr; i++) { 
				List<string> lang = new List<string> ();
				// Do it 2x to match zero-based indexing of list 
				//.. with non-zreo-based CSV-file
				lang.Add (tempArray [i]);
				lang.Add (tempArray [i]);
				AllLanguagesAsString += tempArray [i] + "\n"; 

				allLanguages.Add (i, lang);
			}

			while((line = file.ReadLine()) != null)
			{
				tempArray = line.Split (';', '\t');
				for (int i = 0; i < nr; i++) {
					allLanguages[i].Add(tempArray [i]);
				}
			}

			file.Close();			
		}

		private int SetLanguageID()
		{
			switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName) {
			case "de":
				return 1;
			case "en":
			default:
				return 0;
			}
		}
	}
}

