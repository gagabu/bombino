using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using BombinoBomberBot.Helpers;
using BombinoBomberBot.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BombinoBomberBot.Handlers
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class LeaveRequestHandler : AsyncRequestHandler<LeaveRequest>
    {
        private readonly BomberBotContext _context;
        private readonly ResponseLocalizer _response;
        private readonly ILogger<IRequest> _logger;

        public LeaveRequestHandler(BomberBotContext context, ResponseLocalizer response, ILogger<IRequest> logger)
        {
            _context = context;
            _response = response;
            _logger = logger;
        }

        protected override async Task Handle(LeaveRequest request, CancellationToken cancellationToken)
        {
            var message = request.Message;
            var room = await _context.Rooms.Include(x => x.Users).FirstOrDefaultAsync(x => x.TelegramChatId == message.Chat.Id, cancellationToken);

            if (room == null)
            {
                _logger.LogWarning("User {Username}:{UserId} tries to leave game from unknown chat:{ChatId}", message.From.Username, message.From.Id, message.Chat.Id);
                return;
            }

            var user = await _context.RoomUsers
                                     .FirstOrDefaultAsync(x => x.User.TelegramUserId == message.From.Id && x.Room.TelegramChatId == message.Chat.Id,
                                                          cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User {Username}:{UserId} tries to leave in chat:{ChatId}, but it doesn't play game", message.From.Username, message.From.Id, message.Chat.Id);

                await _response.SendAsync(message.Chat.Id, "UserLeftFailed", message.From.Username);
            }
            else
            {
                room.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("User {Username}:{UserId} left the game in chat:{ChatId}", message.From.Username, message.From.Id, message.Chat.Id);
                
                await _response.SendAsync(message.Chat.Id, "UserLeftGame", message.From.Username);
            }
        }
    }
}
