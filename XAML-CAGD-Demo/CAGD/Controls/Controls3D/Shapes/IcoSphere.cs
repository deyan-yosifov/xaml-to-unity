using System;
using System.Windows;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Shapes
{
    public class IcoSphere : GeodesicSphere
    {
        private static readonly Vector IcoRadiusVector;

        static IcoSphere()
        {
            double goldenRatio = (Math.Sqrt(5) + 1) / 2;
            Vector scaledRadius = new Vector(goldenRatio, 1);

            IcoRadiusVector = scaledRadius * (GeodesicSphere.Radius / scaledRadius.Length);
        }

        public IcoSphere(int subDevisions, bool isSmooth)
            : base(subDevisions, isSmooth, IcoSphere.GetInitialPoints(), IcoSphere.GetInitialTriangles())
        {            
        }

        public override SphereType SphereType
        {
            get
            {
                return Shapes.SphereType.IcoSphere;
            }
        }

        private static Point3D[] GetInitialPoints()
        {
            return new Point3D[]
            {
                new Point3D(-IcoRadiusVector.Y, 0, IcoRadiusVector.X),
                new Point3D(IcoRadiusVector.Y, 0, IcoRadiusVector.X),
                new Point3D(-IcoRadiusVector.Y, 0, -IcoRadiusVector.X),
                new Point3D(IcoRadiusVector.Y, 0, -IcoRadiusVector.X),                
                new Point3D(0, -IcoRadiusVector.X, -IcoRadiusVector.Y),
                new Point3D(0, -IcoRadiusVector.X, IcoRadiusVector.Y),
                new Point3D(0, IcoRadiusVector.X, -IcoRadiusVector.Y),
                new Point3D(0, IcoRadiusVector.X, IcoRadiusVector.Y),
                new Point3D(IcoRadiusVector.X, IcoRadiusVector.Y, 0),
                new Point3D(IcoRadiusVector.X, -IcoRadiusVector.Y, 0),
                new Point3D(-IcoRadiusVector.X, IcoRadiusVector.Y, 0),
                new Point3D(-IcoRadiusVector.X, -IcoRadiusVector.Y, 0),
            };
        }

        private static int[] GetInitialTriangles()
        {
            return new int[]
            {
                0, 5, 1,
                0, 1, 7,
                0, 7, 10,
                0, 10, 11,
                0, 11, 5,
                3, 9, 4,
                3, 4, 2,
                3, 2, 6,
                3, 6, 8,
                3, 8, 9,
                5, 11, 4,
                11, 2, 4,
                11, 10, 2,
                10, 6, 2,
                10, 7, 6,
                7, 8, 6,
                7, 1, 8,
                1, 9, 8,
                1, 5, 9,
                5, 4, 9,
            };
        }
    }
}
