using UnityEngine;

namespace CAGD.Controls.Controls3D.Cameras
{
    internal class OrbitPositionInfo
    {
        public Vector2 PositionOnUnityDistantPlane { get; set; }
        public float FullCircleLength { get; set; }
        public Vector3 CameraX { get; set; }
        public Vector3 CameraY { get; set; }
        public Vector3 CameraZ { get; set; }
    }
}
