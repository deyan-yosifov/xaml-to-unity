using System;
using System.Collections.Generic;
using UnityEngine;

namespace CAGD.Utilities
{
    public static class GeometryHelper
    {
        public static Vector3 GetBoundingRectangleCenter(IEnumerable<Vector3> points, Vector3 iDirection, Vector3 jDirection, Vector3 kDirection)
        {
            float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;
            Vector3 coordinateCenter = new Vector3();

            foreach (Vector3 point in points)
            {
                Vector3 directionVector = point - coordinateCenter;
                float x = Vector3.Dot(iDirection, directionVector);
                float y = Vector3.Dot(jDirection, directionVector);
                float z = Vector3.Dot(kDirection, directionVector);

                minX = Math.Min(minX, x);
                minY = Math.Min(minY, y);
                minZ = Math.Min(minZ, z);
                maxX = Math.Max(maxX, x);
                maxY = Math.Max(maxY, y);
                maxZ = Math.Max(maxZ, z);
            }

            float rotatedCenterX = (minX + maxX) / 2;
            float rotatedCenterY = (minY + maxY) / 2;
            float rotatedCenterZ = (minZ + maxZ) / 2;

            Vector3 center = coordinateCenter + iDirection * rotatedCenterX + jDirection * rotatedCenterY + kDirection * rotatedCenterZ;

            return center;
        }

        public static Vector2 IntersectLines(Vector2 firstPoint, Vector2 firstVector, Vector2 secondPoint, Vector2 secondVector)
        {
            Vector2 secondNormal = new Vector2(-secondVector.y, secondVector.x);
            Vector2 connection = secondPoint - firstPoint;
            float t = Vector2.Dot(connection, secondNormal) / Vector2.Dot(firstVector, secondNormal);
            Vector2 intersection = firstPoint + t * firstVector;

            return intersection;
        }
    }
}
