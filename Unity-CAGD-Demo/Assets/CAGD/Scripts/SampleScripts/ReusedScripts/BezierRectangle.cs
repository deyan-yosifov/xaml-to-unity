using UnityEngine;

namespace CAGD
{
    public class BezierRectangle
    {
        private readonly Vector3[,] controlPoints;

        public BezierRectangle(Vector3[,] controlPoints)
        {
            this.controlPoints = controlPoints;
        }

        public Vector3[,] GetMeshPoints(int uDevisions, int vDevisions)
        {
            int uCount = uDevisions + 1;
            int vCount = vDevisions + 1;
            Vector3[,] surfacePoints = new Vector3[uCount, vCount];

            BezierCurve[] vCurves = this.CalculateVCurves();

            for (int i = 0; i < uCount; i++)
            {
                float u = (float)i / uDevisions;
                BezierCurve uCurve = this.CalculateUBezierCurve(vCurves, u);

                for (int j = 0; j < vCount; j++)
                {
                    float v = (float)j / vDevisions;
                    surfacePoints[i, j] = uCurve.GetPointOnCurve(v);
                }
            }

            return surfacePoints;
        }

        public Vector3 GetMeshPoint(float uCoordinate, float vCoordinate)
        {
            BezierCurve[] vCurves = this.CalculateVCurves();
            BezierCurve uCurve = this.CalculateUBezierCurve(vCurves, uCoordinate);
            Vector3 point = uCurve.GetPointOnCurve(vCoordinate);

            return point;
        }

        private BezierCurve CalculateUBezierCurve(BezierCurve[] vCurves, float u)
        {
            int vCount = this.controlPoints.GetLength(1);
            Vector3[] bezierPoints = new Vector3[vCount];

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
                Vector3[] bezierPoints = new Vector3[uCount];

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
