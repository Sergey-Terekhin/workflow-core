using System;
using System.Linq.Expressions;

namespace WorkflowCore.Models
{
    public class StepOutcome
    {
        private Expression<Func<object, object>> _value;

        /// <summary>
        /// Expression to use in <see cref="GetValue"/>
        /// </summary>
        public Expression<Func<object, object>> Value
        {
            set => _value = value;
        }
        
        /// <summary>
        /// Identifier of the next step
        /// </summary>
        public int NextStep { get; set; }

        /// <summary>
        /// User-defined label
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// External identifier of the next step
        /// </summary>
        public string ExternalNextStepId { get; set; }

        /// <summary>
        /// Calculates and returns value from workflow data
        /// </summary>
        /// <param name="data">Value provided by <see cref="WorkflowInstance"/>.<see cref="WorkflowInstance.Data"/></param>
        /// <returns></returns>
        public object GetValue(object data)
        {
            return _value?.Compile()(data);
        }
    }
}
