// <copyright file="Xunit2DiscovererExtensions.cs" company="xBehave.net contributors">
//  Copyright (c) xBehave.net contributors. All rights reserved.
// </copyright>

namespace Xbehave.Test.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using LiteGuard;
    using Xunit;
    using Xunit.Abstractions;

    public static class Xunit2DiscovererExtensions
    {
        public static IEnumerable<ITestCase> Find(this Xunit2Discoverer discoverer, string collectionName)
        {
            Guard.AgainstNullArgument(nameof(discoverer), discoverer);

            using (var sink = new SpyMessageSink<IDiscoveryCompleteMessage>())
            {
                discoverer.Find(false, sink, TestFrameworkOptions.ForDiscovery());
                sink.Finished.WaitOne();
                return sink.Messages.OfType<ITestCaseDiscoveryMessage>()
                    .Select(message => message.TestCase)
                    .Where(message => message.TestMethod.TestClass.TestCollection.DisplayName == collectionName)
                    .ToList();
            }
        }

        public static IEnumerable<ITestCase> Find(this Xunit2Discoverer discoverer, Type type)
        {
            Guard.AgainstNullArgument(nameof(discoverer), discoverer);
            Guard.AgainstNullArgument(nameof(type), type);

            using (var sink = new SpyMessageSink<IDiscoveryCompleteMessage>())
            {
                discoverer.Find(type.FullName, false, sink, TestFrameworkOptions.ForDiscovery());
                sink.Finished.WaitOne();
                return sink.Messages.OfType<ITestCaseDiscoveryMessage>().Select(message => message.TestCase).ToList();
            }
        }
    }
}
