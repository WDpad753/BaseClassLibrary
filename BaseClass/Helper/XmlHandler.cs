using BaseLogger;
using BaseLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BaseClass.Helper
{
    public class XmlHandler
    {
        private LogWriter _logWriter;
        
        public XmlHandler(LogWriter Logger) 
        {
            _logWriter = Logger;
        }

        public void XmlWrite(string filepath, string key, string value)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                if (File.Exists(filepath))
                {
                    doc.Load(filepath);
                }
                else
                {
                    XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    doc.AppendChild(xmlDeclaration);
                    XmlElement root = doc.CreateElement("root");
                    doc.AppendChild(root);
                }

                XmlNode node = doc.SelectSingleNode($"/root/{key}");
                if (node == null)
                {
                    node = doc.CreateElement(key);
                    doc.DocumentElement.AppendChild(node);
                }

                node.InnerText = value;
                doc.Save(filepath);
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Exception Occurred: {ex.Message}", this.GetType().Name, nameof(XmlWrite), MessageLevels.Fatal);
            }
        }
    }
}
