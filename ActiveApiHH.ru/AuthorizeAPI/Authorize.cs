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
                gran_type = collection.Get("ParamForGetTokenGrant_Type"),
                client_id = collection.Get("ParamClient_Id"),
                client_secret = collection.Get("ParamClient_secret"),
                redirect_uri = collection.Get("LocalPathAuthorize"),
                code = authorization_code
            };

            UriBuilder builder = new UriBuilder();
            builder.Host = host;
            builder.Path = path;



            using HttpClient http = new HttpClient();
            http.Timeout = TimeSpan.FromSeconds(180);
            //http.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            HttpRequestMessage httpRequest = new HttpRequestMessage(HttpMethod.Post, builder.Uri);
            //httpRequest.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            httpRequest.Content = new StringContent(JsonSerializer.Serialize(bodyRequest, bodyRequest.GetType()), encoding:Encoding.UTF8,
                mediaType: "application/x-www-form-urlencoded");

            StringContent content = new StringContent(JsonSerializer.Serialize(bodyRequest, bodyRequest.GetType()), encoding: Encoding.UTF8,
                mediaType: "application/x-www-form-urlencoded");


            HttpResponseMessage httpResponse = http.PostAsync(builder.Uri, content).Result;

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

        public object Refresh_token(object session) {
            Session sessionOld = (Session)session;
            var collection = ConfigurationManager.AppSettings;
            UriBuilder builder = new UriBuilder();
            builder.Host = collection.Get("RemoteHost");
            builder.Path = collection.Get("RemotePathGetToken");

            var bodyRequest = new
            {
                grant_type = collection.Get("ParamForRefreshTokenGrant_type"),
                refresh_token = sessionOld.Refresh_token

            };


            using HttpClient http = new HttpClient();
            //http.DefaultRequestHeaders.Add("Content-Type", "application/x-www-form-urlencoded");
            http.Timeout = TimeSpan.FromSeconds(180);

            HttpResponseMessage httpResponse = http.PostAsync(builder.Uri.AbsoluteUri, new StringContent(JsonSerializer.Serialize(bodyRequest, bodyRequest.GetType()),
                encoding: Encoding.UTF8,mediaType: "application/x-www-form-urlencoded")).Result;

            if (!httpResponse.IsSuccessStatusCode) {
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
