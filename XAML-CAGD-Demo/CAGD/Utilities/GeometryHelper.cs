using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;

namespace CAGD.Utilities
{
    public static class GeometryHelper
    {
        public static Point3D GetBoundingRectangleCenter(IEnumerable<Point3D> points, Vector3D iDirection, Vector3D jDirection, Vector3D kDirection)
        {
            double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;
            Point3D coordinateCenter = new Point3D();

            foreach (Point3D point in points)
            {
                Vector3D directionVector = point - coordinateCenter;
                double x = Vector3D.DotProduct(iDirection, directionVector);
                double y = Vector3D.DotProduct(jDirection, directionVector);
                double z = Vector3D.DotProduct(kDirection, directionVector);

                minX = Math.Min(minX, x);
                minY = Math.Min(minY, y);
                minZ = Math.Min(minZ, z);
                maxX = Math.Max(maxX, x);
                maxY = Math.Max(maxY, y);
                maxZ = Math.Max(maxZ, z);
            }

            double rotatedCenterX = (minX + maxX) / 2;
            double rotatedCenterY = (minY + maxY) / 2;
            double rotatedCenterZ = (minZ + maxZ) / 2;

            Point3D center = coordinateCenter + iDirection * rotatedCenterX + jDirection * rotatedCenterY + kDirection * rotatedCenterZ;

            return center;
        }

        public static Rect3D GetBoundingRectangle(IEnumerable<Point3D> points)
        {
            double minX = double.MaxValue, minY = double.MaxValue, minZ = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue, maxZ = double.MinValue;

            foreach (Point3D point in points)
            {
                minX = Math.Min(minX, point.X);
                minY = Math.Min(minY, point.Y);
                minZ = Math.Min(minZ, point.Z);
                maxX = Math.Max(maxX, point.X);
                maxY = Math.Max(maxY, point.Y);
                maxZ = Math.Max(maxZ, point.Z);
            }

            Rect3D boundingRectangle = new Rect3D(minX, minY, minZ, maxX - minX, maxY - minY, maxZ - minZ);

            return boundingRectangle;
        }
    }
}
