using System;
using TypeScriptDefinitionParser.Types;
using static TypeScriptDefinitionParserTests.EqualityTesters.Common;

namespace TypeScriptDefinitionParserTests.EqualityTesters
{
    public static class TypeScriptTypes
    {
        public static bool DoGenericTypeParameterDetailsMatch(GenericTypeParameterDetails x, GenericTypeParameterDetails y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));

            return
                DoNamedTypesMatch(x.Name, y.Name) &&
                DoOptionalValuesMatch(x.TypeConstraint, y.TypeConstraint, DoNamedTypesMatch) &&
                DoSourceRangeDetailsMatch(x.SourceRange, y.SourceRange);
        }

        public static bool DoNamedTypesMatch(NamedType x, NamedType y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));

            return
                DoIdentifierDetailsMatch(x.Name, y.Name) &&
                DoImmutableListsMatch(x.GenericTypeParams, y.GenericTypeParams, DoGenericTypeParameterDetailsMatch) &&
                DoSourceRangeDetailsMatch(x.SourceRange, y.SourceRange);
        }

        public static bool DoIdentifierDetailsMatch(IdentifierDetails x, IdentifierDetails y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));

            return (x.Value == y.Value) && DoSourceRangeDetailsMatch(x.SourceRange, y.SourceRange);
        }

        public static bool DoSourceRangeDetailsMatch(SourceRangeDetails x, SourceRangeDetails y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));

            return (x.StartIndex == y.StartIndex) && (x.Length == y.Length);
        }
    }
}
