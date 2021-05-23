using System.Linq;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.F;

namespace BddPipe.UnitTests.F
{
    [TestFixture]
    public class OptionTests
    {
        private const string SomeValue = "a value";

        [Test]
        public void IsSome_OptionOfSome_True()
        {
            Option<string> option = SomeValue;
            option.IsSome.Should().BeTrue();
        }

        [Test]
        public void IsSome_OptionOfSomeT_True()
        {
            Option<string> option = new Some<string>(SomeValue);
            option.IsSome.Should().BeTrue();
        }

        [Test]
        public void IsSome_OptionOfNone_False()
        {
            Option<string> option = None;
            option.IsSome.Should().BeFalse();
        }

        [Test]
        public void AsEnumerable_OptionOfSome_ReturnsSingle()
        {
            Option<string> option = SomeValue;
            var asEnumerable = option.AsEnumerable();
            asEnumerable.Should().NotBeNull();

            var result = asEnumerable.ToList();
            result.Count.Should().Be(1);
            result[0].Should().Be(SomeValue);
        }

        [Test]
        public void AsEnumerable_OptionOfSomeT_ReturnsSingle()
        {
            Option<string> option = new Some<string>(SomeValue);
            var asEnumerable = option.AsEnumerable();
            asEnumerable.Should().NotBeNull();

            var result = asEnumerable.ToList();
            result.Count.Should().Be(1);
            result[0].Should().Be(SomeValue);
        }

        [Test]
        public void AsEnumerable_OptionOfNone_ReturnsEmpty()
        {
            Option<string> option = None;
            var asEnumerable = option.AsEnumerable();
            asEnumerable.Should().NotBeNull();

            var result = asEnumerable.ToList();
            result.Count.Should().Be(0);
        }

        [Test]
        public void EqualsOptionT_OptionOfNoneWithNone_True()
        {
            Option<string> option = None;
            Option<string> option2 = None;
            var result = option.Equals(option2);
            result.Should().BeTrue();
        }

        [Test]
        public void EqualsOptionT_OptionOfNoneWithSome_False()
        {
            Option<string> option = None;
            Option<string> option2 = SomeValue;
            var result = option.Equals(option2);
            result.Should().BeFalse();
        }

        [Test]
        public void EqualsOptionT_OptionOfSomeWithNone_False()
        {
            Option<string> option = SomeValue;
            Option<string> option2 = None;
            var result = option.Equals(option2);
            result.Should().BeFalse();
        }

        [Test]
        public void EqualsOptionT_OptionOfSomeWithSome_True()
        {
            Option<string> option = SomeValue;
            Option<string> option2 = SomeValue;
            var result = option.Equals(option2);
            result.Should().BeTrue();
        }

        [Test]
        public void EqualsOptionT_OptionOfSomeWithDifferentSome_False()
        {
            Option<string> option = SomeValue;
            Option<string> option2 = SomeValue + "different";
            var result = option.Equals(option2);
            result.Should().BeFalse();
        }

        [Test]
        public void EqualsOptionTOperator_OptionOfNoneWithNone_True()
        {
            Option<string> option = None;
            Option<string> option2 = None;
            var result = option == option2;
            result.Should().BeTrue();
        }

        [Test]
        public void EqualsOptionTOperator_OptionOfNoneWithSome_False()
        {
            Option<string> option = None;
            Option<string> option2 = SomeValue;
            var result = option == option2;
            result.Should().BeFalse();
        }

        [Test]
        public void EqualsOptionTOperator_OptionOfSomeWithNone_False()
        {
            Option<string> option = SomeValue;
            Option<string> option2 = None;
            var result = option == option2;
            result.Should().BeFalse();
        }

        [Test]
        public void EqualsOptionTOperator_OptionOfSomeWithSome_True()
        {
            Option<string> option = SomeValue;
            Option<string> option2 = SomeValue;
            var result = option == option2;
            result.Should().BeTrue();
        }

        [Test]
        public void EqualsOptionTOperator_OptionOfSomeWithDifferentSome_False()
        {
            Option<string> option = SomeValue;
            Option<string> option2 = SomeValue + "different";
            var result = option == option2;
            result.Should().BeFalse();
        }

        [Test]
        public void NotEqualsOptionTOperator_OptionOfNoneWithNone_True()
        {
            Option<string> option = None;
            Option<string> option2 = None;
            var result = option != option2;
            result.Should().BeFalse();
        }

        [Test]
        public void NotEqualsOptionTOperator_OptionOfNoneWithSome_False()
        {
            Option<string> option = None;
            Option<string> option2 = SomeValue;
            var result = option != option2;
            result.Should().BeTrue();
        }

        [Test]
        public void NotEqualsOptionTOperator_OptionOfSomeWithNone_False()
        {
            Option<string> option = SomeValue;
            Option<string> option2 = None;
            var result = option != option2;
            result.Should().BeTrue();
        }

        [Test]
        public void NotEqualsOptionTOperator_OptionOfSomeWithSome_True()
        {
            Option<string> option = SomeValue;
            Option<string> option2 = SomeValue;
            var result = option != option2;
            result.Should().BeFalse();
        }

        [Test]
        public void NotEqualsOptionTOperator_OptionOfSomeWithDifferentSome_False()
        {
            Option<string> option = SomeValue;
            Option<string> option2 = SomeValue + "different";
            var result = option != option2;
            result.Should().BeTrue();
        }

        [Test]
        public void Equals_OptionOfSomeComparedToNonOptionT_False()
        {
            Option<string> option = SomeValue;
            object option2 = 765;
            var result = option.Equals(option2);
            result.Should().BeFalse();
        }

        [Test]
        public void Equals_OptionOfNoneComparedToNonOptionT_False()
        {
            Option<string> option = None;
            object option2 = 765;
            var result = option.Equals(option2);
            result.Should().BeFalse();
        }

        [Test]
        public void Equals_OptionOfSomeComparedToSameTTypeNonOptionT_False()
        {
            Option<string> option = SomeValue;
            object option2 = SomeValue;
            var result = option.Equals(option2);
            result.Should().BeFalse();
        }

        [Test]
        public void Equals_OptionOfNoneComparedToSameTTypeToNonOptionT_False()
        {
            Option<string> option = None;
            object option2 = SomeValue;
            var result = option.Equals(option2);
            result.Should().BeFalse();
        }

        [Test]
        public void Equals_OptionOfSomeComparedToNull_False()
        {
            Option<string> option = SomeValue;
            object option2 = null;
            var result = option.Equals(option2);
            result.Should().BeFalse();
        }

        [Test]
        public void Equals_OptionOfNoneComparedToNull_False()
        {
            Option<string> option = None;
            object option2 = null;
            var result = option.Equals(option2);
            result.Should().BeFalse();
        }

        [Test]
        public void Equals_OptionOfSomeComparedToSome_True()
        {
            Option<string> option = SomeValue;
            Option<string> option2 = SomeValue;
            var result = option.Equals((object)option2);
            result.Should().BeTrue();
        }

        [Test]
        public void Equals_OptionOfSomeComparedToNone_True()
        {
            Option<string> option = SomeValue;
            Option<string> option2 = None;
            var result = option.Equals((object)option2);
            result.Should().BeFalse();
        }

        [Test]
        public void Equals_OptionOfNoneComparedToSome_True()
        {
            Option<string> option = None;
            Option<string> option2 = SomeValue;
            var result = option.Equals((object)option2);
            result.Should().BeFalse();
        }

        [Test]
        public void Equals_OptionOfNoneComparedToNone_True()
        {
            Option<string> option = None;
            Option<string> option2 = None;
            var result = option.Equals((object)option2);
            result.Should().BeTrue();
        }

        [Test]
        public void EqualsOptionNone_OptionOfSome_False()
        {
            Option<string> option = SomeValue;
            var result = option.Equals(None);
            result.Should().BeFalse();
        }

        [Test]
        public void EqualsOptionNone_OptionOfNone_True()
        {
            Option<string> option = None;
            var result = option.Equals(None);
            result.Should().BeTrue();
        }

        [Test]
        public void GetHashCode_OptionOfNone_Zero()
        {
            Option<string> option = None;
            var result = option.GetHashCode();
            result.Should().Be(0);
        }

        [Test]
        public void GetHashCode_OptionOfSome_ReturnsValuesHashCode()
        {
            const string value = SomeValue;
            var expected = value.GetHashCode();

            Option<string> option = SomeValue;
            var result = option.GetHashCode();
            result.Should().Be(expected);
        }

        [Test]
        public void Match_OptionOfSome_ExecutesSomeFnAndReturnsItsResult()
        {
            const string valueSome = "valueSome";
            const string valueNone = "valueNone";

            Option<string> option = SomeValue;
            var result = option.Match(
                some => valueSome,
                () => valueNone
            );

            result.Should().Be(valueSome);
        }

        [Test]
        public void Match_OptionOfNone_ExecutesSomeFnAndReturnsItsResult()
        {
            const string valueSome = "valueSome";
            const string valueNone = "valueNone";

            Option<string> option = None;
            var result = option.Match(
                some => valueSome,
                () => valueNone
            );

            result.Should().Be(valueNone);
        }

        [Test]
        public void ToString_OptionOfSome_ExecutesSomeFnAndReturnsItsResult()
        {
            Option<string> option = SomeValue;
            var result = option.ToString();
            result.Should().Be($"Some({SomeValue})");
        }

        [Test]
        public void ToString_OptionOfNone_ExecutesSomeFnAndReturnsItsResult()
        {
            Option<string> option = None;
            var result = option.ToString();
            result.Should().Be("None");
        }

        [Test]
        public void IfNone_OptionOfSome_ReturnsSomeValue()
        {
            const string valueIfNone = "value if none";
            Option<string> option = SomeValue;
            var result = option.IfNone(valueIfNone);
            result.Should().Be(SomeValue);
        }

        [Test]
        public void IfNone_OptionOfNone_ReturnsSuppliedValue()
        {
            const string valueIfNone = "value if none";
            Option<string> option = None;
            var result = option.IfNone(valueIfNone);
            result.Should().Be(valueIfNone);
        }

        [Test]
        public void Map_OptionOfSomeToChangedInstance_ReturnsNewInstance()
        {
            Option<string> option = SomeValue;
            var result = option.Map(o => o.Length);

            result.IsSome.Should().BeTrue();
            result.ShouldBeSome(val =>
            {
                val.Should().Be(SomeValue.Length);
            });
        }

        [Test]
        public void Map_OptionOfNoneToChangedInstance_ReturnsNone()
        {
            Option<string> option = None;
            var result = option.Map(o => o.Length);

            result.IsSome.Should().BeFalse();
            result.ShouldBeNone();
        }
    }
}
