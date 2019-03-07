using FluentAssertions;
using NUnit.Framework;
using Vostok.Context;
using Vostok.Logging.Abstractions.Values;

namespace Vostok.Logging.Context.Tests
{
    [TestFixture]
    internal class OperationContextToken_Tests
    {
        [SetUp]
        public void TestSetup()
        {
            FlowingContext.Globals.Set(null as OperationContextValue);
        }

        [Test]
        public void Should_accumulate_contexts_in_a_stack_based_manner_when_using_ctor_and_dispose()
        {
            FlowingContext.Globals.Get<OperationContextValue>().Should().BeNull();

            using (new OperationContextToken("op1"))
            {
                FlowingContext.Globals.Get<OperationContextValue>().Should().Equal("op1");

                using (new OperationContextToken("op2"))
                {
                    FlowingContext.Globals.Get<OperationContextValue>().Should().Equal("op1", "op2");

                    using (new OperationContextToken("op3"))
                    {
                        FlowingContext.Globals.Get<OperationContextValue>().Should().Equal("op1", "op2", "op3");
                    }

                    FlowingContext.Globals.Get<OperationContextValue>().Should().Equal("op1", "op2");
                }

                FlowingContext.Globals.Get<OperationContextValue>().Should().Equal("op1");
            }

            FlowingContext.Globals.Get<OperationContextValue>().Should().BeNull();
        }
    }
}