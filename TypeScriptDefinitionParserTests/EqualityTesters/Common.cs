using System;
using System.Collections.Immutable;
using System.Linq;
using TypeScriptDefinitionParser;
using TypeScriptDefinitionParser.ContentReaders;
using TypeScriptDefinitionParserTests.Parsers;

namespace TypeScriptDefinitionParserTests.EqualityTesters
{
    public static class Common
    {
        public static bool DoOptionalMatchResultsMatch<T>(Optional<MatchResult<T>> x, Optional<MatchResult<T>> y)
            => DoOptionalValuesMatch(x, y, (r1, r2) => DoMatchResultsMatch(r1, r2));

        public static bool DoOptionalMatchResultsMatch<T>(Optional<MatchResult<T>> x, Optional<MatchResult<T>> y, Func<T, T, bool> equalityComparer)
            => DoOptionalValuesMatch(x, y, (r1, r2) => DoMatchResultsMatch(r1, r2, equalityComparer));

        public static bool DoMatchResultsMatch<T>(MatchResult<T> x, MatchResult<T> y) => DoMatchResultsMatch(x, y, DefaultEqualityComparer);

        public static bool DoMatchResultsMatch<T>(MatchResult<T> x, MatchResult<T> y, Func<T, T, bool> equalityComparer)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));

            return
                equalityComparer(x.Result, y.Result) &&
                (x.Reader.ReadRemainingContent() == y.Reader.ReadRemainingContent());
        }

        public static bool DoOptionalValuesMatch<T>(Optional<T> x, Optional<T> y) => DoOptionalValuesMatch(x, y, DefaultEqualityComparer);

        public static bool DoOptionalValuesMatch<T>(Optional<T> x, Optional<T> y, Func<T, T, bool> equalityComparer)
        {
            if (!x.IsDefined && !y.IsDefined)
                return true;
            else if (!x.IsDefined || !y.IsDefined)
                return false;

            return equalityComparer(x.Value, y.Value);
        }

        public static bool DoImmutableListsMatch<T>(ImmutableList<T> x, ImmutableList<T> y) => DoImmutableListsMatch(x, y, DefaultEqualityComparer);

        public static bool DoImmutableListsMatch<T>(ImmutableList<T> x, ImmutableList<T> y, Func<T, T, bool> equalityComparer)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));

            if (x.Count != y.Count)
                return false;

            return x
                .Zip(y, equalityComparer)
                .All(valuesMatch => valuesMatch);
        }

        private static bool DefaultEqualityComparer<T>(T x, T y)
        {
            if ((x == null) && (y == null))
                return true;
            else if ((x == null) || (y == null))
                return false;
            return x.Equals(y);
        }
    }
}
