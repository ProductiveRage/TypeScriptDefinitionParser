using System;

namespace TypeScriptDefinitionParser.ContentReaders
{
    public sealed class MatchResult<T>
    {
        public MatchResult(T result, IReadStringContent reader)
        {
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            Result = result;
            Reader = reader;
        }
        public T Result { get; }
        public IReadStringContent Reader { get; }
    }

    public static class MatchResult
    {
        public static MatchResult<T> New<T>(T value, IReadStringContent reader)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            return new MatchResult<T>(value, reader);
        }
    }
}
