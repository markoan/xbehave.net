// <copyright file="StringExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Xbehave.Sdk;

    /// <summary>
    /// Provides access to step definition methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        public static IStepBuilder x(this string text, Action body)
        {
            var stepDefinition = new StepDefinition
            {
                Text = text,
                Body = body == null
                    ? default(Func<IStepContext, Task>)
                    : c =>
                        {
                            body();
                            return Task.FromResult(0);
                        },
            };

            CurrentThread.StepDefinitions.Add(stepDefinition);
            return stepDefinition;
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        [Obsolete("Deprecated since version 2.2.0 and will soon be removed. Use x() instead.", false)]
        public static IStepBuilder f(this string text, Action body) => text.x(body);

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        [Obsolete("Deprecated since version 2.2.0 and will soon be removed. Use x() instead.", false)]
        public static IStepBuilder _(this string text, Action body) => text.x(body);

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        public static IStepBuilder x(this string text, Action<IStepContext> body)
        {
            var stepDefinition = new StepDefinition
            {
                Text = text,
                Body = body == null
                    ? default(Func<IStepContext, Task>)
                    : c =>
                        {
                            body(c);
                            return Task.FromResult(0);
                        },
            };

            CurrentThread.StepDefinitions.Add(stepDefinition);
            return stepDefinition;
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        [Obsolete("Deprecated since version 2.2.0 and will soon be removed. Use x() instead.", false)]
        public static IStepBuilder f(this string text, Action<IStepContext> body) => text.x(body);

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        [Obsolete("Deprecated since version 2.2.0 and will soon be removed. Use x() instead.", false)]
        public static IStepBuilder _(this string text, Action<IStepContext> body) => text.x(body);

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        public static IStepBuilder x(this string text, Func<Task> body)
        {
            var stepDefinition = new StepDefinition
            {
                Text = text,
                Body = body == null ? default(Func<IStepContext, Task>) : c => body(),
            };

            CurrentThread.StepDefinitions.Add(stepDefinition);
            return stepDefinition;
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        [Obsolete("Deprecated since version 2.2.0 and will soon be removed. Use x() instead.", false)]
        public static IStepBuilder f(this string text, Func<Task> body) => text.x(body);

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        [Obsolete("Deprecated since version 2.2.0 and will soon be removed. Use x() instead.", false)]
        public static IStepBuilder _(this string text, Func<Task> body) => text.x(body);

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        public static IStepBuilder x(this string text, Func<IStepContext, Task> body)
        {
            var stepDefinition = new StepDefinition { Text = text, Body = body, };
            CurrentThread.StepDefinitions.Add(stepDefinition);
            return stepDefinition;
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        [Obsolete("Deprecated since version 2.2.0 and will soon be removed. Use x() instead.", false)]
        public static IStepBuilder f(this string text, Func<IStepContext, Task> body) => text.x(body);

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Fluent API")]
        [Obsolete("Deprecated since version 2.2.0 and will soon be removed. Use x() instead.", false)]
        public static IStepBuilder _(this string text, Func<IStepContext, Task> body) => text.x(body);
    }
}
