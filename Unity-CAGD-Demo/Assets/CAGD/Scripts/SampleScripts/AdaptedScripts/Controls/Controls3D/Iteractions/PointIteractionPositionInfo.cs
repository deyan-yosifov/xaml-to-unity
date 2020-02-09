using UnityEngine;

namespace CAGD.Controls.Controls3D.Iteractions
{
    internal class PointIteractionPositionInfo
    {
        public Camera Camera { get; set; }

        public Plane MovementPlane { get; set; }

        public Vector3 InitialIteractionPosition { get; set; }

        public Vector3? ProjectionLineVector { get; set; }

        public Vector3? ProjectionPlaneVector { get; set; }
    }
}
