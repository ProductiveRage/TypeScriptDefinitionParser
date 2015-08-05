using System.Collections.Immutable;
using System.Linq;

namespace TypeScriptDefinitionParser.Parsers
{
    public static class CommonTerminators
    {
        public static readonly ImmutableList<char> UntilWhiteSpace =
            Enumerable.Range(0, char.MaxValue)
                .Select(c => (char)c)
                .Where(c => char.IsWhiteSpace(c))
                .ToImmutableList();

        public static readonly ImmutableList<char> UntilPunctuation =
            Enumerable.Range(0, char.MaxValue)
                .Select(c => (char)c)
                .Where(c => char.IsPunctuation(c))
                .ToImmutableList();

        public static readonly ImmutableList<char> UntilWhiteSpaceOrPunctuation = UntilWhiteSpace.AddRange(UntilPunctuation);
    }
}
