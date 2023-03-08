using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryModels.Repository
{
    public interface IRepository
    {
        public List<T> Set<T>() where T : class;
        public void Updata<T>(T ob) where T : class;
        public void Add<T>(T ob) where T : class;
        public void Delete<T>(T ob) where T : class;

    }
}
