using System;

namespace TypeScriptDefinitionParser.Types
{
    public sealed class TypeDescriptionDetails : IType
    {
        public TypeDescriptionDetails(string value, SourceRangeDetails sourceRange)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("may not be null or blank", nameof(value));
            if (sourceRange == null)
                throw new ArgumentNullException(nameof(sourceRange));

            Value = value; // TODO: Need some more validity checks here
            SourceRange = sourceRange;
        }

        /// <summary>TODO</summary>
        public string Value { get; }
        public SourceRangeDetails SourceRange { get; }
        public override string ToString() => $"\"{Value}\"";
    }
}
