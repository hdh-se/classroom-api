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
using System.Threading;
using System.Threading.Tasks;
using ExcelDataReader.Log;
using static ManageCourse.Core.DataAuthSources.AppUserStore;
using ManageCourse.Core.Constansts;
using ManageCourseAPI.WebSocket;
using Microsoft.Extensions.Hosting.Internal;

namespace ManageCourseAPI
{
    public class Startup
    {
        private WebSocket.WebSocket socket;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            // socket = new WebSocket.WebSocket();
            // socket.Run();
        }

        public IConfiguration Configuration { get; }

        private void OnStopped()
        {
            socket.Stop();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //SQL Server Context
            var connectionStringUser = Configuration.GetConnectionString("ManagerCourse");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionStringUser, b => b.MigrationsAssembly("ManageCourse.Migrations"));
            });

            //SQL Server Context
            var connectionStringUserAuth = Configuration.GetConnectionString("User");
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseSqlServer(connectionStringUserAuth,
                    b => b.MigrationsAssembly("ManageCourse.Migrations"));
            });

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
                    options.Authority = ConfigConstant.URL_API;
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
            services.AddSingleton<IWebSocket, WebSocket.WebSocket>();

            services.AddScoped<AppUserStore>();
            services.TryAddScoped<ICourseService, CourseService>();
            services.TryAddScoped<IUserService, UserService>();
            services.TryAddScoped<IAdminService, AdminService>();
            services.TryAddScoped<IAppUserStore, AppUserStore>();
            services.TryAddScoped<IGradeReviewService, GradeReviewService>();
            services.TryAddScoped<INotitficationService, NotitficationService>();
            services.TryAddScoped<DbContextContainer>();
            services.TryAddScoped<IGeneralModelRepository, GeneralModelRepository>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "ManageCourseAPI", Version = "v1"});
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

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {{jwtSecurityScheme, Array.Empty<string>()}});
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            IHostApplicationLifetime applicationLifetime)
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

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

       
            applicationLifetime.ApplicationStarted.Register(()=> 
                app.ApplicationServices.GetService<IWebSocket>()?.Run());
            applicationLifetime.ApplicationStopped.Register(() =>
                app.ApplicationServices.GetService<IWebSocket>()?.Stop());
        }

    }
}