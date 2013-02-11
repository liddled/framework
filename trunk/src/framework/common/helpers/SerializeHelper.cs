using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DL.Framework.Common
{
    public static class SerializeHelper
    {
        /// <summary>
        /// Converts object to byte array
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public static byte[] ObjectToByteArray<T>(T o)
        {
            if (o == null)
                return null;

            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, o);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converts byte array to object
        /// </summary>
        /// <param name="arrBytes"></param>
        /// <returns></returns>
        public static T ByteArrayToObject<T>(byte[] arrBytes)
        {
            using (var ms = new MemoryStream())
            {
                var bf = new BinaryFormatter();
                ms.Write(arrBytes, 0, arrBytes.Length);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)bf.Deserialize(ms);
            }
        }
    }
}
