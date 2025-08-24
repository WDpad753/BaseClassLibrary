using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Helper
{
    public static class RegistryValueHandler
    {
        public static T? RegistrySetValue<T>(object? value, RegistryValueKind? valueKind)
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
                        break;
                    case RegistryValueKind.DWord:
                        converted = Convert.ToInt32(value);
                        break;
                    case RegistryValueKind.MultiString:
                        break;
                    case RegistryValueKind.QWord:
                        break;
                    case RegistryValueKind.None:
                        converted = value;
                        break;
                    default:
                        throw new ArgumentException("Unknown Set Value Type.");
                }

                return (T?)converted;
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert value for {valueKind}. Value => {Convert.ToString(value)}", ex);
            }
        }
    }
}
