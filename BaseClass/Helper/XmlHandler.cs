using BaseLogger;
using BaseLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using UtilityClass = BaseClass.MethodNameExtractor.FuncNameExtractor;

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

                //using (var stream = File.OpenRead(_filePath))
                //{
                //    XDocument xdoc = XDocument.Load(stream);

                //    XElement targetNode = xdoc.Descendants(mainKey).FirstOrDefault();
                //    if (targetNode == null)
                //    {
                //        _logWriter.LogWrite($"No element named '{mainKey}' found.", this.GetType().Name, nameof(XmlWrite), MessageLevels.Fatal);
                //        return;
                //    }

                //    XElement found = targetNode.Descendants().FirstOrDefault(el => string.Equals(el.Attribute("key")?.Value, key, StringComparison.OrdinalIgnoreCase));

                //    XElement container = targetNode.Elements().FirstOrDefault(child => child.Elements().Any(el => el.Attribute("key") != null)) ?? targetNode;

                //    if (found != null)
                //    {
                //        // Update its 'value' attribute
                //        found.SetAttributeValue("value", value);
                //        _logWriter.LogWrite($"Updated <{found.Name} key=\"{key}\"> under <{mainKey}> to value=\"{value}\".", this.GetType().Name, nameof(XmlWrite), MessageLevels.Verbose);
                //    }
                //    else
                //    {
                //        // Not found: add a new element under targetNode.
                //        // Decide a tag name for the new element, e.g. <add> or <setting> or something generic like <entry>
                //        XElement newElem = new XElement("add", new XAttribute("key", key), new XAttribute("value", value));

                //        if (container != null)
                //        {
                //            container.Add(newElem);
                //        }
                //        else
                //        {
                //            targetNode.Add(newElem);
                //        }

                //        _logWriter.LogWrite($"Added <add key=\"{key}\" value=\"{value}\"/> under <{mainKey}>.", this.GetType().Name, nameof(XmlWrite), MessageLevels.Verbose);
                //    }

                //    //XElement targetNode = xdoc.Descendants(mainKey).FirstOrDefault();
                //    //if (targetNode == null)
                //    //{
                //    //    Console.WriteLine($"No element named '{mainKey}' found.");
                //    //    return;
                //    //}

                //    //XElement node = targetNode.Descendants("add").FirstOrDefault(el => string.Equals(el.Attribute("key")?.Value, key,
                //    //    StringComparison.OrdinalIgnoreCase));

                //    //XElement node = xdoc.Descendants()  // all elements anywhere
                //    //                     .FirstOrDefault(el => string.Equals(el.Attribute("key")?.Value, key, 
                //    //                      StringComparison.OrdinalIgnoreCase));


                //    //if (node != null)
                //    //{
                //    //    // update existing
                //    //    node.SetAttributeValue("value", value);
                //    //    xdoc.Save(_filePath);
                //    //    _logWriter.LogWrite($"Updated <add key=\"{key}\"> under <{mainKey}>", this.GetType().Name, nameof(XmlWrite), MessageLevels.Log);
                //    //}
                //    //else
                //    //{
                //    //    XElement container = xdoc.Root;
                //    //    if (container == null)
                //    //    {
                //    //        _logWriter.LogWrite("XML has no root; cannot add.", this.GetType().Name, nameof(XmlWrite), MessageLevels.Fatal);
                //    //        return;
                //    //    }

                //    //    // Example: add directly under root as <entry key="..." value="..."/>
                //    //    container.Add(new XElement("entry",
                //    //        new XAttribute("key", key),
                //    //        new XAttribute("value", value)));
                //    //    _logWriter.LogWrite($"Added <entry key=\"{key}\" value=\"{value}\"/> under <{container.Name}>.", this.GetType().Name, nameof(XmlWrite), MessageLevels.Log);

                //    //    //// add new <add> under targetNode
                //    //    //targetNode.Add(new XElement("add",
                //    //    //    new XAttribute("key", key),
                //    //    //    new XAttribute("value", value)));
                //    //    //xdoc.Save(_filePath);
                //    //    //Console.WriteLine($"Added <add key=\"{key}\" value=\"{value}\"/> under <{mainKey}>");
                //    //}

                //    xdoc.Save(_filePath);
                //}

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
                    // Update its 'value' attribute
                    found.SetAttributeValue("value", value);
                    _logWriter.LogWrite($"Updated <{found.Name} key=\"{key}\"> under <{mainKey}> to value=\"{value}\".", this.GetType().Name, nameof(XmlWrite), MessageLevels.Verbose);
                }
                else
                {
                    // Not found: add a new element under targetNode.
                    // Decide a tag name for the new element, e.g. <add> or <setting> or something generic like <entry>
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

                //XElement targetNode = xdoc.Descendants(mainKey).FirstOrDefault();
                //if (targetNode == null)
                //{
                //    Console.WriteLine($"No element named '{mainKey}' found.");
                //    return;
                //}

                //XElement node = targetNode.Descendants("add").FirstOrDefault(el => string.Equals(el.Attribute("key")?.Value, key,
                //    StringComparison.OrdinalIgnoreCase));

                //XElement node = xdoc.Descendants()  // all elements anywhere
                //                     .FirstOrDefault(el => string.Equals(el.Attribute("key")?.Value, key, 
                //                      StringComparison.OrdinalIgnoreCase));


                //if (node != null)
                //{
                //    // update existing
                //    node.SetAttributeValue("value", value);
                //    xdoc.Save(_filePath);
                //    _logWriter.LogWrite($"Updated <add key=\"{key}\"> under <{mainKey}>", this.GetType().Name, nameof(XmlWrite), MessageLevels.Log);
                //}
                //else
                //{
                //    XElement container = xdoc.Root;
                //    if (container == null)
                //    {
                //        _logWriter.LogWrite("XML has no root; cannot add.", this.GetType().Name, nameof(XmlWrite), MessageLevels.Fatal);
                //        return;
                //    }

                //    // Example: add directly under root as <entry key="..." value="..."/>
                //    container.Add(new XElement("entry",
                //        new XAttribute("key", key),
                //        new XAttribute("value", value)));
                //    _logWriter.LogWrite($"Added <entry key=\"{key}\" value=\"{value}\"/> under <{container.Name}>.", this.GetType().Name, nameof(XmlWrite), MessageLevels.Log);

                //    //// add new <add> under targetNode
                //    //targetNode.Add(new XElement("add",
                //    //    new XAttribute("key", key),
                //    //    new XAttribute("value", value)));
                //    //xdoc.Save(_filePath);
                //    //Console.WriteLine($"Added <add key=\"{key}\" value=\"{value}\"/> under <{mainKey}>");
                //}

                xdoc.Save(_filePath);
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Exception Occurred: {ex.Message}", this.GetType().Name, nameof(XmlWrite), MessageLevels.Fatal);
            }
        }
    }
}
