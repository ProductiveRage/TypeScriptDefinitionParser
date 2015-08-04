using System;
using TypeScriptDefinitionParser.Types;
using static TypeScriptDefinitionParserTests.EqualityTesters.Common;

namespace TypeScriptDefinitionParserTests.EqualityTesters
{
    public static class TypeScriptTypes
    {
        public static bool DoTypeParameterDetailsMatch(TypeParameterDetails x, TypeParameterDetails y)
        {
            if (x == null)
                throw new ArgumentNullException(nameof(x));
            if (y == null)
                throw new ArgumentNullException(nameof(y));

            return
                DoIdentifierDetailsMatch(x.Name, y.Name) &&
                DoOptionalValuesMatch(x.TypeConstraint, y.TypeConstraint, DoIdentifierDetailsMatch) &&
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
