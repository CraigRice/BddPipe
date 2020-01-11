using System;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using static BddPipe.F;
using NUnit.Framework;

namespace BddPipe.UnitTests
{
    internal static class OptionTestExtensions
    {
        public static Option<string> AsOption(this string value) =>
            value == null ? None : (Option<string>)Some(value);
    }

    [TestFixture]
    public class StepResultExtensionsTests
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
        public void ToOutcome_Exception_Fail()
        {
            StepResultExtensions.ToOutcome(new Exception()).Should().Be(Outcome.Fail);
        }

        [Test]
        public void ToOutcome_FormatException_Fail()
        {
            StepResultExtensions.ToOutcome(new FormatException()).Should().Be(Outcome.Fail);
        }

        [Test]
        public void ToOutcome_InconclusiveException_Inconclusive()
        {
            StepResultExtensions.ToOutcome(new InconclusiveException("test message")).Should().Be(Outcome.Inconclusive);
        }

        [Test]
        public void ToTitle_NullTitleString_ReturnsCorrectTitle()
        {
            string titleText = null;

            var result = titleText.ToTitle(Step.And).Value;

            result.Step.Should().Be(Step.And);
            result.Text.ShouldBeNone();
        }

        [Test]
        public void ToTitle_TitleString_ReturnsCorrectTitle()
        {
            const string titleText = "title text";

            var result = titleText.ToTitle(Step.And).Value;

            result.Step.Should().Be(Step.And);
            result.Text.ShouldBeSome(title => title.Should().Be(titleText));
        }

        [Test]
        public void ToStepOutcome_TitleAndSuppliedOutcome_ReturnsMappedStepOutcome()
        {
            const string titleText = "title text";

            Some<Title> title = new Title(Step.And, titleText);

            var stepOutcome = title.ToStepOutcome(Outcome.Inconclusive);

            stepOutcome.Should().NotBeNull();
            stepOutcome.Step.Should().Be(Step.And);
            stepOutcome.Outcome.Should().Be(Outcome.Inconclusive);
            stepOutcome.Text.ShouldBeSome(titleOptionValue => titleOptionValue.Should().Be(titleText));
        }

        [Test]
        public void ToStepOutcome_NullTitleAndSuppliedOutcome_ReturnsMappedStepOutcome()
        {
            Some<Title> title = new Title(Step.And, None);

            var stepOutcome = title.ToStepOutcome(Outcome.Inconclusive);

            stepOutcome.Should().NotBeNull();
            stepOutcome.Step.Should().Be(Step.And);
            stepOutcome.Outcome.Should().Be(Outcome.Inconclusive);
            stepOutcome.Text.ShouldBeNone();
        }
    }
}
