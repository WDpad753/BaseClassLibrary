using BaseLogger;
using BaseLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FuncName = BaseClass.MethodNameExtractor.FuncNameExtractor;


namespace BaseClass.Helper
{
    public static class StringHandler
    {
        //private LogWriter _logWriter;

        //public StringHandler(LogWriter Logger)
        //{
        //    _logWriter = Logger;
        //}

        public static List<string>? FindTextDifference(string s1, string s2, string separator)
        {
            try
            {
                List<string> diff;
                IEnumerable<string> set1 = s1.Split(separator).Distinct();
                IEnumerable<string> set2 = s2.Split(separator).Distinct();

                if (set2.Count() > set1.Count())
                {
                    diff = set2.Except(set1).ToList();
                }
                else
                {
                    diff = set1.Except(set2).ToList();
                }

                return diff;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.ToString());
                return null;
            }
        }

        public static string? RemoveDublicatesInString(string? s, string? separator)
        {
            try
            {
                if (s == null)
                {
                    //_logWriter.LogWrite("Error in examination for dublicates. Entered value is empty", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                    throw new Exception("Error in examination for dublicates. Entered value is empty");

                    //return null;
                }
                else if(s.Length > 0 && separator == null)
                {
                    //_logWriter.LogWrite("Error in examination for dublicates. Function requires separator", this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                    throw new Exception("Error in examination for dublicates. Function requires separator");

                    //return null;
                }

                string[] strings = s.Split(separator);
                strings = strings.Distinct().ToArray();
                return string.Join(separator, strings);
            }
            catch (Exception ex)
            {
                //_logWriter.LogWrite("Error reading data to find difference between two strings. Exception Message: " + ex, this.GetType().Name, FuncName.GetMethodName(), MessageLevels.Fatal);
                throw new Exception(ex.ToString());
            }
        }
    }
}
