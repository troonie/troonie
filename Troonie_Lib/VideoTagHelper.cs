using System;
using System.Diagnostics;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

namespace Troonie_Lib
{
	public class VideoTagHelper
	{
        public const TagsFlag AllVideoFlags =   TagsFlag.AllCreateDates | 
                                                TagsFlag.Rating | 
                                                TagsFlag.Title | 
                                                TagsFlag.Comment | 
                                                TagsFlag.Copyright | 
                                                TagsFlag.Keywords;

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

    public static TagsData GetTagsData(string fileName, out bool success)
		{
			TagsData td = new TagsData ();
            success = true;
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
                                //uint u;
                                //if (uint.TryParse(result[i + 1], out u))
                                //    td.Rating = u;
                                td.Rating = result[i + 1];
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

            return td;
		}

		public static bool SetTag(string fileName, TagsFlag flag, TagsData td)
		{
            bool success = true;
            string arg = " -overwrite_original -S";
            uint flagValue = int.MaxValue;
            flagValue += 1;            
            bool KeywordsFlag = false;

            while (flagValue != 0)
            {
                switch (flag & (TagsFlag)flagValue)
                {
                    case TagsFlag.DateTime:
                        arg += " -" + ET_CreateDate + "=" + ExifTool.DateTimeToString(td.DateTime);
                        break;
                    case TagsFlag.TrackCreateDate:
                        arg += " -" + ET_TrackCreateDate + "=" + ExifTool.DateTimeToString(td.TrackCreateDate);
                        break;
                    case TagsFlag.MediaCreateDate:
                        arg += " -" + ET_MediaCreateDate + "=" + ExifTool.DateTimeToString(td.MediaCreateDate);
                        break;
                    case TagsFlag.AllCreateDates:
                        string sdate = "=" + ExifTool.DateTimeToString(td.DateTime); 
                        arg += " -" + ET_CreateDate + sdate + " -" + ET_TrackCreateDate + sdate + " -" + ET_MediaCreateDate + sdate;                        
                        break;
                    case TagsFlag.Rating:
                        string MS_Rating = "=";
                        string sRating = "=";
                        
                        if (td.Rating.Length != 0) { 
                            sRating += td.Rating;
                            switch (td.Rating)
                            {
                                case "0": break;
                                case "1": MS_Rating += 1; break;
                                case "2": MS_Rating += 25; break;
                                case "3": MS_Rating += 50; break;
                                case "4": MS_Rating += 75; break;
                                case "5": MS_Rating += 99; break;
                            }
                        }

                        arg += " -" + ET0_Rating_XMP_xmp + sRating;
                        arg += " -" + ET0_Rating_Microsoft + MS_Rating;
                        break;
                    case TagsFlag.Title:
                        string sTitle = "=";
                        if (td.Title != null)
                            sTitle += "\"" + StringHelper.ReplaceGermanUmlauts(td.Title) + "\"";

                        arg += " -" + ET0_Title_Quicktime + sTitle;
                        arg += " -" + ET0_Title_XMP_dc + sTitle;
                        break;
                    case TagsFlag.Comment:
                        arg += " -" + ET0_Comment_Quicktime + "=";
                        if (td.Comment != null)
                            arg += "\"" + StringHelper.ReplaceGermanUmlauts(td.Comment) + "\"";
                        break;
                    case TagsFlag.Copyright:
                        arg += " -" + ET0_Copyright_Quicktime + "=";
                        if (td.Copyright != null)
                            arg += "\"" + StringHelper.ReplaceGermanUmlauts(td.Copyright) + "\"";
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
                if (td.Keywords != null && td.Keywords.Count > 0)
                {
                    sKeywords = "\"" + StringHelper.ReplaceGermanUmlauts(td.Keywords[0]); // td.Keywords[0];

                    for (int i = 1; i < td.Keywords.Count; i++)
                    {
                        sKeywords += ", " + StringHelper.ReplaceGermanUmlauts(td.Keywords[i]);
                    }
                    sKeywords += "\" -sep " + "\", \"";

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

        public static bool RepairMp4WithFfmpeg(string path)
        {
            bool success = true;
            Constants.I.Init();
            path += Path.DirectorySeparatorChar;
            string[] mp4files = Directory.GetFiles(path);

            foreach (string mp4file in mp4files)
            {
                if (!mp4file.Contains(".mp4"))
                {
                    continue;
                }

                string dir = Path.GetDirectoryName(mp4file);
                string backupdir = dir + Path.DirectorySeparatorChar + "backup";
                Directory.CreateDirectory(backupdir);

                // string origfilename = mp4file.Replace(".mp4", "_orig.mp4");
                string origfilename = mp4file.Replace(dir, backupdir);

                File.Copy(mp4file, origfilename, true);
                TagsData td = GetTagsData(mp4file, out success);
                if (!success)
                    return false;
                // do ffmpeg
                string arg = "-y -i " + origfilename + " -map_metadata 0 -c copy " + mp4file;
                using (Process proc = new Process())
                {
                    try
                    {
                        proc.StartInfo.FileName = path + "ffmpeg.exe";
                        proc.StartInfo.Arguments = arg;
                        proc.StartInfo.UseShellExecute = false;
                        proc.StartInfo.CreateNoWindow = true;
                        proc.StartInfo.RedirectStandardOutput = true;
                        proc.StartInfo.RedirectStandardError = true;
                        proc.Start();
                        proc.WaitForExit(10 * 1000);
                        proc.Close();
                    }
                    catch (Exception)
                    {
                        success = false;
                        return false;
                    }
                }

                success = SetTag(mp4file, AllVideoFlags, td);
                if (!success)
                    return false;
            }

            return success;
        }

        public static bool SetTagsInVideoFromPng(string path, bool repairWithFfmpeg = false)
        {
            bool success = true;
            Constants.I.Init();
            string ffmpeg = Constants.I.EXEPATH + Path.DirectorySeparatorChar + "ffmpeg.exe";
            path += Path.DirectorySeparatorChar;
            string[] mp4files = Directory.GetFiles(path, "*.mp4");
            Array.Sort(mp4files);
            string[] pngfiles = Directory.GetFiles(path, "*.png");
            Array.Sort(pngfiles);           

            foreach (string mp4file in mp4files)
            {
                if (!mp4file.Contains(".mp4"))
                {
                    continue;
                }

                string dir = Path.GetDirectoryName(mp4file);
                string subMp4file = mp4file.Substring(0, dir.Length + 18);
                string pngFile = Array.Find(pngfiles, s => s.Contains(subMp4file));
                if (pngFile == null) 
                {
                    Console.WriteLine("ID1: PNG missing from: " + mp4file);
                    return false;
                }

                
                string backupdir = dir + Path.DirectorySeparatorChar + "backup";
                Directory.CreateDirectory(backupdir);

                string mp4fileOrig = mp4file.Replace(dir, backupdir);
                File.Copy(mp4file, mp4fileOrig, true);

                TagsData td = ImageTagHelper.GetTagsDataET(pngFile);

                // do ffmpeg
                if (repairWithFfmpeg)
                {                     
                    string arg = "-y -i " + mp4fileOrig + " -map_metadata 0 -c copy " + mp4file;
                    using (Process proc = new Process())
                    {
                        Console.WriteLine("ID2: Processing with ffmpeg, file: " + mp4file);
                        try
                        {
                            proc.StartInfo.FileName = ffmpeg; // path + "ffmpeg.exe";
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
                            Console.WriteLine("ID3: Error with ffmpeg, file: " + mp4file);
                            return false;
                        }
                    }
                }

                success = SetTag(mp4file, AllVideoFlags, td);
                if (!success)
                {
                    Console.WriteLine("ID5: Error with Exiftool, file: " + mp4file);
                    return false;
                }
            }

            return success;
        }

	}
}

