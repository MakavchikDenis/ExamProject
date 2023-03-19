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
        /// ��������� ����� ��� �������������� => �������������� �� ��������� ��� ��� ��������������
        /// </summary>
        [HttpGet]
        public RedirectResult Index()
        {
            try
            {

                var ResultAuthorize = activeForRemoteApi.GetAuthorizeUri();

                if (ResultAuthorize.GetType() == typeof(ErrorApp)) { throw (ErrorApp)ResultAuthorize; }

                /// ��������� ��������� ����������������� Uri, ���� ������� ���������� �� �������� �������
                Uri RedirectUri = (Uri)ResultAuthorize;

                repositoryDapper.Insert(new Loggs()
                {
                    DateAction = DateTime.Now,
                    Action = String.Join("/", (string)RouteData.Values["controller"], (string)RouteData.Values["action"]),
                    ActionResult = "Succes",


                });


                return Redirect(RedirectUri.ToString());


            }

            /// ���� ������ ���������� �� �������� WebApplication
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
                ErrorApp error = new ErrorApp(LevelError.ActiveWithLocalApi, e.Message, "��������� ������ ��������������.");

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
        /// ��������� �� ���������� ��� �������������� ������
        /// </summary>
        /// <param name="authorization_code"></param>
        /// <returns></returns>
        [HttpGet("GetAuthorization")]
        public IActionResult GetAuthorization_Code([FromQuery] string? code)
        {
            try
            {
                string authorization_code = code;

                ///���� ������������� ����� �� ������� => ������������ � WepApplication c ������� � ����������
                if (authorization_code == default)
                {
                    throw new ErrorApp()
                    {
                        level = LevelError.ActiveWithLocalApi,
                        ErrorDescription = "�� ������� ������������� ����� �� ���������� �������.",
                        Message = "��������� ������ ��� ��������� �������������� ������ �� �������."
                    };
                }

                //���� ������������� ����� �������=> �������� � ���������� ��� ��������� ��������� ������
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

                ErrorApp error = new ErrorApp(LevelError.ActiveWithLocalApi, e.Message, "��������� ������ �� ����� ��������� �������������� ������ �� �������.");

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


        /////// ��������� Handler!!!

        /// <summary>
        /// �������� �� ���������� ��� �������� ����� ��� ������, ���� ������ ������������ �� �������� �������� WebApp, 
        /// ���� ����=> ����������� � �������� �������� ������ � ������������
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

                // ����� �������
                SessionApi sessionApi = handler.Reverse<SessionApi>((string)result);

                Session session = new Session(sessionApi);

                // �������� ������ �� User �� ���������� ���
                var dataForUser = activeForRemoteApi.GetDataForUsers(session.Acces_token);

                if (dataForUser.GetType() == typeof(ErrorApp)) { throw (ErrorApp)dataForUser; }

                DataUserApi userApi = handler.Reverse<DataUserApi>((string)dataForUser);

                ///��������� ������
                session.IdUser = Int32.Parse(userApi.id);

                UsersData user = new UsersData(userApi);

                /// ������� �������� �� � �� ������ �� ������ user=> ���� ��� ������, ���� �� �� ������ ������ ������
                /// ������ � �� ��� ������

                var checkInDb = repository.Set<UsersData>().Where(x => x.IdUser == user.IdUser).FirstOrDefault();
                if (checkInDb == default)
                {
                    repository.Add<UsersData>(user);
                }

                repository.Add<Session>(session);


                // ��������
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
                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "��������� ������ �� ����� ��������� ������ ������������");

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