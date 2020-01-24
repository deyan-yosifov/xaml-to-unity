using CAGD.Utilities;
using CAGD.Controls.MouseHandlers;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Cameras
{
    public class ZoomToContentsHandler : IPointerHandler
    {
        private readonly SceneEditor sceneEditor;
        private readonly IContentProvider contentProvider;
        private readonly MouseDelayManager delayManager;
        private bool hasHandledFirstDown;

        public ZoomToContentsHandler(SceneEditor sceneEditor, IContentProvider contentProvider)
        {
            Guard.ThrowExceptionIfNull(sceneEditor, "sceneEditor");
            Guard.ThrowExceptionIfNull(contentProvider, "contentProvider");

            this.sceneEditor = sceneEditor;
            this.contentProvider = contentProvider;
            this.delayManager = new MouseDelayManager(false);
            this.DoubleClickInterval = 400;
            this.IsEnabled = true;
        }

        public bool IsEnabled { get; set; }

        public string Name
        {
            get
            {
                return "ZoomToContentsHandler";
            }
        }

        public int DoubleClickInterval
        {
            get
            {
                return this.delayManager.TimeInterval;
            }
            set
            {
                this.delayManager.TimeInterval = value;
            }
        }

        public bool HandlesDragMove
        {
            get
            {
                return false;
            }
        }

        public bool TryHandleMouseDown(PointerEventArgs<MouseButtonEventArgs> e)
        {
            if (e.OriginalArgs.ChangedButton != MouseButton.Middle)
            {
                return false;
            }

            bool shouldHandleDelay = this.delayManager.ShouldHandleMouse(e);

            if (this.hasHandledFirstDown && shouldHandleDelay)
            {
                this.ZoomToContents();
                this.hasHandledFirstDown = false;
            }
            else
            {
                this.hasHandledFirstDown = true;
            }

            return false;
        }

        public bool TryHandleMouseUp(PointerEventArgs<MouseButtonEventArgs> e)
        {
            return false;
        }

        public bool TryHandleMouseMove(PointerEventArgs<MouseEventArgs> e)
        {
            return false;
        }

        public bool TryHandleMouseWheel(PointerEventArgs<MouseWheelEventArgs> e)
        {
            return false;
        }

        public void ZoomToContents()
        {
            this.sceneEditor.DoActionOnCamera((perspectiveCamera) =>
            {
                IEnumerable<Point3D> contentPoints = this.contentProvider.GetContentPoints();
                Point3D fromPoint = CameraHelper.GetZoomToContentsCameraPosition(perspectiveCamera, this.sceneEditor.ViewportSize, contentPoints);
                Vector3D i, j, k;
                CameraHelper.GetCameraLocalCoordinateVectors(perspectiveCamera.LookDirection, perspectiveCamera.UpDirection, out i, out j, out k);
                Point3D boundingCenter = GeometryHelper.GetBoundingRectangleCenter(contentPoints, i, j, k);
                Vector3D lookDirection = perspectiveCamera.LookDirection;
                lookDirection.Normalize();
                double projectedCoordinate = Vector3D.DotProduct(lookDirection, boundingCenter - fromPoint);
                Point3D projectedCenter = fromPoint + projectedCoordinate * lookDirection;
                this.sceneEditor.Look(fromPoint, projectedCenter);
            }, (orthographicCamera) => Guard.ThrowNotSupportedCameraException());
        }
    }
}
