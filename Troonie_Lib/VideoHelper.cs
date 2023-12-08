using System;
using System.Diagnostics;
using System.IO;

namespace Troonie_Lib
{
    /// <summary>
    /// ### EXAMPLE USAGE ### 
    /// VideoHelper.RepairMp4WithFfmpeg ("C:\\Users\\USERNAME\\Desktop\\Photos");
    /// VideoHelper.SetTagsInVideoFromPng("C:\\Users\\USERNAME\\Desktop\\Photos", false);
    /// </summary>
    public class VideoHelper
	{
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
                //TagsData td = ImageTagHelper.GetTags(mp4file);

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

                // TODO: Used ET function to copy all tags NOT tested yet. Please test it. If it does not work, use following line:
                 // success = ImageTagHelper.SetTags(mp4file, (TagsFlag)0xFFFFFF, td);
                string tArg = " -overwrite_original -S -TagsFromFile " + origfilename + " \"-all:all>all:all\" " + " \"" + mp4file + "\" ";
                Constants.I.ET.Process(tArg);
                success = Constants.I.ET.Success;

                if (!success)
                    return false;
            }

            return true;
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

                TagsData td = ImageTagHelper.GetTags(pngFile);

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

                success = ImageTagHelper.SetTags(mp4file, (TagsFlag)0xFFFFFF, td);
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