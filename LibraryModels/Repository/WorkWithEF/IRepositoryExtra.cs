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
