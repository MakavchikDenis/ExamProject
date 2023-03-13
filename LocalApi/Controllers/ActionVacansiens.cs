using Microsoft.AspNetCore.Mvc;
using LocalApi.Service;
using ActiveApiHH.ru;
using LibraryModels.Repository;
using LibraryModels;

namespace LocalApi.Controllers
{

    
    [ApiController]
    [Route("api/Action")]
    public class ActionVacanciens : Controller
    {
        IHandler handler;
        IActiveForApi activeForApi;
        IRepositoryDapper<Loggs> repositoryDapper;
        IRepository repository;
        IRepositoryExtra repositoryExtra;

        public ActionVacanciens (IHandler handler, IActiveForApi activeForApi, IRepositoryDapper<Loggs> repositoryDapper, IRepository repository,
            IRepositoryExtra repositoryExtra)
        {
            this.handler = handler;
            this.activeForApi = activeForApi;
            this.repositoryDapper = repositoryDapper;
            this.repository = repository;
            this.repositoryExtra = repositoryExtra;
        }


        /// <summary>
        ///  Из стороннего АПИ получаем данные по вакансиям название и содержание которых соответствует полученному параметру SearchVacansies
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        [HttpGet]
        public object SearchVacancies([FromHeader] string? Token, [FromQuery] string? SearchVacansies) {
            try
            {
                if (Token is null || SearchVacansies is null) { throw new Exception(); }





                return Ok();

            }
            catch (Exception) when (Token is null || SearchVacansies is null)
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
            catch (Exception e) {

                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка.");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return Problem (detail:"Системная ошибка программы.", statusCode:StatusCodes.Status500InternalServerError);

            }

        
        }
    }
}
