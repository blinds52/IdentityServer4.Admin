using System;
using System.Reflection;
using AutoMapper;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace IdentityServer4.Admin
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        private IConfiguration Configuration { get; }

        private IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add configuration
            services.Configure<AdminOptions>(Configuration.GetSection("IdentityServer4Admin"));

            // Add MVC
            services.AddMvc()
                .AddMvcOptions(o => o.Filters.Add<HttpGlobalExceptionFilter>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add Log
            ConfigureLogService();

            services.AddAuthorization();           

            string connectionString = Configuration.GetSection("IdentityServer4Admin")
                .GetValue<string>("ConnectionString");

            // Add DbContext            
            Action<DbContextOptionsBuilder> dbContextOptionsBuilder;
            if (HostingEnvironment.IsDevelopment())
            {
                dbContextOptionsBuilder = b => b.UseInMemoryDatabase("IDS4");
            }
            else
            {
                var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
                dbContextOptionsBuilder = b =>
                    b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
            }

            services.AddDbContext<IDbContext, AdminDbContext>(dbContextOptionsBuilder);
            
            // Add aspnetcore identity
            IdentityBuilder idBuilder = services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = false;
            }).AddErrorDescriber<CustomIdentityErrorDescriber>();
            
            idBuilder.AddDefaultTokenProviders();
            idBuilder.AddEntityFrameworkStores<AdminDbContext>();

            // Add ids4
            var builder = services.AddIdentityServer()
                .AddAspNetIdentity<User>()
                // todo: config credential in production
                .AddDeveloperSigningCredential();
            builder.AddConfigurationStore<AdminDbContext>(options =>
                {
                    options.ResolveDbContextOptions = (provider, b) => dbContextOptionsBuilder(b);
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore<AdminDbContext>(options =>
                {
                    options.ResolveDbContextOptions = (provider, b) => dbContextOptionsBuilder(b);
                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                });

            // Configure AutoMapper
            ConfigureAutoMapper();

            services.AddCors(options =>
            {
                options.AddPolicy("vue-expert",
                    b => b.WithOrigins("http://localhost:6568").AllowAnyHeader());
            });
        }

        private void ConfigureAutoMapper()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<CreateUserDto, User>();
                cfg.CreateMap<Permission, PermissionDto>();
                cfg.CreateMap<PermissionDto, Permission>();
                cfg.CreateMap<Role, RoleDto>();
                cfg.CreateMap<RoleDto, Role>();
                cfg.CreateMap<User, UserOutputDto>();
            });
        }

        private void ConfigureLogService()
        {
            if (HostingEnvironment.IsDevelopment())
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
                    .MinimumLevel.Override("System", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console().WriteTo.RollingFile("ids4.log")
                    .CreateLogger();
            }
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console().WriteTo.RollingFile("ids4.log")
                    .CreateLogger();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseCors("vue-expert");
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}