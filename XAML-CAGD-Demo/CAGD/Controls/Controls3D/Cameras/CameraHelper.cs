using CAGD.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;

namespace CAGD.Controls.Controls3D.Cameras
{
    public class CameraHelper
    {
        public static readonly Point InfinityPoint = new Point(double.PositiveInfinity, double.PositiveInfinity);
        
        public static void GetCameraPropertiesOnLook(Point3D fromPoint, Point3D toPoint, double rollAngleInDegrees, out Point3D position, out Vector3D lookVector, out Vector3D upDirection)
        {
            position = fromPoint;
            lookVector = toPoint - fromPoint;
            Vector3D lookDirection = lookVector * 1;
            lookDirection.Normalize();

            if (lookDirection.Z == 1)
            {
                upDirection = new Vector3D(0, 1, 0);
            }
            else if (lookDirection.Z == -1)
            {
                upDirection = new Vector3D(0, -1, 0);
            }
            else
            {
                upDirection = new Vector3D(0, 0, 1);
            }

            if (rollAngleInDegrees != 0)
            {
                Matrix3D matrix = new Matrix3D();
                matrix.Rotate(new Quaternion(lookDirection, rollAngleInDegrees));
                upDirection = matrix.Transform(upDirection);
            }
        }

        public static void GetCameraLocalCoordinateVectors(Vector3D lookDirection, Vector3D upDirection, out Vector3D canvasX, out Vector3D canvasY, out Vector3D canvasZ)
        {
            canvasZ = lookDirection * 1;
            canvasZ.Normalize();

            canvasX = Vector3D.CrossProduct(canvasZ, upDirection);
            canvasX.Normalize();

            canvasY = Vector3D.CrossProduct(canvasZ, canvasX);
        }

        public static bool TryGetLineSegmentInVisibleSemiPlane(Point3D start3D, Point3D end3D, Size viewportSize, PerspectiveCamera camera,
            out Point start, out Point end)
        {
            start = CameraHelper.InfinityPoint;
            end = CameraHelper.InfinityPoint;
            bool isStartVisible = CameraHelper.IsPointInVisibleSemiSpace(start3D, camera);
            bool isEndVisible = CameraHelper.IsPointInVisibleSemiSpace(end3D, camera);
            bool isLineSegmentInVisibleSemiSpace = isStartVisible || isEndVisible;

            if (isLineSegmentInVisibleSemiSpace)
            {
                start = isStartVisible ?
                    CameraHelper.GetPointFromPoint3DWhenInVisibleSemiSpace(start3D, viewportSize, camera) :
                    CameraHelper.GetFirstPointFromVisibleSemiSpace(start3D, end3D - start3D, viewportSize, camera);
                end = isEndVisible ?
                    CameraHelper.GetPointFromPoint3DWhenInVisibleSemiSpace(end3D, viewportSize, camera) :
                    CameraHelper.GetFirstPointFromVisibleSemiSpace(start3D, end3D - start3D, viewportSize, camera);
            }

            return isLineSegmentInVisibleSemiSpace;
        }

        public static bool TryGetVisiblePointFromPoint3D(Point3D point3D, Size viewportSize, PerspectiveCamera camera, out Point point)
        {
            point = CameraHelper.InfinityPoint;

            if (CameraHelper.IsPointInVisibleSemiSpace(point3D, camera))
            {
                point = CameraHelper.GetPointFromPoint3DWhenInVisibleSemiSpace(point3D, viewportSize, camera);

                return point.X.IsGreaterThanOrEqualTo(0) && point.X.IsLessThanOrEqualTo(viewportSize.Width) &&
                    point.Y.IsGreaterThanOrEqualTo(0) && point.Y.IsLessThanOrEqualTo(viewportSize.Height);
            }

            return false;
        }

        internal static bool TryGetVisiblePointFromPoint3D(Point3D point3D, Size viewportSize, OrthographicCamera camera, out Point point)
        {
            throw new NotImplementedException();
        }

        public static Vector3D GetLookDirectionFromPoint(Point pointOnViewport, Size viewportSize, PerspectiveCamera camera)
        {
            Point3D point = GetPoint3DOnUnityDistantPlane(pointOnViewport, viewportSize, camera);
            Vector3D lookDirection = point - camera.Position;
            lookDirection.Normalize();

            return lookDirection;
        }

        public static Point3D GetPoint3DOnUnityDistantPlane(Point pointOnViewport, Size viewportSize, PerspectiveCamera camera)
        {
            Vector3D i, j, k;
            GetCameraLocalCoordinateVectors(camera.LookDirection, camera.UpDirection, out i, out j, out k);
            Point point = GetPointOnUnityDistantPlane(pointOnViewport, viewportSize, camera.FieldOfView);
            Point3D point3D = GetPoint3DOnUnityDistantPlane(point, camera.Position, i, j, k);

            return point3D;
        }

        public static Point3D GetPoint3DOnUnityDistantPlane(Point unityDistantPlanePoint, Point3D cameraPosition, Vector3D cameraI, Vector3D cameraJ, Vector3D cameraK)
        {
            Point3D point3D = (cameraPosition + cameraK) + (unityDistantPlanePoint.X * cameraI) + (unityDistantPlanePoint.Y * cameraJ);

            return point3D;
        }

        public static Point GetPointOnUnityDistantPlane(Point pointOnViewport, Size viewportSize, double fieldOfViewInDegrees)
        {
            double unityPlaneWidth = GetUnityDistantPlaneWidth(fieldOfViewInDegrees);
            double scale = unityPlaneWidth / viewportSize.Width;

            Point coordinateSystemCenter = new Point(viewportSize.Width / 2, viewportSize.Height / 2);
            Point transformedPoint = pointOnViewport.Minus(coordinateSystemCenter).MultiplyBy(scale);

            return transformedPoint;
        }

        public static double GetUnityDistantPlaneWidth(double fieldOfViewInDegrees)
        {
            double radians = fieldOfViewInDegrees.DegreesToRadians();
            double unityPlaneWidth = 2 * Math.Tan(radians / 2);

            return unityPlaneWidth;
        }

        public static Point3D GetZoomToContentsCameraPosition(PerspectiveCamera camera, Size viewportSize, IEnumerable<Point3D> contentPoints)
        {
            if (!contentPoints.Any())
            {
                return camera.Position;
            }

            Vector3D i, j, k;
            CameraHelper.GetCameraLocalCoordinateVectors(camera.LookDirection, camera.UpDirection, out i, out j, out k);
            double unitWidth = CameraHelper.GetUnityDistantPlaneWidth(camera.FieldOfView);
            double unitHeight = viewportSize.Height * unitWidth / viewportSize.Width;            
            Vector3D leftSlopeNormal = i + (unitWidth / 2) * k;
            Vector3D rightSlopeNormal = -i + (unitWidth / 2) * k;
            Vector3D topSlopeNormal = j + (unitHeight / 2) * k;
            Vector3D bottomSlopeNormal = -j + (unitHeight / 2) * k;

            Point3D leftMost, rightMost, topMost, bottomMost;
            CameraHelper.CalculateViewSlopesEndMostPoints(contentPoints, camera.Position, leftSlopeNormal, rightSlopeNormal, topSlopeNormal, bottomSlopeNormal,
                out leftMost, out rightMost, out topMost, out bottomMost);

            Point3D center = new Point3D();

            Point leftProjection = ProjectPointInPlane(leftMost, center, i, k);
            Point rightProjection = ProjectPointInPlane(rightMost, center, i, k);
            Vector leftSlope = new Vector(-unitWidth / 2, 1);
            Vector rightSlope = new Vector(unitWidth / 2, 1);
            Point iDirectionIntersection = IntersectionsHelper.IntersectLines(leftProjection, leftSlope, rightProjection, rightSlope);

            Point topProjection = ProjectPointInPlane(topMost, center, j, k);
            Point bottomProjection = ProjectPointInPlane(bottomMost, center, j, k);
            Vector topSlope = new Vector(-unitHeight / 2, 1);
            Vector bottomSlope = new Vector(unitHeight / 2, 1);
            Point jDirectionIntersection = IntersectionsHelper.IntersectLines(topProjection, topSlope, bottomProjection, bottomSlope);

            Vector3D x = iDirectionIntersection.X * i;
            Vector3D y = jDirectionIntersection.X * j;
            Vector3D z = Math.Min(iDirectionIntersection.Y, jDirectionIntersection.Y) * k;

            Point3D cameraPosition = center + x + y + z;

            return cameraPosition;
        }

        private static Point ProjectPointInPlane(Point3D point, Point3D center, Vector3D i, Vector3D j)
        {
            Vector3D radiusVector = point - center;
            double x = Vector3D.DotProduct(i, radiusVector);
            double y = Vector3D.DotProduct(j, radiusVector);
            Point projection = new Point(x, y);

            return projection;
        }

        private static void CalculateViewSlopesEndMostPoints(IEnumerable<Point3D> points, Point3D cameraPosition,
            Vector3D leftSlopeNormal, Vector3D rightSlopeNormal, Vector3D topSlopeNormal, Vector3D bottomSlopeNormal,
            out Point3D leftMostPoint, out Point3D rightMostPoint, out Point3D topMostPoint, out Point3D bottomMostPoint)
        {
            leftMostPoint = rightMostPoint = topMostPoint = bottomMostPoint = cameraPosition;
            double leftMin = double.MaxValue, rightMin = double.MaxValue, topMin = double.MaxValue, bottomMin = double.MaxValue;

            foreach (Point3D point in points)
            {
                Vector3D directionVector = point - cameraPosition;
                double left = Vector3D.DotProduct(leftSlopeNormal, directionVector);
                double right = Vector3D.DotProduct(rightSlopeNormal, directionVector);
                double top = Vector3D.DotProduct(topSlopeNormal, directionVector);
                double bottom = Vector3D.DotProduct(bottomSlopeNormal, directionVector);

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

        private static Point GetFirstPointFromVisibleSemiSpace(Point3D linePoint, Vector3D lineVector, Size viewportSize, PerspectiveCamera camera)
        {
            Vector3D nearestPlaneNormal = camera.LookDirection;
            nearestPlaneNormal.Normalize();
            Point3D nearestPlanePoint = camera.Position + nearestPlaneNormal;
            Point3D intersection = IntersectionsHelper.IntersectLineAndPlane(linePoint, lineVector, nearestPlanePoint, nearestPlaneNormal);
            Point projectedPoint = CameraHelper.GetPointFromPoint3DWhenInVisibleSemiSpace(intersection, viewportSize, camera);

            return projectedPoint;
        }

        private static bool IsPointInVisibleSemiSpace(Point3D point, PerspectiveCamera camera)
        {
            Vector3D projectionPlaneNormal = camera.LookDirection;
            projectionPlaneNormal.Normalize();
            Point3D nearestPlanePoint = camera.Position + projectionPlaneNormal * camera.NearPlaneDistance;
            bool isInVisibleSemiSpace = Vector3D.DotProduct(projectionPlaneNormal, point - nearestPlanePoint).IsGreaterThanOrEqualTo(0);

            return isInVisibleSemiSpace;
        }

        private static Point GetPointFromPoint3DWhenInVisibleSemiSpace(Point3D point3D, Size viewportSize, PerspectiveCamera camera)
        {
            Vector3D i, j, k;
            GetCameraLocalCoordinateVectors(camera.LookDirection, camera.UpDirection, out i, out j, out k);
            double unityPlaneWidth = GetUnityDistantPlaneWidth(camera.FieldOfView);
            double scale = viewportSize.Width / unityPlaneWidth;
            Point3D actualProjectionPlaneTopLeftCenter = camera.Position + k * scale + i * (-viewportSize.Width / 2) + j * (-viewportSize.Height / 2);
            Vector3D intersectionDirection = point3D - camera.Position;
            Point3D intersection = IntersectionsHelper.IntersectLineAndPlane(camera.Position, intersectionDirection, actualProjectionPlaneTopLeftCenter, k);
            Vector3D projectedVectorDirection = intersection - actualProjectionPlaneTopLeftCenter;
            double x = Vector3D.DotProduct(i, projectedVectorDirection);
            double y = Vector3D.DotProduct(j, projectedVectorDirection);

            return new Point(x, y);
        }
    }
}
