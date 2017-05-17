using System;
using System.Security.Cryptography;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Collections;

namespace Troonie_Lib
{
	public class LeonSteg
	{
		protected struct PixelInfo
		{
			public bool IsUsed;
			public bool IsUsedForObfuscation;
			public bool IsValueInChannelBitStegChanged;
			public int ChannelBitSteg;
			public int ChannelObfucsation;
		}

		#region constants and statics
		/// <summary>Final bytes, added in the end of the byte array. </summary>
		private static string endText = string.Empty + (char)1 + (char)2 + (char)3 + (char)4;
		public static int LengthFinalBytes { get { return endText.Length; } }
		#endregion

		#region private and protected member and properties
		private byte indexHash;
		private int pixelSize;
		private int moduloNumber; // if grayscale moduloNumber = 1, if colored moduloNumber = 3 
		private byte[] hash;


		protected int usableChannels; 
		protected int posX, posY; // position of current pixel pointer
		protected int indexChannel;
		protected int indexPixel;
		protected int w, h; // image width and height
		protected List<bool> bits;
		protected PixelInfo[] usedPixel;
		#endregion

		public LeonSteg()
		{
			usableChannels = 1;
		}
			
		#region public methods and functions
		/// <summary>
		/// Writes the <paramref name="text"/> into the image <paramref name="source"/> by using 
		/// specified <paramref name="key"/> for SHA512-encryption. 
		/// Returns error code: 
		/// '0' --> steganography success, no errors;  
		/// '1' --> Too long text (or unvalid text) and to small image resolution;
		/// '2' --> Not supported pixel format of image;
		/// '3' --> Image resolution too small (minimum 256 pixels);
		/// '4' --> Image resolution too big (avoiding integer overflow);
		/// '5' --> No color image. Only color images works with BitStegRGB.;
		/// </summary>
		public int Write(Bitmap source, string key, string text)
		{
			int dim = usableChannels * (source.Width * source.Height) / 8 - LengthFinalBytes;
			if (text == null || text.Length > dim) {
				return 1;
			}

			int error = Init (source, key);
			if (error != 0) {
				return error;
			}

			byte[] tmp_Hash = new byte[256];
			hash.CopyTo(tmp_Hash, 0);

			EncryptBytesAndFillBitList (AsciiTableCharMove.GetBytesFromString (text));

			indexHash = (byte)key.Length; // reset indexHash
			hash = tmp_Hash;  // reset hash

			Write (source);
			return 0;
		}

		/// <summary>
		/// Writes the <paramref name="bytes"/> into the image <paramref name="source"/> by using 
		/// specified <paramref name="key"/> for SHA512-encryption. 
		/// Returns error code: 
		/// '0' --> steganography success, no errors;  
		/// '1' --> Too many bytes (or unvalid bytes) and to small image resolution;
		/// '2' --> Not supported pixel format of image;
		/// '3' --> Image resolution too small (minimum 256 pixels);
		/// '4' --> Image resolution too big (avoiding integer overflow);
		/// '5' --> No color image. Only color images works with BitStegRGB.;
		/// </summary>
		public int Write(Bitmap source, string key, byte[] bytes)
		{
			int dim = usableChannels * (source.Width * source.Height) / 8 - LengthFinalBytes;
			if (bytes == null || bytes.Length > dim) {
				return 1;
			}

			int error = Init (source, key);
			if (error != 0) {
				return error;
			}

			byte[] tmp_Hash = new byte[256];
			hash.CopyTo(tmp_Hash, 0);

			EncryptBytesAndFillBitList (bytes);

			indexHash = (byte)key.Length; // reset indexHash
			hash = tmp_Hash;  // reset hash

			Write (source);			
			return 0;
		}

		public void Read(Bitmap source, string key, out byte[] bytes)
		{
			Read (source, key);
			DecryptBytesFromBitList(out bytes);
		}

		public void Read(Bitmap source, string key, out string text)
		{
			Read (source, key);
			DecryptStringFromBitList(out text);
		}


		#endregion

		#region protected and private methods and functions
		private unsafe void Read(Bitmap source, string key)
		{
			Init (source, key);
			byte[] tmp_Hash = new byte[256];
			hash.CopyTo(tmp_Hash, 0);
			int count8Bit = 0;

			BitmapData srcData = source.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, source.PixelFormat);

			for (int i = 0; i < w * h * usableChannels; i++, count8Bit++) {
				CalcNextIndexPixelAndPosXY();
				byte* src = (byte*)srcData.Scan0.ToPointer();
				src += posY * srcData.Stride + posX * pixelSize;

				bool bit = (src [indexChannel] & 1) == 1;
				bits.Add (bit);

				if (count8Bit == 7) {
					count8Bit = -1;
					if (CheckForEndString ()) {
						break;
					}
				}
			}							

			source.UnlockBits(srcData);

			indexHash = (byte)key.Length; // reset indexHash
			hash = tmp_Hash;  // reset hash
		}

		private unsafe void Write(Bitmap source)
		{
			BitmapData srcData = source.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, source.PixelFormat);
			byte* src;

//			int countX = 0;
			foreach (bool bit in bits) {
				CalcNextIndexPixelAndPosXY ();
				src = (byte*)srcData.Scan0.ToPointer ();
				src += posY * srcData.Stride + posX * pixelSize;
				byte by = src [indexChannel];
				SetBitToChannelbyte (ref by, bit, out usedPixel [indexPixel].IsValueInChannelBitStegChanged);
				src [indexChannel] = by;

//				countX++;
//				Console.WriteLine ("countX4= " + countX);
			}

			if (usableChannels != 1) {
//				src = (byte*)srcData.Scan0.ToPointer ();
//				src = null;
				source.UnlockBits(srcData);
				return;
			}
			// START obfuscation
			for (int i = 0; i < w * h; i++) {
				GetAndTransformHashElement();
				if (!usedPixel [i].IsUsed || (pixelSize > 1 && usedPixel [i].IsUsed && !usedPixel [i].IsValueInChannelBitStegChanged)) {
					if (usedPixel [i].IsUsed) {
						while (indexChannel == usedPixel [i].ChannelBitSteg) {
							GetAndTransformHashElement ();
						}							
					}

					src = (byte*)srcData.Scan0.ToPointer ();
					posY = i / w;
					posX = i - posY * w;
					src += posY * srcData.Stride + posX * pixelSize;
					byte by = src [indexChannel];
					byte tmp = (byte)(by | 1);
					if (by == tmp) {
						tmp = (byte)(by & 254);
					}	

					//					if (by == tmp) {
					//						Console.WriteLine ("Something wrong. Should not happen!");
					//					}
					src [indexChannel] = tmp;
					usedPixel [i].IsUsedForObfuscation = true;
				}
			}

			//			int nothing = 0, intIsUsedOnlyForObfuscation = 0, intIsOnlyUsed = 0, intintIsUsedBoth = 0;
			//			foreach (PixelInfo pi in usedPixel) {
			//				if (pi.IsUsedForObfuscation && !pi.IsUsed) {
			//					intIsUsedOnlyForObfuscation++;
			//				}
			//
			//				if (!pi.IsUsedForObfuscation && pi.IsUsed) {
			//					intIsOnlyUsed++;
			//				}
			//
			//				if (pi.IsUsedForObfuscation && pi.IsUsed) {
			//					intintIsUsedBoth++;
			//				}
			//
			//				if (!pi.IsUsedForObfuscation && !pi.IsUsed) {
			//					nothing++;
			//				}
			//
			//				if (pi.IsUsed && !pi.IsUsedForObfuscation && !pi.IsValueInChannelBitStegChanged) {
			//					// only in grayscale possible
			//
			//				}
			//
			//			}

			// END obfuscation

			source.UnlockBits(srcData);
		}

		/// <summary>
		/// Inits necessary parameters.
		/// Returns error code: 
		/// '0' --> steganography success, no errors;  
		/// '1' --> Too long text (or unvalid text) and to small image resolution;
		/// '2' --> No supported pixel format of image;
		/// '3' --> Image resolution too small (minimum 256 pixels);
		/// '4' --> Image resolution too big (avoiding integer overflow);
		/// '5' --> No color image. Only color images works with BitStegRGB.;
		/// </summary>
		private int Init(Bitmap source, string key)
		{
			if (source.PixelFormat != PixelFormat.Format8bppIndexed &&
				source.PixelFormat != PixelFormat.Format24bppRgb &&
				source.PixelFormat != PixelFormat.Format32bppArgb &&
				source.PixelFormat != PixelFormat.Format32bppPArgb &&
				source.PixelFormat != PixelFormat.Format32bppRgb) {
//				string errorMsg = "No supported pixel format of image.";
//				throw new ArgumentException(errorMsg, "source");
				return 2;
			}				
				
			pixelSize = Image.GetPixelFormatSize(source.PixelFormat) / 8; 
			// Avoiding using BitStegRGB with grayscale image
			if (usableChannels == 3 && pixelSize < 3) {
				return 5;
			}

			moduloNumber = Math.Min (3, pixelSize);
			w = source.Width;
			h = source.Height;

			// avoiding image resolution < 256
			if (w * h * usableChannels < 256) 
			{
//				string errorMsg = "Image resolution too small (minimum 256 pixels).";
//				throw new ArgumentException(errorMsg, "source");
				return 3;
			}

			// avoiding integer overflow
			if ((ulong)w * (ulong)h * (ulong)8 * (ulong)usableChannels >= int.MaxValue) // 2147483647
			{
//				string errorMsg = "Image resolution too big (avoiding integer overflow).";
//				throw new ArgumentException(errorMsg, "source");
				return 4;
			}				
				
			bits = new List<bool> (w * h * usableChannels);
			usedPixel = new PixelInfo[w * h * usableChannels];
			indexPixel = 0;
			indexHash = (byte)key.Length; // start indexHash
			hash = GetCryptedHash (key);
			return 0;
		}

		private bool CheckForEndString()
		{
			if (bits.Count < endText.Length * 8) {
				return false;
			}
			List<bool> b = bits.GetRange (bits.Count - endText.Length * 8, endText.Length * 8);
			BitArray a = new BitArray(b.ToArray());
			byte[] bytes = new byte[endText.Length];
			a.CopyTo(bytes, 0);
			string tmp = string.Empty;
			foreach (byte by in bytes) {
				tmp += (char)by;
			}
//			string tmp = 
//				string.Empty + (char)bytes [0] + (char)bytes [1] + (char)bytes [2] + (char)bytes [3];

			if (tmp == endText) {
				return true;
			}

			return false;
		}

		private void DecryptBytesFromBitList(out byte[] bytes)
		{
			BitArray a = new BitArray(bits.ToArray());
			byte[] tmpBytes = new byte[a.Length / 8];
			a.CopyTo(tmpBytes, 0);
			//			Array.Reverse(bytes); // not necessary
			bytes = new byte[tmpBytes.Length - endText.Length];

			// subtract NOT encrypted 'endText' string
			for (int i = 0; i < tmpBytes.Length - endText.Length; i++) {
				// DECRYPTION by subtracting hash element
				byte decryptedItem = (byte)(tmpBytes[i] - hash [GetAndTransformHashElement()]);
				bytes[i] = decryptedItem;
			}				
		}

		private void DecryptStringFromBitList(out string text)
		{
			text = string.Empty;
			BitArray a = new BitArray(bits.ToArray());
			byte[] tmpBytes = new byte[a.Length / 8];
			a.CopyTo(tmpBytes, 0);

			// subtract NOT encrypted 'endText' string
			for (int i = 0; i < tmpBytes.Length - endText.Length; i++) {
				// DECRYPTION by subtracting hash element
				byte decryptedItem = (byte)(tmpBytes[i] - hash [GetAndTransformHashElement()]);
				char c = (char)decryptedItem;
				text += c;
			}
		}

		private void EncryptBytesAndFillBitList(byte[] bytes)
		{			
			List<byte> encryptedBytes = new List<byte>();

			for (int i = 0; i < bytes.Length; i++) {
				// ENCRYPTION by adding hash element
				bytes[i] += hash [GetAndTransformHashElement()];
			}	

			encryptedBytes.AddRange (bytes);	
			// add NOT encrypted 'endText' string
			encryptedBytes.AddRange(AsciiTableCharMove.GetBytesFromString (endText));

//			if (encryptedBytes.Count > w * h / 8) {
//				return false;
//			}
				
			// converting to bits
			BitArray myBA = new BitArray(encryptedBytes.ToArray());
			IEnumerator ie = myBA.GetEnumerator ();
			while(ie.MoveNext ()) {
//				Console.WriteLine(ie.Current);
				bits.Add ((bool)ie.Current);
			}				
		}

		private static byte[] GetCryptedHash(string key)
		{
			Byte[] final = new byte[256];
			int i_final = 0;
			Byte[] bytes = AsciiTableCharMove.GetBytesFromString (key);
			// SHA256 sha256 = new SHA256CryptoServiceProvider();
			SHA512 sha512 = new SHA512Managed();		

			// round 1: byte element 0-63
			byte[] hashArray = sha512.ComputeHash(bytes);
			for (int i = 0; i < hashArray.Length; i++, i_final++) {
				hashArray [i] += Fraction.DigitSumOfByte (hashArray [i]);
				final [i_final] = hashArray [i];
			}
				
			// round 2: byte element 64-127
			hashArray = sha512.ComputeHash(hashArray);
			for (int i = 0; i < hashArray.Length; i++, i_final++) {
				hashArray [i] += Fraction.DigitSumOfByte (hashArray [i]);
				final [i_final] = hashArray [i];
			}

			// round 3: byte element 128-191
			hashArray = sha512.ComputeHash(hashArray);
			for (int i = 0; i < hashArray.Length; i++, i_final++) {
				hashArray [i] += Fraction.DigitSumOfByte (hashArray [i]);
				final [i_final] = hashArray [i];
			}

			// round 4: byte element 192-255
			hashArray = sha512.ComputeHash(hashArray);
			for (int i = 0; i < hashArray.Length; i++, i_final++) {
				hashArray [i] += Fraction.DigitSumOfByte (hashArray [i]);
				final [i_final] = hashArray [i];
			}
				
			sha512.Clear();

			return final;
		}

//		private static void GetBitOfChannelbyte(byte sourcebyte, out bool bit)
//		{
//			bit = (sourcebyte & 1) == 1;
//		}

		private static void SetBitToChannelbyte(ref byte sourcebyte, bool bit, out bool valueChanged)
		{
			byte tmp;
			if (bit) {
				tmp = (byte)(sourcebyte | 1);
			} else {
				tmp = (byte)(sourcebyte & 254);
			}

			valueChanged = sourcebyte != tmp;
			sourcebyte = tmp;
		}
			
		protected byte GetAndTransformHashElement()
		{
			byte b = hash[indexHash];
			indexChannel = b % moduloNumber; // only for BitSteg NOT for BitStegRGB
			hash[indexHash] += Fraction.DigitSumOfByte (b); // always change value after usage
			indexHash += (byte)(b + 1); // always increment additionally

			return b;
		}

		protected virtual void CalcNextIndexPixelAndPosXY()
		{			
			int max = (w * h);
			indexPixel += GetAndTransformHashElement();
			if (indexPixel >= max) {
				indexPixel = indexPixel - max;
			}				

			while (/*usedPixel[indexPixel] != null &&*/ usedPixel [indexPixel].IsUsed /* || countUsedPixel < max */) {
				indexPixel++;
				if (indexPixel >= max) {
					indexPixel = indexPixel - max;
				}
			} 

			posY = indexPixel / w;
			posX = indexPixel - posY * w;

			usedPixel [indexPixel] = new PixelInfo () { IsUsed = true, ChannelBitSteg = indexChannel };
		}

		#endregion
	}

	public class LeonStegRGB : LeonSteg
	{
		public LeonStegRGB()
		{
			usableChannels = 3;
		}

		protected override void CalcNextIndexPixelAndPosXY()
		{			
			int max = (w * h * usableChannels);
			indexPixel += GetAndTransformHashElement();
			if (indexPixel >= max) {
				indexPixel = indexPixel - max;
			}				

			while (/*usedPixel[indexPixel] != null &&*/ usedPixel [indexPixel].IsUsed /* || countUsedPixel < max */) {
				indexPixel++;
				if (indexPixel >= max) {
					indexPixel = indexPixel - max;
				}
			} 

			indexChannel = indexPixel % usableChannels /* moduloNumber */;
			posY = (indexPixel - indexChannel) / (w * usableChannels);
			posX = (indexPixel - indexChannel) - (posY * w * usableChannels);
			posX /= usableChannels;

			usedPixel [indexPixel] = new PixelInfo () { IsUsed = true /*, ChannelBitSteg = indexChannel */ };
		}			
	}
}

