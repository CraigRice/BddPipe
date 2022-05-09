using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using BddPipe.Model;
using static BddPipe.F;

namespace BddPipe.UnitTests.Model.PipeTests
{
    internal static class PipeTestsHelper
    {
        public static Pipe<T> CreatePipe<T>(T value, bool fromTask, IReadOnlyList<StepOutcome> stepOutcomes, string scenarioTitle)
        {
            Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> ctn = new Ctn<T>(value, stepOutcomes, scenarioTitle);
            return fromTask
                ? new Pipe<T>(Task.FromResult(ctn))
                : new Pipe<T>(ctn);
        }

        public static Pipe<T> CreatePipe<T>(T value, bool fromTask) =>
            fromTask
                ? new Pipe<T>(Task.FromResult<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>>(new Ctn<T>(value, None)))
                : new Pipe<T>(new Ctn<T>(value, None));

        public static Pipe<T> CreatePipeErrorState<T>(bool fromTask, ExceptionDispatchInfo exDispatchInfo = null)
        {
            var exInfo = exDispatchInfo ?? ExceptionDispatchInfo.Capture(new ApplicationException("test error"));

            return
                fromTask
                    ? new Pipe<T>(Task.FromResult<Either<Ctn<ExceptionDispatchInfo>, Ctn<T>>>(new Ctn<ExceptionDispatchInfo>(exInfo, None)))
                    : new Pipe<T>(new Ctn<ExceptionDispatchInfo>(exInfo, None));
        }
    }
}
