using MediatR;
using Telegram.Bot.Types;

namespace BombinoBomberBot.Handlers
{
    public class LeaveRequest : IRequest
    {
        public Message Message { get; }

        public LeaveRequest(Message message)
        {
            Message = message;
        }
    }
}