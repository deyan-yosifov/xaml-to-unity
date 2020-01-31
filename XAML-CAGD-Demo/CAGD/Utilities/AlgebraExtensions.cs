using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CAGD.Utilities
{
    public static class AlgebraExtensions
    {
        public const double Epsilon = 1E-8;
        private const double DegreesToRadiansConstant = Math.PI / 180;

        public static double DegreesToRadians(this double degrees)
        {
            return degrees * DegreesToRadiansConstant;
        }

        public static double RadiansToDegrees(this double radians)
        {
            return radians / DegreesToRadiansConstant;
        }

        public static bool IsZero(this double a, double epsilon = Epsilon)
        {
            return Math.Abs(a) < epsilon;
        }

        public static bool IsEqualTo(this double a, double b, double epsilon = Epsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }

        public static bool IsGreaterThan(this double a, double b, double epsilon = Epsilon)
        {
            return !a.IsLessThanOrEqualTo(b, epsilon);
        }

        public static bool IsLessThan(this double a, double b, double epsilon = Epsilon)
        {
            return !a.IsGreaterThanOrEqualTo(b, epsilon);
        }

        public static bool IsGreaterThanOrEqualTo(this double a, double b, double epsilon = Epsilon)
        {
            return a > b || a.IsEqualTo(b, epsilon);
        }

        public static bool IsLessThanOrEqualTo(this double a, double b, double epsilon = Epsilon)
        {
            return a < b || a.IsEqualTo(b, epsilon);
        }

        public static bool IsInteger(this double a, double epsilon = Epsilon)
        {
            return a.IsEqualTo(Math.Round(a), epsilon);
        }

        public static Point Plus(this Point first, Point second)
        {
            return new Point(first.X + second.X, first.Y + second.Y);
        }

        public static Point Minus(this Point first, Point second)
        {
            return new Point(first.X - second.X, first.Y - second.Y);
        }

        public static Point MultiplyBy(this Point a, double number)
        {
            return new Point(a.X * number, a.Y * number);
        }

        public static double MultiplyBy(this Point a, Point other)
        {
            return a.X * other.X + a.Y * other.Y;
        }

        public static double Length(this Point a)
        {
            return Math.Sqrt(a.MultiplyBy(a));
        }

        public static bool IsZero(this Point a, double epsilon = Epsilon)
        {
            return a.Length() < epsilon;
        }

        public static Point UnitVector(this Point a)
        {
            if (a.IsZero())
            {
                throw new ArgumentException("Cannot calculate unit vector of a point with zero length!");
            }

            return a.MultiplyBy(1 / a.Length());
        }

        public static Matrix MultiplyBy(this Matrix m1, Matrix m2)
        {
            return new Matrix(
                (m1.M11 * m2.M11) + (m1.M12 * m2.M21), (m1.M11 * m2.M12) + (m1.M12 * m2.M22),
                (m1.M21 * m2.M11) + (m1.M22 * m2.M21), (m1.M21 * m2.M12) + (m1.M22 * m2.M22),
                (m1.OffsetX * m2.M11) + (m1.OffsetY * m2.M21) + m2.OffsetX, (m1.OffsetX * m2.M12) + (m1.OffsetY * m2.M22) + m2.OffsetY);
        }

        public static Matrix TranslateMatrix(this Matrix m, double deltaX, double deltaY)
        {
            return m.MultiplyBy(new Matrix(1, 0, 0, 1, deltaX, deltaY));
        }

        public static Rect Transform(this Matrix matrix, Rect rect)
        {
            return new Rect(matrix.Transform(new Point(rect.Left, rect.Top)), matrix.Transform(new Point(rect.Right, rect.Bottom)));
        }

        public static Matrix ScaleMatrix(this Matrix m, double scaleX, double scaleY)
        {
            return m.ScaleMatrixAt(scaleX, scaleY, 0, 0);
        }

        public static Matrix ScaleMatrixAt(this Matrix m, double scaleX, double scaleY, double centerX, double centerY)
        {
            return m.MultiplyBy(new Matrix(scaleX, 0, 0, scaleY, 0, 0).GetTransformationAt(centerX, centerY));
        }

        public static Matrix RotateMatrix(this Matrix m, double angleInDegrees)
        {
            return m.RotateMatrixAt(angleInDegrees, 0, 0);
        }

        public static Matrix RotateMatrixAt(this Matrix m, double angleInDegrees, double centerX, double centerY)
        {
            double radians = angleInDegrees * Math.PI / 180;
            double sin = Math.Sin(radians);
            double cos = Math.Cos(radians);

            return m.MultiplyBy(new Matrix(cos, sin, -sin, cos, 0, 0).GetTransformationAt(centerX, centerY));
        }

        public static Matrix InverseMatrix(this Matrix m)
        {
            var det = (m.M11 * m.M22) - (m.M12 * m.M21);
            var det1 = 1 / det;
            return new Matrix(m.M22 * det1, -m.M12 * det1, -m.M21 * det1, m.M11 * det1, ((m.M21 * m.OffsetY) - (m.OffsetX * m.M22)) * det1, ((m.OffsetX * m.M12) - (m.M11 * m.OffsetY)) * det1);
        }

        public static Vector3D ToVector(this Point3D point)
        {
            return new Vector3D(point.X, point.Y, point.Z);
        }

        public static Point3D ToPoint(this Vector3D vector)
        {
            return new Point3D(vector.X, vector.Y, vector.Z);
        }

        public static bool IsColinearWithOpositeDirection(this Vector3D a, Vector3D b, double epsilon = Epsilon)
        {
            if (a.IsColinear(b, epsilon))
            {
                double dotProduct = Vector3D.DotProduct(a, b);

                return dotProduct.IsLessThanOrEqualTo(0, epsilon);
            }

            return false;
        }

        public static bool IsColinearWithSameDirection(this Vector3D a, Vector3D b, double epsilon = Epsilon)
        {
            if (a.IsColinear(b, epsilon))
            {
                return a.IsSameSemiSpaceDirection(b, epsilon);
            }

            return false;
        }

        public static bool IsColinear(this Vector3D a, Vector3D b, double epsilon = Epsilon)
        {
            double crossProductLength = Vector3D.CrossProduct(a, b).LengthSquared;

            return crossProductLength.IsZero(epsilon);
        }

        public static bool IsSameSemiSpaceDirection(this Vector3D a, Vector3D b, double epsilon = Epsilon)
        {
            double dotProduct = Vector3D.DotProduct(a, b);

            return dotProduct.IsGreaterThanOrEqualTo(0, epsilon);
        }

        public static Point3D GetBarycentricCoordinates(this Point point, Point a, Point b, Point c)
        {
            double iX = b.X - a.X;
            double iY = b.Y - a.Y;
            double jX = c.X - a.X;
            double jY = c.Y - a.Y;
            double determinant = (iX * jY) - (iY * jX);

            if (determinant == 0)
            {
                throw new ArgumentException("Triangle vertices cannot be colinear!");
            }

            double scaledPointRadiusVectorX = (point.X - a.X) / determinant;
            double scaledPointRadiusVectorY = (point.Y - a.Y) / determinant;
            double ijCoordinatesX = (scaledPointRadiusVectorX * jY) - (scaledPointRadiusVectorY * jX);
            double ijCoordinatesY = (scaledPointRadiusVectorY * iX) - (scaledPointRadiusVectorX * iY);
            Point3D barycentricCoordinates = new Point3D(1 - ijCoordinatesX - ijCoordinatesY, ijCoordinatesX, ijCoordinatesY);

            return barycentricCoordinates;
        }

        public static bool AreBarycentricCoordinatesInsideTriangle(this Point3D barycentricCoordinates)
        {
            return barycentricCoordinates.X.IsGreaterThanOrEqualTo(0) &&
                barycentricCoordinates.Y.IsGreaterThanOrEqualTo(0) &&
                barycentricCoordinates.Z.IsGreaterThanOrEqualTo(0);
        }

        private static Matrix GetTransformationAt(this Matrix zeroCenteredTransform, double centerX, double centerY)
        {
            double offsetX = zeroCenteredTransform.OffsetX + (1 - zeroCenteredTransform.M11) * centerX - zeroCenteredTransform.M21 * centerY;
            double offsetY = zeroCenteredTransform.OffsetY + (1 - zeroCenteredTransform.M22) * centerY - zeroCenteredTransform.M12 * centerX;

            return new Matrix(zeroCenteredTransform.M11, zeroCenteredTransform.M12, zeroCenteredTransform.M21, zeroCenteredTransform.M22, offsetX, offsetY);
        }
    }
}
