using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Helper
{
    public static class RegistryValueHandler
    {
        //public static T? RegistrySetValue<T>(object? value, RegistryValueKind? valueKind)
        public static object? RegistrySetValue(object? value, RegistryValueKind? valueKind)
        {
            try
            {
                if (value is null)
                    return default;

                object? converted = value;

                switch (valueKind)
                {
                    case RegistryValueKind.String:
                        converted = Convert.ToString(value) ?? string.Empty;
                        break;
                    case RegistryValueKind.ExpandString:
                        converted = Convert.ToString(value) ?? string.Empty;
                        break;
                    case RegistryValueKind.Binary:
                        if (value is byte[] b)
                            converted = b;
                        else if (value is string s)
                            converted = Convert.FromBase64String(s);
                        else
                            throw new InvalidCastException("Value must be byte[] or Base64 string for Binary.");
                        break;
                    case RegistryValueKind.DWord:
                        converted = Convert.ToInt32(value);
                        break;
                    case RegistryValueKind.MultiString:
                        if (value is string[] arr)
                            converted = arr;
                        else
                            converted = new[] { Convert.ToString(value) ?? string.Empty };
                        break;
                    case RegistryValueKind.QWord:
                        converted = Convert.ToInt64(value);
                        break;
                    case RegistryValueKind.None:
                        converted = value;
                        break;
                    default:
                        throw new ArgumentException("Unknown Set Value Type.");
                }

                //return (T?)converted;
                return converted;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert value for {valueKind}. Value => {Convert.ToString(value)}", ex);
            }
        }
    }
}