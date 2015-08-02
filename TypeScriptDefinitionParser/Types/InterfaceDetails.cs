using System;
using TypeScriptDefinitionParser.TypeScriptDefinitionParser;

namespace TypeScriptDefinitionParser.Types
{
    public sealed class InterfaceDetails : IType
    {
        public InterfaceDetails(IdentifierDetails name, NonNullImmutableSet<IType> genericTypeParams, NonNullImmutableSet<PropertyDetails> contents, SourceRangeDetails source)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (genericTypeParams == null)
                throw new ArgumentNullException(nameof(genericTypeParams));
            if (contents == null)
                throw new ArgumentNullException(nameof(contents));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            Name = name;
            GenericTypeParams = genericTypeParams;
            Contents = contents;
            SourceRange = source;
        }

        public IdentifierDetails Name { get; }
        public NonNullImmutableSet<IType> GenericTypeParams { get; }
        public NonNullImmutableSet<PropertyDetails> Contents { get; }
        public SourceRangeDetails SourceRange { get; }
    }
}
