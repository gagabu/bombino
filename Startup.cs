using System.Reflection;
using Autofac;
using BombinoBomberBot.Handlers;
using BombinoBomberBot.Helpers;
using BombinoBomberBot.Model;
using BombinoBomberBot.Telegram;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BombinoBomberBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTelegram(Configuration.GetValue<string>("TelegramBotApiKey"));
            services.AddLocalization();
            services.AddEntityFrameworkNpgsql()
                    .AddDbContext<BomberBotContext>(x=> x.UseNpgsql(Configuration.GetSection("ConnectionString").Value));
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<Mediator>()
                   .As<IMediator>()
                   .InstancePerLifetimeScope();

            builder.Register<ServiceFactory>(context =>
                                             {
                                                 var c = context.Resolve<IComponentContext>();
                                                 return t => c.Resolve(t);
                                             });

            builder.RegisterType<ResponseLocalizer>().SingleInstance();

            builder.RegisterAssemblyTypes(typeof(GenericUpdateHandler).GetTypeInfo().Assembly)
                   .AsClosedTypesOf(typeof(IRequestHandler<,>))
                   .AsImplementedInterfaces()
                   .InstancePerDependency();

            builder.RegisterAssemblyTypes(typeof(GenericUpdateHandler).GetTypeInfo().Assembly)
                   .AsClosedTypesOf(typeof(IRequestHandler<>))
                   .AsImplementedInterfaces()
                   .InstancePerDependency();
            
            builder.RegisterAssemblyTypes(typeof(GenericUpdateHandler).GetTypeInfo().Assembly)
                   .AsClosedTypesOf(typeof(INotificationHandler<>))
                   .AsImplementedInterfaces()
                   .InstancePerDependency();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseTelegramBot(Configuration);
        }
    }
}
