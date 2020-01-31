using System;
using System.Windows.Media.Media3D;

namespace CAGD
{
    public class BezierRectangle
    {
        private readonly Point3D[,] controlPoints;

        public BezierRectangle(Point3D[,] controlPoints)
        {
            this.controlPoints = controlPoints;
        }

        public Point3D[,] GetMeshPoints(int uDevisions, int vDevisions)
        {
            int uCount = uDevisions + 1;
            int vCount = vDevisions + 1;
            Point3D[,] surfacePoints = new Point3D[uCount, vCount];

            BezierCurve[] vCurves = this.CalculateVCurves();

            for (int i = 0; i < uCount; i++)
            {
                double u = (double)i / uDevisions;
                BezierCurve uCurve = this.CalculateUBezierCurve(vCurves, u);

                for (int j = 0; j < vCount; j++)
                {
                    double v = (double)j / vDevisions;
                    surfacePoints[i, j] = uCurve.GetPointOnCurve(v);
                }
            }

            return surfacePoints;
        }

        public Point3D GetMeshPoint(double uCoordinate, double vCoordinate)
        {
            BezierCurve[] vCurves = this.CalculateVCurves();
            BezierCurve uCurve = this.CalculateUBezierCurve(vCurves, uCoordinate);
            Point3D point = uCurve.GetPointOnCurve(vCoordinate);

            return point;
        }

        private BezierCurve CalculateUBezierCurve(BezierCurve[] vCurves, double u)
        {
            int vCount = this.controlPoints.GetLength(1);
            Point3D[] bezierPoints = new Point3D[vCount];

            for (int v = 0; v < vCount; v++)
            {
                bezierPoints[v] = vCurves[v].GetPointOnCurve(u);
            }

            return new BezierCurve(bezierPoints);
        }

        private BezierCurve[] CalculateVCurves()
        {
            int uCount = this.controlPoints.GetLength(0);
            int vCount = this.controlPoints.GetLength(1);
            BezierCurve[] vCurves = new BezierCurve[vCount];

            for (int v = 0; v < vCount; v++)
            {
                Point3D[] bezierPoints = new Point3D[uCount];

                for (int u = 0; u < uCount; u++)
                {
                    bezierPoints[u] = this.controlPoints[u, v];
                }

                vCurves[v] = new BezierCurve(bezierPoints);
            }

            return vCurves;
        }
    }
}
