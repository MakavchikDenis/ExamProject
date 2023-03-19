using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;


namespace LibraryModels.Repository
{

    public class Vacancy
    {

        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public Salary? Salaries { get; set; }
        public string EmployerName { get; set; }
        public string? Required { get; set; }
        public string? Responsibility { get; set; }

        public Vacancy() { }

        public Vacancy(string _id, string _name, string _url, Salary? _salaries, string employerName, string? required, string? responsibility)
        {
            this.Id = _id;
            this.Name = _name;
            this.Url = _url;
            this.Salaries = _salaries;
            this.EmployerName = employerName;
            this.Required = required; ;
            this.Responsibility = responsibility;

        }
    }

    public class Salary
    {
        public float? From { get; set; }
        public float? To { get; set; }
        public string? Currency { get; set; }

        public Salary(float? from, float? to, string? currency)
        {
            From = from;
            To = to;
            Currency = currency;
        }
    }


}

