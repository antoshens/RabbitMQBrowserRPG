using BrowserTextRPG.Commands;
using BrowserTextRPG.Events;
using MediatR;
using RabbitMQ.Core.Bus;
using System.Threading;
using System.Threading.Tasks;

namespace BrowserTextRPG.CommandHandlers
{
    public class AttackCommandHandler : IRequestHandler<WeaponAttackCommand, bool>
    {
        private readonly IEventBus _bus;

        public AttackCommandHandler(IEventBus bus)
        {
            this._bus = bus;
        }

        public Task<bool> Handle(WeaponAttackCommand request, CancellationToken cancellationToken)
        {
            // Publish event to RabbitMQ
            this._bus.Publish(new AttackCreatedEvent(request.AttackerId, request.OpponentId, request.Damage));

            return Task.FromResult(true);
        }
    }
}
