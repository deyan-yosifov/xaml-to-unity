using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Shapes
{
    public class OctaSphere : GeodesicSphere
    {
        public OctaSphere(int subDevisions, bool isSmooth)
            : base(subDevisions, isSmooth, OctaSphere.GetInitialPoints(), OctaSphere.GetInitialTriangles())
        {            
        }

        public override SphereType SphereType
        {
            get
            {
                return Shapes.SphereType.OctaSphere;
            }
        }

        private static Point3D[] GetInitialPoints()
        {
            return new Point3D[]
            {
                new Point3D(Radius, 0, 0),
                new Point3D(-Radius, 0, 0),
                new Point3D(0, Radius, 0),
                new Point3D(0, -Radius, 0),
                new Point3D(0, 0, Radius),
                new Point3D(0, 0, -Radius),
            };
        }

        private static int[] GetInitialTriangles()
        {
            return new int[]
            {
                4, 0, 2,
                4, 2, 1,
                4, 1, 3,
                4, 3, 0,
                5, 2, 0,
                5, 1, 2,
                5, 3, 1,
                5, 0, 3,
            };
        }
    }
}
