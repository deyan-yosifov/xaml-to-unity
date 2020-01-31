using System.Windows;
using System.Windows.Media.Media3D;

namespace CAGD.Utilities
{
    public class IntersectionsHelper
    {
        public static IntersectionType FindIntersectionTypeBetweenLines(Point firstPoint, Vector firstVector, Point secondPoint, Vector secondVector)
        {
            double determinant = firstVector.X * secondVector.Y - firstVector.Y * secondVector.X;

            if (determinant.IsZero())
            {
                Vector points = secondPoint - firstPoint;
                double pointsDeterminant = points.X * firstVector.Y - points.Y * firstVector.X;

                if (pointsDeterminant.IsZero())
                {
                    return IntersectionType.InfinitePointSet;
                }
                else
                {
                    return IntersectionType.EmptyPointSet;
                }
            }
            else
            {
                return IntersectionType.SinglePointSet;
            }
        }

        public static IntersectionType FindIntersectionTypeBetweenLineAndPlane(Point3D linePoint, Vector3D lineVector, Point3D planePoint, Vector3D planeNormal)
        {
            double dotProduct = Vector3D.DotProduct(lineVector, planeNormal);

            if (dotProduct.IsZero())
            {
                Vector3D connection = planePoint - linePoint;
                double connectionDotProduct = Vector3D.DotProduct(connection, planeNormal);

                if (connectionDotProduct.IsZero())
                {
                    return IntersectionType.InfinitePointSet;
                }
                else
                {
                    return IntersectionType.EmptyPointSet;
                }
            }
            else
            {
                return IntersectionType.SinglePointSet;
            }
        }

        public static Point IntersectLines(Point firstPoint, Vector firstVector, Point secondPoint, Vector secondVector)
        {
            Vector secondNormal = new Vector(-secondVector.Y, secondVector.X);
            Vector connection = secondPoint - firstPoint;
            double t = Vector.Multiply(connection, secondNormal) / Vector.Multiply(firstVector, secondNormal);
            Point intersection = firstPoint + t * firstVector;

            return intersection;
        }

        public static bool AreLineSegmentsIntersecting(Point firstStart, Point firstEnd, Point secondStart, Point secondEnd)
        {
            double firstIntersectionProduct = Vector.CrossProduct(secondEnd - secondStart, firstStart - secondStart) * Vector.CrossProduct(secondEnd - secondStart, firstEnd - secondStart);
            double secondIntersectionProduct = Vector.CrossProduct(firstEnd - firstStart, secondStart - firstStart) * Vector.CrossProduct(firstEnd - firstStart, secondEnd - firstStart);

            if(firstIntersectionProduct == 0 && secondIntersectionProduct == 0)
            {
                Vector firstVector = firstEnd - firstStart;
                double length2 = firstVector.LengthSquared;
                double firstDot = Vector.Multiply(firstVector, secondStart - firstStart);
                double secondDot = Vector.Multiply(firstVector, secondEnd - firstStart);

                double tFirst = firstDot / length2;
                double tSecond = secondDot / length2;

                return (0 <= tFirst && tFirst <= 1) || (0 <= tSecond && tSecond <= 1) || (tFirst * tSecond <= 0);
            }
            else
            {
                bool isFirstIntersectingSecondLine = firstIntersectionProduct <= 0;
                bool isSecondIntersectionFirstLine = secondIntersectionProduct <= 0;

                return isFirstIntersectingSecondLine && isSecondIntersectionFirstLine;
            }
        }

        public static bool TryIntersectLineSegments(Point firstStart, Point firstEnd, Point secondStart, Point secondEnd, out Point intersection)
        {
            Vector firstVector = firstEnd - firstStart;
            Vector secondVector = secondEnd - secondStart;
            IntersectionType type = FindIntersectionTypeBetweenLines(firstStart, firstVector, secondStart, secondVector);

            if (type != IntersectionType.SinglePointSet)
            {
                intersection = new Point();
                return false;
            }

            intersection = IntersectLines(firstStart, firstVector, secondStart, secondVector);

            Vector firstDelta = intersection - firstStart;
            double tFirst = Vector.Multiply(firstDelta, firstVector) / firstVector.LengthSquared;

            Vector secondDelta = intersection - secondStart;
            double tSecond = Vector.Multiply(secondDelta, secondVector) / secondVector.LengthSquared;

            if (0 <= tFirst && tFirst <= 1 && 0 <= tSecond && tSecond <= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Point3D IntersectLineAndPlane(Point3D linePoint, Vector3D lineVector, Point3D planePoint, Vector3D planeNormal)
        {
            Vector3D connection = planePoint - linePoint;
            double t = Vector3D.DotProduct(connection, planeNormal) / Vector3D.DotProduct(lineVector, planeNormal);
            Point3D intersection = linePoint + t * lineVector;

            return intersection;
        }

        public static bool TryFindPlanesIntersectionLine(Point3D firstPlanePoint, Vector3D firstPlaneNormal, Point3D secondPlanePoint, Vector3D secondPlaneNormal, out Point3D linePoint, out Vector3D lineVector)
        {
            lineVector = Vector3D.CrossProduct(firstPlaneNormal, secondPlaneNormal);

            if (lineVector.LengthSquared.IsZero())
            {
                linePoint = new Point3D();
                lineVector = new Vector3D();

                return false;
            }

            lineVector.Normalize();
            Vector3D slope = Vector3D.CrossProduct(firstPlaneNormal, lineVector);
            linePoint = IntersectionsHelper.IntersectLineAndPlane(firstPlanePoint, slope, secondPlanePoint, secondPlaneNormal);

            return true;
        }
    }
}
