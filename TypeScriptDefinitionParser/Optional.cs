﻿using System.Diagnostics;

namespace TypeScriptDefinitionParser
{
    // Borrowed from https://github.com/AArnott/ImmutableObjectGraph
    [DebuggerDisplay("{IsDefined ? Value.ToString() : \"<missing>\",nq}")]
    public struct Optional<T>
    {
        private readonly T value;
        private readonly bool isDefined;

        /// <summary>
        /// Initializes a new instance of the <see cref="Optional{T}"/> struct.
        /// </summary>
        /// <param name="value">The value to specify.</param>
        [DebuggerStepThrough]
        public Optional(T value)
        {
            this.isDefined = (value != null);
            this.value = value;
        }

        /// <summary>
        /// Gets an instance that indicates the value was not specified.
        /// </summary>
        public static Optional<T> Missing
        {
            [DebuggerStepThrough]
            get { return new Optional<T>(); }
        }

        /// <summary>
        /// Gets a value indicating whether the value was specified.
        /// </summary>
        public bool IsDefined
        {
            [DebuggerStepThrough]
            get { return this.isDefined; }
        }

        /// <summary>
        /// Gets the specified value, or the default value for the type if <see cref="IsDefined"/> is <c>false</c>.
        /// </summary>
        public T Value
        {
            [DebuggerStepThrough]
            get { return this.value; }
        }

        /// <summary>
        /// Implicitly wraps the specified value as an Optional.
        /// </summary>
        [DebuggerStepThrough]
        public static implicit operator Optional<T>(T value) => new Optional<T>(value);

        /// <summary>
        /// Gets the value that was given, or the specified fallback value if <see cref="IsDefined"/> is <c>false</c>.
        /// </summary>
        /// <param name="defaultValue">The default value to use if a value was not specified.</param>
        /// <returns>The value.</returns>
        public T GetValueOrDefault(T defaultValue) => this.IsDefined ? this.value : defaultValue;
    }

    public static class Optional
    {
        public static Optional<T> For<T>(T value) => value;
    }
}
