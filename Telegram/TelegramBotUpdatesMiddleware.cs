using BombinoBomberBot.Handlers;
using MediatR;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace BombinoBomberBot.Telegram
{
    internal class TelegramBotUpdatesMiddleware
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TelegramBotUpdatesMiddleware> _logger;
        private readonly RequestDelegate _next;

        public TelegramBotUpdatesMiddleware(IMediator mediator, ILogger<TelegramBotUpdatesMiddleware> logger,
            RequestDelegate next)
        {
            _mediator = mediator;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var reader = new StreamReader(context.Request.Body);
            var body = await reader.ReadToEndAsync();

            var scope = new Dictionary<string, string>
            {
                { "RemoveIp", context.Connection.RemoteIpAddress.ToString() },
                { "Path", context.Request.Path }
            };

            foreach (var header in context.Request.Headers)
            {
                scope.Add($"Header.{header.Key}", header.Value.ToString());
            }

            using (_logger.BeginScope(scope))
            {
                try
                {
                    var update = JsonConvert.DeserializeObject<Update>(body);
                    if (update != null)
                    {
                        await _mediator.Send(new GenericUpdateRequest(update.Message));
                    }
                }
                catch (Exception ex)
                {
                    scope.Add("Body", body);
                    _logger.LogWarning(ex, "Unexpected error");
                }
            }
        }
    }
}
