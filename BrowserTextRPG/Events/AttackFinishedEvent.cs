using RabbitMQ.Core.Events;

namespace BrowserTextRPG.Events
{
    public class AttackFinishedEvent : Event
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; }

        public AttackFinishedEvent(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }
    }
}
