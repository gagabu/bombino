using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using BombinoBomberBot.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace BombinoBomberBot.Handlers
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class InfoRequestHandler : AsyncRequestHandler<InfoRequest>
    {
        private readonly ILogger<IRequest> _logger;
        private readonly TelegramBotClient _telegram;
        private readonly ResponseLocalizer _response;
        
        public InfoRequestHandler(TelegramBotClient telegram, ResponseLocalizer response, ILogger<IRequest> logger)
        {
            _telegram = telegram;
            _response = response;
            _logger = logger;
        }

        protected override async Task Handle(InfoRequest request, CancellationToken cancellationToken)
        {
            var message = request.Message;
            _logger.LogInformation("Info request from {User}", message.From);

            await _response.SendAsync(message.Chat.Id, "HelloInfo", message.From.Mention());

            var info = "``` " + JsonConvert.SerializeObject(message, Formatting.Indented) + " ```";

            await _telegram.SendTextMessageAsync(message.Chat.Id, info, ParseMode.Markdown,
                                                  cancellationToken: cancellationToken);
        }
    }
}