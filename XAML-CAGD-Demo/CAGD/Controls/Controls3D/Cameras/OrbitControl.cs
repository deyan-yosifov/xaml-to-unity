using CAGD.Utilities;
using CAGD.Controls.MouseHandlers;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Cameras
{
    public class OrbitControl : IPointerHandler
    {
        private const double FullCircleAngleInDegrees = 360;
        private const int WheelSingleDelta = 120;
        private static readonly Vector3D zAxisVector = new Vector3D(0, 0, 1);
        private readonly SceneEditor editor;
        private readonly MouseDelayManager moveDelayManager;
        private bool isEnabled;
        private DragAction dragAction;
        private Vector3D firstPanDirection;
        private OrbitPositionInfo firstOrbitPosition;

        internal OrbitControl(SceneEditor editor)
        {
            this.editor = editor;
            this.moveDelayManager = new MouseDelayManager(true);

            this.IsEnabled = true;
            this.ZoomSpeed = 0.1;
            this.MoveDeltaTime = 20;
            this.WidthOrbitAngleInDegrees = 180;
            this.dragAction = DragAction.NoAction;
        }

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

        public double ZoomSpeed
        {
            get;
            set;
        }

        public int MoveDeltaTime
        {
            get
            {
                return this.moveDelayManager.TimeInterval;
            }
            set
            {
                this.moveDelayManager.TimeInterval = value;
            }
        }

        public double WidthOrbitAngleInDegrees
        {
            get;
            set;
        }

        private SceneEditor Editor
        {
            get
            {
                return this.editor;
            }
        }

        public bool TryHandleMouseDown(PointerEventArgs<MouseButtonEventArgs> e)
        {
            Point position = e.Position;
            Size viewportSize = e.SenderSize;

            if (e.OriginalArgs.MouseDevice.MiddleButton == MouseButtonState.Pressed)
            {
                this.dragAction = DragAction.Pan;

                this.Editor.DoActionOnCamera(
                (perspectiveCamera) =>
                {
                    this.StartPan(perspectiveCamera, position, viewportSize);
                },
                (orthographicCamera) =>
                {
                    this.StartPan(orthographicCamera, position, viewportSize);
                });
            }
            else
            {
                this.dragAction = DragAction.Orbit;

                this.Editor.DoActionOnCamera(
                (perspectiveCamera) =>
                {
                    this.StartOrbit(perspectiveCamera, position, viewportSize);
                },
                (orthographicCamera) =>
                {
                    this.StartOrbit(orthographicCamera, position, viewportSize);
                });
            }

            return true;
        }

        public bool TryHandleMouseUp(PointerEventArgs<MouseButtonEventArgs> e)
        {
            this.dragAction = DragAction.NoAction;

            return true;
        }

        public bool TryHandleMouseMove(PointerEventArgs<MouseEventArgs> e)
        {
            if (this.dragAction != DragAction.NoAction)
            {
                if (this.moveDelayManager.ShouldHandleMouse(e))
                {
                    Point position = e.Position;
                    Size viewportSize = e.SenderSize;

                    if (this.dragAction == DragAction.Orbit)
                    {
                        this.Editor.DoActionOnCamera(
                            (perspectiveCamera) =>
                            {
                                this.Orbit(perspectiveCamera, position, viewportSize);
                            },
                            (orthographicCamera) =>
                            {
                                this.Orbit(orthographicCamera, position, viewportSize);
                            });
                    }
                    else if (this.dragAction == DragAction.Pan)
                    {
                        this.Editor.DoActionOnCamera(
                            (perspectiveCamera) =>
                            {
                                this.Pan(perspectiveCamera, position, viewportSize);
                            },
                            (orthographicCamera) =>
                            {
                                this.Pan(orthographicCamera, position, viewportSize);
                            });
                    }

                    return true;
                }
            }

            return false;
        }

        public bool TryHandleMouseWheel(PointerEventArgs<MouseWheelEventArgs> e)
        {
            Point position = e.Position;
            Size viewportSize = e.SenderSize;
            double zoomAmount = (this.ZoomSpeed * e.OriginalArgs.Delta) / WheelSingleDelta;

            this.Editor.DoActionOnCamera(
                (perspectiveCamera) =>
                {
                    this.Zoom(perspectiveCamera, position, zoomAmount, viewportSize);
                },
                (orthographicCamera) =>
                {
                    this.Zoom(orthographicCamera, position, zoomAmount, viewportSize);
                });

            return true;
        }

        private void Orbit(PerspectiveCamera perspectiveCamera, Point position, Size viewportSize)
        {
            Point positionOnUnityDistantPlane = CameraHelper.GetPointOnUnityDistantPlane(position, viewportSize, perspectiveCamera.FieldOfView);
            Point3D currentCameraPosition = perspectiveCamera.Position;
            Vector3D currentCameraLookDirection = perspectiveCamera.LookDirection;

            this.Orbit(positionOnUnityDistantPlane, currentCameraPosition, currentCameraLookDirection);
        }

        private void Orbit(OrthographicCamera orthographicCamera, Point position, Size viewportSize)
        {
            throw new NotImplementedException();
        }

        private void Orbit(Point positionOnUnityDistantPlane, Point3D currentCameraPosition, Vector3D currentCameraLookDirection)
        {
            Vector vector = positionOnUnityDistantPlane - this.firstOrbitPosition.PositionOnUnityDistantPlane;
            double angleInDegrees = ((vector.Length / this.firstOrbitPosition.FullCircleLength) * FullCircleAngleInDegrees) % FullCircleAngleInDegrees;

            if (!angleInDegrees.IsZero())
            {
                Vector3D rotateDirection = this.firstOrbitPosition.CameraX * vector.X + this.firstOrbitPosition.CameraY * vector.Y;
                Vector3D rotationAxis = Vector3D.CrossProduct(this.firstOrbitPosition.CameraZ, rotateDirection);
                rotationAxis.Normalize();

                Matrix3D matrix = new Matrix3D();
                matrix.Rotate(new Quaternion(rotationAxis, angleInDegrees));
                Vector3D reverseLookDirection = matrix.Transform(this.firstOrbitPosition.CameraZ);
                reverseLookDirection *= -currentCameraLookDirection.Length;

                Point3D lookAtPoint = currentCameraPosition + currentCameraLookDirection;
                Point3D cameraPosition = lookAtPoint + reverseLookDirection;
                double rollAngleInDegrees = OrbitControl.CalculateRollAngleOnOrbit(reverseLookDirection, currentCameraLookDirection);

                this.Editor.Look(cameraPosition, lookAtPoint, rollAngleInDegrees);
            }
        }

        private static double CalculateRollAngleOnOrbit(Vector3D reverseLookDirection, Vector3D previousCameraLookDirection)
        {
            double rollAngle = 0;
            reverseLookDirection.Normalize();

            if (Math.Abs(reverseLookDirection.Z).IsEqualTo(zAxisVector.Z))
            {
                previousCameraLookDirection.Normalize();

                if (Math.Abs(previousCameraLookDirection.Z) != zAxisVector.Z)
                {
                    Vector projectedPreviousLook = new Vector(previousCameraLookDirection.X, previousCameraLookDirection.Y);
                    Vector currentLookUp = new Vector(0, -reverseLookDirection.Z);

                    rollAngle = Vector.AngleBetween(projectedPreviousLook, currentLookUp);
                }
            }

            return rollAngle;
        }

        private void Pan(PerspectiveCamera perspectiveCamera, Point panPoint, Size viewportSize)
        {
            Vector3D panDirection = CameraHelper.GetLookDirectionFromPoint(panPoint, viewportSize, perspectiveCamera);

            this.Pan(perspectiveCamera.Position, perspectiveCamera.LookDirection, panDirection);
        }

        private void Pan(OrthographicCamera orthographicCamera, Point panPoint, Size viewportSize)
        {
            throw new NotImplementedException();
        }

        private void Pan(Point3D currentCameraPosition, Vector3D currentCameraLookDirection, Vector3D panDirection)
        {
            double angle = Vector3D.AngleBetween(firstPanDirection, panDirection);

            if (!angle.IsZero())
            {
                Vector3D rotationAxis = Vector3D.CrossProduct(panDirection, firstPanDirection);
                rotationAxis.Normalize();
                Matrix3D matrix = new Matrix3D();
                matrix.Rotate(new Quaternion(rotationAxis, angle));
                Vector3D cameraLookDirection = matrix.Transform(currentCameraLookDirection);

                this.Editor.Look(currentCameraPosition, currentCameraPosition + cameraLookDirection);
            }
        }

        private void StartPan(PerspectiveCamera perspectiveCamera, Point panPoint, Size viewportSize)
        {
            this.firstPanDirection = CameraHelper.GetLookDirectionFromPoint(panPoint, viewportSize, perspectiveCamera);
        }

        private void StartPan(OrthographicCamera orthographicCamera, Point panPoint, Size viewportSize)
        {
            throw new NotImplementedException();
        }

        private void StartOrbit(PerspectiveCamera perspectiveCamera, Point orbitPoint, Size viewportSize)
        {
            Vector3D x, y, z;
            CameraHelper.GetCameraLocalCoordinateVectors(perspectiveCamera.LookDirection, perspectiveCamera.UpDirection, out x, out y, out z);

            this.firstOrbitPosition = new OrbitPositionInfo()
            {
                PositionOnUnityDistantPlane = CameraHelper.GetPointOnUnityDistantPlane(orbitPoint, viewportSize, perspectiveCamera.FieldOfView),
                FullCircleLength = this.CalculateFullCircleLength(perspectiveCamera),
                CameraX = x,
                CameraY = y,
                CameraZ = z,
            };
        }

        private double CalculateFullCircleLength(PerspectiveCamera perspectiveCamera)
        {
            double widthLengths = FullCircleAngleInDegrees / this.WidthOrbitAngleInDegrees;
            double fullCircleLength = widthLengths * CameraHelper.GetUnityDistantPlaneWidth(perspectiveCamera.FieldOfView);

            return fullCircleLength;
        }

        private void StartOrbit(OrthographicCamera orthographicCamera, Point orbitPoint, Size viewportSize)
        {
            throw new NotImplementedException();
        }

        private void Zoom(PerspectiveCamera perspectiveCamera, Point position, double zoomAmount, Size viewportSize)
        {
            Vector3D zoomDirection = CameraHelper.GetLookDirectionFromPoint(position, viewportSize, perspectiveCamera);
            perspectiveCamera.Position = CalculateZoomedPosition(zoomDirection, perspectiveCamera.LookDirection, perspectiveCamera.Position, zoomAmount);
        }

        private void Zoom(OrthographicCamera orthographicCamera, Point position, double zoomAmount, Size viewportSize)
        {
            throw new NotImplementedException();
        }

        private static Point3D CalculateZoomedPosition(Vector3D zoomDirection, Vector3D currentLookVector, Point3D currentPosition, double zoomAmount)
        {
            Vector3D zoomVector = zoomDirection * (currentLookVector.Length * zoomAmount);
            Point3D zoomPosition = currentPosition + zoomVector;

            return zoomPosition;
        }
    }
}
