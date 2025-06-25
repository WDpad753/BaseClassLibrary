using BaseClass.Helper;
using BaseLogger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.API
{
    public class APIClient
    {
        public string? APIURL { get; set; }
        public int? timeOut {  get; set; }
        public string? PerAccTok { get; set; }
        public HttpClient? testClient { get; set; }

        private StringHandler _strHandler;
        private LogWriter _logWriter;

        //public APIClient(string apiURL, string? personAcc, int? TimeOut = null, HttpClient? testClient = null) 
        //{
        //    APIURL = apiURL;
        //    timeOut = TimeOut != null ? TimeOut : 60;
        //    PerAccTok = personAcc;
        //    _testClient = testClient;
        //    _strHandler = new()
        //}

        public APIClient(LogWriter Logger)
        {
            _logWriter = Logger;
            _strHandler = new(Logger);
        }

        public async Task<T?> Get<T>(string? url = null) where T : class
        {
            string? apiURL = url == null ? APIURL : url;

            // Create HttpClient instance
            if (testClient == null)
            {
                using (var client = new HttpClient())
                {
                    // Set personal access token in request headers of the baseurl:
                    if (PerAccTok != null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{PerAccTok}")));
                        client.Timeout = TimeSpan.FromSeconds((double)timeOut);
                    }
                    else
                    {
                        client.Timeout = TimeSpan.FromSeconds((double)timeOut);
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
            else
            {
                using (var client = testClient)
                {
                    // Set personal access token in request headers of the baseurl:
                    if (PerAccTok != null)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($":{PerAccTok}")));
                        client.Timeout = TimeSpan.FromSeconds((double)timeOut);
                    }
                    else
                    {
                        client.Timeout = TimeSpan.FromSeconds((double)timeOut);
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
        }
    }
}
