using CAGD.Controls.Controls3D.Visuals;
using CAGD.Controls.MouseHandlers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Iteractions
{
    public class IteractivePointsHandler : IPointerHandler
    {
        private readonly SceneEditor editor;
        private readonly IteractionRestrictor restrictor;
        private readonly Dictionary<Visual3D, PointVisual> registeredPoints;
        private PointVisual capturedPoint;
        private bool isEnabled;

        public IteractivePointsHandler(SceneEditor editor)
        {
            this.editor = editor;
            this.restrictor = new IteractionRestrictor(editor);
            this.registeredPoints = new Dictionary<Visual3D, PointVisual>();
            this.capturedPoint = null;
            this.IsEnabled = true;
        }

        public string Name
        {
            get
            {
                return Scene3DMouseHandlerNames.IteractivePointsHandler;
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                if (this.isEnabled != value)
                {
                    this.isEnabled = value;

                    if (!this.isEnabled)
                    {
                        this.ReleaseCapturedPoint();
                    }
                }
            }
        }

        public bool HandlesDragMove
        {
            get
            {
                return true;
            }
        }

        public bool CanMoveOnXAxis
        {
            get
            {
                return this.restrictor.CanMoveOnXAxis;
            }
            set
            {
                this.restrictor.CanMoveOnXAxis = value;
            }
        }

        public bool CanMoveOnYAxis
        {
            get
            {
                return this.restrictor.CanMoveOnYAxis;
            }
            set
            {
                this.restrictor.CanMoveOnYAxis = value;
            }
        }

        public bool CanMoveOnZAxis
        {
            get
            {
                return this.restrictor.CanMoveOnZAxis;
            }
            set
            {
                this.restrictor.CanMoveOnZAxis = value;
            }
        }

        public PointVisual CapturedPoint
        {
            get
            {
                return this.capturedPoint;
            }
        }

        public void RegisterIteractivePoint(PointVisual point)
        {
            this.registeredPoints[point.Visual] = point;
        }

        public void UnRegisterIteractivePoint(PointVisual point)
        {
            this.registeredPoints.Remove(point.Visual);
        }

        public bool TryHandleMouseDown(PointerEventArgs<MouseButtonEventArgs> e)
        {
            Visual3D visual;
            PointVisual point;
            this.ReleaseCapturedPoint();
            Point viewportPosition = e.Position;

            if (this.editor.TryHitVisual3D(viewportPosition, out visual) && this.registeredPoints.TryGetValue(visual, out point))
            {
                this.CapturePoint(point);

                return true;
            }

            return false;
        }

        public bool TryHandleMouseUp(PointerEventArgs<MouseButtonEventArgs> e)
        {
            if (this.restrictor.IsInIteraction)
            {
                this.ReleaseCapturedPoint();

                return true;
            }

            return false;
        }

        public bool TryHandleMouseMove(PointerEventArgs<MouseEventArgs> e)
        {
            if (this.restrictor.IsInIteraction)
            {
                Point viewportPosition = e.Position;

                Point3D position;
                if (this.restrictor.TryGetIteractionPoint(viewportPosition, out position))
                {
                    this.capturedPoint.Position = position;
                }

                return true;
            }

            return false;
        }

        public bool TryHandleMouseWheel(PointerEventArgs<MouseWheelEventArgs> e)
        {
            return this.restrictor.IsInIteraction;
        }

        public event EventHandler PointCaptured;
        public event EventHandler PointReleased;

        private void CapturePoint(PointVisual point)
        {
            this.capturedPoint = point;
            this.restrictor.BeginIteraction(point.Position);

            this.OnPointCaptured();
        }

        private void ReleaseCapturedPoint()
        {
            if (this.restrictor.IsInIteraction)
            {
                this.capturedPoint = null;
                this.restrictor.EndIteraction();

                this.OnPointReleased();
            }
        }

        private void OnPointCaptured()
        {
            if (this.PointCaptured != null)
            {
                this.PointCaptured(this, new EventArgs());
            }
        }

        private void OnPointReleased()
        {
            if (this.PointReleased != null)
            {
                this.PointReleased(this, new EventArgs());
            }
        }
    }
}
