using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace BddPipe.UnitTests.F
{
    [TestFixture]
    public class EitherTests
    {
        private const int DefaultLeft = 5;
        private const int NewLeft = 55;
        private const string DefaultRight = "six";
        private const string ExpectedNotInitializedMessage = "Either has not been initialized";
        private const string IntToStringResult = "value is 5";
        private const string StringToStringResult = "value is six";

        [Test]
        public void Ctor_WithLeftNull_ThrowsArgNullException()
        {
            object instance = null!;
            Action call = () =>
            {
                Either<object, string> _ = instance;
            };

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("left");
        }

        [Test]
        public void Ctor_WithRightNull_ThrowsArgNullException()
        {
            string instance = null!;
            Action call = () =>
            {
                Either<object, string> _ = instance;
            };

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("right");
        }

        [Test]
        public void ToString_NotInitialized_ThrowsException()
        {
            Either<int, string> either = default;
            Func<string> call = () => either.ToString();
            call.Should().ThrowExactly<EitherNotInitialzedException>()
                .Which.Message.Should().Be(ExpectedNotInitializedMessage);
        }

        [Test]
        public void Match_NotInitialized_ThrowsException()
        {
            Either<int, string> either = default;
            Action call = () => either.Match(_ => true, _ => true);
            call.Should().ThrowExactly<EitherNotInitialzedException>()
                .Which.Message.Should().Be(ExpectedNotInitializedMessage);
        }

        [Test]
        public void Match_FnLeftNull_ThrowsArgNullException()
        {
            Either<int, string> either = DefaultRight;
            Action call = () => either.Match(_ => true, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("left");
        }

        [Test]
        public void Match_FnRightNull_ThrowsArgNullException()
        {
            Either<int, string> either = DefaultRight;
            Action call = () => either.Match(null, _ => true);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("right");
        }

        [Test]
        public void Match_AssignLeftMatchToBool_RunsLeft()
        {
            Either<int, string> either = DefaultLeft;
            var result = either.Match(_ => false, _ => true);
            result.Should().BeTrue();
        }

        [Test]
        public void Match_AssignRightMatchToBool_RunsRight()
        {
            Either<int, string> either = DefaultRight;
            var result = either.Match(_ => true, _ => false);
            result.Should().BeTrue();
        }

        [Test]
        public void Match_AssignLeftMatchToString_RunsLeft()
        {
            var stringToString = Substitute.For<Func<string, string>>();
            stringToString(DefaultRight).Returns(StringToStringResult);

            var intToString = Substitute.For<Func<int, string>>();
            intToString(DefaultLeft).Returns(IntToStringResult);

            Either<int, string> either = DefaultLeft;
            var result = either.Match(stringToString, intToString);
            result.Should().Be(IntToStringResult);

            stringToString.DidNotReceive()(Arg.Any<string>());
            intToString.Received()(DefaultLeft);
        }

        [Test]
        public void Match_AssignRightMatchToString_RunsRight()
        {
            var stringToString = Substitute.For<Func<string, string>>();
            stringToString(DefaultRight).Returns(StringToStringResult);

            var intToString = Substitute.For<Func<int, string>>();
            intToString(DefaultLeft).Returns(IntToStringResult);

            Either<int, string> either = DefaultRight;
            var result = either.Match(stringToString, intToString);
            result.Should().Be(StringToStringResult);

            intToString.DidNotReceive()(Arg.Any<int>());
            stringToString.Received()(DefaultRight);
        }

        [Test]
        public void Bind_NotInitialized_ThrowsException()
        {
            Either<int, string> either = default;
            Action call = () => either.Bind<bool>(_ => true);
            call.Should().ThrowExactly<EitherNotInitialzedException>()
                .Which.Message.Should().Be(ExpectedNotInitializedMessage);
        }

        [Test]
        public void Bind_FnNull_ThrowsArgNullException()
        {
            Either<int, string> either = DefaultRight;
            Action call = () => either.Bind<bool>(null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("bind");
        }

        [Test]
        public void Bind_AssignLeftFunctionReturnsNewRight_DoesNotCallBindChangesType()
        {
            Either<int, string> either = DefaultLeft;

            var result = either.Bind<bool>(_ => true);

            result.IsLeft.Should().BeTrue();

            var matchToString = result.Match(r => r.ToString(), l => l.ToString());
            matchToString.Should().Be(DefaultLeft.ToString());
        }

        [Test]
        public void Bind_AssignRightFunctionReturnsNewRight_CallsBindChangesType()
        {
            Either<int, string> either = DefaultRight;

            var result = either.Bind<bool>(_ => true);

            result.IsRight.Should().BeTrue();

            var matchToString = result.Match(r => r.ToString(), l => l.ToString());
            matchToString.Should().Be(true.ToString());
        }

        [Test]
        public async Task BindAsync_NotInitialized_ThrowsException()
        {
            Either<int, string> either = default;
            Func<Task<Either<int, bool>>> call = () => either.BindAsync(_ =>
            {
                Either<int, bool> result = true;
                return Task.FromResult(result);
            });

            (await call.Should().ThrowExactlyAsync<EitherNotInitialzedException>())
                .Which
                .Message.Should().Be(ExpectedNotInitializedMessage);
        }

        [Test]
        public async Task BindAsync_FnNull_ThrowsArgNullException()
        {
            Either<int, string> either = DefaultRight;
            Func<Task<Either<int, bool>>> call = () => either.BindAsync<bool>(null);
            (await call.Should().ThrowExactlyAsync<ArgumentNullException>())
                .Which
                .ParamName.Should().Be("bind");
        }

        [Test]
        public async Task BindAsync_AssignLeftFunctionReturnsNewRight_DoesNotCallBindChangesType()
        {
            Either<int, string> either = DefaultLeft;

            var result = await either.BindAsync(_ => Task.FromResult<Either<int, bool>>(true));

            result.IsLeft.Should().BeTrue();

            var matchToString = result.Match(r => r.ToString(), l => l.ToString());
            matchToString.Should().Be(DefaultLeft.ToString());
        }

        [Test]
        public async Task BindAsync_AssignRightFunctionReturnsNewRight_CallsBindChangesType()
        {
            Either<int, string> either = DefaultRight;

            var result = await either.BindAsync(_ => Task.FromResult<Either<int, bool>>(true));

            result.IsRight.Should().BeTrue();

            var matchToString = result.Match(r => r.ToString(), l => l.ToString());
            matchToString.Should().Be(true.ToString());
        }

        [Test]
        public void Bind_AssignLeftFunctionReturnsNewLeft_DoesNotCallBindChangesType()
        {
            Either<int, string> either = DefaultLeft;

            var result = either.Bind<bool>(_ => NewLeft);

            result.IsLeft.Should().BeTrue();

            var matchToString = result.Match(r => r.ToString(), l => l.ToString());
            matchToString.Should().Be(DefaultLeft.ToString());
        }

        [Test]
        public void Bind_AssignRightFunctionReturnsNewLeft_CallsBindChangesType()
        {
            Either<int, string> either = DefaultRight;

            var result = either.Bind<bool>(_ => NewLeft);

            result.IsLeft.Should().BeTrue();

            var matchToString = result.Match(r => r.ToString(), l => l.ToString());
            matchToString.Should().Be(NewLeft.ToString());
        }

        [Test]
        public async Task BindAsync_AssignLeftFunctionReturnsNewLeft_DoesNotCallBindChangesType()
        {
            Either<int, string> either = DefaultLeft;

            var result = await either.BindAsync(_ => Task.FromResult<Either<int, bool>>(NewLeft));

            result.IsLeft.Should().BeTrue();

            var matchToString = result.Match(r => r.ToString(), l => l.ToString());
            matchToString.Should().Be(DefaultLeft.ToString());
        }

        [Test]
        public async Task BindAsync_AssignRightFunctionReturnsNewLeft_CallsBindChangesType()
        {
            Either<int, string> either = DefaultRight;

            var result = await either.BindAsync(_ => Task.FromResult<Either<int, bool>>(NewLeft));

            result.IsLeft.Should().BeTrue();

            var matchToString = result.Match(r => r.ToString(), l => l.ToString());
            matchToString.Should().Be(NewLeft.ToString());
        }

        [Test]
        public void IsLeft_NotInitialized_True()
        {
            Either<int, string> either = default;
            either.IsLeft.Should().BeTrue();
        }

        [Test]
        public void IsLeft_AssignLeft_True()
        {
            Either<int, string> either = DefaultLeft;
            either.IsLeft.Should().BeTrue();
            either.IsRight.Should().BeFalse();
        }

        [Test]
        public void IsRight_NotInitialized_False()
        {
            Either<int, string> either = default;
            either.IsRight.Should().BeFalse();
        }

        [Test]
        public void IsRight_AssignRight_True()
        {
            Either<int, string> either = DefaultRight;
            either.IsRight.Should().BeTrue();
            either.IsLeft.Should().BeFalse();
        }

        [Test]
        public void ToString_AssignLeft_AsExpected()
        {
            Either<int, string> either = DefaultLeft;
            either.ToString().Should().Be("left(5)");
        }

        [Test]
        public void ToString_AssignRight_AsExpected()
        {
            Either<int, string> either = DefaultRight;
            either.ToString().Should().Be("right(six)");
        }

        /// <summary>
        /// m == m.Bind(Return)
        /// </summary>
        [Test]
        public void Fn_RightIdentityHolds()
        {
            Either<int, string> m = DefaultRight;
            Func<string, Either<int, string>> fnReturn = value => new Either<int, string>(value);

            m.Should().Be(m.Bind(fnReturn));
        }

        [Test]
        public void Fn_RightIdentityHolds2()
        {
            Either<int, string> m = DefaultLeft;
            Func<string, Either<int, string>> fnReturn = value => new Either<int, string>(value);

            m.Should().Be(m.Bind(fnReturn));
        }

        /// <summary>
        /// Return(t).Bind(f) == f(t)
        /// </summary>
        [Test]
        public void Fn_LeftIdentityHolds()
        {
            Either<int, string> either = DefaultRight;
            Func<string, Either<int, string>> f = _ => new Either<int, string>("value 1");
            either.Bind(f).Should().Be(f(DefaultRight));
        }

        /// <summary>
        /// m.Bind(f).Bind(g) == m.Bind(x => f(x).Bind(g))
        /// (m >>= f) >>= g == m >>= (x => f(x) >>= g)
        /// </summary>
        [Test]
        public void Fn_AssociativityHolds()
        {
            Either<int, string> m = DefaultRight;
            Func<string, Either<int, string>> f = _ => new Either<int, string>("value 1");
            Func<string, Either<int, string>> g = _ => new Either<int, string>("value 2");

            m.Bind(f).Bind(g).Should().Be(m.Bind(x => f(x).Bind(g)));
        }

        [Test]
        public void Fn_AssociativityHolds2()
        {
            Either<int, string> m = DefaultLeft;
            Func<string, Either<int, string>> f = _ => new Either<int, string>("value 1");
            Func<string, Either<int, string>> g = _ => new Either<int, string>("value 2");

            m.Bind(f).Bind(g).Should().Be(m.Bind(x => f(x).Bind(g)));
        }
    }
}
