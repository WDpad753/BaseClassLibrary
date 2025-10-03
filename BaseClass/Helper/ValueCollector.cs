using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Helper
{
    public class ValueCollector<T>
    {
        public T? Value { get; }
        public string? ValName { get; }

        public ValueCollector(T? value, string? valName)
        {
            Value = value; 
            ValName = valName;
        }
    }
}
