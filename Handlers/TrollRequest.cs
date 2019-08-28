using MediatR;
using Telegram.Bot.Types;

namespace BombinoBomberBot.Handlers
{
    public class TrollRequest : IRequest
    {
        public Message Message { get; }

        public TrollRequest(Message message)
        {
            Message = message;
        }
    }
}