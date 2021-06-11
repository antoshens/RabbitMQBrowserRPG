using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Core.Commands;
using RabbitMQ.Core.Events;
using RabbitMQ.Core.RPC;
using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;

public class RPCClient<TH> : IRPCClient where TH : Event
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _replyQueueName;
    private readonly EventingBasicConsumer _consumer;
    private readonly BlockingCollection<string> _respQueue = new BlockingCollection<string>();
    private readonly IBasicProperties _props;
    private readonly ManualResetEvent _signal;

    public RPCClient()
    {
        // TODO: Make it configurable
        var factory = new ConnectionFactory() { HostName = "localhost" };

        this._connection = factory.CreateConnection();
        this._channel = this._connection.CreateModel();

        this._replyQueueName = this._channel.QueueDeclare().QueueName;
        this._consumer = new EventingBasicConsumer(this._channel);

        this._props = this._channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        this._props.CorrelationId = correlationId;
        this._props.ReplyTo = this._replyQueueName;

        // Receive reply from the consumer microservice
        this._consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);

            if (ea.BasicProperties.CorrelationId == correlationId)
            {
                this._respQueue.Add(response);
            }

            this._signal.Set();
        };

        this._signal = new ManualResetEvent(false);
    }

    public string Call<T>(T request) where T : Command
    {
        var message = JsonConvert.SerializeObject(request);
        var messageBytes = Encoding.UTF8.GetBytes(message);

        this._channel.BasicPublish(
            exchange: "",
            routingKey: typeof(TH).Name,
            basicProperties: _props,
            body: messageBytes);

        this._channel.BasicConsume(
            consumer: this._consumer,
            queue: this._replyQueueName,
            autoAck: true);

        // TODO: Make the timeout value configurable
        var timeout = !this._signal.WaitOne(TimeSpan.FromMilliseconds(1000 * 30));

        if (timeout) // if the timeout is reached
            throw new TimeoutException("Timeout error: no response from the server has been received.");

        return this._respQueue.Take();
    }

    public void Close()
    {
        this._connection.Close();
    }
}