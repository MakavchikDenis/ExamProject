﻿using Microsoft.AspNetCore.Mvc;
using LocalApi.Service;
using ActiveApiHH.ru;
using LibraryModels.Repository;
using LibraryModels;


namespace LocalApi.Controllers
{

    [ApiController]
    [Route("api/Action")]
    public class ActionVacanciensController : Controller
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
        [ProducesResponseType(statusCode: StatusCodes.Status200OK, Type = typeof(List<Vacancie>))]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(ErrorSerializing))]
        [ProducesErrorResponseType(typeof(ObjectResult))]
        public object SearchVacancies([FromHeader] string? Token, [FromQuery] string? SearchVacansies)
        {
            try
            {

                if (Token is null || SearchVacansies is null) { throw new Exception(); }

                // получаем данные из стороннего АПИ
                string Result = activeForApi.GetVacancies(Token, SearchVacansies);

                ModelVacancies.ListVacancies modelVacancies = handler.Reverse<ModelVacancies.ListVacancies>(Result);

                List<Vacancie> listResult = new List<Vacancie>();

                foreach (var i in modelVacancies.items)
                {
                    listResult.Add(new Vacancie(i.id, i.name, i.area.url,
                        i.salary is not null ? new Salary(i.salary.from, i.salary.to, i.salary.currency) : null,
                        i.employer.name, i.snippet.requirement, i.snippet.responsibility));


                }

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Succes", Token, SearchVacansies);

                repositoryDapper.Insert(loggs);

                return Ok(listResult);

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
        /// таких вакансий.
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="textSearch"></param>
        /// <returns></returns>
        [HttpPost("GetTrackingVacancie/{textSearch}")]
        [ProducesResponseType(statusCode: StatusCodes.Status204NoContent)]
        [ProducesResponseType(statusCode: StatusCodes.Status400BadRequest, Type = typeof(ErrorSerializing))]
        [ProducesErrorResponseType(typeof(ObjectResult))]
        public IActionResult GetTrackingVacancie([FromHeader] string Token, [FromRoute] string textSearch) {
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
        
    }
}
