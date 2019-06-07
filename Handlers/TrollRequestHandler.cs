using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using BombinoBomberBot.Helpers;
using BombinoBomberBot.Model;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace BombinoBomberBot.Handlers
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class TrollRequestHandler : AsyncRequestHandler<TrollRequest>
    {
        private readonly BomberBotContext _context;
        private readonly ResponseLocalizer _response;
        private readonly TelegramBotClient _telegram;
        private readonly ILogger<IRequest> _logger;

        public TrollRequestHandler(BomberBotContext context, ResponseLocalizer response, TelegramBotClient telegram, ILogger<IRequest> logger)
        {
            _context = context;
            _response = response;
            _telegram = telegram;
            _logger = logger;
        }

        protected override async Task Handle(TrollRequest request, CancellationToken cancellationToken)
        {
            var message = request.Message;
            var room = await _context.Rooms.Include(x => x.Users).FirstOrDefaultAsync(x => x.TelegramChatId == message.Chat.Id, cancellationToken);

            if (room == null)
            {
                _logger.LogWarning("User {User} tries to play game in unknown chat:{ChatId}", message.From, message.Chat.Id);
                return;
            }

            var user = await _context.RoomUsers
                            .FirstOrDefaultAsync(x => x.User.TelegramUserId == message.From.Id && x.Room.TelegramChatId == message.Chat.Id,
                                                 cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User {User} tries to play game in chat:{ChatId}, but it doesn't play game", message.From, message.Chat.Id);
                await _response.SendAsync(message.Chat.Id, "UserCantTroll", message.From.Mention());
            }
            else
            {
                var rnd = new Random();
                var winnerIndex = rnd.Next(0, room.Users.Count);
                var winner = await _context.Users.FirstOrDefaultAsync(x => x.Id == room.Users[winnerIndex].UserId, cancellationToken);
                _logger.LogWarning("User {User} wins game in chat:{ChatId}", winner.Username, winner.TelegramUserId, message.Chat.Id);

                var userStats = await _context.UserStats.FirstOrDefaultAsync(x => x.RoomId == room.Id && x.UserId == winner.Id, cancellationToken);

                if (userStats == null)
                {
                    userStats = new UserStats { Room = room, User = winner };
                    _context.UserStats.Attach(userStats);
                }

                room.Trolls++;
                userStats.Wins++;
                
                await _context.SaveChangesAsync(cancellationToken);
                var member = await _telegram.GetChatMemberAsync(message.Chat.Id, winner.TelegramUserId, cancellationToken);
                await _response.SendAsync(message.Chat.Id, "UserWinGame", member.User.Mention());
            }
        }
    }
}
