using System;
using System.Collections.Immutable;
using System.Linq;
using TypeScriptDefinitionParser.ContentReaders;
using TypeScriptDefinitionParser.Types;

namespace TypeScriptDefinitionParser
{
    public sealed class ParsedContent
    {
        public ParsedContent(ImmutableList<IType> types, IReadStringContent reader)
        {
            if (types == null)
                throw new ArgumentNullException(nameof(types));
            if (types.Any(t => t == null))
                throw new ArgumentException("Null reference encountered in set", nameof(types));

            Types = types;
            Reader = reader;
        }

        public ImmutableList<IType> Types { get; }
        public IReadStringContent Reader { get; }
    }
}
