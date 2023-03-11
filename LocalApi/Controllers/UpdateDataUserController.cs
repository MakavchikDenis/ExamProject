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

        public UpdateDataUserController(IRepositoryDapper<Loggs> _repositoryDapper, IHandler _handler, IRepository _repository, IRepositoryExtra _repositoryExtra,
            IActiveForApi _activeForApi) => (repositoryDapper, handler, repository, repositoryExtra, activeForRemoteApi) = (_repositoryDapper,
            _handler, _repository, _repositoryExtra, _activeForApi);


        /// <summary>
        /// Возвращаем из нашей БД данные по пользователю
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UsersData))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(UsersData))]
        public async Task<object> GetUser([FromHeader] string? Token)
        {
            try
            {
                if (Token is null)
                {
                    throw new ErrorApp(LevelError.ActiveWithLocalApi, "Не передан токен", "Системная ошибка на этапе изменения данных пользователя.");


                }

                var sessionUser = repositoryExtra.Find(Token);
                UsersData user = repository.Set<UsersData>().Where(x => x.IdUser == sessionUser.IdUser).First();

                
                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", (string)RouteData.Values["controller"],
                    (string)RouteData.Values["action"]), "Succes", _token: Token);

                repositoryDapper.Insert(loggs);

                return Ok(user);


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
