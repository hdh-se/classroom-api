using ManageCourse.Core.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageCourse.Migrations
{
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<AuthDbContext> optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            string connstr = "localhost";
            _ = optionsBuilder.UseMySql(connstr, ServerVersion.Parse("8.0.19-mysql"), options =>
            {
                _ = options.MigrationsAssembly(GetType().Assembly.FullName);
            });
            
            return new AuthDbContext(optionsBuilder.Options);
        }
    }
}
