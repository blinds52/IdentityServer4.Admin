using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Hosting;
using IdentityServer4.Admin.Controllers.API.Dtos;
using IdentityServer4.Admin.Entities;
using IdentityServer4.Admin.Infrastructure;
using IdentityServer4.Admin.Rpc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace IdentityServer4.Admin
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            _configuration = configuration;
            _hostingEnvironment = env;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var adminOptions = new AdminOptions(_configuration);
            // Add configuration
            services.AddSingleton(adminOptions);

            // Add MVC
            services.AddMvc()
                .AddMvcOptions(o => o.Filters.Add<HttpGlobalExceptionFilter>())
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthorization();

            string connectionString = _configuration["ConnectionString"];

            // Add DbContext            
            Action<DbContextOptionsBuilder> dbContextOptionsBuilder;
            if (_hostingEnvironment.IsDevelopment())
            {
                dbContextOptionsBuilder = b => b.UseInMemoryDatabase("IDS4");
            }
            else
            {
                var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
                var provider = _configuration["DatabaseProvider"];
                switch (provider.ToLower())
                {
                    case "mysql":
                    {
                        dbContextOptionsBuilder = b =>
                            b.UseMySql(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                        break;
                    }
                    case "sqlserver":
                    {
                        dbContextOptionsBuilder = b =>
                            b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                        break;
                    }
                    default:
                    {
                        throw new Exception($"Unsupported database provider: {provider}");
                    }
                }
            }

            services.AddDbContext<IDbContext, AdminDbContext>(dbContextOptionsBuilder);

            // Add aspnetcore identity
            IdentityBuilder idBuilder = services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireUppercase = adminOptions.RequireUppercase;
                options.Password.RequireNonAlphanumeric = adminOptions.RequireNonAlphanumeric;
                options.Password.RequireDigit = adminOptions.RequireDigit;
                options.Password.RequiredLength = adminOptions.RequiredLength;
                options.User.RequireUniqueEmail = adminOptions.RequireUniqueEmail;
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
                    options.EnableTokenCleanup = adminOptions.EnableTokenCleanup;
                });
            builder.AddProfileService<ProfileService>();

            // Configure AutoMapper
            ConfigureAutoMapper();

            AddRpc(services);
        }

        private void AddRpc(IServiceCollection services)
        {
            //添加协议支持
            services.AddDotBPE();
            //注册服务
            services.AddServiceActors<AmpMessage>(actors =>
            {
                actors.Add<UserService>();
                actors.Add<PermissionService>();
            });

            //添加挂载的宿主服务
            services.AddSingleton<IHostedService, RpcHostedService>();
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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            PrePareDatabase(app.ApplicationServices, _hostingEnvironment);
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void PrePareDatabase(IServiceProvider serviceProvider, IHostingEnvironment env)
        {
            using (IServiceScope scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var sp = scope.ServiceProvider;
                var logger = sp.GetRequiredService<ILogger<Startup>>();
                var options = sp.GetRequiredService<AdminOptions>();
                logger.LogInformation("Configuration: " + options.Version);
                if (sp.GetRequiredService<AdminDbContext>().Database.EnsureCreated())
                {
                    logger.LogInformation("Created database success");
                }
            }

            using (IServiceScope scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var sp = scope.ServiceProvider;
                SeedData.EnsureData(sp).Wait();

                if (env.IsDevelopment() && _configuration["seed"] == "true")
                {
                    SeedData.EnsureTestData(sp).Wait();
                }
            }
        }
    }
}