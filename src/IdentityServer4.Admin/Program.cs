using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotBPE.Protocol.Amp;
using DotBPE.Rpc;
using DotBPE.Rpc.Hosting;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
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

            var host = CreateWebHostBuilder(args).Build();
            if (seed)
            {
                SeedData.EnsureSeedData(host.Services).ConfigureAwait(true);
            }

            host.Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>().UseSerilog().UseUrls("http://*:6566");
    }
}