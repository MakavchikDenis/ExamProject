using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.Net.Http;



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

            uri.Query = String.Join("&",new string[] { ParameterResponse_type, ParameterClient_id, ParameterRedirect_uri });


            return uri.Uri;



        }
    }
}
