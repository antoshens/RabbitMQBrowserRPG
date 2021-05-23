using RabbitMQ.Core.Commands;

namespace BrowserTextRPG.Commands
{
    public abstract class AttackCommand : Command
    {
        public int AttackerId { get; protected set; }
        public int OpponentId { get; protected set; }
        public int Damage { get; protected set; }
    }
}
