using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Models;

namespace LibraryModels.Repository
{
    internal class Context : DbContext
    {
        private string ConnectToDb;

        public DbSet<Session> Sessions { get; set; }

        public Context(string _ConnectToDb) => this.ConnectToDb = _ConnectToDb;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(ConnectToDb);
       
        

    }
}
