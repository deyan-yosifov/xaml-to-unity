using System;
using System.Windows;

namespace CAGD.Controls.Controls3D.Shapes
{
    public class Sphere : RotationalShape, ISphereShape
    {
        public const double Diameter = 1;
        public const double Radius = Diameter / 2;

        public Sphere(int parallelsCount, int meridiansCount, bool isSmooth)
            : base(Sphere.CalculateSectionPoints(parallelsCount), meridiansCount, isSmooth)
        {
        }

        public SphereType SphereType
        {
            get
            {
                return Shapes.SphereType.UVSphere;
            }
        }

        public ShapeBase Shape
        {
            get
            {
                return this;
            }
        }

        private static Point[][] CalculateSectionPoints(int parallelsCount)
        {
            Point[] points = new Point[parallelsCount + 2];
            points[0] = new Point(0, Radius);
            points[parallelsCount + 1] = new Point(0, -Radius);
            double deltaAngle = Math.PI / (parallelsCount + 1);

            for (int i = 1; i <= parallelsCount; i++)
            {
                double angle = i * deltaAngle;
                points[i] = new Point(Radius * Math.Sin(angle), Radius * Math.Cos(angle));
            }

            return new Point[][] { points };
        }
    }
}
