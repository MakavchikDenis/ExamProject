using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryModels.Repository
{
    [Table("DetailsVacanciesForUser", Schema = "Main")]
    public class DetailsVacanciesForUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; } = default;

        public int idUser { get; set; }

        [Column("Name")]
        public string NameVacancy { get; set; }

        [Column("Url")]
        public string? UrlVacancy { get; set; }

        public double? SalaryFrom { get; set; }

        public double? SalaryTo { get; set; }

        public string? SalaryCurrency { get; set; }

        public string EmployerName { get; set; }

        public string? Description { get; set; }

        //public string?  Responsibility {get;set;}

        public DetailsVacanciesForUser() { }


        public DetailsVacanciesForUser(int _idUser, string _nameVacancy, string? _urlVacancy, float? _salaryFrom, float? _salaryTo, string? _salaryCurrency,
            string _employerName, string? _description)
        {
            this.idUser = _idUser;
            this.NameVacancy = _nameVacancy;
            this.UrlVacancy = _urlVacancy;
            this.SalaryFrom = _salaryFrom;
            this.SalaryTo = _salaryTo;
            this.SalaryCurrency = _salaryCurrency;
            this.EmployerName = _employerName;
            this.Description = _description;
            //this.Responsibility = _responsibility;
        }

    }
}
