using ActiveApiHH.ru;
using API.Models;
using LibraryModels;
using LibraryModels.Repository;
using LocalApi.Service;
using Microsoft.AspNetCore.Mvc;
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
                (int level, string description, string message) = e;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

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

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

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
                repositoryDapper.Insert(new Loggs()
                {
                    DateAction = DateTime.Now,
                    Action = String.Join("/", (string)RouteData.Values["controller"], (string)RouteData.Values["action"]),
                    ActionResult = "Succes",
                    ActionDetails = code

                });


                return RedirectToAction(actionName: "GetTokenRemoteApi", controllerName: "Based", new { authorization_code = authorization_code });

            }
            catch (ErrorApp e)
            {
                (int level, string description, string message) = e;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

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

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public IActionResult GetTokenRemoteApi([FromQuery] string authorization_code)
        {
            try
            {
                object result = activeForRemoteApi.GetToken(authorization_code);

                if (result.GetType() == typeof(ErrorApp))
                {
                    throw (ErrorApp)result;

                }

                // Токен получен
                SessionApi sessionApi = handler.Reverse<SessionApi>((string)result);

                Session session = new Session(sessionApi);

                // получаем данные по User из стороннего АПИ
                var dataForUser = activeForRemoteApi.GetDataForUsers(session.Acces_token);

                if (dataForUser.GetType() == typeof(ErrorApp)) { throw (ErrorApp)dataForUser; }

                DataUserApi userApi = handler.Reverse<DataUserApi>((string)dataForUser);

                ///дополняем сессию
                session.IdUser = Int32.Parse(userApi.id);

                UsersData user = new UsersData(userApi);

                /// Смотрим имееются ли в БД данные по такому user=> если нет вносим, если да то только вносим сессию
                /// вносим в БД все данные

                var checkInDb = repository.Set<UsersData>().Where(x => x.IdUser == user.IdUser).FirstOrDefault();
                if (checkInDb == default)
                {
                    repository.Add<UsersData>(user);
                }

                repository.Add<Session>(session);


                // логируем
                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", nameof(BasedController), nameof(GetTokenRemoteApi)),
                    "Succes", _token: session.Acces_token, _actionDetails: authorization_code);

                repositoryDapper.Insert(loggs);

                return Ok(session.Acces_token);


            }
            catch (ErrorApp e)
            {
                (int level, string description, string message) = e;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", nameof(BasedController), nameof(GetTokenRemoteApi)), "Error",
                    _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return Redirect(String.Concat(config["UriWebApplication"], $"?ErrorPoint=1&Message={e.Message}"));

            }
            catch (Exception e)
            {
                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка на этапе получения токена пользователя");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", nameof(BasedController), nameof(GetTokenRemoteApi)), "Error",
                    _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return Redirect(String.Concat(config["UriWebApplication"], $"?ErrorPoint=1&Message={error.Message}"));

            }


        }


    }
}