using System;
using UnityEngine;

namespace CAGD.Utilities
{
    public static class AlgebraExtensions
    {
        public const float Epsilon = 1E-8f;

        public static float DegreesToRadians(this float degrees)
        {
            return degrees * Mathf.Deg2Rad;
        }

        public static float RadiansToDegrees(this float radians)
        {
            return radians / Mathf.Deg2Rad;
        }

        public static bool IsZero(this float a, float epsilon = Epsilon)
        {
            return Math.Abs(a) < epsilon;
        }

        public static bool IsEqualTo(this float a, float b, float epsilon = Epsilon)
        {
            return Math.Abs(a - b) < epsilon;
        }

        public static bool IsGreaterThan(this float a, float b, float epsilon = Epsilon)
        {
            return !a.IsLessThanOrEqualTo(b, epsilon);
        }

        public static bool IsLessThan(this float a, float b, float epsilon = Epsilon)
        {
            return !a.IsGreaterThanOrEqualTo(b, epsilon);
        }

        public static bool IsGreaterThanOrEqualTo(this float a, float b, float epsilon = Epsilon)
        {
            return a > b || a.IsEqualTo(b, epsilon);
        }

        public static bool IsLessThanOrEqualTo(this float a, float b, float epsilon = Epsilon)
        {
            return a < b || a.IsEqualTo(b, epsilon);
        }

        public static bool IsInteger(this float a, float epsilon = Epsilon)
        {
            return a.IsEqualTo(Mathf.Round(a), epsilon);
        }
    }
}
