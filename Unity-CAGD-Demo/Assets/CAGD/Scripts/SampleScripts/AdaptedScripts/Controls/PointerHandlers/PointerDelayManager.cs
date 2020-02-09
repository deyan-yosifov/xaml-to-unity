using System;

namespace CAGD.Controls.PointerHandlers
{
    public class PointerDelayManager
    {
        private readonly Func<float, bool> shouldHandleTimeStamp;
        private float previousMoveTimestamp;

        public PointerDelayManager(bool handlesIntervalsBiggerThanTimeInterval)
        {
            this.previousMoveTimestamp = 0;

            if (handlesIntervalsBiggerThanTimeInterval)
            {
                this.shouldHandleTimeStamp = this.IsTimeIntervalBigEnough;
            }
            else
            {
                this.shouldHandleTimeStamp = this.IsTimeIntervalShortEnough;
            }
        }

        public float TimeInterval
        {
            get;
            set;
        }

        public bool ShouldHandlePointer<T>(PointerEventArgs<T> e)
        {
            return this.shouldHandleTimeStamp(e.timeStamp);
        }

        private bool IsTimeIntervalBigEnough(float timestamp)
        {
            if (Math.Abs(timestamp - this.previousMoveTimestamp) > this.TimeInterval)
            {
                this.previousMoveTimestamp = timestamp;

                return true;
            }

            return false;
        }

        private bool IsTimeIntervalShortEnough(float timestamp)
        {
            bool shouldHandle = Math.Abs(timestamp - this.previousMoveTimestamp) < this.TimeInterval;
            this.previousMoveTimestamp = timestamp;

            return shouldHandle;
        }
    }
}
