using System.Threading.Tasks;
using Microsoft.Extensions.Localization;
using Telegram.Bot;

namespace BombinoBomberBot.Helpers
{
    public class ResponseLocalizer
    {
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly TelegramBotClient _telegram;

        public ResponseLocalizer(IStringLocalizer<SharedResource> localizer, TelegramBotClient telegram)
        {
            _localizer = localizer;
            _telegram = telegram;
        }

        public Task SendAsync(long chatId, string key, params object[] args)
        {   
            var response = _localizer[key, args];
            return _telegram.SendTextMessageAsync(chatId, response.ToString());

        }
    }
}
