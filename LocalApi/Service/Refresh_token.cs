using ActiveApiHH.ru;
using API.Models;
using LibraryModels;
using LibraryModels.Repository;
using LocalApi.Service;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LocalApi.Service
{
    public class Refresh_token : IRefresh_token
    {
        IRepositoryExtra repositoryExtra;
        IRepository repository;
        IHandler handler;
        IRepositoryDapper<Loggs> repositoryDapper;
        IActiveForApi activeForRemoteApi;

        public Refresh_token(IRepositoryExtra _repositoryExtra, IRepository _repository, IHandler _handler, IRepositoryDapper<Loggs> _repositoryDapper,
            IActiveForApi _activeForRemoteApi)
        {
            this.repositoryExtra = _repositoryExtra;
            this.repository = _repository;
            this.handler = _handler;
            this.repositoryDapper = _repositoryDapper;
            this.activeForRemoteApi = _activeForRemoteApi;
        }

        /// <summary>
        /// По истечению времени токена=>обращение в сторонний АПИ для получения нового токена
        /// </summary>
        /// <returns> Error || NewToken </returns>
        public object CreateRefresh_token(string token)
        {
            try
            {
                Session? sessionOld = repositoryExtra.Find(token.ToString());

                var result = activeForRemoteApi.GetRefresh_token(sessionOld);

                if (result.GetType() == typeof(ErrorApp)) { throw (ErrorApp)result; }

                
                SessionApi sessionApi = handler.Reverse<SessionApi>((String)result);

                Session sessionNew = new Session(sessionApi);

                ///создаем в БД новую сесию
                repository.Add<Session>(sessionNew);

                // логируем 
                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", nameof(Refresh_token), nameof(CreateRefresh_token)), "Succes",
                    _token: sessionNew.Acces_token, _actionDetails: token.ToString());

                repositoryDapper.Insert(loggs);

                return sessionNew.Acces_token;

            }
            catch (ErrorApp e)
            {   
                (int level, string description, string message) = e;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", nameof(Refresh_token), nameof(CreateRefresh_token)), "Error",
                    _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return e;


            }
            catch (Exception e)
            {

                ErrorApp error = handler.CreateErrorApp(LevelError.ActiveWithLocalApi, e.Message, "Системная ошибка на этапе замены токена пользователя");

                (int level, string description, string message) = error;

                var ErrorForDB = new ErrorSerializing((level, description, message));

                string Error = handler.Exchange<ErrorSerializing>(ErrorForDB);

                Loggs loggs = handler.CreateLoggsBeforeInsert(DateTime.Now, String.Join("/", nameof(Refresh_token), nameof(CreateRefresh_token)), "Error",
                    _errorMessage: Error);

                repositoryDapper.Insert(loggs);

                return error;

            }

        }
    }
}
