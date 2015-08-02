using System;
using System.Linq;
using System.Text;
using TypeScriptDefinitionParser.ContentReaders;

namespace TypeScriptDefinitionParser.Parsers
{
    public static class StandardParsers
    {
        public static Parser<char> Match(char character) =>
            reader =>
            {
                if (reader == null)
                    throw new ArgumentNullException(nameof(reader));

                if (reader.Current != character)
                    return Optional<MatchResult<char>>.Missing;

                return MatchResult.New(character, reader.Next());
            };

        public static Parser<string> Match(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("may not be null or blank", nameof(value));

            return reader =>
            {
                if (reader == null)
                    throw new ArgumentNullException(nameof(reader));

                var numberOfMatchedCharacters = value
                    .TakeWhile(c =>
                    {
                        if (reader.Current != c)
                            return false;
                        reader = reader.Next();
                        return true;
                    })
                    .Count();
                if (numberOfMatchedCharacters < value.Length)
                    return Optional<MatchResult<string>>.Missing;

                return MatchResult.New(value, reader); // Don't need to progress reader position since the work above moved it to the point after the content that it was trying to match
            };
        }

        public static Parser<string> Whitespace { get; } =
            reader =>
            {
                if (!PositionIsOverWhitespace(reader))
                    return Optional<MatchResult<string>>.Missing;

                var content = new StringBuilder();
                while (PositionIsOverWhitespace(reader))
                {
                    content.Append(reader.Current.Value);
                    reader = reader.Next();
                }
                return MatchResult.New(content.ToString(), reader); // Don't need to progress reader position since loop above moved beyond the whitespace content
            };

        private static bool PositionIsOverWhitespace(IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return (reader.Current != null) && char.IsWhiteSpace(reader.Current.Value);
        }
    }
}
