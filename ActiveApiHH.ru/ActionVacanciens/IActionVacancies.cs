using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ActiveApiHH.ru
{
    internal interface IActionVacancies
    {
        public string SearchVacancies(string Token, string SearchText);
    }
}
