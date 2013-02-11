using System;

namespace DL.Framework.Common
{
    public class FilteredEventHandler
    {
        private Action<Action<CacheUpdate>> RemovalEvent { get; set; }
        public Action<CacheUpdate> Event { get; protected set; }

        public FilteredEventHandler(Action<Action<CacheUpdate>> removal)
        {
            RemovalEvent = removal;
        }

        public FilteredEventHandler(Action<CacheUpdate> evt, Action<Action<CacheUpdate>> removal)
            : this(removal)
        {
            Event = evt;
        }

        public void RemoveHandler()
        {
            RemovalEvent(Event);
        }
    }
}