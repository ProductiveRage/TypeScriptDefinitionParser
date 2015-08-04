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
                    Optional<MatchResult<ImmutableList<GenericTypeParameterDetails>>>.Missing,
                    TypeDefinitionParsers.GenericTypeParameters(new StringNavigator(""))
                )
            );
        }

        [TestMethod]
        public void SingleGenericTypeParameter()
        {
            var content = "<T>";
            var expected = MatchResult.New(
                ImmutableList<GenericTypeParameterDetails>.Empty
                    .Add(new GenericTypeParameterDetails(
                        new NamedTypeDetails(
                            name: new IdentifierDetails("T", new SourceRangeDetails(1, 1)),
                            genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                        ),
                        typeConstraint: Optional<NamedTypeDetails>.Missing
                    )),
                new StringNavigator("")
            );
            AssertResultsMatch(expected, TypeDefinitionParsers.GenericTypeParameters(new StringNavigator(content)));
        }

        [TestMethod]
        public void SingleGenericTypeParameterWithBaseType()
        {
            var content = "<T extends Something>";
            var expected = MatchResult.New(
                ImmutableList<GenericTypeParameterDetails>.Empty
                    .Add(new GenericTypeParameterDetails(
                        name: new NamedTypeDetails(
                            name: new IdentifierDetails("T", new SourceRangeDetails(1, 1)),
                            genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                        ),
                        typeConstraint: new NamedTypeDetails(
                            name: new IdentifierDetails("Something", new SourceRangeDetails(11, 9)),
                            genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                        )
                    )),
                new StringNavigator("")
            );
            AssertResultsMatch(expected, TypeDefinitionParsers.GenericTypeParameters(new StringNavigator(content)));
        }

        [TestMethod]
        public void TwoGenericTypeParameters()
        {
            var content = "<TKey, TValue>";
            var expected = MatchResult.New(
                ImmutableList<GenericTypeParameterDetails>.Empty
                    .Add(new GenericTypeParameterDetails(
                        new NamedTypeDetails(
                            name: new IdentifierDetails("TKey", new SourceRangeDetails(1, 4)),
                            genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                        ),
                        typeConstraint: Optional<NamedTypeDetails>.Missing
                    ))
                    .Add(new GenericTypeParameterDetails(
                        new NamedTypeDetails(
                            name: new IdentifierDetails("TValue", new SourceRangeDetails(7, 6)),
                            genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                        ),
                        typeConstraint: Optional<NamedTypeDetails>.Missing
                    )),
                new StringNavigator("")
            );
            AssertResultsMatch(expected, TypeDefinitionParsers.GenericTypeParameters(new StringNavigator(content)));
        }

        [TestMethod]
        public void SingleGenericTypeParameterWithBaseTypeThenOneWithout()
        {
            var content = "<TKey extends Something, TValue>";
            var expected = MatchResult.New(
                ImmutableList<GenericTypeParameterDetails>.Empty
                    .Add(new GenericTypeParameterDetails(
                        name: new NamedTypeDetails(
                            name: new IdentifierDetails("TKey", new SourceRangeDetails(1, 4)),
                            genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                        ),
                        typeConstraint: new NamedTypeDetails(
                            name: new IdentifierDetails("Something", new SourceRangeDetails(14, 9)),
                            genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                        )
                    ))
                    .Add(new GenericTypeParameterDetails(
                        new NamedTypeDetails(
                            name: new IdentifierDetails("TValue", new SourceRangeDetails(25, 6)),
                            genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                        ),
                        typeConstraint: Optional<NamedTypeDetails>.Missing
                    )),
                new StringNavigator("")
            );
            AssertResultsMatch(expected, TypeDefinitionParsers.GenericTypeParameters(new StringNavigator(content)));
        }

        [TestMethod]
        public void NoLeadingBracket()
        {
            var content = "T";
            Assert.AreEqual(
                Optional<MatchResult<ImmutableList<GenericTypeParameterDetails>>>.Missing,
                TypeDefinitionParsers.GenericTypeParameters(new StringNavigator(content))
            );
        }

        private void AssertResultsMatch(Optional<MatchResult<ImmutableList<GenericTypeParameterDetails>>> expected, Optional<MatchResult<ImmutableList<GenericTypeParameterDetails>>> actual)
        {
            Assert.IsTrue(
                DoOptionalMatchResultsMatch(
                    expected,
                    actual,
                    (x, y) => DoImmutableListsMatch(x, y, DoGenericTypeParameterDetailsMatch)
                )
            );
        }
    }
}
