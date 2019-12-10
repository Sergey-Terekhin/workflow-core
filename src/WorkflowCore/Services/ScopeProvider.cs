using Microsoft.Extensions.DependencyInjection;
using System;
using WorkflowCore.Interface;

namespace WorkflowCore.Services
{
    /// <summary>
    /// A concrete implementation for the <see cref="IScopeProvider"/> interface
    /// Largely to get around the problems of unit testing an extension method (CreateScope())
    /// </summary>
    public class ScopeProvider : IScopeProvider
    {
        private readonly IServiceProvider _provider;

        /// <summary>
        /// ctor
        /// </summary>
        public ScopeProvider(IServiceProvider provider)
        {
            this._provider = provider;
        }

        /// <inheritdoc />
        public IServiceScope CreateScope()
        {
            return _provider.CreateScope();
        }
    }
}
