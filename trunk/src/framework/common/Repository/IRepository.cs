using System.Collections.Generic;

namespace DL.Framework.Common
{
    public interface IRepository<T> where T : class, IItemKey
    {
        void Insert(T entity);
        void Insert(IEnumerable<T> entities);
        void Update(T entity);

        IEnumerable<T> GetAll();
    }
}
