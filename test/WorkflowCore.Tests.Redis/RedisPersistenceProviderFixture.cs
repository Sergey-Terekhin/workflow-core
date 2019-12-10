using System;
using WorkflowCore.Interface;
using WorkflowCore.Providers.Redis.Services;
using WorkflowCore.UnitTests;
using Xunit;

namespace WorkflowCore.Tests.Redis
{
    [Collection("Redis collection")]
    public class RedisPersistenceProviderFixture : BasePersistenceFixture
    {
        private IPersistenceProvider _subject;

        protected override IPersistenceProvider Subject
        {
            get
            {
                if (_subject == null)
                {
                    var client = new RedisPersistenceProvider(RedisDockerSetup.ConnectionString, "test");
                    client.EnsureStoreExists();
                    _subject = client;
                }
                return _subject;
            }
        }
    }
}
