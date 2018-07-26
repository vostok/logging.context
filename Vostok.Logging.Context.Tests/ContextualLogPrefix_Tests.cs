using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Vostok.Context;

namespace Vostok.Logging.Context.Tests
{
    [TestFixture]
    public class ContextualLogPrefix_Tests
    {
        [Test]
        public void Should_create_context_and_remove_on_dispose_because_previous_not_existed()
        {
            const string prefixName = "some_prefix";
            var prefix = new ContextualLogPrefix(prefixName, true);
            FlowingContext.Get<IReadOnlyList<string>>(ContextualLogPrefix.PrefixKey).Should().BeEquivalentTo(prefixName);
            prefix.Dispose();
            FlowingContext.Get<IReadOnlyList<string>>(ContextualLogPrefix.PrefixKey).Should().BeNull();
        }

        [Test]
        public void Should_create_context_and_remove_on_dispose_because_not_need_to_restore()
        {
            const string prefixName1 = "some_prefix_1";
            const string prefixName2 = "some_prefix_2";
            var prefix1 = new ContextualLogPrefix(prefixName1, true);
            var prefix2 = new ContextualLogPrefix(prefixName2, false);
            FlowingContext.Get<IReadOnlyList<string>>(ContextualLogPrefix.PrefixKey).Should().BeEquivalentTo(prefixName1, prefixName2);
            prefix2.Dispose();
            FlowingContext.Get<IReadOnlyList<string>>(ContextualLogPrefix.PrefixKey).Should().BeNull();
            prefix1.Dispose();
            FlowingContext.Get<IReadOnlyList<string>>(ContextualLogPrefix.PrefixKey).Should().BeNull();
        }

        [Test]
        public void Should_create_context_and_restore_on_dispose()
        {
            const string prefixName1 = "some_prefix_1";
            const string prefixName2 = "some_prefix_2";
            var prefix1 = new ContextualLogPrefix(prefixName1, true);
            var prefix2 = new ContextualLogPrefix(prefixName2, true);
            FlowingContext.Get<IReadOnlyList<string>>(ContextualLogPrefix.PrefixKey).Should().BeEquivalentTo(prefixName1, prefixName2);
            prefix2.Dispose();
            FlowingContext.Get<IReadOnlyList<string>>(ContextualLogPrefix.PrefixKey).Should().BeEquivalentTo(prefixName1);
            prefix1.Dispose();
            FlowingContext.Get<IReadOnlyList<string>>(ContextualLogPrefix.PrefixKey).Should().BeNull();
        }
    }
}