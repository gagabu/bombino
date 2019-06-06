using Autofac.Extensions.DependencyInjection;
using Serilog;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

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
                                    .WriteTo.Console()
                                    .WriteTo.File("messages.log");
                               })
                   .ConfigureServices(s => s.AddAutofac());
    }
}
