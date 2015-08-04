using System;
using System.Collections.Immutable;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeScriptDefinitionParser;
using TypeScriptDefinitionParser.ContentReaders;
using TypeScriptDefinitionParser.Parsers;
using TypeScriptDefinitionParser.Types;
using static TypeScriptDefinitionParserTests.EqualityTesters.Common;
using static TypeScriptDefinitionParserTests.EqualityTesters.TypeScriptTypes;

namespace TypeScriptDefinitionParserTests.Parsers
{
    [TestClass]
    public class TypeDefinitionParsers_GenericTypeParameters
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null()
        {
            TypeDefinitionParsers.GenericTypeParameters(new StringNavigator(null));
        }

        [TestMethod]
        public void BlankContent()
        {
            Assert.IsTrue(
                DoOptionalMatchResultsMatch(
                    Optional<MatchResult<ImmutableList<TypeParameterDetails>>>.Missing,
                    TypeDefinitionParsers.GenericTypeParameters(new StringNavigator(""))
                )
            );
        }

        [TestMethod]
        public void SingleGenericTypeParameter()
        {
            AssertResultsMatch(
                MatchResult.New(
                    ImmutableList<TypeParameterDetails>.Empty.Add(
                        new TypeParameterDetails(
                            new IdentifierDetails("T", new SourceRangeDetails(1, 1)),
                            typeConstraint: Optional<IdentifierDetails>.Missing
                        )
                    ),
                    new StringNavigator("")
                ),
                TypeDefinitionParsers.GenericTypeParameters(new StringNavigator("<T>"))
            );
        }

        // TODO: SingleGenericTypeParameterWithBaseType
        // TODO: MultipleGenericTypeParameters

        [TestMethod]
        public void NoLeadingBracket()
        {
            Assert.AreEqual(
                Optional<MatchResult<ImmutableList<TypeParameterDetails>>>.Missing,
                TypeDefinitionParsers.GenericTypeParameters(new StringNavigator("T"))
            );
        }

        private void AssertResultsMatch(Optional<MatchResult<ImmutableList<TypeParameterDetails>>> expected, Optional<MatchResult<ImmutableList<TypeParameterDetails>>> actual)
        {
            Assert.IsTrue(
                DoOptionalMatchResultsMatch(
                    expected,
                    actual,
                    (x, y) => DoImmutableListsMatch(x, y, DoTypeParameterDetailsMatch)
                )
            );
        }
    }
}
