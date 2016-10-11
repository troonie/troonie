namespace Troonie_Lib {	public partial class Version { public const string VERSION = "1.0.4"; } }

namespace Troonie_Lib 
{
	using System.IO;

	public partial class Version 
	{ 
		public static void SetNewVersionNumberInAllFiles(string newVersion)
		{			
			DirectoryInfo di = new DirectoryInfo (Constants.I.EXEPATH);

			string WinInstaller_AssemblyInfo_cs = di.Parent.Parent.Parent.ToString() + Path.DirectorySeparatorChar + 
						"WinInstaller" + Path.DirectorySeparatorChar + 
						"Properties" + Path.DirectorySeparatorChar + "AssemblyInfo.cs";
			string s = IOFile.I.ReadLine (WinInstaller_AssemblyInfo_cs, 25);
			s = s.Replace (VERSION, newVersion);
			IOFile.I.WriteLine (WinInstaller_AssemblyInfo_cs, 25, s, true);
			s = IOFile.I.ReadLine (WinInstaller_AssemblyInfo_cs, 27);
			s = s.Replace (VERSION, newVersion);
			IOFile.I.WriteLine (WinInstaller_AssemblyInfo_cs, 27, s, true);


			string README_md = di.Parent.Parent.Parent.ToString() + Path.DirectorySeparatorChar + "README.md";
			s = IOFile.I.ReadLine (README_md, 1);
			s = s.Replace (VERSION, newVersion);
			IOFile.I.WriteLine (README_md, 1, s, true);


			// has to be changed last
			string Version_cs = di.Parent.Parent.Parent.ToString() + Path.DirectorySeparatorChar + 
				"Troonie_Lib" + Path.DirectorySeparatorChar + "Version.cs";
			s = IOFile.I.ReadLine (Version_cs, 1);
			s = s.Replace (VERSION, newVersion);
			IOFile.I.WriteLine (Version_cs, 1, s, true);
		}
	} 
}