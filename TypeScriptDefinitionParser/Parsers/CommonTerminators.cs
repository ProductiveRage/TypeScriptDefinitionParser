using System.Collections.Immutable;
using System.Linq;

namespace TypeScriptDefinitionParser.Parsers
{
    public static class CommonTerminators
    {
        public static readonly ImmutableList<char> UntilWhiteSpace =
            ImmutableList<char>.Empty.AddRange(
                Enumerable.Range(0, char.MaxValue).Select(c => (char)c).Where(c => char.IsWhiteSpace(c))
            );

        public static readonly ImmutableList<char> UntilPunctuation =
            ImmutableList<char>.Empty.AddRange(
                Enumerable.Range(0, char.MaxValue).Select(c => (char)c).Where(c => char.IsPunctuation(c))
            );

        public static readonly ImmutableList<char> UntilWhiteSpaceOrPunctuation =
            ImmutableList<char>.Empty
                .AddRange(UntilWhiteSpace)
                .AddRange(UntilPunctuation);
    }
}
