using System.Reflection;
using System.Runtime.CompilerServices;

// Information about this assembly is defined by the following attributes. 
// Change them to the values specific to your project.
[assembly: AssemblyTitle (Version.AssemblyTitle)]
[assembly: AssemblyDescription (Version.AssemblyTitle)]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("www.troonie.com")]
[assembly: AssemblyProduct (Version.AssemblyProduct)]
[assembly: AssemblyCopyright ("Troonie Project")]
[assembly: AssemblyTrademark ("www.troonie.com")]
[assembly: AssemblyCulture ("")]
// The assembly version has the format "{Major}.{Minor}.{Build}.{Revision}".
// The form "{Major}.{Minor}.*" will automatically update the build and revision,
// and "{Major}.{Minor}.{Build}.*" will update just the revision.
[assembly: AssemblyVersion (Version.Number)]
// The following attributes are used to specify the signing key for the assembly, 
// if desired. See the Mono documentation for more information about signing.
//[assembly: AssemblyDelaySign(false)]
//[assembly: AssemblyKeyFile("")]

public class Version
{
	public const string Number = "1.1.0";
	public const string AssemblyTitle = "Troonie Installer for Windows 10, 8.1 and 7";
	public const string AssemblyProduct = "Troonie 1.1.0 beta";
}
