using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ControlLucesXamarin.Helpers
{
    public class FunctionHelper
    {
        public static async void SendDataToFunction(string light, string handler)
        {
            HttpClient request = new HttpClient();
            var requestedLink = new Uri("https://iothubfunctioncloud.azurewebsites.net/api/HttpTriggerCSharp1?code=clQrIevXN1zlcaPjNWaL2YUC5C4B30kCIKXMkvI21SDqcnzIADv/tQ==");
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestedLink);

            var sendString = String.Format("{{\"DeviceId\":\"devicetest\",\"Message\":\"{0},{1}\"}}", light, handler);

            requestMessage.Content = new StringContent(sendString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await request.SendAsync(requestMessage);
            var responseString = await response.Content.ReadAsStringAsync();
        }
    }
}
