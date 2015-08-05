using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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

        /// <summary>
        /// At least one terminator character must be specified, otherwise all of the remaining content would be returned
        /// </summary>
        public static Optional<MatchResult<string>> MatchAnythingUntil(this IReadStringContent reader, ImmutableList<char> acceptableTerminators)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (acceptableTerminators == null)
                throw new ArgumentNullException(nameof(acceptableTerminators));
            if (!acceptableTerminators.Any())
                throw new ArgumentException("at least one terminator character must be specified", nameof(acceptableTerminators));

            var matchedCharacters = reader
                .Read()
                .TakeWhile(c => !acceptableTerminators.Contains(c.Character));

            // Ensure some content was actually read (if not then return a no-data result)
            if (!matchedCharacters.Any())
                return null;

            // Ensure that the content was correctly terminated (the content may have been exhausted, which is not the same as identifying content that
            // was explicitly terminated by one of a particular set of characters - if no terminator was met then it a no-data result must be returned)
            var readerAfterMatch = matchedCharacters.Last().NextReader;
            if (readerAfterMatch.Current == null)
                return null;

            return MatchResult.New(
                string.Join("", matchedCharacters.Select(c => c.Character)),
                readerAfterMatch
            );
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
