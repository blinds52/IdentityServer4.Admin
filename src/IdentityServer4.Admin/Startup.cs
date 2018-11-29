using System.Reflection;
using IdentityServer4.Admin.Common;
using IdentityServer4.Admin.Data;
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

            if (!HostingEnvironment.IsDevelopment())
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
            else
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                    .MinimumLevel.Override("System", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                    .Enrich.FromLogContext()
                    .WriteTo.Console().WriteTo.RollingFile("ids4.log")
                    .CreateLogger();
            }

            // Add aspnetcore identity
            IdentityBuilder idBuilder = services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.User.RequireUniqueEmail = false;
            });

            string connectionString = Configuration.GetSection("IdentityServer4Admin")
                .GetValue<string>("ConnectionString");

            // Add DbContext
            if (!HostingEnvironment.IsDevelopment())
            {
                services.AddDbContext<AdminDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }
            else
            {
                services.AddDbContext<AdminDbContext>(options => options.UseInMemoryDatabase("IDS4"));
            }

            idBuilder.AddDefaultTokenProviders();
            idBuilder.AddEntityFrameworkStores<AdminDbContext>();

            // Add ids4
            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            }).AddAspNetIdentity<User>();
            if (HostingEnvironment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
                builder.AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = b =>
                            b.UseInMemoryDatabase("IDS4");
                    })
                    // this adds the operational data from DB (codes, tokens, consents)
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = b =>
                            b.UseInMemoryDatabase("IDS4");
                        // this enables automatic token cleanup. this is optional.
                        options.EnableTokenCleanup = true;
                        // frequency in seconds to cleanup stale grants. 15 is useful during debugging
                        options.TokenCleanupInterval = 3600;
                    });
            }
            else
            {
                var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
                builder.AddConfigurationStore(options =>
                    {
                        options.ConfigureDbContext = b =>
                            b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                    })
                    // this adds the operational data from DB (codes, tokens, consents)
                    .AddOperationalStore(options =>
                    {
                        options.ConfigureDbContext = b =>
                            b.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly));
                        // this enables automatic token cleanup. this is optional.
                        options.EnableTokenCleanup = true;
                        // frequency in seconds to cleanup stale grants. 15 is useful during debugging
                        options.TokenCleanupInterval = 15;
                    });
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
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}