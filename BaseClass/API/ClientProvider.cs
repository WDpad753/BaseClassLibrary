using BaseClass.API.Interface;
using BaseLogger;
using BaseLogger.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using UtilityClass = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.API
{
    public class ClientProvider<T> : IWebFactoryProvider where T : class
    {
        private readonly WebApplicationFactory<T>? _factory;
        private readonly LogWriter _logWriter;
        public bool? testClient { get; set; }
        public string? clientBase { get; set; }
        public string? appName { get; set; }


        public ClientProvider(LogWriter Logger, WebApplicationFactory<T>? factory = null)
        {
            _logWriter = Logger;
            _factory = factory;
        }

        public HttpClient? CreateClient(Uri baseAddress)
        {
            try
            {
                HttpClient? client = null;

                if (testClient == null || testClient != true)
                {
                    client = new HttpClient();
                    client.BaseAddress = baseAddress;
                }
                else if (testClient == true)
                {
                    client = _factory.CreateClient();
                    client.BaseAddress = baseAddress;
                }
                else
                {
                    throw new InvalidOperationException("Utilising this class requires to know if it is used for testing or for live project");
                }

                return client;
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite($"Unable to create client. Error Message: {ex.Message}; Trace: {ex.StackTrace}; Exception: {ex.InnerException}; Error Source: {ex.Source}", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
        }
    }
}
