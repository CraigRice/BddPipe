using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.F;

namespace BddPipe.UnitTests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        private const string DefaultValue = "test";
        private const string DefaultPrefix = "Given";
        private const string DefaultExpectation = "Given test";

        [Test]
        public void WithPrefix_DoesNotHavePrefix_PrefixAdded()
        {
            var result = DefaultValue.AsOption().WithPrefix(DefaultPrefix);
            result.Value.Should().Be(DefaultExpectation);
        }

        [Test]
        public void WithPrefix_AlreadyHasPrefix_PrefixNotAdded()
        {
            var result = DefaultExpectation.AsOption().WithPrefix(DefaultPrefix);
            result.Value.Should().Be(DefaultExpectation);
        }

        [Test]
        public void WithPrefix_AlreadyHasPrefixCaseLower_PrefixNotAdded()
        {
            var result = "given something".AsOption().WithPrefix(DefaultPrefix);
            result.Value.Should().Be("given something");
        }

        [Test]
        public void WithPrefix_AlreadyHasPrefixCaseUpper_PrefixNotAdded()
        {
            var result = "GIVEN something".AsOption().WithPrefix(DefaultPrefix);
            result.Value.Should().Be("GIVEN something");
        }

        [Test]
        public void WithPrefix_AlreadyHasPrefixCaseMixed_PrefixNotAdded()
        {
            var result = "GiVeN something".AsOption().WithPrefix(DefaultPrefix);
            result.Value.Should().Be("GiVeN something");
        }

        [Test]
        public void WithPrefix_Null_PrefixReturned()
        {
            var result = ((string)null).AsOption().WithPrefix(DefaultPrefix);
            result.Value.Should().Be(DefaultPrefix);
        }

        [Test]
        public void WithPrefix_Empty_PrefixReturned()
        {
            var result = string.Empty.AsOption().WithPrefix(DefaultPrefix);
            result.Value.Should().Be(DefaultPrefix);
        }

        [Test]
        public void WithPrefix_Whitespace_PrefixReturned()
        {
            var result = " ".AsOption().WithPrefix(DefaultPrefix);
            result.Value.Should().Be(DefaultPrefix);
        }

        [Test]
        public void NoneIfWhiteSpace_OptionIsNone_None()
        {
            Option<string> source = None;

            source.NoneIfWhiteSpace().ShouldBeNone();
        }

        [Test]
        public void NoneIfWhiteSpace_OptionIsSomeText_SomeText()
        {
            Option<string> source = "value";

            source.NoneIfWhiteSpace().ShouldBeSome(txt =>
            {
                txt.Should().Be("value");
            });
        }

        [Test]
        public void NoneIfWhiteSpace_OptionIsSomeTextTrailingWhiteSpace_SomeTextDoesNotTrim()
        {
            Option<string> source = "value ";

            source.NoneIfWhiteSpace().ShouldBeSome(txt =>
            {
                txt.Should().Be("value ");
            });
        }

        [Test]
        public void NoneIfWhiteSpace_OptionIsEmpty_None()
        {
            Option<string> source = string.Empty;

            source.NoneIfWhiteSpace().ShouldBeNone();
        }

        [TestCase(" ")]
        [TestCase("  ")]
        [TestCase("\t")]
        [TestCase("\t ")]
        [TestCase(" \t")]
        [TestCase("\r\n")]
        [TestCase(" \t\r\n")]
        public void NoneIfWhiteSpace_OptionIsWhiteSpace_None(string value)
        {
            Option<string> source = value;

            source.NoneIfWhiteSpace().ShouldBeNone();
        }
    }
}
