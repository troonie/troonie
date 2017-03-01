using System;

namespace Troonie_Lib
{
	public static class Fraction
	{
		public static void Fractionalize5D(int number, out int tenthousend, out int thousend, out int hundred, out int ten, out int one)
		{
			if (number > 99999)
				throw new ArgumentException ("Number cannot be larger than 9999.", "number");

			tenthousend = (int)(number / 10000);
			thousend = (int)((number - tenthousend * 10000) / 1000);
			hundred = (int)((number - tenthousend * 10000 - thousend * 1000) / 100);
			ten = (int)((number - tenthousend * 10000 - thousend * 1000 - hundred * 100) / 10);
			one = (int)(number - tenthousend * 10000 - thousend * 1000 - hundred * 100 - ten * 10);
		}

		public static void Fractionalize4D(int number, out int thousend, out int hundred, out int ten, out int one)
		{
			if (number > 9999)
				throw new ArgumentException ("Number cannot be larger than 9999.", "number");

			thousend = (int)(number / 1000);
			hundred = (int)((number - thousend * 1000) / 100);
			ten = (int)((number - thousend * 1000 - hundred * 100) / 10);
			one = (int)(number - thousend * 1000 - hundred * 100 - ten * 10);
		}

		public static int Defractionalize4D(int thousend, int hundred, int ten, int one)
		{
			return (thousend * 1000 + hundred * 100 + ten * 10 + one);
		}

		public static void Fractionalize3D(int number, out int hundred, out int ten, out int one)
		{
			if (number > 999)
				throw new ArgumentException ("Number cannot be larger than 999.", "number");

			hundred = (int)(number / 100);
			ten = (int)((number - (hundred * 100)) / 10);
			one = (int)(number - hundred * 100 - ten * 10);
		}

		public static int Defractionalize3D(int hundred, int ten, int one)
		{
			return (hundred * 100 + ten * 10 + one);
		}

		public static void Fractionalize2D(int number, out int ten, out int one)
		{
			if (number > 99)
				throw new ArgumentException ("Number cannot be larger than 99.", "number");

			ten = (int)(number / 10);
			one = (int)(number - ten * 10);
		}

		public static int Defractionalize2D(int ten, int one)
		{
			return (ten * 10 + one);
		}

		public static byte DigitSumOfByte(byte b)
		{
			int three = (int)(b / 100);
			int two = (int)((b - (three * 100)) / 10);
			int one = (int)(b - three * 100 - two * 10);
			b = (byte)(three + two + one);

			return b;
		}
	}
}

