using GetVersion.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetVersion.DbContexts
{
    internal class PosgresqlContext:DbContext
    {
        public DbSet<VersionData> Versions { get; set; }

        public PosgresqlContext()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=VersionsDb;Username=ln;Password=ProAdmin777");
        }

    }
}
