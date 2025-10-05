using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                    if (method?.Name == ".ctor")
                        return method?.DeclaringType.Name;

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
                            return realType.Name;
                    }

                    return type?.Name;
                }
            }

            return string.Empty;
        }
    }
}
