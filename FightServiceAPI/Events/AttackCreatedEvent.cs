using RabbitMQ.Core.Events;

namespace FightServiceAPI.Events
{
    public class AttackCreatedEvent : Event
    {
        public int AttackerId { get; private set; }
        public int OpponentId { get; private set; }
        public int Damage { get; private set; }

        public AttackCreatedEvent(int attackerId, int opponentId, int damage)
        {
            AttackerId = attackerId;
            OpponentId = opponentId;
            Damage = damage;
        }
    }
}
