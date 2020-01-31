using System;

namespace CAGD.Utilities
{
    public class BeginEndUpdateCounter
    {
        private int count;
        private bool shouldDoDelayedUpdate;
        private readonly Action updateAction;

        public BeginEndUpdateCounter(Action updateAction)
        {
            Guard.ThrowExceptionIfNull(updateAction, "updateAction");

            this.updateAction = updateAction;
            this.count = 0;
            this.shouldDoDelayedUpdate = false;
        }

        public IDisposable BeginUpdateGroup()
        {
            this.count++;

            return new DisposableAction(this.EndUpdateGroup);
        }

        public void Update()
        {
            this.shouldDoDelayedUpdate = this.count > 0;

            if (!this.shouldDoDelayedUpdate)
            {
                this.updateAction();
            }
        }

        private void EndUpdateGroup()
        {
            this.count--;
            Guard.ThrowExceptionIfLessThan(this.count, 0, "count");

            if (this.shouldDoDelayedUpdate)
            {
                this.Update();
            }
        }
    }
}
