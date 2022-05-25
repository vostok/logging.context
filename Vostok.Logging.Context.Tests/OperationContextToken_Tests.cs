using System.Collections.Generic;
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
        public void SetUp()
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

        [Test]
        public void Should_work_with_object_properties()
        {
            using (new OperationContextToken("Hello {Name1} {Name2}",
                       new
                       {
                           Name1 = "Vostok",
                           Name2 = 42,
                           Name3 = "Up"
                       }))
            {
                var value = FlowingContext.Globals.Get<OperationContextValue>();

                value.Should().NotBeNull();
                
                value.Should().Equal("Hello {Name1} {Name2}");
                
                value!.Properties.Should()
                    .BeEquivalentTo(new Dictionary<string, object>
                    {
                        ["Name1"] = "Vostok",
                        ["Name2"] = 42,
                        ["Name3"] = "Up"
                    });
            }
        }
        
        [Test]
        public void Should_work_with_parameters_properties()
        {
            using (new OperationContextToken("Hello {Name1} {Name2}", "Vostok", 42))
            {
                var value = FlowingContext.Globals.Get<OperationContextValue>();

                value.Should().NotBeNull();
                
                value.Should().Equal("Hello {Name1} {Name2}");
                
                value!.Properties.Should()
                    .BeEquivalentTo(new Dictionary<string, object>
                    {
                        ["Name1"] = "Vostok",
                        ["Name2"] = 42
                    });
            }
        }

#if NET6_0_OR_GREATER
        [Test]
        public void Should_work_with_interpolated_properties()
        {
            var Name1 = "Vostok";
            var Name2 = 42;
            using (new OperationContextToken($"Hello {Name1} {Name2}"))
            {
                var value = FlowingContext.Globals.Get<OperationContextValue>();

                value.Should().NotBeNull();
                
                value.Should().Equal("Hello {Name1} {Name2}");
                
                value!.Properties.Should()
                    .BeEquivalentTo(new Dictionary<string, object>
                    {
                        ["Name1"] = "Vostok",
                        ["Name2"] = 42
                    });
            }
        }
#endif
    }
}