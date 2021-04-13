using System;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepBut = Step.But;

        public static Pipe<R> But<T, R>(this Pipe<T> pipe, string title, Func<T, R> step) =>
            RunPipe(pipe, StepBut, title, step);

        public static Pipe<R> But<T, R>(this Pipe<T> pipe, string title, Func<T, Task<R>> step) =>
            RunPipe(pipe, StepBut, title, step);

        public static Pipe<R> But<T, R>(this Pipe<T> pipe, string title, Func<R> step) =>
            RunPipe(pipe, StepBut, title, step);

        public static Pipe<R> But<T, R>(this Pipe<T> pipe, string title, Func<Task<R>> step) =>
            RunPipe(pipe, StepBut, title, step);

        public static Pipe<T> But<T>(this Pipe<T> pipe, string title, Func<T, Task> step) =>
            RunPipe(pipe, StepBut, title, step);

        public static Pipe<T> But<T>(this Pipe<T> pipe, string title, Func<Task> step) =>
            RunPipe(pipe, StepBut, title, step);

        public static Pipe<T> But<T>(this Pipe<T> pipe, string title, Action<T> step) =>
            RunPipe(pipe, StepBut, title, step);

        public static Pipe<T> But<T>(this Pipe<T> pipe, string title, Action step) =>
            RunPipe(pipe, StepBut, title, step);
    }
}
