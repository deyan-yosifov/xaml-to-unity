using CAGD.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CAGD.Controls.Controls3D.Cameras
{
    public static class CameraHelper
    {
        private static readonly Vector3 WorldUp = Vector3.up;

        public static void Look(this Camera camera, Vector3 fromPoint, Vector3 toPoint, float rollAngleInDegrees)
        {
            Vector3 lookDirection = (toPoint - fromPoint).normalized;
            camera.transform.rotation = Quaternion.LookRotation(lookDirection, WorldUp) * Quaternion.AngleAxis(rollAngleInDegrees, lookDirection);
            camera.transform.position = fromPoint;
        }

        public static float GetUnityDistantPlaneWidth(Camera camera)
        {
            Ray centralRay = camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
            Vector3 unityDistantRectCenter = centralRay.GetPoint(1);
            Plane unityDistantPlane = new Plane(centralRay.direction, unityDistantRectCenter);

            Ray sideRay = camera.ViewportPointToRay(new Vector2(1, 0.5f));
            unityDistantPlane.Raycast(sideRay, out float distance);
            Vector3 sidePoint = sideRay.GetPoint(distance);

            float width = 2 * (sidePoint - unityDistantRectCenter).magnitude;

            return width;
        }

        public static Vector3 GetZoomToContentsCameraPosition(Camera camera, IEnumerable<Vector3> contentPoints)
        {
            if (!contentPoints.Any())
            {
                return camera.transform.position;
            }

            Vector3 i = camera.transform.right;
            Vector3 j = camera.transform.up;
            Vector3 k = camera.transform.forward;
            float unitWidth = CameraHelper.GetUnityDistantPlaneWidth(camera);
            float unitHeight = camera.pixelHeight * unitWidth / camera.pixelWidth;
            Vector3 leftSlopeNormal = i + (unitWidth / 2) * k;
            Vector3 rightSlopeNormal = -i + (unitWidth / 2) * k;
            Vector3 topSlopeNormal = j + (unitHeight / 2) * k;
            Vector3 bottomSlopeNormal = -j + (unitHeight / 2) * k;

            Vector3 leftMost, rightMost, topMost, bottomMost;
            CameraHelper.CalculateViewSlopesEndMostPoints(contentPoints, camera.transform.position, leftSlopeNormal, rightSlopeNormal, topSlopeNormal, bottomSlopeNormal,
                out leftMost, out rightMost, out topMost, out bottomMost);

            Vector3 center = new Vector3();

            Vector2 leftProjection = ProjectPointInPlane(leftMost, center, i, k);
            Vector2 rightProjection = ProjectPointInPlane(rightMost, center, i, k);
            Vector2 leftSlope = new Vector2(-unitWidth / 2, 1);
            Vector2 rightSlope = new Vector2(unitWidth / 2, 1);
            Vector2 iDirectionIntersection = GeometryHelper.IntersectLines(leftProjection, leftSlope, rightProjection, rightSlope);

            Vector2 topProjection = ProjectPointInPlane(topMost, center, j, k);
            Vector2 bottomProjection = ProjectPointInPlane(bottomMost, center, j, k);
            Vector2 topSlope = new Vector2(-unitHeight / 2, 1);
            Vector2 bottomSlope = new Vector2(unitHeight / 2, 1);
            Vector2 jDirectionIntersection = GeometryHelper.IntersectLines(topProjection, topSlope, bottomProjection, bottomSlope);

            Vector3 x = iDirectionIntersection.x * i;
            Vector3 y = jDirectionIntersection.x * j;
            Vector3 z = Math.Min(iDirectionIntersection.y, jDirectionIntersection.y) * k;

            Vector3 cameraPosition = center + x + y + z;

            return cameraPosition;
        }

        private static Vector2 ProjectPointInPlane(Vector3 point, Vector3 center, Vector3 i, Vector3 j)
        {
            Vector3 radiusVector = point - center;
            float x = Vector3.Dot(i, radiusVector);
            float y = Vector3.Dot(j, radiusVector);
            Vector2 projection = new Vector2(x, y);

            return projection;
        }

        public static Vector2 GetPointOnUnityDistantPlane(Vector2 screenPoint, Camera perspectiveCamera)
        {
            float unityPlaneWidth = GetUnityDistantPlaneWidth(perspectiveCamera);
            float scale = unityPlaneWidth / perspectiveCamera.pixelWidth;

            Vector2 coordinateSystemCenter = new Vector2(perspectiveCamera.pixelWidth / 2, perspectiveCamera.pixelHeight / 2);
            Vector2 transformedPoint = scale * (screenPoint - coordinateSystemCenter);

            return transformedPoint;
        }

        private static void CalculateViewSlopesEndMostPoints(IEnumerable<Vector3> points, Vector3 cameraPosition,
            Vector3 leftSlopeNormal, Vector3 rightSlopeNormal, Vector3 topSlopeNormal, Vector3 bottomSlopeNormal,
            out Vector3 leftMostPoint, out Vector3 rightMostPoint, out Vector3 topMostPoint, out Vector3 bottomMostPoint)
        {
            leftMostPoint = rightMostPoint = topMostPoint = bottomMostPoint = cameraPosition;
            double leftMin = double.MaxValue, rightMin = double.MaxValue, topMin = double.MaxValue, bottomMin = double.MaxValue;

            foreach (Vector3 point in points)
            {
                Vector3 directionVector = point - cameraPosition;
                double left = Vector3.Dot(leftSlopeNormal, directionVector);
                double right = Vector3.Dot(rightSlopeNormal, directionVector);
                double top = Vector3.Dot(topSlopeNormal, directionVector);
                double bottom = Vector3.Dot(bottomSlopeNormal, directionVector);

                if (left < leftMin)
                {
                    leftMin = left;
                    leftMostPoint = point;
                }

                if (right < rightMin)
                {
                    rightMin = right;
                    rightMostPoint = point;
                }

                if (top < topMin)
                {
                    topMin = top;
                    topMostPoint = point;
                }

                if (bottom < bottomMin)
                {
                    bottomMin = bottom;
                    bottomMostPoint = point;
                }
            }
        }
    }
}
