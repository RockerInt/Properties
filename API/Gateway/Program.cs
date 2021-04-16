using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Properties.Gateway
{
    public class Program
    {
        private const string AppName = "Properties.Gateway";
        public static int Main(string[] args)
        {
            Log.Logger = CreateSerilogLogger(GetConfiguration());
            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var host = CreateHostBuilder(args);

                Log.Information("Starting web host ({ApplicationContext})...", AppName);
                host.Build().Run();

                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Program terminated unexpectedly ({ApplicationContext})!", AppName);
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration(config =>
                {
                    var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    var envFile = Environment.GetEnvironmentVariable("ENV_FILE") ?? env;
                    config.Sources.Remove(
                        config.Sources.FirstOrDefault(x => x.GetType().Name == "JsonConfigurationSource" 
                            && ((Microsoft.Extensions.Configuration.Json.JsonConfigurationSource)x).Path == $"appsettings.{env}.json"));


                    config.AddJsonFile($"appsettings.{envFile}.json", optional: true, reloadOnChange: true);

                    config.AddEnvironmentVariables();

                    if (args != null)
                        config.AddCommandLine(args);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        private static ILogger CreateSerilogLogger(IConfiguration configuration)
        {
            var mongoServerUrl = configuration["Serilog:MongoServerUrl"];
            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.WithProperty("ApplicationContext", "Properties.Gateway")
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File($"{Directory.GetCurrentDirectory()}/GatewayLog.json")
                .WriteTo.MongoDB(mongoServerUrl, collectionName: "properties.gateway")
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}
