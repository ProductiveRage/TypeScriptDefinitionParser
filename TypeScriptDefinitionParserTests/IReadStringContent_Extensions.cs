using System;
using System.Text;
using TypeScriptDefinitionParser.ContentReaders;

namespace TypeScriptDefinitionParserTests.Parsers
{
    public static class IReadStringContent_Extensions
    {
        public static string ReadRemainingContent(this IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var content = new StringBuilder();
            while (reader.Current != null)
            {
                content.Append(reader.Current);
                reader = reader.Next();
            }
            return content.ToString();
        }
    }
}
