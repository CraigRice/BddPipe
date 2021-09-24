using System;
using System.Threading.Tasks;
using BddPipe.UnitTests.Asserts;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace BddPipe.UnitTests.F
{
    public class EitherTests
    {
        private const int defaultLeft = 5;
        private const int newLeft = 55;
        private const string defaultRight = "six";
        private const string expectedNotInitializedMessage = "Either has not been initialized";
        private class SomeInstance { }
        private const string intToStringResult = "value is 5";
        private const string stringToStringResult = "value is six";

        [Test]
        public void Ctor_WithLeftNull_ThrowsArgNullException()
        {
            SomeInstance instance = null;
            Action call = () =>
            {
                Either<SomeInstance, string> either = instance;
            };

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("left");
        }

        [Test]
        public void Ctor_WithRightNull_ThrowsArgNullException()
        {
            string instance = null;
            Action call = () =>
            {
                Either<SomeInstance, string> either = instance;
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
                .Which.Message.Should().Be(expectedNotInitializedMessage);
        }

        [Test]
        public void Match_NotInitialized_ThrowsException()
        {
            Either<int, string> either = default;
            Action call = () => either.Match(r => true, l => true);
            call.Should().ThrowExactly<EitherNotInitialzedException>()
                .Which.Message.Should().Be(expectedNotInitializedMessage);
        }

        [Test]
        public void Match_FnLeftNull_ThrowsArgNullException()
        {
            Either<int, string> either = defaultRight;
            Action call = () => either.Match(x => true, null);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("left");
        }

        [Test]
        public void Match_FnRightNull_ThrowsArgNullException()
        {
            Either<int, string> either = defaultRight;
            Action call = () => either.Match(null, x => true);
            call.Should().ThrowExactly<ArgumentNullException>()
                .Which.ParamName.Should().Be("right");
        }

        [Test]
        public void Match_AssignLeftMatchToBool_RunsLeft()
        {
            Either<int, string> either = defaultLeft;
            var result = either.Match(r => false, l => true);
            result.Should().BeTrue();
        }

        [Test]
        public void Match_AssignRightMatchToBool_RunsRight()
        {
            Either<int, string> either = defaultRight;
            var result = either.Match(r => true, l => false);
            result.Should().BeTrue();
        }

        [Test]
        public void Match_AssignLeftMatchToString_RunsLeft()
        {
            var stringToString = Substitute.For<Func<string, string>>();
            stringToString(defaultRight).Returns(stringToStringResult);

            var intToString = Substitute.For<Func<int, string>>();
            intToString(defaultLeft).Returns(intToStringResult);

            Either<int, string> either = defaultLeft;
            var result = either.Match(stringToString, intToString);
            result.Should().Be(intToStringResult);

            stringToString.DidNotReceive()(Arg.Any<string>());
            intToString.Received()(defaultLeft);
        }

        [Test]
        public void Match_AssignRightMatchToString_RunsRight()
        {
            var stringToString = Substitute.For<Func<string, string>>();
            stringToString(defaultRight).Returns(stringToStringResult);

            var intToString = Substitute.For<Func<int, string>>();
            intToString(defaultLeft).Returns(intToStringResult);

            Either<int, string> either = defaultRight;
            var result = either.Match(stringToString, intToString);
            result.Should().Be(stringToStringResult);

            intToString.DidNotReceive()(Arg.Any<int>());
            stringToString.Received()(defaultRight);
        }

        //[Test]
        //public void Bind_NotInitialized_ThrowsException()
        //{
        //    Either<int, string> either = default;
        //    Action call = () => either.Bind<bool>(r => true);
        //    call.Should().ThrowExactly<EitherNotInitialzedException>()
        //        .Which.Message.Should().Be(expectedNotInitializedMessage);
        //}

        //[Test]
        //public void Bind_FnNull_ThrowsArgNullException()
        //{
        //    Either<int, string> either = defaultRight;
        //    Action call = () => either.Bind<bool>(null);
        //    call.Should().ThrowExactly<ArgumentNullException>()
        //        .Which.ParamName.Should().Be("bind");
        //}

        //[Test]
        //public void Bind_AssignLeftFunctionReturnsNewRight_DoesNotCallBindChangesType()
        //{
        //    var fn = Substitute.For<Func<string, Either<int, bool>>>();
        //    fn(defaultRight).Returns<Either<int, bool>>(true);

        //    Either<int, string> either = defaultLeft;

        //    var result = either.Bind(fn);

        //    result.IsLeft.Should().BeTrue();

        //    var matchToString = result.Match(r => r.ToString(), l => l.ToString());
        //    matchToString.Should().Be(defaultLeft.ToString());

        //    fn.DidNotReceive()(Arg.Any<string>());
        //}

        //[Test]
        //public void Bind_AssignRightFunctionReturnsNewRight_CallsBindChangesType()
        //{
        //    var fn = Substitute.For<Func<string, Either<int, bool>>>();
        //    fn(defaultRight).Returns<Either<int, bool>>(true);

        //    Either<int, string> either = defaultRight;

        //    var result = either.Bind(fn);

        //    result.IsRight.Should().BeTrue();

        //    var matchToString = result.Match(r => r.ToString(), l => l.ToString());
        //    matchToString.Should().Be(true.ToString());

        //    fn.Received()(defaultRight);
        //}

        //[Test]
        //public async Task BindAsync_NotInitialized_ThrowsException()
        //{
        //    Either<int, string> either = default;
        //    Func<Task<Either<int, bool>>> call = () => either.BindAsync(r =>
        //    {
        //        Either<int, bool> result = true;
        //        return Task.FromResult(result);
        //    });

        //    (await call.Should().ThrowExactlyAsync<EitherNotInitialzedException>())
        //        .Which
        //        .Message.Should().Be(expectedNotInitializedMessage);
        //}

        //[Test]
        //public async Task BindAsync_FnNull_ThrowsArgNullException()
        //{
        //    Either<int, string> either = defaultRight;
        //    Func<Task<Either<int, bool>>> call = () => either.BindAsync<bool>(null);
        //    (await call.Should().ThrowExactlyAsync<ArgumentNullException>())
        //        .Which
        //        .ParamName.Should().Be("bind");
        //}

        //[Test]
        //public async Task BindAsync_AssignLeftFunctionReturnsNewRight_DoesNotCallBindChangesType()
        //{
        //    var fn = Substitute.For<Func<string, Task<Either<int, bool>>>>();
        //    fn(defaultRight).Returns(Task.FromResult<Either<int, bool>>(true));

        //    Either<int, string> either = defaultLeft;

        //    var result = await either.BindAsync(fn);

        //    result.IsLeft.Should().BeTrue();

        //    var matchToString = result.Match(r => r.ToString(), l => l.ToString());
        //    matchToString.Should().Be(defaultLeft.ToString());

        //    await fn.DidNotReceive()(Arg.Any<string>());
        //}

        //[Test]
        //public async Task BindAsync_AssignRightFunctionReturnsNewRight_CallsBindChangesType()
        //{
        //    var fn = Substitute.For<Func<string, Task<Either<int, bool>>>>();
        //    fn(defaultRight).Returns(Task.FromResult<Either<int, bool>>(true));

        //    Either<int, string> either = defaultRight;

        //    var result = await either.BindAsync(fn);

        //    result.IsRight.Should().BeTrue();

        //    var matchToString = result.Match(r => r.ToString(), l => l.ToString());
        //    matchToString.Should().Be(true.ToString());

        //    await fn.Received()(defaultRight);
        //}

        //[Test]
        //public void Bind_AssignLeftFunctionReturnsNewLeft_DoesNotCallBindChangesType()
        //{
        //    var fn = Substitute.For<Func<string, Either<int, bool>>>();
        //    fn(defaultRight).Returns<Either<int, bool>>(newLeft);

        //    Either<int, string> either = defaultLeft;

        //    var result = either.Bind(fn);

        //    result.IsLeft.Should().BeTrue();

        //    var matchToString = result.Match(r => r.ToString(), l => l.ToString());
        //    matchToString.Should().Be(defaultLeft.ToString());

        //    fn.DidNotReceive()(Arg.Any<string>());
        //}

        //[Test]
        //public void Bind_AssignRightFunctionReturnsNewLeft_CallsBindChangesType()
        //{
        //    var fn = Substitute.For<Func<string, Either<int, bool>>>();
        //    fn(defaultRight).Returns<Either<int, bool>>(newLeft);

        //    Either<int, string> either = defaultRight;

        //    var result = either.Bind(fn);

        //    result.IsLeft.Should().BeTrue();

        //    var matchToString = result.Match(r => r.ToString(), l => l.ToString());
        //    matchToString.Should().Be(newLeft.ToString());

        //    fn.Received()(defaultRight);
        //}

        //[Test]
        //public async Task BindAsync_AssignLeftFunctionReturnsNewLeft_DoesNotCallBindChangesType()
        //{
        //    var fn = Substitute.For<Func<string, Task<Either<int, bool>>>>();
        //    fn(defaultRight).Returns(Task.FromResult<Either<int, bool>>(newLeft));

        //    Either<int, string> either = defaultLeft;

        //    var result = await either.BindAsync(fn);

        //    result.IsLeft.Should().BeTrue();

        //    var matchToString = result.Match(r => r.ToString(), l => l.ToString());
        //    matchToString.Should().Be(defaultLeft.ToString());

        //    await fn.DidNotReceive()(Arg.Any<string>());
        //}

        //[Test]
        //public async Task BindAsync_AssignRightFunctionReturnsNewLeft_CallsBindChangesType()
        //{
        //    var fn = Substitute.For<Func<string, Task<Either<int, bool>>>>();
        //    fn(defaultRight).Returns(Task.FromResult<Either<int, bool>>(newLeft));

        //    Either<int, string> either = defaultRight;

        //    var result = await either.BindAsync(fn);

        //    result.IsLeft.Should().BeTrue();

        //    var matchToString = result.Match(r => r.ToString(), l => l.ToString());
        //    matchToString.Should().Be(newLeft.ToString());

        //    await fn.Received()(defaultRight);
        //}

        [Test]
        public void IsLeft_NotInitialized_True()
        {
            Either<int, string> either = default;
            either.IsLeft.Should().BeTrue();
        }

        [Test]
        public void IsLeft_AssignLeft_True()
        {
            Either<int, string> either = defaultLeft;
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
            Either<int, string> either = defaultRight;
            either.IsRight.Should().BeTrue();
            either.IsLeft.Should().BeFalse();
        }

        [Test]
        public void ToString_AssignLeft_AsExpected()
        {
            Either<int, string> either = defaultLeft;
            either.ToString().Should().Be("left(5)");
        }

        [Test]
        public void ToString_AssignRight_AsExpected()
        {
            Either<int, string> either = defaultRight;
            either.ToString().Should().Be("right(six)");
        }

        [Test]
        public void BiBind_WhenRightBindToLeft_NewLeft()
        {
            const int newLeft = 100;
            Func<int, Either<int, bool>> fnLeft = val => 1234;
            Func<string, Either<int, bool>> fnRight = val => newLeft;

            // act
            Either<int, string> either = defaultRight;
            var result = either.BiBind(fnRight, fnLeft);

            result.ShouldBeLeft(left =>
            {
                left.Should().Be(newLeft);
            });
        }

        [Test]
        public void BiBind_WhenRightBindToRight_NewRight()
        {
            const bool newRight = true;
            Func<int, Either<int, bool>> fnLeft = val => false;
            Func<string, Either<int, bool>> fnRight = val => newRight;

            // act
            Either<int, string> either = defaultRight;
            var result = either.BiBind(fnRight, fnLeft);

            result.ShouldBeRight(right =>
            {
                right.Should().Be(newRight);
            });
        }

        [Test]
        public void BiBind_WhenLeftBindToLeft_NewLeft()
        {
            const int newLeft = 100;
            Func<int, Either<int, bool>> fnLeft = val => newLeft;
            Func<string, Either<int, bool>> fnRight = val => 1234;

            // act
            Either<int, string> either = defaultLeft;
            var result = either.BiBind(fnRight, fnLeft);

            result.ShouldBeLeft(left =>
            {
                left.Should().Be(newLeft);
            });
        }

        [Test]
        public void BiBind_WhenLeftBindToRight_NewRight()
        {
            const bool newRight = true;
            Func<int, Either<int, bool>> fnLeft = val => newRight;
            Func<string, Either<int, bool>> fnRight = val => false;

            // act
            Either<int, string> either = defaultLeft;
            var result = either.BiBind(fnRight, fnLeft);

            result.ShouldBeRight(right =>
            {
                right.Should().Be(newRight);
            });
        }
    }
}
