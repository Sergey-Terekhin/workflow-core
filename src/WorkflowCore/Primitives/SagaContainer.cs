using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace WorkflowCore.Primitives
{
    public class SagaContainer<TStepBody> : WorkflowStep<TStepBody>
        where TStepBody : IStepBody
    {
        /// <inheritdoc />
        public override bool ResumeChildrenAfterCompensation => false;
        /// <inheritdoc />
        public override bool RevertChildrenAfterCompensation => true;

        /// <inheritdoc />
        public override void PrimeForRetry(IExecutionPointer pointer)
        {
            base.PrimeForRetry(pointer);
            pointer.PersistenceData = null;
        }
    }
}
