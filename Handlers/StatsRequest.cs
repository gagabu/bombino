using MediatR;
using Telegram.Bot.Types;

namespace BombinoBomberBot.Handlers
{
    public class StatsRequest : IRequest
    {
        public Message Message { get; }

        public StatsRequest(Message message)
        {
            Message = message;
        }
    }
}