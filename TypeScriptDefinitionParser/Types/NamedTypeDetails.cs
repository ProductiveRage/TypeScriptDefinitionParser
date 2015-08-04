using System;
using System.Collections.Immutable;
using System.Linq;

namespace TypeScriptDefinitionParser.Types
{
    public sealed class NamedTypeDetails : IType
    {
        public NamedTypeDetails(IdentifierDetails name, ImmutableList<GenericTypeParameterDetails> genericTypeParams)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (genericTypeParams == null)
                throw new ArgumentNullException(nameof(genericTypeParams));
            if (genericTypeParams.Any(t => t == null))
                throw new ArgumentException("Null reference encountered in set", nameof(genericTypeParams));

            Name = name;
            GenericTypeParams = genericTypeParams;

            var endOfContent = genericTypeParams.Any()
                ? genericTypeParams.Max(t => t.SourceRange.StartIndex + t.SourceRange.Length)
                : (name.SourceRange.StartIndex + name.SourceRange.Length);
            SourceRange = new SourceRangeDetails(
                name.SourceRange.StartIndex,
                endOfContent - name.SourceRange.StartIndex
            );
        }

        public IdentifierDetails Name { get; }
        public ImmutableList<GenericTypeParameterDetails> GenericTypeParams { get; }
        public SourceRangeDetails SourceRange { get; }

        public override string ToString() => $"{Name}" + (GenericTypeParams.Any() ? ("<" + string.Join(", ", GenericTypeParams) + ">") : "");
    }
}
