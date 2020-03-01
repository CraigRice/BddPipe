using System;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepBut = Step.But;

        public static Pipe<R> But<T, R>(this Pipe<T> pipe, string title, Func<T, R> step) =>
            RunStep(pipe, title.ToTitle(StepBut), step);

        public static Pipe<R> But<T, R>(this Pipe<T> pipe, string title, Func<T, Task<R>> step) =>
            RunStep(pipe, title.ToTitle(StepBut), step);

        public static Pipe<R> But<T, R>(this Pipe<T> pipe, string title, Func<R> step) =>
            RunStep(pipe, title.ToTitle(StepBut), step.PipeFunc<T, R>());

        public static Pipe<R> But<T, R>(this Pipe<T> pipe, string title, Func<Task<R>> step) =>
            RunStep(pipe, title.ToTitle(StepBut), step.PipeFunc<T, R>());

        public static Pipe<T> But<T>(this Pipe<T> pipe, string title, Func<T, Task> step) =>
            RunStep(pipe, title.ToTitle(StepBut), step.PipeFunc());

        public static Pipe<T> But<T>(this Pipe<T> pipe, string title, Func<Task> step) =>
            RunStep(pipe, title.ToTitle(StepBut), step.PipeFunc<T>());

        public static Pipe<T> But<T>(this Pipe<T> pipe, string title, Action<T> step) =>
            RunStep(pipe, title.ToTitle(StepBut), step.PipeFunc());

        public static Pipe<T> But<T>(this Pipe<T> pipe, string title, Action step) =>
            RunStep(pipe, title.ToTitle(StepBut), step.PipeFunc<T>());
    }
}
