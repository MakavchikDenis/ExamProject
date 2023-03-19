using System.Configuration;
using System.Net;

namespace ActiveApiHH.ru
{
    internal class ActionVacancies : IActionVacancies
    {
        public string SearchVacancies(string Token, string SearchText) {
            var collection = ConfigurationManager.AppSettings;

            //Не нужно-окрытый метод в стороннем апи
            //KeyValuePair<string, string> HeaderParam = new KeyValuePair<string, string>(key: "Authorization", value: String.Concat("Bearer ", Token));

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

            //requestMessage.Headers.Add(HeaderParam.Key, HeaderParam.Value);

            requestMessage.Headers.Add(HeaderParamUserAgent.Key, HeaderParamUserAgent.Value);

            
            HttpResponseMessage httpResponse = client.SendAsync(requestMessage).Result;

            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception(httpResponse.Content.ReadAsStringAsync().Result);

            }

            return httpResponse.Content.ReadAsStringAsync().Result;



        
        }

        public async Task<List<string>> SearchDetailsVacanciesForUserAsync(string Token, string[] SearchVacancies) {

            List<string> listResult = new List<string>();

            listResult.Capacity = SearchVacancies.Count();

            Task<string>[] arrayTasks = new Task<string>[SearchVacancies.Count()];

            for (var i = 0; i<arrayTasks.Length;i++) {
                arrayTasks[i] = GetForRemoteAPIAsync((string)Token.Clone(), SearchVacancies[i]);
             
            }

            string[] arrayPrepareResult = await Task.WhenAll(arrayTasks);

            return arrayPrepareResult.ToList();
            

        
        }

        private async Task<string> GetForRemoteAPIAsync(string Token, string SearchVacancy) {

            var collention = ConfigurationManager.AppSettings;

            var headerToken = String.Join(" ", "Bearer", Token);

            var headerUserAgent = collention.Get("HH-User-Agent");

            var queryParam = collention.Get("RemoteHost");

            var pathParam = SearchVacancy;

            var uri = String.Concat("https://", collention.Get("RemoteHostForFollowingRequestMainApi"), "/vacancies/", SearchVacancy, "?", "host=", queryParam);

            using HttpClient client = new HttpClient();

            client.Timeout = TimeSpan.FromSeconds(180);

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);

            requestMessage.Headers.Add("Authorization", headerToken);

            requestMessage.Headers.Add("HH-User-Agent", headerUserAgent);

            HttpResponseMessage responseMessage = await client.SendAsync(requestMessage);

            return await responseMessage.Content.ReadAsStringAsync();
        
        
        }



    }
}
