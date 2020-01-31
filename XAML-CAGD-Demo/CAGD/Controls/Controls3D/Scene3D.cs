using CAGD.Controls.Controls3D.Cameras;
using CAGD.Controls.Controls3D.Iteractions;
using CAGD.Controls.MouseHandlers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CAGD.Controls.Controls3D
{
    public class Scene3D : FrameworkElement
    {
        private readonly Canvas viewport2D;
        private readonly SceneEditor editor;
        private readonly Viewport3D viewport3D;
        private readonly PointerHandlersController pointerHandlersController;
        private readonly Grid container;
        private bool isListeningToMouseEvents;

        public Scene3D()
        {
            this.viewport2D = new Canvas() { IsHitTestVisible = true, Background = new SolidColorBrush(Colors.Transparent), ClipToBounds = true };
            this.viewport3D = new Viewport3D() { IsHitTestVisible = false };
            this.editor = new SceneEditor(this);
            this.pointerHandlersController = new PointerHandlersController();
            this.pointerHandlersController.Handlers.AddLast(new IteractivePointsHandler(this.editor));
            this.pointerHandlersController.Handlers.AddLast(new OrbitControl(this.editor));
            this.isListeningToMouseEvents = false;
            
            this.container = new Grid();
            this.container.Children.Add(this.viewport3D);
            this.container.Children.Add(this.viewport2D);

            this.AddVisualChild(this.container);
        }

        public SceneEditor Editor
        {
            get
            {
                return this.editor;
            }
        }

        public PointerHandlersController PointerHandlersController
        {
            get
            {
                return this.pointerHandlersController;
            }
        }

        public OrbitControl OrbitControl
        {
            get
            {
                OrbitControl orbitControl;
                if (this.PointerHandlersController.Handlers.TryGetElementOfType<OrbitControl>(Scene3DMouseHandlerNames.OrbitControlHandler, out orbitControl))
                {
                    return orbitControl;
                }

                return null;
            }
        }

        public IteractivePointsHandler IteractivePointsHandler
        {
            get
            {
                IteractivePointsHandler iteractivePointsHandler;
                if (this.PointerHandlersController.Handlers.TryGetElementOfType<IteractivePointsHandler>(Scene3DMouseHandlerNames.IteractivePointsHandler, out iteractivePointsHandler))
                {
                    return iteractivePointsHandler;
                }

                return null;
            }
        }

        internal Canvas Viewport2D
        {
            get
            {
                return this.viewport2D;
            }
        }

        internal Viewport3D Viewport
        {
            get
            {
                return this.viewport3D;
            }
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        public void StartListeningToMouseEvents()
        {
            if (!this.isListeningToMouseEvents)
            {
                this.isListeningToMouseEvents = true;

                this.viewport2D.MouseDown += this.Viewport2D_MouseDown;
                this.viewport2D.MouseUp += this.Viewport2D_MouseUp;
                this.viewport2D.MouseMove += this.Viewport2D_MouseMove;
                this.viewport2D.MouseWheel += this.Viewport2D_MouseWheel;
                this.pointerHandlersController.HandlerCaptured += this.PointerHandlersController_HandlerCaptured;
                this.pointerHandlersController.HandlerReleased += this.PointerHandlersController_HandlerReleased;
            }
        }

        public void StopListeningToMouseEvents()
        {
            if (this.isListeningToMouseEvents)
            {
                this.viewport2D.MouseDown -= this.Viewport2D_MouseDown;
                this.viewport2D.MouseUp -= this.Viewport2D_MouseUp;
                this.viewport2D.MouseMove -= this.Viewport2D_MouseMove;
                this.viewport2D.MouseWheel -= this.Viewport2D_MouseWheel;
                this.pointerHandlersController.HandlerCaptured -= this.PointerHandlersController_HandlerCaptured;
                this.pointerHandlersController.HandlerReleased -= this.PointerHandlersController_HandlerReleased;

                this.isListeningToMouseEvents = false;
            }
        }

        private void PointerHandlersController_HandlerReleased(object sender, EventArgs e)
        {
            this.viewport2D.ReleaseMouseCapture();
        }

        private void PointerHandlersController_HandlerCaptured(object sender, EventArgs e)
        {
            this.viewport2D.CaptureMouse();
        }

        private void Viewport2D_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.pointerHandlersController.TryHandleMouseWheel(new PointerEventArgs<MouseWheelEventArgs>(this.viewport2D, e));
        }

        private void Viewport2D_MouseMove(object sender, MouseEventArgs e)
        {
            this.pointerHandlersController.TryHandleMouseMove(new PointerEventArgs<MouseEventArgs>(this.viewport2D, e));
        }

        private void Viewport2D_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.pointerHandlersController.TryHandleMouseUp(new PointerEventArgs<MouseButtonEventArgs>(this.viewport2D, e));
        }

        private void Viewport2D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.pointerHandlersController.TryHandleMouseDown(new PointerEventArgs<MouseButtonEventArgs>(this.viewport2D, e));
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.container;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            this.container.Measure(availableSize);

            return base.MeasureOverride(availableSize);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            this.container.Arrange(new Rect(finalSize));

            return base.ArrangeOverride(finalSize);
        }
    }
}
