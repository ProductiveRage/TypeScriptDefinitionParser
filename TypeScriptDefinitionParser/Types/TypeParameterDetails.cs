using System;

namespace TypeScriptDefinitionParser.Types
{
    public sealed class TypeParameterDetails : IType
    {
        public TypeParameterDetails(IdentifierDetails name, Optional<IdentifierDetails> typeConstraint)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Name = name;
            TypeConstraint = typeConstraint;

            var endOfContent = typeConstraint.IsDefined
                ? (typeConstraint.Value.SourceRange.StartIndex + typeConstraint.Value.SourceRange.Length)
                : (name.SourceRange.StartIndex + name.SourceRange.Length);
            SourceRange = new SourceRangeDetails(
                name.SourceRange.StartIndex,
                endOfContent - name.SourceRange.StartIndex
            );
        }

        public IdentifierDetails Name { get; }
        public Optional<IdentifierDetails> TypeConstraint { get; }
        public SourceRangeDetails SourceRange { get; }

        public override string ToString()
        {
            return $"{Name}" + (TypeConstraint.IsDefined ? $" extends {TypeConstraint.Value}" : "");
        }
    }
}
