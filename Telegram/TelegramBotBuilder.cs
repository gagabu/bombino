using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
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

        public TelegramBotBuilder WithPolling()
        {
            _applicationBuilder.ApplicationServices.GetService<TelegramBotPolling>().Start();
            return this;
        }

        public TelegramBotBuilder WithWebHooks(string callback)
        {
            _applicationBuilder.Map("/updates", x => { x.UseMiddleware<TelegramBotUpdatesMiddleware>(); });

            _applicationBuilder.ApplicationServices.GetService<TelegramBotClient>().SetWebhookAsync(callback).Wait();

            return this;
        }
    }
}