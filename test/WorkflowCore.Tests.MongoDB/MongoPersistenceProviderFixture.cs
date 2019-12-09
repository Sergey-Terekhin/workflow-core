﻿using MongoDB.Driver;
using System;
using WorkflowCore.Interface;
using WorkflowCore.Persistence.MongoDB.Services;
using WorkflowCore.UnitTests;
using Xunit;

namespace WorkflowCore.Tests.MongoDB
{
    [Collection("Mongo collection")]
    public class MongoPersistenceProviderFixture : BasePersistenceFixture
    {
        protected override IPersistenceProvider Subject
        {
            get
            {
                var client = new MongoClient(MongoDockerSetup.ConnectionString);
                var db = client.GetDatabase("workflow-tests");
                return new MongoPersistenceProvider(db);
            }
        }
    }
}
