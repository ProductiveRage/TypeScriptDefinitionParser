using System;
using TypeScriptDefinitionParser.ContentReaders;
using TypeScriptDefinitionParser.Types;
using TypeScriptDefinitionParser.TypeScriptDefinitionParser;

namespace TypeScriptDefinitionParser
{
    public sealed class ParsedContent
    {
        public ParsedContent(NonNullImmutableSet<IType> types, IReadStringContent reader)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));

            Types = types;
            Reader = reader;
        }

        public NonNullImmutableSet<IType> Types { get; }
        public IReadStringContent Reader { get; }
    }
}
