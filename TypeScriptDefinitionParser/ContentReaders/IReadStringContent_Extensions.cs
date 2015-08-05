using System;
using System.Collections.Generic;
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

        public static Optional<MatchResult<string>> MatchAnythingUntil(this IReadStringContent reader, ImmutableList<char> acceptableTerminators)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (acceptableTerminators == null)
                throw new ArgumentNullException(nameof(acceptableTerminators));

            var matchedCharacters = reader
                .Read()
                .TakeWhile(c => !acceptableTerminators.Contains(c.Character));
            if (!matchedCharacters.Any())
                return null;
            var readerAfterMatch = matchedCharacters.Last().NextReader;
            if (readerAfterMatch.Current == null)
                return null;
            return MatchResult.New(
                string.Join("", matchedCharacters.Select(c => c.Character)),
                readerAfterMatch
            );

            /* TODO: ??
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
            */
        }

        private static IEnumerable<CharacterAndNextReader> Read(this IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            while (true)
            {
                var currentCharacter = reader.Current;
                if (currentCharacter == null)
                    yield break;

                var nextReader = reader.Next();
                yield return new CharacterAndNextReader(currentCharacter.Value, nextReader);
                reader = nextReader;
            }
        }

        private sealed class CharacterAndNextReader
        {
            public CharacterAndNextReader(char character, IReadStringContent nextReader)
            {
                if (nextReader == null)
                    throw new ArgumentNullException(nameof(nextReader));

                Character = character;
                NextReader = nextReader;
            }

            public char Character { get; }
            public IReadStringContent NextReader { get; }
        }
    }
}
