using System;
using System.Threading.Tasks;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepWhen = Step.When;

        public static Either<Ctn<Exception>, Ctn<R>> When<T, R>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Func<T, R> step) =>
            RunStep(t, title.ToTitle(StepWhen), step);

        public static Either<Ctn<Exception>, Ctn<R>> When<T, R>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Func<T, Task<R>> step) =>
            RunStep(t, title.ToTitle(StepWhen), step);

        public static Either<Ctn<Exception>, Ctn<R>> When<T, R>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Func<R> step) =>
            RunStep(t, title.ToTitle(StepWhen), step.PipeFunc<T, R>());

        public static Either<Ctn<Exception>, Ctn<R>> When<T, R>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Func<Task<R>> step) =>
            RunStep(t, title.ToTitle(StepWhen), step.PipeFunc<T, R>());

        public static Either<Ctn<Exception>, Ctn<T>> When<T>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Func<T, Task> step) =>
            RunStep(t, title.ToTitle(StepWhen), step.PipeFunc());

        public static Either<Ctn<Exception>, Ctn<T>> When<T>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Func<Task> step) =>
            RunStep(t, title.ToTitle(StepWhen), step.PipeFunc<T>());

        public static Either<Ctn<Exception>, Ctn<T>> When<T>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Action<T> step) =>
            RunStep(t, title.ToTitle(StepWhen), step.PipeFunc());

        public static Either<Ctn<Exception>, Ctn<T>> When<T>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Action step) =>
            RunStep(t, title.ToTitle(StepWhen), step.PipeFunc<T>());
    }
}
