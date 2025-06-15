using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Helper
{
    public static class JsonVerifier
    {
        public static bool IsValidJson(string json)
        {
            try
            {
                JToken.Parse(json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
