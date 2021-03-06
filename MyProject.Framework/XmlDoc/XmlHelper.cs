﻿using MyProject.Framework.XmlDoc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace MyProject.Framework.XmlDoc
{
    public static class XmlHelper
    {
        private static XmlDocument xmlRoot;


        public static DataRoot DataRoot { get; set; }

        /// <summary>
        /// 用于SQL配置文件的读取
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <returns></returns>
        public static void GetXmlDocuments(string path)
        {
            IEnumerable<FileInfo> fileInfos = ConfigFile(path);

            xmlRoot  =  NewXmlConfig();

            foreach (FileInfo fileInfo in fileInfos)
            {
                using (TextReader stream = fileInfo.OpenText())
                {
                    try
                    {
                        XmlDocument doc = new XmlDocument();
                        doc.Load(stream);
                        XmlNodeList xmlNodeList = doc.SelectNodes("//dataCommand");

                        foreach (XmlNode xmlNode in xmlNodeList)
                        {
                            if (!IsExistName(xmlNode.Attributes["name"].Value))
                            {
                                xmlRoot.LastChild.AppendChild(
                                    xmlRoot.LastChild.OwnerDocument.ImportNode(xmlNode,true));
                            }
                        }
                    }
                    catch (XmlException xmlEx)
                    {
                        throw xmlEx;
                    }
                    catch (InvalidOperationException ioeEx)
                    {
                        throw ioeEx;
                    }
                    catch (NotSupportedException nosEx)
                    {
                        throw nosEx;
                    }
                } 
            }
            DataRoot = CreateDataRoot();
        }

        private static XmlDocument NewXmlConfig()
        {
            XmlDocument xmlRoot = new XmlDocument();
            XmlNode xmlNode = xmlRoot.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlRoot.AppendChild(xmlNode);

            XmlElement xmlElement = xmlRoot.CreateElement("dataRoot");
            xmlRoot.AppendChild(xmlElement);
            return xmlRoot;
        }

        private static bool IsExistName(string name)
        {
            bool isExist = false;
       
            foreach (XmlNode item in xmlRoot.ChildNodes)
            {
                if (item.Attributes != null && item.Attributes["name"] != null)
                {
                    isExist = item.Attributes["name"].Value == name;
                    if (isExist) throw new Exception("dataCommand的name属性值不能重复");
                }
            }

            return isExist;
        }

        private static DataRoot CreateDataRoot()
        {
            DataRoot dataRoot = null;
            try
            {
                using (StringReader stringReader = new StringReader(xmlRoot.InnerXml))
                {
                    XmlReader xmlReader = XmlReader.Create(stringReader);

                    XmlSerializer serializer = new XmlSerializer(typeof(DataRoot));

                    dataRoot = serializer.Deserialize(xmlReader) as DataRoot;
                }
            }
            catch (ArgumentNullException anEx)
            {
                throw anEx;
            }
            catch (InvalidOperationException inoEx)
            {
                throw inoEx;
            }
            return dataRoot;
        }

        private static IEnumerable<FileInfo> ConfigFile(string path)
        {
            FileInfo[] fileInfos = null;
            DirectoryInfo directory = Directory.CreateDirectory(path);

            if (directory.Exists)
            {
                fileInfos =directory.GetFiles("Sql_*.config");
                if (fileInfos.Length == 0)
                {
                    throw new DirectoryNotFoundException("文件不存在");
                }
            }
            else
            {
                throw new DirectoryNotFoundException("目录不存在");
            }
            return fileInfos;
        }
    }
}
