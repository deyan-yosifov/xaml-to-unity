using System;

namespace CAGD.Utilities
{
    public static class Guard
    {
        public static void ThrowExceptionIfTrue(bool value, string parameterName)
        {
            if (value)
            {
                throw new ArgumentException(string.Format("{0} must be false!", parameterName));
            }
        }

        public static void ThrowExceptionIfFalse(bool value, string parameterName)
        {
            if (!value)
            {
                throw new ArgumentException(string.Format("{0} must be true!", parameterName));
            }
        }

        public static void ThrowExceptionIfNotNull(object value, string parameterName)
        {
            if (value != null)
            {
                throw new ArgumentException(string.Format("{0} must be null!", parameterName));
            }
        }

        public static void ThrowExceptionIfNull(object value, string parameterName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void ThrowExceptionIfNotEqual(object value, object expectedValue, string parameterName)
        {
            if (!value.IsEqualTo(expectedValue))
            {
                throw new ArgumentOutOfRangeException(string.Format("{0} should be equal to {1}!", parameterName, expectedValue));
            }
        }

        public static void ThrowExceptionIfNotInRange<T>(T value, T minValue, T maxValue, string parameterName)
            where T : IComparable
        {
            Guard.ThrowExceptionIfNotInRange(value, minValue, maxValue, true, true, parameterName);
        }

        public static void ThrowExceptionIfNotInRange<T>(T value, T minValue, T maxValue, bool includeMin, bool includeMax, string parameterName)
            where T : IComparable
        {
            bool isMinFulfilled = includeMin ? value.CompareTo(minValue) >= 0 : value.CompareTo(minValue) > 0;
            bool isMaxFulfilled = includeMax ? value.CompareTo(maxValue) <= 0 : value.CompareTo(maxValue) < 0;
            bool areBothFulfilled = isMinFulfilled && isMaxFulfilled;

            if (!areBothFulfilled)
            {
                throw new ArgumentOutOfRangeException(string.Format("{0} should be in range {1}{2}; {3}{4}!",
                    parameterName,
                    includeMin ? "[" : "(",
                    minValue,
                    maxValue,
                    includeMax ? "]" : ")"));
            }
        }

        public static void ThrowExceptionIfLessThan<T>(T value, T minimumValue, string parameterName)
            where T : IComparable
        {
            if (value.CompareTo(minimumValue) < 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("{0} should not be less than {1}!", parameterName, minimumValue));
            }
        }

        public static void ThrowExceptionIfBiggerThan<T>(T value, T maximumValue, string parameterName)
            where T : IComparable
        {
            if (value.CompareTo(maximumValue) > 0)
            {
                throw new ArgumentOutOfRangeException(string.Format("{0} should not be bigger than {1}!", parameterName, maximumValue));
            }
        }

        public static void ThrowNotSupportedCameraException()
        {
            throw new NotSupportedException("Not supported camera type!");
        }
    }
}
