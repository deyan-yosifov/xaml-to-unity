using CAGD.Utilities;
using CAGD.Controls.PointerHandlers;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CAGD.Controls.Controls3D.Cameras
{
    public class OrbitControl : MonoBehaviour, IPointerHandler
    {
        private const float FullCircleAngleInDegrees = 360;
        private const float WheelSingleDelta = 0.1f;
        private static readonly Vector3 zAxisVector = new Vector3(0, 0, 1);
        [SerializeField]
        private float zoomSpeed = 0.1f;
        [SerializeField]
        private float moveDeltaTime = 0.05f;
        [SerializeField]
        private float widthOrbitAngleInDegrees = 180;
        private PointerDelayManager moveDelayManager;
        private bool isEnabled;
        private DragAction dragAction;
        private Vector3 firstPanDirection;
        private OrbitPositionInfo firstOrbitPosition;


        public string Name
        {
            get
            {
                return Scene3DMouseHandlerNames.OrbitControlHandler;
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
                    this.dragAction = DragAction.NoAction;
                    this.isEnabled = value;
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

        public bool TryHandleMouseDown(PointerEventArgs<PointerEventData> e)
        {
            Vector2 position = e.data.position;
            //Size viewportSize = e.SenderSize;

            if (e.data.button == PointerEventData.InputButton.Middle)
            {
                this.dragAction = DragAction.Pan;
                this.StartPan(e.raycastCamera, position);
            }
            else
            {
                this.dragAction = DragAction.Orbit;
                //this.StartOrbit(e.raycastCamera, position, viewportSize);
            }

            return true;
        }

        public bool TryHandleMouseUp(PointerEventArgs<PointerEventData> e)
        {
            this.dragAction = DragAction.NoAction;

            return true;
        }

        public bool TryHandleMouseMove(PointerEventArgs<PointerEventData> e)
        {
            if (this.dragAction != DragAction.NoAction)
            {
                if (this.moveDelayManager.ShouldHandlePointer(e))
                {
                    Vector2 position = e.data.position;
                    //Size viewportSize = e.SenderSize;

                    if (this.dragAction == DragAction.Orbit)
                    {
                        //this.Orbit(e.raycastCamera, position, viewportSize);
                    }
                    else if (this.dragAction == DragAction.Pan)
                    {
                        this.Pan(e.raycastCamera, position);
                    }

                    return true;
                }
            }

            return false;
        }

        public bool TryHandleMouseWheel(PointerEventArgs<MouseWheelEventData> e)
        {
            Vector2 position = e.data.position;
            float zoomAmount = (this.zoomSpeed * e.data.delta) / WheelSingleDelta;
            this.Zoom(e.raycastCamera, position, zoomAmount);

            return true;
        }
        private void Awake()
        {
            this.moveDelayManager = new PointerDelayManager(true);
            this.moveDelayManager.TimeInterval = this.moveDeltaTime;

            this.IsEnabled = true;
            this.dragAction = DragAction.NoAction;
        }

        private void Update()
        {
            this.moveDelayManager.TimeInterval = this.moveDeltaTime;
        }

        //private void Orbit(Camera perspectiveCamera, Vector2 position, Size viewportSize)
        //{
        //    Point positionOnUnityDistantPlane = CameraHelper.GetPointOnUnityDistantPlane(position, viewportSize, perspectiveCamera.FieldOfView);
        //    Vector3 currentCameraPosition = perspectiveCamera.Position;
        //    Vector3D currentCameraLookDirection = perspectiveCamera.LookDirection;

        //    this.Orbit(perspectiveCamera, positionOnUnityDistantPlane, currentCameraPosition, currentCameraLookDirection);
        //}

        //private void Orbit(Vector2 positionOnUnityDistantPlane, Vector3 currentCameraPosition, Vector3 currentCameraLookDirection)
        //{
        //    Vector2 vector = positionOnUnityDistantPlane - this.firstOrbitPosition.PositionOnUnityDistantPlane;
        //    float angleInDegrees = ((vector.magnitude / this.firstOrbitPosition.FullCircleLength) * FullCircleAngleInDegrees) % FullCircleAngleInDegrees;

        //    if (!angleInDegrees.IsZero())
        //    {
        //        Vector3 rotateDirection = this.firstOrbitPosition.CameraX * vector.X + this.firstOrbitPosition.CameraY * vector.Y;
        //        Vector3 rotationAxis = Vector3.Cross(this.firstOrbitPosition.CameraZ, rotateDirection);
        //        rotationAxis.Normalize();

        //        Matrix3D matrix = new Matrix3D();
        //        matrix.Rotate(new Quaternion(rotationAxis, angleInDegrees));
        //        Vector3D reverseLookDirection = matrix.Transform(this.firstOrbitPosition.CameraZ);
        //        reverseLookDirection *= -currentCameraLookDirection.magnitude;

        //        Vector3 lookAtPoint = currentCameraPosition + currentCameraLookDirection;
        //        Vector3 cameraPosition = lookAtPoint + reverseLookDirection;
        //        double rollAngleInDegrees = OrbitControl.CalculateRollAngleOnOrbit(reverseLookDirection, currentCameraLookDirection);

        //        this.Editor.Look(cameraPosition, lookAtPoint, rollAngleInDegrees);
        //    }
        //}

        private static double CalculateRollAngleOnOrbit(Vector3 reverseLookDirection, Vector3 previousCameraLookDirection)
        {
            double rollAngle = 0;
            reverseLookDirection.Normalize();

            if (Math.Abs(reverseLookDirection.z).IsEqualTo(zAxisVector.z))
            {
                previousCameraLookDirection.Normalize();

                if (Math.Abs(previousCameraLookDirection.z) != zAxisVector.z)
                {
                    Vector2 projectedPreviousLook = new Vector2(previousCameraLookDirection.x, previousCameraLookDirection.y);
                    Vector2 currentLookUp = new Vector2(0, -reverseLookDirection.z);

                    rollAngle = Vector2.Angle(projectedPreviousLook, currentLookUp);
                }
            }

            return rollAngle;
        }

        private void Pan(Camera perspectiveCamera, Vector2 panPoint)
        {
            Ray ray = perspectiveCamera.ScreenPointToRay(panPoint);
            Vector3 panDirection = ray.direction;

            this.Pan(perspectiveCamera, perspectiveCamera.transform.position, perspectiveCamera.transform.forward, panDirection);
        }

        private void Pan(Camera camera, Vector3 currentCameraPosition, Vector3 currentCameraLookDirection, Vector3 panDirection)
        {
            float angle = Vector3.Angle(firstPanDirection, panDirection);

            if (!angle.IsZero())
            {
                Vector3 rotationAxis = Vector3.Cross(panDirection, firstPanDirection);
                rotationAxis.Normalize();
                Vector3 cameraLookDirection = Quaternion.AngleAxis(angle, rotationAxis) * currentCameraLookDirection;

                camera.Look(currentCameraPosition, currentCameraPosition + cameraLookDirection, 0);
            }
        }

        private void StartPan(Camera perspectiveCamera, Vector2 panPoint)
        {
            Ray ray = perspectiveCamera.ScreenPointToRay(panPoint);
            this.firstPanDirection = ray.direction;
        }

        //private void StartOrbit(Camera perspectiveCamera, Vector2 orbitPoint, Size viewportSize)
        //{
        //    Vector3 x, y, z;
        //    CameraHelper.GetCameraLocalCoordinateVectors(perspectiveCamera.LookDirection, perspectiveCamera.UpDirection, out x, out y, out z);

        //    this.firstOrbitPosition = new OrbitPositionInfo()
        //    {
        //        PositionOnUnityDistantPlane = CameraHelper.GetPointOnUnityDistantPlane(orbitPoint, viewportSize, perspectiveCamera.FieldOfView),
        //        FullCircleLength = this.CalculateFullCircleLength(perspectiveCamera),
        //        CameraX = x,
        //        CameraY = y,
        //        CameraZ = z,
        //    };
        //}

        //private float CalculateFullCircleLength(Camera perspectiveCamera)
        //{
        //    float widthLengths = FullCircleAngleInDegrees / this.WidthOrbitAngleInDegrees;
        //    float fullCircleLength = widthLengths * CameraHelper.GetUnityDistantPlaneWidth(perspectiveCamera.FieldOfView);

        //    return fullCircleLength;
        //}

        private void Zoom(Camera perspectiveCamera, Vector2 position, float zoomAmount)
        {
            Ray ray = perspectiveCamera.ScreenPointToRay(position);
            Vector3 zoomDirection = ray.direction;
            perspectiveCamera.transform.position = CalculateZoomedPosition(zoomDirection, perspectiveCamera.transform.forward, perspectiveCamera.transform.position, zoomAmount);
        }

        private static Vector3 CalculateZoomedPosition(Vector3 zoomDirection, Vector3 currentLookVector, Vector3 currentPosition, float zoomAmount)
        {
            Vector3 zoomVector = zoomDirection * (currentLookVector.magnitude * zoomAmount);
            Vector3 zoomPosition = currentPosition + zoomVector;

            return zoomPosition;
        }
    }
}
