using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace DL.Framework.Messaging
{
    public class RabbitMqConnection : IDisposable
    {
        private readonly string _hostName;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _virtualHost;

        private IConnection _connection;

        public RabbitMqConnection(string hostName, string userName, string password, string virtualHost)
        {
            _hostName = hostName;
            _userName = userName;
            _password = password;
            _virtualHost = virtualHost;
        }

        private void EstablishConnection()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = _hostName,
                UserName = _userName,
                Password = _password,
                VirtualHost = _virtualHost
            };

            _connection = connectionFactory.CreateConnection();
        }

        public IModel CreateChannel()
        {
            if (_connection == null || (_connection != null && !_connection.IsOpen))
                EstablishConnection();

            return _connection.CreateModel();
        }

        public void Close()
        {
            lock (this)
            {
                if (_connection != null && _connection.IsOpen)
                    _connection.Close();
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
