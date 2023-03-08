using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Models;

namespace LibraryModels.Repository
{
    public interface IRepositoryExtra
    {
        public Session? Find(string id);
    }
}
