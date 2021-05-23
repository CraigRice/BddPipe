using System;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepAnd = Step.And;

        public static Pipe<R> And<T, R>(this Pipe<T> pipe, string title, Func<T, R> step) =>
            RunPipe(pipe, StepAnd, title, step);

        public static Pipe<R> And<T, R>(this Pipe<T> pipe, string title, Func<T, Task<R>> step) =>
            RunPipe(pipe, StepAnd, title, step);

        public static Pipe<R> And<T, R>(this Pipe<T> pipe, string title, Func<R> step) =>
            RunPipe(pipe, StepAnd, title, step);

        public static Pipe<R> And<T, R>(this Pipe<T> pipe, string title, Func<Task<R>> step) =>
            RunPipe(pipe, StepAnd, title, step);

        public static Pipe<T> And<T>(this Pipe<T> pipe, string title, Func<T, Task> step) =>
            RunPipe(pipe, StepAnd, title, step);

        public static Pipe<T> And<T>(this Pipe<T> pipe, string title, Func<Task> step) =>
            RunPipe(pipe, StepAnd, title, step);

        public static Pipe<T> And<T>(this Pipe<T> pipe, string title, Action<T> step) =>
            RunPipe(pipe, StepAnd, title, step);

        public static Pipe<T> And<T>(this Pipe<T> pipe, string title, Action step) =>
            RunPipe(pipe, StepAnd, title, step);
    }
}
