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
    public class TypeDefinitionParsers_Interface
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null()
        {
            TypeDefinitionParsers.Interface(new StringNavigator(null));
        }

        [TestMethod]
        public void BlankContent()
        {
            Assert.IsTrue(
                DoOptionalMatchResultsMatch(
                    Optional<MatchResult<InterfaceDetails>>.Missing,
                    TypeDefinitionParsers.Interface(new StringNavigator(""))
                )
            );
        }

        [TestMethod]
        public void InterfaceThatExtendsNestedGenericBaseTypeButHasNoProperties()
        {
            var content = "interface DOMAttributes extends Props<DOMComponent<any>> { }";
            var expected = MatchResult.New(
                value: new InterfaceDetails(
                    name: new IdentifierDetails("DOMAttributes", new SourceRangeDetails(10, 13)),
                    genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty,
                    baseTypes: ImmutableList<NamedTypeDetails>.Empty.Add(new NamedTypeDetails(
                        name: new IdentifierDetails("Props", new SourceRangeDetails(32, 5)),
                        genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty.Add(new GenericTypeParameterDetails(
                            name: new NamedTypeDetails(
                                name: new IdentifierDetails("DOMComponent", new SourceRangeDetails(38, 12)),
                                genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty.Add(new GenericTypeParameterDetails(
                                    name: new NamedTypeDetails(
                                        name: new IdentifierDetails("any", new SourceRangeDetails(51, 3)),
                                        genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                                    ),
                                    typeConstraint: Optional<NamedTypeDetails>.Missing
                                ))
                            ),
                            typeConstraint: Optional<NamedTypeDetails>.Missing
                        ))
                    )),
                    contents: ImmutableList<PropertyDetails>.Empty,
                    source: new SourceRangeDetails(0, 60)
                ),
                reader: new StringNavigator("")
            );
            Assert.IsTrue(
                DoOptionalMatchResultsMatch(
                    Optional.For(expected),
                    TypeDefinitionParsers.Interface(new StringNavigator(content)),
                    DoInterfaceDetailsMatch
                )
            );
        }

        [TestMethod]
        public void InterfaceWithGenericTypeParameterThatExtendsAnotherGenericInterfaceButThatHasNoProperties()
        {
            var content = "interface ClassicElement<P> extends ReactElement<P> { }";
            var expected = MatchResult.New(
                value: new InterfaceDetails(
                    name: new IdentifierDetails("ClassicElement", new SourceRangeDetails(10, 14)),
                    genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty.Add(new GenericTypeParameterDetails(
                        name: new NamedTypeDetails(
                            name: new IdentifierDetails("P", new SourceRangeDetails(25, 1)),
                            genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                        ),
                        typeConstraint: Optional<NamedTypeDetails>.Missing
                    )),
                    baseTypes: ImmutableList<NamedTypeDetails>.Empty.Add(new NamedTypeDetails(
                        name: new IdentifierDetails("ReactElement", new SourceRangeDetails(36, 12)),
                        genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty.Add(new GenericTypeParameterDetails(
                            name: new NamedTypeDetails(
                                name: new IdentifierDetails("P", new SourceRangeDetails(49, 1)),
                                genericTypeParams: ImmutableList<GenericTypeParameterDetails>.Empty
                            ),
                            typeConstraint: Optional<NamedTypeDetails>.Missing
                        ))
                    )),
                    contents: ImmutableList<PropertyDetails>.Empty,
                    source: new SourceRangeDetails(0, 55)
                ),
                reader: new StringNavigator("")
            );
            Assert.IsTrue(
                DoOptionalMatchResultsMatch(
                    Optional.For(expected),
                    TypeDefinitionParsers.Interface(new StringNavigator(content)),
                    DoInterfaceDetailsMatch
                )
            );
        }
    }
}
