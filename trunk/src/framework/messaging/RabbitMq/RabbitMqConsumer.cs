using System;
using Common.Logging;
using DL.Framework.Common;
using RabbitMQ.Client;

namespace DL.Framework.Messaging
{
    public class RabbitMqConsumer : DefaultBasicConsumer
    {
        private readonly static ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IMessageProcessor _messageProcessor;

        public RabbitMqConsumer(IMessageProcessor messageProcessor)
        {
            _messageProcessor = messageProcessor;
        }

        public override void HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, string exchange, string routingKey, IBasicProperties properties, byte[] body)
        {
            try
            {
                Log.DebugFormat("Received message tag {0} on exchange {1}, routingKey {2}", deliveryTag, exchange, routingKey);

                var message = SerializeHelper.ByteArrayToObject<IMessage>(body);
                _messageProcessor.Process(message);
            }
            catch (Exception ex)
            {
                Log.ErrorFormat("Unhandled exception when receiving message tag {0} on exchange {1}, routingKey {2}", ex, deliveryTag, exchange, routingKey);
            }
        }
    }
}
