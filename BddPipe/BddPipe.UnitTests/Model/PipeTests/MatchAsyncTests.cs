using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using BddPipe.Model;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using static BddPipe.F;

namespace BddPipe.UnitTests.Model.PipeTests
{
    [TestFixture]
    public class MatchAsyncTests
    {
        private const int DefaultValue = 45;

        [Test]
        public async Task MatchAsync_DefaultPipe_ThrowsNotInitializedException()
        {
            Func<Task> call = async () =>
            {
                await default(Pipe<int>).MatchAsync(pipeState => Task.FromResult(pipeState.Value), e => Task.FromResult(DefaultValue));
            };

            (await call.Should().ThrowExactlyAsync<PipeNotInitializedException>())
                .WithMessage("Pipe has not been initialized");
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncRight_CallsFuncRight(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);

            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnT = Substitute.For<Func<PipeState<int>, Task<Unit>>>();
            var fnError = Substitute.For<Func<PipeErrorState, Task<Unit>>>();

            await pipe.MatchAsync(fnT, fnError);

            await fnT.Received()(Arg.Is<PipeState<int>>(state => state.Value == DefaultValue));
            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncLeft_CallsFuncLeft(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnT = Substitute.For<Func<PipeState<int>, Task<Unit>>>();
            var fnError = Substitute.For<Func<PipeErrorState, Task<Unit>>>();

            await pipe.MatchAsync(fnT, fnError);

            fnT.DidNotReceive();
            await fnError.Received()(Arg.Any<PipeErrorState>());
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncRight_ReturnsFuncOutput(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnError = Substitute.For<Func<PipeErrorState, Task<string>>>();

            const string resultText = "some result";
            var result = await pipe.MatchAsync(value => Task.FromResult(resultText), fnError);

            result.Should().Be(resultText);

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncLeft_ReturnsFuncOutput(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnT = Substitute.For<Func<PipeState<int>, Task<string>>>();

            const string resultText = "some result";
            var result = await pipe.MatchAsync(fnT, value => Task.FromResult(resultText));

            result.Should().Be(resultText);

            fnT.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncRightNull_ThrowsArgNullException(bool async)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(DefaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnError = Substitute.For<Func<PipeErrorState, Task<Unit>>>();

            Func<Task> call = () => pipe.MatchAsync(null, fnError);
            (await call.Should().ThrowExactlyAsync<ArgumentNullException>())
                .Which
                .ParamName.Should().Be("value");

            fnError.DidNotReceive();
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task MatchAsync_WithFuncLeftNull_ThrowsArgNullException(bool async)
        {
            var exInfo = ExceptionDispatchInfo.Capture(new ApplicationException("test error"));
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<ExceptionDispatchInfo>(exInfo, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var fnT = Substitute.For<Func<PipeState<int>, Task<Unit>>>();

            Func<Task> call = () => pipe.MatchAsync(fnT, null);
            (await call.Should().ThrowExactlyAsync<ArgumentNullException>())
                .Which
                .ParamName.Should().Be("error");

            fnT.DidNotReceive();
        }
    }
}
