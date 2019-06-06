using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BombinoBomberBot.Helpers;
using BombinoBomberBot.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace BombinoBomberBot.Handlers
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class JoinRequestHandler : AsyncRequestHandler<JoinRequest>
    {
        private readonly BomberBotContext _context;
        private readonly ResponseLocalizer _response;
        private readonly TelegramBotClient _telegram;
        private readonly IStringLocalizer<SharedResource> _localizer;
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
                _logger.LogInformation("Room:{RoomId} was created by user {Username}:{TelegramUserId})",
                                       message.Chat.Id, message.From.Username, message.From.Id);
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

                room.Users.Add(new RoomUser
                                   {
                                       Room = room,
                                       User = user
                                   });
                
                _logger.LogInformation("User {Username}:{UserId} has been created and joined to game in chat:{ChatId}", message.From.Username,
                                       message.From.Id, room.TelegramChatId);

                await _context.SaveChangesAsync(cancellationToken);
                return;
            }

            if (room.Users.All(x => x.User.TelegramUserId != message.From.Id))
            {
                var roomUser = new RoomUser
                                   {
                                       Room = room,
                                       User = user
                                   };
                
                _context.RoomUsers.Add(roomUser);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("User {Username}:{UserId} joined to play in chat:{ChatId}", message.From.Username, message.From.Id, message.Chat.Id);

                await _response.SendAsync(message.Chat.Id, "UserJoined", user.Username);
            }
            else
            {
                _logger.LogWarning("User {Username}:{UserId} has already been joined to play in chat:{ChatId}", message.From.Username, message.From.Id, message.Chat.Id);
                
                await _response.SendAsync(message.Chat.Id, "UserAlreadyJoined", user.Username);
            }
        }
    }
}
