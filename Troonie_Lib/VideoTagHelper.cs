using System;
using TagLib;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using TagLib.IFD.Entries;
using TagLib.IFD.Tags;
using TagLib.IFD;

namespace Troonie_Lib
{
	public class VideoTagHelper
	{
		private static VideoTagHelper instance;
		public static VideoTagHelper I
		{
			get
			{
				if (instance == null) {
					instance = new VideoTagHelper ();
				}
				return instance;
			}
		}			
			
//		public static void SetDateAndRatingInVideoTag(string fileName, uint rating)
//		{
//			TagsFlag flag = TagsFlag.Track;
//			uint dateAsUint;
//			string dateAsString;
//			GetDateFromFilenameAsUint (fileName, out dateAsUint, out dateAsString);
//			TagsData td = new TagsData { Track = rating };
//
//			if (dateAsUint != 0) {
//				flag = TagsFlag.Track | TagsFlag.Year | TagsFlag.Composers;
//				td.Year = dateAsUint;
//				td.Composers = new List<string>{ "Creation date (Troonie): " + dateAsString, "Rating (Troonie): " + rating };
//			} 
//
//			SetTag(fileName, flag, td);
//		}

		public static TagsData GetTagsData(string fileName)
		{
			TagsData td = new TagsData ();
			Tag cit = GetTag (fileName);
			if (cit != null) {


                // ++++
                td.Comment = cit.Comment;
				td.Copyright = cit.Copyright;
				td.Title = cit.Title;
            }

            if (Constants.I.EXIFTOOL) 				
				ChangeValueOfTagWithExiftool(true, fileName, TagsFlag.AllCreateDates | TagsFlag.Rating | TagsFlag.Keywords, ref td);

            return td;
		}

		public static bool SetTag(string fileName, TagsFlag flag, TagsData newData)
		{
			bool success = true;
			TagLib.File tagFile = LoadTagFile (fileName);
			Tag tag = tagFile.Tag;

			try{
				ChangeValueOfTag (tag, flag, newData);
				tagFile.Save();
				tagFile.Dispose ();

                if (Constants.I.EXIFTOOL)
                    success = ChangeValueOfTagWithExiftool(false, fileName, flag, ref newData);
            }
			catch (Exception /* UnsupportedFormatException */ ) {
				success = false;
			}

			return success;
		}

		#region private static functions

		private static Tag GetTag(string fileName)
		{
			TagLib.File tagFile = LoadTagFile(fileName);			

			if (tagFile == null)
				return null;
			
			Tag tag = tagFile.Tag;
			tagFile.Dispose ();
			return tag;
		}

		private static void ChangeValueOfTag(Tag tag, TagsFlag flag, TagsData newData)
		{
			uint flagValue = int.MaxValue;
			flagValue += 1;

			while(flagValue != 0)
			{
				switch (flag & (TagsFlag)flagValue) {
				case TagsFlag.Comment:		tag.Comment = newData.Comment;				break;
				case TagsFlag.Copyright:	tag.Copyright = newData.Copyright;			break;
				case TagsFlag.Title:		tag.Title = newData.Title;					break;
                }

				flagValue >>= 1;
			}
		}

		private static bool ChangeValueOfTagWithExiftool(bool readOnly, string fileName, TagsFlag flag, ref TagsData td)
		{
			bool success = true;
			string arg = " -S";
			uint flagValue = int.MaxValue;
			flagValue += 1;

            #region readOnly
			if (readOnly)
			{ 
				while (flagValue != 0)
				{
					switch (flag & (TagsFlag)flagValue)
					{
						case TagsFlag.DateTime:
							arg += " -" + TagsData.sCreateDate;							
							break;
						case TagsFlag.TrackCreateDate:
							arg += " -" + TagsData.sTrackCreateDate;							
							break;
						case TagsFlag.MediaCreateDate:
							arg += " -" + TagsData.sMediaCreateDate;							
							break;
						case TagsFlag.AllCreateDates:
							arg += " -" + TagsData.sCreateDate + " -" + TagsData.sTrackCreateDate + " -" + TagsData.sMediaCreateDate;
							break;
						case TagsFlag.Rating:
							arg += " -" + TagsData.sXMPRating;
							break;
						case TagsFlag.Keywords:
                            arg += " -" + TagsData.sXMPSubject;
                            break;
					}

					flagValue >>= 1;
				}
            }
            #endregion readOnly

            #region NOT readOnly
            else
            {               
				string s = string.Empty;
				bool KeywordsFlag = false;

				while (flagValue != 0)
				{
					switch (flag & (TagsFlag)flagValue)
					{
						case TagsFlag.DateTime:
							arg += " -" + TagsData.sCreateDate + "=";
							if (td.DateTime.HasValue)
								s = "\"" + td.DateTime.Value.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture) + "\"";                        
							break;
						case TagsFlag.TrackCreateDate: 
							arg += " -" + TagsData.sTrackCreateDate + "=";
							if (td.TrackCreateDate.HasValue)
								s = "\"" + td.TrackCreateDate.Value.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture) + "\"";
							break;
						case TagsFlag.MediaCreateDate: 						
							arg += " -" + TagsData.sMediaCreateDate + "=";
							if (td.MediaCreateDate.HasValue)
								s = "\"" + td.MediaCreateDate.Value.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture) + "\"";
							break;
						case TagsFlag.AllCreateDates:
							s = "=";
							if (td.DateTime.HasValue)
								s = "=\"" + td.DateTime.Value.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture) + "\"";

							arg += " -" + TagsData.sCreateDate + s + " -" + TagsData.sTrackCreateDate + s + " -" + TagsData.sMediaCreateDate; // + s;                        

							break;
						case TagsFlag.Rating:
							arg += " -" + TagsData.sXMPRating + "=";
							if (td.Rating.HasValue)
								s = "\"" + td.Rating.Value + "\"";
							break;
						case TagsFlag.Keywords:
							// Note: TagsFlag.Keywords needs to be last flag, BUT order of flags is NOT linear. SO ist needs to be ensure,
							// that keywords will processed at the end
							KeywordsFlag = true;
							break;
					}

					flagValue >>= 1;
				}

				if (KeywordsFlag)
				{
					arg += " -" + TagsData.sXMPSubject + "=";
					if (td.Keywords.Count > 0)
					{
						s = "\"" + td.Keywords[0];

						for (int i = 1; i < td.Keywords.Count; i++)
						{
							s += ", " + td.Keywords[i];
						}
						s += "\" -sep " + "\", \""; ;

					}
				}

				arg += s;
            }
            #endregion NOT readOnly

            arg += Constants.WS + fileName;

			using (Process proc = new Process())
			{
				try
				{
					proc.StartInfo.FileName = Constants.I.EXEPATH + Path.DirectorySeparatorChar + Constants.EXIFTOOLNAME;
					proc.StartInfo.Arguments = arg;
					proc.StartInfo.UseShellExecute = false;
					proc.StartInfo.CreateNoWindow = true;
					proc.StartInfo.RedirectStandardOutput = true;
					proc.StartInfo.RedirectStandardError = true;
					proc.Start();
					//* Read the output
					if (readOnly)
					{
						string output = proc.StandardOutput.ReadToEnd();
						string[] result = output.Split(new string[] { "\r\n", ": ", ", " }, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < result.Length - 1; i = i + 2)
						{
                            DateTime t;
                            DateTime? tt = null;
                            // bool b = DateTime.TryParseExact(result[i + 1], "yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.AssumeUniversal, out t);
                            bool b = DateTime.TryParseExact(result[i + 1], "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out t);
                            if (b) 
							{
								tt = t; // t.ToLocalTime();
                            }
							switch (result[i])
							{
                                case TagsData.sCreateDate: td.DateTime = tt; break;
								case TagsData.sTrackCreateDate: td.TrackCreateDate = tt; break;
								case TagsData.sMediaCreateDate: td.MediaCreateDate = tt; break;
								case "Rating":
									uint u;
									if (uint.TryParse(result[i + 1], out u))
										td.Rating = u;
									break;
                                case "Subject":
									List<string> list = new List<string>();

                                    while (i + 1 < result.Length)
									{
										list.Add(result[i + 1]);
										i++;
									}
									td.Keywords = list; break;
							}

						}
                    }

                    proc.WaitForExit();
					proc.Close();						
				}
				catch (Exception)
				{
					success = false;
				}
			}

			return success;
        }

        private static TagLib.File LoadTagFile(string fileName)
		{
			TagLib.File tagFile;
			try{
				tagFile = TagLib.File.Create(fileName);
				if (tagFile == null){
					return null;
				}
			}
			catch (Exception /* UnsupportedFormatException */) {
				return null;
			}									

			return tagFile;
		}			

		#endregion private static functions
	}
}

