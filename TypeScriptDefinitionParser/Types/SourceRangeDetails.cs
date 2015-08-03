using System;

namespace TypeScriptDefinitionParser.Types
{
    public sealed class SourceRangeDetails
    {
        public SourceRangeDetails(uint startIndex, uint length)
        {
            if (length == 0)
                throw new ArgumentOutOfRangeException(nameof(length), "must not be zero");

            StartIndex = startIndex;
            Length = length;
        }

        public uint StartIndex { get; }

        /// <summary>This will always be greater than zero</summary>
        public uint Length { get; }

        public override string ToString()
        {
            return $"{{ {StartIndex}, {Length} }}";
        }
    }
}
