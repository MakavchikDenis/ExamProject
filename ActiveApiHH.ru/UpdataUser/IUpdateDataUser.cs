using LibraryModels.Repository;

namespace ActiveApiHH.ru.UpdataUser
{
    internal interface IUpdateDataUser
    {
          Task UpdateMainData(UsersData ob, string acces_token);
    }
}
