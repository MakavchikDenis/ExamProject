using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryModels
{

    /// <summary>
    /// уровни возникновения ошибок
    /// </summary>
    public enum LevelError
    {
        Succes =0,
        ActiveWithRemoteApi =1,
        ActiveWithLocalApi =2,
        ActiveWithFrontApp =3
    }
}
