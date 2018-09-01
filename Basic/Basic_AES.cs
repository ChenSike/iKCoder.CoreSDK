using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace iKCoderSDK
{

	public class Basic_AES
    {

		private string key = "ikcoderdefaultabcdefghjikleiuytl";
		
		public string optionalKey
		{
			set
			{
				key = value;
			}
			get
			{
				return key;
			}
		}

		public string AesEncrypt(string input)
		{
			if (string.IsNullOrEmpty(input))
				return string.Empty;

			var encryptKey = Encoding.UTF8.GetBytes(key);

			using (var aesAlg = Aes.Create())
			{
				using (var encryptor = aesAlg.CreateEncryptor(encryptKey, aesAlg.IV))
				{
					using (var msEncrypt = new MemoryStream())
					{
						using (var csEncrypt = new CryptoStream(msEncrypt, encryptor,
							CryptoStreamMode.Write))

						using (var swEncrypt = new StreamWriter(csEncrypt))
						{
							swEncrypt.Write(input);
						}

						var iv = aesAlg.IV;

						var decryptedContent = msEncrypt.ToArray();

						var result = new byte[iv.Length + decryptedContent.Length];

						Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
						Buffer.BlockCopy(decryptedContent, 0, result,
							iv.Length, decryptedContent.Length);

						return Convert.ToBase64String(result);
					}
				}
			}
		}

		public string AesDecrypt(string input)
		{

			if (string.IsNullOrEmpty(input))
				return string.Empty;

			var fullCipher = Convert.FromBase64String(input);

			var iv = new byte[16];
			var cipher = new byte[16];

			Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
			Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
			var decryptKey = Encoding.UTF8.GetBytes(key);

			using (var aesAlg = Aes.Create())
			{
				using (var decryptor = aesAlg.CreateDecryptor(decryptKey, iv))
				{
					string result;
					using (var msDecrypt = new MemoryStream(cipher))
					{
						using (var csDecrypt = new CryptoStream(msDecrypt,
							decryptor, CryptoStreamMode.Read))
						{
							using (var srDecrypt = new StreamReader(csDecrypt))
							{
								result = srDecrypt.ReadToEnd();
							}
						}
					}

					return result;
				}
			}
		}
		
    }
}
