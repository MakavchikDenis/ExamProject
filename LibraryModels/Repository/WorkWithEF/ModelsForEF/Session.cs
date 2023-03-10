using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using LibraryModels.Repository;

namespace API.Models
{

    [Table("Session", Schema = "Main")]
    public class Session
    {
        [Key]
        public string Acces_token { get; set; }
        public string Token_Type { get; set; }
        public int Expires_In { get; set; }
        public string Refresh_token { get; set; }
        public DateTime? StartToken { get; set; }
        public DateTime? EndToken { get; set; }
        public int? IdUser { get; set; }

        public Session() { }

        public Session(SessionApi sessionApi) {
            Acces_token = sessionApi.Access_token;
            Token_Type = sessionApi.Token_type;
            Expires_In = sessionApi.Expires_in;
            Refresh_token = sessionApi.Refresh_token;
            StartToken = DateTime.Now;
            EndToken = StartToken.Value.AddSeconds(Expires_In);
        
        }
    }
}
