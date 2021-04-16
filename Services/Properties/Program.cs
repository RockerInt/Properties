using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Properties
{
    public class Program
    {
        private const string AppName = "Properties.Microservice";
        public static int Main(string[] args)
        {
            var config = GetConfiguration();
            Log.Logger = CreateSerilogLogger(config);
            try
            {
                Log.Information("Configuring web host ({ApplicationContext})...", AppName);
                var host = CreateHostBuilder(config, args);

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

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.ConfigureKestrel(options =>
                    //{
                    //    var (httpPort, httpsPort, grpcPort) = GetDefinedPorts(configuration);
                    //    options.Listen(IPAddress.Any, httpPort, listenOptions =>
                    //    {
                    //        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    //    });

                    //    options.Listen(IPAddress.Any, httpsPort, listenOptions =>
                    //    {
                    //        listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                    //    });

                    //    options.Listen(IPAddress.Any, grpcPort, listenOptions =>
                    //    {
                    //        listenOptions.Protocols = HttpProtocols.Http2;
                    //    });

                    //});
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
                .Enrich.WithProperty("ApplicationContext", AppName)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File($"{Directory.GetCurrentDirectory()}/PropertiesLog.json")
                .WriteTo.MongoDB(mongoServerUrl, collectionName: "properties.microservice")
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }

        private static (int httpPort, int httpsPort, int grpcPort) GetDefinedPorts(IConfiguration config)
        {
            var httpPort = config.GetValue("HTTP_PORT", 52964);
            var httpsPort = config.GetValue("HTTPS_PORT", 44304);
            var grpcPort = config.GetValue("GRPC_PORT", 5001);
            return (httpPort, httpsPort, grpcPort);
        }
    }
}
