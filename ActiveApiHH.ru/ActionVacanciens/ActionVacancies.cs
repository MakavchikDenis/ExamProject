using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;

namespace ActiveApiHH.ru
{
    internal class ActionVacancies : IActionVacancies
    {
        public string SearchVacancies(string Token, string SearchText) {
            var collection = ConfigurationManager.AppSettings;

            KeyValuePair<string, string> HeaderParam = new KeyValuePair<string, string>(key: "Authorization", value: String.Concat("Bearer ", Token));

            KeyValuePair<string, string> HeaderParamUserAgent = new KeyValuePair<string, string>(key: "HH-User-Agent", value: collection.Get("HH-User-Agent"));

            var Scheme = "https://";

            var MainHost = collection.Get("RemoteHostForFollowingRequestMainApi");

            var path = String.Concat("/", collection.Get("SearchPath"));

            var paramHost = String.Concat("?host=", collection.Get("AddHostForRequest"));

            var paramSearchText = String.Concat("&", collection.Get("SearchNameParamText"),"=", collection.Get("SearchPartTextValue").Replace("VAR1", SearchText).Replace("VAR2", SearchText));

            var paramArea = String.Concat("&", collection.Get("SearchNameParamArea"), "=", collection.Get("SearchAreaValue"));

            using HttpClient client = new HttpClient();

            Uri uri = new Uri(String.Concat(Scheme, MainHost, path, paramHost, paramSearchText, paramArea));

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            requestMessage.Headers.Add(HeaderParam.Key, HeaderParam.Value);

            requestMessage.Headers.Add(HeaderParamUserAgent.Key, HeaderParamUserAgent.Value);

            
            HttpResponseMessage httpResponse = client.SendAsync(requestMessage).Result;

            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(httpResponse.Content.ReadAsStringAsync().Result);

            }

            return httpResponse.Content.ReadAsStringAsync().Result;



        
        }



    }
}
