﻿// <copyright file="SpecificationExtensions.cs" company="Adam Ralph">
//  Copyright (c) Adam Ralph. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;

    using Xbehave.Fluent;

    /// <summary>
    /// Provides extensions for a fluent specification syntax
    /// </summary>
    [Obsolete("Use StringExtensions instead.")]
    public static class SpecificationExtensions
    {
        /// <summary>
        /// Records a context setup for this specification.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="arrange">The action that will establish the context.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use Given() instead.")]
        public static IStep Context(this string message, Action arrange)
        {
            return ScenarioContext.Given(
                message,
                () =>
                {
                    arrange();
                    return DisposableAction.None;
                });
        }

        /// <summary>
        /// Trap for using contexts implementing IDisposable with the wrong overload.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="arrange">The action that will establish and return the context for this test.</param>
        [Obsolete("Use Given() instead.")]
        public static void Context(this string message, ContextDelegate arrange)
        {
            throw new InvalidOperationException("Use Given() instead.");
        }

        /// <summary>
        /// Records a disposable context for this specification. The context lifecycle will be managed by Xbehave.
        /// </summary>
        /// <param name="message">A message describing the established context.</param>
        /// <param name="arrange">The action that will establish and return the context for this test.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use Given() instead.")]
        public static IStep ContextFixture(this string message, ContextDelegate arrange)
        {
            return ScenarioContext.Given(message, arrange);
        }

        /// <summary>
        /// Records an action to be performed on the context for this specification.
        /// </summary>
        /// <param name="message">A message describing the action.</param>
        /// <param name="act">The action to perform.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use When() instead.")]
        public static IStep Do(this string message, Action act)
        {
            return ScenarioContext.When(message, act);
        }

        /// <summary>
        /// Records an assertion for this specification.
        /// Each assertion is executed on an isolated context.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="assert">The action that will verify the expectation.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use ThenInIsolation() instead.")]
        public static IStep Assert(this string message, Action assert)
        {
            return ScenarioContext.ThenInIsolation(message, assert);
        }

        /// <summary>
        /// Records an observation for this specification.
        /// All observations are executed on the same context.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="observation">The action that will verify the expectation.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use Then() instead.")]
        public static IStep Observation(this string message, Action observation)
        {
            return ScenarioContext.Then(message, observation);
        }

        /// <summary>
        /// Records a skipped assertion for this specification.
        /// </summary>
        /// <param name="message">A message describing the expected result.</param>
        /// <param name="skippedAction">The action that will verify the expectation.</param>
        /// <returns>An instance of <see cref="IStep"/>.</returns>
        [Obsolete("Use ThenSkip() instead.")]
        public static IStep Todo(this string message, Action skippedAction)
        {
            return ScenarioContext.ThenSkip(message, skippedAction);
        }
    }
}