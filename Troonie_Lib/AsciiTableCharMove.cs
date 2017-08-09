namespace Troonie_Lib
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

		public static string GetStringFromBytes(byte[] bytes)
		{
			string s = string.Empty;

			for (int i = 0; i < bytes.Length; i++)
			{				
				char c = (char)bytes[i];
				s += c;
			}

			return s;
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
}
