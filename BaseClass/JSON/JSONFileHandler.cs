using BaseLogger;
using BaseLogger.Models;

//using Common.Abstractions;
//using Common.Abstractions.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UtilityClass = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.JSON
{
    public class JSONFileHandler
    {
        //private readonly ILogWriter _logWriter;
        //private DebugState _debugState;
        //private string? NameSpace = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
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
                //if (!EventLog.SourceExists("ChangeLogWriterClass"))
                //{
                //    EventLog.CreateEventSource("ChangeLogWriterClass", "TagImportUI");
                //}

                //using (EventLog eventLog = new EventLog())
                //{
                //    eventLog.Source = "ChangeLogWriterClass";
                //    eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                //}

                _logWriter.LogWrite("Error saving data to file: " + ex.Message, this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
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
                using (StreamReader file = File.OpenText(targetfilepath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    T? json = (T?)serializer.Deserialize(file, typeof(T));
                    return json;
                }
            }
            catch (Exception ex)
            {
                //if (!EventLog.SourceExists("ChangeLogWriterClass"))
                //{
                //    EventLog.CreateEventSource("ChangeLogWriterClass", "TagImportUI");
                //}

                //using (EventLog eventLog = new EventLog())
                //{
                //    eventLog.Source = "ChangeLogWriterClass";
                //    eventLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                //}

                _logWriter.LogWrite("Error reading data to file: " + ex.Message, this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
        }
    }
}
