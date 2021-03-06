﻿using System;
using System.Collections.Immutable;
using System.Linq;

namespace TypeScriptDefinitionParser.Types
{
    public sealed class IdentifierDetails : IType
    {
        public static ImmutableList<char> DisallowedCharacters { get; }
            = Enumerable.Range(0, char.MaxValue)
                .Select(c => (char)c)
                .Where(c => !char.IsLetterOrDigit(c) && (c != '$') && (c != '_'))
                .ToImmutableList();

        public IdentifierDetails(string value, SourceRangeDetails sourceRange)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("may not be null or blank", nameof(value));
            if (sourceRange == null)
                throw new ArgumentNullException(nameof(sourceRange));

            var firstDisallowedCharacter = value
                .Select((c, i) => new { Index = i, Character = c })
                .FirstOrDefault(indexedCharacter => DisallowedCharacters.Contains(indexedCharacter.Character));
            if (firstDisallowedCharacter != null)
                throw new ArgumentException($"Contains invalid character at index {firstDisallowedCharacter.Index}: '{firstDisallowedCharacter.Character}'", nameof(value));

            Value = value;
            SourceRange = sourceRange;
        }

        /// <summary>This will always be a non-null, non-blank, valid TypeScript identifier</summary>
        public string Value { get; }

        public SourceRangeDetails SourceRange { get; }

        public override string ToString() => $"{Value}";
    }
}
