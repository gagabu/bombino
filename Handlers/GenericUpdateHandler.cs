using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BombinoBomberBot.Handlers
{
    public class GenericUpdateHandler : AsyncRequestHandler<GenericUpdateRequest>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<GenericUpdateHandler> _logger;

        public GenericUpdateHandler(IMediator mediator, ILogger<GenericUpdateHandler> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        protected override async Task Handle(GenericUpdateRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // TODO: support full command name with bot name
                switch (request.Message.Text)
                {
                    case string m when m.StartsWith("/help"):
                        await _mediator.Send(new HelpRequest(request.Message), cancellationToken);
                        break;
                    case string m when m.StartsWith("/stats"):
                        await _mediator.Send(new StatsRequest(request.Message), cancellationToken);
                        break;
                    case string m when m.StartsWith("/join"):
                        await _mediator.Send(new JoinRequest(request.Message), cancellationToken);
                        break;
                    case string m when m.StartsWith("/leave"):
                        await _mediator.Send(new LeaveRequest(request.Message), cancellationToken);
                        break;
                    case string m when m.StartsWith("/troll"):
                        await _mediator.Send(new TrollRequest(request.Message), cancellationToken);
                        break;
                    case string m when m.StartsWith("/info"):
                        await _mediator.Send(new InfoRequest(request.Message), cancellationToken);
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Something went wrong in main handler");
            }
        }
    }
}