namespace TypeScriptDefinitionParser.ContentReaders
{
    public delegate Optional<MatchResult<T>> Parser<T>(IReadStringContent reader);
}
