using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace IdentityServer4.Admin
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            args = new[] {"/seed"};

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console().WriteTo.RollingFile("ids4.log")
                .CreateLogger();
            var seed = args.Contains("/seed");
            if (seed)
            {
                args = args.Except(new[] {"/seed"}).ToArray();
            }

            var host = CreateWebHostBuilder(args).Build();
            if (seed)
            {
                SeedData.EnsureSeedData(host.Services);
            }

            host.Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}