using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
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
    public class StatsRequestHandler : AsyncRequestHandler<StatsRequest>
    {
        private readonly BomberBotContext _context;
        private readonly ResponseLocalizer _response;
        private readonly ILogger<IRequest> _logger;

        public StatsRequestHandler(BomberBotContext context, ResponseLocalizer response, ILogger<IRequest> logger)
        {
            _context = context;
            _response = response;
            _logger = logger;
        }

        protected override async Task Handle(StatsRequest request, CancellationToken cancellationToken)
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

                await _response.SendAsync(message.Chat.Id, "EmptyRoomStats");
            }
            else
            {
                var userStats = await _context.UserStats.Include(x => x.User).Where(x => x.RoomId == room.Id).OrderByDescending(x => x.Wins).ToListAsync(cancellationToken);

                if (!userStats.Any())
                {
                    await _response.SendAsync(message.Chat.Id, "EmptyRoomStats");
                    return;
                }

                var sb = new StringBuilder();

                for (int i = 0; i < userStats.Count; i++)
                {
                    var userStat = userStats[i];
                    var rate = userStat.Wins / (double)room.Trolls * 100;
                    sb.Append(i + 1).Append(". ").Append(userStat.User.Mention())
                      .Append(" - ").Append(userStat.Wins)
                      .Append(" (").Append(rate).Append("%)").AppendLine();
                }

                await _response.SendAsync(message.Chat.Id, "StatsInfo", sb.ToString(), userStats.First().User.Mention());
            }
        }
    }
}
