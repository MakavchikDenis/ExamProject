using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryModels.Repository;

namespace ActiveApiHH.ru.UpdataUser
{
    internal interface IUpdateDataUser
    {
          Task UpdateMainData(UsersData ob, string acces_token);
    }
}
