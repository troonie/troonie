namespace Picturez_Lib
{
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.Security.Cryptography;
    
	/// <summary>
	/// Steganography filter. All rights are reserved. 
	/// Copyright © Picturez Project
	/// </summary>
	public class SteganographyFilter2 : SteganographyFilter
    {
        public SteganographyFilter2()
        {
			SupportedSrcPixelFormat = PixelFormatFlags.Color;
			SupportedDstPixelFormat = PixelFormatFlags.SameLikeSource;

//			lines = new List<string>();
//			Key = string.Empty;
        }

		private void Fractionalize(int number, out int two, out int one)
		{
			if (number > 99)
				throw new ArgumentException ("Number cannot be larger than 99.", "number");

//			int three = (int)(number / 100);
//			int two = (int)((number - (three * 100)) / 10);
//			int one = (int)(number - three * 100 - two * 10);

			two = (int)(number / 10);
			one = (int)(number - two * 10);
		}

		private int Defractionalize(int two, int one)
		{
			return (two * 10 + one);
		}

		protected override internal unsafe void Process(BitmapData srcData, BitmapData dstData)
        {
			const int ps = 4;     
			const int numberOfPinChar = 2;

            byte[] pwSha = EncryptKey(Key, false);
            byte[] pwMurmur = EncryptKey(Key, true);
            byte[] hash = GetHashByShaAndMurmur(pwSha, pwMurmur);
			hash = GetQuerSumOfBytes(hash);
			int hashLengthMinus1 = hash.Length - 1;
			int sumHashElements = GetSumOfElements (hash); 
//			Console.WriteLine ("sumHashElements: " + sumHashElements);
			int distance;
            int position = 0;            
            int indexKey = 0;          
            int w = srcData.Width;
            int h = srcData.Height;
            int stride = srcData.Stride;
            int offset = stride - w * ps;

			int startH = sumHashElements / w;
			int startW = sumHashElements - w * startH;
			// check and correct special startW border position
			if (startW >= w - numberOfPinChar) {
				startW -= numberOfPinChar;
			} 

            byte* src = (byte*)srcData.Scan0.ToPointer();      
			byte* dst = (byte*)dstData.Scan0.ToPointer();

			#region clone bitmap
			Random r = new Random();
//			long avg0 = 0;
			// for each line
			for (int y = 0; y < h; y++)
			{
				// for each pixel
				for (int x = 0; x < w; x++, src += ps, dst += ps)
				{
					dst[RGBA.R] = src[RGBA.R];
					dst[RGBA.G] = src[RGBA.G];
					dst[RGBA.B] = src[RGBA.B];
					dst[RGBA.A] = src[RGBA.A];

					if (WritingMode)
					{                        
						// original: dst[RGBA.A] = (byte)r.Next(200, 255);
						dst[RGBA.A] = (byte)r.Next(100, 255);
//						avg0 += dst[RGBA.A];

//						RGB rgb = new RGB(dst[RGBA.R], dst[RGBA.G], dst[RGBA.B]);
//						ConvertColorspace(rgb);
//						ColorTriple ct = colorspace.To<RGB>().Color;
//						dst[RGBA.R] = (byte) ct.A;
//						dst[RGBA.G] = (byte) ct.B;
//						dst[RGBA.B] = (byte) ct.C;
					}
				}
				src += offset;
				dst += offset;
			}
//			avg0 = avg0 / (w * h);
//			Console.WriteLine ("avg0= " + avg0);
			#endregion clone bitmap
           
            if (WritingMode)
            {
//				long avg1 = 0;
				int indexChar = 0;
				// -w as one line reserved for saving distance value
				int numberUsablePx = w * h - sumHashElements - w; 
//				distance = Math.Min(numberUsablePx / charCount - 1, 9999); // old
				distance = numberUsablePx / charCount - 1;
				while (distance > 25599) {
					distance /= 10; 
				}

				if (charCount > numberUsablePx || charCount == 0)
                {
                    Success = false;
                    return;
                }

				#region Save Pin in startH line                
                int distancePart1 = distance / 100;
                int distancePart2 = distance - distancePart1 * 100;
                // do encryption by substracting hash element
                distancePart1 -= hash[0];
                distancePart2 -= hash[1];
				dst = (byte*)dstData.Scan0.ToPointer();
				dst +=  startH * stride + startW * ps;

//				SetValue(dst, distancePart1);
                dst[RGBA.A] = (byte)(255 - distancePart1);
//				dst[RGBA.G] = 255;
                dst += ps;
//				SetValue(dst, distancePart2);
                dst[RGBA.A] = (byte)(255 - distancePart2);
//				dst[RGBA.G] = 255;
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
					     position = position < distance ? position + 1 : 0,
					     indexKey = indexKey == hashLengthMinus1 ? 0 : indexKey + 1)
                    {
                        if (position != 0 || indexChar >= linesAsString.Length)
                        {                            
                            continue;
                        }

                        char c = linesAsString[indexChar];
                        int intC = c;
						// INNER encryption by substracting hash element
						intC -= hash [indexKey];

						byte byteC = (byte)(255 - intC);                           
						dst [RGBA.A] = byteC;
//						dst [RGBA.R] = 255;
//						avg1 += byteC;

						indexChar++;
                    }

					src += offset;
					dst += offset;
                }
                
                Success = true;

//				avg1 = avg1 / linesAsString.Length;
//				Console.WriteLine ("avg1= " + avg1);
            }
            else // read-in mode
            {
                #region Read out distance in StartH line
				src = (byte*)srcData.Scan0.ToPointer();
				src +=  startH * stride + startW * ps;
//				GetValue(src);
				byte pinPart1 = (byte)Math.Abs(src[RGBA.A] - 255);
                // do decryption by adding hash element
                pinPart1 += hash[0];
				src += ps;
//				GetValue(src);
				byte pinPart2 = (byte)Math.Abs(src[RGBA.A] - 255);
                pinPart2 += hash[1];
                distance = pinPart1 * 100 + pinPart2;  
				startH++; // one line reserved for saving distance value
				#endregion Read out distance in StartH line 

                string tmp = string.Empty;

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
                        position = position < distance ? position + 1 : 0,
					     indexKey = indexKey == hashLengthMinus1 ? 0 : indexKey + 1)
                    {
                        if (position != 0)
                        {
                            continue;
                        }

                        byte byteC = src[RGBA.A];
                        byteC = (byte)Math.Abs(byteC - 255);
                        // INNER decryption by adding hashArray element
                        byteC += hash[indexKey];
                        char c = (char)byteC;

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

                    src += offset;
                }
            }
        }
    }
}
