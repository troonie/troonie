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
		/// <summary>Final bytes as string, added at the end of the encrypted byte array. </summary>
		private static string endText = string.Empty + (char)1 + (char)2 + (char)3 + (char)4;
		/// <summary>Length of final bytes, added at the end of the encrypted byte array. </summary>
		public static int LengthEndText { get { return endText.Length; } }

		private byte indexHash;
		/// <summary>Modulo number, if grayscale mod=1, if colored mod=3. </summary>
		private int mod;
		private byte[] hash;
		private List<bool> bits;

		protected struct PixelInfo
		{
			public bool Used;
//			public bool UsedForObfuscation;
			public bool ValueChanged;
			public int Channel;
//			public int ChannelObfucsation;
		}			
		/// <summary> Channel per pixel. </summary>
		protected int cpp;
		protected int usableChannels; 
		protected int posX, posY; // position of current pixel pointer
		protected int indexChannel;
		protected int indexPixel;
		protected int w, h; // image width and height
		protected PixelInfo[] usedPixel;

		// TODO: Comment!
		public byte CancelToken { get; set; }

		public LeonSteg()
		{
			usableChannels = 1;
		}
			
		#region public methods and functions
		/// <summary>
		/// Writes the <paramref name="bytes"/> into the image <paramref name="source"/> by using 
		/// specified <paramref name="key"/> for SHA512-encryption. 
		/// Returns error code: 
		/// '0' --> steganography success, no errors;  
		/// '1' --> Too many bytes (or unvalid bytes) and to small image resolution;
		/// '2' --> Not supported pixel format of image;
		/// '3' --> Image resolution too small (minimum 256 pixels);
		/// '4' --> Image resolution too big (avoiding integer overflow);
		/// '5' --> No color image. Only color images works with LeonStegRGB.;
		/// </summary>
		public unsafe int Write(Bitmap source, string key, byte[] bytes)
		{
			int dim = usableChannels * (source.Width * source.Height) / 8 - LengthEndText;
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

			BitmapData srcData = source.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, source.PixelFormat);

			foreach (bool bit in bits) {
				CalcNextIndexPixelAndPosXY ();
				byte* src = (byte*)srcData.Scan0.ToPointer ();
				src += posY * srcData.Stride + posX * cpp;
				byte by = src [indexChannel];
				byte tmp = bit ? (byte)(by | 1) : (byte)(by & 254);
				usedPixel [indexPixel].ValueChanged = by != tmp;
				by = tmp;

				src [indexChannel] = by;
			}				

			Obfuscation(srcData);				
			source.UnlockBits(srcData);

			return 0;
		}

		public unsafe void Read(Bitmap source, string key, out byte[] bytes)
		{
			Init (source, key);
			byte[] tmp_Hash = new byte[256];
			hash.CopyTo(tmp_Hash, 0);
			int count8Bit = 0;

			BitmapData srcData = source.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, source.PixelFormat);

			for (int i = 0; i < w * h * usableChannels; i++, count8Bit++) {
				if (CancelToken == 1) {
					source.UnlockBits (srcData);
					//Console.WriteLine ("Method LS.READ(..) canceled");
					bytes = new byte [] { (byte)'e', (byte)'r', (byte)'r', (byte)'o', (byte)'r' };
					//CancelToken = 2;
					return;
				}
				CalcNextIndexPixelAndPosXY ();
				byte* src = (byte*)srcData.Scan0.ToPointer();
				src += posY * srcData.Stride + posX * cpp;

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

			DecryptBytesFromBitList(out bytes);
			//CancelToken = 2;
		}

		#endregion

		#region protected methods and functions

		protected byte GetAndTransformHashElement()
		{
			byte b = hash[indexHash];
			indexChannel = b % mod; // only for LeonSteg NOT for LeonStegRGB
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

			while (usedPixel [indexPixel].Used) {
				indexPixel++;
				if (indexPixel >= max) {
					indexPixel = indexPixel - max;
				}
			} 

			posY = indexPixel / w;
			posX = indexPixel - posY * w;

			usedPixel [indexPixel] = new PixelInfo () { Used = true, Channel = indexChannel };
		}

		protected unsafe virtual void Obfuscation(BitmapData bitmapData)
		{
			for (int i = 0; i < w * h; i++) {
				GetAndTransformHashElement();
				if (!usedPixel [i].Used || (cpp > 1 && usedPixel [i].Used && !usedPixel [i].ValueChanged)) {
					if (usedPixel [i].Used) {
						while (indexChannel == usedPixel [i].Channel) {
							GetAndTransformHashElement ();
						}							
					}

					byte* src = (byte*)bitmapData.Scan0.ToPointer ();
					posY = i / w;
					posX = i - posY * w;
					src += posY * bitmapData.Stride + posX * cpp;
					byte by = src [indexChannel];
					byte tmp = (byte)(by | 1);
					if (by == tmp) {
						tmp = (byte)(by & 254);
					}	

					src [indexChannel] = tmp;
					//	usedPixel [i].IsUsedForObfuscation = true;
				}
			}				
		}

		#endregion

		#region private methods and functions
		/// <summary>
		/// Inits necessary parameters.
		/// Returns error code: 
		/// '0' --> steganography success, no errors;  
		/// '1' --> Too long text (or unvalid text) and to small image resolution;
		/// '2' --> No supported pixel format of image;
		/// '3' --> Image resolution too small (minimum 256 pixels);
		/// '4' --> Image resolution too big (avoiding integer overflow);
		/// '5' --> No color image. Only color images works with LeonStegRGB.;
		/// </summary>
		private int Init(Bitmap source, string key)
		{
			if (source.PixelFormat != PixelFormat.Format8bppIndexed &&
				source.PixelFormat != PixelFormat.Format24bppRgb &&
				source.PixelFormat != PixelFormat.Format32bppArgb &&
				source.PixelFormat != PixelFormat.Format32bppPArgb &&
				source.PixelFormat != PixelFormat.Format32bppRgb) {
				return 2;
			}				
				
			cpp = Image.GetPixelFormatSize(source.PixelFormat) / 8; 
			// Avoiding using LeonStegRGB with grayscale image
			if (usableChannels == 3 && cpp < 3) {
				return 5;
			}

			mod = Math.Min (3, cpp);
			w = source.Width;
			h = source.Height;

			// avoiding image resolution < 256
			if (w * h * usableChannels < 256) 
			{
				return 3;
			}

			// avoiding integer overflow
			if ((ulong)w * (ulong)h * (ulong)8 * (ulong)usableChannels >= int.MaxValue) // 2147483647
			{
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
				if (CancelToken == 1) {
					//Console.WriteLine ("Method LS.DecryptBytesFromBitList(..) canceled");
					bytes = new byte [] { (byte)'e', (byte)'r', (byte)'r', (byte)'o', (byte)'r' };
					//CancelToken = 2;
					return;
				}

				// DECRYPTION by subtracting hash element
				byte decryptedItem = (byte)(tmpBytes[i] - hash [GetAndTransformHashElement()]);
				bytes[i] = decryptedItem;
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
							
			// converting to bits
			BitArray myBA = new BitArray(encryptedBytes.ToArray());
			IEnumerator ie = myBA.GetEnumerator ();
			while(ie.MoveNext ()) {
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

			while (usedPixel [indexPixel].Used) {
				indexPixel++;
				if (indexPixel >= max) {
					indexPixel = indexPixel - max;
				}
			} 

			indexChannel = indexPixel % usableChannels /* moduloNumber */;
			posY = (indexPixel - indexChannel) / (w * usableChannels);
			posX = (indexPixel - indexChannel) - (posY * w * usableChannels);
			posX /= usableChannels;

			usedPixel [indexPixel] = new PixelInfo () { Used = true /*, ChannelLeonSteg = indexChannel */ };
		}	

		protected unsafe override void Obfuscation(BitmapData bitmapData)
		{
			Random rand = new Random();

			for (int i = 0; i < w * h * usableChannels; i++) {
				indexChannel = i % usableChannels /* moduloNumber */;
				posY = (i - indexChannel) / (w * usableChannels);
				posX = (i - indexChannel) - (posY * w * usableChannels);
				posX /= usableChannels;

				byte* src = (byte*)bitmapData.Scan0.ToPointer();
				src += posY * bitmapData.Stride + posX * cpp + indexChannel;

				// rand determines (randomly 0 or 1) whether a not used channel will be changed
				if (!usedPixel [i].Used && rand.Next (2) == 1) {
					byte tmp = (byte)(*src | 1);
					if (*src == tmp) {
						tmp = (byte)(*src & 254);
					}	
					*src = tmp;
				}
			}				
		}
	}
}

