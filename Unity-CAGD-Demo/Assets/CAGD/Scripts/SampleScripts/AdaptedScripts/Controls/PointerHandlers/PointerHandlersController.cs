using CAGD.Controls.Common;
using CAGD.Utilities;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CAGD.Controls.PointerHandlers
{
    public class PointerHandlersController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private Camera raycastCamera;
        private bool isPointerEntered = false;
        private NamedObjectsCollection<IPointerHandler> handlers;
        private IPointerHandler capturedHandler;
        private PointerEventArgs<PointerEventData> lastCaptureArgs;
        private Vector2 lastMovePosition;

        public bool HandleMoveWhenNoHandlerIsCaptured { get; set; }

        public event EventHandler HandlerCaptured;
        public event EventHandler HandlerReleased;

        private void Awake()
        {
            this.capturedHandler = null;
            this.lastCaptureArgs = null;
            this.HandleMoveWhenNoHandlerIsCaptured = false;
            this.handlers = new NamedObjectsCollection<IPointerHandler>();

            foreach (IPointerHandler handler in this.GetComponents<IPointerHandler>())
            {
                this.handlers.AddLast(handler);
            }
        }

        private void Update()
        {
            float wheel = Input.GetAxis("Mouse ScrollWheel");
            
            if (this.isPointerEntered && wheel != 0)
            {
                PointerEventArgs<MouseWheelEventData> e = new PointerEventArgs<MouseWheelEventData>(this.raycastCamera, new MouseWheelEventData() { delta = wheel });

                if (this.capturedHandler != null && this.capturedHandler.IsEnabled)
                {
                    this.capturedHandler.TryHandleMouseWheel(e);
                    return;
                }

                foreach (IPointerHandler handler in this.handlers)
                {
                    if (handler.IsEnabled && handler.TryHandleMouseWheel(e))
                    {
                        return;
                    }
                }
            }
        }

        protected void OnHandlerCaptured()
        {
            if (this.HandlerCaptured != null)
            {
                this.HandlerCaptured(this, EventArgs.Empty);
            }
        }

        protected void OnHandlerReleased()
        {
            if (this.HandlerReleased != null)
            {
                this.HandlerReleased(this, EventArgs.Empty);
            }
        }

        private void CaptureHandler(IPointerHandler handler, PointerEventArgs<PointerEventData> e)
        {
            Guard.ThrowExceptionIfNull(handler, "handler");
            this.capturedHandler = handler;
            this.lastCaptureArgs = e;

            this.OnHandlerCaptured();
        }

        private void ReleaseHandler()
        {
            this.capturedHandler = null;
            this.OnHandlerReleased();
        }

        private bool TryCaptureNextValidHandler(Func<IPointerHandler, bool> isValidHandler)
        {
            bool shouldTryCapture = false;
            IPointerHandler initialCapture = this.capturedHandler;
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

        void IPointerDownHandler.OnPointerDown(PointerEventData e)
        {
            this.lastMovePosition = e.position;
            IPointerUpHandler upHandler = this;
            upHandler.OnPointerUp(e);

            PointerEventArgs<PointerEventData> eventArgs = new PointerEventArgs<PointerEventData>(this.raycastCamera, e);
            foreach (IPointerHandler handler in this.handlers)
            {
                if (handler.IsEnabled && handler.TryHandleMouseDown(eventArgs))
                {
                    this.CaptureHandler(handler, eventArgs);
                    return;
                }
            }
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData e)
        {
            if (this.capturedHandler != null)
            {
                bool success = this.capturedHandler.TryHandleMouseUp(new PointerEventArgs<PointerEventData>(this.raycastCamera, e));
                this.ReleaseHandler();
            }
        }

        void IDragHandler.OnDrag(PointerEventData e)
        {
            if (this.lastMovePosition.Equals(e.position))
            {
                return;
            }

            this.lastMovePosition = e.position;
            PointerEventArgs<PointerEventData> eventArgs = new PointerEventArgs<PointerEventData>(this.raycastCamera, e);

            if (this.capturedHandler != null)
            {
                if (PointerHandlersController.IsValidDragMoveHandler(this.capturedHandler) ||
                    this.TryCaptureNextValidHandler(PointerHandlersController.IsValidDragMoveHandler))
                {
                    this.capturedHandler.TryHandleMouseMove(eventArgs);
                    return;
                }
            }

            if (this.HandleMoveWhenNoHandlerIsCaptured && this.capturedHandler == null)
            {
                foreach (IPointerHandler handler in this.handlers)
                {
                    if (handler.IsEnabled && handler.TryHandleMouseMove(eventArgs))
                    {
                        return;
                    }
                }
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            this.isPointerEntered = true;
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            this.isPointerEntered = false;
        }
    }
}
