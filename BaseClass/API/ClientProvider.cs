using BaseClass.API.Interface;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.API
{
    public class ClientProvider<T> : IWebFactoryProvider where T : class
    {
        private readonly WebApplicationFactory<T>? _factory;
        private readonly HttpClient? _client;
        private readonly Uri? _uri;


        public ClientProvider(Uri baseUri, WebApplicationFactory<T>? factory = null, HttpClient? client = null)
        {
            _uri = baseUri;
            _factory = factory;
            _client = client;
        }

        public HttpClient CreateClient(Uri baseAddress)
        {
            throw new NotImplementedException();
        }
    }
}
