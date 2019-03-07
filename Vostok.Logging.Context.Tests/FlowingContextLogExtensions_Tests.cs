using System;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Vostok.Context;
using Vostok.Logging.Abstractions;

// ReSharper disable PossibleNullReferenceException

namespace Vostok.Logging.Context.Tests
{
    [TestFixture]
    internal class FlowingContextLogExtensions_Tests
    {
        private ILog baseLog;
        private ILog enrichedLog;
        private LogEvent originalEvent;
        private LogEvent observedEvent;

        private string globalName;
        private string propertyName1;
        private string propertyName2;
        private string propertyName3;

        [SetUp]
        public void TestSetup()
        {
            FlowingContext.Properties.Clear();

            baseLog = Substitute.For<ILog>();
            baseLog.When(log => log.Log(Arg.Any<LogEvent>())).Do(info => observedEvent = info.Arg<LogEvent>());

            originalEvent = new LogEvent(LogLevel.Info, DateTimeOffset.Now, null);
            observedEvent = null;

            globalName = Guid.NewGuid().ToString();

            propertyName1 = Guid.NewGuid().ToString();
            propertyName2 = Guid.NewGuid().ToString();
            propertyName3 = Guid.NewGuid().ToString();
        }

        [Test]
        public void WithFlowingContextGlobal_should_return_a_log_that_adds_global_value_of_given_type_to_events()
        {
            FlowingContext.Globals.Set("123");

            enrichedLog = baseLog.WithFlowingContextGlobal<string>(globalName);

            enrichedLog.Log(originalEvent);

            observedEvent.Properties?[globalName].Should().Be("123");
        }

        [Test]
        public void WithFlowingContextGlobal_should_not_overwrite_existing_values_by_default()
        {
            FlowingContext.Globals.Set("123");

            originalEvent = originalEvent.WithProperty(globalName, "456");

            enrichedLog = baseLog.WithFlowingContextGlobal<string>(globalName);

            enrichedLog.Log(originalEvent);

            observedEvent.Properties?[globalName].Should().Be("456");
        }

        [Test]
        public void WithFlowingContextGlobal_should_not_add_null_values_by_default()
        {
            FlowingContext.Globals.Set(null as string);

            enrichedLog = baseLog.WithFlowingContextGlobal<string>(globalName);

            enrichedLog.Log(originalEvent);

            observedEvent.Should().BeSameAs(originalEvent);
        }

        [Test]
        public void WithFlowingContextGlobal_should_add_null_values_if_asked_to()
        {
            FlowingContext.Globals.Set(null as string);

            enrichedLog = baseLog.WithFlowingContextGlobal<string>(globalName, allowNullValues: true);

            enrichedLog.Log(originalEvent);

            observedEvent.Properties.TryGetValue(globalName, out var value).Should().BeTrue();

            value.Should().BeNull();
        }

        [Test]
        public void WithFlowingContextGlobal_should_overwrite_existing_values_if_asked_to()
        {
            FlowingContext.Globals.Set("123");

            originalEvent = originalEvent.WithProperty(globalName, "456");

            enrichedLog = baseLog.WithFlowingContextGlobal<string>(globalName, true);

            enrichedLog.Log(originalEvent);

            observedEvent.Properties?[globalName].Should().Be("123");
        }

        [Test]
        public void WithFlowingContextProperty_should_return_a_log_that_does_not_modify_events_when_given_context_property_does_not_exist()
        {
            enrichedLog = baseLog.WithFlowingContextProperty(Guid.NewGuid().ToString());

            enrichedLog.Log(originalEvent);

            observedEvent.Should().BeSameAs(originalEvent);
        }

        [Test]
        public void WithFlowingContextProperty_should_return_a_log_that_adds_given_property_from_context_to_events()
        {
            FlowingContext.Properties.Set(propertyName1, "value");

            enrichedLog = baseLog.WithFlowingContextProperty(propertyName1);

            enrichedLog.Log(originalEvent);

            observedEvent.Properties?[propertyName1].Should().Be("value");
        }

        [Test]
        public void WithFlowingContextProperty_should_respect_log_property_name_parameter()
        {
            FlowingContext.Properties.Set(propertyName1, "value");

            enrichedLog = baseLog.WithFlowingContextProperty(propertyName1, propertyName2);

            enrichedLog.Log(originalEvent);

            observedEvent.Properties?[propertyName2].Should().Be("value");
        }

        [Test]
        public void WithFlowingContextProperty_should_not_overwrite_existing_properties_by_default()
        {
            FlowingContext.Properties.Set(propertyName1, "value");

            originalEvent = originalEvent.WithProperty(propertyName1, "valueX");

            enrichedLog = baseLog.WithFlowingContextProperty(propertyName1);

            enrichedLog.Log(originalEvent);

            observedEvent.Should().BeSameAs(originalEvent);
        }

        [Test]
        public void WithFlowingContextProperty_should_overwrite_existing_properties_when_asked_to()
        {
            FlowingContext.Properties.Set(propertyName1, "value");

            originalEvent = originalEvent.WithProperty(propertyName1, "valueX");

            enrichedLog = baseLog.WithFlowingContextProperty(propertyName1, allowOverwrite: true);

            enrichedLog.Log(originalEvent);

            observedEvent.Properties?[propertyName1].Should().Be("value");
        }

        [Test]
        public void WithFlowingContextProperty_should_not_allow_null_values_by_default()
        {
            FlowingContext.Properties.Set(propertyName1, null);

            enrichedLog = baseLog.WithFlowingContextProperty(propertyName1);

            enrichedLog.Log(originalEvent);

            observedEvent.Should().BeSameAs(originalEvent);
        }

        [Test]
        public void WithFlowingContextProperty_should_allow_null_values_when_asked_to()
        {
            FlowingContext.Properties.Set(propertyName1, null);

            enrichedLog = baseLog.WithFlowingContextProperty(propertyName1, allowNullValues: true);

            enrichedLog.Log(originalEvent);

            observedEvent.Properties.TryGetValue(propertyName1, out var value).Should().BeTrue();

            value.Should().BeNull();
        }

        [Test]
        public void WithFlowingContextProperties_should_return_a_log_that_adds_given_context_properties_to_events()
        {
            FlowingContext.Properties.Set(propertyName1, "value1");
            FlowingContext.Properties.Set(propertyName2, "value2");

            enrichedLog = baseLog.WithFlowingContextProperties(new[] {propertyName1, propertyName2, propertyName3});

            enrichedLog.Log(originalEvent);

            observedEvent.Properties.Should().HaveCount(2);
            observedEvent.Properties[propertyName1].Should().Be("value1");
            observedEvent.Properties[propertyName2].Should().Be("value2");
        }

        [Test]
        public void WithFlowingContextProperties_should_not_overwrite_existing_properties_by_default()
        {
            FlowingContext.Properties.Set(propertyName1, "value1");
            FlowingContext.Properties.Set(propertyName2, "value2");

            originalEvent = originalEvent.WithProperty(propertyName2, "valueX");

            enrichedLog = baseLog.WithFlowingContextProperties(new[] { propertyName1, propertyName2 });

            enrichedLog.Log(originalEvent);

            observedEvent.Properties.Should().HaveCount(2);
            observedEvent.Properties[propertyName1].Should().Be("value1");
            observedEvent.Properties[propertyName2].Should().Be("valueX");
        }

        [Test]
        public void WithFlowingContextProperties_should_overwrite_existing_properties_when_asked_to()
        {
            FlowingContext.Properties.Set(propertyName1, "value1");
            FlowingContext.Properties.Set(propertyName2, "value2");

            originalEvent = originalEvent.WithProperty(propertyName2, "valueX");

            enrichedLog = baseLog.WithFlowingContextProperties(new[] { propertyName1, propertyName2 }, true);

            enrichedLog.Log(originalEvent);

            observedEvent.Properties.Should().HaveCount(2);
            observedEvent.Properties[propertyName1].Should().Be("value1");
            observedEvent.Properties[propertyName2].Should().Be("value2");
        }

        [Test]
        public void WithFlowingContextProperties_should_not_allow_null_values_by_default()
        {
            FlowingContext.Properties.Set(propertyName1, null);
            FlowingContext.Properties.Set(propertyName2, null);

            enrichedLog = baseLog.WithFlowingContextProperties(new[] { propertyName1, propertyName2 });

            enrichedLog.Log(originalEvent);

            observedEvent.Should().BeSameAs(originalEvent);
        }

        [Test]
        public void WithFlowingContextProperties_should_allow_null_values_when_asked_to()
        {
            FlowingContext.Properties.Set(propertyName1, null);
            FlowingContext.Properties.Set(propertyName2, null);

            enrichedLog = baseLog.WithFlowingContextProperties(new[] { propertyName1, propertyName2 }, allowNullValues: true);

            enrichedLog.Log(originalEvent);

            observedEvent.Properties.Should().HaveCount(2);
            observedEvent.Properties[propertyName1].Should().BeNull();
            observedEvent.Properties[propertyName2].Should().BeNull();
        }

        [Test]
        public void WithFlowingContextProperties_should_not_add_non_existing_properties_with_null_values()
        {
            FlowingContext.Properties.Set(propertyName1, null);

            enrichedLog = baseLog.WithFlowingContextProperties(new[] { propertyName1, propertyName2 }, allowNullValues: true);

            enrichedLog.Log(originalEvent);

            observedEvent.Properties.Should().HaveCount(1);
            observedEvent.Properties[propertyName1].Should().BeNull();
        }

        [Test]
        public void WithAllFlowingContextProperties_should_return_a_log_that_adds_all_context_properties_to_events()
        {
            FlowingContext.Properties.Set(propertyName1, "value1");
            FlowingContext.Properties.Set(propertyName2, "value2");
            FlowingContext.Properties.Set(propertyName3, "value3");

            enrichedLog = baseLog.WithAllFlowingContextProperties();

            enrichedLog.Log(originalEvent);

            observedEvent.Properties[propertyName1].Should().Be("value1");
            observedEvent.Properties[propertyName2].Should().Be("value2");
            observedEvent.Properties[propertyName3].Should().Be("value3");
        }

        [Test]
        public void WithAllFlowingContextProperties_should_not_overwrite_existing_properties_by_default()
        {
            FlowingContext.Properties.Set(propertyName1, "value1");
            FlowingContext.Properties.Set(propertyName2, "value2");

            originalEvent = originalEvent.WithProperty(propertyName2, "valueX");

            enrichedLog = baseLog.WithAllFlowingContextProperties();

            enrichedLog.Log(originalEvent);

            observedEvent.Properties[propertyName1].Should().Be("value1");
            observedEvent.Properties[propertyName2].Should().Be("valueX");
        }

        [Test]
        public void WithAllFlowingContextProperties_should_overwrite_existing_properties_when_asked_to()
        {
            FlowingContext.Properties.Set(propertyName1, "value1");
            FlowingContext.Properties.Set(propertyName2, "value2");

            originalEvent = originalEvent.WithProperty(propertyName2, "valueX");

            enrichedLog = baseLog.WithAllFlowingContextProperties(true);

            enrichedLog.Log(originalEvent);

            observedEvent.Properties[propertyName1].Should().Be("value1");
            observedEvent.Properties[propertyName2].Should().Be("value2");
        }

        [Test]
        public void WithAllFlowingContextProperties_should_not_allow_null_values_by_default()
        {
            FlowingContext.Properties.Set(propertyName1, null);
            FlowingContext.Properties.Set(propertyName2, null);

            enrichedLog = baseLog.WithAllFlowingContextProperties();

            enrichedLog.Log(originalEvent);

            observedEvent.Should().BeSameAs(originalEvent);
        }

        [Test]
        public void WithAllFlowingContextProperties_should_allow_null_values_when_asked_to()
        {
            FlowingContext.Properties.Set(propertyName1, null);
            FlowingContext.Properties.Set(propertyName2, null);

            enrichedLog = baseLog.WithAllFlowingContextProperties(allowNullValues: true);

            enrichedLog.Log(originalEvent);

            observedEvent.Properties.TryGetValue(propertyName1, out var value1).Should().BeTrue();
            observedEvent.Properties.TryGetValue(propertyName2, out var value2).Should().BeTrue();

            value1.Should().BeNull();
            value2.Should().BeNull();
        }
    }
}