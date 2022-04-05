using Telegram.Bot;

namespace BombinoBomberBot.Telegram
{
    public static class TelegramExtensions
    {
        public static void AddTelegram(this IServiceCollection services, string apiKey)
        {
            var telegram = 
            services.AddSingleton(_ => new TelegramBotClient(apiKey));
        }
        
        public static void UseTelegramBot(this IApplicationBuilder app, IConfiguration configuration)
        {
            var builder = new TelegramBotBuilder(app);
            builder.WithWebHooks(configuration.GetValue<string>("TelegramCallback"));
        }
    }
}
