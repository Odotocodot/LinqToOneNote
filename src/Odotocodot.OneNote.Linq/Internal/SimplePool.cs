using System.Collections.Concurrent;
using System.Threading;

namespace Odotocodot.OneNote.Linq.Internal
{
    internal class SimplePool<T>(int max) where T : new()
    {
        private readonly ConcurrentBag<T> pool = [];
        private int count = 0;

        public T Rent()
        {
            if (pool.TryTake(out var obj))
            {
                Interlocked.Decrement(ref count);
                return obj;
            }
            return new T();
        }

        public void Return(T obj)
        {
            if (Interlocked.Increment(ref count) <= max)
            {
                pool.Add(obj);
            }
            else
            {
                Interlocked.Decrement(ref count);
            }
        }
    }
}
