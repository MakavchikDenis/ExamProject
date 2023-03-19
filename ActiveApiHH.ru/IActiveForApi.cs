using LibraryModels.Repository;

namespace ActiveApiHH.ru
{
    public interface IActiveForApi
    {
        object GetAuthorizeUri();

        object GetToken(string authorization_code);

        object GetRefresh_token(object? session);

        object GetDataForUsers(string acces_token);

        Task<object> UpdateDataUser(UsersData user, string acces_token);

        string GetVacancies(string Token, string SearchText);

        Task<List<string>> SearchDetailsVacanciesAsync(string Token, string[] SearchVacancies);

    }
}
