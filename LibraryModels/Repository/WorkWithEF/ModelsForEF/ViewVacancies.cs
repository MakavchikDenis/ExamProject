using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryModels.Repository
{
    public class ViewVacancies
    {
        public int Id { get; set; }

        public string Vacancie { get; set; }

        public DateTime Date { get; set; }

        public int User { get; set; }

        public byte[] Content { get; set; }
    }
}
