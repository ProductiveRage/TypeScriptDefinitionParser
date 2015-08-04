using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeScriptDefinitionParser;
using TypeScriptDefinitionParser.ContentReaders;
using TypeScriptDefinitionParser.Parsers;
using static TypeScriptDefinitionParserTests.EqualityTesters.Common;

namespace TypeScriptDefinitionParserTests.Parsers
{
    [TestClass]
    public class StandardParsersTests_MatchCharacter
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null()
        {
            StandardParsers.Match('<')(new StringNavigator(null));
        }

        [TestMethod]
        public void BlankContent()
        {
            Assert.AreEqual(
                Optional<MatchResult<char>>.Missing,
                StandardParsers.Match('<')(new StringNavigator(""))
            );
        }

        [TestMethod]
        public void MatchSuccess()
        {
            Assert.IsTrue(
                DoOptionalMatchResultsMatch(
                    Optional.For(MatchResult.New('<', new StringNavigator("T>"))),
                    StandardParsers.Match('<')(new StringNavigator("<T>"))
                )
            );
        }

        [TestMethod]
        public void MatchFailure()
        {
            Assert.AreEqual(
                Optional<MatchResult<char>>.Missing,
                StandardParsers.Match('>')(new StringNavigator(""))
            );
        }
    }
}
