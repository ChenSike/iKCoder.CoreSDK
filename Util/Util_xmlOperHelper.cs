using System;
using System.Xml;
using System.Collections.Generic;
using System.Data;
using MySql;
using MySql.Data.MySqlClient;

namespace iKCoderSDK
{
    public class Util_XmlOperHelper
    {
        public const string closeTagL = "</";
        public const string closeTagR = ">";
        public const string ERRORROOT = "ERRORROOT";
        public const string openCloseTag = "/>";
        public const string openTagL = "<";
        public const string openTagR = ">";
        public const string quote = "\"";
        public const string rootTag = "root";
        public delegate void ActionForNode(XmlNode selectedNode);

        public static void AddAttribute(XmlNode parent, string name, string data)
        {
            if (((parent != null) && (name != null)) && (name.Length != 0))
            {
                string[] strArray = name.Split(new char[] { '|' });
                if (data == null)
                {
                    data = "";
                }
                string[] strArray2 = data.Split(new char[] { '|' });
                if ((strArray.Length == 1) && (strArray2[0] != null))
                {
                    strArray2[0] = data;
                }
                for (int i = 0; i < strArray.Length; i++)
                {
                    if (parent.SelectSingleNode("@" + strArray[i]) == null)
                    {
                        XmlAttribute node = parent.OwnerDocument.CreateAttribute(strArray[i]);
                        parent.Attributes.Append(node);
                        if (strArray2[i] != null)
                        {
                            node.InnerText = strArray2[i];
                        }
                    }
                }
            }
        }

        public static void AddChildElement(XmlDocument srcDoc, XmlNode subNode)
        {
            if ((srcDoc != null) && (subNode != null))
            {
                srcDoc.DocumentElement.AppendChild(srcDoc.ImportNode(subNode, true));
            }
        }

        public static void AddChildElement(XmlNode node, string srcChildNode)
        {
            try
            {
                if (node != null)
                {
                    XmlNode node2 = node.OwnerDocument.CreateElement("tmp");
                    node2.InnerXml = srcChildNode;
                    foreach (XmlNode node3 in node2.SelectNodes("*"))
                    {
                        node.AppendChild(node3);
                    }
                }
            }
            catch
            {
            }
        }

        public static void AddXmlChildElement(XmlNode parentNode, string strChildNodeName, string strChildOuterXml, bool bForce, bool bAppend)
        {
            try
            {
                if ((((strChildNodeName != "") && (strChildOuterXml != "")) && (parentNode != null)) && (bForce || (parentNode.SelectSingleNode(strChildNodeName) == null)))
                {
                    XmlNode newChild = parentNode.SelectSingleNode(strChildNodeName);
                    if (newChild == null)
                    {
                        newChild = parentNode.OwnerDocument.CreateElement(strChildNodeName);
                        parentNode.AppendChild(newChild);
                    }
                    newChild.InnerXml = strChildOuterXml;
                    XmlNode node2 = newChild.SelectSingleNode(strChildNodeName);
                    parentNode.RemoveChild(newChild);
                    if (node2 != null)
                    {
                        if (bAppend)
                        {
                            parentNode.AppendChild(node2);
                        }
                        else
                        {
                            parentNode.PrependChild(node2);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        public static string CreateAttributeNode(string tagName, string[] attributeNames, string[] attributeValues)
        {
            string str = "";
            int length = attributeNames.Length;
            if (length <= 0)
            {
                return str;
            }
            str = str + "<" + tagName + " ";
            for (int i = 0; i < length; i++)
            {
                if (attributeValues[i] != null)
                {
                    string str3 = str;
                    str = str3 + attributeNames[i] + "=\"" + attributeValues[i] + "\" ";
                }
                else
                {
                    str = str + attributeNames[i] + "=\"\" ";
                }
            }
            return (str + "/>");
        }

        public static string CreateAttributeNode(string tagName, string attributeName, string attributeValue)
        {
            string str = "";
            str = str + "<" + tagName + " ";
            if (attributeValue != null)
            {
                string str3 = str;
                str = str3 + attributeName + "=\"" + attributeValue + "\" ";
            }
            else
            {
                str = str + attributeName + "=\"\" ";
            }
            return (str + "/>");
        }

        public static string CreateCloseTag(string tagName)
        {
            return ("</" + tagName + ">");
        }

        public static string CreateNode(string tagName, string nodeValue)
        {
            if (nodeValue != null)
            {
                return ("<" + tagName + ">" + nodeValue + "</" + tagName + ">");
            }
            return ("<" + tagName + "></" + tagName + ">");
        }

        public static XmlNode CreateNodeByXpath(XmlDocument xd, string nodeTag, string nodeValue)
        {
            if (xd == null)
            {
                return null;
            }
            string[] levelNodes = nodeTag.Split('/');
            string xpath = "";
            XmlNode exsitedNode = null;
            int index = 1;
            foreach (string activeNode in levelNodes)
            {
                xpath = xpath + "/" + activeNode;
                exsitedNode = xd.SelectSingleNode(xpath);
                if (exsitedNode == null)
                {
                    string activeNodeValue = "";
                    if (index == levelNodes.Length)
                        activeNodeValue = nodeValue;
                    XmlNode newNode = CreateNode(xd, activeNode, activeNodeValue);
                    XmlNode parentNode = xd.SelectSingleNode(xpath.Replace("/" + activeNode, ""));
                    parentNode.AppendChild(newNode);
                    exsitedNode = newNode;
                }
                index++;
            }
            return exsitedNode;
        }

        public static string CreateNode(string tagName, XmlNode node)
        {
            string str = "";
            str = str + CreateOpenTag(tagName);
            if (node != null)
            {
                str = str + node.InnerText;
            }
            return (str + CreateCloseTag(tagName));
        }

        public static XmlNode CreateNode(XmlDocument xd, string nodeTag, string nodeValue)
        {
            if (xd == null)
            {
                return null;
            }
            XmlElement element = xd.CreateElement(nodeTag);
            element.InnerText = nodeValue;
            return element;
        }

        public static string CreateOpenTag(string tagName)
        {
            return ("<" + tagName + ">");
        }

        public static string EncodeString(string strBefore)
        {
            return strBefore.Replace("&", "&amp;");
        }

        public static string EncodeXmlString(string strBefore)
        {
            return EncodeXmlString(strBefore, false, false, false);
        }

        public static string EncodeXmlString(string strBefore, bool bLtGt)
        {
            return EncodeXmlString(strBefore, false, false, false);
        }

        public static string EncodeXmlString(string strBefore, bool bLtGt, bool bQuote, bool bApos)
        {
            string str = "";
            str = string.Copy(strBefore.Replace("&amp;", "&"));
            if (bApos)
            {
                str = string.Copy(str.Replace("&apos;", "'"));
            }
            if (bQuote)
            {
                str = string.Copy(str.Replace("&quot;", "\""));
            }
            if (bLtGt)
            {
                str = string.Copy(string.Copy(str.Replace("&lt;", "<")).Replace("&gt;", ">"));
            }
            str = string.Copy(str.Replace("&", "&amp;"));
            if (bApos)
            {
                str = string.Copy(str.Replace("'", "&apos;"));
            }
            if (bQuote)
            {
                str = string.Copy(str.Replace("\"", "&quot;"));
            }
            if (bLtGt)
            {
                str = string.Copy(string.Copy(str.Replace("<", "&lt;")).Replace(">", "&gt;"));
            }
            return str;
        }

        public static double GetAttrDoublueValue(XmlNode node, string name)
        {
            double num = 0.0;
            try
            {
                num = Convert.ToDouble(node.Attributes[name].Value);
            }
            catch
            {
            }
            return num;
        }

        public static string GetAttrValue(XmlNode node, string name)
        {
            string str = "";
            try
            {
                str = node.Attributes[name].Value;
            }
            catch
            {
            }
            return str;
        }

        public static string GetCDATASection(string strText)
        {
            return ("<![CDATA[" + strText + "]]>");
        }

        public static string GetDataValue(string xmlResult, string tagName)
        {
            string str = "";
            string str2 = CreateOpenTag(tagName);
            string str3 = CreateCloseTag(tagName);
            int index = xmlResult.IndexOf(str2);
            int num2 = xmlResult.IndexOf(str3);
            if (num2 > (index + str2.Length))
            {
                str = xmlResult.Substring(index + str2.Length, (num2 - index) - str2.Length);
            }
            return str;
        }

        public static XmlNode GetNode(XmlNode node, string xPath)
        {
            XmlNode node2 = null;
            try
            {
                if (node != null)
                {
                    node2 = node.SelectSingleNode(xPath);
                }
            }
            catch
            {
            }
            return node2;
        }

        public static double GetNodeDoubleValue(string tagName, XmlNode node, double defaultValue)
        {
            string innerText = "";
            double num = defaultValue;
            try
            {
                if (node == null)
                {
                    return num;
                }
                XmlNode node2 = node.SelectSingleNode(tagName);
                if (node2 != null)
                {
                    innerText = node2.InnerText;
                }
                if (innerText.Length > 0)
                {
                    num = Convert.ToDouble(innerText);
                }
            }
            catch
            {
            }
            return num;
        }

        public static int GetNodeIntValue(string tagName, XmlNode node, int defaultValue)
        {
            string s = "";
            int result = defaultValue;
            if (node != null)
            {
                XmlNode node2 = node.SelectSingleNode(tagName);
                if (node2 != null)
                {
                    s = node2.InnerText;
                }
                if (!((s.Length > 0) && int.TryParse(s, out result)))
                {
                    result = defaultValue;
                }
            }
            return result;
        }

        public static string GetNodeValue(string tagName, XmlNode node)
        {
            if (node != null)
            {
                if (tagName == "")
                    return node.InnerText;
                XmlNode node2 = node.SelectSingleNode(tagName);
                if (node2 != null)
                {
                    return node2.InnerText;
                }
            }
            return "";
        }

        public static string BuildXpath(XmlNode node)
        {
            if (node != null)
            {
                string returnXpath = string.Empty;
                while (node.ParentNode != null)
                {
                    if (node.Name == "#document" || node.Name == "#text")
                    {
                        node = node.ParentNode;
                        continue;
                    }
                    else
                    {
                        returnXpath = "/" + node.Name + returnXpath;
                        node = node.ParentNode;
                    }
                }
                return returnXpath;
            }
            else
                return string.Empty;
        }

        public static Dictionary<string, string> BuildNodeAttributes(XmlNode node)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            try
            {
                foreach (XmlAttribute activeAttr in node.Attributes)
                {
                    if (!result.ContainsKey(activeAttr.Name))
                        result.Add(activeAttr.Name, activeAttr.Value);
                }
            }
            catch
            {

            }
            return result;
        }

        public static string BuildParentXpath(XmlNode node)
        {
            if (node != null)
            {
                string returnXpath = string.Empty;
                while (node.ParentNode != null)
                {
                    if (node.Name != "#document")
                    {
                        returnXpath = "/" + node.Name + returnXpath;
                    }
                    node = node.ParentNode;
                }
                if (string.IsNullOrEmpty(returnXpath))
                {
                    returnXpath = "/" + node.Name;
                }
                return returnXpath;
            }
            else
                return string.Empty;
        }

        public static void TraverseAllNodes(XmlNode rootNode, ref List<XmlNode> result)
        {
            if (rootNode == null)
                return;
            if (rootNode != null && rootNode.ChildNodes.Count == 0)
                result.Add(rootNode);
            else
            {
                foreach (XmlNode activeChildNode in rootNode.ChildNodes)
                    TraverseAllNodes(activeChildNode, ref result);
                result.Add(rootNode);
            }

        }

        public static string GetNodeValue(XmlNode node)
        {
            if (node != null)
            {
                if (node.ChildNodes.Count == 1)
                {
                    if (node.ChildNodes[0].Name == "#text")
                    {
                        if (node != null)
                        {
                            return node.InnerText;
                        }
                    }
                }
                return string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        public static void TraverseNodes(XmlNode Node, ActionForNode degActionForNode)
        {
            degActionForNode(Node);
            foreach (XmlNode childNode in Node.ChildNodes)
            {
                TraverseNodes(childNode, degActionForNode);
            }
        }

        public static string GetNodeValue(XmlDocument xmldoc, string nodePath)
        {
            string innerText = "";
            if ((xmldoc != null) && (nodePath.Length > 0))
            {
                XmlNode node = xmldoc.SelectSingleNode(nodePath);
                if (node != null)
                {
                    innerText = node.InnerText;
                }
            }
            return innerText;
        }

        public static string GetNodeValueEx(XmlNode node, string nodePath)
        {
            if (node != null)
            {
                XmlNode node2 = node.SelectSingleNode(nodePath);
                if (node2 != null)
                {
                    return node2.InnerText;
                }
            }
            return null;
        }

        public static string GetRowIdsFromXml(string idsXml)
        {
            if (idsXml.Length == 0)
            {
                return "";
            }
            string str = "";
            XmlDocument document = new XmlDocument();
            document.LoadXml(idsXml);
            foreach (XmlNode node in document.SelectNodes("RowIds/RowId"))
            {
                if (node.InnerText != "")
                {
                    if (str.Length > 0)
                    {
                        str = str + ",";
                    }
                    int index = node.InnerText.IndexOf(";");
                    if (index > 0)
                    {
                        str = str + node.InnerText.Substring(0, index);
                    }
                    else
                    {
                        str = str + node.InnerText;
                    }
                }
            }
            return str;
        }

        public static string GetXmlAttr(XmlNode node, string name)
        {
            return GetAttrValue(node, name);
        }

        public static bool IsErrorXml(string xmlResult)
        {
            return ((xmlResult.Length == 0) || (xmlResult.IndexOf("<Error>") >= 0));
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (phoneNumber.Length != 12)
            {
                return false;
            }
            if (phoneNumber.IndexOf("-", 0) != 3)
            {
                return false;
            }
            if (phoneNumber.IndexOf("-", 4) != 7)
            {
                return false;
            }
            return true;
        }

        public static bool IsValidSSNNumber(string ssnNumber)
        {
            if (ssnNumber.Length != 11)
            {
                return false;
            }
            if (ssnNumber.IndexOf("-", 0) != 3)
            {
                return false;
            }
            if (ssnNumber.IndexOf("-", 4) != 6)
            {
                return false;
            }
            return true;
        }

        public static void RemoveNode(XmlNode parent, string xpath)
        {
            if (((xpath != null) && (xpath.Length != 0)) && (parent != null))
            {
                string[] strArray = xpath.Split(new char[] { '|' });
                for (int i = 0; i < strArray.Length; i++)
                {
                    foreach (XmlNode node in parent.SelectNodes(strArray[i]))
                    {
                        node.ParentNode.RemoveChild(node);
                    }
                }
            }
        }

        public static void SetAttribute(XmlNode parent, string name, string data)
        {
            if (((parent != null) && (name != null)) && (name.Length != 0))
            {
                string[] strArray = name.Split(new char[] { '|' });
                if (data == null)
                {
                    data = "";
                }
                string[] strArray2 = data.Split(new char[] { '|' });
                if ((strArray.Length == 1) && (strArray2[0] != null))
                {
                    strArray2[0] = data;
                }
                for (int i = 0; i < strArray.Length; i++)
                {
                    if (parent.SelectSingleNode("@" + strArray[i]) != null)
                    {
                        parent.SelectSingleNode("@" + strArray[i]).InnerText = (strArray2[i] != null) ? strArray2[i] : "";
                    }
                    else
                    {
                        XmlAttribute node = parent.OwnerDocument.CreateAttribute(strArray[i]);
                        parent.Attributes.Append(node);
                        if (strArray2[i] != null)
                        {
                            node.InnerText = strArray2[i];
                        }
                    }
                }
            }
        }

        public static void SetAttrValue(XmlAttribute attribute, string text)
        {
            if (attribute != null)
            {
                attribute.InnerText = text;
            }
        }

        public static bool SetAttrValue(XmlNode node, string name, string attValue)
        {
            if (((node == null) || (name == null)) || (name.Length == 0))
            {
                return false;
            }
            bool flag = true;
            try
            {
                XmlAttribute attribute = node.Attributes[name];
                if (attValue == null)
                {
                    if (attribute != null)
                    {
                        node.Attributes.Remove(attribute);
                    }
                }
                else
                {
                    if (attribute == null)
                    {
                        attribute = node.OwnerDocument.CreateAttribute(name);
                        node.Attributes.Append(attribute);
                    }
                    attribute.Value = attValue;
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public static bool SetAttrValue(XmlDocument doc, XmlNode node, string name, string attValue)
        {
            bool flag = true;
            try
            {
                XmlAttribute attribute = node.Attributes[name];
                if (attValue == null)
                {
                    if (attribute != null)
                    {
                        node.Attributes.Remove(attribute);
                    }
                    return flag;
                }
                if (attribute == null)
                {
                    attribute = doc.CreateAttribute(name);
                    node.Attributes.Append(attribute);
                }
                if (attribute != null)
                {
                    attribute.Value = attValue;
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public static void SetNodeValue(XmlNode node, string text)
        {
            if (node != null)
            {
                node.InnerText = text;
            }
        }

        public static XmlNode SetNodeValue(XmlNode node, string name, string text, bool createNew, bool append)
        {
            XmlNode newChild = null;
            try
            {
                if ((node == null) || (text == null))
                {
                    return newChild;
                }
                newChild = node.SelectSingleNode(name);
                if ((newChild == null) && createNew)
                {
                    newChild = node.OwnerDocument.CreateElement(name);
                    if (append)
                    {
                        node.AppendChild(newChild);
                    }
                    else
                    {
                        node.PrependChild(newChild);
                    }
                }
                newChild.InnerText = text;
            }
            catch
            {
            }
            return newChild;
        }

        public static bool SetNodeValue(XmlDocument doc, XmlNode node, string name, string text, bool createNew, bool append)
        {
            bool flag = true;
            try
            {
                if ((doc == null) || (node == null))
                {
                    return flag;
                }
                XmlNode newChild = node.SelectSingleNode(name);
                if (((newChild == null) && createNew) && (text != null))
                {
                    newChild = doc.CreateElement(name);
                    if (append)
                    {
                        node.AppendChild(newChild);
                    }
                    else
                    {
                        node.PrependChild(newChild);
                    }
                }
                if (newChild == null)
                {
                    return flag;
                }
                if (text != null)
                {
                    newChild.InnerText = text;
                    return flag;
                }
                newChild.ParentNode.RemoveChild(newChild);
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public static void SetOneAttribute(XmlNode parent, string name, string data)
        {
            if (((parent != null) && (name != null)) && (name.Length != 0))
            {
                try
                {
                    if (parent.SelectSingleNode("@" + name) != null)
                    {
                        parent.SelectSingleNode("@" + name).InnerText = (data != null) ? data : "";
                    }
                    else
                    {
                        XmlAttribute node = parent.OwnerDocument.CreateAttribute(name);
                        parent.Attributes.Append(node);
                        if (data != null)
                        {
                            node.InnerText = data;
                        }
                    }
                }
                catch
                {
                }
            }
        }

        public static bool SetXmlAttr(XmlDocument doc, XmlNode node, string name, string attValue)
        {
            return SetAttrValue(doc, node, name, attValue);
        }

        public static void WriteElement(XmlTextWriter writer, string name, string value)
        {
            if ((value != null) && (value.Length > 0))
            {
                writer.WriteStartElement(name);
                writer.WriteString(value);
                writer.WriteEndElement();
            }
        }

        public static void WriteElementAttribute(XmlTextWriter writer, string ElementName, string AttributeName, string AttributeValue)
        {
            if ((AttributeValue != null) && (AttributeValue.Length > 0))
            {
                writer.WriteStartElement(ElementName);
                writer.WriteAttributeString(AttributeName, AttributeValue);
                writer.WriteEndElement();
            }
        }
    }
}
