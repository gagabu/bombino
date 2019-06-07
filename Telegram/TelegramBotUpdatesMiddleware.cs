using System.IO;
using System.Threading.Tasks;
using BombinoBomberBot.Handlers;
using Google.Apis.Logging;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace BombinoBomberBot.Telegram
{
    internal class TelegramBotUpdatesMiddleware
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TelegramBotUpdatesMiddleware> _logger;
        private readonly RequestDelegate _next;

        private static readonly JsonSerializer Serializer = new JsonSerializer();

        public TelegramBotUpdatesMiddleware(IMediator mediator, ILogger<TelegramBotUpdatesMiddleware> logger, RequestDelegate next)
        {
            _mediator = mediator;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var update = Serializer.Deserialize<Update>(new JsonTextReader(new StreamReader(context.Request.Body)));
            if (update != null)
            {
                await _mediator.Send(new GenericUpdateRequest(update.Message));
            }
            else
            {
                using (_logger.BeginScope(new
                                       {
                                            context.Connection.RemoteIpAddress,
                                       }))
                {
                    _logger.LogWarning("Someone came here");
                }
            }
        }
    }
}