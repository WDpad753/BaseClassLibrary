using BaseLogger;
using BaseLogger.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UtilityClass = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.Helper
{
    public class EnvFileReader
    {
        private string? _filepath;
        private LogWriter _writer;
        private JsonSerializer _serializer;

        public EnvFileReader(LogWriter Logger) 
        {
            _writer = Logger;
            _serializer = new JsonSerializer();
        }

        public string? EnvFileRead(string filepath, string key, string mainkey)
        {
            _filepath = filepath;
            string? data = null;
            string ext = Path.GetExtension(filepath).ToLower().Substring(1, Path.GetExtension(filepath).Length-1);

            switch (ext)
            {
                case "json":
                    data = JsonEnvFileReader(key, mainkey);
                    break;
                case "env":
                    data = envEnvFileReader(key, filepath);
                    break;
                case "xml":
                    data = XmlEnvFileReader(key, mainkey);
                    break;
                default:
                    _writer.LogWrite($"Exception Occured. Exception: Unspported Environment File Extension, Exiting the Method.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                    return null;
            }

            return data;
        }

        private string? JsonEnvFileReader(string key, string mainKey)
        {
            try
            {
                string? res = null;

                using (var streamreader = new StreamReader(_filepath))
                {
                    using (var textreader = new JsonTextReader(streamreader))
                    {
                        var val = _serializer.Deserialize<JObject>(textreader);

                        if (val != null && val is JObject jsonVal)
                        {
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
                                    res = keyVal[key].ToString();
                                }
                            }
                        }
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                _writer.LogWrite($"Exception Occured. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
        }

        private string? envEnvFileReader(string Key, string filePath)
        {
            try
            {
                string? res = null;

                if (!File.Exists(filePath))
                    _writer.LogWrite($"The file '{filePath}' does not exist.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Log);


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
                        break; // Exit loop once the key is found
                    }
                }

                if(res == null)
                    _writer.LogWrite($"Key '{Key}' not found in the environment file '{filePath}'.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Verbose);

                return res;
            }
            catch (Exception ex)
            {
                _writer.LogWrite($"Exception Occured. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
        }

        private string? XmlEnvFileReader(string key, string mainKey)
        {
            try
            {
                string? res = null;
                bool mainKeyFound = false;

                if (!File.Exists(_filepath))
                    _writer.LogWrite($"The file '{_filepath}' does not exist.", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Log);

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
                                break;
                            }
                        }
                        else if (mainKeyFound && reader.NodeType == XmlNodeType.Element && reader.Name.Equals(key, StringComparison.OrdinalIgnoreCase))
                        {
                            string value = reader.ReadElementContentAsString();
                            res = value;
                            break;
                        }
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                _writer.LogWrite($"Exception Occured. Exception:{ex.InnerException}; Stack: {ex.StackTrace}; Message: {ex.Message}; Data: {ex.Data}; Source: {ex.Source}", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
        }
    }
}
