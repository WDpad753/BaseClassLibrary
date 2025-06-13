using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.MethodNameExtractor
{
    public static class FuncNameExtractor
    {
        public static string GetMethodName([CallerMemberName] string? MethodName = null)
        {
            return MethodName ?? string.Empty;
        }
    }
}
