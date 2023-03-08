using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryModels.Repository
{
    /// <summary>
    /// Интерфейс CRUD для Dapper
    /// </summary>
    public interface IRepositoryDapper<Entity> where Entity : class
    {
        public void Insert(Entity entity);
    }
}
