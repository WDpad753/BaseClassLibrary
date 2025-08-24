using BaseClass.Base.Interface;
using BaseLogger;
using BaseLogger.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;


namespace BaseClass.Helper
{
    public class XmlHandler
    {
        private readonly IBase baseConfig;
        private ILogger? _logWriter;
        private string? _filePath;
        private List<string> fileExtensions = new List<string>() { ".xml", ".config" };

        //public XmlHandler(LogWriter Logger, string? FilePath) 
        //{
        //    _logWriter = Logger;
        //    _filePath = FilePath;
        //}
        public XmlHandler(IBase? BaseConfig) 
        {
            baseConfig = BaseConfig;
            _logWriter = BaseConfig.Logger;

            if(BaseConfig.FilePath != null && fileExtensions.Contains(Path.GetExtension(BaseConfig.FilePath)))
            {
                _filePath = BaseConfig.FilePath;
            }
            else if(BaseConfig.ConfigPath != null && fileExtensions.Contains(Path.GetExtension(BaseConfig.ConfigPath)))
            {
                _filePath = BaseConfig.ConfigPath;
            }
            else if(BaseConfig.FilePath == null && BaseConfig.ConfigPath != null && fileExtensions.Contains(Path.GetExtension(BaseConfig.ConfigPath)))
            {
                _filePath = BaseConfig.ConfigPath;
            }
            else
            {
                throw new ArgumentException("File type is unknown will be stopping the process. Entered Path => {}");
            }
            //_filePath = BaseConfig.FilePath == null;
        }

        public void XmlWrite(string mainKey, string key, string value)
        {
            try
            {
                if(_filePath == null)
                    _filePath = baseConfig.FilePath;

                if (!File.Exists(_filePath) && (!string.Equals(Path.GetExtension(_filePath), ".xml", StringComparison.OrdinalIgnoreCase) || !fileExtensions.Contains(Path.GetExtension(_filePath))))
                {
                    _logWriter.LogError($"XML File does not exist in the given path. Path => {_filePath}");
                    return;
                }

                XDocument xdoc = XDocument.Load(_filePath);

                XElement targetNode = xdoc.Descendants(mainKey).FirstOrDefault();
                if (targetNode == null)
                {
                    _logWriter.LogError($"No element named '{mainKey}' found.");
                    return;
                }

                //XElement found = targetNode.Descendants().FirstOrDefault(el => string.Equals(el.Attribute("key")?.Value, key, StringComparison.OrdinalIgnoreCase));
                XElement? found = targetNode.Descendants().FirstOrDefault(el => string.Equals(el.Attribute("key")?.Value, key, StringComparison.OrdinalIgnoreCase) || 
                                    string.Equals(el.Attribute("Key")?.Value, key, StringComparison.OrdinalIgnoreCase)) ??
                                    targetNode.Descendants().FirstOrDefault(el => string.Equals(el.Name.LocalName, key, StringComparison.OrdinalIgnoreCase));

                XElement? container = targetNode.Elements().FirstOrDefault(child => child.Elements().Any(el => el.Attribute("key") != null) || child.Elements().Any(el => el.Attribute("Key") != null)) ??
                    targetNode.Descendants().FirstOrDefault(el => !string.IsNullOrEmpty(el.Name.LocalName)) ?? targetNode;

                if (found != null)
                {
                    if(targetNode.Descendants().FirstOrDefault(el => string.Equals(el.Name.LocalName, key, StringComparison.OrdinalIgnoreCase))?.Name.LocalName == key)
                    {
                        found.Value = value;
                    }
                    else
                    {
                        found.SetAttributeValue("value", value);
                    }
                    _logWriter.LogDebug($"Updated <{found.Name} key=\"{key}\"> under <{mainKey}> to value=\"{value}\".");
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

                    _logWriter.LogDebug($"Added <add key=\"{key}\" value=\"{value}\"/> under <{mainKey}>.");
                }

                xdoc.Save(_filePath);
            }
            catch (Exception ex)
            {
                _logWriter.LogError($"Exception Occurred: {ex.Message}");
            }
        }
    }
}
