﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace WorkflowCore.Models
{
    /// <summary>
    /// Model for execution result
    /// </summary>
    [PublicAPI]
    public class ExecutionResult
    {
        public bool Proceed { get; set; }
        /// <summary>
        /// Set if workflow was terminated
        /// </summary>
        public bool Terminated { get; set; }
        public object OutcomeValue { get; set; }

        public TimeSpan? SleepFor { get; set; }

        public object PersistenceData { get; set; }

        /// <summary>
        /// Event name to wait
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// Event names to wait
        /// </summary>
        public List<string> EventsNames { get; set; }
        /// <summary>
        /// Event key to wait
        /// </summary>
        public string EventKey { get; set; }

        public DateTime EventAsOf { get; set; }

        public List<object> BranchValues { get; set; } = new List<object>();

        /// <summary>
        /// ctor
        /// </summary>
        public ExecutionResult()
        {
        }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="outcome"></param>
        public ExecutionResult(object outcome)
        {
            Proceed = true;
            OutcomeValue = outcome;
        }

        public static ExecutionResult Outcome(object value)
        {
            return new ExecutionResult
            {
                Proceed = true,
                OutcomeValue = value
            };
        }

        public static ExecutionResult Next()
        {
            return new ExecutionResult
            {
                Proceed = true,
                OutcomeValue = null
            };
        }

        public static ExecutionResult Persist(object persistenceData)
        {
            return new ExecutionResult
            {
                Proceed = false,
                PersistenceData = persistenceData
            };
        }

        public static ExecutionResult Branch(List<object> branches, object persistenceData)
        {
            return new ExecutionResult
            {
                Proceed = false,
                PersistenceData = persistenceData,
                BranchValues = branches
            };
        }

        public static ExecutionResult Sleep(TimeSpan duration, object persistenceData)
        {
            return new ExecutionResult
            {
                Proceed = false,
                SleepFor = duration,
                PersistenceData = persistenceData
            };
        }

        public static ExecutionResult WaitForEvent(string eventName, string eventKey, DateTime effectiveDate)
        {
            return new ExecutionResult
            {
                Proceed = false,
                EventName = eventName,
                EventKey = eventKey,
                EventAsOf = effectiveDate.ToUniversalTime()
            };
        }

        public static ExecutionResult WaitForMultipleEvents(List<string> eventsNames, string eventKey, DateTime effectiveDate)
        {
            return new ExecutionResult
            {
                Proceed = false,
                EventsNames = eventsNames,
                EventKey = eventKey,
                EventAsOf = effectiveDate.ToUniversalTime()
            };
        }

        public static ExecutionResult Terminate()
        {
            return new ExecutionResult
            {
                Terminated = true
            };
        }
    }
}