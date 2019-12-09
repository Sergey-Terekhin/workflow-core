using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace WorkflowCore.Models
{
    /// <summary>
    /// Collection of execution pointers
    /// </summary>
    [PublicAPI]
    public class ExecutionPointerCollection : ICollection<IExecutionPointer>
    {
        private readonly Dictionary<string, IExecutionPointer> _dictionary = new Dictionary<string, IExecutionPointer>();

        private readonly Dictionary<string, List<IExecutionPointer>> _scopeMap =
            new Dictionary<string, List<IExecutionPointer>>();

        /// <summary>
        /// ctor
        /// </summary>
        public ExecutionPointerCollection()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public ExecutionPointerCollection(int capacity)
        {
            _dictionary = new Dictionary<string, IExecutionPointer>(capacity);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public ExecutionPointerCollection(ICollection<IExecutionPointer> pointers)
        {
            foreach (var ptr in pointers)
            {
                Add(ptr);
            }
        }

        /// <inheritdoc />
        public IEnumerator<IExecutionPointer> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns execution pointer by its identifier. If pointer does not exist, <c>null</c> is returned
        /// </summary>
        /// <param name="id">Identifier of execution pointer</param>
        /// <returns></returns>
        public IExecutionPointer FindById(string id)
        {
            if (!_dictionary.ContainsKey(id))
                return null;

            return _dictionary[id];
        }

        /// <summary>
        /// return collection of execution pointers which belong to the specified scope
        /// </summary>
        /// <param name="stackFrame">Identifier of the scope's owner (usually, it's parent execution pointer)</param>
        /// <returns></returns>
        // ReSharper disable once ReturnTypeCanBeEnumerable.Global
        public IReadOnlyCollection<IExecutionPointer> FindByScope(string stackFrame)
        {
            if (!_scopeMap.ContainsKey(stackFrame))
                return new List<IExecutionPointer>();

            return _scopeMap[stackFrame];
        }

        /// <inheritdoc />
        public void Add(IExecutionPointer item)
        {
            _dictionary.Add(item.Id, item);

            foreach (var stackFrame in item.Scope)
            {
                if (!_scopeMap.ContainsKey(stackFrame))
                    _scopeMap.Add(stackFrame, new List<IExecutionPointer>());
                _scopeMap[stackFrame].Add(item);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            _dictionary.Clear();
            _scopeMap.Clear();
        }

        /// <inheritdoc />
        public bool Contains(IExecutionPointer item)
        {
            return _dictionary.ContainsValue(item);
        }

        /// <inheritdoc />
        public void CopyTo(IExecutionPointer[] array, int arrayIndex)
        {
            _dictionary.Values.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(IExecutionPointer item)
        {
            foreach (var stackFrame in item.Scope)
            {
                _scopeMap[stackFrame].Remove(item);
            }

            return _dictionary.Remove(item.Id);
        }

        /// <summary>
        /// Returns execution pointer by predicate. If step does not exist, <c>null</c> is returned
        /// </summary>
        /// <param name="match">Predicate to use</param>
        /// <returns></returns>
        public IExecutionPointer Find(Predicate<IExecutionPointer> match)
        {
            return _dictionary.Values.FirstOrDefault(x => match(x));
        }

        /// <inheritdoc />
        public int Count => _dictionary.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;
    }
}