using RabbitMQ.Core.Events;
using System.Threading.Tasks;

namespace RabbitMQ.Core.Bus
{
    public interface IRPCEventHandler<in TEvent> : IRPCEventHandler where TEvent : Event
    {
        Task<TH> Handle<TH>(TEvent @event) where TH : Event;
    }

    public interface IRPCEventHandler
    {

    }
}
