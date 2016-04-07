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
    public class SteganographyFilter : AbstractFilter
    {
        /// <summary>
        /// Determines whether the <see cref="Steganography"/> algorithm reads 
        /// out or writes into the image.
        /// </summary>
        public bool WritingMode { get; set; }

        /// <summary>
		/// Determines whether the steganography writing process was successful or not.
        /// </summary>
        public bool Success { get; protected set; }

		private string key;
        /// <summary>
		/// The key to encrypt the text.
		/// Default value (set in constructor): "Steganography". 
		/// </summary>
		public string Key {
			get {
				return key;
			}
			set {
				key = value;
				asciiMoveValue = GetQuerSumOfByte((byte)value.Length);
				hash = GetCryptedHash (value, !(this is SteganographyFilter2));
				hashLengthMinus1 = hash.Length - 1;
				sumHashElements = GetSumOfElements (hash); 
			}
		}

		/// <summary>
		/// Minimum range of alpha value by filling alpha channels with random values.
		/// Default (and most secure) value: 100
		/// </summary>
		public int MinAlphaValue { get; set; }

		/// <summary>Final byte, added in the end of a line. Signals line wrapping. </summary>
		protected const byte endByte = 126; // '~' 
		protected byte asciiMoveValue; 
		protected byte[] hash;
		protected int hashLengthMinus1;
		protected int sumHashElements; 
		protected string linesAsString;

		protected int charCount;

        #region lines properties

		protected List<string> lines;

        public string[] GetLines()
        {
            return lines.ToArray();
        }

        public virtual void FillLines(string[] pLines)
        {
			charCount = 0;
			linesAsString = string.Empty;
			lines.Clear ();
            lines.AddRange(pLines);            
            
            foreach (string s in pLines)
            {
				string s2 = s + (char)endByte;
				s2 = AsciiTableCharMove.MovingBySubtracting(s2, endByte, asciiMoveValue);

                linesAsString += s2;
                charCount += s2.Length;                
            }
        }

        #endregion lines properties

        public SteganographyFilter()
        {
			SupportedSrcPixelFormat = PixelFormatFlags.Format32All;
			SupportedDstPixelFormat = PixelFormatFlags.Format32BppArgb;

			lines = new List<string>();
			Key = string.Empty;
			MinAlphaValue = 100;
        }

		protected override void SetProperties (double[] filterProperties){	}

		protected override internal unsafe void Process(BitmapData srcData, BitmapData dstData)
        {
			const int ps = 4;     
			const int numberOfPinChar = 2;

//            byte[] pwSha = EncryptKey(Key, false);
//            byte[] pwMurmur = EncryptKey(Key, true);
//            byte[] hash = GetHashByShaAndMurmur(pwSha, pwMurmur);
//			hash = GetQuerSumOfBytes(hash);
//			int hashLengthMinus1 = hash.Length - 1;
//			int sumHashElements = GetSumOfElements (hash); 
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
						dst[RGBA.A] = (byte)r.Next(MinAlphaValue, 255);
//						avg0 += dst[RGBA.A];
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
                dst[RGBA.A] = (byte)(255 - distancePart1);
//				dst[RGBA.G] = 255;
                dst += ps;
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
				byte pinPart1 = (byte)Math.Abs(src[RGBA.A] - 255);
                // do decryption by adding hash element
                pinPart1 += hash[0];
				src += ps;
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

        #region private helper methods

		protected static int GetSumOfElements(byte[] array)
		{
			int sum = 0;
			foreach (byte item in array) {
				sum += item;
			}

			return sum;
		}

		protected static byte[] GetHashByShaAndMurmur(byte[] pwSha, byte[] pwMurmur)
        {
//            SHA256 sha256 = new SHA256CryptoServiceProvider();
			SHA512 sha512 = new SHA512Managed();			       
            byte[] hashArray = sha512.ComputeHash(pwSha);
            sha512.Clear();

			uint seedMurmur = 0;
			foreach (byte b in pwMurmur)
			{
				seedMurmur += b;
			}
            Murmur3 m = new Murmur3(seedMurmur);
            byte[] murmur = m.ComputeHash(pwMurmur);
            // m = null;
                        
			for (int i = 0, j = 0; i < hashArray.Length; i++, j++ )
            {
                if (j == murmur.Length)
                {
                    j = 0;
                }
                hashArray[i] = (byte) (hashArray[i] + murmur[j]);
            }

            return hashArray;
        }

		protected static byte[] GetCryptedHash(string key, bool asQuerSum)
		{
			byte[] pwSha = EncryptKey(key, false);
			byte[] pwMurmur = EncryptKey(key, true);
			byte[] hash = GetHashByShaAndMurmur(pwSha, pwMurmur);
			if (asQuerSum) {
				hash = GetQuerSumOfBytes (hash);
			}
			return hash;
		}

		protected static byte GetQuerSumOfByte(byte b)
		{
			int three = (int)(b / 100);
			int two = (int)((b - (three * 100)) / 10);
			int one = (int)(b - three * 100 - two * 10);
			b = (byte)(three + two + one);

			return b;
		}

		protected static byte[] GetQuerSumOfBytes(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
				array[i] = GetQuerSumOfByte(array[i]);
            }

            return array;
        }
        
		/// <summary>
		/// (Simple) Encrypts the passed key. Two simple algorithms are available. 
		/// If parameter useVersion2 is set to true, algorithm 2 is used. Otherwise algorithm 1.
		/// </summary>
		protected static byte[] EncryptKey(string key, bool useAlgorithm2)
        {
            double e = Math.E;
            double pi = Math.PI;

            if (!useAlgorithm2)
            {
                e = e * pi * pi;
                pi = pi * e * e;
            }

            byte[] keyBytes = AsciiTableCharMove.GetBytesFromString(key);
            double t1 = 0;
            double t2 = 0;
            double t3 = 0;
            for (int i = 0; i < keyBytes.Length; i++)
            {
                t1 -= keyBytes[i] * e;
                t2 -= keyBytes[i] * pi;
                t3 += t1 + t2;

                if (t1 < 0)
                {
                    t1 = Math.Abs(t1);
                }

                if (t2 < 0)
                {
                    t2 = Math.Abs(t2);
                }
            }

            // t = 1 / t;
            double t4 = t1 + t2+ t3;
            t4 = t4 * t4;
            while (t4 > 10)
            {
                t4 = Math.Sqrt(t4);
            }

            const int n = 24;
            byte[] result = new byte[n + keyBytes.Length];
            for (int i = 0; i < n; i++)
            {
                t4 -= (int) (t4);
                t4 *= 100;
                byte byteT = (byte)(t4);
                result[i] = byteT;
            }

            for (int i = 0; i < keyBytes.Length; i++)
            {
                result[n + i] = useAlgorithm2 ? keyBytes[i] : result[i];    
            }

            return result;
        }

        #endregion private helper methods
    }
}
