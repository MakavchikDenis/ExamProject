using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;


namespace LibraryModels.Repository
{
    
    public class Vacancie
    {
        
        public string Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string? Salary { get; set; }
        public string EmployerName { get; set; }
        public string? Required { get; set; }
        public string? Responsibility { get; set; }

        public Vacancie(string _id, string _name, string _url, string? _salary, string employerName, string? required, string? responsibility) {
            this.Id = _id;
            this.Name = _name;
            this.Url = _url;
            this.Salary = _salary;
            this.EmployerName = employerName;
            this.Required=required; ;
            this.Responsibility = responsibility;

        }
    }
}
