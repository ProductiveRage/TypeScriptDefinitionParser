using System;
using System.Collections.Immutable;
using System.Linq;
using TypeScriptDefinitionParser.ContentReaders;
using TypeScriptDefinitionParser.Types;
using static TypeScriptDefinitionParser.Parsers.CommonTerminators;
using static TypeScriptDefinitionParser.Parsers.StandardParsers;

namespace TypeScriptDefinitionParser.Parsers
{
    public static class TypeDefinitionParsers
    {
        public static Optional<MatchResult<IdentifierDetails>> Identifier(IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var result = reader.MatchAny(UntilWhiteSpaceOrPunctuation);
            if (!result.IsDefined)
                return null;

            return MatchResult.New(
                new IdentifierDetails(result.Value.Result, GetSourceRangeFromReaders(reader, result.Value.Reader)),
                result.Value.Reader
            );
        }

        public static Optional<MatchResult<TypeDescriptionDetails>> TypeDescription(IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var result = reader.MatchAny(UntilWhiteSpaceOrPunctuation);
            if (!result.IsDefined)
                return null;

            return MatchResult.New(
                new TypeDescriptionDetails(result.Value.Result, GetSourceRangeFromReaders(reader, result.Value.Reader)),
                result.Value.Reader
            );
        }

        public static Optional<MatchResult<InterfaceDetails>> Interface(IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var identifiedInterface = false;
            var interfaceName = Optional<IdentifierDetails>.Missing;
            var genericTypeParams = Optional<ImmutableList<IType>>.Missing;
            var properties = ImmutableList<PropertyDetails>.Empty;
            var readerAfterInterfaceContent = reader
                .StartMatching()
                .Then(Match("interface"))
                .Then(Whitespace)
                .IfDefined(() => identifiedInterface = true)
                .ThenOptionally(Whitespace)
                .Then(Identifier, value => interfaceName = value)
                // TODO .If('<', GenericTypeParamList(value => genericTypeParams = value))
                .ThenOptionally(Whitespace)
                .Then(Match('{'))
                .ThenOptionally(Whitespace)
                .ThenOptionally(InterfaceProperties, value => properties = value)
                .ThenOptionally(Whitespace)
                .Then(Match('}'));
                ;
            if (!readerAfterInterfaceContent.IsDefined)
            {
                if (identifiedInterface)
                    throw new ArgumentException($"Invalid interface content starting at {reader.Index}");
                return Optional<MatchResult<InterfaceDetails>>.Missing;
            }
            if (genericTypeParams.IsDefined && !genericTypeParams.Value.Any())
                throw new ArgumentException($"Invalid interface content starting at {reader.Index} - has generic type param opening bracket but zero type params present");

            return MatchResult.New(
                new InterfaceDetails(
                    interfaceName.Value,
                    genericTypeParams.GetValueOrDefault(ImmutableList<IType>.Empty),
                    properties,
                    new SourceRangeDetails(reader.Index, readerAfterInterfaceContent.Value.Index - reader.Index)
                ),
                readerAfterInterfaceContent.Value
            );
        }

        public static Optional<MatchResult<ImmutableList<IType>>> GenericTypeParamList(IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            // TODO: Check for opening '<' and closing '>'
            // TODO: Needs to support types with optional "extends" constraints
            throw new NotImplementedException(); // TODO
        }

        private static Optional<MatchResult<ImmutableList<PropertyDetails>>> InterfaceProperties(IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var contents = ImmutableList<PropertyDetails>.Empty;
            while (true)
            {
                var identifier = Optional<IdentifierDetails>.Missing;
                var typeDescription = Optional<TypeDescriptionDetails>.Missing;
                var readerAfterValue = reader
                    .StartMatching()
                    .ThenOptionally(Whitespace)
                    .Then(Identifier, value => identifier = value)
                    .ThenOptionally(Whitespace)
                    .Then(Match(':'))
                    .ThenOptionally(Whitespace)
                    .Then(TypeDescription, value => typeDescription = value)
                    .Then(Match(';'))
                    .ThenOptionally(Whitespace);
                if (!readerAfterValue.IsDefined)
                    break;

                contents = contents.Add(new PropertyDetails(identifier.Value, typeDescription.Value));
                reader = readerAfterValue.Value;
            }
            if (!contents.Any())
                return Optional<MatchResult<ImmutableList<PropertyDetails>>>.Missing;
            return MatchResult.New(contents, reader);
        }

        private static SourceRangeDetails GetSourceRangeFromReaders(IReadStringContent readerStart, IReadStringContent readerAfter)
        {
            if (readerAfter == null)
                throw new ArgumentNullException(nameof(readerAfter));
            if (readerAfter == null)
                throw new ArgumentNullException(nameof(readerAfter));

            return new SourceRangeDetails(readerStart.Index, (readerAfter.Index - readerStart.Index) + 1);
        }
    }
}