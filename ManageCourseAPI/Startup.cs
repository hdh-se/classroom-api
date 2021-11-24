using IdentityServer4;
using ManageCourse.Core.Helpers;
using ManageCourse.Core.Data;
using ManageCourse.Core.DataAuthSources;
using ManageCourse.Core.DbContexts;
using ManageCourse.Core.Repositories;
using ManageCourse.Core.Services;
using ManageCourse.Core.Services.Implementation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using static ManageCourse.Core.DataAuthSources.AppUserStore;

namespace ManageCourseAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //SQL Server Context
            var connectionStringUser = Configuration.GetConnectionString("ManagerCourse");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionStringUser, b => b.MigrationsAssembly("ManageCourse.Migrations"));
            });

            //MySql context
            //var connectionStringUser_MYSQL = Configuration.GetConnectionString("ManagerCourse_MYSQL");
            //services.AddDbContext<AppDbContext>(options => options.UseMySql(
            //    connectionStringUser_MYSQL
            //    , ServerVersion.Parse("8.0.19-mysql")
            //    , b => b.MigrationsAssembly("ManageCourse.Migrations")
            //    )
            //, ServiceLifetime.Scoped, ServiceLifetime.Scoped);

            //SQL Server Context
            var connectionStringUserAuth = Configuration.GetConnectionString("User");
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(connectionStringUserAuth, b => b.MigrationsAssembly("ManageCourse.Migrations"));
            });

            //MySql context
            //var connectionStringUserAuth_MYSQL = Configuration.GetConnectionString("ManagerCourse_MYSQL");
            //services.AddDbContext<AppDbContext>(options => options.UseMySql(
            //    connectionStringUserAuth_MYSQL
            //    , ServerVersion.Parse("8.0.19-mysql")
            //    , b => b.MigrationsAssembly("ManageCourse.Migrations")
            //    )
            //, ServiceLifetime.Scoped, ServiceLifetime.Scoped);

            services.AddIdentity<AppUser, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<AuthDbContext>()
                .AddSignInManager<AppSignInManager>()
                .AddUserManager<AppUserManager>()
                .AddUserStore<AppUserStore>()
                .AddDefaultTokenProviders();

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                .AddInMemoryApiResources(IdentityServerInMemoryStores.ApiResources)
                .AddInMemoryApiScopes(IdentityServerInMemoryStores.ApiScopes)
                .AddInMemoryClients(IdentityServerInMemoryStores.Clients)
                .AddInMemoryIdentityResources(IdentityServerInMemoryStores.IdentityResources)
                .AddAspNetIdentity<AppUser>()
                .AddProfileService<IdentityServerProfileService>()
                .AddResourceOwnerValidator<IdentityServerResourceOwnerPasswordValidator>();

            builder.AddDeveloperSigningCredential();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.Authority = "https://localhost:44344";
                    options.ApiName = "courseapi";
                });

            services.AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            }).AddGoogle("Google", options =>
            {
                options.SaveTokens = true;

                options.ClientId = "738829553818-v8ci3noh3g7vr3rce4ob1f70dcd59pn9.apps.googleusercontent.com";
                options.ClientSecret = "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";
            });
            services.AddScoped<AppUserManager>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<AppUserStore>();
            services.TryAddScoped<ICourseService, CourseService>();
            services.TryAddScoped<IUserService, UserService>();
            services.TryAddScoped<IAppUserStore, AppUserStore>();
            services.TryAddScoped<DbContextContainer>();
            services.TryAddScoped<IGeneralModelRepository, GeneralModelRepository>();
            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ManageCourseAPI", Version = "v1" });
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
                { jwtSecurityScheme, Array.Empty<string>() }
    });
            });
            services.AddCors(options =>
            {
                options.AddPolicy(name: "MyPolicy",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ManageCourseAPI v1"));
            }

            app.UseRouting();
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseIdentityServer();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
