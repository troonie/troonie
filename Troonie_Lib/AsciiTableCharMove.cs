namespace Troonie_Lib
{
    using System;

    public static class AsciiTableCharMove
    {
		/// <summary>
		/// The code page of used encoding.
		/// https://docs.microsoft.com/de-de/dotnet/api/system.text.encoding.codepage
		/// Windows-1252 --> Western European (Windows)
		/// </summary>
		public const int CodePage = 1252;

		public static byte[] GetBytesFromString(string text)
        {
			return System.Text.Encoding.GetEncoding(CodePage).GetBytes (text);
		}

		public static string GetStringFromBytes(byte[] bytes)
		{
			return System.Text.Encoding.GetEncoding(CodePage).GetString (bytes);
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
