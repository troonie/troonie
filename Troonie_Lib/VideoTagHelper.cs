using System;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace Troonie_Lib
{
	public class VideoTagHelper
	{
        /// <summary>
        /// CreateDate in two tags simultaneously: "XMP-xmp:CreateDate" and "QuickTime:CreateDate"
        /// </summary>
        public const string ET_CreateDate = "CreateDate";
        public const string ET_TrackCreateDate = "TrackCreateDate";
        public const string ET_MediaCreateDate = "MediaCreateDate";

        /// <summary>
        /// Subject tag of XMP Dublin Core namespace. Subject means keywords.
        /// 
        /// </summary>
        public const string ET0_Subject_XMP_dc = "XMP-dc:Subject";
        /// <summary>
        /// ExifTool output name of Subject tag of XMP Dublin Core namespace. Subject means keywords.
        /// </summary>
        public const string ET1_Subject_XMP_dc = "Subject";

        /// <summary>
        /// Category tag of Microsoft Xtra Tags. Category means keywords.
        /// 
        /// <seealso cref="See"/> https://exiftool.org/TagNames/Microsoft.html.
        /// </summary>
        public const string ET0_Category_Microsoft = "Microsoft:Category";
        /// <summary>
        /// ExifTool output name of Category tag of Microsoft Xtra Tags. Category means keywords.
        /// </summary>
        public const string ET1_Category_Microsoft = "Category";

        /// <summary>
        /// Rating tag of XMP-xmp namespace.
        /// </summary>
        public const string ET0_Rating_XMP_xmp = "XMP-xmp:Rating";
        /// <summary>
        /// ExifTool output name of Rating tag of XMP-xmp namespace.
        /// </summary>
        public const string ET1_Rating_XMP_xmp = "Rating";

        /// <summary>
        /// Rating tag of Microsoft Xtra Tags. XMP-xmp:Rating values of 1,2,3,4 and 5 stars correspond to 
        /// SharedUserRating values of 1,25,50,75 and 99 respectively. 
        /// 
        /// <seealso cref="See"/> https://exiftool.org/TagNames/Microsoft.html.
        /// </summary>
        public const string ET0_Rating_Microsoft = "Microsoft:SharedUserRating";
        /// <summary>
        /// ExifTool output name of Rating tag of Microsoft Xtra Tags.
        /// </summary>
        public const string ET1_Rating_Microsoft = "Shared User Rating";

        /// <summary>
        /// Title tag of Quicktime namespace.
        /// </summary>
        public const string ET0_Title_Quicktime = "Quicktime:Title";
        /// <summary>
        /// ExifTool output name of Title tag of Quicktime namespace.
        /// </summary>
        public const string ET1_Title_Quicktime = "Title";

        /// <summary>
        /// Title tag of XMP Dublin Core namespace.
        /// </summary>
        public const string ET0_Title_XMP_dc = "XMP-dc:Title";
        /// <summary>
        /// ExifTool output name of Title tag of XMP Dublin Core namespace.
        /// </summary>
        public const string ET1_Title_XMP_dc = "Title";

        /// <summary>
        /// Comment tag of Quicktime namespace.
        /// </summary>
        public const string ET0_Comment_Quicktime = "Quicktime:Comment";
        /// <summary>
        /// ExifTool output name of Comment tag of Quicktime namespace.
        /// </summary>
        public const string ET1_Comment_Quicktime = "Comment";

        /// <summary>
        /// Copyright tag of Quicktime namespace.
        /// </summary>
        public const string ET0_Copyright_Quicktime = "Quicktime:Copyright";
        /// <summary>
        /// ExifTool output name of Copyright tag of Quicktime namespace.
        /// </summary>
        public const string ET1_Copyright_Quicktime = "Copyright";

        private static VideoTagHelper instance;
		public static VideoTagHelper I
		{
			get
			{
				if (instance == null) {
					instance = new VideoTagHelper();
				}
				return instance;
			}
		}

    public static TagsData GetTagsData(string fileName)
		{
			TagsData td = new TagsData ();
            // Tag cit = GetTag (fileName);
            // if (cit != null) {
            // ++++
            // TODO td.Comment = cit.Comment;
            // TODO td.Copyright = cit.Copyright;
            // td.Title = cit.Title;
            // }

            //        if (Constants.I.EXIFTOOL) 				
            //ChangeValueOfTagWithExiftool(true, fileName, TagsFlag.Title | TagsFlag.AllCreateDates | TagsFlag.Rating | TagsFlag.Keywords, ref td);

            bool success = true;
            string arg = " -S" + 
				" -" + ET_CreateDate + 
				" -" + ET_TrackCreateDate + 
				" -" + ET_MediaCreateDate +
                " -" + ET0_Rating_XMP_xmp +
                " -" + ET0_Title_Quicktime +
                " -" + ET0_Comment_Quicktime +
                " -" + ET0_Copyright_Quicktime +
                " -" + ET0_Subject_XMP_dc +
                Constants.WS + fileName;

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
                            case ET_CreateDate: td.DateTime = tt; break;
                            case ET_TrackCreateDate: td.TrackCreateDate = tt; break;
                            case ET_MediaCreateDate: td.MediaCreateDate = tt; break;
                            case ET1_Rating_XMP_xmp:
                                uint u;
                                if (uint.TryParse(result[i + 1], out u))
                                    td.Rating = u;
                                break;
                            case ET1_Title_Quicktime:
                                td.Title = result[i + 1]; break;
                            case ET1_Comment_Quicktime:
                                td.Comment = result[i + 1]; break;
                            case ET1_Copyright_Quicktime:
                                td.Copyright = result[i + 1]; break;
                            case ET1_Subject_XMP_dc:
                                List<string> list = new List<string>();

                                while (i + 1 < result.Length)
                                {
                                    list.Add(result[i + 1]);
                                    i++;
                                }
                                td.Keywords = list; break;
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

            //return success;
            return td;
		}

		public static bool SetTag(string fileName, TagsFlag flag, TagsData td)
		{
            bool success = true;
            string arg = " -S";
            uint flagValue = int.MaxValue;
            flagValue += 1;            
            bool KeywordsFlag = false;

            while (flagValue != 0)
            {
                switch (flag & (TagsFlag)flagValue)
                {
                    case TagsFlag.DateTime:
                        arg += " -" + ET_CreateDate + "=" + DateTimeToString(td.DateTime);
                        break;
                    case TagsFlag.TrackCreateDate:
                        arg += " -" + ET_TrackCreateDate + "=" + DateTimeToString(td.TrackCreateDate);
                        break;
                    case TagsFlag.MediaCreateDate:
                        arg += " -" + ET_MediaCreateDate + "=" + DateTimeToString(td.MediaCreateDate);
                        break;
                    case TagsFlag.AllCreateDates:
                        string sdate = "=" + DateTimeToString(td.DateTime); 
                        arg += " -" + ET_CreateDate + sdate + " -" + ET_TrackCreateDate + sdate + " -" + ET_MediaCreateDate + sdate;                        
                        break;
                    case TagsFlag.Rating:
                        string MS_Rating = "=";
                        string sRating = "=";
                        
                        if (td.Rating.HasValue) { 
                            sRating += td.Rating.Value;
                            switch (td.Rating.Value)
                            {
                                case 0: break;
                                case 1: MS_Rating += 1; break;
                                case 2: MS_Rating += 25; break;
                                case 3: MS_Rating += 50; break;
                                case 4: MS_Rating += 75; break;
                                case 5: MS_Rating += 99; break;
                            }
                        }

                        arg += " -" + ET0_Rating_XMP_xmp + sRating;
                        arg += " -" + ET0_Rating_Microsoft + MS_Rating;
                        break;
                    case TagsFlag.Title:
                        string sTitle = "=";
                        if (td.Title != null)
                            sTitle += "\"" + td.Title + "\"";

                        arg += " -" + ET0_Title_Quicktime + sTitle;
                        arg += " -" + ET0_Title_XMP_dc + sTitle;
                        break;
                    case TagsFlag.Comment:
                        arg += " -" + ET0_Comment_Quicktime + "=";
                        if (td.Comment != null)
                            arg += "\"" + td.Comment + "\"";
                        break;
                    case TagsFlag.Copyright:
                        arg += " -" + ET0_Copyright_Quicktime + "=";
                        if (td.Copyright != null)
                            arg += "\"" + td.Copyright + "\"";
                        break;
                    case TagsFlag.Keywords:
                        // Note: TagsFlag.Keywords needs to be last flag, BUT order of flags is NOT linear. SO ist needs to be ensure,
                        // that keywords will be processed at the end
                        KeywordsFlag = true;
                        break;
                }

                flagValue >>= 1;
            }

            if (KeywordsFlag)
            {                
                string sKeywords = string.Empty;
                if (td.Keywords.Count > 0)
                {
                    sKeywords = "\"" + td.Keywords[0];

                    for (int i = 1; i < td.Keywords.Count; i++)
                    {
                        sKeywords += ", " + td.Keywords[i];
                    }
                    sKeywords += "\" -sep " + "\", \""; ;

                }

                arg += " -" + ET0_Subject_XMP_dc + "=" + sKeywords;
                arg += " -" + ET0_Category_Microsoft + "=" + sKeywords;
            }

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

        #region private static functions

        private static string DateTimeToString(DateTime? dt)
        {
            string s = string.Empty;
            if (dt.HasValue)
                s = "\"" + dt.Value.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture) + "\"";

            return s;
        }


		#endregion private static functions
	}
}

