using System;
using System.Diagnostics;
using System.IO;

namespace Troonie_starter
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			string arg = string.Empty;
			for (var i = 0; i < args.Length; i++)
			{
				arg += "\"" + args[i] + "\" ";
			}

//			Console.WriteLine ("Hello World!");
			using (Process proc = new Process ()) 
			{
				try {
				proc.StartInfo.FileName = "Troonie" + Path.DirectorySeparatorChar + "Troonie.exe";
				proc.StartInfo.Arguments = arg; 
				proc.StartInfo.UseShellExecute = false; 
				proc.StartInfo.CreateNoWindow = true;
				//					proc.StartInfo.RedirectStandardOutput = true;
				//					proc.StartInfo.RedirectStandardError = true;
				proc.Start();
				//					proc.WaitForExit();
				proc.Close();
				}
				catch (Exception ex) {
					Console.WriteLine (ex.Message);
				}
			}
		}
	}
}
