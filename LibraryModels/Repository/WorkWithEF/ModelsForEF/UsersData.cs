using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization

namespace LibraryModels.Repository
{
    [Table("UsersData", Schema = "Main")]
    public class UsersData
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int IdUser { get; set; }

        [JsonPropertyName("first")]
        public string FirstName { get; set; }=null!;

        [JsonPropertyName("secondName")]
        public string SecondName { get; set; }=null!;


        public string? MiddleName { get; set; }

        public string? Tel { get; set; }

        public string? Email { get; set; }

        [Required]
        public bool SearchWork { get; set; }

        public UsersData() { }

        public UsersData(DataUserApi userApi) => (this.IdUser, this.FirstName, this.SecondName, this.MiddleName, this.Tel, this.Email, this.SearchWork) =
            (Int32.Parse(userApi.id), userApi.first_name, userApi.last_name, userApi.mid_name is null ? null : (string)userApi.mid_name, userApi.phone ?? null,
            userApi.email ?? null, userApi.is_in_search);
    }
}
