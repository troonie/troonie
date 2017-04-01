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

		public int Write(Bitmap source, string key, string text)
		{
			return 0;
		}

		public int Write(Bitmap source, string key, byte[] bytes)
		{
			return 0;
		}

		public void Read(Bitmap source, string key, out string text)
		{
			text = null;
		}	

		public void Read(Bitmap source, string key, out byte[] bytes)
		{
			bytes = null;
		}
	}

    public class BitStegRGB : BitSteg
	{

    }
}

