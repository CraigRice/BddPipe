using System;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepBut = Step.But;

        public static Pipe<R> But<T, R>(this Pipe<T> t, string title, Func<T, R> step) =>
            RunStep(t, title.ToTitle(StepBut), step);

        public static Pipe<R> But<T, R>(this Pipe<T> t, string title, Func<T, Task<R>> step) =>
            RunStep(t, title.ToTitle(StepBut), step);

        public static Pipe<R> But<T, R>(this Pipe<T> t, string title, Func<R> step) =>
            RunStep(t, title.ToTitle(StepBut), step.PipeFunc<T, R>());

        public static Pipe<R> But<T, R>(this Pipe<T> t, string title, Func<Task<R>> step) =>
            RunStep(t, title.ToTitle(StepBut), step.PipeFunc<T, R>());

        public static Pipe<T> But<T>(this Pipe<T> t, string title, Func<T, Task> step) =>
            RunStep(t, title.ToTitle(StepBut), step.PipeFunc());

        public static Pipe<T> But<T>(this Pipe<T> t, string title, Func<Task> step) =>
            RunStep(t, title.ToTitle(StepBut), step.PipeFunc<T>());

        public static Pipe<T> But<T>(this Pipe<T> t, string title, Action<T> step) =>
            RunStep(t, title.ToTitle(StepBut), step.PipeFunc());

        public static Pipe<T> But<T>(this Pipe<T> t, string title, Action step) =>
            RunStep(t, title.ToTitle(StepBut), step.PipeFunc<T>());
    }
}
