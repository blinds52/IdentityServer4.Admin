using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using EnvironmentName = Microsoft.AspNetCore.Hosting.EnvironmentName;

namespace IdentityServer4.Admin
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console().WriteTo.RollingFile("ids4.log")
                .CreateLogger();


            var builder = WebHost.CreateDefaultBuilder(args).ConfigureAppConfiguration(config =>
                {
                    var configFile = args.FirstOrDefault(a => a.Contains("appsettings.json"));
                    if (configFile != null && File.Exists(configFile))
                    {
                        config.AddJsonFile(configFile);
                        Log.Logger.Information("Use extend config");
                    }
                })
                .UseStartup<Startup>().UseSerilog().UseUrls("http://*:6566");

            var seed = args.Contains("/seed");
            if (seed)
            {
                builder.UseSetting("seed", "true");
            }

            if (args.Contains("/dev"))
            {
                builder.UseEnvironment(EnvironmentName.Development);
            }

            if (args.Contains("/prod"))
            {
                builder.UseEnvironment(EnvironmentName.Production);
            }

            var host = builder.Build();

            host.Run();
        }
    }
}