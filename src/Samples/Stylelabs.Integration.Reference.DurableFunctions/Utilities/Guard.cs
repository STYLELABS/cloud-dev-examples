using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Stylelabs.Integration.Reference.DurableFunctions.Utilities
{
    public static class Guard
    {
        /// <summary>
        /// Checks if the specified <paramref name="value"/> is <see langword="null"/>.
        /// </summary>
        /// <param name="argument">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <exception cref="ArgumentNullException">Thrown when the specified
        ///     <paramref name="value"/> is <see langword="null"/>.</exception>
        public static void NotNull(string argument, object value)
        {
            if (value == null)
                throw new ArgumentNullException(
                        // yes, stupidly enough for ArgumentNullException,
                        // the parameter name is first
                        argument,
                        $"Argument '{argument}' cannot be null.");
        }

        /// <summary>
        /// Checks if the specified <paramref name="value"/> is <see langword="null"/>
        /// or an empty string.
        /// </summary>
        /// <param name="argument">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <exception cref="ArgumentNullException">Thrown when the specified
        ///     <paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the specified
        ///     <paramref name="value"/> is an empty string.</exception>
        public static void NotNullOrEmpty(string argument, string value)
        {
            NotNull(argument, value);

            if (value == string.Empty)
                throw new ArgumentException(
                        $"Argument '{argument}' cannot be an empty string.",
                        argument);
        }

        /// <summary>
        /// Checks if the specified <paramref name="value"/> is <see langword="null"/>,
        /// an empty string or white space string.
        /// </summary>
        /// <param name="argument">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <exception cref="ArgumentNullException">Thrown when the specified
        ///     <paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the specified
        ///     <paramref name="value"/> is an empty or whitespace string.</exception>
        public static void NotNullOrWhiteSpace(string argument, string value)
        {
            NotNull(argument, value);

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(
                        $"Argument '{argument}' cannot be an empty or white space string.",
                        argument);
        }

        /// <summary>
        /// Checks if the specified collection is null or empty.
        /// </summary>
        /// <typeparam name="T">Type of the collection.</typeparam>
        /// <param name="argument">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <exception cref="ArgumentNullException">Thrown when the specified
        ///     <paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the specified
        ///     <paramref name="value"/> is an emtpy collection.</exception>
        public static void NotNullOrEmptyCollection<T>(string argument, T value)
            where T : IEnumerable
        {
            NotNull(argument, value);

            if (!value.Cast<object>().Any())
                throw new ArgumentException(
                        $"Argument '{argument}' cannot be an empty collection.",
                        argument);
        }

        /// <summary>
        /// Checks if the specified collection is null or contains null values.
        /// </summary>
        /// <typeparam name="T">Type of the collection.</typeparam>
        /// <param name="argument">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <exception cref="ArgumentNullException">Thrown when the specified
        ///     <paramref name="value"/> is <see langword="null"/> or contain
        ///     null value.</exception>
        public static void NotNullEach<T>(string argument, T value)
            where T : IEnumerable
        {
            NotNull(argument, value);

            if (value.Cast<object>().Any(v => v == null))
                throw new ArgumentNullException(
                        $"Argument '{argument}' cannot be a collection with null values.",
                        argument);
        }

        /// <summary>
        /// Checks if the specified collection is null or contains null
        /// or empty strings.
        /// </summary>
        /// <param name="argument">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <exception cref="ArgumentNullException">Thrown when the specified
        ///     <paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the specified
        ///     <paramref name="value"/> is collection with null or empty
        ///     strings.</exception>
        public static void NotNullOrEmptyEach(string argument, IEnumerable<string> value)
        {
            NotNull(argument, value);

            if (value.Any(v => string.IsNullOrEmpty(v)))
                throw new ArgumentException(
                        $"Argument '{argument}' cannot be a collection with null or empty values.",
                        argument);
        }

        /// <summary>
        /// Checks if the specified collection is null or contains null
        /// or white space strings.
        /// </summary>
        /// <param name="argument">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <exception cref="ArgumentNullException">Thrown when the specified
        ///     <paramref name="value"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when the specified
        ///     <paramref name="value"/> is collection with null or
        ///     white space strings.</exception>
        public static void NotNullOrWhiteSpaceEach(string argument, IEnumerable<string> value)
        {
            NotNull(argument, value);

            if (value.Any(v => string.IsNullOrWhiteSpace(v)))
                throw new ArgumentException(
                        $"Argument '{argument}' cannot be a collection with null values.",
                        argument);
        }

        /// <summary>
        /// Checks if the specified integer is greater than a given minimum.
        /// Otherwise if the specified integer is lower or equal to given minimum, fail.
        /// </summary>
        /// <param name="argument">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <param name="minimum">The minimum.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        ///     the specified <paramref name="value"/> is smaller or equal to the
        ///     <paramref name="minimum"/>.</exception>
        public static void GreaterThan(string argument, int value, int minimum)
        {
            if (value <= minimum)
                throw new ArgumentOutOfRangeException(
                        argument,
                        value,
                        $"Argument '{argument}' must be greater than '{minimum}'");
        }

        /// <summary>
        /// Checks if the specified long is greater than a given minimum.
        /// Otherwise if the specified long is lower or equal to given minimum, fail.
        /// </summary>
        /// <param name="argument">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <param name="minimum">The minimum.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        ///     the specified <paramref name="value"/> is smaller or equal to
        ///     the <paramref name="minimum"/>.</exception>
        public static void GreaterThan(string argument, long value, long minimum)
        {
            if (value <= minimum)
                throw new ArgumentOutOfRangeException(
                        argument,
                        value,
                        $"Argument '{argument}' must be greater than '{minimum}'");
        }

        /// <summary>
        /// Checks if the specified double is greater than a given minimum.
        /// Otherwise if the specified double is lower or equal to given minimum, fail.
        /// </summary>
        /// <param name="argument">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <param name="minimum">The minimum.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        ///     the specified <paramref name="value"/> is smaller or equal to
        ///     the <paramref name="minimum"/>.</exception>
        public static void GreaterThan(string argument, double value, double minimum)
        {
            if (value <= minimum)
                throw new ArgumentOutOfRangeException(
                        argument,
                        value,
                        $"Argument '{argument}' must be greater than '{minimum}'");
        }

        /// <summary>
        /// Checks if the specified integer is inside the given range, otherwise fails.
        /// By default the range is inclusive.
        /// </summary>
        /// <param name="argument">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <param name="lowerBound">The lower bound, included by default.</param>
        /// <param name="upperBound">The upper bound, included by default.</param>
        /// <param name="includeLower">Indicates if the <paramref name="lowerBound"/>
        ///     is included in allowed range, defaults to <see langword="true"/></param>
        /// <param name="includeUpper">Indicates if the <paramref name="upperBound"/>
        ///     is included in allowed range, defaults to <see langword="true"/></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified
        ///     <paramref name="value"/> is outside of the given range
        ///     ( | [ <paramref name="lowerBound"/>, <paramref name="upperBound"/> ] | ).</exception>
        public static void InRange(
                string argument,
                int value,
                int lowerBound,
                int upperBound,
                bool includeLower = true,
                bool includeUpper = true
        )
        {
            if (
                (includeLower ? value < lowerBound : value <= lowerBound)
                || (includeUpper ? upperBound < value : upperBound <= value)
            )
            {
                var bra = includeLower ? "[" : "(";
                var ket = includeUpper ? "]" : ")";
                throw new ArgumentOutOfRangeException(
                        argument,
                        value,
                        $"Argument '{argument}' must be in range {bra}{lowerBound}, {upperBound}{ket}");
            }
        }

        /// <summary>
        /// Checks if the specified long is inside the given range, otherwise fails.
        /// By default the range is inclusive.
        /// </summary>
        /// <param name="argument">The name of the argument.</param>
        /// <param name="value">The value of the argument.</param>
        /// <param name="lowerBound">The lower bound, included by default.</param>
        /// <param name="upperBound">The upper bound, included by default.</param>
        /// <param name="includeLower">Indicates if the <paramref name="lowerBound"/>
        ///     is included in allowed range, defaults to <see langword="true"/></param>
        /// <param name="includeUpper">Indicates if the <paramref name="upperBound"/>
        ///     is included in allowed range, defaults to <see langword="true"/></param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the specified
        ///     <paramref name="value"/> is outside of the given range
        ///     ( | [ <paramref name="lowerBound"/>, <paramref name="upperBound"/> ] | ).</exception>
        public static void InRange(
                string argument,
                long value,
                long lowerBound,
                long upperBound,
                bool includeLower = true,
                bool includeUpper = true
        )
        {
            if (
                (includeLower ? value < lowerBound : value <= lowerBound)
                || (includeUpper ? upperBound < value : upperBound <= value)
            )
            {
                var bra = includeLower ? "[" : "(";
                var ket = includeUpper ? "]" : ")";
                throw new ArgumentOutOfRangeException(
                        argument,
                        value,
                        $"Argument '{argument}' must be in range {bra}{lowerBound}, {upperBound}{ket}");
            }
        }

        /// <summary>
        /// Checks if the specified string value is a valid guid.
        /// </summary>
        /// <param name="argument">name of the argument</param>
        /// <param name="value">string representation of the guid value</param>
        public static void AgainstInvalidGuid(string argument, string value)
        {
            Guid result;
            if (!Guid.TryParse(value, out result))
                throw new ArgumentException(
                        $"Argument '{argument}' must be a valid string representation of a Guid.",
                        argument);
        }
    }
}
