using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DFC.App.DiscoverSkillsCareers.TestSuite.Helpers
{
    public static class RestHelper
    {
        public static IRestResponse Get(string url, Dictionary<string, string> headers = null )
        {
            //Thread.Sleep(250);
            Console.WriteLine("Attempt to GET: " + url);
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.GET);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.AddHeader(header.Key, header.Value);
                    }
                }
                IRestResponse response = null;
                bool retry = true;
                int tries = 0;
                int maxTries = 5;
                while (retry)
                {
                    tries++;
                    response = client.Execute(request);
                    Console.WriteLine("Rest call Attempt (" + tries + ") Returned " + response.StatusCode);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        if (tries <= maxTries)
                        {
                            Console.WriteLine("Sleep and retry");
                            Thread.Sleep(500);
                        }
                        else retry = false;
                    }
                    else retry = false;
                }

                return response;
            }
            catch (Exception e) { throw e; }
        }

        public static IRestResponse Post(string url, string requestBody, Dictionary<string, string> headers = null)
        {
            Console.WriteLine("Attempt to POST to: " + url);
            try
            {
                var client = new RestClient(url);
                var request = new RestRequest(Method.POST);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.AddHeader(header.Key, header.Value);
                    }
                }
                request.AddJsonBody(requestBody);

                IRestResponse response = null;
                bool retry = true;
                int tries = 0;
                int maxTries = 5;
                while (retry)
                {
                    tries++;
                    response = client.Execute(request);
                    Console.WriteLine("Rest call Attempt (" + tries + ") Returned " + response.StatusCode);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        if (tries <= maxTries)
                        {
                            Console.WriteLine("Sleep and retry");
                            Thread.Sleep(500);
                        }
                        else retry = false;
                    }
                    else retry = false;
                }

                return response;
            }
            catch (Exception e) { throw e; }


        }

    }
}
