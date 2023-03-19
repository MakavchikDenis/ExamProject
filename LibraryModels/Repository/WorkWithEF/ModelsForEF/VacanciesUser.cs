using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace LibraryModels.Repository
{
    [Table("VacanciesUser", Schema ="Main")]
    public class VacanciesUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdUser { get; set; }

        public string TextVacancy { get; set; }

        public DateTime DateUpdate { get; set; }

        // не сериализуется
        [JsonIgnore]
        public byte[] Content { get; set; }

        // в БД не ложим
        [NotMapped]
        public List<Vacancy> Vacancies { get; set; }
    }
}
