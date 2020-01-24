using System.Windows;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Cameras
{
    internal class OrbitPositionInfo
    {
        public Point PositionOnUnityDistantPlane { get; set; }
        public double FullCircleLength { get; set; }
        public Vector3D CameraX { get; set; }
        public Vector3D CameraY { get; set; }
        public Vector3D CameraZ { get; set; }
    }
}
