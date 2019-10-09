using System;
using System.Threading.Tasks;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepBut = Step.But;

        public static Either<Ctn<Exception>, Ctn<R>> But<T, R>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Func<T, R> step)
            => Pipe(t, title.ToTitle(StepBut), step);

        public static Either<Ctn<Exception>, Ctn<R>> But<T, R>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Func<T, Task<R>> step)
            => Pipe(t, title.ToTitle(StepBut), step);

        public static Either<Ctn<Exception>, Ctn<R>> But<T, R>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Func<R> step) =>
            Pipe(t, title.ToTitle(StepBut), step.PipeFunc<T, R>());

        public static Either<Ctn<Exception>, Ctn<R>> But<T, R>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Func<Task<R>> step) =>
            Pipe(t, title.ToTitle(StepBut), step.PipeFunc<T, R>());

        public static Either<Ctn<Exception>, Ctn<T>> But<T>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Func<T, Task> step)
            => Pipe(t, title.ToTitle(StepBut), step.PipeFunc());

        public static Either<Ctn<Exception>, Ctn<T>> But<T>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Func<Task> step) =>
            Pipe(t, title.ToTitle(StepBut), step.PipeFunc<T>());

        public static Either<Ctn<Exception>, Ctn<T>> But<T>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Action<T> step) =>
            Pipe(t, title.ToTitle(StepBut), step.PipeFunc());

        public static Either<Ctn<Exception>, Ctn<T>> But<T>(this Either<Ctn<Exception>, Ctn<T>> t, string title, Action step) =>
            Pipe(t, title.ToTitle(StepBut), step.PipeFunc<T>());
    }
}
