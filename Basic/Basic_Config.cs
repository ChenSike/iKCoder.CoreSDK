using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace iKCoderSDK
{
    public class Basic_Config
    {
        XmlDocument _configDoc = new XmlDocument();
        string _fileName = "";
        bool isloadedDoc = false;
        Basic_InternalPlugin refInternalPlugin;
        Basic_AES _objectDes = new Basic_AES();
        string _desKey = "ASDFGHJK";
        bool isDesMode = false;

        public bool Is_LoadedDoc
        {
            get
            {
                return isloadedDoc;
            }
        }

        public Basic_Config(string xmlData)
        {
            _configDoc = new XmlDocument();
            _configDoc.LoadXml(xmlData);
            isloadedDoc = true;
        }

        public Basic_Config(Basic_InternalPlugin refInternalPluginObj)
        {
            refInternalPlugin = refInternalPluginObj;
        }
        public Basic_Config()
        {
        }

        public Basic_Config(string fileName, Basic_InternalPlugin refInternalPluginObj)
        {
            try
            {
                _fileName = fileName;
                _configDoc.Load(fileName);
                isloadedDoc = true;
                refInternalPlugin = refInternalPluginObj;
            }
            catch
            {
                _configDoc.LoadXml("<root></root>");
            }
        }

        public void SwitchToAESModeON(string desKey)
        {
            _desKey = desKey;
            _objectDes.optionalKey = desKey;
            isDesMode = true;
        }

        public void SwitchToAESModeON()
        {            
            isDesMode = true;
        }

        public void SwitchToAESModeOFF()
        {
            isDesMode = false;
        }

        public XmlDocument GetConfigDocument()
        {
            return _configDoc;
        }

        public bool CreateNewConfigDocument()
        {
            try
            {
                _configDoc.LoadXml("<root></root>");
                _configDoc.Save(_fileName);
                isloadedDoc = true;
                return true;
            }
            catch
            {
                return false;
            }

        }

        public bool CreateNewConfigDocument(string newFilePath)
        {
            try
            {
                _configDoc.LoadXml("<root></root>");
                _configDoc.Save(newFilePath);
                _fileName = newFilePath;
                isloadedDoc = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DoSave()
        {
            try
            {
                lock (_configDoc)
                {
                    _configDoc.Save(_fileName);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool DoOpen(string fileNamePath)
        {
            try
            {
                _configDoc = new XmlDocument();
                _configDoc.Load(fileNamePath);
                _fileName = fileNamePath;
                isloadedDoc = true;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void DoReset()
        {
            isloadedDoc = false;
        }

        public bool IsDocumentExisted(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return false;
            else
            {
                FileInfo activeFile = new FileInfo(filePath);
                if (activeFile.Exists)
                    return true;
                else
                    return false;
            }
        }

        public XmlNode CreateNewSession(string SessionName, string SessionValue, bool IsInternalPluginUsed, string configKey)
        {
            if (SessionName == "")
                return null;
            else
            {
                string result = SessionValue;
                if (IsInternalPluginUsed && refInternalPlugin != null)
                {
                    Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                    paramsDirc.Add("key", configKey);
                    paramsDirc.Add("value", SessionValue);
                    paramsDirc.Add("return", "");
                    refInternalPlugin.actionSet(paramsDirc);
                    result = paramsDirc["return"].ToString();
                }
                XmlNode newSessionNode = Util_XmlOperHelper.CreateNode(_configDoc, "session", result);
                Util_XmlOperHelper.SetAttribute(newSessionNode, "name", SessionName);
                _configDoc.SelectSingleNode("/root").AppendChild(newSessionNode);
                return newSessionNode;
            }
        }

        public XmlNode CreateNewSession(string SessionName, string SessionValue)
        {
            if (SessionName == "")
                return null;
            else
            {
                string sessionValueResult = "";
                if (isDesMode)
                    sessionValueResult = _objectDes.AesEncrypt(SessionValue);
                else
                    sessionValueResult = SessionValue;
                XmlNode newSessionNode = Util_XmlOperHelper.CreateNode(_configDoc, "session", sessionValueResult);
                Util_XmlOperHelper.SetAttribute(newSessionNode, "name", SessionName);
                _configDoc.SelectSingleNode("/root").AppendChild(newSessionNode);
                return newSessionNode;
            }
        }

        public XmlNode CreateItem(XmlNode activeParentNode, string ItemName, string ItemValue, bool IsInternalPluginUsed, string configKey)
        {
            if (activeParentNode == null)
                return null;
            string result = ItemValue;
            if (IsInternalPluginUsed && refInternalPlugin != null)
            {
                Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                paramsDirc.Add("key", configKey);
                paramsDirc.Add("value", ItemValue);
                paramsDirc.Add("return", "");
                refInternalPlugin.actionSet(paramsDirc);
                result = paramsDirc["return"].ToString();
            }
            try
            {
                XmlNode newItemNode = Util_XmlOperHelper.CreateNode(_configDoc, "item", result);
                Util_XmlOperHelper.SetAttribute(newItemNode, "name", ItemName);
                activeParentNode.AppendChild(newItemNode);
                return newItemNode;
            }
            catch
            {
                return null;
            }
        }

        public XmlNode CreateItem(XmlNode activeParentNode, string ItemName, string ItemValue)
        {
            if (activeParentNode == null)
                return null;
            string result = ItemValue;
            try
            {
                if (isDesMode)
                    result = _objectDes.AesEncrypt(ItemValue);
                XmlNode newItemNode = Util_XmlOperHelper.CreateNode(_configDoc, "item", result);
                Util_XmlOperHelper.SetAttribute(newItemNode, "name", ItemName);
                activeParentNode.AppendChild(newItemNode);
                return newItemNode;
            }
            catch
            {
                return null;
            }
        }

        public XmlNode GetItemNode(string SessionName, string ItemName)
        {
            XmlNode activeSession = GetSessionNode(SessionName);
            if (activeSession != null)
            {
                XmlNode itemNode = activeSession.SelectSingleNode("item[@name='" + ItemName + "']");
                return itemNode;
            }
            else
                return null;
        }

        public XmlNode GetItemNode(XmlNode parentNode, string ItemName)
        {
            if (parentNode != null)
            {
                XmlNode itemNode = parentNode.SelectSingleNode("item[@name='" + ItemName + "']");
                return itemNode;
            }
            else
                return null;
        }

        public XmlNodeList GetItemNodes(string SessionName, string ItemName)
        {
            XmlNode activeSession = GetSessionNode(SessionName);
            if (activeSession != null)
            {
                XmlNodeList itemNodes = activeSession.SelectNodes("item[@name='" + ItemName + "']");
                return itemNodes;
            }
            else
                return null;
        }

        public XmlNodeList GetItemNodes(string SessionName)
        {
            XmlNode activeSession = GetSessionNode(SessionName);
            if (activeSession != null)
            {
                XmlNodeList itemNodes = activeSession.SelectNodes("item");
                return itemNodes;
            }
            else
                return null;
        }

        public string GetNodeValue(XmlNode activeNode, bool IsInternalPluginUsed, string configKey)
        {
            if (activeNode != null)
            {
                string sourceData = Util_XmlOperHelper.GetNodeValue("", activeNode);
                string resultData = "";
                if (IsInternalPluginUsed && refInternalPlugin != null)
                {
                    Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                    paramsDirc.Add("key", configKey);
                    paramsDirc.Add("value", sourceData);
                    paramsDirc.Add("return", "");
                    refInternalPlugin.actionGet(paramsDirc);
                    resultData = paramsDirc["return"].ToString();
                }
                else
                {
                    resultData = sourceData;
                }
                return resultData;
            }
            else
                return "";
        }

        public string GetNodeValue(XmlNode activeNode)
        {
            if (activeNode != null)
            {
                string sourceData = Util_XmlOperHelper.GetNodeValue("", activeNode);
                string resultData = "";
                resultData = sourceData;
                if (isDesMode)
                    resultData = _objectDes.AesDecrypt(sourceData);
                return resultData;
            }
            else
                return "";
        }

        public XmlNode GetSessionNode(string SessionName)
        {
            if (SessionName == "")
                return null;
            else
            {
                XmlNode activeSessionNode = _configDoc.SelectSingleNode("/root/session[@name='" + SessionName + "']");
                if (activeSessionNode != null)
                    return activeSessionNode;
                else
                    return null;
            }
        }

        public XmlNodeList GetSessionNodes()
        {
            if (_configDoc != null)
            {
                return _configDoc.SelectNodes("/root/session");
            }
            else
                return null;
        }

        public List<XmlAttribute> GetSessionAttrs(string SessionName)
        {
            List<XmlAttribute> resultList = new List<XmlAttribute>();
            if (IsSessionExisted(SessionName))
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                if (activeSessionNode != null)
                {
                    foreach (XmlAttribute activeAttr in activeSessionNode.Attributes)
                        resultList.Add(activeAttr);
                }
                return resultList;
            }
            else
                return resultList;
        }

        public bool SetSessionAttr(string SessionName, string AttrName, string AttrValue, bool IsInternalPluginUsed, string configKey)
        {
            if (SessionName == "" || AttrName == "")
                return false;
            string activeAttrValue = AttrValue;
            string result = AttrValue;
            if (IsInternalPluginUsed && refInternalPlugin != null)
            {
                Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                paramsDirc.Add("key", configKey);
                paramsDirc.Add("value", AttrValue);
                paramsDirc.Add("return", "");
                refInternalPlugin.actionGet(paramsDirc);
                result = paramsDirc["return"].ToString();
            }
            XmlNode activeSessionNode = GetSessionNode(SessionName);
            if (activeSessionNode != null)
            {
                Util_XmlOperHelper.SetAttribute(activeSessionNode, AttrName, result);
                return true;
            }
            else
                return false;
        }

        public bool SetSessionAttr(string SessionName, string AttrName, string AttrValue)
        {
            if (SessionName == "" || AttrName == "")
                return false;
            string activeAttrValue = AttrValue;
            string result = AttrValue;
            XmlNode activeSessionNode = GetSessionNode(SessionName);
            if (activeSessionNode != null)
            {
                if (isDesMode)
                    result=_objectDes.AesEncrypt(AttrValue);
                Util_XmlOperHelper.SetAttribute(activeSessionNode, AttrName, result);
                return true;
            }
            else
                return false;
        }

        public bool SetSessionValue(string SessionName, string SessionValue, bool IsInternalPluginUsed, string configKey)
        {
            if (SessionName == "")
                return false;
            else
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                if (activeSessionNode != null)
                {
                    string SessionValueResult = "";
                    if (IsInternalPluginUsed && refInternalPlugin != null)
                    {
                        Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                        paramsDirc.Add("key", configKey);
                        paramsDirc.Add("value", SessionValue);
                        paramsDirc.Add("return", "");
                        refInternalPlugin.actionSet(paramsDirc);
                        SessionValueResult = paramsDirc["return"].ToString();
                    }
                    else
                        SessionValueResult = SessionValue;
                    activeSessionNode.InnerText = SessionValueResult;
                    return true;
                }
                else
                    return false;

            }
        }

        public bool SetSessionValue(string SessionName, string SessionValue)
        {
            if (SessionName == "")
                return false;
            else
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                if (activeSessionNode != null)
                {
                    string SessionValueResult = "";
                    SessionValueResult = SessionValue;
                    if (isDesMode)
                        SessionValueResult=_objectDes.AesEncrypt(SessionValue);
                    activeSessionNode.InnerText = SessionValueResult;
                    return true;
                }
                else
                    return false;

            }
        }

        public bool SetItemAttr(XmlNode Item, string AttrName, string AttrValue, bool IsInternalPluginUsed, string configKey)
        {
            if (Item == null)
                return false;
            else
            {
                string result = AttrValue;
                if (IsInternalPluginUsed && refInternalPlugin != null)
                {
                    Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                    paramsDirc.Add("key", configKey);
                    paramsDirc.Add("value", AttrValue);
                    paramsDirc.Add("return", "");
                    refInternalPlugin.actionSet(paramsDirc);
                    result = paramsDirc["return"].ToString();

                }
                Util_XmlOperHelper.SetAttribute(Item, AttrName, result);
                return true;
            }
        }

        public bool SetItemAttr(XmlNode Item, string AttrName, string AttrValue)
        {
            if (Item == null)
                return false;
            else
            {
                string result = AttrValue;
                if (isDesMode)
                    result=_objectDes.AesEncrypt(AttrValue);
                Util_XmlOperHelper.SetAttribute(Item, AttrName, result);
                return true;
            }
        }

        public bool SetInitDocument(string tagName, string tagValue, bool IsInternalPluginUsed, string configKey)
        {
            if (_configDoc == null)
                return false;
            else
            {
                XmlNode rootNode = _configDoc.SelectSingleNode("/root");
                if (rootNode == null)
                    return false;
                else
                {
                    string result = "";
                    if (IsInternalPluginUsed && refInternalPlugin != null)
                    {
                        Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                        paramsDirc.Add("key", configKey);
                        paramsDirc.Add("value", tagValue);
                        paramsDirc.Add("return", "");
                        refInternalPlugin.actionSet(paramsDirc);
                        result = paramsDirc["return"].ToString();
                    }
                    Util_XmlOperHelper.SetAttribute(rootNode, tagName, IsInternalPluginUsed ? result : tagValue);
                    return true;
                }
            }
        }

        public bool IsInitDocument(string tagName, string tagValue, bool IsInternalPluginUsed, string configKey)
        {
            if (_configDoc == null)
                return false;
            else
            {
                XmlNode rootNode = _configDoc.SelectSingleNode("/root");
                if (rootNode == null)
                    return false;
                else
                {
                    if (!IsInternalPluginUsed || refInternalPlugin == null)
                        return Util_XmlOperHelper.GetNodeValue(tagName, rootNode) == tagValue ? true : false;
                    else
                    {
                        string result = "";
                        Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                        paramsDirc.Add("key", configKey);
                        paramsDirc.Add("value", tagValue);
                        paramsDirc.Add("return", "");
                        refInternalPlugin.actionSet(paramsDirc);
                        result = paramsDirc["return"].ToString();
                        return Util_XmlOperHelper.GetNodeValue(tagName, rootNode) == result ? true : false;
                    }
                }

            }
        }

        public bool IsSessionExisted(string SessionName)
        {
            XmlNode activeSessionNode = GetSessionNode(SessionName);
            if (activeSessionNode != null)
                return true;
            else
                return false;
        }

        public string GetSessionValue(string SessionName, bool IsInternalPluginUsed, string configKey)
        {
            if (SessionName == "")
                return "";
            else
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                string result = "";
                if (activeSessionNode != null)
                {
                    string value = Util_XmlOperHelper.GetNodeValue("", activeSessionNode);
                    result = value;
                    if (IsInternalPluginUsed && refInternalPlugin != null)
                    {
                        Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                        paramsDirc.Add("key", configKey);
                        paramsDirc.Add("value", value);
                        paramsDirc.Add("return", "");
                        refInternalPlugin.actionGet(paramsDirc);
                        result = paramsDirc["return"].ToString();
                    }
                }

                return result;
            }
        }

        public string GetSessionValue(string SessionName)
        {
            if (SessionName == "")
                return "";
            else
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                string result = activeSessionNode.InnerText;
                if (isDesMode)
                    result = _objectDes.AesEncrypt(activeSessionNode.InnerText);
                return result;
            }
        }

        public string GetAttrValue(XmlNode ActiveNode, string AttrName, bool IsInternalPluginUsed, string configKey)
        {
            if (ActiveNode == null)
                return "";
            else
            {
                string attrResult = Util_XmlOperHelper.GetAttrValue(ActiveNode, AttrName);
                string result = attrResult;
                if (IsInternalPluginUsed && refInternalPlugin != null)
                {
                    Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                    paramsDirc.Add("key", configKey);
                    paramsDirc.Add("value", attrResult);
                    paramsDirc.Add("return", "");
                    refInternalPlugin.actionGet(paramsDirc);
                    result = paramsDirc["return"].ToString();
                }
                return result;
            }
        }

        public string GetAttrValue(XmlNode ActiveNode, string AttrName)
        {
            if (ActiveNode == null)
                return "";
            else
            {
                string attrResult = Util_XmlOperHelper.GetAttrValue(ActiveNode, AttrName);
                string result = attrResult;
                if (isDesMode)
                    result = _objectDes.AesDecrypt(attrResult);
                return result;
            }
        }

        public string GetItemValue(string SessionName, string ItemName, bool IsInternalPluginUsed, string configKey)
        {
            if (SessionName == "" || ItemName == "")
                return "";
            else
            {
                XmlNode activeItemNode = GetItemNode(SessionName, ItemName);
                string attrResult = Util_XmlOperHelper.GetNodeValue("", activeItemNode);
                string Result = attrResult;
                if (IsInternalPluginUsed && refInternalPlugin != null)
                {
                    Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                    paramsDirc.Add("key", configKey);
                    paramsDirc.Add("value", attrResult);
                    paramsDirc.Add("return", "");
                    refInternalPlugin.actionGet(paramsDirc);
                    Result = paramsDirc["return"].ToString();
                }
                return Result;
            }
        }

        public string GetItemValue(string SessionName, string ItemName)
        {
            if (SessionName == "" || ItemName == "")
                return "";
            else
            {
                XmlNode activeItemNode = GetItemNode(SessionName, ItemName);
                string attrResult = Util_XmlOperHelper.GetNodeValue("", activeItemNode);
                string Result = attrResult;
                if (isDesMode)
                    Result=_objectDes.AesDecrypt(attrResult);
                return Result;
            }
        }

        public string GetItemValue(XmlNode parentNode, bool IsInternalPluginUsed, string configKey)
        {
            if (parentNode == null)
                return "";
            else
            {
                string attrResult = Util_XmlOperHelper.GetNodeValue("", parentNode);
                string Result = attrResult;
                if (IsInternalPluginUsed && refInternalPlugin != null)
                {
                    Dictionary<string, string> paramsDirc = new Dictionary<string, string>();
                    paramsDirc.Add("key", configKey);
                    paramsDirc.Add("value", attrResult);
                    paramsDirc.Add("return", "");
                    refInternalPlugin.actionGet(paramsDirc);
                    Result = paramsDirc["return"].ToString();
                }
                return Result;
            }
        }

        public string GetItemValue(XmlNode parentNode)
        {
            if (parentNode == null)
                return "";
            else
            {
                string attrResult = Util_XmlOperHelper.GetNodeValue("", parentNode);
                string Result = attrResult;
                if (isDesMode)
                    Result = _objectDes.AesDecrypt(attrResult);
                return Result;
            }
        }

        public bool RemoveSession(string SessionName)
        {
            if (IsSessionExisted(SessionName))
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                if (activeSessionNode != null)
                {
                    activeSessionNode.ParentNode.RemoveChild(activeSessionNode);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public bool RemoveSessionItem(string SessionName, string SessionItemName)
        {
            if (SessionName != "" && SessionItemName != "")
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                if (activeSessionNode != null)
                {
                    XmlNode itemNode = GetItemNode(SessionName, SessionItemName);
                    if (itemNode != null)
                    {
                        activeSessionNode.RemoveChild(itemNode);
                        return true;
                    }
                    else
                        return false;

                }
                else
                    return false;
            }
            else
                return false;
        }

        public bool RemoveActiveItem(XmlNode ParentNode, string ActiveItemName)
        {
            if (ParentNode == null)
                return false;
            else
            {
                XmlNode activeItemNode = ParentNode.SelectSingleNode("item[@name='" + ActiveItemName + "']");
                if (activeItemNode != null)
                {
                    ParentNode.RemoveChild(activeItemNode);
                    return true;
                }
                else
                    return false;
            }
        }

        public bool RemoveSessionAttr(string SessionName, string SessionAttrName)
        {
            if (IsSessionExisted(SessionName))
            {
                XmlNode activeSessionNode = GetSessionNode(SessionName);
                XmlAttribute activeAttr = activeSessionNode.Attributes[SessionAttrName];
                if (activeAttr != null)
                {
                    activeSessionNode.Attributes.Remove(activeAttr);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

    }
}
