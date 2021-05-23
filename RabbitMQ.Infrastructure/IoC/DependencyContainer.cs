using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Core.Bus;

namespace RabbitMQ.Infrastructure.IoC
{
    public class DependencyContainer
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // RabbitMQ.Core
            services.AddSingleton<IEventBus, RabbitMQBus.RabbitMQBus>(sp =>
            {
                var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new RabbitMQBus.RabbitMQBus(sp.GetService<IMediator>(), scopeFactory);
            });
        }
    }
}
