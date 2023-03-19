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
        Session? Find(string id);

        IQueryable<ViewVacancies> FindVacanciesForUser(string? idUser, string? textVacancie);

        List<DetailsVacanciesForUser> FindDetailsVacancies(string? user);
    }
}
