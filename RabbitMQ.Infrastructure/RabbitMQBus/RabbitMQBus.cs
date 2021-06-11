using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Core.Bus;
using RabbitMQ.Core.Commands;
using RabbitMQ.Core.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQ.Infrastructure.RabbitMQBus
{
    public sealed class RabbitMQBus : IEventBus
    {
        private readonly IMediator _mediator;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQBus(IMediator mediator, IServiceScopeFactory serviceScopeFactory)
        {
            this._mediator = mediator;
            this._serviceScopeFactory = serviceScopeFactory;
            this._handlers = new Dictionary<string, List<Type>>();
            this._eventTypes = new List<Type>();
        }

        public Task SendCommand<T>(T command) where T : Command
        {
            return this._mediator.Send(command);
        }

        public void Publish<T>(T @event) where T : Event
        {
            var factory = new ConnectionFactory
            {
                // TODO: make it configurable through app.config file
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var eventName = @event.GetType().Name; // It'll be an event name, since Event abstract class is very generic
                channel.QueueDeclare(eventName, false, false, false, null);

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("", eventName, null, body);
            }
        }

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            // Generic part
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!this._eventTypes.Contains(typeof(T)))
            {
                this._eventTypes.Add(typeof(T));
            }

            if (!this._handlers.ContainsKey(eventName))
            {
                this._handlers.Add(eventName, new List<Type>());
            }

            if (this._handlers[eventName].Any(h => h.GetType() == handlerType))
            {
                throw new ArgumentException($"Event handler {handlerType} is already exist for '{eventName}' event type.");
            }

            this._handlers[eventName].Add(handlerType);

            // RabbitMQ Consumer part
            StartBasicConsume<T>();
        }

        private void StartBasicConsume<T>() where T : Event
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                DispatchConsumersAsync = true
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var eventName = typeof(T).Name;
            channel.QueueDeclare(eventName, false, false, false, null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.Received += Consumer_Received;

            channel.BasicConsume(eventName, true, consumer);
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            try
            {
                await ProcessEvent(eventName, message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (this._handlers.ContainsKey(eventName))
            {
                using (var scope = this._serviceScopeFactory.CreateScope())
                {
                    var subscriptions = this._handlers[eventName];

                    foreach (var subscription in subscriptions)
                    {
                        var handler = scope.ServiceProvider.GetService(subscription);
                        if (handler == null) continue;

                        var eventType = this._eventTypes.FirstOrDefault(h => h.Name == eventName);
                        var @event = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IEventHandler<>).MakeGenericType(eventType);

                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { @event });
                    }
                }
            }
        }

        #region RPC

        public Tres RPCCall<T, TReq, Tres>(T request)
            where T : Command // request type
            where TReq : Event // response type
            where Tres : Event // reply type
        {
            var rpcClient = new RPCClient<TReq>();

            var response = rpcClient.Call(request);

            rpcClient.Close();

            var responseType = typeof(Tres);
            var concreteResponse = (Tres)JsonConvert.DeserializeObject(response, responseType);

            return concreteResponse;
        }


        public void RPCSubscribe<T, TH, TR>()
            where T : Event
            where TH : IRPCEventHandler
            where TR : Event // response event
        {
            {
                // Generic part
                var eventName = typeof(T).Name;
                var handlerType = typeof(TH);

                if (!this._eventTypes.Contains(typeof(T)))
                {
                    this._eventTypes.Add(typeof(T));
                }

                if (!this._handlers.ContainsKey(eventName))
                {
                    this._handlers.Add(eventName, new List<Type>());
                }

                if (this._handlers[eventName].Any(h => h.GetType() == handlerType))
                {
                    throw new ArgumentException($"Event handler {handlerType} is already exist for '{eventName}' event type.");
                }

                this._handlers[eventName].Add(handlerType);

                // RabbitMQ Consumer part
                StartRPCBasicConsume<T, TR>();
            }
        }

        private void StartRPCBasicConsume<T, TR>()
            where T : Event
            where TR : Event
        {
            // TODO: Make it configurable
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                DispatchConsumersAsync = true,
            };

            var requestTypeName = typeof(T).Name;

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: requestTypeName, durable: false, exclusive: false, autoDelete: false, arguments: null);
            channel.BasicQos(0, 1, false); // prefetch-count = 1, in case if we would have several comsumers

            var consumer = new AsyncEventingBasicConsumer(channel);
            
            consumer.Received += async (sender, e) =>
            {
                var props = e.BasicProperties;
                var replyProps = channel.CreateBasicProperties();
                replyProps.CorrelationId = props.CorrelationId;

                var reply = await RPCConsumer_Received<TR>(sender, e);
                var responseBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(reply));

                channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
                     basicProperties: replyProps, body: responseBytes);
            };

            channel.BasicConsume(queue: requestTypeName, autoAck: true, consumer: consumer);
        }

        private async Task<T> RPCConsumer_Received<T>(object sender, BasicDeliverEventArgs e) where T : Event
        {
            var eventName = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            try
            {
                return await RPCProcessEvent<T>(eventName, message).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<T> RPCProcessEvent<T>(string eventName, string message) where T : Event
        {
            if (this._handlers.ContainsKey(eventName))
            {
                T reply = new object() as T;

                using (var scope = this._serviceScopeFactory.CreateScope())
                {
                    var subscriptions = this._handlers[eventName];

                    foreach (var subscription in subscriptions)
                    {
                        var handler = scope.ServiceProvider.GetService(subscription);
                        if (handler == null) continue;

                        var eventType = this._eventTypes.FirstOrDefault(h => h.Name == eventName);
                        var @event = JsonConvert.DeserializeObject(message, eventType);
                        var concreteType = typeof(IRPCEventHandler<>).MakeGenericType(eventType);
                        
                        // To call the generic method
                        var method = concreteType.GetMethod("Handle", new Type[] { @event.GetType() });
                        method = method.MakeGenericMethod(typeof(T));

                        reply = await (Task<T>)method.Invoke(handler, new object[] { @event });
                    }
                }

                return reply;
            }

            return null;
        }
        #endregion
    }
}
