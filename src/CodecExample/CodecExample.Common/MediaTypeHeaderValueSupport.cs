// Copyright (c) 2021 Nathan Allen-Wagner. All rights reserved.
// Licensed under the MIT license. See License.txt in root of repo.

// Portions of code are based on .NET Source, shared under MIT license
// Primarily based on: https://source.dot.net/#Microsoft.Net.Http.Headers/MediaTypeHeaderValue.cs,3107d1c70fb0fe7f


using System;
using System.Linq;
using Microsoft.Net.Http.Headers;

namespace CodecExample.Common
{
    public static class MediaTypeHeaderValueSupport
    {

        /// <summary>
        /// A revised implementation that excludes the charset parameter.
        /// </summary>
        /// <param name="left">The mediatype that should be a subset of <paramref name="right"/>.</param>
        /// <param name="right">The mediatype that holds the full set.</param>
        /// <returns>
        /// Returns true if the type, subtype, and all parameters of the left mediatype are present, and match the right media type.
        /// </returns>
        /// <remarks>
        /// Supports wildcards in the type/subtype.
        /// Ignores the charset, q, and * parameter names.
        /// Examples:
        /// - "text/plain" is a subset of "text/plain" "text/*" and "*/*". 
        /// - "*/*" is a subset only of "*/*".
        /// </remarks>
        public static bool IsSubsetOf(MediaTypeHeaderValue left, MediaTypeHeaderValue right)
        {
            if (left == null || right == null)
            {
                return false;
            }

            // "text/plain" is a subset of "text/plain", "text/*" and "*/*". "*/*" is a subset only of "*/*".
            return MatchesType(left, right) &&
                MatchesSubtype(left, right) &&
                ContainsAllParameters(left, right);
        }

        /// <summary>
        /// A strong match on media types that requires all parameters to be present and match on both sides,
        /// except for selected parameters to exclude from matching logic.
        /// </summary>
        /// <param name="left">The mediatype that should be a subset of <paramref name="right"/>.</param>
        /// <param name="right">The mediatype that holds the full set.</param>
        /// <returns>
        /// Returns true if the type, subtype, and all parameters of the left mediatype are present, and match the right media type.
        /// </returns>
        /// <remarks>
        /// Supports wildcards in the type/subtype.
        /// Ignores the charset, q, and * parameter names.
        /// Examples:
        /// - "text/plain" is a subset of "text/plain" "text/*" and "*/*". 
        /// - "*/*" is a subset only of "*/*".
        /// </remarks>
        public static bool IsMatch(MediaTypeHeaderValue left, MediaTypeHeaderValue right)
        {
            if (left == null || right == null)
            {
                return false;
            }

            // "text/plain" is a subset of "text/plain", "text/*" and "*/*". "*/*" is a subset only of "*/*".
            return MatchesType(left, right) &&
                MatchesSubtype(left, right) &&
                ContainsAllParameters(left, right) &&
                ContainsAllParameters(right, left);
        }

        public static bool MatchesType(MediaTypeHeaderValue left, MediaTypeHeaderValue right)
        {
            return right.MatchesAllTypes ||
                right.Type.Equals(left.Type, StringComparison.OrdinalIgnoreCase);
        }

        public static bool MatchesSubtype(MediaTypeHeaderValue left, MediaTypeHeaderValue right)
        {
            if (right.MatchesAllSubTypes)
            {
                return true;
            }

            if (right.Suffix.HasValue)
            {
                if (left.Suffix.HasValue)
                {
                    return MatchesSubtypeWithoutSuffix(left, right) && MatchesSubtypeSuffix(left, right);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                // If this subtype or suffix matches the subtype of the set,
                // it is considered a subtype.
                // Ex: application/json > application/val+json
                return MatchesEitherSubtypeOrSuffix(left, right);
            }
        }

        public static bool MatchesEitherSubtypeOrSuffix(MediaTypeHeaderValue left, MediaTypeHeaderValue right)
        {
            return right.SubType.Equals(left.SubType, StringComparison.OrdinalIgnoreCase) ||
                right.SubType.Equals(left.Suffix, StringComparison.OrdinalIgnoreCase);
        }

        public static bool MatchesSubtypeWithoutSuffix(MediaTypeHeaderValue left, MediaTypeHeaderValue right)
        {
            return right.MatchesAllSubTypesWithoutSuffix ||
                right.SubTypeWithoutSuffix.Equals(left.SubTypeWithoutSuffix, StringComparison.OrdinalIgnoreCase);
        }

        public static bool MatchesSubtypeSuffix(MediaTypeHeaderValue left, MediaTypeHeaderValue right)
        {
            // We don't have support for wildcards on suffixes alone (e.g., "application/entity+*")
            // because there's no clear use case for it.
            return right.Suffix.Equals(left.Suffix, StringComparison.OrdinalIgnoreCase);
        }


        public static bool ContainsAllParameters(MediaTypeHeaderValue left, MediaTypeHeaderValue right)
        {
            foreach (var leftParam in left.Parameters)
            {
                if (leftParam.Name.Equals("q", StringComparison.OrdinalIgnoreCase))
                {
                    // "q" and later parameters are not involved in media type matching. Quoting the RFC: The first
                    // "q" parameter (if any) separates the media-range parameter(s) from the accept-params.
                    continue;
                }

                if (leftParam.Name.Equals("*", StringComparison.OrdinalIgnoreCase))
                {
                    // A parameter named "*" has no effect on media type matching, as it is only used as an indication
                    // that the entire media type string should be treated as a wildcard.
                    continue;
                }

                if (leftParam.Name.Equals("charset", StringComparison.OrdinalIgnoreCase))
                {
                    // Charset is appended by the system. Ignore it here for mediatype comparisons.
                    continue;
                }

                var parameterFound = false;
                foreach (var rightParam in right.Parameters)
                {
                    if (leftParam.Name.Equals(rightParam.Name, StringComparison.OrdinalIgnoreCase)
                        && leftParam.Value.Equals(rightParam.Value, StringComparison.Ordinal))
                    {
                        parameterFound = true;
                        break;
                    }
                }

                if (!parameterFound)
                {
                    return false;
                }

            }

            return true;
        }

        /// <summary>
        /// Determines whether the current <see cref="ReadOnlyMediaTypeHeaderValue"/> contains a wildcard.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this <see cref="ReadOnlyMediaTypeHeaderValue"/> contains a wildcard; otherwise <c>false</c>.
        /// </returns>
        public static bool HasWildcard(MediaTypeHeaderValue value)
        {
            return MatchesAllTypes(value) ||
                MatchesAllSubTypesWithoutSuffix(value) ||
                value.Parameters.Any(v => v.Name.Equals("*", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets whether this <see cref="ReadOnlyMediaTypeHeaderValue"/> matches all types.
        /// </summary>
        public static bool MatchesAllTypes(MediaTypeHeaderValue value) => value.Type.Equals("*", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets whether this <see cref="ReadOnlyMediaTypeHeaderValue"/> matches all subtypes, ignoring any structured syntax suffix.
        /// </summary>
        /// <example>
        /// For the media type <c>"application/*+json"</c>, this property is <c>true</c>.
        /// </example>
        /// <example>
        /// For the media type <c>"application/vnd.example+json"</c>, this property is <c>false</c>.
        /// </example>
        public static bool MatchesAllSubTypesWithoutSuffix(MediaTypeHeaderValue value) => value.SubTypeWithoutSuffix.Equals("*", StringComparison.OrdinalIgnoreCase);

    }

}
