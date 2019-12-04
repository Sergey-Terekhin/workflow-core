﻿using Microsoft.EntityFrameworkCore;
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
        private readonly bool _canCreateDB;
        private readonly bool _canMigrateDB;
        private readonly IWorkflowDbContextFactory _contextFactory;

        public EntityFrameworkPersistenceProvider(IWorkflowDbContextFactory contextFactory, bool canCreateDB, bool canMigrateDB)
        {
            _contextFactory = contextFactory;
            _canCreateDB = canCreateDB;
            _canMigrateDB = canMigrateDB;
        }

        public async Task<string> CreateEventSubscription(EventSubscription subscription)
        {
            using (var db = ConstructDbContext())
            {
                subscription.Id = Guid.NewGuid().ToString();
                var persistable = subscription.ToPersistable();
                var result = db.Set<PersistedSubscription>().Add(persistable);
                db.SaveChanges();
                return subscription.Id;
            }
        }

        public async Task<string> CreateNewWorkflow(WorkflowInstance workflow)
        {
            using (var db = ConstructDbContext())
            {
                workflow.Id = Guid.NewGuid().ToString();
                var persistable = workflow.ToPersistable();
                var result = db.Set<PersistedWorkflow>().Add(persistable);
                db.SaveChanges();
                return workflow.Id;
            }
        }

        public async Task<IEnumerable<string>> GetRunnableInstances(DateTime asAt)
        {
            using (var db = ConstructDbContext())
            {
                var now = asAt.ToUniversalTime().Ticks;
                var raw = db.Set<PersistedWorkflow>()
                    .Where(x => x.NextExecution.HasValue && (x.NextExecution <= now) && (x.Status == WorkflowStatus.Runnable))
                    .Select(x => x.InstanceId)
                    .ToList();

                return raw.Select(s => s.ToString()).ToList();
            }
        }

        public async Task<IEnumerable<WorkflowInstance>> GetWorkflowInstances(WorkflowStatus? status, string type, DateTime? createdFrom, DateTime? createdTo, int skip, int take)
        {
            using (var db = ConstructDbContext())
            {
                IQueryable<PersistedWorkflow> query = db.Set<PersistedWorkflow>()
                    .Include(wf => wf.ExecutionPointers)
                    .ThenInclude(ep => ep.ExtensionAttributes)
                    .Include(wf => wf.ExecutionPointers)
                    .AsQueryable();

                if (status.HasValue)
                    query = query.Where(x => x.Status == status.Value);

                if (!String.IsNullOrEmpty(type))
                    query = query.Where(x => x.WorkflowDefinitionId == type);

                if (createdFrom.HasValue)
                    query = query.Where(x => x.CreateTime >= createdFrom.Value);

                if (createdTo.HasValue)
                    query = query.Where(x => x.CreateTime <= createdTo.Value);

                var rawResult = query.Skip(skip).Take(take).ToList();
                List<WorkflowInstance> result = new List<WorkflowInstance>();

                foreach (var item in rawResult)
                    result.Add(item.ToWorkflowInstance());

                return result;
            }
        }

        public async Task<WorkflowInstance> GetWorkflowInstance(string Id)
        {
            using (var db = ConstructDbContext())
            {
                var uid = new Guid(Id);
                var raw = db.Set<PersistedWorkflow>()
                    .Include(wf => wf.ExecutionPointers)
                    .ThenInclude(ep => ep.ExtensionAttributes)
                    .Include(wf => wf.ExecutionPointers)
                    .First(x => x.InstanceId == uid);

                if (raw == null)
                    return null;

                return raw.ToWorkflowInstance();
            }
        }

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

                return (raw.ToList()).Select(i => i.ToWorkflowInstance());
            }
        }

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

                var persistable = workflow.ToPersistable(existingEntity);
                db.SaveChanges();
            }
        }

        public async Task TerminateSubscription(string eventSubscriptionId)
        {
            using (var db = ConstructDbContext())
            {
                var uid = new Guid(eventSubscriptionId);
                var existing = db.Set<PersistedSubscription>().First(x => x.SubscriptionId == uid);
                db.Set<PersistedSubscription>().Remove(existing);
                db.SaveChanges();
            }
        }

        public virtual void EnsureStoreExists()
        {
            using (var context = ConstructDbContext())
            {
                if (_canCreateDB && !_canMigrateDB)
                {
                    context.Database.EnsureCreated();
                    return;
                }

                if (_canMigrateDB)
                {
                    context.Database.Migrate();
                    return;
                }
            }
        }

        public async Task<IEnumerable<EventSubscription>> GetSubcriptions(string eventName, string eventKey, DateTime asOf)
        {
            using (var db = ConstructDbContext())
            {
                asOf = asOf.ToUniversalTime();
                var querry = db.Set<PersistedSubscription>()
                    .Where(x => x.EventKey == eventKey && x.SubscribeAsOf <= asOf).AsQueryable();
                if (!string.IsNullOrEmpty(eventName))
                {
                    querry = querry.Where(x => x.EventName == eventName);
                }

                var raw = querry.ToList();
                return raw.Select(item => item.ToEventSubscription()).ToList();
            }
        }

        public async Task<string> CreateEvent(Event newEvent)
        {
            using (var db = ConstructDbContext())
            {
                newEvent.Id = Guid.NewGuid().ToString();
                var persistable = newEvent.ToPersistable();
                var result = db.Set<PersistedEvent>().Add(persistable);
                db.SaveChanges();
                return newEvent.Id;
            }
        }

        public async Task<Event> GetEvent(string id)
        {
            using (var db = ConstructDbContext())
            {
                Guid uid = new Guid(id);
                var raw = db.Set<PersistedEvent>()
                    .First(x => x.EventId == uid);

                if (raw == null)
                    return null;

                return raw.ToEvent();
            }
        }

        public async Task<IEnumerable<string>> GetRunnableEvents(DateTime asAt)
        {
            var now = asAt.ToUniversalTime();
            using (var db = ConstructDbContext())
            {
                asAt = asAt.ToUniversalTime();
                var raw = db.Set<PersistedEvent>()
                    .Where(x => !x.IsProcessed)
                    .Where(x => x.EventTime <= now)
                    .Select(x => x.EventId)
                    .ToList();

                return raw.Select(s => s.ToString()).ToList();
            }
        }

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

        public async Task<IEnumerable<string>> GetEvents(string eventName, string eventKey, DateTime asOf)
        {
            using (var db = ConstructDbContext())
            {
                var raw = db.Set<PersistedEvent>()
                    .Where(x => x.EventName == eventName && x.EventKey == eventKey)
                    .Where(x => x.EventTime >= asOf)
                    .Select(x => x.EventId)
                    .ToList();

                var result = new List<string>();

                foreach (var s in raw)
                    result.Add(s.ToString());

                return result;
            }
        }

        public async Task MarkEventUnprocessed(string id)
        {
            using (var db = ConstructDbContext())
            {
                var uid = new Guid(id);
                var existingEntity = db.Set<PersistedEvent>()
                    .Where(x => x.EventId == uid)
                    .AsTracking()
                    .First();

                existingEntity.IsProcessed = false;
                db.SaveChanges();
            }
        }

        public async Task RemoveEventsByKey(string eventKey)
        {
            using (var db = ConstructDbContext())
            {
                var rowsToDelete = db.Set<PersistedEvent>().Where(x => x.EventKey == eventKey);
                if (rowsToDelete.Any())
                {
                    db.Set<PersistedEvent>().RemoveRange(rowsToDelete);
                }

                db.SaveChanges();
            }
        }

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
                    db.SaveChanges();

                }
            }
        }

        private WorkflowDbContext ConstructDbContext()
        {
            return _contextFactory.Build();
        }

    }
}
