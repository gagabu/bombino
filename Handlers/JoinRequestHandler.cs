using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
    public class JoinRequestHandler : AsyncRequestHandler<JoinRequest>
    {
        private readonly BomberBotContext _context;
        private readonly ResponseLocalizer _response;
        private readonly ILogger<IRequest> _logger;

        public JoinRequestHandler(BomberBotContext context, ResponseLocalizer response, ILogger<IRequest> logger)
        {
            _context = context;
            _response = response;
            _logger = logger;
        }

        protected override async Task Handle(JoinRequest request, CancellationToken cancellationToken)
        {
            var message = request.Message;
            var room = await _context.Rooms.FirstOrDefaultAsync(x => x.TelegramChatId == message.Chat.Id, cancellationToken);

            if (room == null)
            {
                room = new Room
                           {
                               TelegramChatId = message.Chat.Id,
                               Title = message.Chat.Title
                           };
                
                _context.Rooms.Add(room);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Chat:{ChatId} was created by user {User})", message.Chat.Id, message.From);
            }

            var user = await _context.Users.FirstOrDefaultAsync(x => x.TelegramUserId == message.From.Id,
                                                                cancellationToken);

            if (user == null)
            {
                user = new User
                           {
                               TelegramUserId = message.From.Id,
                               Username = message.From.Username,
                               FirstName = message.From.FirstName,
                               LastName = message.From.LastName
                           };

                _context.Users.Add(user);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User {User} has been created and joined to game in chat:{ChatId}", message.From, room.TelegramChatId);
            }

            var roomUser = await _context.RoomUsers
                            .FirstOrDefaultAsync(x => x.User.TelegramUserId == message.From.Id && x.Room.TelegramChatId == message.Chat.Id, cancellationToken);

            if (roomUser == null)
            {
                roomUser = new RoomUser
                                   {
                                       Room = room,
                                       User = user
                                   };
                
                _context.RoomUsers.Add(roomUser);
                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User {User} joined to play in chat:{ChatId}", message.From, message.Chat.Id);

                await _response.SendAsync(message.Chat.Id, "UserJoined", message.From.Mention());
            }
            else
            {
                _logger.LogWarning("User {User} has already been joined to play in chat:{ChatId}", message.From, message.Chat.Id);
                
                await _response.SendAsync(message.Chat.Id, "UserAlreadyJoined", message.From.Mention());
            }
        }
    }
}
