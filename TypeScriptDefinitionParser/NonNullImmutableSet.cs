using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace TypeScriptDefinitionParser
{
    namespace TypeScriptDefinitionParser
    {
        /// <summary>This exists so that a set can be known not only to be immutable but also to contain only non-null references</summary>
        [DebuggerDisplay("Count = {Count}")]
        public sealed class NonNullImmutableSet<T> : IEnumerable<T> where T : class
        {
            public static NonNullImmutableSet<T> Empty { get; } = new NonNullImmutableSet<T>(null);

            private readonly Optional<Node> _tail;
            private readonly Lazy<IEnumerable<T>> _allValues;
            private NonNullImmutableSet(Optional<Node> tail)
            {
                _tail = tail;
                _allValues = new Lazy<IEnumerable<T>>(() =>
                {
                    var itemCount = _tail.IsDefined ? (int)_tail.Value.Count : 0;
                    var values = new List<T>(itemCount);
                    var node = _tail;
                    for (var i = 0; i < itemCount; i++)
                    {
                        values.Insert(0, node.Value.Value);
                        node = node.Value.Previous;
                    }
                    return values.AsReadOnly();
                });
            }

            [DebuggerStepThrough]
            public NonNullImmutableSet<T> Add(T value)
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                return new NonNullImmutableSet<T>(new Node(value, _tail));
            }

            [DebuggerStepThrough]
            public NonNullImmutableSet<T> AddRange(IEnumerable<T> values)
            {
                if (values == null)
                    throw new ArgumentNullException(nameof(values));

                var updatedList = this;
                foreach (var value in values)
                {
                    if (value == null)
                        throw new ArgumentNullException(nameof(value), "null reference encountered in set");
                    updatedList = updatedList.Add(value);
                }
                return updatedList;
            }
            public uint Count => _tail.IsDefined ? _tail.Value.Count : 0;

            public bool Any() => Count > 0; // LINQ's Any method will use GetEnumerator, which is more expensive to call than a simple Count property check

            public IEnumerator<T> GetEnumerator() => _allValues.Value.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            private sealed class Node
            {
                public Node(T value, Optional<Node> previous)
                {
                    Value = value;
                    Previous = previous;
                    Count = (previous.IsDefined) ? (previous.Value.Count + 1) : 1;
                }
                public T Value { get; }
                public Optional<Node> Previous { get; }
                public uint Count { get; }
            }
        }
    }
}