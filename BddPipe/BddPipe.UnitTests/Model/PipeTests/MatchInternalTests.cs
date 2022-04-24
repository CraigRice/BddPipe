using System;
using System.Runtime.ExceptionServices;
using static BddPipe.F;
using System.Threading.Tasks;
using BddPipe.Model;
using FluentAssertions;
using NUnit.Framework;

namespace BddPipe.UnitTests.Model.PipeTests
{
    [TestFixture]
    public class MatchInternalTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void MatchInternal_WithPipe_MatchesOnState(bool async)
        {
            const int defaultValue = 68;
            Either<Ctn<ExceptionDispatchInfo>, Ctn<int>> pipeState = new Ctn<int>(defaultValue, None);
            var pipe = async
                ? new Pipe<int>(Task.FromResult(pipeState))
                : new Pipe<int>(pipeState);

            var result = pipe.MatchInternal(
                syncState => false,
                asyncState => true);

            result.Should().Be(async);
        }

        [Test]
        public void MatchInternal_DefaultPipe_ThrowsNotInitializedException()
        {
            Pipe<int> pipe = default;

            Action call = () => pipe.MatchInternal(
                syncState => false,
                asyncState => true);

            call.Should().ThrowExactly<PipeNotInitializedException>()
                .WithMessage("Pipe has not been initialized");
        }

        [Test]
        public void MatchInternal_FnSyncStateNull_ThrowsArgNullException()
        {
            Pipe<int> pipe = default;

            Action call = () => pipe.MatchInternal(
                null,
                asyncState => true);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("fnSyncState");
        }

        [Test]
        public void MatchInternal_AsyncStateNull_ThrowsArgNullException()
        {
            Pipe<int> pipe = default;

            Action call = () => pipe.MatchInternal(
                syncState => false,
                null);

            call.Should().ThrowExactly<ArgumentNullException>()
                .Which
                .ParamName.Should().Be("fnAsyncState");
        }
    }
}
