using System;

namespace TypeScriptDefinitionParser.ContentReaders
{
    public sealed class StringNavigator : IReadStringContent
    {
        private readonly string _content;
        public StringNavigator(string content) : this(content, 0) { }
        private StringNavigator(string content, uint index)
        {
            if (content == null)
                throw new ArgumentNullException(nameof(content));
            if (index > content.Length)
                throw new ArgumentOutOfRangeException(nameof(index), "must either indicate an index within the string or the position immediately after it");

            _content = content;
            Index = index;

            if (Index == content.Length)
                Current = null;
            else
                Current = _content[(int)Index];
        }

        /// <summary>This will be null if all of the content has been consumed</summary>
        public char? Current { get; }

        /// <summary>This will be an index within the string or the position immediately after it (if all content has been read)</summary>
        public uint Index { get; }

        /// <summary>This will never return null, if all of the content has been read then the returned reference will have a null Current character/// </summary>
        public IReadStringContent Next()
        {
            if (Index >= _content.Length)
                return this;
            return new StringNavigator(_content, Index + 1);
        }

        public override string ToString()
        {
            if (Index >= _content.Length)
                return "NoContent";

            const int maximumNumberOfCharactersFromContentToDisplay = 16;
            var endIndexForSegment = Math.Min((int)Index + maximumNumberOfCharactersFromContentToDisplay, _content.Length);
            var isTruncatedContent = endIndexForSegment < _content.Length;
            return string.Format(
                "\"{0}{1}\"",
                _content.Substring((int)Index, endIndexForSegment - (int)Index),
                isTruncatedContent ? ".." : ""
            );
        }
    }
}
