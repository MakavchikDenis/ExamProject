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

                //repositoryDapper.Insert(new Loggs()
                //{
                //    DateAction = DateTime.Now,
                //    Action = String.Join("/", (string)RouteData.Values["controller"], (string)RouteData.Values["action"]),
                //    ActionResult = "Succes",


                //});

                return Redirect(RedirectUri.ToString());


            }

            /// ���� ������ ���������� �� �������� WebApplication
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
                ErrorApp error = new ErrorApp(LevelError.ActiveWithLocalApi, e.Message, "��������� ������ ��������������.");

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

                ErrorApp error = new ErrorApp(LevelError.ActiveWithLocalApi, e.Message, "��������� ������ �� ����� ��������� �������������� ������ �� �������.");
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


        /////// ��������� Handler!!!

        /// <summary>
        /// �������� �� ���������� ��� �������� ����� ��� ������, ���� ������ ������������ �� �������� �������� WebApp, 
        /// ���� ����=> ����������� � �������� �������� ������ � ������������
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

                // ����� �������, ����������� � ������ � ��
                SessionApi sessionApi = handler.Reverse <SessionApi>((string)result); 
                Session session = new Session(sessionApi);
                //// �������� ������ �� ������������
                
                /// ������ � �� ��� ������
                //repository.Add<Session>(session);

                // ��������
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
                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "��������� ������ �� ����� ��������� ������ ������������");
                (int level, string description, string message) = error;
                string Error = handler.Exchange<(int, string, string)>((level, description, message));
                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", nameof(BasedController), nameof(GetTokenRemoteApi)), "Error",
                    _errorMessage: Error);
                repositoryDapper.Insert(loggs);
                return Redirect(String.Concat(config["UriWebApplication"], $"?ErrorPoint=1&Message={error.Message}"));

            }


        }


        /// <summary>
        /// �� ��������� ������� ������=>��������� � ��������� ��� ��� ��������� ������ ������
        /// </summary>
        /// <returns> Error || NewToken </returns>
        [NonAction]
        public object GetRefresh_Token(Guid token) {
            try
            {
                Session? sessionOld = repositoryExtra.Find(token.ToString());
                var result = activeForRemoteApi.GetRefresh_token(sessionOld);
                if (result.GetType() == typeof(ErrorApp)) { throw (ErrorApp)result; }

                /// ��������� � �� ����� ������
                SessionApi sessionApi = handler.Reverse<SessionApi>((String)result);

                Session sessionNew = new Session(sessionApi);

                ///������� � �� ����� �����
                repository.Add<Session>(sessionNew);

                // �������� 
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

                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "��������� ������ �� ����� ������ ������ ������������");
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