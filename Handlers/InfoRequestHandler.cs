using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using BombinoBomberBot.Helpers;
using MediatR;
using Microsoft.Extensions.Localization;
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
            // TODO: add rules description and localization
            var message = request.Message;
            _logger.LogInformation("Info request from {From}", message.From.Username);

            await _response.SendAsync(message.Chat.Id, "HelloInfo", message.From.Username);

            var info = "``` " + JsonConvert.SerializeObject(message, Formatting.Indented) + " ```";

            await _telegram.SendTextMessageAsync(message.Chat.Id, info, ParseMode.Markdown,
                                                  cancellationToken: cancellationToken);
        }
    }
}