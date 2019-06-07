using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace BombinoBomberBot.Handlers
{
    public class GenericUpdateHandler : AsyncRequestHandler<GenericUpdateRequest>
    {
        private readonly IMediator _mediator;

        public GenericUpdateHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected override async Task Handle(GenericUpdateRequest request, CancellationToken cancellationToken)
        {
            // TODO: support full command name with bot name
            switch (request.Message.Text)
            {
                case string m when m.StartsWith("/help"):
                    await _mediator.Send(new HelpRequest(request.Message), cancellationToken);
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
    }
}