using System.Windows;
using System.Windows.Input;

namespace CAGD.Controls.MouseHandlers
{
    public class PointerEventArgs<T>
        where T : MouseEventArgs
    {
        private readonly T originalArgs;
        private readonly Point position;
        private readonly int timestamp;
        private readonly FrameworkElement sender;

        public PointerEventArgs(FrameworkElement sender, T originalArgs)
        {
            this.sender = sender;
            this.originalArgs = originalArgs;
            this.position = originalArgs.GetPosition(sender);
            this.timestamp = originalArgs.Timestamp;
        }

        public FrameworkElement Sender
        {
            get
            {
                return this.sender;
            }
        }

        public Size SenderSize
        {
            get
            {
                return new Size(this.sender.ActualWidth, this.sender.ActualHeight);
            }
        }

        public Point Position
        {
            get
            {
                return this.position;
            }
        }

        public int Timestamp
        {
            get
            {
                return this.timestamp;
            }
        }

        public T OriginalArgs
        {
            get
            {
                return this.originalArgs;
            }
        }
    }
}
