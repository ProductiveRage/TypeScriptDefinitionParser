using System;

namespace TypeScriptDefinitionParser.Types
{
    public sealed class GenericTypeParameterDetails : IType
    {
        public GenericTypeParameterDetails(NamedTypeDetails name, Optional<NamedTypeDetails> typeConstraint)
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

        public NamedTypeDetails Name { get; }
        public Optional<NamedTypeDetails> TypeConstraint { get; }
        public SourceRangeDetails SourceRange { get; }

        public override string ToString() => $"{Name}" + (TypeConstraint.IsDefined ? $" extends {TypeConstraint.Value}" : "");
    }
}
