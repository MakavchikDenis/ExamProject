using ActiveApiHH.ru.AuthorizeAPI;
using LibraryModels;
using System;

namespace ActiveApiHH.ru
{
    public class ActiveForApi : IActiveForApi
    {

        public object GetAuthorizeUri()
        {


            try
            {

                IAuthorize authorize = new Authorize();
                Uri authorizeRunRemoteApi = authorize.AuthorizeUri();
                if (authorizeRunRemoteApi == default)
                {
                    throw new ErrorApp(LevelError.ActiveWithRemoteApi, "Не сформировался URI для стороннего сервиса.", "Системная ошибка аутентификации.");
                }
                return authorizeRunRemoteApi;


            }
            catch (ErrorApp error)
            {

                return error;

            }
            catch (Exception e)
            {
                return new ErrorApp(LevelError.ActiveWithRemoteApi, e.Message, "Системная ошибка аутентификации.");

            }

        }

        
        public object GetToken(string authorization_code) {
            try
            {
                IAuthorize authorize = new Authorize();
                var result = authorize.GetTokenRemoteApi(authorization_code);
                return result;

            }
            catch (Exception e) { 
                return new ErrorApp
                {
                    level = LevelError.ActiveWithRemoteApi,
                    ErrorDescription = e.Message,
                    Message = "Ошибка ответа от стороннего сервиса на стадии получения токена"
                };


            }
        
        
        }


       public object GetRefresh_token(object session) {
            try
            {
                IAuthorize authorize = new Authorize();
                var result = authorize.Refresh_token(session);
                return result;

            }
            catch (Exception e)
            {
                return new ErrorApp
                {
                    level = LevelError.ActiveWithRemoteApi,
                    ErrorDescription = e.Message,
                    Message = "Ошибка ответа от стороннего сервиса на стадии замены токена"
                };


            }

        }

    }
}