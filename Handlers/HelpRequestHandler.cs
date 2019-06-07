using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using BombinoBomberBot.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace BombinoBomberBot.Handlers
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class HelpRequestHandler : AsyncRequestHandler<HelpRequest>
    {
        private readonly ILogger<IRequest> _logger;
        private readonly TelegramBotClient _telegram;
        private readonly ResponseLocalizer _response;
        
        public HelpRequestHandler(TelegramBotClient telegram, ResponseLocalizer response, ILogger<IRequest> logger)
        {
            _telegram = telegram;
            _response = response;
            _logger = logger;
        }

        protected override async Task Handle(HelpRequest request, CancellationToken cancellationToken)
        {
            var message = request.Message;
            _logger.LogInformation("Help request from {User}", message.From);

            await _response.SendAsync(message.Chat.Id, "Help", message.From.Id);
        }
    }
}