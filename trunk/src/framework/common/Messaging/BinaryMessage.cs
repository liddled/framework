using System;

namespace DL.Framework.Common
{
    [Serializable]
    public class BinaryMessage : IMessage
    {
        public byte[] Data { get; private set; }

        public BinaryMessage(byte[] data)
        {
            Data = data;
        }

        public static IMessage CreateMessage(byte[] dataInBytes)
        {
            return new BinaryMessage(dataInBytes);
        }
    }
}
