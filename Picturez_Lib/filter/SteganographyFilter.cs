namespace Picturez_Lib
{
//    using Helper;
    using System;
    using System.Collections.Generic;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.Security.Cryptography;
    
    /// <summary>
    /// The filter grayscales colored and images.
    /// </summary>
    public class SteganographyFilter : AbstractFilter
    {
        /// <summary>
        /// Determines whether the <see cref="Steganography"/> algorithm reads 
        /// out or writes into the image.
        /// </summary>
        public bool WritingMode { get; set; }

        /// <summary>
        /// Determines whether the writing process was successful or not.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// The key to <see cref="SHA256"/>-encrypt the text.
        /// Default value (set in constructor): "Steganography". 
        /// </summary>
        public string Key { get; set; }

		// 126 == ~
		// 32 == SP
		const byte endByte = 32; 
        private string linesAsString;
        private int linesCount;

        #region lines properties
        /// <summary>
        /// The text in form of lines to (de-)code.
        /// </summary>
        private List<string> lines;

//        public void InitLines()
//        {
//            lines = new List<string>();
//        }

        public string[] GetLines()
        {
            return lines.ToArray();
        }

        public void FillLines(string[] pLines)
        {
            lines.AddRange(pLines);
            linesCount = 0;
            linesAsString = "";
            foreach (string s in pLines)
            {
				string s2 = s + (char)endByte; // "~";
                // TODO: OUTER encryption by using RC4
                s2 = AsciiInvertCharEncryption.Process(s2, endByte, 0);

                linesAsString += s2;
                linesCount += s2.Length;                
            }
        }
        #endregion lines properties

        /// <summary>
        ///  The used distance of two characters.
        /// </summary>
        private int Pin { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Steganography"/> class.
        /// </summary>
        public SteganographyFilter()
        {
			// TODO: Maybe allowing also 24 bit images?
			SupportedSrcPixelFormat = PixelFormatFlags.Format32All;
			SupportedDstPixelFormat = PixelFormatFlags.Format32BppArgb;

			lines = new List<string>();
//            ResetKey();
			Key = "Steganography";
        }

//        public void ResetKey()
//        {
//            Key = "Steganography";
//        }

		protected override internal unsafe void Process(BitmapData srcData, BitmapData dstData)
        {
//            EndlessProgressBarFormInThread form = 
//                new EndlessProgressBarFormInThread("Processing ... ", 
//                    "Please wait, this takes a view seconds.");
//            form.Start();

            byte[] pwSha = GetPassword(Key, false);
            byte[] pwMurmur = GetPassword(Key, true);
            byte[] hashArray = GetHashArray(pwSha, pwMurmur);
            Random r = new Random();
            int pinPosition = 0;
            int indexChar = 0;
            int indexKey = 0;

            const int ps = 4;            
            int w = srcData.Width;
            int h = srcData.Height;
            int borderDistInPercent = GetborderDist(w, h);
            int stride = srcData.Stride;
            int offset = stride - w * ps;

            int distW = (int)(w * borderDistInPercent / 100.0f);
            int distH = (int)(h * borderDistInPercent / 100.0f);

            byte* src = (byte*)srcData.Scan0.ToPointer();
            byte* dst = (byte*)dstData.Scan0.ToPointer();

            // clone bitmap to provide an (almost, but with coded text) 
            // identical result image.
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
                        dst[RGBA.A] = (byte)r.Next(180, 255);
                    }
                }
                src += offset;
                dst += offset;
            }
           
            if (WritingMode)
            {
                int countY = h - 2 * distH;
                int countX = w - 2 * distW;
                // -1 to disable overflow
                int countXY = countX * countY - 1;

                if (linesCount > countXY || linesCount == 0)
                {
                    Success = false;
//                    // close form
//                    form.End();
                    return;
                }

                Pin = Math.Min(countXY / linesCount, 9999);                

                #region Save Pin in first line
                dst = (byte*)dstData.Scan0.ToPointer();
                // TODO save PIN
                int pinPart1 = Pin / 100;
                int pinPart2 = Pin - pinPart1 * 100;
                // do encryption by substracting hashArray element
                pinPart1 -= hashArray[0];
                pinPart2 -= hashArray[1];
                // set PIN position at half of image width
                dst +=  (w / 2) * ps;
                dst[RGBA.A] = (byte)(255 - pinPart1);
                dst += ps;
                dst[RGBA.A] = (byte)(255 - pinPart2);
                #endregion Save Pin in first line
                
                src = (byte*)srcData.Scan0.ToPointer();
                dst = (byte*)dstData.Scan0.ToPointer();
                // align pointer
                src += distH * stride + distW * ps;
                dst += distH * stride + distW * ps;

				byte smallest = 255;
				int smallestCount = 0;

                // for each line
                for (int y = distH; y < h - distH; y++)
                {
                    // for each pixel
                    for (int x = distW; x < w - distW;
                        x++,
                        src += ps,
                        dst += ps,                        
                        pinPosition = pinPosition < Pin ? pinPosition + 1 : 0,
                        indexKey = indexKey == 31 ? 0 : indexKey + 1)
                    {

                        if (pinPosition != 0 || indexChar >= linesAsString.Length)
                        {                            
                            continue;
                        }

                        char c = linesAsString[indexChar];
                        indexChar++;
                        int intC = c;

                        // INNER encryption by substracting hashArray element
                        intC -= hashArray[indexKey];

                        byte byteC = (byte)(255 - intC);                           
                        dst[RGBA.A] = byteC;
                        // TODO write
                         Console.WriteLine("Alpha write= " + byteC);
						smallest = Math.Min (smallest, byteC);
						if (byteC <= 180) {
							smallestCount++;
						}
                    }

                    src += 2 * distW * ps + offset;
                    dst += 2 * distW * ps + offset;
                }
                
                Success = true;
				Console.WriteLine ("Smallest= " + smallest);
				Console.WriteLine ("Small counter= " + smallestCount + " / " + linesAsString.Length);
            }
            else // read-in mode
            {
                // TODO read-out PIN
                #region Read out Pin in first line
                dst = (byte*)dstData.Scan0.ToPointer();
                // set PIN position at half of image width
                dst += (w / 2) * ps;
                byte pinPart1 = (byte)Math.Abs(dst[RGBA.A] - 255);
                // do decryption by adding sha256Key element
                pinPart1 += hashArray[0];
                dst += ps;
                byte pinPart2 = (byte)Math.Abs(dst[RGBA.A] - 255);
                pinPart2 += hashArray[1];
                Pin = pinPart1 * 100 + pinPart2;                
                #endregion Read out in first line

                string tmp = "";

                src = (byte*)srcData.Scan0.ToPointer();
                dst = (byte*)dstData.Scan0.ToPointer();
                // align pointer
                src += distH * stride + distW * ps;
                dst += distH * stride + distW * ps;

                // for each line
                for (int y = distH; y < h - distH; y++)
                {                                       
                    // for each pixel
                    for (int x = distW; x < w - distW;
                        x++,
                        src += ps,
                        dst += ps,
                        pinPosition = pinPosition < Pin ? pinPosition + 1 : 0,
                        indexKey = indexKey == 31 ? 0 : indexKey + 1)
                    {
                        if (pinPosition != 0)
                        {
                            continue;
                        }

                        byte byteC = src[RGBA.A];
                        byteC = (byte)Math.Abs(byteC - 255);


                        // INNER decryption by adding hashArray element
                        byteC += hashArray[indexKey];

                        char c = (char)byteC;

                        //// replaces white space and control characters
                        //if (c == '\0' || c == '\f' || c == '\n' || c == '\t' ||
                        //    c == '\v' || c == '\r')
                        //{
                        //    c = 'X';
                        //}
                        //else if (char.IsControl(c))
                        //{
                        //    c = 'Z';
                        //}

                        if (c == '~')
                        {
                            // OUTER decryption by using RC4
							tmp = AsciiInvertCharEncryption.Process(tmp, endByte, 1);
                            lines.Add(tmp);
                            tmp = "";
                        }
                        else
                        {
                            tmp += c;
                        }
                    }

                    src += 2 * distW * ps + offset;
                    dst += 2 * distW * ps + offset;
                }
            }

//            // close form
//            form.End();
        }

        #region private helper methods

        private static byte[] GetHashArray(byte[] pwSha, byte[] pwMurmur)
        {
            SHA256 sha256 = new SHA256CryptoServiceProvider();
            // byte[] textToHash = Encoding.ASCII.GetBytes(key);
            
            uint seedMurmur = 0;
            foreach (byte b in pwMurmur)
            {
                seedMurmur += b;
            }
            byte[] hashArray = sha256.ComputeHash(pwSha);
            sha256.Clear();


            Murmur3 m = new Murmur3(seedMurmur);
            byte[] murmur = m.ComputeHash(pwMurmur);
            // m = null;
            
            
            for (int i = 0, j = 0; i < 32; i++, j++ )
            {
                if (j == 16)
                {
                    j = 0;
                }
                hashArray[i] = (byte) (hashArray[i] + murmur[j]);
            }

            hashArray = GetQuerSumOfBytes(hashArray);

            return hashArray;
        }

        private static byte[] GetQuerSumOfBytes(byte[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                float b = array[i];

                int three = (int)(b / 100);
                int two = (int)((b - (three * 100)) / 10);
                int one = (int)(b - three * 100 - two * 10);
                array[i] = (byte)(three + two + one);
            }

            return array;
        }
        
        private static int GetborderDist(int w, int h)
        {
            int res = w * h; // resolution
            // shifting right (e.g. 8 >> 2 = 2 --> 8 / 2 = 4 / 2 = 2)            
            string shift = (res >> 2).ToString(CultureInfo.InvariantCulture);
            int a = int.Parse(shift.Substring(0, 1));            
            return Math.Max(a, 1);
        }

        private static byte[] GetPassword(string key, bool appendKey)
        {
            double e = Math.E;
            double pi = Math.PI;

            if (!appendKey)
            {
                e = e * pi * pi;
                pi = pi * e * e;
            }

            byte[] keyBytes = AsciiInvertCharEncryption.GetBytesFromString(key);
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
                result[n + i] = appendKey ? keyBytes[i] : result[i];    
            }

            return result;
        }

        #endregion private helper methods
    }
}
