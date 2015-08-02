using System;

namespace TypeScriptDefinitionParser.ContentReaders
{
    public static class OptionalIReadStringContent_Extensions
    {
        public static Optional<IReadStringContent> Then<T>(this Optional<IReadStringContent> reader, Parser<T> parser, Action<T> report)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            if (!reader.IsDefined)
                return reader;

            var result = parser(reader.Value);
            if (!result.IsDefined)
                return Optional<IReadStringContent>.Missing;

            report(result.Value.Result);
            return Optional.For(result.Value.Reader);
        }

        public static Optional<IReadStringContent> Then<T>(this Optional<IReadStringContent> reader, Parser<T> parser) => Then(reader, parser, value => { });

        public static Optional<IReadStringContent> ThenOptionally<T>(this Optional<IReadStringContent> reader, Parser<T> parser, Action<T> report)
        {
            if (parser == null)
                throw new ArgumentNullException(nameof(parser));
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            if (!reader.IsDefined)
                return reader;

            return Optional.For(
                Then(reader, parser, report).GetValueOrDefault(reader.Value)
            );
        }

        public static Optional<IReadStringContent> ThenOptionally<T>(this Optional<IReadStringContent> reader, Parser<T> parser) => ThenOptionally(reader, parser, value => { });

        public static Optional<IReadStringContent> IfDefined(this Optional<IReadStringContent> reader, Action report)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));

            if (reader.IsDefined)
                report();
            return reader;
        }
    }
}
