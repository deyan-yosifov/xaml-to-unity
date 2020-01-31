using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Iteractions
{
    internal class PointIteractionPositionInfo
    {
        public Point3D MovementPlanePoint { get; set; }

        public Vector3D MovementPlaneNormal { get; set; }

        public Point3D InitialIteractionPosition { get; set; }

        public Vector3D? ProjectionLineVector { get; set; }

        public Vector3D? ProjectionPlaneVector { get; set; }
    }
}
