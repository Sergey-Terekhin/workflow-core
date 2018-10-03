using System;
using WorkflowCore.Interface;
using WorkflowCore.Services;

namespace WorkflowCore.UnitTests.Services
{
    public class MemoryPersistenceProviderFixture : BasePersistenceFixture
    {
        protected override IPersistenceProvider Subject => new MemoryPersistenceProvider();
    }
}
