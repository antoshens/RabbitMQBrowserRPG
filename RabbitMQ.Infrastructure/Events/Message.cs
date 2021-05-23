using MediatR;

namespace RabbitMQ.Core.Events
{
    public abstract class Message : IRequest<bool>
    {
        public string MessageType { get; protected set; }

        protected Message()
        {
            MessageType = this.GetType().Name;
        }
    }
}
