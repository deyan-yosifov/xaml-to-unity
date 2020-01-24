using System;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D
{
    public class OrthographicCameraState : CameraState
    {
        private readonly double width;
        private readonly Transform3D transform;
        private readonly Point3D position;
        private readonly double nearPlaneDistance;
        private readonly double farPlaneDistance;
        private readonly Vector3D direction;
        private readonly Vector3D upDirection;

        public OrthographicCameraState(OrthographicCamera camera)
            : base(camera)
        {
            this.width = camera.Width;
            this.transform = camera.Transform;
            this.position = camera.Position;
            this.nearPlaneDistance = camera.NearPlaneDistance;
            this.farPlaneDistance = camera.FarPlaneDistance;
            this.direction = camera.LookDirection;
            this.upDirection = camera.UpDirection;
        }

        public override bool Equals(object obj)
        {
            OrthographicCameraState other = obj as OrthographicCameraState;

            if (other == null)
            {
                return false;
            }

            return
            this.width.Equals(other.width) &&
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
            this.width.GetHashCode() ^
            this.transform.GetHashCode() ^
            this.position.GetHashCode() ^
            this.nearPlaneDistance.GetHashCode() ^
            this.farPlaneDistance.GetHashCode() ^
            this.direction.GetHashCode() ^
            this.upDirection.GetHashCode();
        }
    }
}
