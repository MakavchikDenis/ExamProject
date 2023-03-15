using LibraryModels;
using LibraryModels.Repository;
using System;
namespace LocalApi.Service
{
    public interface IHandler
    {
        public string Exchange<T>(T ob);
        public byte[] ExchangeToByte<T>(T ob);
        public T Reverse<T>(string ob);
        public T Reverse<T>(byte[] arrayBytes);
        public ErrorApp CreateErrorApp(LevelError level, string description, string message);
        public Loggs CreateLoggsBeforeInsert(DateTime _dateAction, string _action, string _actionResult,
            string? _token = default, string? _actionDetails = default, string? _errorMessage = default);

    }
}
