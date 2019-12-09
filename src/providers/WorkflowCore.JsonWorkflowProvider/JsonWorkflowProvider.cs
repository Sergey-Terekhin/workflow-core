﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;
using WorkflowCore.Exceptions;
using WorkflowCore.Interface;
using WorkflowCore.Models;
using WorkflowCore.Models.DefinitionStorage.v1;
using WorkflowCore.Primitives;

namespace WorkflowCore.JsonWorkflowProvider
{
    /// <summary>
    /// Implementation of <see cref="IWorkflowProvider"/> which reads workflow definition from json
    /// </summary>
    [PublicAPI]
    public class JsonWorkflowProvider : IWorkflowProvider
    {
        private readonly JsonSerializer _serializer = JsonSerializer.CreateDefault();

        /// <inheritdoc />
        public Task<WorkflowDefinition> LoadDefinition(TextReader reader, CancellationToken token)
        {
            using var jsonReader = new JsonTextReader(reader);
            var source = _serializer.Deserialize<DefinitionSourceV1>(jsonReader);

            var def = Convert(source);
            return Task.FromResult(def);
        }

        private static WorkflowDefinition Convert(DefinitionSourceV1 source)
        {
            var dataType = typeof(object);
            if (!string.IsNullOrEmpty(source.DataType))
                dataType = FindType(source.DataType);

            var result = new WorkflowDefinition
            {
                Id = source.Id,
                Version = source.Version,
                Steps = ConvertSteps(source.Steps, dataType),
                DefaultErrorBehavior = source.DefaultErrorBehavior,
                DefaultErrorRetryInterval = source.DefaultErrorRetryInterval,
                Description = source.Description,
                DataType = dataType
            };

            return result;
        }

        private static WorkflowStepCollection ConvertSteps(ICollection<StepSourceV1> source, Type dataType)
        {
            var result = new WorkflowStepCollection();
            int i = 0;
            var stack = new Stack<StepSourceV1>(source.Reverse());
            var parents = new List<StepSourceV1>();
            var compensatables = new List<StepSourceV1>();

            while (stack.Count > 0)
            {
                var nextStep = stack.Pop();

                var stepType = FindType(nextStep.StepType);
                var containerType = nextStep.Saga
                    ? typeof(SagaContainer<>).MakeGenericType(stepType)
                    : typeof(WorkflowStep<>).MakeGenericType(stepType);
                
                var constructor = containerType.GetConstructor(Type.EmptyTypes);
                if (constructor == null)
                    throw new InvalidOperationException(
                        $"Unable to find constructor without parameters for the step type: {containerType}");
                
                var targetStep = (WorkflowStep) constructor.Invoke(null);

                if (!string.IsNullOrEmpty(nextStep.CancelCondition))
                {
                    var dataParameter = Expression.Parameter(dataType, "data");
                    var cancelExpr = DynamicExpressionParser.ParseLambda(new[] {dataParameter}, typeof(bool),
                        nextStep.CancelCondition);
                    targetStep.CancelCondition = cancelExpr;
                }

                targetStep.Id = i;
                targetStep.Name = nextStep.Name;
                targetStep.ErrorBehavior = nextStep.ErrorBehavior;
                targetStep.RetryInterval = nextStep.RetryInterval;
                targetStep.ExternalId = $"{nextStep.Id}";

                AttachInputs(nextStep, dataType, stepType, targetStep);
                AttachOutputs(nextStep, dataType, stepType, targetStep);

                if (nextStep.Do != null)
                {
                    foreach (var branch in nextStep.Do)
                    {
                        foreach (var child in branch.Reverse<StepSourceV1>())
                            stack.Push(child);
                    }

                    if (nextStep.Do.Count > 0)
                        parents.Add(nextStep);
                }

                if (nextStep.CompensateWith != null)
                {
                    foreach (var compChild in nextStep.CompensateWith.Reverse<StepSourceV1>())
                        stack.Push(compChild);

                    if (nextStep.CompensateWith.Count > 0)
                        compensatables.Add(nextStep);
                }

                if (!string.IsNullOrEmpty(nextStep.NextStepId))
                    targetStep.Outcomes.Add(new StepOutcome {ExternalNextStepId = $"{nextStep.NextStepId}"});

                result.Add(targetStep);

                i++;
            }

            foreach (var step in result)
            {
                if (result.Any(x => x.ExternalId == step.ExternalId && x.Id != step.Id))
                    throw new WorkflowDefinitionLoadException($"Duplicate step Id {step.ExternalId}");

                foreach (var outcome in step.Outcomes)
                {
                    if (result.All(x => x.ExternalId != outcome.ExternalNextStepId))
                        throw new WorkflowDefinitionLoadException($"Cannot find step id {outcome.ExternalNextStepId}");

                    outcome.NextStep = result.Single(x => x.ExternalId == outcome.ExternalNextStepId).Id;
                }
            }

            foreach (var parent in parents)
            {
                var target = result.Single(x => x.ExternalId == parent.Id);
                foreach (var branch in parent.Do)
                {
                    var childTags = branch.Select(x => x.Id).ToList();
                    target.Children.AddRange(result
                        .Where(x => childTags.Contains(x.ExternalId))
                        .OrderBy(x => x.Id)
                        .Select(x => x.Id)
                        .Take(1)
                        .ToList());
                }
            }

            foreach (var item in compensatables)
            {
                var target = result.Single(x => x.ExternalId == item.Id);
                var tag = item.CompensateWith.Select(x => x.Id).FirstOrDefault();
                if (tag != null)
                {
                    var compStep = result.FirstOrDefault(x => x.ExternalId == tag);
                    if (compStep != null)
                        target.CompensationStepId = compStep.Id;
                }
            }

            return result;
        }

        private static void AttachInputs(StepSourceV1 source, Type dataType, Type stepType, WorkflowStep step)
        {
            foreach (var input in source.Inputs)
            {
                var dataParameter = Expression.Parameter(dataType, "data");
                var contextParameter = Expression.Parameter(typeof(IStepExecutionContext), "context");
                var sourceExpr = DynamicExpressionParser.ParseLambda(new[] {dataParameter, contextParameter},
                    typeof(object), input.Value);

                var stepParameter = Expression.Parameter(stepType, "step");
                var targetProperty = Expression.Property(stepParameter, input.Key);
                var targetExpr = Expression.Lambda(targetProperty, stepParameter);

                step.Inputs.Add(new MemberMapParameter(sourceExpr, targetExpr));
            }
        }

        private static void AttachOutputs(StepSourceV1 source, Type dataType, Type stepType, WorkflowStep step)
        {
            foreach (var output in source.Outputs)
            {
                var stepParameter = Expression.Parameter(stepType, "step");
                var sourceExpr =
                    DynamicExpressionParser.ParseLambda(new[] {stepParameter}, typeof(object), output.Value);

                var dataParameter = Expression.Parameter(dataType, "data");
                Expression targetProperty;

                // Check if our datatype has a matching property
                var propertyInfo = dataType.GetProperty(output.Key);
                if (propertyInfo != null)
                {
                    targetProperty = Expression.Property(dataParameter, propertyInfo);
                    var targetExpr = Expression.Lambda(targetProperty, dataParameter);
                    step.Outputs.Add(new MemberMapParameter(sourceExpr, targetExpr));
                }
                else
                {
                    Action<IStepBody, object> acn;
                    // If we did not find a matching property try to find a Indexer with string parameter
                    propertyInfo = dataType.GetProperty("Item");
                    if (propertyInfo == null)
                    {
                        //if dict in context
                        acn = (pStep, pData) =>
                        {
                            object resolvedValue = sourceExpr.Compile().DynamicInvoke(pStep);
                            var targetExpr = DynamicExpressionParser.ParseLambda(new[] {Expression.Parameter(dataType)},
                                null,
                                output.Key, null);
                            var setter = CreateSetter(targetExpr);
                            setter.Compile().DynamicInvoke(pData, resolvedValue);
                        };
                    }

                    else
                    {
                        acn = (pStep, pData) =>
                        {
                            object resolvedValue = sourceExpr.Compile().DynamicInvoke(pStep);
                            propertyInfo.SetValue(pData, resolvedValue, new object[] {output.Key});
                        };
                    }

                    step.Outputs.Add(new ActionParameter<IStepBody, object>(acn));
                }
            }
        }

        private static Type FindType(string name)
        {
            return Type.GetType(name, true, true);
        }

        private static LambdaExpression CreateSetter(LambdaExpression getterExpression)
        {
            var valueParameter = Expression.Parameter(getterExpression.ReturnType, "value");
            Expression assignment;
            if (getterExpression.Body is MethodCallExpression callExpression &&
                callExpression.Method.Name == "get_Item")
            {
                //Get Matching setter method for the indexer
                var parameterTypes = callExpression.Method.GetParameters()
                    .Select(p => p.ParameterType)
                    .ToArray();
                
                Debug.Assert(callExpression.Method.DeclaringType != null, "callExpression.Method.DeclaringType != null");
                
                var itemProperty = callExpression.Method.DeclaringType.GetProperty("Item", valueParameter.Type, parameterTypes);

                Debug.Assert(itemProperty != null, nameof(itemProperty) + " != null");
                
                assignment = Expression.Call(
                    callExpression.Object, 
                    itemProperty.SetMethod,
                    callExpression.Arguments.Concat(new[] {valueParameter}));
            }
            else
            {
                assignment = Expression.Assign(getterExpression.Body, valueParameter);
            }

            var parameters = getterExpression.Parameters.Concat(new[] {valueParameter}).ToArray();
            return Expression.Lambda(assignment, parameters);
        }
    }
}