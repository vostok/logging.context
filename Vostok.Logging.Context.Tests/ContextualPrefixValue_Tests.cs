using FluentAssertions;
using NUnit.Framework;

namespace Vostok.Logging.Context.Tests
{
    [TestFixture]
    internal class ContextualPrefixValue_Tests
    {
        private ContextualPrefixValue value;

        [SetUp]
        public void TestSetup()
        {
            value = null;
        }

        [Test]
        public void Should_be_able_to_append_string_to_a_null_value()
        {
            value += "prefix";

            value.Value.Should().Be("[prefix] ");
        }

        [Test]
        public void Should_be_able_to_append_string_to_a_non_null_value()
        {
            value += "prefix1";
            value += "prefix2";
            value += "prefix3";

            value.Value.Should().Be("[prefix1] [prefix2] [prefix3] ");
        }

        [Test]
        public void Should_have_a_tostring_returning_internal_value()
        {
            value += "prefix1";
            value += "prefix2";
            value += "prefix3";

            var internalValue = value.Value;

            value.ToString().Should().BeSameAs(internalValue);
            value.ToString().Should().BeSameAs(internalValue);
        }

        [Test]
        public void Should_be_able_to_append_a_null_string()
        {
            value += null;

            value.Value.Should().Be("[] ");
        }

        [Test]
        public void Should_be_able_to_append_an_empty_string()
        {
            value += string.Empty;

            value.Value.Should().Be("[] ");
        }
    }
}