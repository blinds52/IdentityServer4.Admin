using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace IdentityServer4.Admin
{
    public static class Program
    {
        public static void Main(string[] args)
        {
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
                .UseStartup<Startup>().UseSerilog();
    }
}