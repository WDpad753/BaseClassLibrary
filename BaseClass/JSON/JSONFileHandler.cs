using BaseLogger;
using BaseLogger.Models;
using Newtonsoft.Json;
using System;
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
        private LogWriter _logWriter;

        public JSONFileHandler(LogWriter Logger) 
        {
            _logWriter = Logger;
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

        public T? GetJson<T>(string targetfilepath) where T : class
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
                    return null;  
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
                return null;
            }
        }
    }
}
