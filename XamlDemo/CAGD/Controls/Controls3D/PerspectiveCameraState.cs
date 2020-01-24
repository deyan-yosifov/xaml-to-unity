using System;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D
{
    public class PerspectiveCameraState : CameraState
    {
        private readonly double fieldOfView;
        private readonly Transform3D transform;
        private readonly Point3D position;
        private readonly double nearPlaneDistance;
        private readonly double farPlaneDistance;
        private readonly Vector3D direction;
        private readonly Vector3D upDirection;

        public PerspectiveCameraState(PerspectiveCamera camera)
            : base(camera)
        {
            this.fieldOfView = camera.FieldOfView;
            this.transform = camera.Transform;
            this.position = camera.Position;
            this.nearPlaneDistance = camera.NearPlaneDistance;
            this.farPlaneDistance = camera.FarPlaneDistance;
            this.direction = camera.LookDirection;
            this.upDirection = camera.UpDirection;
        }

        public override bool Equals(object obj)
        {
            PerspectiveCameraState other = obj as PerspectiveCameraState;

            if (other == null)
            {
                return false;
            }

            return
            this.fieldOfView.Equals(other.fieldOfView) &&
            this.transform.Equals(other.transform) &&
            this.position.Equals(other.position) &&
            this.nearPlaneDistance.Equals(other.nearPlaneDistance) &&
            this.farPlaneDistance.Equals(other.farPlaneDistance) &&
            this.direction.Equals(other.direction) &&
            this.upDirection.Equals(other.upDirection);
        }

        public override int GetHashCode()
        {
            return
            this.fieldOfView.GetHashCode() ^
            this.transform.GetHashCode() ^
            this.position.GetHashCode() ^
            this.nearPlaneDistance.GetHashCode() ^
            this.farPlaneDistance.GetHashCode() ^
            this.direction.GetHashCode() ^
            this.upDirection.GetHashCode();
        }
    }
}
