using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Models
{

    [Table("Session", Schema ="Main")]
    public class Session
    {
        [Key]
        public Guid Acces_token { get; set; }
        public string Token_Type { get; set; }
        public int Expires_In { get; set; }
        public Guid Refresh_token { get; set; }
        [JsonIgnore]
        public DateTime? StartToken { get; set; }
        [JsonIgnore]
        public DateTime? EndToken { get; set; }
        [JsonIgnore]
        public int? IdUser { get; set; }
    }
}
