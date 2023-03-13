using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ActiveApiHH.ru
{
    internal class ActionVacancies : IActionVacancies
    {
        public string SearchVacancies(string Token, string SearchText) {
            var collection = ConfigurationManager.AppSettings;
            KeyValuePair<string, string> HeaderParam = new KeyValuePair<string, string>(key: "Authorization", value: Token);
            var Scheme = "https://";
            var MainHost = collection.Get("RemoteHostForFollowingRequestMainApi");
            var path = String.Concat("/", collection.Get("SearchPath"));
            var paramHost = String.Concat("&host=", collection.Get("AddHostForRequest"));




        
        }



    }
}
