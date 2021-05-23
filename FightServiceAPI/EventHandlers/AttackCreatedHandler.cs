using FightServiceAPI.Events;
using FightServiceAPI.Model;
using FightServiceAPI.Services;
using RabbitMQ.Core.Bus;
using System.Threading.Tasks;

namespace FightServiceAPI.EventHandlers
{
    public class AttackCreatedHandler : IEventHandler<AttackCreatedEvent>
    {
        private readonly IAttackService _attackService;

        public AttackCreatedHandler(IAttackService attackService)
        {
            this._attackService = attackService;
        }

        public async Task Handle(AttackCreatedEvent @event)
        {
            await this._attackService.AddAttackLog(new AttackLog
            {
                AttackerId = @event.AttackerId,
                OpponentId = @event.OpponentId,
                Damage = @event.Damage
            });
        }
    }
}
