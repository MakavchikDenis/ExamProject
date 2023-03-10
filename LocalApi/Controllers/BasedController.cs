using Microsoft.AspNetCore.Mvc;
using ActiveApiHH.ru;
using LibraryModels.Repository;
using LibraryModels;
using LocalApi.Service;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System;
using API.Models;
using System.Text.Json;
namespace LocalApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BasedController : ControllerBase
    {
        IHttpContextAccessor contextAccessor;
        IActiveForApi activeForRemoteApi;
        IConfigurationRoot config;
        IRepositoryDapper<Loggs> repositoryDapper;
        IHandler handler;
        IRepository repository;
        IRepositoryExtra repositoryExtra;

        public BasedController(IActiveForApi _activeForApi, IHttpContextAccessor _contextAccessor,
            IConfigurationRoot _config, IRepositoryDapper<Loggs> _repositoryDapper, IHandler _handler, IRepository _repository, IRepositoryExtra _repositoryExtra) =>
             (activeForRemoteApi, contextAccessor, config, repositoryDapper, handler, repository, repositoryExtra) = (_activeForApi, _contextAccessor, _config, _repositoryDapper, _handler, _repository, _repositoryExtra);



        /// <summary>
        /// Стартовый метод для аутентификации => перенаправляет на сторонний АПИ для аутентификации
        /// </summary>
        [HttpGet]
        public RedirectResult Index()
        {
            try
            {

                var ResultAuthorize = activeForRemoteApi.GetAuthorizeUri();

                if (ResultAuthorize.GetType() == typeof(ErrorApp)) { throw (ErrorApp)ResultAuthorize; }

                /// Проверяем результат сформировавшегося Uri, если успешно редиректим на страницу парнера
                Uri RedirectUri = (Uri)ResultAuthorize;

                //repositoryDapper.Insert(new Loggs()
                //{
                //    DateAction = DateTime.Now,
                //    Action = String.Join("/", (string)RouteData.Values["controller"], (string)RouteData.Values["action"]),
                //    ActionResult = "Succes",


                //});

                return Redirect(RedirectUri.ToString());


            }

            /// если ошибки редиректим на страницу WebApplication
            catch (ErrorApp e)
            {
                (int level, string description, string message) = e;
                
                string Error = handler.Exchange<(int,string,string)>((level,description, message));
                //repositoryDapper.Insert(new Loggs()
                //{
                //    DateAction = DateTime.Now,
                //    Action = String.Join("/", (string)RouteData.Values["controller"], (string)RouteData.Values["action"]),
                //    ActionResult = "Error",
                //    ErrorMessage = Error

                //});

                return Redirect(String.Concat(config["UriWebApplication"], $"?ErrorPoint=1&Message={e.Message}"));
            }
            catch (Exception e)
            {
                ErrorApp error = new ErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка аутентификации.");

                (int level, string description, string message) = error;
                string Error = handler.Exchange<(int, string, string)>((level, description, message));
               
                repositoryDapper.Insert(new Loggs()
                {
                    DateAction = DateTime.Now,
                    Action = String.Join("/", (string)RouteData.Values["controller"], (string)RouteData.Values["action"]),
                    ActionResult = "Error",
                    ErrorMessage = Error

                });

                return Redirect(String.Concat(config["UriWebApplication"], $"?ErrorPoint=1&Message={error.Message}"));


            }
        }

        /// <summary>
        /// Получение от стороннего АПИ промежуточного токена
        /// </summary>
        /// <param name="authorization_code"></param>
        /// <returns></returns>
        [HttpGet("GetAuthorization")]
        public IActionResult GetAuthorization_Code([FromQuery] string? code)
        {
            try
            {
                string authorization_code = code;

                ///Если промежуточный токен не получен => возвращаемся в WepApplication c ошибкой в параметрах
                if (authorization_code == default)
                {
                    throw new ErrorApp()
                    {
                        level = LevelError.ActiveWithLocalApi,
                        ErrorDescription = "Не получен промежуточный токен от стороннего сервиса.",
                        Message = "Системная ошибка при получении промежуточного токена от сервиса."
                    };
                }

                //Если промежуточный токен получен=> логируем и редиректим для получения основного токена
                //repositoryDapper.Insert(new Loggs()
                //{
                //    DateAction = DateTime.Now,
                //    Action = String.Join("/", (string)RouteData.Values["controller"], (string)RouteData.Values["action"]),
                //    ActionResult = "Succes",
                //    ActionDetails = code

                //});


                return RedirectToAction(actionName: "GetTokenRemoteApi", controllerName: "Based", new { authorization_code = authorization_code });

            }
            catch (ErrorApp e)
            {
                (int level, string description, string message) = e;
                string Error = handler.Exchange<(int, string, string)>((level, description, message));
                repositoryDapper.Insert(new Loggs()
                {
                    DateAction = DateTime.Now,
                    Action = String.Join("/", (string)RouteData.Values["controller"], (string)RouteData.Values["action"]),
                    ActionResult = "Error",
                    ErrorMessage = Error

                });

                return Redirect(String.Concat(config["UriWebApplication"], $"?ErrorPoint=1&Message={e.Message}"));

            }
            catch (Exception e)
            {

                ErrorApp error = new ErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка на этапе получения промежуточного токена от сервиса.");
                (int level, string description, string message) = error;
                string Error = handler.Exchange<(int, string, string)>((level, description, message));
                repositoryDapper.Insert(new Loggs()
                {
                    DateAction = DateTime.Now,
                    Action = String.Join("/", (string)RouteData.Values["controller"], (string)RouteData.Values["action"]),
                    ActionResult = "Error",
                    ErrorMessage = Error

                });

                return Redirect(String.Concat(config["UriWebApplication"], $"?ErrorPoint=1&Message={error.Message}"));
            }


        }


        /////// Переделал Handler!!!

        /// <summary>
        /// Получаем от стороннего АПИ основной токен для работы, если ошибка возвращаемся на основную страницу WebApp, 
        /// если норм=> запрашиваем у сторонне сервичас данные о пользователе
        /// </summary>
        /// <param name="authorization_code"></param>
        [HttpGet("GetTokenRemoteApi")]
        public object GetTokenRemoteApi([FromQuery] string authorization_code)
        {
            try
            {
                object result = activeForRemoteApi.GetToken(authorization_code);

                if (result.GetType() == typeof(ErrorApp))
                {
                    throw (ErrorApp)result;

                }

                // Токен получен, дозаполняем и вносим в БД
                SessionApi sessionApi = handler.Reverse <SessionApi>((string)result); 
                Session session = new Session(sessionApi);
                //// получаем данные по пользователю
                
                /// вносим в БД все данные
                //repository.Add<Session>(session);

                // логируем
                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", nameof(BasedController), nameof(GetTokenRemoteApi)), "Succes", _token: session.Acces_token,
                    _actionDetails: authorization_code);
                //repositoryDapper.Insert(loggs);

                return Redirect("");


            }
            catch (ErrorApp e)
            {
                (int level, string description, string message) = e;
                string Error = handler.Exchange<(int, string, string)>((level, description, message));
                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", nameof(BasedController), nameof(GetTokenRemoteApi)), "Error",
                    _errorMessage: Error);
                repositoryDapper.Insert(loggs);
                return Redirect(String.Concat(config["UriWebApplication"], $"?ErrorPoint=1&Message={e.Message}"));

            }
            catch (Exception e)
            {
                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка на этапе получения токена пользователя");
                (int level, string description, string message) = error;
                string Error = handler.Exchange<(int, string, string)>((level, description, message));
                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", nameof(BasedController), nameof(GetTokenRemoteApi)), "Error",
                    _errorMessage: Error);
                repositoryDapper.Insert(loggs);
                return Redirect(String.Concat(config["UriWebApplication"], $"?ErrorPoint=1&Message={error.Message}"));

            }


        }


        /// <summary>
        /// По истечению времени токена=>обращение в сторонний АПИ для получения нового токена
        /// </summary>
        /// <returns> Error || NewToken </returns>
        [NonAction]
        public object GetRefresh_Token(Guid token) {
            try
            {
                Session? sessionOld = repositoryExtra.Find(token.ToString());
                var result = activeForRemoteApi.GetRefresh_token(sessionOld);
                if (result.GetType() == typeof(ErrorApp)) { throw (ErrorApp)result; }

                /// Добавляем в БД новые данные
                SessionApi sessionApi = handler.Reverse<SessionApi>((String)result);

                Session sessionNew = new Session(sessionApi);

                ///создаем в БД новую сесию
                repository.Add<Session>(sessionNew);

                // логируем 
                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", nameof(BasedController), nameof(GetRefresh_Token)), "Succes", _token: sessionNew.Acces_token,
                   _actionDetails: token.ToString());
                repositoryDapper.Insert(loggs);

                return sessionNew.Acces_token;

            }
            catch (ErrorApp e)
            {
                (int level, string description, string message) = e;
                string Error = handler.Exchange<(int, string, string)>((level, description, message));
                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", nameof(BasedController), nameof(GetRefresh_Token)), "Error",
                    _errorMessage: Error);
                repositoryDapper.Insert(loggs);
                return e;


            }
            catch (Exception e) {

                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка на этапе замены токена пользователя");
                (int level, string description, string message) = error;
                string Error = handler.Exchange<(int, string, string)>((level, description, message));
                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", nameof(BasedController), nameof(GetRefresh_Token)), "Error",
                    _errorMessage: Error);
                repositoryDapper.Insert(loggs);
                return error;

            }

        }
        


    }
}