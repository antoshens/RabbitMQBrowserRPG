using RabbitMQ.Core.Events;

namespace BrowserTextRPG.Events
{
    public class AttackCreatedEvent : Event
    {
        public int AttackerId { get; private set; }
        public int OpponentId { get; private set; }
        public int Damage { get; private set; }

        public AttackCreatedEvent(int attacker, int opponent, int damage)
        {
            AttackerId = attacker;
            OpponentId = opponent;
            Damage = damage;
        }
    }
}
