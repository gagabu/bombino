using Telegram.Bot;

namespace BombinoBomberBot.Telegram
{
    public class TelegramBotBuilder
    {
        private readonly IApplicationBuilder _applicationBuilder;

        public TelegramBotBuilder(IApplicationBuilder app)
        {
            _applicationBuilder = app;
        }

        public void WithWebHooks(string callback)
        {
            _applicationBuilder.Map("/updates", x => { x.UseMiddleware<TelegramBotUpdatesMiddleware>(); });

            _applicationBuilder.ApplicationServices.GetRequiredService<TelegramBotClient>().SetWebhookAsync(callback).Wait();
        }
    }
}