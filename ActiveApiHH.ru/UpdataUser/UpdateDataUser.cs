using LibraryModels.Repository;
using System.Configuration;
using System.Net;

namespace ActiveApiHH.ru.UpdataUser
{
    public class UpdateDataUser : IUpdateDataUser
    {
        public async Task UpdateMainData(UsersData ob, string acces_token)
        {


            var collection = ConfigurationManager.AppSettings;

            Uri uri = new Uri(String.Concat("https://", collection.Get("RemoteHostForFollowingRequestMainApi"), "/", collection.Get("PathDataUser"),
               "?host=", collection.Get("AddHostForRequest")));

            var paraHeaderAuthorization = acces_token;

            var paramUserAgent = collection.Get("HH-User-Agent");

            Dictionary<string, string?> parameters = new Dictionary<string, string?>();

            parameters.Add(collection.Get("ParamUpdateFirstName"), ob.FirstName);

            parameters.Add(collection.Get("ParamUpdateLastName"), ob.SecondName);

            parameters.Add(collection.Get("ParamUpdateMiddleName"), ob.MiddleName);


            using HttpClient client = new HttpClient();

            client.Timeout = TimeSpan.FromSeconds(180);

            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, uri.ToString());
            requestMessage.Headers.Add("Authorization", String.Join(" ", "Bearer", paraHeaderAuthorization));
            requestMessage.Headers.Add("HH-User-Agent", paramUserAgent);

            var content = new FormUrlEncodedContent(parameters);
            requestMessage.Content = content;

            HttpResponseMessage httpResponse = await client.SendAsync(requestMessage);

            if (httpResponse.StatusCode != HttpStatusCode.NoContent)
            {
                throw new Exception(httpResponse.Content.ReadAsStringAsync().Result);

            }


        }

    }
}
