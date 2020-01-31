using CAGD.Utilities;
using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace CAGD.Controls.Controls3D.Shapes
{
    public abstract class GeodesicSphere : ShapeBase, ISphereShape
    {
        private class PointIndexWithMeridianAngle
        {
            private readonly int index;
            private readonly double angle;

            public PointIndexWithMeridianAngle(int index, double angle)
            {
                this.index = index;
                this.angle = angle;
            }

            public int Index
            {
                get
                {
                    return index;
                }
            }

            public double Angle
            {
                get
                {
                    return angle;
                }
            }

            public override string ToString()
            {
                return string.Format("<i:{0}, a:{1}>", this.Index, this.Angle);
            }
        }

        private class PointIndicesCouple
        {
            private readonly int a;
            private readonly int b;

            public PointIndicesCouple(int a, int b)
            {
                this.a = a;
                this.b = b;
            }

            public override bool Equals(object obj)
            {
                PointIndicesCouple other = obj as PointIndicesCouple;

                if (other == null)
                {
                    return false;
                }

                return (this.a == other.a && this.b == other.b) || (this.a == other.b && this.b == other.a);
            }

            public override int GetHashCode()
            {
                return this.a ^ this.b;
            }

            public override string ToString()
            {
                return string.Format("<{0}, {1}>", this.a, this.b);
            }
        }

        public const double Radius = 0.5;
        private static readonly Point3D Center = new Point3D();
        private static readonly Vector ZeroMeridianDirection = new Vector(1, 0);

        protected GeodesicSphere(int subDevisions, bool isSmooth, Point3D[] initialPoints, int[] initialTriangleIndices)
        {
            List<Point3D> points = new List<Point3D>(initialPoints);
            Queue<int> triangleIndices = new Queue<int>(initialTriangleIndices);

            GeodesicSphere.DoSubDevisions(subDevisions, points, triangleIndices);

            Point[] textureCoordinates;
            GeodesicSphere.SplitTrianglesOnTextureMappingBoundaryAndGetTextureCoordinates(points, triangleIndices, out textureCoordinates);

            this.GeometryModel.Geometry = isSmooth ?
                GeodesicSphere.GenerateSmoothGeometry(points, triangleIndices, textureCoordinates) :
                GeodesicSphere.GenerateSharpGeometry(points, triangleIndices, textureCoordinates);

            this.GeometryModel.Geometry.Freeze();
        }

        public abstract SphereType SphereType { get; }

        public ShapeBase Shape
        {
            get
            {
                return this;
            }
        }

        private static void DoSubDevisions(int subDevisions, List<Point3D> points, Queue<int> triangleIndices)
        {
            for (int subDevision = 0; subDevision < subDevisions; subDevision++)
            {
                Dictionary<PointIndicesCouple, int> subDevisionMidPointsCache = new Dictionary<PointIndicesCouple, int>();
                int triangleIndicesBeforeSubDevision = triangleIndices.Count;

                for (int i = 0; i < triangleIndicesBeforeSubDevision; i += 3)
                {
                    int aIndex = triangleIndices.Dequeue();
                    int bIndex = triangleIndices.Dequeue();
                    int cIndex = triangleIndices.Dequeue();
                    int abMidPoint = GeodesicSphere.GetSubDevisionMidPointIndex(aIndex, bIndex, points, subDevisionMidPointsCache);
                    int bcMidPoint = GeodesicSphere.GetSubDevisionMidPointIndex(bIndex, cIndex, points, subDevisionMidPointsCache);
                    int caMidPoint = GeodesicSphere.GetSubDevisionMidPointIndex(cIndex, aIndex, points, subDevisionMidPointsCache);

                    triangleIndices.Enqueue(aIndex);
                    triangleIndices.Enqueue(abMidPoint);
                    triangleIndices.Enqueue(caMidPoint);
                    triangleIndices.Enqueue(abMidPoint);
                    triangleIndices.Enqueue(bIndex);
                    triangleIndices.Enqueue(bcMidPoint);
                    triangleIndices.Enqueue(caMidPoint);
                    triangleIndices.Enqueue(bcMidPoint);
                    triangleIndices.Enqueue(cIndex);
                    triangleIndices.Enqueue(abMidPoint);
                    triangleIndices.Enqueue(bcMidPoint);
                    triangleIndices.Enqueue(caMidPoint);
                }
            }
        }

        private static int GetSubDevisionMidPointIndex(int aIndex, int bIndex, List<Point3D> points, Dictionary<PointIndicesCouple, int> subDevisionMidPointsCache)
        {
            PointIndicesCouple couple = new PointIndicesCouple(aIndex, bIndex);

            int midPointIndex;
            if (!subDevisionMidPointsCache.TryGetValue(couple, out midPointIndex))
            {
                midPointIndex = points.Count;
                subDevisionMidPointsCache.Add(couple, midPointIndex);

                Point3D a = points[aIndex];
                Point3D b = points[bIndex];
                Point3D midPoint = a + 0.5 * (b - a);
                Vector3D midPointVector = midPoint - Center;
                midPointVector = midPointVector * (Radius / midPointVector.Length);
                Point3D subDevisionMidPoint = Center + midPointVector;
                points.Add(subDevisionMidPoint);
            }

            return midPointIndex;
        }

        private static MeshGeometry3D GenerateSmoothGeometry(List<Point3D> points, Queue<int> triangleIndices, Point[] textureCoordinates)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            for (int i = 0; i < points.Count; i++)
            {
                Point3D point = points[i];
                Point texturePoint = textureCoordinates[i];
                mesh.Positions.Add(point);
                mesh.TextureCoordinates.Add(texturePoint);
                Vector3D normal = point - Center;
                normal.Normalize();
                mesh.Normals.Add(normal);
            }

            while (triangleIndices.Count > 0)
            {
                mesh.TriangleIndices.Add(triangleIndices.Dequeue());
            }

            return mesh;
        }

        private static MeshGeometry3D GenerateSharpGeometry(List<Point3D> points, Queue<int> triangleIndices, Point[] textureCoordinates)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            while (triangleIndices.Count > 0)
            {
                int index = triangleIndices.Dequeue();
                Point3D point = points[index];
                Point texturePoint = textureCoordinates[index];
                mesh.Positions.Add(point);
                mesh.TextureCoordinates.Add(texturePoint);
            }

            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                mesh.TriangleIndices.Add(i);
            }

            return mesh;
        }

        private static void SplitTrianglesOnTextureMappingBoundaryAndGetTextureCoordinates(List<Point3D> points, Queue<int> triangleIndices, out Point[] textureCoordinates)
        {
            int initialTrianglesIndicesCount = triangleIndices.Count;
            Dictionary<int, Point> pointIndexToTextureCoordinate = new Dictionary<int, Point>();
            Dictionary<int, PointIndexWithMeridianAngle> zeroMeridianVertexToDuplicatedVertex = new Dictionary<int, PointIndexWithMeridianAngle>();
            Dictionary<PointIndicesCouple, PointIndexWithMeridianAngle[]> coupleToSplitMidpoints = new Dictionary<PointIndicesCouple, PointIndexWithMeridianAngle[]>();

            Action<int, double> enqueueTriangleIndexWithTexture = (pointIndex, meridianAngle) =>
                {
                    triangleIndices.Enqueue(pointIndex);

                    if (!pointIndexToTextureCoordinate.ContainsKey(pointIndex))
                    {
                        Point texture = RotationalShape.GetTextureCoordinate(meridianAngle, points[pointIndex].Z, -Radius, Radius);
                        pointIndexToTextureCoordinate.Add(pointIndex, texture);
                    }
                };

            for (int i = 0; i < initialTrianglesIndicesCount; i+=3)
            {
                int[] currentTriangleIndices = { triangleIndices.Dequeue(), triangleIndices.Dequeue(), triangleIndices.Dequeue() };
                Point3D[] currentTrianglePoints = { points[currentTriangleIndices[0]], points[currentTriangleIndices[1]], points[currentTriangleIndices[2]] };
                double[] currentMeridianAngles = { GetMeridianAngle(currentTrianglePoints[0]), GetMeridianAngle(currentTrianglePoints[1]), GetMeridianAngle(currentTrianglePoints[2]) };

                bool hasModifiedCurrentTriangle = false;
                int zeroIndex = GeodesicSphere.IndexOfZeroMeridianAngle(currentMeridianAngles);

                if (zeroIndex > -1)
                {
                    bool hasPointInNegativeSemiplane, hasPointInPositiveSemiplane, hasPolePoint, allPointsHaveNonNegativeXCoordinate;
                    GeodesicSphere.GetCurrentTriangleVerticesInformation(currentTrianglePoints, out hasPolePoint, out hasPointInNegativeSemiplane, out hasPointInPositiveSemiplane, out allPointsHaveNonNegativeXCoordinate);
                    bool shouldSplitTriangleTextures = (hasPolePoint && hasPointInPositiveSemiplane && hasPointInNegativeSemiplane && allPointsHaveNonNegativeXCoordinate)
                        || (!hasPolePoint && hasPointInNegativeSemiplane && hasPointInPositiveSemiplane);

                    if (shouldSplitTriangleTextures)
                    { 
                        PointIndexWithMeridianAngle[] triangle = new PointIndexWithMeridianAngle[]
                        {
                            new PointIndexWithMeridianAngle(currentTriangleIndices[zeroIndex], currentMeridianAngles[zeroIndex]),
                            new PointIndexWithMeridianAngle(currentTriangleIndices[(zeroIndex + 1) % 3], currentMeridianAngles[(zeroIndex + 1) % 3]),
                            new PointIndexWithMeridianAngle(currentTriangleIndices[(zeroIndex + 2) % 3], currentMeridianAngles[(zeroIndex + 2) % 3]),
                        };
                        GeodesicSphere.SplitTriangleWithZeroPointTextureChanges
                            (triangle, points, zeroMeridianVertexToDuplicatedVertex, coupleToSplitMidpoints, pointIndexToTextureCoordinate, enqueueTriangleIndexWithTexture);
                    }
                    else if (hasPointInNegativeSemiplane || hasPolePoint)
                    {
                        GeodesicSphere.AddTriangleWithZeroPointTextureChanges
                            (currentTrianglePoints, currentMeridianAngles, hasPointInNegativeSemiplane, currentTriangleIndices, points, zeroMeridianVertexToDuplicatedVertex, enqueueTriangleIndexWithTexture);
                    }

                    hasModifiedCurrentTriangle = shouldSplitTriangleTextures || hasPointInNegativeSemiplane || hasPolePoint;
                }

                if (!hasModifiedCurrentTriangle)
                {
                    GeodesicSphere.AddTriangleWithTextures(currentTriangleIndices, currentMeridianAngles, enqueueTriangleIndexWithTexture);
                }
            }

            textureCoordinates = new Point[points.Count];

            for (int i = 0; i < points.Count; i++)
            {
                textureCoordinates[i] = pointIndexToTextureCoordinate[i];
            }
        }
  
        private static void AddTriangleWithZeroPointTextureChanges(Point3D[] currentTrianglePoints, double[] currentMeridianAngles, bool hasPointInNegativeSemiplane, int[] currentTriangleIndices,
            List<Point3D> points, Dictionary<int, PointIndexWithMeridianAngle> zeroMeridianVertexToDuplicatedVertex, Action<int, double> enqueueTriangleIndexWithTexture)
        {
            for (int index = 0; index < 3; index++)
            {
                bool isPolePoint = currentTrianglePoints[index].X.IsZero() && currentTrianglePoints[index].Y.IsZero();
                bool isZeroMeridianPoint = currentMeridianAngles[index].IsZero();

                if (isZeroMeridianPoint)
                {
                    PointIndexWithMeridianAngle pointWithAngle;

                    if (isPolePoint)
                    {
                        double nextAngle = currentMeridianAngles[(index + 1) % 3];
                        double lastAngle = currentMeridianAngles[(index + 2) % 3];

                        double angle;
                        if ((nextAngle.IsZero() || lastAngle.IsZero()))
                        {
                            angle = hasPointInNegativeSemiplane ? RotationalShape.FullCircleAngleInRadians : 0;
                        }
                        else
                        {
                            angle = Math.Min(nextAngle, lastAngle);
                        }

                        if (angle.IsZero())
                        {
                            pointWithAngle = new PointIndexWithMeridianAngle(currentTriangleIndices[index], angle);
                        }
                        else
                        {
                            points.Add(currentTrianglePoints[index]);
                            pointWithAngle = new PointIndexWithMeridianAngle(points.Count - 1, angle);
                        }
                    }
                    else if (hasPointInNegativeSemiplane)
                    {
                        pointWithAngle = GeodesicSphere.GetZeroAngleOppositeAngleDuplicatePoint(currentTriangleIndices[index], points, zeroMeridianVertexToDuplicatedVertex);
                    }
                    else
                    {
                        pointWithAngle = new PointIndexWithMeridianAngle(currentTriangleIndices[index], 0);
                    }

                    enqueueTriangleIndexWithTexture(pointWithAngle.Index, pointWithAngle.Angle);
                }
                else
                {
                    enqueueTriangleIndexWithTexture(currentTriangleIndices[index], currentMeridianAngles[index]);
                }
            }
        }

        private static void GetCurrentTriangleVerticesInformation(IEnumerable<Point3D> currentTrianglePoints,
            out bool hasPolePoint, out bool hasNegativeSemiplanePoint, out bool hasPositiveSemiplanePoint, out bool allPointsHaveNonNegativeXCoordinate)
        {
            hasNegativeSemiplanePoint = false;
            hasPositiveSemiplanePoint = false;
            hasPolePoint = false;
            allPointsHaveNonNegativeXCoordinate = true;

            foreach (Point3D point in currentTrianglePoints)
            {
                hasNegativeSemiplanePoint |= point.Y.IsLessThan(0);
                hasPositiveSemiplanePoint |= point.Y.IsGreaterThan(0);
                hasPolePoint |= (point.X.IsZero() && point.Y.IsZero());
                allPointsHaveNonNegativeXCoordinate &= point.X.IsGreaterThanOrEqualTo(0);
            }
        }
  
        private static void SplitTriangleWithZeroPointTextureChanges(PointIndexWithMeridianAngle[] triangle, List<Point3D> points, Dictionary<int, PointIndexWithMeridianAngle> zeroMeridianVertexToDuplicatedVertex,
            Dictionary<PointIndicesCouple, PointIndexWithMeridianAngle[]> coupleToSplitMidpoints, Dictionary<int, Point> pointIndexToTextureCoordinate, Action<int, double> enqueueTriangleIndexWithTexture)
        {
            PointIndexWithMeridianAngle duplicatePointIndex = GeodesicSphere.GetZeroAngleOppositeAngleDuplicatePoint(triangle[0].Index, points, zeroMeridianVertexToDuplicatedVertex);
            PointIndexWithMeridianAngle[] midPoints = GeodesicSphere.GetMidpointsIndices(triangle[1].Index, triangle[2].Index, points, coupleToSplitMidpoints, pointIndexToTextureCoordinate);
            GeodesicSphere.SplitAndEnqueueTriangle(points, triangle, midPoints, duplicatePointIndex, enqueueTriangleIndexWithTexture);
        }

        private static void SplitAndEnqueueTriangle(List<Point3D> points, PointIndexWithMeridianAngle[] triangle, PointIndexWithMeridianAngle[] midPoints,
            PointIndexWithMeridianAngle duplicatePointIndex, Action<int, double> enqueueTriangleIndexWithTexture)
        {
            int[] firstTriangleIndices, secondTriangleIndices;
            double[] firstTriangleMeridianAngles, secondTriangleMeridianAngles;
            PointIndexWithMeridianAngle zeroAnglePoint = triangle[0];

            if (points[triangle[1].Index].Y > 0)
            {
                firstTriangleIndices = new int[] { zeroAnglePoint.Index, triangle[1].Index, midPoints[0].Index };
                secondTriangleIndices = new int[] { midPoints[1].Index, triangle[2].Index, duplicatePointIndex.Index };
                firstTriangleMeridianAngles = new double[] { zeroAnglePoint.Angle, triangle[1].Angle, midPoints[0].Angle };
                secondTriangleMeridianAngles = new double[] { midPoints[1].Angle, triangle[2].Angle, duplicatePointIndex.Angle };
            }
            else
            {
                firstTriangleIndices = new int[] { duplicatePointIndex.Index, triangle[1].Index, midPoints[1].Index };
                secondTriangleIndices = new int[] { midPoints[0].Index, triangle[2].Index, zeroAnglePoint.Index };
                firstTriangleMeridianAngles = new double[] { duplicatePointIndex.Angle, triangle[1].Angle, midPoints[1].Angle };
                secondTriangleMeridianAngles = new double[] { midPoints[0].Angle, triangle[2].Angle, zeroAnglePoint.Angle };
            }

            GeodesicSphere.AddTriangleWithTextures(firstTriangleIndices, firstTriangleMeridianAngles, enqueueTriangleIndexWithTexture);
            GeodesicSphere.AddTriangleWithTextures(secondTriangleIndices, secondTriangleMeridianAngles, enqueueTriangleIndexWithTexture);
        }
  
        private static void AddTriangleWithTextures(int[] currentTriangleIndices, double[] currentMeridianAngles, Action<int, double> enqueueTriangleIndexWithTexture)
        {
            for (int index = 0; index < 3; index++)
            {
                enqueueTriangleIndexWithTexture(currentTriangleIndices[index], currentMeridianAngles[index]);
            }
        }

        private static PointIndexWithMeridianAngle[] GetMidpointsIndices(int firstNonZeroIndex, int secondNonZeroIndex, List<Point3D> points,
            Dictionary<PointIndicesCouple, PointIndexWithMeridianAngle[]> coupleToSplitMidpoints, Dictionary<int, Point> pointIndexToTextureCoordinate)
        {
            PointIndicesCouple couple = new PointIndicesCouple(firstNonZeroIndex, secondNonZeroIndex);

            PointIndexWithMeridianAngle[] midPoints;
            if (!coupleToSplitMidpoints.TryGetValue(couple, out midPoints))
            {
                Point3D midPoint = points[firstNonZeroIndex] + 0.5 * (points[secondNonZeroIndex] - points[firstNonZeroIndex]);
                midPoints = new PointIndexWithMeridianAngle[] 
                { 
                    new PointIndexWithMeridianAngle(points.Count, 0), 
                    new PointIndexWithMeridianAngle(points.Count + 1, RotationalShape.FullCircleAngleInRadians) 
                };
                pointIndexToTextureCoordinate.Add(midPoints[0].Index, RotationalShape.GetTextureCoordinate(midPoints[0].Angle, midPoint.Z, -Radius, Radius));
                pointIndexToTextureCoordinate.Add(midPoints[1].Index, RotationalShape.GetTextureCoordinate(midPoints[1].Angle, midPoint.Z, -Radius, Radius));
                points.Add(midPoint);
                points.Add(midPoint);

                coupleToSplitMidpoints.Add(couple, midPoints);
            }

            return midPoints;
        }

        private static PointIndexWithMeridianAngle GetZeroAngleOppositeAngleDuplicatePoint(int zeroAngleIndex, List<Point3D> points, Dictionary<int, PointIndexWithMeridianAngle> zeroMeridianVertexToDuplicatedVertex)
        {
            PointIndexWithMeridianAngle duplicatePointIndex;
            if (!zeroMeridianVertexToDuplicatedVertex.TryGetValue(zeroAngleIndex, out duplicatePointIndex))
            {
                duplicatePointIndex = new PointIndexWithMeridianAngle(points.Count, RotationalShape.FullCircleAngleInRadians);
                points.Add(points[zeroAngleIndex]);
                zeroMeridianVertexToDuplicatedVertex.Add(zeroAngleIndex, duplicatePointIndex);
            }

            return duplicatePointIndex;
        }

        private static int IndexOfZeroMeridianAngle(double[] currentMeridianAngles)
        {
            for (int i = 0; i < currentMeridianAngles.Length; i++)
            {
                if (currentMeridianAngles[i].IsZero())
                {
                    return i;
                }
            }

            return -1;
        }

        private static double GetMeridianAngle(Point3D point)
        {
            Vector projection = new Vector(point.X, point.Y);
            double angleInRadians;

            if (projection.LengthSquared.IsZero())
            {
                angleInRadians = 0;
            }
            else
            {
                double angleInDegrees = Vector.AngleBetween(ZeroMeridianDirection, projection);
                angleInRadians = angleInDegrees.DegreesToRadians();

                if (angleInRadians < 0)
                {
                    angleInRadians = RotationalShape.FullCircleAngleInRadians + angleInRadians;
                }
            }

            return angleInRadians;
        }
    }
}
