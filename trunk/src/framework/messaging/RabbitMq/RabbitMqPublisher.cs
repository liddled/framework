using System;
using System.Collections.Generic;
using Common.Logging;
using DL.Framework.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;

namespace DL.Framework.Messaging
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        private readonly static ILog Log = LogManager.GetCurrentClassLogger();

        private readonly RabbitMqConnection _connection;
        private IModel _channel;
        private readonly string _exchange;

        public bool Durable { get; set; }

        public RabbitMqPublisher(RabbitMqConnection connection, string exchange)
        {
            _connection = connection;
            _exchange = exchange;
            Durable = true;
        }

        internal void EstablishChannel()
        {
            if (_channel != null && !_channel.IsOpen)
                return;

            Log.DebugFormat("Establishing rabbitmq connection");
            _channel = _connection.CreateChannel();
            Log.DebugFormat("Exchange declare - exchange {0}", _exchange);
            _channel.ExchangeDeclare(_exchange, ExchangeType.Topic, Durable);
        }

        public void Send(string destination, IMessage message)
        {
            try
            {
                EstablishChannel();

                Log.DebugFormat("Send message - exchange {0}, routing key {1}", _exchange, destination);

                var props = _channel.CreateBasicProperties();
                props.Headers = new Dictionary<string, string>();
                props.SetPersistent(true);

                _channel.TxSelect();
                _channel.BasicPublish(_exchange, destination, props, SerializeHelper.ObjectToByteArray(message));
                _channel.TxCommit();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        public void Dispose()
        {
            if (_channel != null)
                _channel.Close();

            if (_connection != null)
                _connection.Close();
        }
    }
}
