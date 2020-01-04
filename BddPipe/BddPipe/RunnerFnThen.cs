using System;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepThen = Step.Then;

        public static Pipe<R> Then<T, R>(this Pipe<T> t, string title, Func<T, R> step) =>
            RunStep(t, title.ToTitle(StepThen), step);

        public static Pipe<R> Then<T, R>(this Pipe<T> t, string title, Func<T, Task<R>> step) =>
            RunStep(t, title.ToTitle(StepThen), step);

        public static Pipe<R> Then<T, R>(this Pipe<T> t, string title, Func<R> step) =>
            RunStep(t, title.ToTitle(StepThen), step.PipeFunc<T, R>());

        public static Pipe<R> Then<T, R>(this Pipe<T> t, string title, Func<Task<R>> step) =>
            RunStep(t, title.ToTitle(StepThen), step.PipeFunc<T, R>());

        public static Pipe<T> Then<T>(this Pipe<T> t, string title, Func<T, Task> step) =>
            RunStep(t, title.ToTitle(StepThen), step.PipeFunc());

        public static Pipe<T> Then<T>(this Pipe<T> t, string title, Func<Task> step) =>
            RunStep(t, title.ToTitle(StepThen), step.PipeFunc<T>());

        public static Pipe<T> Then<T>(this Pipe<T> t, string title, Action<T> step) =>
            RunStep(t, title.ToTitle(StepThen), step.PipeFunc());

        public static Pipe<T> Then<T>(this Pipe<T> t, string title, Action step) =>
            RunStep(t, title.ToTitle(StepThen), step.PipeFunc<T>());
    }
}
