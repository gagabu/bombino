using MediatR;
using Telegram.Bot.Types;

namespace BombinoBomberBot.Handlers
{
    public class GenericUpdateRequest : IRequest
    {
        public GenericUpdateRequest(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}