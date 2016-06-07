using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace Picturez_Lib
{    
	/// <summary>
	/// 'StegHash' steganography filter. All rights are reserved. 
	/// Copyright © Picturez Project, http://picturez-project.de
	/// </summary>
	public class StegHashFilter : SteganographyFilter
    {
		protected List<int> charTriple;

        public StegHashFilter()
        {
			SupportedSrcPixelFormat = PixelFormatFlags.Color;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;

			charTriple = new List<int> ();
        }

		public override void FillLines(string[] pLines)
		{
			int index = 0;
			lines.Clear ();
			lines.AddRange(pLines);   
			charTriple.Clear ();

			foreach (string s in pLines)
			{
				string s2 = s + (char)endByte;
				// OUTER encryption
				s2 = AsciiTableCharMove.MovingBySubtracting(s2, endByte, asciiMoveValue);
				byte[] textInByte = AsciiTableCharMove.GetBytesFromString (s2);

				foreach (byte item in textInByte) {
					int one, ten, hundred;
					// INNER encryption by substracting hash element
					byte encryptedItem = (byte)(item - hash [index]);
					Fraction.Fractionalize3D (encryptedItem, out hundred, out ten, out one);
					charTriple.Add (hundred);
					charTriple.Add (ten);
					charTriple.Add (one);
					index = index == hashLengthMinus1 ? 0 : index + 1;
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
				int item = Fraction.Defractionalize3D (hundred, ten, one);
				// INNER decryption by adding hash element
				byte decryptedItem = (byte)(item + hash [indexKey]);
				char c = (char)decryptedItem;

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
			int distance, tenthousend, thousend, hundred, ten, one, rgbIndex;
            int position = 0, indexHash = 0;                              
            int w = srcData.Width;
            int h = srcData.Height;
            int stride = srcData.Stride;
            int offset = stride - w * ps;

			Fraction.Fractionalize5D (sumHashElements, out tenthousend, out thousend, out hundred, out ten, out one);
			int digitSumOfHashElements = tenthousend + thousend + hundred + ten + one;
			byte encryptValueForDistance = hash[digitSumOfHashElements];
			encryptValueForDistance = GetDigitSumOfByte (encryptValueForDistance);

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

				if (charTriple.Count > numberUsablePx || charTriple.Count == 0 || encryptValueForDistance >= distance)
                {
                    Success = false;
                    return;
                }

				#region Save Pin in startH line                
                // do encryption by substracting hash element
				int encryptedDistance = distance - encryptValueForDistance;
				Fraction.Fractionalize4D(encryptedDistance, out thousend, out hundred, out ten, out one);
				src = (byte*)srcData.Scan0.ToPointer();
				dst = (byte*)dstData.Scan0.ToPointer();
				src +=  startH * stride + startW * ps;
				dst +=  startH * stride + startW * ps;

				CalcRgbIndexFromHashArray(out rgbIndex, ref indexHash, hash);
				dst[rgbIndex] = ManipulateByte(src[rgbIndex], thousend);
				src += ps;
				dst += ps;
				CalcRgbIndexFromHashArray(out rgbIndex, ref indexHash, hash);
				dst[rgbIndex] = ManipulateByte(src[rgbIndex], hundred);
				src += ps;
				dst += ps;
				CalcRgbIndexFromHashArray(out rgbIndex, ref indexHash, hash);
				dst[rgbIndex] = ManipulateByte(src[rgbIndex], ten);
				src += ps;
				dst += ps;
				CalcRgbIndexFromHashArray(out rgbIndex, ref indexHash, hash);
				dst[rgbIndex] = ManipulateByte(src[rgbIndex], one);

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
						CalcRgbIndexFromHashArray(out rgbIndex, ref indexHash, hash);

						if (position != 0 || indexChar >= charTriple.Count)
                        {                            
                            continue;
                        }

						int intC = charTriple[indexChar];
						dst[rgbIndex] = ManipulateByte(src[rgbIndex], intC);
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
				CalcRgbIndexFromHashArray(out rgbIndex, ref indexHash, hash);
				thousend = src[rgbIndex];
				thousend = GetManipulatedByte(thousend);
				src += ps;
				CalcRgbIndexFromHashArray(out rgbIndex, ref indexHash, hash);
				hundred = src[rgbIndex];
				hundred = GetManipulatedByte(hundred);
				src += ps;
				CalcRgbIndexFromHashArray(out rgbIndex, ref indexHash, hash);
				ten = src[rgbIndex];
				ten = GetManipulatedByte(ten);
				src += ps;
				CalcRgbIndexFromHashArray(out rgbIndex, ref indexHash, hash);
				one = src[rgbIndex];
				one = GetManipulatedByte(one);
				distance = Fraction.Defractionalize4D(thousend, hundred, ten, one);
				// do decryption by adding hash element
				distance += encryptValueForDistance;

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
						CalcRgbIndexFromHashArray(out rgbIndex, ref indexHash, hash);

                        if (position != 0)
                        {
                            continue;
                        }

						int intC = src[rgbIndex];
						intC = GetManipulatedByte(intC);
						charTriple.Add(intC);                        
                    }

                    src += offset;
                }

				DecryptLines ();
            }
        }

		#region Private Helper functions

		private static void CalcRgbIndexFromHashArray(out int rgbIndex, ref int indexHash, byte[] hash)
		{
			rgbIndex = hash[indexHash] % 3;
			indexHash = indexHash < hash.Length - 1 ? indexHash + 1 : 0;
//			Console.WriteLine ("rgbIndex: " + rgbIndex);
		}

		private static int GetManipulatedByte(int sourceByte)
		{
			int hundred, ten, one;
			Fraction.Fractionalize3D (sourceByte, out hundred, out ten, out one);
			return one;
		}

		private static byte ManipulateByte(byte sourceByte, int number)
		{
			if (sourceByte >= 250 && number > 5) {
				sourceByte = 240;
			}
			int hundred, ten, one, destbyte;
			Fraction.Fractionalize3D (sourceByte, out hundred, out ten, out one);
			destbyte = Fraction.Defractionalize3D (hundred, ten, number);
			return (byte)destbyte;
		}

		#endregion
    }
}
