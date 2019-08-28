using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace BombinoBomberBot.Telegram
{
    public static class TelegramExtensions
    {
        public static void AddTelegram(this IServiceCollection services, string apiKey)
        {
            var telegram = 
            services.AddSingleton<TelegramBotClient>(x => new TelegramBotClient(apiKey));
            services.AddSingleton<TelegramBotPolling>();
        }
        
        public static TelegramBotBuilder UseTelegramBot(this IApplicationBuilder app, IConfiguration configuration)
        {
            var builder = new TelegramBotBuilder(app);

            var mode = configuration.GetValue<string>("TelegramUpdateMode");

            switch (mode)
            {
                case "Pooling":
                    builder.WithPolling();
                    break;
                case "WebHook":
                    builder.WithWebHooks(configuration.GetValue<string>("TelegramCallback"));
                    break;
            }

            return builder;
        }
    }
}
