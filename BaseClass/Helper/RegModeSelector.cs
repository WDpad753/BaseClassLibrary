using BaseClass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Helper
{
    public static class RegModeSelector
    {
        public static RegPath? RegSelector(string RegType)
        {
            try
            {
                RegPath? reg = null;

                switch (RegType)
                {
                    case "User":
                        reg = RegPath.User;
                        break;
                    case "Machine":
                        reg = RegPath.Machine;
                        break;
                    default:
                        throw new Exception("Unable to select a type.");
                }

                return reg;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to select a type. Exception: {ex}");
            }
        }
    }
}
