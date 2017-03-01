using System;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Collections;

namespace Troonie_Lib
{
	public class BitSteg
	{
		public const bool SHOW_IN_GUI = false;

		public unsafe bool Write(Bitmap source, string key, string[] pLines)
		{
			return true;
		}

		public unsafe void Read(Bitmap source, string key, out string[] lines)
		{
			lines = null;
		}			
	}
}

