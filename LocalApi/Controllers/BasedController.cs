using Microsoft.AspNetCore.Mvc;
using ActiveApiHH.ru;
using LibraryModels.Repository;
using LibraryModels;
using LocalApi.Service;

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

        public BasedController(IActiveForApi _activeForApi, IHttpContextAccessor _contextAccessor, 
            IConfigurationRoot _config, IRepositoryDapper<Loggs> _repositoryDapper, IHandler _handler) =>
             (activeForRemoteApi, contextAccessor, config, repositoryDapper, handler) = (_activeForApi, _contextAccessor, _config, _repositoryDapper, _handler);



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

                repositoryDapper.Insert(new Loggs()
                {
                    DateAction = DateTime.Now,
                    Action = String.Join("/", (string)RouteData.Values["controller"], (string)RouteData.Values["action"]),
                    ActionResult = "Succes",


                });

                return Redirect(RedirectUri.ToString());


            }

            /// если ошибки редиректим на страницу WebApplication
            catch (ErrorApp e)
            {
                string Error = handler.Exchange<ErrorApp>(e);
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
                ErrorApp error = new ErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка аутентификации.");
                string Error = handler.Exchange<ErrorApp>(error);
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
        public RedirectResult GetAuthorization_Code([FromQuery] string? code)
        {
            try
            {
                string authorization_code = code;

                ///Если промежуточный токен не получен => возвращаемся в WepApplication c ошибкой в параметрах
                if (authorization_code == default)
                {
                    throw new ErrorApp
                    {
                        level = LevelError.ActiveWithLocalApi,
                        ErrorDescription = "Не получен промежуточный токен от стороннего сервиса.",
                        Message = "Системная ошибка при получении промежуточного токена от сервиса."
                    };
                }

                //Если промежуточный токен получен=> логируем и редиректим для получения основного токена
                repositoryDapper.Insert(new Loggs()
                {
                    DateAction = DateTime.Now,
                    Action = String.Join("/", (string)RouteData.Values["controller"], (string)RouteData.Values["action"]),
                    ActionResult = "Succes",
                    ActionDetails = code
                    
                }) ; 


                return Redirect("");

            }
            catch (ErrorApp e)
            {
                string Error = handler.Exchange<ErrorApp>(e);
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
                string Error = handler.Exchange<ErrorApp>(error);
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
        



    }
}