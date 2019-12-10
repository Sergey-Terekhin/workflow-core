using System;
using System.Linq;
using WorkflowCore.Interface;

namespace WorkflowCore.Models
{
    /// <summary>
    /// Implementation of <see cref="IStepParameter"/> which accepts action to connect step body and provided data
    /// </summary>
    /// <typeparam name="TStepBody">Type of step body</typeparam>
    /// <typeparam name="TData">Type of workflow data</typeparam>
    public class ActionParameter<TStepBody, TData> : IStepParameter
    {
        private readonly Action<TStepBody, TData> _action;
     
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="action">Action to connect step body and provided data</param>
        public ActionParameter(Action<TStepBody, TData> action)
        {
            _action = action;
        }

        private void Assign(object data, IStepBody step)
        {
            _action.Invoke((TStepBody)step, (TData)data);
        }

        /// <inheritdoc />
        public void AssignInput(object data, IStepBody body, IStepExecutionContext context)
        {
            Assign(data, body);
        }

        /// <inheritdoc />
        public void AssignOutput(object data, IStepBody body, IStepExecutionContext context)
        {
            Assign(data, body);
        }
    }
}
