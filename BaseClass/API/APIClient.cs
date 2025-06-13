using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.API
{
    public class APIClient
    {
        private string? APIURL;
        private int? timeOut;
        private string? PerAccTok;

        public APIClient(string apiURL, string? personAcc,int? TimeOut) 
        {
            APIURL = apiURL;
            timeOut = TimeOut != null ? TimeOut : 60;
            PerAccTok = personAcc;
        }

        public async Task<T?> Get<T>() where T : class
        {
            // Create HttpClient instance
            using (var client = new HttpClient())
            {
                // Set personal access token in request headers of the baseurl:
                if(PerAccTok != null)
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
                    HttpResponseMessage response = await client.GetAsync(APIURL);

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
