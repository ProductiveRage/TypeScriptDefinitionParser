using System;

namespace TypeScriptDefinitionParser.Types
{
    public sealed class PropertyDetails : IType
    {
        public PropertyDetails(IdentifierDetails name, TypeDescriptionDetails type)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            Name = name;
            Type = type;

            SourceRange = new SourceRangeDetails(
                name.SourceRange.StartIndex,
                (type.SourceRange.StartIndex + type.SourceRange.Length) - name.SourceRange.StartIndex
            );
        }

        public IdentifierDetails Name { get; }
        public TypeDescriptionDetails Type { get; }
        public SourceRangeDetails SourceRange { get; }
    }
}
