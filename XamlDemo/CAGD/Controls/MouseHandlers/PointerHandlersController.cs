using CAGD.Utilities;
using CAGD.Controls.Common;
using System;
using System.Windows;
using System.Windows.Input;

namespace CAGD.Controls.MouseHandlers
{
    public class PointerHandlersController
    {
#if DEBUG
        private static readonly bool DebugPointerHandlers = false;
#endif
        private readonly NamedObjectsCollection<IPointerHandler> handlers;
        private IPointerHandler capturedHandler;
        private PointerEventArgs<MouseButtonEventArgs> lastCaptureArgs;
        private Point lastMovePosition;

        public PointerHandlersController()
        {
            this.handlers = new NamedObjectsCollection<IPointerHandler>();
            this.capturedHandler = null;
            this.lastCaptureArgs = null;
            this.HandleMoveWhenNoHandlerIsCaptured = false;
        }

        public bool HandleMoveWhenNoHandlerIsCaptured { get; set; }

        public NamedObjectsCollection<IPointerHandler> Handlers
        {
            get
            {
                return this.handlers;
            }
        }

        private IPointerHandler CapturedHandler
        {
            get
            {
                return this.capturedHandler;
            }
        }
        
        public bool TryHandleMouseDown(PointerEventArgs<MouseButtonEventArgs> e)
        {
#if DEBUG
            DebugEventHandler("TryHandleMouseDown before:", this.CapturedHandler, e);
#endif
            this.lastMovePosition = e.Position;
            this.TryHandleMouseUp(e);

            foreach (IPointerHandler handler in this.Handlers)
            {
                if (handler.IsEnabled && handler.TryHandleMouseDown(e))
                {
                    this.CaptureHandler(handler, e);
                    return true;
                }
            }

            return false;
        }

        public bool TryHandleMouseUp(PointerEventArgs<MouseButtonEventArgs> e)
        {
#if DEBUG
            DebugEventHandler("TryHandleMouseUp before:", this.CapturedHandler, e);
#endif

            if (this.CapturedHandler != null)
            {
                bool success = this.CapturedHandler.TryHandleMouseUp(e);
                this.ReleaseHandler();

                return success;
            }

            return false;
        }

        public bool TryHandleMouseMove(PointerEventArgs<MouseEventArgs> e)
        {
            if (this.lastMovePosition.Equals(e.Position))
            {
#if DEBUG
                DebugEventHandler("Move with same position:", this.CapturedHandler, e);
#endif
                return false;
            }

            this.lastMovePosition = e.Position;

            if (this.CapturedHandler != null)
            {
#if DEBUG
                DebugEventHandler("TryHandleMouseMove before:", this.CapturedHandler, e);
#endif

                if(PointerHandlersController.IsValidDragMoveHandler(this.CapturedHandler) ||
                    this.TryCaptureNextValidHandler(PointerHandlersController.IsValidDragMoveHandler))
                {
#if DEBUG
                    DebugEventHandler("TryHandleMouseMove captured:", this.CapturedHandler, e);
#endif
                    return this.CapturedHandler.TryHandleMouseMove(e);
                }
            }
            
            if (this.HandleMoveWhenNoHandlerIsCaptured && this.CapturedHandler == null)
            {
                foreach (IPointerHandler handler in this.Handlers)
                {
                    if (handler.IsEnabled && handler.TryHandleMouseMove(e))
                    {
#if DEBUG
                        DebugEventHandler("TryHandleMouseMove not captured:", handler, e);
#endif
                        return true;
                    }
                }
            }

            return false;
        }

        public bool TryHandleMouseWheel(PointerEventArgs<MouseWheelEventArgs> e)
        {
#if DEBUG
            DebugEventHandler("TryHandleMouseWheel before:", this.CapturedHandler, e);
#endif

            if (this.CapturedHandler != null && this.CapturedHandler.IsEnabled)
            {
                return this.CapturedHandler.TryHandleMouseWheel(e);
            }

            foreach (IPointerHandler handler in this.Handlers)
            {
                if (handler.IsEnabled && handler.TryHandleMouseWheel(e))
                {
                    return true;
                }
            }

            return false;
        }   

        public event EventHandler HandlerCaptured;
        public event EventHandler HandlerReleased;

        protected void OnHandlerCaptured()
        {
            if (this.HandlerCaptured != null)
            {
                this.HandlerCaptured(this, new EventArgs());
            }
        }

        protected void OnHandlerReleased()
        {
            if (this.HandlerReleased != null)
            {
                this.HandlerReleased(this, new EventArgs());
            }
        }

        private void CaptureHandler(IPointerHandler handler, PointerEventArgs<MouseButtonEventArgs> e)
        {
            Guard.ThrowExceptionIfNull(handler, "handler");
            this.capturedHandler = handler;
            this.lastCaptureArgs = e;
#if DEBUG
            DebugEventHandler("CaptureHandler:", this.CapturedHandler, e);
#endif
            this.OnHandlerCaptured();
        }

        private void ReleaseHandler()
        {
#if DEBUG
            DebugLine("ReleaseHandler: <{0}>", this.CapturedHandler);
#endif
            this.capturedHandler = null;
            this.OnHandlerReleased();
        }

        private bool TryCaptureNextValidHandler(Func<IPointerHandler, bool> isValidHandler)
        {
            bool shouldTryCapture = false;
            IPointerHandler initialCapture = this.CapturedHandler;
            this.ReleaseHandler();

            foreach (IPointerHandler handler in this.handlers)
            {
                shouldTryCapture = shouldTryCapture ? shouldTryCapture : handler == initialCapture;

                if (shouldTryCapture && isValidHandler(handler) && handler.TryHandleMouseDown(this.lastCaptureArgs))
                {
                    this.CaptureHandler(handler, this.lastCaptureArgs);

                    return true;
                }
            }

            return false;
        }

        private static bool IsValidDragMoveHandler(IPointerHandler handler)
        {
            return handler.IsEnabled && handler.HandlesDragMove;
        }
#if DEBUG
        private static void DebugLine(string text, params object[] parameters)
        {
            if (!DebugPointerHandlers) return;

            System.Diagnostics.Debug.WriteLine(text, parameters);
        }

        private static void DebugEventHandler<T>(string text, IPointerHandler handler, PointerEventArgs<T> e)
            where T : MouseEventArgs
        {
            if (!DebugPointerHandlers) return;

            Func<object, string> getShortName = (obj) => 
                {
                    if(obj == null)
                    {
                        return string.Empty;
                    }

                    string str = obj.ToString();
                    int index = str.LastIndexOf('.');

                    if(index < 0 || index == str.Length - 1)
                    {
                        return str;
                    }

                    return str.Substring(index + 1);
                };

            System.Diagnostics.Debug.WriteLine("{0}: <{1}> E{2} P<{3}> T{4} <{5}> S{6} <{7}> ", 
                text, 
                getShortName(handler), 
                e.GetHashCode(), 
                e.Position,  
                e.Timestamp,
                getShortName(e.Sender),
                e.Sender.GetHashCode(),
                e.SenderSize);
        }
#endif
    }
}
