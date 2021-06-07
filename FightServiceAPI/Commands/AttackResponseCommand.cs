using RabbitMQ.Core.Commands;

namespace FightServiceAPI.Commands
{
    public class AttackResponseCommand : Command
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; }

        public AttackResponseCommand(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }
    }
}
