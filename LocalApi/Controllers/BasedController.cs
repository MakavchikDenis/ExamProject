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
                ErrorApp error = new ErrorApp(LevelError.ActiveWithLocalApi, e.Message, "��������� ������ ��������������.");
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
        /// ��������� �� ���������� ��� �������������� ������
        /// </summary>
        /// <param name="authorization_code"></param>
        /// <returns></returns>
        [HttpGet("GetAuthorization")]
        public RedirectResult GetAuthorization_Code([FromQuery] string? code)
        {
            try
            {
                string authorization_code = code;

                ///���� ������������� ����� �� ������� => ������������ � WepApplication c ������� � ����������
                if (authorization_code == default)
                {
                    throw new ErrorApp
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

                ErrorApp error = new ErrorApp(LevelError.ActiveWithLocalApi, e.Message, "��������� ������ �� ����� ��������� �������������� ������ �� �������.");
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


        /////// ��������� Handler!!!
        



    }
}