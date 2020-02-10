using CAGD.Controls.Controls3D.Visuals;
using CAGD.Controls.PointerHandlers;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CAGD.Controls.Controls3D.Iteractions
{
    public class IteractivePointsHandler : MonoBehaviour, IPointerHandler
    {
        private IteractionRestrictor restrictor;
        private HashSet<PointVisual> registeredPoints;
        private PointVisual capturedPoint;
        private bool isEnabled;
        
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
            this.registeredPoints.Add(point);
        }

        public void UnRegisterIteractivePoint(PointVisual point)
        {
            this.registeredPoints.Remove(point);
        }

        public bool TryHandleMouseDown(PointerEventArgs<PointerEventData> e)
        {
            this.ReleaseCapturedPoint();
            Camera camera = e.raycastCamera;

            if (Physics.Raycast(camera.ScreenPointToRay(e.data.pressPosition), out RaycastHit hit) )
            {
                PointVisual point = hit.transform.GetComponent<PointVisual>();

                if (point && this.registeredPoints.Contains(point))
                {
                    this.CapturePoint(camera, point);

                    return true;
                }
            }

            return false;
        }

        public bool TryHandleMouseUp(PointerEventArgs<PointerEventData> e)
        {
            if (this.restrictor.IsInIteraction)
            {
                this.ReleaseCapturedPoint();

                return true;
            }

            return false;
        }

        public bool TryHandleMouseMove(PointerEventArgs<PointerEventData> e)
        {
            if (this.restrictor.IsInIteraction)
            {
                Vector2 viewportPosition = e.data.position;

                Vector3 position;
                if (this.restrictor.TryGetIteractionPoint(viewportPosition, out position))
                {
                    this.capturedPoint.Position = position;
                }

                return true;
            }

            return false;
        }

        public bool TryHandleMouseWheel(PointerEventArgs<MouseWheelEventData> e)
        {
            return this.restrictor.IsInIteraction;
        }

        public event EventHandler PointCaptured;
        public event EventHandler PointReleased;

        private void Awake()
        {
            this.restrictor = new IteractionRestrictor();
            this.registeredPoints = new HashSet<PointVisual>();
            this.capturedPoint = null;
            this.IsEnabled = true;
        }

        private void CapturePoint(Camera camera, PointVisual point)
        {
            this.capturedPoint = point;
            this.restrictor.BeginIteraction(camera, point.Position);

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
