using System;
using System.Collections.Immutable;
using System.Linq;

namespace TypeScriptDefinitionParser.Types
{
    public sealed class InterfaceDetails : IType
    {
        public InterfaceDetails(IdentifierDetails name, ImmutableList<TypeParameterDetails> genericTypeParams, ImmutableList<PropertyDetails> contents, SourceRangeDetails source)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (genericTypeParams == null)
                throw new ArgumentNullException(nameof(genericTypeParams));
            if (genericTypeParams.Any(t => t == null))
                throw new ArgumentException("Null reference encountered in set", nameof(genericTypeParams));
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));
            if (contents.Any(p => p == null))
                throw new ArgumentException("Null reference encountered in set", nameof(contents));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Name = name;
            GenericTypeParams = genericTypeParams;
            Contents = contents;
            SourceRange = source;
        }

        public IdentifierDetails Name { get; }
        public ImmutableList<TypeParameterDetails> GenericTypeParams { get; }
        public ImmutableList<PropertyDetails> Contents { get; }
        public SourceRangeDetails SourceRange { get; }
    }
}
