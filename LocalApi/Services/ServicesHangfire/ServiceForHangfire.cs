using ActiveApiHH.ru;
using API.Models;
using Hangfire;
using LibraryModels.Repository;
using System.Text.Json;


namespace LocalApi.Service
{
    internal class ServiceForHangfire : IServiceForHangfire
    {

        IRepository repository;
        IRepositoryExtra repositoryExtra;
        IActiveForApi activeForApi;
        
       

        public ServiceForHangfire(IRepository _repository, IRepositoryExtra _repositoryExtra,IActiveForApi _activeForApi)
        {
            this.repository = _repository;
            this.repositoryExtra=_repositoryExtra;
            this.activeForApi = _activeForApi;

        }

        /// <summary>
        /// Планируемое обновление данных каждый день,но для теста каждые 3 минуты поставил
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="searchVacancie"></param>
        public string RunRecurringJob(string Token, string searchVacancie)
        {
            //Именование процесса обеспечит создание нового процесса=>для кажого пользователя и выбранной вакансии 
            RecurringJob.AddOrUpdate(recurringJobId:String.Concat(Token,searchVacancie),() =>CreateTask(Token, searchVacancie), Cron.MinuteInterval(3));
            return "Create Task";
        }

        public void CreateTask(string Token, string searchVacancie)
        {

            // Достанем из БД IdUser;
            Session? session = repositoryExtra.Find(Token);

            //получаем данные из стороннего АПИ
            string Result = activeForApi.GetVacancies(Token, searchVacancie);

            ModelVacancies.ListVacancies modelVacancies = JsonSerializer.Deserialize<ModelVacancies.ListVacancies>(Result);


            List<Vacancy> listResult = new List<Vacancy>();

            foreach (var i in modelVacancies.items)
            {
                listResult.Add(new Vacancy(i.id, i.name, i.area.url,
                    i.salary is not null ? new Salary(i.salary.from, i.salary.to, i.salary.currency) : null,
                    i.employer.name, i.snippet.requirement, i.snippet.responsibility));


            }

            // если данные по отслеживаемой вакансии получены=> вносим в БД
            if (listResult.Count != default)
            {

                VacanciesUser vacanciesUser = new VacanciesUser();

                vacanciesUser.IdUser = session.IdUser.Value;
                vacanciesUser.TextVacancy = searchVacancie;
                vacanciesUser.DateUpdate = DateTime.Now;
                vacanciesUser.Content = JsonSerializer.SerializeToUtf8Bytes<List<Vacancy>>(listResult);


                repository.Add<VacanciesUser>(vacanciesUser);

            }
        }


    }

}
