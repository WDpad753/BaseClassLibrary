using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BaseLogger.Models
{
    public static class FuncName
    {
        public static string GetMethodName()
        {
            var stackTrace = new StackTrace();
            foreach (var frame in stackTrace.GetFrames() ?? Array.Empty<StackFrame>())
            {
                var method = frame?.GetMethod();
                var type = method?.DeclaringType;

                if (type == null)
                    continue;

                var fullName = type.FullName;

                if (!fullName.StartsWith("BaseLogger.Logger") && !fullName.StartsWith("BaseLogger.Models"))
                {
                    string? methodName = method?.DeclaringType.Name;

                    if (method?.Name == ".ctor")
                        return methodName;

                    if (methodName.StartsWith("<") && methodName.Contains(">"))
                    {
                        //var realType = methodName;
                        int indexStart = methodName.IndexOf("<");
                        int indexEnd = methodName.IndexOf(">");
                        int length = indexEnd - (indexStart+1);

                        return methodName.Substring((indexStart + 1), length);
                    }

                    return method?.Name;
                }
            }

            return string.Empty;
        }

        public static string GetCallingClassName()
        {
            var stackTrace = new StackTrace();
            foreach(var frame in stackTrace.GetFrames() ?? Array.Empty<StackFrame>())
            {
                var method = frame?.GetMethod();
                var type = method?.DeclaringType;

                if (type == null)
                    continue;

                var fullName = type.FullName;

                if (!fullName.StartsWith("BaseLogger.Logger") && !fullName.StartsWith("BaseLogger.Models"))
                {
                    if (type.Name.StartsWith("<") && type.Name.Contains(">"))
                    {
                        var realType = type.DeclaringType;
                        
                        if (realType != null)
                        {
                            string typeName = realType.Name;

                            if (typeName.Contains("`"))
                            {
                                int index = typeName.IndexOf("`");
                                typeName = typeName.Substring(0, index);
                            }

                            return typeName;
                        }

                    }

                    return type?.Name;
                }
            }

            return string.Empty;
        }
    }
}
