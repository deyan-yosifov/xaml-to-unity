using CAGD.Utilities;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Shapes
{
    public class RotationalShape : ShapeBase
    {
        public const double FullCircleAngleInRadians = 2 * Math.PI;

        public RotationalShape(Point[][] sectionsInXZPlaneCounterClockSide, int meridiansCount, bool isSmooth)
        {
            base.GeometryModel.Geometry = this.GenerateGeometry(sectionsInXZPlaneCounterClockSide, meridiansCount, isSmooth);
            this.GeometryModel.Geometry.Freeze();
        }

        internal static Point GetTextureCoordinate(double positiveAngleInRadians, double parallelZ, double minZ, double maxZ)
        {
            double u = positiveAngleInRadians / FullCircleAngleInRadians;
            double v = (maxZ - parallelZ) / (maxZ - minZ);

            return new Point(u, v);
        }

        private static int AddPointWithTextureCoordinateToGeometry(MeshGeometry3D geometry, Point3D point, double angleInRadians, double minZ, double maxZ)
        {
            geometry.Positions.Add(point);
            geometry.TextureCoordinates.Add(GetTextureCoordinate(angleInRadians, point.Z, minZ, maxZ));

            return geometry.Positions.Count - 1;
        }
        
        private Geometry3D GenerateGeometry(Point[][] sectionsInXYPlane, int meridiansCount, bool isSmooth)
        {
            double minZ, maxZ;
            Point3D[][] sectionsPoints;
            CalculateSectionsPoints(sectionsInXYPlane, out sectionsPoints, out minZ, out maxZ);
            
            MeshGeometry3D geometry = isSmooth ? GenerateSmoothEdgesGeometry(sectionsPoints, minZ, maxZ, meridiansCount) : GenerateSharpEdgesGeometry(sectionsPoints, minZ, maxZ, meridiansCount);
            
            return geometry;
        }

        private MeshGeometry3D GenerateSharpEdgesGeometry(Point3D[][] sectionsPoints, double minZ, double maxZ, int meridiansCount)
        {
            MeshGeometry3D geometry = new MeshGeometry3D();

            for (int section = 0; section < sectionsPoints.Length; section++)
            {
                this.GenerateSharpEdgesGeometry(geometry, sectionsPoints[section], minZ, maxZ, meridiansCount);
            }

            return geometry;
        }

        private void GenerateSharpEdgesGeometry(MeshGeometry3D geometry, Point3D[] sectionPoints, double minZ, double maxZ, int meridiansCount)
        {
            Func<int, bool> isOnAxis = (parallelIndex) => { return sectionPoints[parallelIndex].X == 0; };

            Point3D[] firstPoints = sectionPoints.ToArray();
            Point3D[] secondPoints = new Point3D[firstPoints.Length];

            for (int meridian = 0; meridian < meridiansCount; meridian++)
            {
                double previousAngleInRadians = GetMeridianRotationInRadians(meridian, meridiansCount);
                double angleInRadians = GetMeridianRotationInRadians(meridian + 1, meridiansCount);

                for (int parallel = 0; parallel < sectionPoints.Length; parallel++)
                {
                    secondPoints[parallel] = RotateSectionPoint(sectionPoints[parallel], angleInRadians);
                }

                Func<Point3D, double, int> addPointToGeometry = (point, meridianAngleInRadians) => { return AddPointWithTextureCoordinateToGeometry(geometry, point, meridianAngleInRadians, minZ, maxZ); };
                Action<Point3D, Point3D, Point3D> addPointsToGeomety = (first, second, third) =>
                {
                    AddTriangle(geometry, addPointToGeometry(first, previousAngleInRadians), addPointToGeometry(second, previousAngleInRadians), addPointToGeometry(third, angleInRadians));
                };

                for (int parallel = 0; parallel < sectionPoints.Length - 1; parallel++)
                {
                    if (isOnAxis(parallel))
                    {
                        if (!isOnAxis(parallel + 1))
                        {
                            addPointsToGeomety(firstPoints[parallel], firstPoints[parallel + 1], secondPoints[parallel + 1]);
                        }
                    }
                    else if (isOnAxis(parallel + 1))
                    {
                        addPointsToGeomety(firstPoints[parallel], firstPoints[parallel + 1], secondPoints[parallel]);
                    }
                    else
                    {
                        addPointsToGeomety(firstPoints[parallel], firstPoints[parallel + 1], secondPoints[parallel + 1]);
                        AddTriangle(geometry, geometry.Positions.Count - 3, geometry.Positions.Count - 1, addPointToGeometry(secondPoints[parallel], angleInRadians));
                    }
                }

                Point3D[] swap = firstPoints;
                firstPoints = secondPoints;
                secondPoints = swap;
            }
        }

        private MeshGeometry3D GenerateSmoothEdgesGeometry(Point3D[][] sectionsPoints, double minZ, double maxZ, int meridiansCount)
        {
            MeshGeometry3D geometry = new MeshGeometry3D();

            for (int section = 0; section < sectionsPoints.Length; section++)
            {
                this.GenerateSmoothEdgesGeometry(geometry, sectionsPoints[section], minZ, maxZ, meridiansCount);
            }

            return geometry;
        }

        private void GenerateSmoothEdgesGeometry(MeshGeometry3D geometry, Point3D[] sectionPoints, double minZ, double maxZ, int meridiansCount)
        {
            Func<int, bool> isOnAxis = (parallelIndex) => { return sectionPoints[parallelIndex].X == 0; };

            int[] firstPoints = new int[sectionPoints.Length];
            int[] secondPoints = new int[firstPoints.Length];
            for (int i = 0; i < firstPoints.Length; i++)
            {
                firstPoints[i] = AddPointWithNormalToGeometry(geometry, sectionPoints, i, 0, minZ, maxZ);
            }

            for (int meridian = 0; meridian < meridiansCount; meridian++)
            {
                double angleInRadians = GetMeridianRotationInRadians(meridian + 1, meridiansCount);

                for (int parallel = 0; parallel < sectionPoints.Length; parallel++)
                {
                    secondPoints[parallel] = AddPointWithNormalToGeometry(geometry, sectionPoints, parallel, angleInRadians, minZ, maxZ);
                }

                for (int parallel = 0; parallel < sectionPoints.Length - 1; parallel++)
                {
                    if (isOnAxis(parallel))
                    {
                        if (!isOnAxis(parallel + 1))
                        {
                            AddTriangle(geometry, firstPoints[parallel], firstPoints[parallel + 1], secondPoints[parallel + 1]);
                        }
                    }
                    else if (isOnAxis(parallel + 1))
                    {
                        AddTriangle(geometry, firstPoints[parallel], firstPoints[parallel + 1], secondPoints[parallel]);
                    }
                    else
                    {
                        AddTriangle(geometry, firstPoints[parallel], firstPoints[parallel + 1], secondPoints[parallel + 1]);
                        AddTriangle(geometry, firstPoints[parallel], secondPoints[parallel + 1], secondPoints[parallel]);
                    }
                }

                int[] swap = firstPoints;
                firstPoints = secondPoints;
                secondPoints = swap;
            }
        }

        private static int AddPointWithNormalToGeometry(MeshGeometry3D geometry, Point3D[] sectionPoints, int pointIndex, double meridianAngleInRadians, double minZ, double maxZ)
        {
            Point3D normalVectorInSectionPlane = CalculateNormalVectorInSectionPlane(sectionPoints, pointIndex);
            Point3D rotatedNormalVector = RotateSectionPoint(normalVectorInSectionPlane, meridianAngleInRadians);
            geometry.Normals.Add(new Vector3D(rotatedNormalVector.X, rotatedNormalVector.Y, rotatedNormalVector.Z));

            Point3D sectionPoint = sectionPoints[pointIndex];
            Point3D rotatedPoint = RotateSectionPoint(sectionPoint, meridianAngleInRadians);

            return AddPointWithTextureCoordinateToGeometry(geometry, rotatedPoint, meridianAngleInRadians, minZ, maxZ);
        }

        private static Point3D CalculateNormalVectorInSectionPlane(Point3D[] sectionPoints, int pointIndex)
        {
            Vector3D tangent = new Vector3D();
            Point3D sectionPoint = sectionPoints[pointIndex];

            if (pointIndex > 0)
            {
                Vector3D previousTangent = sectionPoint - sectionPoints[pointIndex - 1];
                previousTangent.Normalize();
                tangent += previousTangent;
            }

            if (pointIndex < sectionPoints.Length - 1)
            {
                Vector3D nextTangent = sectionPoints[pointIndex + 1] - sectionPoint;
                nextTangent.Normalize();
                tangent += nextTangent;
            }

            tangent.Normalize();
            Point3D normalVectorInSectionPlane = new Point3D(-tangent.Z, 0, tangent.X);

            return normalVectorInSectionPlane;
        }

        private static Point3D RotateSectionPoint(Point3D sectionPoint, double angleInRadians)
        {
            if (sectionPoint.X == 0 || angleInRadians == 0 || (FullCircleAngleInRadians - angleInRadians).IsZero())
            {
                return sectionPoint;
            }
            else
            {
                return new Point3D(sectionPoint.X * Math.Cos(angleInRadians), sectionPoint.X * Math.Sin(angleInRadians), sectionPoint.Z);
            }
        }

        private static void CalculateSectionsPoints(Point[][] sectionsInXZPlane, out Point3D[][] sectionsPoints, out double minZ, out double maxZ)
        {
            minZ = double.MaxValue;
            maxZ = double.MinValue;
            sectionsPoints = new Point3D[sectionsInXZPlane.Length][];

            for(int section = 0; section < sectionsPoints.Length; section++)
            {
                Point[] sectionInXZPlane = sectionsInXZPlane[section];
                Point3D[] sectionPoints = new Point3D[sectionInXZPlane.Length];
                sectionsPoints[section] = sectionPoints;

                for (int i = 0; i < sectionPoints.Length; i++)
                {
                    Point point = sectionInXZPlane[i];
                    sectionPoints[i] = new Point3D(point.X, 0, point.Y);
                    minZ = Math.Min(minZ, point.Y);
                    maxZ = Math.Max(maxZ, point.Y);
                }
            }
        }

        private static double GetMeridianRotationInRadians(int meridian, int meridiansCount)
        {
            return (meridian / (double)meridiansCount) * FullCircleAngleInRadians;
        }

        private static void AddTriangle(MeshGeometry3D geometry, int first, int second, int third)
        {
            geometry.TriangleIndices.Add(first);
            geometry.TriangleIndices.Add(second);
            geometry.TriangleIndices.Add(third);
        }
    }
}
