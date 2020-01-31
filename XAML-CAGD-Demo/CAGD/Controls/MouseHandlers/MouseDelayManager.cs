using System;
using System.Windows.Input;

namespace CAGD.Controls.MouseHandlers
{
    public class MouseDelayManager
    {
        private readonly Func<int, bool> shouldHandleTimeStamp;
        private int previousMoveTimestamp;

        public MouseDelayManager(bool handlesIntervalsBiggerThanTimeInterval)
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

        public int TimeInterval
        {
            get;
            set;
        }

        public bool ShouldHandleMouse<T>(PointerEventArgs<T> e)
            where T : MouseEventArgs
        {
            return this.shouldHandleTimeStamp(e.Timestamp);
        }

        private bool IsTimeIntervalBigEnough(int timestamp)
        {
            if (Math.Abs(timestamp - this.previousMoveTimestamp) > this.TimeInterval)
            {
                this.previousMoveTimestamp = timestamp;

                return true;
            }

            return false;
        }

        private bool IsTimeIntervalShortEnough(int timestamp)
        {
            bool shouldHandle = Math.Abs(timestamp - this.previousMoveTimestamp) < this.TimeInterval;
            this.previousMoveTimestamp = timestamp;

            return shouldHandle;
        }
    }
}
