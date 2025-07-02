using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.API.Interface
{
    public interface IWebFactoryProvider
    {
        HttpClient? CreateClient(Uri baseAddress);
    }
}
