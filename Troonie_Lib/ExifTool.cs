
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Troonie_Lib
{
    /// <summary>
    /// Exiftool processing in C#. Some source from forum: https://exiftool.org/forum/index.php?topic=11286.0.
    /// </summary>
    public class ExifTool
    {
        public List<string> Lines { get; private set; }

        public bool IsStarted { get; private set; }
        //public bool Success { get; private set; }
        private Process pExifTool;

        public ExifTool() 
        { 
            IsStarted = false;
            pExifTool = new Process();
            Lines = new List<string>();
        }

        ~ExifTool()
        {
            IsStarted = false;
            pExifTool.Close();
            pExifTool.Dispose();
            Lines.Clear();
            Lines = null;
        }

        //public List<string> RunWhileStayingOpen(string args)
        //{
        //    //string[]argsArray = args.Split(new string[] { "\r\n", " " }, StringSplitOptions.RemoveEmptyEntries);
        //    return RunWhileStayingOpen(new[] { args });
        //}


        /// <summary>
        /// Runs exiftool process by using the passed <paramref name="args"/>.
        /// </summary>        
        /// <param name="args">The arguments for exiftool.</param>
        public bool Process(string args, bool readOnly)
        {
            //  Start exiftool and keep it in memory.
            if (!IsStarted)
            {
                pExifTool.StartInfo.FileName = Constants.I.EXEPATH + Path.DirectorySeparatorChar + Constants.EXIFTOOLNAME;
                pExifTool.StartInfo.Arguments = args;
                pExifTool.StartInfo.RedirectStandardOutput = true;

                //  NOTE:  If you do not implement an asynchronous error handler like in this example, instead simply using pExifTool.StandardError.ReadLine()
                //         in the following, you risk that your program might stall.  This is because ExifTool sometimes reports failure only through a
                //         StandardOutput line saying something like "0 output files created" without reporting anything in addition via StandardError, so
                //         pExifTool.StandardError.ReadLine() would wait indefinitely for an error message that never comes.
                pExifTool.StartInfo.RedirectStandardError = true;
                pExifTool.ErrorDataReceived += new DataReceivedEventHandler(ETErrorHandler);

                pExifTool.StartInfo.UseShellExecute = false;
                pExifTool.StartInfo.CreateNoWindow = true;
                pExifTool.Start();
                pExifTool.BeginErrorReadLine();  //  This command starts the error handling, meaning ETErrorHandler() will now be called whenever ExifTool reports an error.
                pExifTool.WaitForExit();
                IsStarted = true;
            }
            else
            {
                pExifTool.StartInfo.Arguments = args;
                pExifTool.Start();
                pExifTool.WaitForExit();
            }

            Lines.Clear();
            string line;
            //List<string> lines = new List<string>();
            while ((line = pExifTool.StandardOutput.ReadLine()) != null)
            {
                Lines.Add(line);
            }

            // EXAMPLE OUTPUT 1: Error by setting a tag and the image file was not found.
            // exiftool.exe -S -GPSAltitude#=177 test.jpg
            // Error: File not found - test.jpg
            // 0 image files updated
            // 1 files weren't updated due to errors

            // EXAMPLE OUTPUT 2: Error by setting a tag and the value is not a number.
            // exiftool.exe -S -GPSAltitude#=aaa test.jpg
            // Warning: Error converting value for GPS:GPSAltitude(ValueConvInv)
            // Nothing to do.
            if (Lines.Count != 0 &&
                (Lines[0].StartsWith("Warning: Error") || Lines[0].StartsWith("Error:")))
                return false;            
            else
                return true;
        }

        /// <summary>
        /// The asynchronous error handler
        /// </summary>
        /// <param name="sendingProcess"></param>
        /// <param name="errLine"></param>
        private void ETErrorHandler(object sendingProcess, DataReceivedEventArgs errLine)
        {
            if (!string.IsNullOrEmpty(errLine.Data))
            {
                //  ...  do something with the information provided in errLine.Data...
                //Success = false;
                Lines[0] = "Error: " + errLine.Data;
            }
        }

        public static string DateTimeToString(DateTime? dt)
        {
            string s = string.Empty;
            if (dt.HasValue)
                s = "\"" + dt.Value.ToString("yyyy:MM:dd HH:mm:ss", CultureInfo.CurrentCulture) + "\"";

            return s;
        }
    }
}