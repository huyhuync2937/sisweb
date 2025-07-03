using Newtonsoft.Json;
using SisCom;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;

namespace SisSo.Pages.Input.HDBanle
{
    public class PaymentRequest
    {
        public PaymentRequest()
        {
        }
        public static string sendPaymentRequest(string endpoint, string token, string postJsonString)
        {
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(endpoint);
                var postData = postJsonString;
                var data = Encoding.UTF8.GetBytes(postData);
                httpWReq.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + token);
                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/json";
                httpWReq.ContentLength = data.Length;
                httpWReq.ReadWriteTimeout = 30000;
                httpWReq.Timeout = 15000;

                Stream stream = httpWReq.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();
                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                string jsonresponse = "";
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string temp = null;
                    while ((temp = reader.ReadLine()) != null)
                    {
                        jsonresponse += temp;
                    }
                }
                //todo parse it
                return jsonresponse;
                //return new MomoResponse(mtid, jsonresponse);
            }
            catch (WebException e)
            {
                return e.Message;
            }
        }
        public static async Task<Root> sendHttpClientRequest(string endpoint, string token, string postJsonString)
        {
            HttpResponseMessage ResponseMessage;
            HttpClientHandler ClientHandler = new HttpClientHandler();
            HttpClient Client;
            try
            {
                Client = new HttpClient(ClientHandler);
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Client.Timeout = TimeSpan.FromSeconds(30);

                // string jsonData = JsonConvert.SerializeObject(postJsonString);
                var content = new StringContent(postJsonString, Encoding.UTF8, "application/json");
                ResponseMessage = await Client.PostAsync(endpoint, content);

                using (HttpContent Content = ResponseMessage.Content)
                {
                    var data = await Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(data))
                    {
                        Root LSData = JsonConvert.DeserializeObject<Root>(data);
                        return LSData;
                    }
                    else
                    {
                        return default;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default;
            }

        }
    }
}
