using CAGD.Utilities;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public class BezierCurve
    {
        private readonly Point3D[] points;

        public BezierCurve(Point3D[] controlPoints)
        {
            Guard.ThrowExceptionIfLessThan(controlPoints.Length, 2, "controlPoints.Length");

            this.points = controlPoints;
        }

        public int Degree
        {
            get
            {
                return this.points.Length - 1;
            }
        }

        public Point3D GetPointOnCurve(double tBarycentricCoordinate)
        {
            return this.GetPointOnCurve(tBarycentricCoordinate, 1 - tBarycentricCoordinate);
        }

        private Point3D GetPointOnCurve(double t, double s)
        {
            if (this.Degree == 1)
            {
                return InterpolatePoints(this.points[0], this.points[1], t, s);
            }

            Point3D[] nextPoints = new Point3D[this.points.Length - 1];

            for (int i = 0; i < nextPoints.Length; i++)
            {
                nextPoints[i] = InterpolatePoints(this.points[i], this.points[i + 1], t, s);
            }

            return new BezierCurve(nextPoints).GetPointOnCurve(t, s);
        }

        private static Point3D InterpolatePoints(Point3D start, Point3D end, double t, double s)
        {
            return new Point3D(start.X * s + end.X * t, start.Y * s + end.Y * t, start.Z * s + end.Z * t);
        }
    }
}
