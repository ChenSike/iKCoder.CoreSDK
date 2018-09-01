using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace iKCoderSDK
{
    public class Basic_Exceptions:ApplicationException
    {
        public Basic_Exceptions()
        {
            string exceptionLogDir = Environment.CurrentDirectory + "\\" + "exceptions";
            DirectoryInfo objDir = new DirectoryInfo(exceptionLogDir);
            if (!objDir.Exists)
            {
                try
                {
                    objDir.Create();
                }
                catch
                {
                    return;
                }
            }
            else
            {
                string exceptionLogFile = "Exception_" + DateTime.Now.ToString("yyyy-MM-dd") + ".xml";
                FileInfo objFi = new FileInfo(exceptionLogDir + "\\" + exceptionLogFile);
                FileStream objFS;
                if (!objFi.Exists)
                {
                    try
                    {
                        objFS = new FileStream(exceptionLogFile, FileMode.Create);
                        StreamWriter objSW = new StreamWriter(objFS);
                        objSW.WriteLine("<root></root>");
                        objSW.Flush();
                        objSW.Close();
                        objFS.Close();
                    }
                    catch
                    {
                        return;
                    }
                }
                XmlDocument exceptionLogDoc = new XmlDocument();
                exceptionLogDoc.Load(exceptionLogFile);
                XmlNode rootNode = exceptionLogDoc.SelectSingleNode("/root");
                XmlNode newItem = Util_XmlOperHelper.CreateNode(exceptionLogDoc, "item","");
                Util_XmlOperHelper.SetAttribute(newItem, "message", Message);
                Util_XmlOperHelper.SetAttribute(newItem, "time", DateTime.Now.ToString());
                Util_XmlOperHelper.SetAttribute(newItem, "stake", StackTrace.ToString());
                rootNode.AppendChild(newItem);
                try
                {
                    lock (exceptionLogDoc)
                    {
                        exceptionLogDoc.Save(exceptionLogFile);
                    }
                }
                catch
                {
                    return;
                }
            }
        }
    }
}
