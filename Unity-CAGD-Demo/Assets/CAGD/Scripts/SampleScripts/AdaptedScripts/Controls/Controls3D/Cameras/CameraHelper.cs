using UnityEngine;

namespace CAGD.Controls.Controls3D.Cameras
{
    public static class CameraHelper
    {
        private static readonly Vector3 WorldUp = Vector3.forward;

        public static void Look(this Camera camera, Vector3 fromPoint, Vector3 toPoint, float rollAngleInDegrees)
        {
            Vector3 lookDirection = (toPoint - fromPoint).normalized;
            camera.transform.rotation = Quaternion.LookRotation(lookDirection, WorldUp) * Quaternion.AngleAxis(rollAngleInDegrees, lookDirection);
            camera.transform.position = fromPoint;
        }
    }
}
