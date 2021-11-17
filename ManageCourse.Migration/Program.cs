using ManageCourse.Core.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ManageCourse.Migrations
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //SQL Server
            //var serviceCollection = new ServiceCollection();

            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true).Build();
            //serviceCollection.AddSingleton(configuration);

            var connectionStringUser = configuration.GetConnectionString("ManagerCourse");
            var optionsUser = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(connectionStringUser, ctx =>
                {
                    ctx.MigrationsAssembly(typeof(Program).Assembly.FullName);
                })
                .Options;
            var appDbContext = new AppDbContext(optionsUser);
            appDbContext.Database.Migrate();

            //serviceCollection.AddScoped<AppDbContext>();
            //var serviceProvider = serviceCollection.BuildServiceProvider();


            //==================================================================================

            var connectionStringUserAuth = configuration.GetConnectionString("User");
            var optionsUserAuth = new DbContextOptionsBuilder<AuthDbContext>()
                .UseSqlServer(connectionStringUserAuth, ctx =>
                {
                    ctx.MigrationsAssembly(typeof(Program).Assembly.FullName);
                })
                .Options;
            var authDbContext = new AuthDbContext(optionsUserAuth);
            authDbContext.Database.Migrate();

            //==================================================================================

            //MYSQL
            //var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", true).Build();

            //var connectionStringUser_MYSQL = configuration.GetConnectionString("ManagerCourse_MYSQL");
            //var optionsUser = new DbContextOptionsBuilder<AppDbContext>()
            //    .UseMySql(connectionStringUser_MYSQL
            //    , ServerVersion.Parse("8.0.19-mysql")
            //    , ctx =>
            //    {
            //        ctx.MigrationsAssembly(typeof(Program).Assembly.FullName);
            //    })
            //    .Options;

            //var appDbContext = new AppDbContext(optionsUser);
            //appDbContext.Database.Migrate();

            ////==================================================================================

            //var connectionStringUserAuth_MYSQL = configuration.GetConnectionString("User_MYSQL");
            //var optionsUserAuth = new DbContextOptionsBuilder<AuthDbContext>()
            //    .UseMySql(
            //    connectionStringUserAuth_MYSQL
            //    , ServerVersion.Parse("8.0.19-mysql")
            //    , ctx =>
            //    {
            //        ctx.MigrationsAssembly(typeof(Program).Assembly.FullName);
            //    })
            //    .Options;

            //var authDbContext = new AuthDbContext(optionsUserAuth);
            //authDbContext.Database.Migrate();
        }
    }
}
