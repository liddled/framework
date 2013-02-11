using Spring.Context.Support;

namespace DL.Framework.Common
{
    public static class ObjectFactoryManager
    {
        public static T GetObject<T>()
        {
            return GetObject<T>(typeof(T).Name);
        }

        public static T GetObject<T>(string name)
        {
            return (T)ContextRegistry.GetContext().GetObject(name);
        }
    }
}
