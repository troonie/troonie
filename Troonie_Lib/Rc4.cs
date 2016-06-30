using System;

namespace Troonie_Lib
{
	public static class Rc4
	{
		public static string Process(string text, Byte[] key)
		{
			Byte[] bytes = AsciiTableCharMove.GetBytesFromString(text);

			Byte[] s = new Byte[256];
			Byte[] k = new Byte[256];
			Byte temp;
			int i;

			for (i = 0; i < 256; i++)
			{
				s[i] = (Byte)i;
				k[i] = key[i % key.GetLength(0)];
			}

			int j = 0;
			for (i = 0; i < 256; i++)
			{
				j = (j + s[i] + k[i]) % 256;
				temp = s[i];
				s[i] = s[j];
				s[j] = temp;
			}

			i = j = 0;
			for (int x = 0; x < bytes.GetLength(0); x++)
			{
				i = (i + 1) % 256;
				j = (j + s[i]) % 256;
				temp = s[i];
				s[i] = s[j];
				s[j] = temp;
				int t = (s[i] + s[j]) % 256;
				bytes[x] ^= s[t];
			}


			string result = "";
			foreach (byte b in bytes)
			{
				result += (char)b;
			}

			return result;
		}
	}
}

