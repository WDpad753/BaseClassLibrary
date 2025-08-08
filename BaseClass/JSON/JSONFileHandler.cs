using BaseClass.Base.Interface;
using BaseLogger;
using BaseLogger.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FuncName = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.JSON
{
    public class JSONFileHandler
    {
        private readonly IBase? baseConfig;
        private LogWriter? _logWriter;

        //public JSONFileHandler(LogWriter Logger) 
        //{
        //    _logWriter = Logger;
        //}
        public JSONFileHandler(IBase? baseConfig) 
        {
            _logWriter = baseConfig.Logger;
        }

        public void SaveJson<T>(T json, string targetfilepath)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(targetfilepath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetfilepath));
                }

                // serialize JSON to a string and then write string to a file
                using (StreamWriter file = File.CreateText(targetfilepath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, json);
                }
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite("Error saving data to file: " + ex.Message, this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                return;
            }
        }

        public T? GetJson<T>(string targetfilepath)
        {
            try
            {
                // read file into a string and deserialize JSON to a type
                if (targetfilepath.Contains(@"\\\\"))
                {
                    targetfilepath = Regex.Replace(targetfilepath.ToString(), targetfilepath.ToString(), @"\\");
                }

                if (!File.Exists(targetfilepath))
                {
                    _logWriter.LogWrite($"File not found: {targetfilepath}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);
                    return default;  
                }

                using (StreamReader file = File.OpenText(targetfilepath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    T? json = (T?)serializer.Deserialize(file, typeof(T));
                    return json;
                }
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite("Error reading data to file: " + ex.Message, this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                return default;
            }
        }

        public IEnumerable<JProperty> ValueSearch(string Json, string KeyName)
        {
            try
            {
                using var reader = new JsonTextReader(new StringReader(Json));
                var JsonObject = JObject.Load(reader);

                //var JsonObject = JObject.Parse(Json);

                var JsonSearch = JsonObject.DescendantsAndSelf().OfType<JProperty>().Where(el => el.Path.Contains(KeyName, StringComparison.OrdinalIgnoreCase));

                if(JsonSearch.Any())
                {
                    _logWriter.LogWrite($"Found some matches based on the given Key ({KeyName}). Matches Count: {JsonSearch.Count()}", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Debug);
                }
                else
                {
                    _logWriter.LogWrite($"Unable to find any entries that ties to the inserted Key ({KeyName}).", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Verbose);
                    return Enumerable.Empty<JProperty>();
                }

                return JsonSearch;
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite("Error searching for value in the input JSON: " + ex.Message, this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                return Enumerable.Empty<JProperty>();
            }
        }
    }
}
