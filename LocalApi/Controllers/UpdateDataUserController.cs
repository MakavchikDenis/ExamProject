using ActiveApiHH.ru;
using LibraryModels;
using LibraryModels.Repository;
using LocalApi.Service;
using Microsoft.AspNetCore.Mvc;

namespace LocalApi.Controllers
{
    [ApiController]
    [Route("api/DataUser")]
    public class UpdateDataUserController : Controller
    {
        IRepositoryDapper<Loggs> repositoryDapper;
        IHandler handler;
        IRepository repository;
        IRepositoryExtra repositoryExtra;
        IActiveForApi activeForRemoteApi;
        IRefresh_token refreshToken;

        public UpdateDataUserController(IRepositoryDapper<Loggs> _repositoryDapper, IHandler _handler, IRepository _repository, IRepositoryExtra _repositoryExtra,
            IActiveForApi _activeForApi, IRefresh_token _refresh_Token) => (repositoryDapper, handler, repository, repositoryExtra, activeForRemoteApi, refreshToken) =
            (_repositoryDapper, _handler, _repository, _repositoryExtra, _activeForApi, _refresh_Token);




        /// <summary>
        /// Возвращаем из нашей БД данные по пользователю
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsersData))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(UsersData))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(UsersData))]
        public async Task<object> GetUser([FromHeader] string? Token)
        {
            // Записываем ответный токен
            string? ResponseToken = Token;

            try
            {
                if (Token is null)
                {
                    throw new ErrorApp(LevelError.ActiveWithLocalApi, "Не передан токен", "Системная ошибка на этапе получения данных пользователя.");


                }


                //нахоим сессию пользователя
                var sessionUser = repositoryExtra.Find(Token);

                // Проверяем,что токен актуальный, если нет=>меняем токен в стороннем АПИ 
                if (sessionUser.EndToken < DateTime.Now)
                {
                    var result = refreshToken.CreateRefresh_token(Token);

                    // если ошибка выкидываем ее
                    if (result.GetType() == typeof(ErrorApp))
                    {
                        throw (ErrorApp)result;

                    }

                    // если положительно => в ответный токен вносим новый токен
                    ResponseToken = (string)result;


                }

                // из БД получаем данные по пользователю
                UsersData user = repository.Set<UsersData>().Where(x => x.IdUser == sessionUser.IdUser).FirstOrDefault();

                // если не найден выкидиваем ошибку
                if (user == default) {
                    throw new Exception("Пользователь не найден.");
                }


                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Succes", _token: ResponseToken);

                repositoryDapper.Insert(loggs);

                // в header отдаем новый токен
                HttpContext.Response.Headers.Add("Token", ResponseToken);
                return Ok(user);


            }
            catch (ErrorApp e) when (Token is null)
            {
                (int level, string description, string message) = e;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                HttpContext.Response.Headers.Add("Token", ResponseToken);
                return BadRequest(e);


            }
            catch (ErrorApp e) {
                (int level, string description, string message) = e;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                HttpContext.Response.Headers.Add("Token", ResponseToken);
                return BadRequest(e);


            }

            catch (Exception e)
            {
                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка на этапе изменения данных пользователя.");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                HttpContext.Response.Headers.Add("Token", ResponseToken);
                return NotFound(error);

            }


        }



        /// <summary>
        /// Обновляем данные пользователя как на стороннем сервере так и в БД приложения
        /// </summary>
        /// <param name="Token"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        [HttpPost("UpdataMainDataUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorApp))]
        public async Task<IActionResult> UpdataMainDataUser([FromHeader] string? Token, [FromBody] UsersData user)
        {
            try
            {
                if (Token is null)
                {
                    throw new ErrorApp(LevelError.ActiveWithLocalApi, "Не передан токен", "Системная ошибка на этапе изменения данных пользователя.");

                }

                // обвновляем данные на стороннем сервере
                var result = await activeForRemoteApi.UpdateDataUser(user, Token);
                if (result.GetType() == typeof(ErrorApp))
                {
                    throw (ErrorApp)result;

                }

                repository.Update<UsersData>(user);


                string jsonUser = handler.Exchange<UsersData>(user);
                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Succes", _token: Token, _actionDetails: jsonUser);

                repositoryDapper.Insert(loggs);

                return Ok();


            }
            catch (ErrorApp e)
            {
                (int level, string description, string message) = e;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return BadRequest(e);

            }
            catch (Exception e)
            {
                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка на этапе изменения данных пользователя.");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Error", _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return BadRequest(error);

            }


        }
    }
}
