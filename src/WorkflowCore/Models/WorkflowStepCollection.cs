using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace WorkflowCore.Models
{
    /// <summary>
    /// Collection of workflow steps
    /// </summary>
    [PublicAPI]
    // ReSharper disable once InconsistentNaming
    public class WorkflowStepCollection : ICollection<WorkflowStep>
    {
        private readonly Dictionary<int, WorkflowStep> _dictionary = new Dictionary<int, WorkflowStep>();
        
        /// <summary>
        /// ctor
        /// </summary>
        public WorkflowStepCollection()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        public WorkflowStepCollection(int capacity)
        {
            _dictionary = new Dictionary<int, WorkflowStep>(capacity);
        }

        /// <summary>
        /// ctor
        /// </summary>
        public WorkflowStepCollection(ICollection<WorkflowStep> steps)
        {
            foreach (var step in steps)
            {
                Add(step);
            }
        }

        /// <inheritdoc />
        public IEnumerator<WorkflowStep> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Returns step by its identifier. If step does not exist, <c>null</c> is returned
        /// </summary>
        /// <param name="id">Step identifier</param>
        /// <returns></returns>
        public WorkflowStep FindById(int id)
        {
            if (!_dictionary.ContainsKey(id))
                return null;

            return _dictionary[id];
        }

        /// <inheritdoc />
        public void Add(WorkflowStep item)
        {
            if (item != null) _dictionary.Add(item.Id, item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _dictionary.Clear();
        }

        /// <inheritdoc />
        public bool Contains(WorkflowStep item)
        {
            return _dictionary.ContainsValue(item);
        }

        /// <inheritdoc />
        public void CopyTo(WorkflowStep[] array, int arrayIndex)
        {
            _dictionary.Values.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove(WorkflowStep item)
        {
            return item != null && _dictionary.Remove(item.Id);
        }

        /// <summary>
        /// Returns step by predicate. If step does not exist, <c>null</c> is returned
        /// </summary>
        /// <param name="match">Predicate to use</param>
        /// <returns></returns>
        public WorkflowStep Find(Predicate<WorkflowStep> match)
        {
            return _dictionary.Values.FirstOrDefault(x => match(x));
        }

        /// <inheritdoc />
        public int Count => _dictionary.Count;

        /// <inheritdoc />
        public bool IsReadOnly => false;
    }
}
