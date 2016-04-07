using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Picturez_Lib
{    
	/// <summary>
	/// Steganography filter. All rights are reserved. 
	/// Copyright © Picturez Project
	/// </summary>
	public class SteganographyFilter2 : SteganographyFilter
    {
		protected List<int> charTriple;

        public SteganographyFilter2()
        {
			SupportedSrcPixelFormat = PixelFormatFlags.Color;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;

			charTriple = new List<int> ();
        }

		public override void FillLines(string[] pLines)
		{
			int indexKey = 0;
			lines.Clear ();
			lines.AddRange(pLines);   
			charTriple.Clear ();

			foreach (string s in pLines)
			{
				string s2 = s + (char)endByte;
				// OUTER encryption
				s2 = AsciiTableCharMove.MovingBySubtracting(s2, endByte, asciiMoveValue);
				byte[] byteArray = AsciiTableCharMove.GetBytesFromString (s2);

				foreach (byte item in byteArray) {
					int one, ten, hundred;
					// INNER encryption by substracting hash element
					byte itemEncrypted = (byte)(item - hash [indexKey]);
					Fractionalize3D (itemEncrypted, out hundred, out ten, out one);
					charTriple.Add (hundred);
					charTriple.Add (ten);
					charTriple.Add (one);
					indexKey = indexKey == hashLengthMinus1 ? 0 : indexKey + 1;
				}
			}
		}

		protected void DecryptLines()
		{
			int hundred, ten, one;
			string tmp = string.Empty;
			lines.Clear ();

			for (int i = 0, indexKey = 0; 
			     i < charTriple.Count - 2; 
			     i+=3, indexKey = indexKey == hashLengthMinus1 ? 0 : indexKey + 1) {
				hundred = charTriple [i];
				ten = charTriple [i+1];
				one = charTriple [i+2];
				int item = Defractionalize3D (hundred, ten, one);
				// INNER decryption by adding hash element
				byte itemEncrypted = (byte)(item + hash [indexKey]);
				char c = (char)itemEncrypted;

				if (c == (char)endByte)
				{
					// OUTER decryption
					tmp = AsciiTableCharMove.MovingByAdding(tmp, asciiMoveValue);
					lines.Add(tmp);
					tmp = string.Empty;
				}
				else
				{
					tmp += c;
				}
			}
		}

		protected override internal unsafe void Process(BitmapData srcData, BitmapData dstData)
        {
			int ps = Image.GetPixelFormatSize(srcData.PixelFormat) / 8;     
			const int numberOfPinChar = 4;
			int distance, tenthousend, thousend, hundred, ten, one;
            int position = 0;                              
            int w = srcData.Width;
            int h = srcData.Height;
            int stride = srcData.Stride;
            int offset = stride - w * ps;

			Fractionalize5D (sumHashElements, out tenthousend, out thousend, out hundred, out ten, out one);
			int querSumOfHashElements = tenthousend + thousend + hundred + ten + one;
			byte hashDistance = hash[querSumOfHashElements];
			hashDistance = GetQuerSumOfByte (hashDistance);

			int startH = sumHashElements / w;
			int startW = sumHashElements - w * startH;
			// check and correct special startW border position
			if (startW >= w - numberOfPinChar) {
				startW -= numberOfPinChar;
			} 

            byte* src = (byte*)srcData.Scan0.ToPointer();      
			byte* dst = (byte*)dstData.Scan0.ToPointer();

			#region clone bitmap
			// for each line
			for (int y = 0; y < h; y++)
			{
				// for each pixel
				for (int x = 0; x < w; x++, src += ps, dst += ps)
				{
					dst[RGBA.R] = src[RGBA.R];
					dst[RGBA.G] = src[RGBA.G];
					dst[RGBA.B] = src[RGBA.B];
					// alpha, 32 bit
					if (ps == 4) {
						dst [RGBA.A] = src [RGBA.A];
					}
				}
				src += offset;
				dst += offset;
			}
			#endregion clone bitmap
           
            if (WritingMode)
            {
//				long avg1 = 0;
				int indexChar = 0;
				// -w as one line reserved for saving distance value
				int numberUsablePx = w * h - sumHashElements - w; 
				distance = numberUsablePx / charTriple.Count - 1;
				while (distance > 9999) {  // old: 25599
					distance = (int)(distance / Math.PI);
				}

				if (charTriple.Count > numberUsablePx || charTriple.Count == 0 || hashDistance >= distance)
                {
                    Success = false;
                    return;
                }

				#region Save Pin in startH line                
                // do encryption by substracting hash element
				int encrpytedDistance = distance - hashDistance;
				Fractionalize4D(encrpytedDistance, out thousend, out hundred, out ten, out one);
				src = (byte*)srcData.Scan0.ToPointer();
				dst = (byte*)dstData.Scan0.ToPointer();
				src +=  startH * stride + startW * ps;
				dst +=  startH * stride + startW * ps;

				dst[RGBA.R] = ManipulateByte(src[RGBA.R], thousend);
				dst += ps;
				dst[RGBA.G] = ManipulateByte(src[RGBA.G], hundred);
				dst += ps;
				dst[RGBA.B] = ManipulateByte(src[RGBA.B], ten);
				dst += ps;
				dst[RGBA.R] = ManipulateByte(src[RGBA.R], one);

				startH++; // One line reserved for saving distance value
				#endregion Save Pin in startH line
                
                src = (byte*)srcData.Scan0.ToPointer();
                dst = (byte*)dstData.Scan0.ToPointer();
				src += startH * stride + startW * ps;
				dst += startH * stride + startW * ps;

                // for each line
				for (int y = startH; y < h; y++)
                {
					int startX = 0;
					// when starting, set to correct start in line, otherwise it starts at pos 0
					if (y == startH) {
						startX = startW;
					}

                    // for each pixel
					for (int x = startX; x < w;
                        x++,
                        src += ps,
                        dst += ps,                        
					    position = position < distance ? position + 1 : 0)
                    {
						if (position != 0 || indexChar >= charTriple.Count)
                        {                            
                            continue;
                        }

						if (indexChar == 60)
							Console.WriteLine ("indexChar == 60");
						int intC = charTriple[indexChar];
						dst[RGBA.R] = ManipulateByte(src[RGBA.R], intC);
						indexChar++;
                    }

					src += offset;
					dst += offset;
                }
                
                Success = true;
            }
            else // read-in mode
            {
                #region Read out distance in StartH line

				src = (byte*)srcData.Scan0.ToPointer();
				src +=  startH * stride + startW * ps;
				thousend = (byte)Math.Abs(src[RGBA.R]);
				thousend = GetManipulatedByte(thousend);
				src += ps;
				hundred = (byte)Math.Abs(src[RGBA.G]);
				hundred = GetManipulatedByte(hundred);
				src += ps;
				ten = (byte)Math.Abs(src[RGBA.B]);
				ten = GetManipulatedByte(ten);
				src += ps;
				one = (byte)Math.Abs(src[RGBA.R]);
				one = GetManipulatedByte(one);
				distance = Defractionalize4D(thousend, hundred, ten, one);
				// do decryption by adding hash element
				distance += hashDistance;

				startH++; // One line reserved for saving distance value

				#endregion Read out distance in StartH line 

				charTriple.Clear ();
				src = (byte*)srcData.Scan0.ToPointer();
				src += startH * stride + startW * ps;

                // for each line
				for (int y = startH; y < h; y++)
                {                 
					int startX = 0;
					// when starting, set to correct start in line, otherwise it starts at pos 0
					if (y == startH) {
						startX = startW;
					}

                    // for each pixel
					for (int x = startX; x < w;
                        x++,
                        src += ps,
                        position = position < distance ? position + 1 : 0)
                    {
                        if (position != 0)
                        {
                            continue;
                        }

						int intC = src[RGBA.R];
						intC = GetManipulatedByte(intC);
						charTriple.Add(intC);                        
                    }

                    src += offset;
                }

				DecryptLines ();
            }
        }

		#region Private Helper functions

		private static int GetManipulatedByte(int sourceByte)
		{
			int hundred, ten, one;
			Fractionalize3D (sourceByte, out hundred, out ten, out one);
			return one;
		}

		private static byte ManipulateByte(byte sourceByte, int number)
		{
			if (sourceByte >= 250) {
				sourceByte = 240;
			}
			int hundred, ten, one, destbyte;
			Fractionalize3D (sourceByte, out hundred, out ten, out one);
			destbyte = Defractionalize3D (hundred, ten, number);
			return (byte)destbyte;
		}

		private static void Fractionalize5D(int number, out int tenthousend, out int thousend, out int hundred, out int ten, out int one)
		{
			if (number > 99999)
				throw new ArgumentException ("Number cannot be larger than 9999.", "number");

			tenthousend = (int)(number / 10000);
			thousend = (int)((number - tenthousend * 10000) / 1000);
			hundred = (int)((number - tenthousend * 10000 - thousend * 1000) / 100);
			ten = (int)((number - tenthousend * 10000 - thousend * 1000 - hundred * 100) / 10);
			one = (int)(number - tenthousend * 10000 - thousend * 1000 - hundred * 100 - ten * 10);
		}

		private static void Fractionalize4D(int number, out int thousend, out int hundred, out int ten, out int one)
		{
			if (number > 9999)
				throw new ArgumentException ("Number cannot be larger than 9999.", "number");

			thousend = (int)(number / 1000);
			hundred = (int)((number - thousend * 1000) / 100);
			ten = (int)((number - thousend * 1000 - hundred * 100) / 10);
			one = (int)(number - thousend * 1000 - hundred * 100 - ten * 10);
		}

		private static int Defractionalize4D(int thousend, int hundred, int ten, int one)
		{
			return (thousend * 1000 + hundred * 100 + ten * 10 + one);
		}

		private static void Fractionalize3D(int number, out int hundred, out int ten, out int one)
		{
			if (number > 999)
				throw new ArgumentException ("Number cannot be larger than 999.", "number");

			hundred = (int)(number / 100);
			ten = (int)((number - (hundred * 100)) / 10);
			one = (int)(number - hundred * 100 - ten * 10);
		}

		private static int Defractionalize3D(int hundred, int ten, int one)
		{
			return (hundred * 100 + ten * 10 + one);
		}

		private static void Fractionalize2D(int number, out int ten, out int one)
		{
			if (number > 99)
				throw new ArgumentException ("Number cannot be larger than 99.", "number");

			ten = (int)(number / 10);
			one = (int)(number - ten * 10);
		}

		private static int Defractionalize2D(int ten, int one)
		{
			return (ten * 10 + one);
		}

		#endregion
    }
}
