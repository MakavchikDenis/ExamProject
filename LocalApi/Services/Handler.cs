using System.Text.Json;
using LibraryModels;
using LibraryModels.Repository;
using System;
using System.IO;

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
        /// сериализуем любой объект в массив байтов
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        /// <returns></returns>
        public byte[] ExchangeToByte<T>(T ob) => JsonSerializer.SerializeToUtf8Bytes(ob);


        /// <summary>
        /// Десериализуем данные 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ob"></param>
        /// <returns></returns>
        public T Reverse<T>(string ob) => JsonSerializer.Deserialize<T>(ob);

        /// <summary>
        /// Десериализуем из байтов
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arrayBytes"></param>
        /// <returns></returns>
        public T Reverse<T>(byte[] arrayBytes) { 
            using MemoryStream stream = new MemoryStream(arrayBytes);

            return JsonSerializer.Deserialize<T>(stream);
        
        }

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
