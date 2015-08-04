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

            var result = reader.MatchAnythingUntil(IdentifierDetails.DisallowedCharacters);
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

            var result = reader.MatchAnythingUntil(UntilWhiteSpaceOrPunctuation);
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
            var genericTypeParameters = Optional<ImmutableList<TypeParameterDetails>>.Missing;
            var properties = ImmutableList<PropertyDetails>.Empty;
            var readerAfterInterfaceContent = reader
                .StartMatching()
                .Then(Match("interface"))
                .Then(Whitespace)
                .IfDefined(() => identifiedInterface = true)
                .ThenOptionally(Whitespace)
                .Then(Identifier, value => interfaceName = value)
                .ThenOptionally(Whitespace)
                .If(Match('<')).Then(GenericTypeParameters, value => genericTypeParameters = value)
                .ThenOptionally(Whitespace)
                // TODO: Check for interfaces that this one implements (eg. "interface Square extends Shape, PenStroke")
                .Then(Match('{'))
                .ThenOptionally(Whitespace)
                .ThenOptionally(InterfaceProperties, value => properties = value)
                .ThenOptionally(Whitespace)
                .Then(Match('}'));
            if (!readerAfterInterfaceContent.IsDefined)
            {
                if (identifiedInterface)
                    throw new ArgumentException($"Invalid interface content starting at {reader.Index}");
                return Optional<MatchResult<InterfaceDetails>>.Missing;
            }
            if (genericTypeParameters.IsDefined && !genericTypeParameters.Value.Any())
                throw new ArgumentException($"Invalid interface content starting at {reader.Index} - has generic type param opening bracket but zero type params present");

            return MatchResult.New(
                new InterfaceDetails(
                    interfaceName.Value,
                    genericTypeParameters.GetValueOrDefault(ImmutableList<TypeParameterDetails>.Empty),
                    properties,
                    new SourceRangeDetails(reader.Index, readerAfterInterfaceContent.Value.Index - reader.Index)
                ),
                readerAfterInterfaceContent.Value
            );
        }

        public static Optional<MatchResult<ImmutableList<TypeParameterDetails>>> GenericTypeParameters(IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var readerAtTypeParameterDefinition = reader.StartMatching().Then(Match('<'));
            if (!readerAtTypeParameterDefinition.IsDefined)
                return Optional<MatchResult<ImmutableList<TypeParameterDetails>>>.Missing;

            var genericTypeParameters = ImmutableList<TypeParameterDetails>.Empty;
            while (true)
            {
                var readerAfterImminentClosingBrace = readerAtTypeParameterDefinition.ThenOptionally(Whitespace).Then(Match('>'));
                if (readerAfterImminentClosingBrace.IsDefined)
                {
                    reader = readerAfterImminentClosingBrace.Value;
                    break;
                }
                if (genericTypeParameters.Any())
                {
                    var readerAfterTypeParameterSeparator = readerAtTypeParameterDefinition.ThenOptionally(Whitespace).Then(Match(','));
                    if (readerAfterTypeParameterSeparator.IsDefined)
                        readerAtTypeParameterDefinition = readerAfterTypeParameterSeparator;
                }

                var identifier = Optional<IdentifierDetails>.Missing;
                var typeConstraint = Optional<IdentifierDetails>.Missing;
                var readerAfterValue = readerAtTypeParameterDefinition
                    .ThenOptionally(Whitespace)
                    .Then(Identifier, value => identifier = value)
                    .ThenOptionally(Whitespace)
                    .If(Match("extends")).Then(BaseType, value => typeConstraint = value);
                if (!readerAfterValue.IsDefined)
                    break;
                genericTypeParameters = genericTypeParameters.Add(new TypeParameterDetails(
                    identifier.Value,
                    typeConstraint
                ));
                readerAtTypeParameterDefinition = readerAfterValue;
            }
            return MatchResult.New(genericTypeParameters, reader);
        }

        public static Optional<MatchResult<IdentifierDetails>> BaseType(IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var result = InheritanceChain(reader, supportMultipleBaseTypes: false);
            if (!result.IsDefined)
                return Optional<MatchResult<IdentifierDetails>>.Missing;

            return MatchResult.New(result.Value.Result.Single(), result.Value.Reader);
        }

        public static Optional<MatchResult<ImmutableList<IdentifierDetails>>> InheritanceChain(IReadStringContent reader) => InheritanceChain(reader, supportMultipleBaseTypes: true);

        private static Optional<MatchResult<ImmutableList<IdentifierDetails>>> InheritanceChain(IReadStringContent reader, bool supportMultipleBaseTypes)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var readerAtTypeList = reader.StartMatching().Then(Match("extends"));
            if (!readerAtTypeList.IsDefined)
                return Optional<MatchResult<ImmutableList<IdentifierDetails>>>.Missing;

            var baseTypes = ImmutableList<IdentifierDetails>.Empty;
            while (true)
            {
                if (baseTypes.Any())
                {
                    var readerAfterTypeParameterSeparator = readerAtTypeList.ThenOptionally(Whitespace).Then(Match(','));
                    if (readerAfterTypeParameterSeparator.IsDefined)
                        readerAtTypeList = readerAfterTypeParameterSeparator;
                }

                var identifier = Optional<IdentifierDetails>.Missing;
                var readerAfterValue = readerAtTypeList
                    .ThenOptionally(Whitespace)
                    .Then(Identifier, value => identifier = value);
                if (!readerAfterValue.IsDefined)
                    break;
                baseTypes = baseTypes.Add(identifier.Value);
                readerAtTypeList = readerAfterValue;
                if (!supportMultipleBaseTypes)
                    break;
            }
            return MatchResult.New(baseTypes, readerAtTypeList.Value);
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
                    .ThenOptionally(Whitespace)
                    .Then(Match(';'));
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

            return new SourceRangeDetails(readerStart.Index, readerAfter.Index - readerStart.Index);
        }
    }
}