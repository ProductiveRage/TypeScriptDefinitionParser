namespace TypeScriptDefinitionParser.ContentReaders
{
    public interface IReadStringContent
    {
        /// <summary>This will be null if all of the content has been consumed</summary>
        char? Current { get; }

        /// <summary>This will be an index within the string or the position immediately after it (if all content has been read)</summary>
        uint Index { get; }

        /// <summary>This will never return null, if all of the content has been read then the returned reference will have a null Current character/// </summary>
        IReadStringContent GetNext();
    }
}
