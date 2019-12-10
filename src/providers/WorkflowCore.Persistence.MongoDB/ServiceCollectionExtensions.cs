using MongoDB.Driver;
using System;
using System.Linq;
using WorkflowCore.Models;
using WorkflowCore.Persistence.MongoDB.Services;
// ReSharper disable CheckNamespace
// ReSharper disable UnusedMethodReturnValue.Global

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static WorkflowOptions UseMongoDB(this WorkflowOptions options, string mongoUrl, string databaseName)
        {
            options.UsePersistence(sp =>
            {
                var client = new MongoClient(mongoUrl);
                var db = client.GetDatabase(databaseName);
                return new MongoPersistenceProvider(db);
            });
            return options;
        }
    }
}
