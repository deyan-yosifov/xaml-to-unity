using CAGD.Utilities;
using UnityEngine;

namespace CAGD
{
    public class BezierCurve
    {
        private readonly Vector3[] points;

        public BezierCurve(Vector3[] controlPoints)
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

        public Vector3 GetPointOnCurve(float tBarycentricCoordinate)
        {
            return this.GetPointOnCurve(tBarycentricCoordinate, 1 - tBarycentricCoordinate);
        }

        private Vector3 GetPointOnCurve(float t, float s)
        {
            if (this.Degree == 1)
            {
                return InterpolatePoints(this.points[0], this.points[1], t, s);
            }

            Vector3[] nextPoints = new Vector3[this.points.Length - 1];

            for (int i = 0; i < nextPoints.Length; i++)
            {
                nextPoints[i] = InterpolatePoints(this.points[i], this.points[i + 1], t, s);
            }

            return new BezierCurve(nextPoints).GetPointOnCurve(t, s);
        }

        private static Vector3 InterpolatePoints(Vector3 start, Vector3 end, float t, float s)
        {
            return new Vector3(start.x * s + end.x * t, start.y * s + end.y * t, start.z * s + end.z * t);
        }
    }
}
