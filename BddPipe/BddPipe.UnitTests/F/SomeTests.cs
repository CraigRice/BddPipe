using FluentAssertions;
using NUnit.Framework;
using System;

namespace BddPipe.UnitTests.F
{
    [TestFixture]
    public class SomeTests
    {
        [Test]
        public void Value_Struct_Ok()
        {
            var some = new Some<int>(5);
            some.Value.Should().Be(5);
        }

        [Test]
        public void Value_Reference_Ok()
        {
            var instance = new Scenario("test");
            var some = new Some<Scenario>(instance);
            some.Value.Should().Be(instance);
        }

        [Test]
        public void Value_OnDefault_ThrowsNotInitializedException()
        {
            Some<Scenario> some = default;
            Func<Scenario> call = () => some.Value;
            call.Should().ThrowExactly<NotInitializedException>();
        }

        [Test]
        public void ToString_OnDefault_ThrowsNotInitializedException()
        {
            Some<Scenario> some = default;
            Func<string> call = () => some.ToString();
            call.Should().ThrowExactly<NotInitializedException>();
        }

        [Test]
        public void ToString_OfInstance_ReturnsInstanceToString()
        {
            var instance = new Scenario("test");
            Some<Scenario> some = instance;

            var asString = some.ToString();
            asString.Should().Be(instance.ToString());
        }

        [Test]
        public void GetHashCode_OnDefault_ThrowsNotInitializedException()
        {
            Some<Scenario> some = default;
            Func<int> call = () => some.GetHashCode();
            call.Should().ThrowExactly<NotInitializedException>();
        }

        [Test]
        public void GetHashCode_OfInstance_ReturnsInstanceHashCode()
        {
            var instance = new Scenario("test");
            Some<Scenario> some = instance;

            var hashCode = some.GetHashCode();
            hashCode.Should().Be(instance.GetHashCode());
        }

        [Test]
        public void Ctor_ReferenceNull_ThrowsArgNullException()
        {
            var call = () => new Some<Scenario>(null!);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("value");
        }

        [Test]
        public void Implicit_Ctor_CreatesInstance()
        {
            var instance = new Scenario("test");
            Some<Scenario> some = instance;
            some.Value.Should().Be(instance);
        }

        [Test]
        public void Implicit_Assign_AssignsInstance()
        {
            var instance = new Scenario("test");
            Some<Scenario> some = instance;

            Scenario result = some;

            result.Should().NotBeNull();
            result.Should().Be(instance);
        }

        [TestCase(5, 5, true)]
        [TestCase(5, 6, false)]
        public void EqualsOperator_InnerValues_ReturnsExpected(int a, int b, bool expected)
        {
            Some<int> someA = a;
            Some<int> someB = b;
            var result = someA == someB;
            result.Should().Be(expected);
        }

        [TestCase(5, 5, false)]
        [TestCase(5, 6, true)]
        public void NotEqualsOperator_InnerValues_ReturnsExpected(int a, int b, bool expected)
        {
            Some<int> someA = a;
            Some<int> someB = b;
            var result = someA != someB;
            result.Should().Be(expected);
        }

        [TestCase(5, 5, true)]
        [TestCase(5, 6, false)]
        public void Equals_SomeAgainstInnerValue_ReturnsExpected(int a, int b, bool expected)
        {
            Some<int> someA = a;
            // ReSharper disable once SuspiciousTypeConversion.Global
            var result = someA.Equals(b);
            result.Should().Be(expected);
        }

        [TestCase(5, 5, true)]
        [TestCase(5, 6, false)]
        public void Equals_SomeAgainstSome_ReturnsExpected(int a, int b, bool expected)
        {
            Some<int> someA = a;
            Some<int> someB = b;
            var result = someA.Equals(someB);
            result.Should().Be(expected);
        }

        [Test]
        public void Equals_SourceSomeDefault_ThrowsNotInitializedException()
        {
            Some<int> someA = default;
            Some<int> someB = 5;

            Func<bool> call = () => someA.Equals(someB);
            call.Should().ThrowExactly<NotInitializedException>();
        }

        [Test]
        public void Equals_ComparisonSomeDefault_ThrowsNotInitializedException()
        {
            Some<int> someA = 5;
            Some<int> someB = default;

            Func<bool> call = () => someA.Equals(someB);
            call.Should().ThrowExactly<NotInitializedException>();
        }

        [Test]
        public void Map_ConvertSome_ReturnsNewValue()
        {
            Some<int> someA = 5;
            var mapped = someA.Map(val => val.ToString());
            mapped.Value.Should().Be("5");
        }

        [Test]
        public void Map_MapFnNull_ThrowArgNullException()
        {
            Some<int> someA = 5;
            Func<int, string> fn = null!;

            Action call = () => someA.Map(fn);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("map");
        }
    }
}
