using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Persistence.EntityFramework.Models;
using WorkflowCore.Models;
using WorkflowCore.Persistence.EntityFramework.Interfaces;

namespace WorkflowCore.Persistence.EntityFramework.Services
{
    public class EntityFrameworkPersistenceProvider : IPersistenceProvider
    {
        private readonly bool _canCreateDatabase;
        private readonly bool _canMigrateDatabase;
        private readonly IWorkflowDbContextFactory _contextFactory;

        public EntityFrameworkPersistenceProvider(IWorkflowDbContextFactory contextFactory, bool canCreateDatabase,
            bool canMigrateDatabase)
        {
            _contextFactory = contextFactory;
            _canCreateDatabase = canCreateDatabase;
            _canMigrateDatabase = canMigrateDatabase;
        }

        /// <inheritdoc />
        public async Task<string> CreateEventSubscription(EventSubscription subscription)
        {
            using (var db = ConstructDbContext())
            {
                subscription.Id = Guid.NewGuid().ToString();
                var persistable = subscription.ToPersistable();
                await db.Set<PersistedSubscription>().AddAsync(persistable);
                await db.SaveChangesAsync();
                return subscription.Id;
            }
        }

        /// <inheritdoc />
        public async Task<string> CreateNewWorkflow(WorkflowInstance workflow)
        {
            using (var db = ConstructDbContext())
            {
                workflow.Id = Guid.NewGuid().ToString();
                var persistable = workflow.ToPersistable();
                await db.Set<PersistedWorkflow>().AddAsync(persistable);
                await db.SaveChangesAsync();
                return workflow.Id;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetRunnableInstances(DateTime asAt)
        {
            using (var db = ConstructDbContext())
            {
                var now = asAt.ToUniversalTime().Ticks;
                var raw = await db.Set<PersistedWorkflow>()
                    .Where(x => x.NextExecution.HasValue && x.NextExecution <= now &&
                                x.Status == WorkflowStatus.Runnable)
                    .Select(x => x.InstanceId)
                    .ToListAsync();

                return raw.Select(s => s.ToString()).ToList();
            }
        }


        /// <inheritdoc />
        public async Task<WorkflowInstance> GetWorkflowInstance(string id)
        {
            using (var db = ConstructDbContext())
            {
                var uid = new Guid(id);
                var raw = await db.Set<PersistedWorkflow>()
                    .Include(wf => wf.ExecutionPointers)
                    .ThenInclude(ep => ep.ExtensionAttributes)
                    .Include(wf => wf.ExecutionPointers)
                    .FirstAsync(x => x.InstanceId == uid);

                return raw?.ToWorkflowInstance();
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(IEnumerable<string> ids)
        {
            if (ids == null)
            {
                return new List<WorkflowInstance>();
            }

            using (var db = ConstructDbContext())
            {
                var uids = ids.Select(i => new Guid(i));
                var raw = db.Set<PersistedWorkflow>()
                    .Include(wf => wf.ExecutionPointers)
                    .ThenInclude(ep => ep.ExtensionAttributes)
                    .Include(wf => wf.ExecutionPointers)
                    .Where(x => uids.Contains(x.InstanceId));

                return (await raw.ToListAsync()).Select(i => i.ToWorkflowInstance());
            }
        }

        /// <inheritdoc />
        public async Task PersistWorkflow(WorkflowInstance workflow)
        {
            using (var db = ConstructDbContext())
            {
                var uid = new Guid(workflow.Id);
                var existingEntity = db.Set<PersistedWorkflow>()
                    .Where(x => x.InstanceId == uid)
                    .Include(wf => wf.ExecutionPointers)
                    .ThenInclude(ep => ep.ExtensionAttributes)
                    .Include(wf => wf.ExecutionPointers)
                    .AsTracking()
                    .First();

                workflow.ToPersistable(existingEntity);
                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc />
        public async Task TerminateSubscription(string eventSubscriptionId)
        {
            using (var db = ConstructDbContext())
            {
                var uid = new Guid(eventSubscriptionId);
                var existing = db.Set<PersistedSubscription>().First(x => x.SubscriptionId == uid);
                db.Set<PersistedSubscription>().Remove(existing);
                await db.SaveChangesAsync();
            }
        }

        /// <inheritdoc />
        public virtual void EnsureStoreExists()
        {
            using (var context = ConstructDbContext())
            {
                if (_canCreateDatabase && !_canMigrateDatabase)
                {
                    context.Database.EnsureCreated();
                    return;
                }

                if (_canMigrateDatabase)
                {
                    context.Database.Migrate();
                }
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventSubscription>> GetSubscriptions(string eventName, string eventKey,
            DateTime asOf)
        {
            using (var db = ConstructDbContext())
            {
                asOf = asOf.ToUniversalTime();
                var query = db.Set<PersistedSubscription>()
                    .Where(x => x.EventKey == eventKey && x.SubscribeAsOf <= asOf).AsQueryable();
                if (!string.IsNullOrEmpty(eventName))
                {
                    query = query.Where(x => x.EventName == eventName);
                }

                var raw = await query.ToListAsync();
                return raw.Select(item => item.ToEventSubscription()).ToList();
            }
        }

        /// <inheritdoc />
        public async Task<string> CreateEvent(Event newEvent)
        {
            using (var db = ConstructDbContext())
            {
                newEvent.Id = Guid.NewGuid().ToString();
                var persistable = newEvent.ToPersistable();
                await db.Set<PersistedEvent>().AddAsync(persistable);
                await db.SaveChangesAsync();
                return newEvent.Id;
            }
        }

        /// <inheritdoc />
        public async Task<Event> GetEvent(string id)
        {
            using (var db = ConstructDbContext())
            {
                var uid = new Guid(id);
                var raw = await db.Set<PersistedEvent>()
                    .FirstAsync(x => x.EventId == uid);

                return raw?.ToEvent();
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetRunnableEvents(DateTime asAt)
        {
            var now = asAt.ToUniversalTime();
            using (var db = ConstructDbContext())
            {
                var raw = await db.Set<PersistedEvent>()
                    .Where(x => !x.IsProcessed)
                    .Where(x => x.EventTime <= now)
                    .Select(x => x.EventId)
                    .ToListAsync();

                return raw.Select(s => s.ToString()).ToList();
            }
        }

        /// <inheritdoc />
        public async Task MarkEventProcessed(string id)
        {
            using (var db = ConstructDbContext())
            {
                var uid = new Guid(id);
                var existingEntity = await db.Set<PersistedEvent>()
                    .Where(x => x.EventId == uid)
                    .AsTracking()
                    .FirstAsync();

                existingEntity.IsProcessed = true;
                db.SaveChanges();
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<string>> GetEvents(string eventName, string eventKey, DateTime asOf)
        {
            using (var db = ConstructDbContext())
            {
                var raw = await db.Set<PersistedEvent>()
                    .Where(x => x.EventName == eventName && x.EventKey == eventKey)
                    .Where(x => x.EventTime >= asOf)
                    .Select(x => x.EventId)
                    .ToListAsync();

                var result = new List<string>();

                foreach (var s in raw)
                    result.Add(s.ToString());

                return result;
            }
        }

        /// <inheritdoc />
        public async Task MarkEventUnprocessed(string id)
        {
            using (var db = ConstructDbContext())
            {
                var uid = new Guid(id);
                var existingEntity = await db.Set<PersistedEvent>()
                    .Where(x => x.EventId == uid)
                    .AsTracking()
                    .FirstAsync();

                existingEntity.IsProcessed = false;
                db.SaveChanges();
            }
        }

        /// <inheritdoc />
        public async Task PersistErrors(IEnumerable<ExecutionError> errors)
        {
            using (var db = ConstructDbContext())
            {
                var executionErrors = errors as ExecutionError[] ?? errors.ToArray();
                if (executionErrors.Any())
                {
                    foreach (var error in executionErrors)
                    {
                        db.Set<PersistedExecutionError>().Add(error.ToPersistable());
                    }

                    await db.SaveChangesAsync();
                }
            }
        }

        private WorkflowDbContext ConstructDbContext()
        {
            return _contextFactory.Build();
        }
    }
}