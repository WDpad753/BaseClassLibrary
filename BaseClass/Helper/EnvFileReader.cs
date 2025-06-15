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
                    data = envEnvFileReader(key, mainkey);
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

        private string? envEnvFileReader(string key, string mainKey)
        {
            try
            {
                string? res = null;

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
