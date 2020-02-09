using System;
using System.Collections.Generic;
using UnityEngine;

namespace CAGD
{
    public class BezierTriangle
    {
        public const int MinimumDegree = 1;
        public const int MinimumPointsCount = 3;
        private static readonly Dictionary<int, int> pointsCountToDegreeCache;
        private static int maxDegreeCached;
        private static int maxPointsCountCached;
        private readonly Vector3[] points;
        private readonly int degree;

        static BezierTriangle()
        {
            maxDegreeCached = MinimumDegree;
            maxPointsCountCached = MinimumPointsCount;
            BezierTriangle.pointsCountToDegreeCache = new Dictionary<int, int>();
            BezierTriangle.pointsCountToDegreeCache[maxPointsCountCached] = maxDegreeCached;
        }

        public BezierTriangle(Vector3[] controlPoints)
        {
            this.points = controlPoints;

            int degree;
            if (!BezierTriangle.pointsCountToDegreeCache.TryGetValue(controlPoints.Length, out degree))
            {
                while (BezierTriangle.maxPointsCountCached < controlPoints.Length)
                {
                    BezierTriangle.maxPointsCountCached += ++BezierTriangle.maxDegreeCached + 1;
                    BezierTriangle.pointsCountToDegreeCache[maxPointsCountCached] = maxDegreeCached;
                }

                if (BezierTriangle.maxPointsCountCached == controlPoints.Length)
                {
                    degree = BezierTriangle.maxDegreeCached;
                }
                else
                {
                    throw new ArgumentException(string.Format("Invalid control points count: {0}", controlPoints.Length));
                }
            }

            this.degree = degree;
        }

        public Vector3 this[int index]
        {
            get
            {
                return this.points[index];
            }
        }

        public int MeshPointsCount
        {
            get
            {
                return this.points.Length;
            }
        }

        public int Degree
        {
            get
            {
                return this.degree;
            }
        }

        public Vector3 GetMeshPoint(float uBarycentricCoordinate, float vBarycentricCoordinate)
        {
            return this.GetMeshPoint(uBarycentricCoordinate, vBarycentricCoordinate, 1 - uBarycentricCoordinate - vBarycentricCoordinate);
        }

        public static int GetMeshPointsCount(int devisions)
        {
            return ((devisions + 1) * (devisions + 2)) / 2;
        }

        public static void IterateTriangleCoordinates(int sideDevisions, Action<double, double> actionOnUVCoordinates)
        {
            int wLevelsMaximum = sideDevisions;

            for (int wLevel = 0; wLevel <= wLevelsMaximum; wLevel++)
            {
                double w = (double)wLevel / sideDevisions;
                int vLevelsMaximum = sideDevisions - wLevel;

                for (int vLevel = 0; vLevel <= vLevelsMaximum; vLevel++)
                {
                    double v = (double)vLevel / sideDevisions;
                    double u = 1 - v - w;
                    actionOnUVCoordinates(u, v);
                }
            }
        }

        public void IterateTriangleIndexes(bool skipNonParallelTriangles, Action<int, int, int> actionOnTrianlgePointsIndexes)
        {
            int firstTriangleIndex = 0;

            for (int parallelTrianglesInLevel = this.Degree; parallelTrianglesInLevel > 0; parallelTrianglesInLevel--)
            {
                int nextLevelFirstTriangleIndex = firstTriangleIndex + parallelTrianglesInLevel + 1;
                int nonParallelTrianglesInLevel = parallelTrianglesInLevel - 1;

                for (int i = 0; i < nonParallelTrianglesInLevel; i++)
                {
                    actionOnTrianlgePointsIndexes(firstTriangleIndex + i, firstTriangleIndex + i + 1, nextLevelFirstTriangleIndex + i);

                    if (!skipNonParallelTriangles)
                    {
                        actionOnTrianlgePointsIndexes(firstTriangleIndex + i + 1, nextLevelFirstTriangleIndex + i + 1, nextLevelFirstTriangleIndex + i);
                    }
                }

                int iLast = nonParallelTrianglesInLevel;
                actionOnTrianlgePointsIndexes(firstTriangleIndex + iLast, firstTriangleIndex + iLast + 1, nextLevelFirstTriangleIndex + iLast);

                firstTriangleIndex = nextLevelFirstTriangleIndex;
            }
        }

        public void IterateTrianlges(bool skipNonParallelTriangles, Action<Vector3, Vector3, Vector3> actionOnTrianglePoints)
        {
            this.IterateTriangleIndexes(skipNonParallelTriangles, (i, j, k) =>
            {
                Vector3 a = this.points[i];
                Vector3 b = this.points[j];
                Vector3 c = this.points[k];

                actionOnTrianglePoints(a, b, c);
            });
        }

        private Vector3 GetMeshPoint(float u, float v, float w)
        {
            if (this.Degree == 1)
            {
                return InterpolatePoints(this.points[0], this.points[1], this.points[2], u, v, w);
            }

            int pointIndex = 0;
            Vector3[] nextPoints = new Vector3[this.points.Length - this.Degree - 1];

            this.IterateTrianlges(true, (a, b, c) =>
            {
                nextPoints[pointIndex++] = BezierTriangle.InterpolatePoints(a, b, c, u, v, w);
            });

            return new BezierTriangle(nextPoints).GetMeshPoint(u, v, w);
        }

        private static Vector3 InterpolatePoints(Vector3 a, Vector3 b, Vector3 c, float u, float v, float w)
        {
            return new Vector3(u * a.x + v * b.x + w * c.x, u * a.y + v * b.y + w * c.y, u * a.z + v * b.z + w * c.z);
        }
    }
}
