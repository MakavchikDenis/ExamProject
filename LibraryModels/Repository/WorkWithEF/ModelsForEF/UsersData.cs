using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryModels.Repository
{
    [Table("UsersData", Schema = "Main")]
    public class UsersData
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        public int IdUser { get; set; }


        public string FirstName { get; set; } = null!;


        public string SecondName { get; set; } = null!;


        public string? MiddleName { get; set; }

        public string? Tel { get; set; }

        public string? Email { get; set; }


        public bool SearchWork { get; set; }

        public UsersData() { }

        public UsersData(DataUserApi userApi)
        {
            this.IdUser = Int32.Parse(userApi.id);
            this.FirstName = userApi.first_name;
            this.SecondName = userApi.last_name;
            this.MiddleName = userApi.middle_name is null ? null : userApi.middle_name.ToString();
            this.Tel = userApi.phone ?? null;
            this.Email = userApi.email ?? null;
            this.SearchWork = userApi.is_in_search;
        }
                
    }
}
