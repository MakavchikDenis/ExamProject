using System;
namespace ActiveApiHH.ru.AuthorizeAPI
{
    internal interface IAuthorize
    {
        Uri AuthorizeUri();
        object GetTokenRemoteApi(string authorization_code);
        object Refresh_token(object session);

    }
}
