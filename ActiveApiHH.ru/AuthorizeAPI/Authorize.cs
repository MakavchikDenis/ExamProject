using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.Net.Http;
using API.Models;
using LibraryModels;
using System.Text.Json;
using System.Net;



namespace ActiveApiHH.ru.AuthorizeAPI
{
    internal class Authorize : IAuthorize
    {
        /// <summary>
        /// Первоначальная аутентификация
        /// </summary>
        public Uri AuthorizeUri()
        {
            // получаем данные из конфиг. файла
            var collection = ConfigurationManager.AppSettings;

            // готовим запрос в стороний АПИ
            // конструируем URI
            UriBuilder uri = new UriBuilder();
            uri.Host = collection.Get("RemoteHost");
            uri.Path = collection.Get("RemotePathAuthorize");

            string ParameterResponse_type = String.Join("=", "response_type", collection.Get("ParamForFirstEtapAuthorizeResponse_Code"));
            string ParameterClient_id = String.Join("=", "client_id", collection.Get("ParamClient_Id"));
            string ParameterRedirect_uri = String.Join("=", "redirect_uri", collection.Get("LocalPathAuthorize"));

            uri.Query = String.Join("&", new string[] { ParameterResponse_type, ParameterClient_id, ParameterRedirect_uri });


            return uri.Uri;



        }


        public object GetTokenRemoteApi(string authorization_code)
        {
            var collection = ConfigurationManager.AppSettings;
            string? host = collection.Get("RemoteHost");
            string? path = collection.Get("RemotePathGetToken");
            var bodyRequest = new
            {
                grant_type = collection.Get("ParamForGetTokenGrant_Type"),
                client_id = collection.Get("ParamClient_Id"),
                client_secret = collection.Get("ParamClient_secret"),
                redirect_uri = collection.Get("LocalPathAuthorize"),
                code = authorization_code
            };

            Uri uri = new Uri(String.Concat("https://",host, path));


            List<KeyValuePair<string, string>> param = new List<KeyValuePair<string, string>>();
            param.Add(new KeyValuePair<string, string>(nameof(bodyRequest.grant_type), bodyRequest.grant_type));
            param.Add(new KeyValuePair<string, string>(nameof(bodyRequest.client_id), bodyRequest.client_id));
            param.Add(new KeyValuePair<string, string>(nameof(bodyRequest.client_secret), bodyRequest.client_secret));
            param.Add(new KeyValuePair<string, string>(nameof(bodyRequest.redirect_uri), bodyRequest.redirect_uri));
            param.Add(new KeyValuePair<string, string>(nameof(bodyRequest.code), bodyRequest.code));


            using HttpClient http = new HttpClient();
            http.Timeout = TimeSpan.FromSeconds(180);

            var content = new FormUrlEncodedContent(param);

            HttpResponseMessage httpResponse = http.PostAsync(uri, content).Result;

            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                return new ErrorApp
                {
                    level = LevelError.ActiveWithRemoteApi,
                    ErrorDescription = ((int)httpResponse.StatusCode).ToString(),
                    Message = "Ошибка ответа от стороннего сервиса на стадии получения токена"
                };


            }

            string result = httpResponse.Content.ReadAsStringAsync().Result;
            return result;

        }

        public object Refresh_token(object? session)
        {
            Session? sessionOld = (Session)session;
            var collection = ConfigurationManager.AppSettings;
            string? Host = collection.Get("RemoteHost");
            string? Path = collection.Get("RemotePathGetToken");

            Uri uri = new Uri(String.Concat("https://",Host, Path));

            var bodyRequest = new
            {
                grant_type = collection.Get("ParamForRefreshTokenGrant_type"),
                refresh_token = sessionOld.Refresh_token

            };


            KeyValuePair<string, string>[] array = new KeyValuePair<string, string>[] { new KeyValuePair<string,string>(nameof(bodyRequest.grant_type),bodyRequest.grant_type),
            new KeyValuePair<string, string>(nameof(bodyRequest.refresh_token), bodyRequest.refresh_token.ToString())};

            using HttpClient http = new HttpClient();
            http.Timeout = TimeSpan.FromSeconds(180);

            HttpContent content = new FormUrlEncodedContent(array);

            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, uri.ToString());
            httpRequest.Content = content;


            HttpResponseMessage httpResponse = http.SendAsync(httpRequest).Result;

            if (!httpResponse.IsSuccessStatusCode)
            {
                return new ErrorApp
                {
                    level = LevelError.ActiveWithRemoteApi,
                    ErrorDescription = ((int)httpResponse.StatusCode).ToString(),
                    Message = "Ошибка ответа от стороннего сервиса на стадии замены токена"
                };

            }

            return httpResponse.Content.ReadAsStringAsync().Result;


        }
    }
}
