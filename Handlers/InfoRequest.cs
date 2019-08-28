using MediatR;
using Telegram.Bot.Types;

namespace BombinoBomberBot.Handlers
{
    public class InfoRequest : IRequest
    {
        public InfoRequest(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}