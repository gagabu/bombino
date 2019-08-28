﻿using System.Threading.Tasks;
using BombinoBomberBot.Handlers;
using MediatR;
using Newtonsoft;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using System.IO;

namespace BombinoBomberBot.Telegram
{
    internal class TelegramBotUpdatesMiddleware
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TelegramBotUpdatesMiddleware> _logger;
        private readonly RequestDelegate _next;
        
        public TelegramBotUpdatesMiddleware(IMediator mediator, ILogger<TelegramBotUpdatesMiddleware> logger, RequestDelegate next)
        {
            _mediator = mediator;
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var reader = new StreamReader(context.Request.Body);
            var raw = await reader.ReadToEndAsync();            
            var update = JsonConvert.DeserializeObject<Update>(raw);
            
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