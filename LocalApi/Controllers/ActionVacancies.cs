using Microsoft.AspNetCore.Mvc;
using LocalApi.Service;
using ActiveApiHH.ru;
using LibraryModels.Repository;
using LibraryModels;


namespace LocalApi.Controllers
{

    [ApiController]
    [Route("api/Action")]
    public partial class ActionVacanciensController : Controller
    {
        IHandler handler;
        IActiveForApi activeForApi;
        IRepositoryDapper<Loggs> repositoryDapper;
        IRepository repository;
        IRepositoryExtra repositoryExtra;
        IServiceForHangfire hangfire;

        public ActionVacanciensController(IHandler handler, IActiveForApi activeForApi, IRepositoryDapper<Loggs> repositoryDapper, IRepository repository,
            IRepositoryExtra repositoryExtra, IServiceForHangfire _hangfire)
        {
            this.handler = handler;
            this.activeForApi = activeForApi;
            this.repositoryDapper = repositoryDapper;
            this.repository = repository;
            this.repositoryExtra = repositoryExtra;
            this.hangfire = _hangfire;
        }


        /// <summary>
        ///  Из стороннего АПИ получаем данные по вакансиям название и содержание которых соответствует полученному параметру SearchVacansies
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(List<Vacancy>))]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(ErrorSerializing))]
        [ProducesErrorResponseType(typeof(ObjectResult))]
        public object SearchVacancies([FromHeader] string? Token, [FromQuery] string? SearchVacancy)
        {
            try
            {

                if (Token is null || SearchVacancy is null) { throw new Exception(); }

                // получаем данные из стороннего АПИ
                string Result = activeForApi.GetVacancies(Token, SearchVacancy);

                ModelVacancies.ListVacancies modelVacancies = handler.Reverse<ModelVacancies.ListVacancies>(Result);

                List<Vacancy> listResult = new List<Vacancy>();

                foreach (var i in modelVacancies.items)
                {
                    listResult.Add(new Vacancy(i.id, i.name, i.area.url,
                        i.salary is not null ? new Salary(i.salary.from, i.salary.to, i.salary.currency) : null,
                        i.employer.name, i.snippet.requirement, i.snippet.responsibility));


                }

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Succes", Token, SearchVacancy);

                repositoryDapper.Insert(loggs);

                return Ok(listResult);

            }
            catch (Exception) when (Token is null || SearchVacancy is null)
            {
                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, "Не передан токен либо название вакансии",
                    "Не передан токен либо название вакансии.");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return BadRequest(ErrorForDB);

            }
            catch (Exception e)
            {

                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка.");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return Problem(detail: "Системная ошибка программы.", statusCode: StatusCodes.Status500InternalServerError);

            }


        }


        /// <summary>
        /// Добавляем выбранную вакансию в отслеживаемые. И раз в сутки в сторонний апи делается запрос для данного пользователя для получения 
        /// таких вакансий. Полученные данные записываем в БД.
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="textSearch"></param>
        /// <returns></returns>
        [HttpPost("SetTrackingVacancy/{textSearch}")]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(ErrorSerializing))]
        [ProducesErrorResponseType(typeof(ObjectResult))]
        public object SetTrackingVacancy([FromHeader] string Token, [FromRoute] string textSearch)
        {
            try
            {

                if (Token is null || textSearch is null) { throw new Exception(); }

                hangfire.RunRecurringJob(Token, textSearch);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Succes", Token, textSearch);

                repositoryDapper.Insert(loggs);

                return Ok();

            }
            catch (Exception) when (Token is null || textSearch is null)
            {
                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, "Не передан токен либо название вакансии",
                    "Не передан токен либо название вакансии.");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return BadRequest(ErrorForDB);

            }
            catch (Exception e)
            {

                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка.");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return Problem(detail: "Системная ошибка программы.", statusCode: StatusCodes.Status500InternalServerError);

            }


        }


        /// <summary>
        /// Метод возращает результаты отслежимой вакансии. С учетом того что вакансии берутся из стороннего АПИ раз в день, в клиентский интерфейс отдается 
        /// результаты последнего запроса в сторонний АПИ
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetTrackingVacancy/{idUser}/{textVacancy}")]
        public JsonResult GetTrackingVacancy()
        {

            string? Token = default;

            try
            {
                Token = HttpContext.Request.Headers["Token"];

                if (Token is null)
                {
                    throw new Exception();
                }

                string? idUser = (string?)HttpContext.Request.RouteValues["idUser"];

                string? textVacancy = (string?)HttpContext.Request.RouteValues["textVacancy"];
                
                VacanciesUser vacanciesUser = new VacanciesUser();

                var ResultSearchDb = repositoryExtra.FindVacanciesForUser(idUser, textVacancy);

                ViewVacancies OnlyResult = ResultSearchDb.OrderBy(x => x.Date).Last();

                vacanciesUser.IdUser = OnlyResult.User;

                vacanciesUser.DateUpdate = OnlyResult.Date;

                vacanciesUser.Vacancies = handler.Reverse<List<Vacancy>>(OnlyResult.Content);

                vacanciesUser.TextVacancy = OnlyResult.Vacancy;

                HttpContext.Response.StatusCode = 200;


                return Json(vacanciesUser);
            }
            catch (Exception) when (Token is null)
            {

                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, "Не передан токен",
                   "Не передан токен.");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

                return Json(ErrorForDB);


            }

            catch (Exception e)
            {

                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка.");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

                return Json(ErrorForDB);

            }

        }

    }
}
