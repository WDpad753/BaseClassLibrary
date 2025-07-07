using BaseLogger;
using BaseLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using FuncName = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.Helper
{
    public class XmlHandler
    {
        private LogWriter _logWriter;
        private string? _filePath;

        public XmlHandler(LogWriter Logger, string? FilePath) 
        {
            _logWriter = Logger;
            _filePath = FilePath;
        }

        public void XmlWrite(string mainKey, string key, string value)
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    _logWriter.LogWrite($"XML File does not exist in the given path. Path => {_filePath}", this.GetType().Name, nameof(XmlWrite), MessageLevels.Fatal);
                }

                XDocument xdoc = XDocument.Load(_filePath);

                XElement targetNode = xdoc.Descendants(mainKey).FirstOrDefault();
                if (targetNode == null)
                {
                    _logWriter.LogWrite($"No element named '{mainKey}' found.", this.GetType().Name, nameof(XmlWrite), MessageLevels.Fatal);
                    return;
                }

                XElement found = targetNode.Descendants().FirstOrDefault(el => string.Equals(el.Attribute("key")?.Value, key, StringComparison.OrdinalIgnoreCase));

                XElement container = targetNode.Elements().FirstOrDefault(child => child.Elements().Any(el => el.Attribute("key") != null)) ?? targetNode;

                if (found != null)
                {
                    found.SetAttributeValue("value", value);
                    _logWriter.LogWrite($"Updated <{found.Name} key=\"{key}\"> under <{mainKey}> to value=\"{value}\".", this.GetType().Name, nameof(XmlWrite), MessageLevels.Verbose);
                }
                else
                {
                    XElement newElem = new XElement("add", new XAttribute("key", key), new XAttribute("value", value));

                    if (container != null)
                    {
                        container.Add(newElem);
                    }
                    else
                    {
                        targetNode.Add(newElem);
                    }

                    _logWriter.LogWrite($"Added <add key=\"{key}\" value=\"{value}\"/> under <{mainKey}>.", this.GetType().Name, nameof(XmlWrite), MessageLevels.Verbose);
                }

                xdoc.Save(_filePath);
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Exception Occurred: {ex.Message}", this.GetType().Name, nameof(XmlWrite), MessageLevels.Fatal);
            }
        }
    }
}
