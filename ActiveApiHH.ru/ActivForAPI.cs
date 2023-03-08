using ActiveApiHH.ru.AuthorizeAPI;
using LibraryModels;

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


    }
}