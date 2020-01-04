using System;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepWhen = Step.When;

        public static Pipe<R> When<T, R>(this Pipe<T> t, string title, Func<T, R> step) =>
            RunStep(t, title.ToTitle(StepWhen), step);

        public static Pipe<R> When<T, R>(this Pipe<T> t, string title, Func<T, Task<R>> step) =>
            RunStep(t, title.ToTitle(StepWhen), step);

        public static Pipe<R> When<T, R>(this Pipe<T> t, string title, Func<R> step) =>
            RunStep(t, title.ToTitle(StepWhen), step.PipeFunc<T, R>());

        public static Pipe<R> When<T, R>(this Pipe<T> t, string title, Func<Task<R>> step) =>
            RunStep(t, title.ToTitle(StepWhen), step.PipeFunc<T, R>());

        public static Pipe<T> When<T>(this Pipe<T> t, string title, Func<T, Task> step) =>
            RunStep(t, title.ToTitle(StepWhen), step.PipeFunc());

        public static Pipe<T> When<T>(this Pipe<T> t, string title, Func<Task> step) =>
            RunStep(t, title.ToTitle(StepWhen), step.PipeFunc<T>());

        public static Pipe<T> When<T>(this Pipe<T> t, string title, Action<T> step) =>
            RunStep(t, title.ToTitle(StepWhen), step.PipeFunc());

        public static Pipe<T> When<T>(this Pipe<T> t, string title, Action step) =>
            RunStep(t, title.ToTitle(StepWhen), step.PipeFunc<T>());
    }
}
