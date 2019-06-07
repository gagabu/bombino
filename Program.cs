using Autofac.Extensions.DependencyInjection;
using Serilog;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog.Sinks.GoogleCloudLogging;

namespace BombinoBomberBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                   .UseStartup<Startup>()
                   .UseSockets()
                   .UseSerilog((b, c) =>
                               {
                                   c.MinimumLevel.Debug()
                                    .Enrich.FromLogContext()
                                    .WriteTo.Console(LogEventLevel.Error);
                                   
                                   if (b.HostingEnvironment.IsProduction())
                                   {
                                       c.WriteTo.GoogleCloudLogging(new GoogleCloudLoggingSinkOptions
                                                                        {
                                                                                ProjectId = b.Configuration.GetValue<string>("GoogleProjectId"),
                                                                                UseJsonOutput = true
                                                                        });
                                   }
                               })
                   .ConfigureServices(s => s.AddAutofac());
    }
}
