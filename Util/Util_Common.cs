using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace iKCoderSDK
{
    public class Util_Common
    {
        public static string Encoder_Base64(string data)
        {
            if (string.IsNullOrEmpty(data))
                return string.Empty;
            else
            {
                try
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);
                    string str = Convert.ToBase64String(bytes);
                    return str;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        public static long GuidToLongID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }

		public static byte[] Object2Bytes(object obj)
		{
			byte[] buff;
			using (MemoryStream ms = new MemoryStream())
			{
				IFormatter iFormatter = new BinaryFormatter();
				iFormatter.Serialize(ms, obj);
				buff = ms.GetBuffer();
			}
			return buff;
		}

		
		public static object Bytes2Object(byte[] buff)
		{
			object obj;
			using (MemoryStream ms = new MemoryStream(buff))
			{
				IFormatter iFormatter = new BinaryFormatter();
				obj = iFormatter.Deserialize(ms);
			}
			return obj;
		}


	}
}
