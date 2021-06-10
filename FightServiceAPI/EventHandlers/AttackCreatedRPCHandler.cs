using FightServiceAPI.Model;
using FightServiceAPI.Services;
using RabbitMQ.Core.Bus;
using RabbitMQ.Core.Events;
using System;
using System.Threading.Tasks;

namespace FightServiceAPI.Events
{
    public class AttackCreatedRPCHandler : IRPCEventHandler<AttackCreatedEvent>
    {
        private readonly IAttackService _attackService;

        public AttackCreatedRPCHandler(IAttackService attackService)
        {
            this._attackService = attackService;
        }

        public async Task<TH> Handle<TH>(AttackCreatedEvent @event) where TH : Event
        {
            try
            {
                await this._attackService.AddAttackLog(new AttackLog
                {
                    AttackerId = @event.AttackerId,
                    OpponentId = @event.OpponentId,
                    Damage = @event.Damage
                });

                return new AttackFinishedEvent(true, "Attack has been finished") as TH;
            }
            catch (Exception ex)
            {
                return new AttackFinishedEvent(false, $"Attack has been failed due to an error: {ex.Message}") as TH;
            }

        }
    }
}
