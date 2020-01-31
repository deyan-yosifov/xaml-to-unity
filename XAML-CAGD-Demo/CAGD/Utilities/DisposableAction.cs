using System;

namespace CAGD.Utilities
{
    public sealed class DisposableAction : IDisposable
    {
        private bool isDisposed;
        private readonly Action action;

        public DisposableAction(Action action)
        {
            this.isDisposed = false;
            this.action = action;
        }

        public void Dispose()
        {
            Guard.ThrowExceptionIfTrue(this.isDisposed, "isDisposed");

            this.isDisposed = true;
            this.action();
        }
    }
}
