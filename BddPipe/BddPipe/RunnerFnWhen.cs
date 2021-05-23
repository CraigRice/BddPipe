using System;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepWhen = Step.When;

        public static Pipe<R> When<T, R>(this Pipe<T> pipe, string title, Func<T, R> step) =>
            RunPipe(pipe, StepWhen, title, step);

        public static Pipe<R> When<T, R>(this Pipe<T> pipe, string title, Func<T, Task<R>> step) =>
            RunPipe(pipe, StepWhen, title, step);

        public static Pipe<R> When<T, R>(this Pipe<T> pipe, string title, Func<R> step) =>
            RunPipe(pipe, StepWhen, title, step);

        public static Pipe<R> When<T, R>(this Pipe<T> pipe, string title, Func<Task<R>> step) =>
            RunPipe(pipe, StepWhen, title, step);

        public static Pipe<T> When<T>(this Pipe<T> pipe, string title, Func<T, Task> step) =>
            RunPipe(pipe, StepWhen, title, step);

        public static Pipe<T> When<T>(this Pipe<T> pipe, string title, Func<Task> step) =>
            RunPipe(pipe, StepWhen, title, step);

        public static Pipe<T> When<T>(this Pipe<T> pipe, string title, Action<T> step) =>
            RunPipe(pipe, StepWhen, title, step);

        public static Pipe<T> When<T>(this Pipe<T> pipe, string title, Action step) =>
            RunPipe(pipe, StepWhen, title, step);
    }
}
