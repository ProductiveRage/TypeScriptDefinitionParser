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
            AssertResultsMatch(
                MatchResult.New(
                    ImmutableList<GenericTypeParameterDetails>.Empty
                        .Add(new GenericTypeParameterDetails(
                            new NamedType(
                                name: new IdentifierDetails("T", new SourceRangeDetails(1, 1)),
                                genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                            ),
                            typeConstraint: Optional<NamedType>.Missing
                        )),
                    new StringNavigator("")
                ),
                TypeDefinitionParsers.GenericTypeParameters(new StringNavigator("<T>"))
            );
        }

        [TestMethod]
        public void SingleGenericTypeParameterWithBaseType()
        {
            AssertResultsMatch(
                MatchResult.New(
                    ImmutableList<GenericTypeParameterDetails>.Empty
                        .Add(new GenericTypeParameterDetails(
                            name: new NamedType(
                                name: new IdentifierDetails("T", new SourceRangeDetails(1, 1)),
                                genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                            ),
                            typeConstraint: new NamedType(
                                name: new IdentifierDetails("Something", new SourceRangeDetails(11, 9)),
                                genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                            )
                        )),
                    new StringNavigator("")
                ),
                TypeDefinitionParsers.GenericTypeParameters(new StringNavigator("<T extends Something>"))
            );
        }

        [TestMethod]
        public void TwoGenericTypeParameters()
        {
            AssertResultsMatch(
                MatchResult.New(
                    ImmutableList<GenericTypeParameterDetails>.Empty
                        .Add(new GenericTypeParameterDetails(
                            new NamedType(
                                name: new IdentifierDetails("TKey", new SourceRangeDetails(1, 4)),
                                genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                            ),
                            typeConstraint: Optional<NamedType>.Missing
                        ))
                        .Add(new GenericTypeParameterDetails(
                            new NamedType(
                                name: new IdentifierDetails("TValue", new SourceRangeDetails(7, 6)),
                                genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                            ),
                            typeConstraint: Optional<NamedType>.Missing
                        )),
                    new StringNavigator("")
                ),
                TypeDefinitionParsers.GenericTypeParameters(new StringNavigator("<TKey, TValue>"))
            );
        }

        [TestMethod]
        public void SingleGenericTypeParameterWithBaseTypeThenOneWithout()
        {
            AssertResultsMatch(
                MatchResult.New(
                    ImmutableList<GenericTypeParameterDetails>.Empty
                        .Add(new GenericTypeParameterDetails(
                            name: new NamedType(
                                name: new IdentifierDetails("TKey", new SourceRangeDetails(1, 4)),
                                genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                            ),
                            typeConstraint: new NamedType(
                                name: new IdentifierDetails("Something", new SourceRangeDetails(14, 9)),
                                genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                            )
                        ))
                        .Add(new GenericTypeParameterDetails(
                            new NamedType(
                                name: new IdentifierDetails("TValue", new SourceRangeDetails(25, 6)),
                                genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                            ),
                            typeConstraint: Optional<NamedType>.Missing
                        )),
                    new StringNavigator("")
                ),
                TypeDefinitionParsers.GenericTypeParameters(new StringNavigator("<TKey extends Something, TValue>"))
            );
        }


        [TestMethod]
        public void NoLeadingBracket()
        {
            Assert.AreEqual(
                Optional<MatchResult<ImmutableList<GenericTypeParameterDetails>>>.Missing,
                TypeDefinitionParsers.GenericTypeParameters(new StringNavigator("T"))
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
