namespace Picturez_Lib
{
    using System;

    public static class AsciiInvertCharEncryption
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

        public static string Process(string text, int mod)
        {
            Byte[] bytes = GetBytesFromString(text);
            string result = "";
            
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] != 59 && mod == 0) // 59 == ASCII for ';'
                {
                    bytes[i] = (byte) (bytes[i] - 2);
                }
                else if (bytes[i] != 57 && mod == 1) // 57+2=59 == ASCII for ';'
                {
                    bytes[i] = (byte)(bytes[i] + 2);
                }
                result += (char)bytes[i];
            }                        

            return result;            
        }
    }

    public static class Rc4
    {
        public static string Process(string text, Byte[] key)
        {
            Byte[] bytes = AsciiInvertCharEncryption.GetBytesFromString(text);

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
