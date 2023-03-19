using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using API.Models;

namespace LibraryModels.Repository.WorkWithEF
{
    internal class Context : DbContext
    {
        private string ConnectToDb;

        public DbSet<Session> Sessions { get; set; }
        public DbSet<UsersData> Users { get; set; }
        public DbSet<VacanciesUser> VacanciesUsers { get; set; }
        public DbSet<ViewVacancies> ViewVacancies { get; set; }
        public DbSet<DetailsVacanciesForUser> DetailsVacanciesForUsers { get; set; }

        public Context(string _ConnectToDb) => ConnectToDb = _ConnectToDb;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(ConnectToDb);

        protected override void OnModelCreating(ModelBuilder modelBuilder) {

            modelBuilder.Entity<ViewVacancies>().HasNoKey().ToView("ViewVacanciesForUser");
            

        }




    }
}
