using System;

namespace DL.Framework.Common
{
    public interface IMessagePublisher : IDisposable
    {
        void Send(string destination, IMessage message);
    }
}
