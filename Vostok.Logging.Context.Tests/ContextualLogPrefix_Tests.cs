using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Logging.Context.Tests
{
    [TestFixture]
    internal class ContextualLogPrefix_Tests
    {
        [SetUp]
        public void TestSetup()
        {
            ContextualLogPrefix.Drop();
        }

        [Test]
        public void Current_property_should_return_null_when_there_is_no_prefix()
        {
            ContextualLogPrefix.Current.Should().BeNull();
        }

        [Test]
        public void Should_accumulate_prefix_segments_in_a_stack_based_manner_when_using_ctor_and_dispose()
        {
            using (new ContextualLogPrefix("p1"))
            {
                ContextualLogPrefix.Current.Should().Be("[p1] ");

                using (new ContextualLogPrefix("p2"))
                {
                    ContextualLogPrefix.Current.Should().Be("[p1] [p2] ");

                    using (new ContextualLogPrefix("p3"))
                    {
                        ContextualLogPrefix.Current.Should().Be("[p1] [p2] [p3] ");
                    }

                    ContextualLogPrefix.Current.Should().Be("[p1] [p2] ");
                }

                ContextualLogPrefix.Current.Should().Be("[p1] ");
            }

            ContextualLogPrefix.Current.Should().BeNull();
        }

        [Test]
        public void Drop_should_remove_current_prefix_from_context()
        {
            using (new ContextualLogPrefix("p1"))
            {
                ContextualLogPrefix.Drop();

                ContextualLogPrefix.Current.Should().BeNull();
            }
        }
    }
}