using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace TypeScriptDefinitionParser.ContentReaders
{
    public static class IReadStringContent_Extensions
    {
        public static Optional<IReadStringContent> StartMatching(this IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return Optional.For(reader);
        }

        public static Optional<MatchResult<string>> MatchAny(this IReadStringContent reader, ImmutableList<char> acceptableTerminators)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (acceptableTerminators == null)
                throw new ArgumentNullException(nameof(acceptableTerminators));

            var content = new StringBuilder();
            while (reader.Current != null)
            {
                if (acceptableTerminators.Contains(reader.Current.Value))
                {
                    if (content.Length > 0)
                        return MatchResult.New(content.ToString(), reader);
                    break;
                }
                content.Append(reader.Current);
                reader = reader.Next();
            }
            return null;
        }
    }
}
