using ActiveApiHH.ru.AuthorizeAPI;
using ActiveApiHH.ru.UpdataUser;
using LibraryModels;
using LibraryModels.Repository;


namespace ActiveApiHH.ru
{
    public class ActiveForApi : IActiveForApi
    {
        IAuthorize authorize = new Authorize();

        Lazy<UpdateDataUser> update = new Lazy<UpdateDataUser>();

        Lazy<ActionVacancies> vacancies = new Lazy<ActionVacancies>();

        IUpdateDataUser? iupdate;
        IActionVacancies? iaction;

        public object GetAuthorizeUri()
        {


            try
            {


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

        public object GetToken(string authorization_code)
        {
            try
            {

                var result = authorize.GetTokenRemoteApi(authorization_code);
                return result;

            }
            catch (Exception e)
            {
                return new ErrorApp
                {
                    level = LevelError.ActiveWithRemoteApi,
                    ErrorDescription = e.Message,
                    Message = "Ошибка ответа от стороннего сервиса на стадии получения токена"
                };


            }


        }

        public object GetRefresh_token(object session)
        {
            try
            {

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

        public object GetDataForUsers(string acces_token) => authorize.GetDataForUser(acces_token);

        public async Task<object> UpdateDataUser(UsersData user,string acces_token) {
            try
            {
                iupdate = update.Value;
                await iupdate.UpdateMainData(user, acces_token);
                return "Ok";
                
                   
            }
            catch (Exception e) {
                return new ErrorApp(LevelError.ActiveWithRemoteApi, e.Message, "Системная ошибка на этапе изменения данных пользователя.");
            
            }
        
        }

        public  string GetVacancies(string Token, string SearchText) {
            iaction = vacancies.Value;
            return iaction.SearchVacancies(Token, SearchText);
        
        }

    }
}