﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryModels.Repository
{
    public  class Loggs
    {
        public int? Id { get; set; }
        public Guid? Token { get; set; }
        public DateTime DateAction { get; set; }
        public string Action { get; set; }
        public string ActionResult { get; set; }
        public string? ActionDetails { get; set; }
        public string? ErrorMessage { get; set; }

    }
}
