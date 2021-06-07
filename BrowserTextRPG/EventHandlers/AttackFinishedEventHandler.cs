using BrowserTextRPG.Events;
using Microsoft.Extensions.Logging;
using RabbitMQ.Core.Bus;
using System.Threading.Tasks;

namespace BrowserTextRPG.EventHandlers
{
    public class AttackFinishedEventHandler : IEventHandler<AttackFinishedEvent>
    {
        private readonly ILogger<AttackFinishedEventHandler> _logger;

        public AttackFinishedEventHandler(ILogger<AttackFinishedEventHandler> logger)
        {
            this._logger = logger;
        }

        public Task Handle(AttackFinishedEvent @event)
        {
            this._logger.LogInformation($"Handle 'AttackFinishedEvent' queue with success - {@event.Success}.");

            return Task.FromResult(true);
        }
    }
}
