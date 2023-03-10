using System.Text.Json;
using LibraryModels;
using LibraryModels.Repository;
using System;

namespace LocalApi.Service
{
    internal class Handler : IHandler
    {
        /// <summary>
        /// сериализуем любой объект
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        /// <returns></returns>
        public string Exchange<T>(T ob) => JsonSerializer.Serialize<T>(ob);


        /// <summary>
        /// Десериализуем данные 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        /// <returns></returns>
        public T Reverse<T>(string ob) => JsonSerializer.Deserialize<T>(ob);

        /// <summary>
        /// Создаем объект с ошибкой
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="level"></param>
        /// <param name="description"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public ErrorApp CreateErrorApp(LevelError _level, string _description, string _message) => new ErrorApp(_level, _description, _message);


        /// <summary>
        /// Создаем объект Loggs
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Loggs CreateLoggsBeforeInsert(DateTime _dateAction, string _action, string _actionResult,
            string? _token = default, string? _actionDetails = default, string? _errorMessage = default)
        {
            return new Loggs
            {
                Token = _token,
                DateAction = _dateAction,
                Action = _action,
                ActionResult = _actionResult,
                ActionDetails = _actionDetails,
                ErrorMessage = _errorMessage



            };



        }


       

    }
}
