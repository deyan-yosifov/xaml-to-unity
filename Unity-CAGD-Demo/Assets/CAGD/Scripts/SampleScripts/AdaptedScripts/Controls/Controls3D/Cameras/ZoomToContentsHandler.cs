using CAGD.Utilities;
using CAGD.Controls.PointerHandlers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CAGD.Controls.Controls3D.Cameras
{
    public class ZoomToContentsHandler : MonoBehaviour, IPointerHandler
    {
        [SerializeField]
        private float doubleClickInterval = 0.4f;
        private IContentProvider contentProvider;
        private PointerDelayManager delayManager;
        private bool hasHandledFirstDown;

        private void Awake()
        {
            this.contentProvider = this.GetComponentInParent<IContentProvider>();
            this.delayManager = new PointerDelayManager(false);
            this.delayManager.TimeInterval = this.doubleClickInterval;
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

        public bool HandlesDragMove
        {
            get
            {
                return false;
            }
        }

        public bool TryHandleMouseDown(PointerEventArgs<PointerEventData> e)
        {
            if (e.data.button != PointerEventData.InputButton.Middle)
            {
                return false;
            }

            bool shouldHandleDelay = this.delayManager.ShouldHandlePointer(e);

            if (this.hasHandledFirstDown && shouldHandleDelay)
            {
                this.ZoomToContents(e.raycastCamera);
                this.hasHandledFirstDown = false;
            }
            else
            {
                this.hasHandledFirstDown = true;
            }

            return false;
        }

        public bool TryHandleMouseUp(PointerEventArgs<PointerEventData> e)
        {
            return false;
        }

        public bool TryHandleMouseMove(PointerEventArgs<PointerEventData> e)
        {
            return false;
        }

        public bool TryHandleMouseWheel(PointerEventArgs<MouseWheelEventData> e)
        {
            return false;
        }

        public void ZoomToContents(Camera camera)
        {
            IEnumerable<Vector3> contentPoints = this.contentProvider.GetContentPoints();
            Vector3 fromPoint = CameraHelper.GetZoomToContentsCameraPosition(camera, contentPoints);
            Vector3 boundingCenter = GeometryHelper.GetBoundingRectangleCenter(contentPoints, camera.transform.right, camera.transform.up, camera.transform.forward);
            Vector3 lookDirection = camera.transform.forward;
            lookDirection.Normalize();
            float projectedCoordinate = Vector3.Dot(lookDirection, boundingCenter - fromPoint);
            Vector3 projectedCenter = fromPoint + projectedCoordinate * lookDirection;
            camera.Look(fromPoint, projectedCenter, 0);
        }
    }
}
