using System;
using Common.Logging;
using DL.Framework.Common;
using RabbitMQ.Client;

namespace DL.Framework.Messaging
{
    public class RabbitMqSubscriber : IMessageSubscriber
    {
        private readonly static ILog Log = LogManager.GetCurrentClassLogger();

        private readonly RabbitMqConnection _connection;
        private readonly RabbitMqConsumer _consumer;
        private IModel _channel;
        private readonly string _exchange;

        public string RoutingKey { get; set; }
        public bool Durable { get; set; }
        public string Queue { get; set; }

        public RabbitMqSubscriber(RabbitMqConnection connection, RabbitMqConsumer consumer, string exchange, string routingKey)
        {
            _connection = connection;
            _consumer = consumer;
            _exchange = exchange;

            RoutingKey = routingKey;
            Durable = true;
        }

        internal void CreateChannel()
        {
            if (_channel != null && _channel.IsOpen)
                return;

            Log.DebugFormat("Establishing rabbitmq connection");
            _channel = _connection.CreateChannel();
            Log.DebugFormat("Exchange declare - exchange {0}", _exchange);
            _channel.ExchangeDeclare(_exchange, ExchangeType.Topic, Durable);
        }

        public void Subscribe()
        {
            try
            {
                CreateChannel();

                var qDeclare = !String.IsNullOrEmpty(Queue) ? _channel.QueueDeclare(Queue, Durable, false, false, null) : _channel.QueueDeclare();

                Log.DebugFormat("Subscribing - exchange: {0}, queue: {1}, routing key: {2}", _exchange, qDeclare.QueueName, RoutingKey);

                _channel.QueueBind(qDeclare.QueueName, _exchange, RoutingKey);
                _channel.BasicConsume(qDeclare.QueueName, true, _consumer);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw ex;
            }
        }

        public void Unsubscribe()
        {
            _connection.Close();
        }
    }
}
