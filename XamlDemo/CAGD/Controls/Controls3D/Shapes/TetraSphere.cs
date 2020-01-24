using System;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Shapes
{
    public class TetraSphere : GeodesicSphere
    {
        public TetraSphere(int subDevisions, bool isSmooth)
            : base(subDevisions, isSmooth, TetraSphere.GetInitialPoints(), TetraSphere.GetInitialTriangles())
        {            
        }

        public override SphereType SphereType
        {
            get
            {
                return Shapes.SphereType.TetraSphere;
            }
        }

        private static Point3D[] GetInitialPoints()
        {
            double sqrt2 = Math.Sqrt(2);
            double sqrt3 = Math.Sqrt(3);
            double groundHeight = -Radius / 3;

            return new Point3D[]
            {
                new Point3D(0, 0, Radius),
                new Point3D(2 * Radius * sqrt2 / 3, 0, groundHeight),
                new Point3D(-Radius * sqrt2 / 3, Radius * sqrt2 / sqrt3, groundHeight),
                new Point3D(-Radius * sqrt2 / 3, -Radius * sqrt2 / sqrt3, groundHeight),
            };
        }

        private static int[] GetInitialTriangles()
        {
            return new int[]
            {
                0, 1, 2,
                0, 2, 3,
                0, 3, 1,
                1, 3, 2,
            };
        }
    }
}
