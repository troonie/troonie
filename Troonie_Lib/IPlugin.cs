using System.Collections.Generic;

namespace Troonie_Lib
{
	public interface IPlugin
	{
		void Start (string filename);
		void Start (List<string> filenames);
	}
}

