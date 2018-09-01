using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace iKCoderSDK
{
    public class Util_NetServices
    {
		public object RequestWithGet(string Url)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(Url);
			httpWebRequest.Method = "GET";
			using (WebResponse wr = httpWebRequest.GetResponse())
			{
				StreamReader reader = new StreamReader(wr.GetResponseStream());
				string result = reader.ReadToEnd();
				return result;
			}
		}

		public object RequestWithPost(string Url,byte[] data)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(Url);
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/x-www-form-urlencoded";
			httpWebRequest.ContentLength = data.Length;
			using (Stream writeStream = httpWebRequest.GetRequestStream())
			{
				writeStream.Write(data, 0, data.Length);
			}
			using (WebResponse wr = httpWebRequest.GetResponse())
			{
				StreamReader reader = new StreamReader(wr.GetResponseStream());
				string result = reader.ReadToEnd();
				return result;
			}
		}
    }
}
