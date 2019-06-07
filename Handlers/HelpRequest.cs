using MediatR;
using Telegram.Bot.Types;

namespace BombinoBomberBot.Handlers
{
    public class HelpRequest : IRequest
    {
        public HelpRequest(Message message)
        {
            Message = message;
        }

        public Message Message { get; }
    }
}