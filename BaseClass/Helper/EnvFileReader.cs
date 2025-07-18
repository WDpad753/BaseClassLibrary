using BaseLogger;
using BaseLogger.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Formatting = Newtonsoft.Json.Formatting;
using FuncName = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.Helper
{
    public class EnvFileReader
    {
        private string? _filepath;
        private LogWriter _writer;
        private JsonSerializer _serializer;
        private XmlHandler _handler;
        private string? result;

        public EnvFileReader(LogWriter Logger) 
        {
            _writer = Logger;
            _serializer = new();
        }

        public string? EnvFileRead(string filepath, string key, string mainkey)
        {
            _filepath = filepath;
            string? data = null;
            string ext = Path.GetExtension(filepath).ToLower().Substring(1, Path.GetExtension(filepath).Length-1);

            switch (ext)
            {
                case "json":
                    JsonEnvFileReader(key, mainkey);
                    data = result;
                    break;
                case "env":
                    envEnvFileReader(key, filepath);
                    data = result;
                    break;
                case "xml":
                    XmlEnvFileReader(key, mainkey);
                    data = result;
                    break;
                default:
                    _writer.LogWrite($"Exception Occured. Exception: Unspported Environment File Extension, Exiting the Method.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                    return null;
            }

            return data;
        }

        public void EnvFileSave(string filepath, string key, string mainkey, string data)
        {
            _filepath = filepath;
            string ext = Path.GetExtension(filepath).ToLower().Substring(1, Path.GetExtension(filepath).Length-1);

            switch (ext)
            {
                case "json":
                    JsonEnvFileReader(key, mainkey, data);
                    break;
                case "env":
                    envEnvFileReader(key, filepath, data);
                    break;
                case "xml":
                    XmlEnvFileReader(key, mainkey, data);
                    break;
                default:
                    _writer.LogWrite($"Exception Occured. Exception: Unspported Environment File Extension, Exiting the Method.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                    return;
            }

            return;
        }

        private void JsonEnvFileReader(string key, string mainKey, string? data = null)
        {
            try
            {
                string? res = null;
                JObject? JsonVal = null;

                using (var streamreader = new StreamReader(_filepath))
                {
                    using (var textreader = new JsonTextReader(streamreader))
                    {
                        var val = _serializer.Deserialize<JObject>(textreader);

                        if (val != null && val is JObject jsonVal)
                        {
                            JsonVal = jsonVal;

                            // All JProperty tokens in the tree:
                            var allProps = jsonVal.DescendantsAndSelf().OfType<JProperty>();

                            // Filter by property name:
                            var matchingProps = allProps.Where(p => p.Name.Equals(mainKey, StringComparison.OrdinalIgnoreCase));

                            foreach (var prop in matchingProps)
                            {
                                // All JProperty tokens in the nested tree:
                                var nestedprops = prop.DescendantsAndSelf().OfType<JProperty>();

                                // Filter by property name:
                                var matchingNestedProps = nestedprops.Where(p => p.Name.Equals(key, StringComparison.OrdinalIgnoreCase));

                                foreach(var envKeyVal in matchingNestedProps)
                                {
                                    JToken keyVal = new JObject(envKeyVal);

                                    if(data != null)
                                    {
                                        envKeyVal.Value.Replace(data);
                                    }
                                    else
                                    {
                                        res = keyVal[key].ToString();
                                        result = res;
                                    }
                                }
                            }
                        }
                    }
                }

                if(data != null)
                {
                    File.WriteAllText(_filepath, JsonVal.ToString(Formatting.Indented));
                }

                return;
            }
            catch (Exception ex)
            {
                _writer.LogWrite($"Exception Occured. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                return;
            }
        }

        private void envEnvFileReader(string Key, string filePath, string? data = null)
        {
            try
            {
                string? res = null;
                var lines = File.ReadAllLines(filePath).ToList();

                if (!File.Exists(filePath))
                    _writer.LogWrite($"The file '{filePath}' does not exist.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Log);


                foreach (var line in File.ReadAllLines(filePath))
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                        continue; // Skip empty lines and comments

                    var parts = line.Split('=', 2);
                    if (parts.Length != 2)
                        continue; // Skip lines that are not key-value pairs

                    var key = parts[0].Trim();

                    if(key.Equals(Key, StringComparison.OrdinalIgnoreCase))
                    {
                        res = parts[1].Trim().Replace("\"", ""); // Remove quotes if present

                        if (data != null)
                        {
                            var newline = $"{Key}={data}";

                            int idx = lines.FindIndex(ln => ln.StartsWith(Key + "=", StringComparison.OrdinalIgnoreCase));
                            if (idx >= 0)
                            {
                                lines[idx] = newline;
                            }
                            else
                            {
                                lines.Add(newline);
                            }

                            File.WriteAllLines(_filepath, lines);
                        }
                        else
                        {
                            result = res;
                        }


                        break; // Exit loop once the key is found
                    }
                }

                if(res == null)
                    _writer.LogWrite($"Key '{Key}' not found in the environment file '{filePath}'.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Verbose);

                return;
            }
            catch (Exception ex)
            {
                _writer.LogWrite($"Exception Occured. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                return;
            }
        }

        private void XmlEnvFileReader(string key, string mainKey, string? data = null)
        {
            try
            {
                string? res = null;
                bool mainKeyFound = false;

                if (!File.Exists(_filepath))
                    _writer.LogWrite($"The file '{_filepath}' does not exist.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Log);

                //XDocument xdoc = XDocument.Load(_filepath);
                
                //XElement targetNode = xdoc.Descendants(mainKey).FirstOrDefault();
                //if (targetNode == null)
                //{
                //    _writer.LogWrite($"No element named '{mainKey}' found.", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                //    return;
                //}

                //XElement? found = targetNode.Descendants().FirstOrDefault(el => string.Equals(el.Attribute("Key")?.Value, key, StringComparison.OrdinalIgnoreCase)) == null ?
                //    targetNode.Descendants().FirstOrDefault(el => string.Equals(el.Name.LocalName, key, StringComparison.OrdinalIgnoreCase)) :
                //    null;

                //XElement container = targetNode.Elements().FirstOrDefault(child => child.Elements().Any(el => el.Attribute("key") != null)) ?? targetNode;

                ////if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals(mainKey, StringComparison.OrdinalIgnoreCase))
                ////{
                ////    mainKeyFound = true; // Main key found, start looking for the nested key
                ////}

                if(data != null)
                {
                    _handler = new(_writer, _filepath);

                    _handler.XmlWrite(mainKey,key, data);
                }
                else
                {
                    using (XmlReader reader = XmlReader.Create(_filepath))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals(mainKey, StringComparison.OrdinalIgnoreCase))
                            {
                                mainKeyFound = true; // Main key found, start looking for the nested key
                            }

                            if (mainKeyFound && reader.NodeType == XmlNodeType.Element && reader.Name.Equals("add", StringComparison.OrdinalIgnoreCase))
                            {
                                string? keyAttr = reader.GetAttribute("Key");
                                string? valueAttr = reader.GetAttribute("value");

                                if (keyAttr == key)
                                {
                                    res = valueAttr;
                                    result = res;
                                    break;
                                }
                            }
                            else if (mainKeyFound && reader.NodeType == XmlNodeType.Element && reader.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                            {
                                string value = reader.ReadElementContentAsString();
                                res = value;
                                result = res;
                                break;
                            }
                        }
                    }
                }


                return;
            }
            catch (Exception ex)
            {
                _writer.LogWrite($"Exception Occured. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                return;
            }
        }
    }
}
