using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryModels.Repository
{
    [Table("VacanciesUser", Schema ="Main")]
    public class VacanciesUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int IdUser { get; set; }

        public DateTime DateUpdate { get; set; } 

        public byte[] Content { get; set; }
    }
}
