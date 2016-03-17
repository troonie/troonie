namespace Picturez_Lib
{
    using System;

    public static class AsciiTableCharMove
    {
        /// <summary>
        /// Own Encoding.ASCII.GetBytes() method, because original one does not 
        /// work correct for special characters (ä, ü, ...).
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static byte[] GetBytesFromString(string text)
        {
            Byte[] bytes = new byte[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                bytes[i] = (byte)text[i];
            }

            return bytes;
        }

		public static string MovingBySubtracting(string text, byte endByte, int value)
		{
			Byte[] bytes = GetBytesFromString(text);
			string result = string.Empty;

			for (int i = 0; i < bytes.Length; i++)
			{
				result += bytes[i] == endByte ? (char)bytes[i] : (char)(bytes[i] - value);
			}                        

			return result;            
		}

		public static string MovingByAdding(string text, int value)
        { 
			Byte[] bytes = GetBytesFromString(text);
			string result = string.Empty;

			for (int i = 0; i < bytes.Length; i++)
			{
				result += (char)(bytes[i] + value);
			}                        

			return result;  
        }
    }

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
