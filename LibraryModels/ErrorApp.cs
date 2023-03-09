using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace LibraryModels
{
    // Класс для ошибок
    public class ErrorApp :Exception
    {

        // уровень ошибки
        public LevelError level { get; set; }
        // описание ошибки
        public string ErrorDescription {get;set;}
        // вывод на клиентскую сторону
        public string Message { get; set; }

        public ErrorApp() { }

        public ErrorApp(LevelError _level, string _ErrorDescription, string _Message) => 
            (this.level, this.ErrorDescription, this.Message) = (_level, _ErrorDescription, _Message);

        public void Deconstruct(out int level, out string ErrorDescription, out string Message) {
            level = this.level switch
            {
                LevelError.Succes => 0,
                LevelError.ActiveWithRemoteApi => 1,
                LevelError.ActiveWithLocalApi => 2,
                LevelError.ActiveWithFrontApp => 3
            };

            (ErrorDescription, Message) = (this.ErrorDescription, this.Message);
            
        
        }

    }
}
