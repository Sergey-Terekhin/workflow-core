﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using WorkflowCore.Models;
using WorkflowCore.Persistence.EntityFramework.Models;

namespace WorkflowCore.Persistence.EntityFramework
{
    internal static class ExtensionMethods
    {
        private static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        internal static PersistedWorkflow ToPersistable(this WorkflowInstance instance, PersistedWorkflow persistable = null)
        {
            if (persistable == null)            
                persistable = new PersistedWorkflow();                        

            persistable.Data = JsonConvert.SerializeObject(instance.Data, SerializerSettings);
            persistable.Description = instance.Description;
            persistable.Reference = instance.Reference;
            persistable.InstanceId = new Guid(instance.Id);
            persistable.NextExecution = instance.NextExecution;
            persistable.Version = instance.Version;
            persistable.WorkflowDefinitionId = instance.WorkflowDefinitionId;
            persistable.Status = instance.Status;
            persistable.CreateTime = instance.CreateTime;
            if (!persistable.CompleteTime.HasValue)
            {
                persistable.CompleteTime = instance.CompleteTime;
            }
            persistable.ExecutionPointers = new PersistedExecutionPointerCollection(instance.ExecutionPointers.Count);
            
            foreach (var ep in instance.ExecutionPointers)
            {
                var persistedPointer = persistable.ExecutionPointers.FindById(ep.Id);
                
                if (persistedPointer == null)
                {
                    persistedPointer = new PersistedExecutionPointer();
                    persistedPointer.Id = ep.Id ?? Guid.NewGuid().ToString();
                    persistable.ExecutionPointers.Add(persistedPointer);
                }                 
                
                persistedPointer.StepId = ep.StepId;
                persistedPointer.Active = ep.Active;
                persistedPointer.SleepUntil = ep.SleepUntil;
                persistedPointer.PersistenceData = JsonConvert.SerializeObject(ep.PersistenceData, SerializerSettings);
                persistedPointer.StartTime = ep.StartTime;
                persistedPointer.EndTime = ep.EndTime;
                persistedPointer.StepName = ep.StepName;
                persistedPointer.RetryCount = ep.RetryCount;
                persistedPointer.PredecessorId = ep.PredecessorId;
                persistedPointer.ContextItem = JsonConvert.SerializeObject(ep.ContextItem, SerializerSettings);
                persistedPointer.Children = string.Empty;

                foreach (var child in ep.Children)
                    persistedPointer.Children += child + ";";

                persistedPointer.EventName = ep.EventName;
                persistedPointer.EventKey = ep.EventKey;
                persistedPointer.EventPublished = ep.EventPublished;
                persistedPointer.EventData = JsonConvert.SerializeObject(ep.EventData, SerializerSettings);
                persistedPointer.Outcome = JsonConvert.SerializeObject(ep.Outcome, SerializerSettings);
                persistedPointer.Status = ep.Status;

                persistedPointer.Scope = string.Empty;
                foreach (var item in ep.Scope)
                    persistedPointer.Scope += item + ";";

                foreach (var attr in ep.ExtensionAttributes)
                {
                    var persistedAttr = persistedPointer.ExtensionAttributes.FirstOrDefault(x => x.AttributeKey == attr.Key);
                    if (persistedAttr == null)
                    {
                        persistedAttr = new PersistedExtensionAttribute();
                        persistedPointer.ExtensionAttributes.Add(persistedAttr);
                    }

                    persistedAttr.AttributeKey = attr.Key;
                    persistedAttr.AttributeValue = JsonConvert.SerializeObject(attr.Value, SerializerSettings);
                }
            }

            return persistable;
        }

        internal static PersistedExecutionError ToPersistable(this ExecutionError instance)
        {
            var result = new PersistedExecutionError();            
            result.ErrorTime = instance.ErrorTime;
            result.Message = instance.Message;
            result.ExecutionPointerId = instance.ExecutionPointerId;
            result.WorkflowId = instance.WorkflowId;

            return result;
        }

        internal static PersistedSubscription ToPersistable(this EventSubscription instance)
        {
            PersistedSubscription result = new PersistedSubscription();            
            result.SubscriptionId = new Guid(instance.Id);
            result.EventKey = instance.EventKey;
            result.EventName = instance.EventName;
            result.StepId = instance.StepId;
            result.WorkflowId = instance.WorkflowId;
            result.SubscribeAsOf = DateTime.SpecifyKind(instance.SubscribeAsOf, DateTimeKind.Utc);

            return result;
        }

        internal static PersistedEvent ToPersistable(this Event instance)
        {
            PersistedEvent result = new PersistedEvent();
            result.EventId = new Guid(instance.Id);
            result.EventKey = instance.EventKey;
            result.EventName = instance.EventName;
            result.EventTime = DateTime.SpecifyKind(instance.EventTime, DateTimeKind.Utc);
            result.IsProcessed = instance.IsProcessed;
            result.EventData = JsonConvert.SerializeObject(instance.EventData, SerializerSettings);

            return result;
        }

        internal static WorkflowInstance ToWorkflowInstance(this PersistedWorkflow instance)
        {
            WorkflowInstance result = new WorkflowInstance();
            result.Data = JsonConvert.DeserializeObject(instance.Data, SerializerSettings);
            result.Description = instance.Description;
            result.Reference = instance.Reference;
            result.Id = instance.InstanceId.ToString();
            result.NextExecution = instance.NextExecution;
            result.Version = instance.Version;
            result.WorkflowDefinitionId = instance.WorkflowDefinitionId;
            result.Status = instance.Status;
            result.CreateTime = DateTime.SpecifyKind(instance.CreateTime, DateTimeKind.Utc);
            if (instance.CompleteTime.HasValue)
                result.CompleteTime = DateTime.SpecifyKind(instance.CompleteTime.Value, DateTimeKind.Utc);

            result.ExecutionPointers = new ExecutionPointerCollection(instance.ExecutionPointers.Count + 8);

            foreach (var ep in instance.ExecutionPointers)
            {
                var pointer = ExecutionPointer.Create();                

                pointer.Id = ep.Id;
                pointer.StepId = ep.StepId;
                pointer.Active = ep.Active;

                if (ep.SleepUntil.HasValue)
                    pointer.SleepUntil = DateTime.SpecifyKind(ep.SleepUntil.Value, DateTimeKind.Utc);

                pointer.PersistenceData = JsonConvert.DeserializeObject(ep.PersistenceData ?? string.Empty, SerializerSettings);

                if (ep.StartTime.HasValue)
                    pointer.StartTime = DateTime.SpecifyKind(ep.StartTime.Value, DateTimeKind.Utc);

                if (ep.EndTime.HasValue)
                    pointer.EndTime = DateTime.SpecifyKind(ep.EndTime.Value, DateTimeKind.Utc);

                pointer.StepName = ep.StepName;

                pointer.RetryCount = ep.RetryCount;
                pointer.PredecessorId = ep.PredecessorId;
                pointer.ContextItem = JsonConvert.DeserializeObject(ep.ContextItem ?? string.Empty, SerializerSettings);

                if (!string.IsNullOrEmpty(ep.Children))
                    pointer.Children = ep.Children.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                pointer.EventName = ep.EventName;
                pointer.EventKey = ep.EventKey;
                pointer.EventPublished = ep.EventPublished;
                pointer.EventData = JsonConvert.DeserializeObject(ep.EventData ?? string.Empty, SerializerSettings);
                pointer.Outcome = JsonConvert.DeserializeObject(ep.Outcome ?? string.Empty, SerializerSettings);
                pointer.Status = ep.Status;

                if (!string.IsNullOrEmpty(ep.Scope))
                    pointer.Scope = new List<string>(ep.Scope.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));

                foreach (var attr in ep.ExtensionAttributes)
                {
                    pointer.ExtensionAttributes[attr.AttributeKey] = JsonConvert.DeserializeObject(attr.AttributeValue, SerializerSettings);
                }

                result.ExecutionPointers.Add(pointer);
            }

            return result;
        }

        internal static EventSubscription ToEventSubscription(this PersistedSubscription instance)
        {
            EventSubscription result = new EventSubscription();
            result.Id = instance.SubscriptionId.ToString();
            result.EventKey = instance.EventKey;
            result.EventName = instance.EventName;
            result.StepId = instance.StepId;
            result.WorkflowId = instance.WorkflowId;
            result.SubscribeAsOf = DateTime.SpecifyKind(instance.SubscribeAsOf, DateTimeKind.Utc);

            return result;
        }

        internal static Event ToEvent(this PersistedEvent instance)
        {
            Event result = new Event();
            result.Id = instance.EventId.ToString();
            result.EventKey = instance.EventKey;
            result.EventName = instance.EventName;
            result.EventTime = DateTime.SpecifyKind(instance.EventTime, DateTimeKind.Utc);
            result.IsProcessed = instance.IsProcessed;
            result.EventData = JsonConvert.DeserializeObject(instance.EventData, SerializerSettings);

            return result;
        }
    }
}
