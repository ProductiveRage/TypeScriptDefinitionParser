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

        public static Optional<MatchResult<NamedType>> NamedType(IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var name = Optional<IdentifierDetails>.Missing;
            var genericTypeParameters = Optional<ImmutableList<GenericTypeParameterDetails>>.Missing;
            var readerAfterValue = reader
                .StartMatching()
                .Then(Identifier, value => name = value)
                .ThenOptionally(Whitespace)
                .If(Match('<')).Then(GenericTypeParameters, value => genericTypeParameters = value);
            if (!readerAfterValue.IsDefined)
                return null;
            if (genericTypeParameters.IsDefined && !genericTypeParameters.Value.Any())
                throw new ArgumentException($"Invalid named type content starting at {reader.Index} - has generic type param opening bracket but zero type params present");

            return MatchResult.New(
                new NamedType(
                    name.Value,
                    genericTypeParameters.GetValueOrDefault(ImmutableList<GenericTypeParameterDetails>.Empty)
                ),
                readerAfterValue.Value
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
            var genericTypeParameters = Optional<ImmutableList<GenericTypeParameterDetails>>.Missing;
            var baseTypes = Optional<ImmutableList<NamedType>>.Missing;
            var properties = ImmutableList<PropertyDetails>.Empty;
            var readerAfterValue = reader
                .StartMatching()
                .Then(Match("interface"))
                .Then(Whitespace)
                .IfDefined(() => identifiedInterface = true)
                .ThenOptionally(Whitespace)
                .Then(Identifier, value => interfaceName = value)
                .ThenOptionally(Whitespace)
                .If(Match('<')).Then(GenericTypeParameters, value => genericTypeParameters = value)
                .ThenOptionally(Whitespace)
                .ThenOptionally(InheritanceChain, value => baseTypes = value)
                .ThenOptionally(Whitespace)
                .Then(Match('{'))
                .ThenOptionally(Whitespace)
                .ThenOptionally(InterfaceProperties, value => properties = value)
                .ThenOptionally(Whitespace)
                .Then(Match('}'));
            if (!readerAfterValue.IsDefined)
            {
                if (identifiedInterface)
                    throw new ArgumentException($"Invalid interface content starting at {reader.Index}");
                return null;
            }
            if (genericTypeParameters.IsDefined && !genericTypeParameters.Value.Any())
                throw new ArgumentException($"Invalid interface content starting at {reader.Index} - has generic type param opening bracket but zero type params present");

            return MatchResult.New(
                new InterfaceDetails(
                    interfaceName.Value,
                    genericTypeParameters.GetValueOrDefault(ImmutableList<GenericTypeParameterDetails>.Empty),
                    baseTypes.GetValueOrDefault(ImmutableList<NamedType>.Empty),
                    properties,
                    new SourceRangeDetails(reader.Index, readerAfterValue.Value.Index - reader.Index)
                ),
                readerAfterValue.Value
            );
        }

        public static Optional<MatchResult<ImmutableList<GenericTypeParameterDetails>>> GenericTypeParameters(IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var readerAtTypeParameterDefinition = reader.StartMatching().Then(Match('<'));
            if (!readerAtTypeParameterDefinition.IsDefined)
                return null;

            var genericTypeParameters = ImmutableList<GenericTypeParameterDetails>.Empty;
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

                var name = Optional<NamedType>.Missing;
                var typeConstraint = Optional<NamedType>.Missing;
                var readerAfterValue = readerAtTypeParameterDefinition
                    .ThenOptionally(Whitespace)
                    .Then(NamedType, value => name = value)
                    .ThenOptionally(Whitespace)
                    .If(Match("extends")).Then(BaseType, value => typeConstraint = value);
                if (!readerAfterValue.IsDefined)
                    break;
                genericTypeParameters = genericTypeParameters.Add(new GenericTypeParameterDetails(
                    name.Value,
                    typeConstraint
                ));
                readerAtTypeParameterDefinition = readerAfterValue;
            }
            return MatchResult.New(genericTypeParameters, reader);
        }

        public static Optional<MatchResult<NamedType>> BaseType(IReadStringContent reader)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var result = InheritanceChain(reader, supportMultipleBaseTypes: false);
            if (!result.IsDefined)
                return null;

            return MatchResult.New(result.Value.Result.Single(), result.Value.Reader);
        }

        public static Optional<MatchResult<ImmutableList<NamedType>>> InheritanceChain(IReadStringContent reader) => InheritanceChain(reader, supportMultipleBaseTypes: true);

        private static Optional<MatchResult<ImmutableList<NamedType>>> InheritanceChain(IReadStringContent reader, bool supportMultipleBaseTypes)
        {
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            var readerAtTypeList = reader.StartMatching().Then(Match("extends"));
            if (!readerAtTypeList.IsDefined)
                return null;

            var baseTypes = ImmutableList<NamedType>.Empty;
            while (true)
            {
                if (baseTypes.Any())
                {
                    var readerAfterTypeParameterSeparator = readerAtTypeList.ThenOptionally(Whitespace).Then(Match(','));
                    if (readerAfterTypeParameterSeparator.IsDefined)
                        readerAtTypeList = readerAfterTypeParameterSeparator;
                }

                var name = Optional<NamedType>.Missing;
                var readerAfterValue = readerAtTypeList
                    .ThenOptionally(Whitespace)
                    .Then(NamedType, value => name = value);
                if (!readerAfterValue.IsDefined)
                    break;
                baseTypes = baseTypes.Add(name.Value);
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
                return null;
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