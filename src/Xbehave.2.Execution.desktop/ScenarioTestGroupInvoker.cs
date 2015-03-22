﻿// <copyright file="ScenarioTestGroupInvoker.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Execution
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Xbehave.Sdk;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class ScenarioTestGroupInvoker
    {
        private readonly ExecutionTimer timer = new ExecutionTimer();
        private readonly int scenarioNumber;
        private readonly IScenarioTestGroup testGroup;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> beforeAfterTestGroupAttributes;
        private readonly Stack<BeforeAfterTestAttribute> beforeAfterTestGroupAttributesRun =
            new Stack<BeforeAfterTestAttribute>();

        public ScenarioTestGroupInvoker(
            int scenarioNumber,
            IScenarioTestGroup testGroup,
            IMessageBus messageBus,
            Type testClass,
            object[] constructorArguments,
            MethodInfo testMethod,
            object[] testMethodArguments,
            IReadOnlyList<BeforeAfterTestAttribute> beforeAfterTestGroupAttributes,
            ExceptionAggregator aggregator,
            CancellationTokenSource cancellationTokenSource)
        {
            this.testGroup = testGroup;
            this.scenarioNumber = scenarioNumber;
            this.MessageBus = messageBus;
            this.TestClass = testClass;
            this.ConstructorArguments = constructorArguments;
            this.TestMethod = testMethod;
            this.TestMethodArguments = testMethodArguments;
            this.beforeAfterTestGroupAttributes = beforeAfterTestGroupAttributes;
            this.Aggregator = aggregator;
            this.CancellationTokenSource = cancellationTokenSource;
        }

        protected int ScenarioNumber
        {
            get { return this.scenarioNumber; }
        }

        protected IScenarioTestGroup TestGroup
        {
            get { return this.testGroup; }
        }

        protected IMessageBus MessageBus { get; set; }

        protected Type TestClass { get; set; }

        protected object[] ConstructorArguments { get; set; }

        protected MethodInfo TestMethod { get; set; }

        protected object[] TestMethodArguments { get; set; }

        protected IReadOnlyList<BeforeAfterTestAttribute> BeforeAfterTestGroupAttributes
        {
            get { return this.beforeAfterTestGroupAttributes; }
        }

        protected ExceptionAggregator Aggregator { get; set; }

        protected CancellationTokenSource CancellationTokenSource { get; set; }

        protected ExecutionTimer Timer
        {
            get { return this.timer; }
        }

        public Task<RunSummary> RunAsync()
        {
            var summary = new RunSummary();
            return this.Aggregator.RunAsync(async () =>
            {
                if (!CancellationTokenSource.IsCancellationRequested)
                {
                    var testClassInstance = CreateTestClass();

                    if (!CancellationTokenSource.IsCancellationRequested)
                    {
                        await BeforeTestMethodInvokedAsync();

                        if (!this.CancellationTokenSource.IsCancellationRequested && !this.Aggregator.HasExceptions)
                        {
                            summary.Aggregate(await InvokeTestMethodAsync(testClassInstance));
                        }

                        await AfterTestMethodInvokedAsync();
                    }

                    var disposable = testClassInstance as IDisposable;
                    if (disposable != null)
                    {
                        timer.Aggregate(() => Aggregator.Run(disposable.Dispose));
                    }
                }

                summary.Time += this.timer.Total;
                return summary;
            });
        }

        protected virtual object CreateTestClass()
        {
            object testClass = null;

            if (!this.TestMethod.IsStatic && !this.Aggregator.HasExceptions)
            {
                this.timer.Aggregate(() => testClass = Activator.CreateInstance(this.TestClass, this.ConstructorArguments));
            }

            return testClass;
        }

        protected virtual Task BeforeTestMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in this.beforeAfterTestGroupAttributes)
            {
                try
                {
                    this.timer.Aggregate(() => beforeAfterAttribute.Before(this.TestMethod));
                    this.beforeAfterTestGroupAttributesRun.Push(beforeAfterAttribute);
                }
                catch (Exception ex)
                {
                    this.Aggregator.Add(ex);
                    break;
                }

                if (CancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
            }

            return Task.FromResult(0);
        }

        protected virtual Task AfterTestMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in this.beforeAfterTestGroupAttributesRun)
            {
                this.Aggregator.Run(() => this.Timer.Aggregate(() => beforeAfterAttribute.After(this.TestMethod)));
            }

            return Task.FromResult(0);
        }

        protected async virtual Task<RunSummary> InvokeTestMethodAsync(object testClassInstance)
        {
            var stepDiscoveryTimer = new ExecutionTimer();
            try
            {
                CurrentScenario.AddingBackgroundSteps = true;
                try
                {
                    foreach (var backgroundMethod in this.testGroup.TestCase.TestMethod.Method.Type
                        .GetMethods(false)
                        .Where(candidate => candidate.GetCustomAttributes(typeof(BackgroundAttribute)).Any())
                        .Select(method => method.ToRuntimeMethod()))
                    {
                        await stepDiscoveryTimer.AggregateAsync(() =>
                            backgroundMethod.InvokeAsync(testClassInstance, null));
                    }
                }
                finally
                {
                    CurrentScenario.AddingBackgroundSteps = false;
                }

                await stepDiscoveryTimer.AggregateAsync(() =>
                    this.TestMethod.InvokeAsync(testClassInstance, this.TestMethodArguments));
            }
            catch (Exception ex)
            {
                CurrentScenario.ExtractSteps();
                this.MessageBus.Queue(
                    new XunitTest(this.testGroup.TestCase, this.testGroup.DisplayName),
                    test => new TestFailed(test, stepDiscoveryTimer.Total, null, ex.Unwrap()),
                    this.CancellationTokenSource);

                return new RunSummary { Failed = 1, Total = 1, Time = stepDiscoveryTimer.Total };
            }

            var steps = CurrentScenario.ExtractSteps().ToArray();
            if (!steps.Any())
            {
                this.MessageBus.Queue(
                    new XunitTest(this.testGroup.TestCase, this.testGroup.DisplayName),
                    test => new TestPassed(test, stepDiscoveryTimer.Total, null),
                    this.CancellationTokenSource);

                return new RunSummary { Total = 1, Time = stepDiscoveryTimer.Total };
            }

            var stepFailed = false;
            var interceptingBus = new DelegatingMessageBus(
                this.MessageBus,
                message =>
                {
                    if (message is ITestFailed)
                    {
                        stepFailed = true;
                    }
                });

            var stepTestRunners = new List<StepTestRunner>();
            string failedStepName = null;
            var summary = new RunSummary { Time = stepDiscoveryTimer.Total };
            foreach (var item in steps.Select((step, index) => new { step, index }))
            {
                var stepTest = new StepTest(
                    this.TestGroup.TestCase,
                    this.TestGroup.DisplayName,
                    this.scenarioNumber,
                    item.index + 1,
                    item.step.Text,
                    this.TestMethodArguments);

                var stepTestRunner = new StepTestRunner(
                    item.step,
                    stepTest,
                    interceptingBus,
                    this.TestClass,
                    this.ConstructorArguments,
                    this.TestMethod,
                    this.TestMethodArguments,
                    item.step.SkipReason,
                    this.beforeAfterTestGroupAttributes,
                    new ExceptionAggregator(this.Aggregator),
                    this.CancellationTokenSource);

                stepTestRunners.Add(stepTestRunner);

                if (failedStepName != null)
                {
                    summary.Failed++;
                    summary.Total++;
                    var message = string.Format(
                        CultureInfo.InvariantCulture, "Failed to execute preceding step \"{0}\".", failedStepName);

                    this.MessageBus.Queue(
                        stepTest,
                        test => new TestFailed(test, 0, string.Empty, new InvalidOperationException(message)),
                        this.CancellationTokenSource);

                    continue;
                }

                summary.Aggregate(await stepTestRunner.RunAsync());

                if (stepFailed)
                {
                    failedStepName = stepTest.StepName;
                }
            }

            var teardowns = stepTestRunners.SelectMany(stepTestRunner => stepTestRunner.Teardowns).ToArray();
            if (teardowns.Any())
            {
                var teardownTimer = new ExecutionTimer();
                var teardownAggregator = new ExceptionAggregator();
                foreach (var teardown in teardowns.Reverse())
                {
                    teardownTimer.Aggregate(() => teardownAggregator.Run(() => teardown()));
                }

                summary.Time += teardownTimer.Total;

                if (teardownAggregator.HasExceptions)
                {
                    summary.Failed++;
                    summary.Total++;

                    var stepTest = new StepTest(
                        this.testGroup.TestCase,
                        this.TestGroup.DisplayName,
                        this.scenarioNumber,
                        steps.Length + 1,
                        "(Teardown)");

                    this.MessageBus.Queue(
                        stepTest,
                        test => new TestFailed(test, teardownTimer.Total, null, teardownAggregator.ToException()),
                        this.CancellationTokenSource);
                }
            }

            return summary;
        }
    }
}
