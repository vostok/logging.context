using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Context;
using Vostok.Logging.Abstractions;
using Vostok.Logging.Abstractions.Values;

namespace Vostok.Logging.Context.Tests
{
    [TestFixture]
    internal class OperationContextLogExtensions_Tests
    {
        private ILog baseLog;
        private ILog enrichedLog;
        private LogEvent originalEvent;
        private LogEvent observedEvent;

        [SetUp]
        public void TestSetup()
        {
            FlowingContext.Globals.Set(null as OperationContextValue);

            baseLog = Substitute.For<ILog>();
            baseLog.When(log => log.Log(Arg.Any<LogEvent>())).Do(info => observedEvent = info.Arg<LogEvent>());

            originalEvent = new LogEvent(LogLevel.Info, DateTimeOffset.Now, null);
            observedEvent = null;
        }

        [Test]
        public void WithOperationContext_should_return_a_log_that_does_not_modify_events_when_there_is_no_operation_context()
        {
            enrichedLog = baseLog.WithOperationContext();

            enrichedLog.Log(originalEvent);

            observedEvent.Should().BeSameAs(originalEvent);
        }

        [Test]
        public void WithOperationContext_should_return_a_log_that_adds_current_operation_context_to_events()
        {
            enrichedLog = baseLog.WithOperationContext();

            using (new OperationContextToken("op1"))
            using (new OperationContextToken("op2"))
            {
                enrichedLog.Log(originalEvent);
            }

            observedEvent.Properties![WellKnownProperties.OperationContext]
                .Should().BeOfType<OperationContextValue>().Which.Should().Equal("op1", "op2");
        }
        
        [Test]
        public void WithOperationContext_should_return_a_log_that_adds_current_operation_context_properties_to_events()
        {
            enrichedLog = baseLog.WithOperationContext();

            using (new OperationContextToken("aa {Name1} bb", "Vostok"))
            using (new OperationContextToken("aa {Name2} bb", 42))
            {
                enrichedLog.Log(originalEvent);
            }

            observedEvent.Properties![WellKnownProperties.OperationContext]
                .Should().BeOfType<OperationContextValue>().Which.Should().Equal("aa {Name1} bb", "aa {Name2} bb");

            observedEvent.Properties!["Name1"].Should().Be("Vostok");
            observedEvent.Properties!["Name2"].Should().Be(42);
        }
    }
}