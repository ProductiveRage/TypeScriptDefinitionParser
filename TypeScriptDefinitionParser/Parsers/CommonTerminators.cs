using System.Collections.Immutable;
using System.Linq;

namespace TypeScriptDefinitionParser.Parsers
{
    public static class CommonTerminators
    {
        public static readonly ImmutableHashSet<char> UntilWhiteSpace =
            Enumerable.Range(0, char.MaxValue)
                .Select(c => (char)c)
                .Where(c => char.IsWhiteSpace(c))
                .ToImmutableHashSet();

        public static readonly ImmutableHashSet<char> UntilPunctuation =
            Enumerable.Range(0, char.MaxValue)
                .Select(c => (char)c)
                .Where(c => char.IsPunctuation(c))
                .ToImmutableHashSet();

        public static readonly ImmutableHashSet<char> UntilWhiteSpaceOrPunctuation =
            UntilWhiteSpace.Union(UntilPunctuation).ToImmutableHashSet();
    }
}
