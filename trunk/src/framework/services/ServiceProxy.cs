using System;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace DL.Framework.Services
{
    public class ServiceProxy<T> : IServiceProxy where T : class
    {
        private T _instance;
        private ChannelFactory<T> _channelFactory;

        private readonly string _endPoint;
        private readonly object _lock = new object();

        public ServiceProxy(string endPoint)
        {
            _endPoint = endPoint;
        }

        public T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_channelFactory == null)
                        _channelFactory = CreateSubscription();

                    if (_instance == null || _channelFactory.State == CommunicationState.Faulted)
                        CreateChannel();

                    return _instance;
                }
            }
        }

        public string EndPoint
        {
            get { return _endPoint; }
        }

        protected virtual ChannelFactory<T> CreateChannelFactory()
        {
            return new ChannelFactory<T>(_endPoint);
        }

        private ChannelFactory<T> CreateSubscription()
        {
            Close();

            var newChannelFactory = CreateChannelFactory();

            return newChannelFactory;
        }

        private void CreateChannel()
        {
            _instance = _channelFactory.CreateChannel();
        }

        public void Abort()
        {
            if (_channelFactory != null && (_channelFactory.State == CommunicationState.Opened || _channelFactory.State == CommunicationState.Faulted))
            {
                try
                {
                    _channelFactory.Abort();
                }
                catch { }
            }

            _channelFactory = null;
            _instance = null;
        }

        public void Close()
        {
            if (_channelFactory != null && (_channelFactory.State == CommunicationState.Opened || _channelFactory.State == CommunicationState.Faulted))
            {
                try
                {
                    _channelFactory.Close();
                }
                catch { }
            }

            _channelFactory = null;
            _instance = null;
        }

        public void Dispose()
        {
            Close();
        }
    }
}
