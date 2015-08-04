using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TypeScriptDefinitionParser;
using TypeScriptDefinitionParser.ContentReaders;
using TypeScriptDefinitionParser.Parsers;

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
            var result = StandardParsers.Match('<')(new StringNavigator("<T>"));
            Assert.IsTrue(result.IsDefined);
            Assert.AreEqual('<', result.Value.Result);
            Assert.AreEqual("T>", result.Value.Reader.ReadRemainingContent());
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
