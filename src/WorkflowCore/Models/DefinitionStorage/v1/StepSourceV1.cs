using System;
using System.Collections.Generic;
using JetBrains.Annotations;
#pragma warning disable 1574,1584,1581,1580

namespace WorkflowCore.Models.DefinitionStorage.v1
{
    /// <summary>
    /// Step definition source which can be serialized and stored
    /// </summary>
    [PublicAPI]
    public class StepSourceV1
    {
        /// <summary>
        /// Step type. Used to create specific instance of <see cref="WorkflowStep"/>
        /// </summary>
        public string StepType { get; set; }
        
        /// <summary>
        /// Step identifier
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Step name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Serialized representation of <see cref="WorkflowStep.CancelCondition"/>
        /// </summary>
        public string CancelCondition { get; set; }

        /// <summary>
        /// Error handling policy for step. If not set, policy for workflow will be used
        /// </summary>
        public WorkflowErrorHandling? ErrorBehavior { get; set; }

        /// <summary>
        /// Retry interval for step. If not set, retry interval for workflow will be used
        /// </summary>
        public TimeSpan? RetryInterval { get; set; }

        /// <summary>
        /// List of child steps to execute
        /// </summary>
        public List<List<StepSourceV1>> Do { get; set; } = new List<List<StepSourceV1>>();

        /// <summary>
        /// List of compensation steps. Used if <see cref="ErrorBehavior"/> is set to <see cref="WorkflowErrorHandling.Compensate"/>
        /// </summary>
        public List<StepSourceV1> CompensateWith { get; set; } = new List<StepSourceV1>();

        /// <summary>
        /// If set to <c>true</c>, step is marked as Saga (see <see cref="https://microservices.io/patterns/data/saga.html"/>)
        /// </summary>
        public bool Saga { get; set; }

        /// <summary>
        /// Identifier of the next step
        /// </summary>
        public string NextStepId { get; set; }

        /// <summary>
        /// List of step's inputs
        /// </summary>
        public Dictionary<string, string> Inputs { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// List of step's outputs
        /// </summary>
        public Dictionary<string, string> Outputs { get; set; } = new Dictionary<string, string>();

        
    }
}
