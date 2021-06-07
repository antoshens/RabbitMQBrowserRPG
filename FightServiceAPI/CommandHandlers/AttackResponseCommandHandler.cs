using FightServiceAPI.Commands;
using FightServiceAPI.Events;
using MediatR;
using RabbitMQ.Core.Bus;
using System.Threading;
using System.Threading.Tasks;

namespace FightServiceAPI.CommandHandlers
{
    public class AttackResponseCommandHandler : IRequestHandler<AttackResponseCommand, bool>
    {
        private readonly IEventBus _bus;

        public AttackResponseCommandHandler(IEventBus bus)
        {
            this._bus = bus;
        }
        public Task<bool> Handle(AttackResponseCommand request, CancellationToken cancellationToken)
        {
            // Publish event to RabbitMQ
            this._bus.Publish(new AttackFinishedEvent(request.Success, request.Message));

            return Task.FromResult(true);
        }
    }
}
