using System;
using BombinoBomberBot.Handlers;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace BombinoBomberBot.Telegram
{
    public class TelegramBotPolling : IDisposable
    {
        private readonly TelegramBotClient _client;
        private readonly IMediator _mediator;
        private readonly ILogger<TelegramBotPolling> _logger;

        public TelegramBotPolling(TelegramBotClient client, IMediator mediator, ILogger<TelegramBotPolling> logger)
        {
            _client = client;
            _mediator = mediator;
            _logger = logger;
            _client.OnMessage += OnNewMessage;
        }

        public void Start()
        {
            _client.StartReceiving();
        }

        public void Dispose()
        {
            _client.StopReceiving();
            _client.OnMessage -= OnNewMessage;
        }
        private async void OnNewMessage(object sender, MessageEventArgs e)
        {
            await _mediator.Send(new GenericUpdateRequest(e.Message));
        }
    }
}