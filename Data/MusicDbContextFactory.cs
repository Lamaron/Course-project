using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class MusicDbContextFactory : IDesignTimeDbContextFactory<MusicDbContext>
    {
        public MusicDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<MusicDbContext>();
            optionsBuilder.UseSqlServer(
                @"Server=.\SQLEXPRESS;Database=MusicPlayerDB;Trusted_Connection=True;TrustServerCertificate=True");

            return new MusicDbContext(optionsBuilder.Options);
        }
    }
}
