using System;
using System.Collections.Generic;

namespace CAGD.Controls.Common
{
    public class ObjectPool<T>
    {
        private readonly Stack<T> pool;
        private readonly Action<T> hideElement;
        private readonly Action<T> showElement;

        public ObjectPool(Action<T> showElement, Action<T> hideElement)
        {
            this.pool = new Stack<T>();
            this.showElement = showElement;
            this.hideElement = hideElement;
        }

        public void PushElementToPool(T element)
        {
            this.hideElement(element);
            this.pool.Push(element);
        }

        public T PopElementFromPool()
        {
            T element = this.pool.Pop();
            this.showElement(element);

            return element;
        }

        public bool TryPopElementFromPool(out T element)
        {
            if (this.pool.Count > 0)
            {
                element = this.PopElementFromPool();
                return true;
            }

            element = default(T);
            return false;
        }
    }
}
