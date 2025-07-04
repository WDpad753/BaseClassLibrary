﻿using BaseClass.API.Interface;
using BaseClass.Helper;
using BaseClass.Model;
using BaseLogger;
using BaseLogger.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using UtilityClass = BaseClass.MethodNameExtractor.FuncNameExtractor;

namespace BaseClass.API
{
    public class APIClient<TEntryPoint> : IDisposable where TEntryPoint : class
    {
        public string? APIURL { get; set; }
        public int? timeOut {  get; set; }
        public string? PerAccTok { get; set; }

        private static HttpClient? _client = null;
        private static bool? clientCreated = false;
        private bool disposedValue;
        private readonly StringHandler _strHandler;
        private readonly LogWriter _logWriter;
        private readonly PathCombine _pathHandler;
        private readonly ClientProvider<TEntryPoint>? _clientProvider;

        public APIClient(LogWriter Logger, ClientProvider<TEntryPoint>? clientProvider = null)
        {
            _logWriter = Logger;
            _strHandler = new(Logger);
            _clientProvider = clientProvider;
            _pathHandler = new(Logger);
        }

        public async Task<T?> Get<T>(string? url = null) where T : class
        {
            try
            {
                string? apiURL = url == null ? APIURL : (!url.Contains("http://")) ? _pathHandler.CombinePath(CombinationType.URL,APIURL, url).TrimEnd('/') : url;

                if (_client == null)
                {
                    _client = _clientProvider.CreateClient(new Uri(apiURL));
                }

                if (_clientProvider.testClient == null || _clientProvider.testClient == false)
                {
                    var client = _client;

                    if (clientCreated == false)
                    {
                        // Set personal access token in request headers of the baseurl:
                        if (PerAccTok != null)
                        {
                            client.Timeout = TimeSpan.FromSeconds((double)timeOut);

                            if(_clientProvider.clientBase == "GitHub")
                            {
                                client.DefaultRequestHeaders.UserAgent.ParseAdd($"{_clientProvider.appName}");
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", PerAccTok);
                            }
                            else if(_clientProvider.clientBase == "AzureDevOps")
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{PerAccTok}")));
                            }
                            else
                            {
                                throw new Exception();
                            }

                            clientCreated = true;
                        }
                        else
                        {
                            client.Timeout = TimeSpan.FromSeconds((double)timeOut);
                            clientCreated = true;
                        }
                    }

                    try
                    {
                        // Initiate Get request for the API url:
                        Task<HttpResponseMessage> taskcol = _client.GetAsync(apiURL);
                        Task.WaitAll(taskcol);
                        if (taskcol.IsFaulted)
                        {
                            Console.WriteLine(taskcol.Exception.ToString());
                            System.Diagnostics.Debug.WriteLine($@"Here is the Content of the Error Message: {taskcol.Exception.ToString()}");
                            _logWriter.LogWrite($"Error in acquiring response from url {apiURL}: {taskcol.Exception.ToString()}", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                        }
                        else
                        {
                            // Check if the request was successful:
                            HttpResponseMessage response = taskcol.Result;

                            // Check if the request was successful:
                            response.EnsureSuccessStatusCode();

                            // Get the response content from the request:
                            string responseBody = await response.Content.ReadAsStringAsync();

                            // Deserialize the JSON response:
                            T? responseObject = JsonConvert.DeserializeObject<T>(responseBody);

                            return responseObject;
                        }

                        return null;
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex.ToString());
                        System.Diagnostics.Debug.WriteLine($@"Here is the Content of the Error Message: {ex.ToString()}");
                        _logWriter.LogWrite("Error in De-Serializing the JSON Object: " + ex, this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                        return null;
                    }
                }
                else
                {
                    var client = _client;

                    if (clientCreated == false)
                    {
                        // Set personal access token in request headers of the baseurl:
                        if (PerAccTok != null)
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{PerAccTok}")));
                            client.Timeout = TimeSpan.FromSeconds((double)timeOut);
                            clientCreated = true;
                        }
                        else
                        {
                            client.Timeout = TimeSpan.FromSeconds((double)timeOut);
                            clientCreated = true;
                        }
                    }

                    try
                    {
                        // Initiate Get request for the API url:
                        Task<HttpResponseMessage> taskcol = client.GetAsync(apiURL);
                        Task.WaitAll(taskcol);
                        if (taskcol.IsFaulted)
                        {
                            Console.WriteLine(taskcol.Exception.ToString());
                            System.Diagnostics.Debug.WriteLine($@"Here is the Content of the Error Message: {taskcol.Exception.ToString()}");
                            _logWriter.LogWrite($"Error in acquiring response from url {apiURL}: {taskcol.Exception.ToString()}", this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                        }
                        else
                        {
                            // Check if the request was successful:
                            HttpResponseMessage response = taskcol.Result;

                            // Check if the request was successful:
                            response.EnsureSuccessStatusCode();

                            // Get the response content from the request:
                            string responseBody = await response.Content.ReadAsStringAsync();

                            // Deserialize the JSON response:
                            T? responseObject = JsonConvert.DeserializeObject<T>(responseBody);

                            return responseObject;
                        }

                        return null;
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex.ToString());
                        System.Diagnostics.Debug.WriteLine($@"Here is the Content of the Error Message: {ex.ToString()}");
                        _logWriter.LogWrite("Error in De-Serializing the JSON Object: " + ex, this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($@"Here is the Content of the Error Message: {ex.ToString()}");
                _logWriter.LogWrite("Error in De-Serializing the JSON Object: " + ex, this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
        }

        public async Task<T?> GetAsync<T>(string? url = null) where T : class
        {
            try
            {
                string? apiURL = url == null ? APIURL : url;

                if (_client == null)
                {
                    _client = _clientProvider.CreateClient(new Uri(apiURL));
                }

                // Create HttpClient instance
                if (_clientProvider.testClient == null || _clientProvider.testClient == false)
                {
                    var client = _client;

                    if (clientCreated == false)
                    {
                        // Set personal access token in request headers of the baseurl:
                        if (PerAccTok != null)
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{PerAccTok}")));
                            client.Timeout = TimeSpan.FromSeconds((double)timeOut);
                            clientCreated = true;
                        }
                        else
                        {
                            client.Timeout = TimeSpan.FromSeconds((double)timeOut);
                            clientCreated = true;
                        }
                    }

                    try
                    {
                        // Initiate Get request for the API url:
                        HttpResponseMessage response = await client.GetAsync(apiURL);

                        // Check if the request was successful:
                        response.EnsureSuccessStatusCode();

                        // Get the response content from the request:
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON response:
                        T? responseObject = JsonConvert.DeserializeObject<T>(responseBody);

                        return responseObject;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        System.Diagnostics.Debug.WriteLine($@"Here is the Content of the Error Message: {ex.ToString()}");
                        return null;
                    }
                }
                else
                {
                    var client = _client;

                    if (clientCreated == false)
                    {
                        // Set personal access token in request headers of the baseurl:
                        if (PerAccTok != null)
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{PerAccTok}")));
                            client.Timeout = TimeSpan.FromSeconds((double)timeOut);
                            clientCreated = true;
                        }
                        else
                        {
                            client.Timeout = TimeSpan.FromSeconds((double)timeOut);
                            clientCreated = true;
                        }
                    }

                    try
                    {
                        // Initiate Get request for the API url:
                        HttpResponseMessage response = await client.GetAsync(apiURL);

                        // Check if the request was successful:
                        response.EnsureSuccessStatusCode();

                        // Get the response content from the request:
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Deserialize the JSON response:
                        T? responseObject = JsonConvert.DeserializeObject<T>(responseBody);

                        return responseObject;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        System.Diagnostics.Debug.WriteLine($@"Here is the Content of the Error Message: {ex.ToString()}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                _logWriter.LogWrite("Error saving data to file: " + ex, this.GetType().Name, UtilityClass.GetMethodName(), MessageLevels.Fatal);
                return null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _client.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue=true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        //~APIClient()
        //{
        //    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //    Dispose(disposing: false);
        //}

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
