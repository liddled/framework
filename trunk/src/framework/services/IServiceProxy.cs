using System;

namespace DL.Framework.Services
{
    public interface IServiceProxy : IDisposable
    {
        void Close();
    }
}
