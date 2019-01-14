using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
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

            if (args.Contains("/init"))
            {
                // EF Migrate
                var proc = Process.Start(new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "ef database update " + string.Join(" ", args),
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                });
                if (proc == null)
                {
                    Log.Logger.Information("EF Migrate execute failed.");
                    return;
                }

                proc.WaitForExit();
                var info = proc.StandardOutput.ReadToEnd();
                if (!info.EndsWith("Done.\n"))
                {
                    Log.Logger.Error(("EF Migrate execute failed."));
                }

                return;
            }

            var seed = args.Contains("/seed");
            if (seed)
            {
                args = args.Except(new[] {"/seed"}).ToArray();
            }

            var builder = CreateWebHostBuilder(args);

            if (args.Contains("/dev"))
            {
                builder.UseEnvironment(EnvironmentName.Development);
            }

            if (args.Contains("/prod"))
            {
                builder.UseEnvironment(EnvironmentName.Production);
            }

            var host = builder.Build();

            if (args.Contains("/dev") && seed)
            {
                SeedData.EnsureTestData(host.Services).ConfigureAwait(true);
            }

            if (args.Contains("/prod"))
            {
                SeedData.EnsureData(host.Services).ConfigureAwait(true);
            }

            host.Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>().UseSerilog().UseUrls("http://*:6566");
    }
}