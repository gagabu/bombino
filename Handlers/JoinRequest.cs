using MediatR;
using Telegram.Bot.Types;

namespace BombinoBomberBot.Handlers
{
    public class JoinRequest : IRequest
    {
        public Message Message { get; }

        public JoinRequest(Message message)
        {
            Message = message;
        }
    }
}