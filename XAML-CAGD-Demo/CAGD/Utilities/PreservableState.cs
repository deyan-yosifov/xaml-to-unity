using System;
using System.Collections.Generic;

namespace CAGD.Utilities
{
    public class PreservableState<T>
        where T : ICloneable<T>, new()
    {
        private readonly Stack<T> states;

        public PreservableState()
        {
            this.states = new Stack<T>();
            this.states.Push(new T());
        }

        public T Value
        {
            get
            {
                return this.states.Peek();
            }
        }

        public IDisposable Preserve()
        {
            this.states.Push(this.Value.Clone());
            this.OnStatePreservedOverride();

            return new DisposableAction(this.Restore);
        }

        public void Restore()
        {
            if (this.states.Count > 1)
            {
                this.states.Pop();
                this.OnStateRestoredOverride();
            }
            else
            {
                throw new InvalidOperationException("Cannot call Restore() method more times than Preserve() method!");
            }
        }

        protected virtual void OnStatePreservedOverride()
        {
        }

        protected virtual void OnStateRestoredOverride()
        {
        }
    }
}
