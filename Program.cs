using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BombinoBomberBot.Handlers;
using BombinoBomberBot.Helpers;
using BombinoBomberBot.Model;
using BombinoBomberBot.Telegram;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.GoogleCloudLogging;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SerilogSetup);
builder.Services.AddControllers();
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(ConfigureAutofac));

builder.Services.AddTelegram(builder.Configuration.GetValue<string>("TelegramBotApiKey"));
builder.Services.AddLocalization();

builder.Services.AddDbContext<BomberBotContext>(x=> x.UseNpgsql(builder.Configuration.GetSection("ConnectionString").Value));

var app = builder.Build();
app.MapControllers();
app.UseTelegramBot(app.Configuration);

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    var context = serviceScope.ServiceProvider.GetRequiredService<BomberBotContext>();
    context.Database.Migrate();
}

app.Run();

void ConfigureAutofac(ContainerBuilder b)
{
    b.RegisterType<Mediator>()
        .As<IMediator>()
        .InstancePerLifetimeScope();

    b.Register<ServiceFactory>(context =>
    {
        var c = context.Resolve<IComponentContext>();
        return t => c.Resolve(t);
    });

    b.RegisterType<ResponseLocalizer>().SingleInstance();

    b.RegisterAssemblyTypes(typeof(GenericUpdateHandler).GetTypeInfo().Assembly)
        .AsClosedTypesOf(typeof(IRequestHandler<,>))
        .AsImplementedInterfaces()
        .InstancePerDependency();

    b.RegisterAssemblyTypes(typeof(GenericUpdateHandler).GetTypeInfo().Assembly)
        .AsClosedTypesOf(typeof(IRequestHandler<>))
        .AsImplementedInterfaces()
        .InstancePerDependency();
            
    b.RegisterAssemblyTypes(typeof(GenericUpdateHandler).GetTypeInfo().Assembly)
        .AsClosedTypesOf(typeof(INotificationHandler<>))
        .AsImplementedInterfaces()
        .InstancePerDependency();
}

void SerilogSetup(HostBuilderContext b, LoggerConfiguration c)
{
    c.MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .WriteTo.Console(LogEventLevel.Debug);

    if (b.HostingEnvironment.IsProduction())
    {
        c.WriteTo.Console(LogEventLevel.Error);
        c.WriteTo.GoogleCloudLogging(new GoogleCloudLoggingSinkOptions { ProjectId = b.Configuration.GetValue<string>("GoogleProjectId"), UseJsonOutput = true });
    }

    c.ReadFrom.Configuration(b.Configuration);
}
