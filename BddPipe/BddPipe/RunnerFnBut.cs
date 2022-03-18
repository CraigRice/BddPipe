using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepBut = Step.But;

        /// <summary>
        /// <see cref="Step.But"/>
        /// </summary>
        public static Pipe<R> But<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<T, R> step) =>
            RunPipe(pipe, StepBut, title, step);

        /// <summary>
        /// <see cref="Step.But"/>
        /// </summary>
        public static Pipe<R> But<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<T, Task<R>> step) =>
            RunPipe(pipe, StepBut, title, step);

        /// <summary>
        /// <see cref="Step.But"/>
        /// </summary>
        public static Pipe<R> But<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<R> step) =>
            RunPipe(pipe, StepBut, title, step);

        /// <summary>
        /// <see cref="Step.But"/>
        /// </summary>
        public static Pipe<R> But<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<Task<R>> step) =>
            RunPipe(pipe, StepBut, title, step);

        /// <summary>
        /// <see cref="Step.But"/>
        /// </summary>
        public static Pipe<T> But<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<T, Task> step) =>
            RunPipe(pipe, StepBut, title, step);

        /// <summary>
        /// <see cref="Step.But"/>
        /// </summary>
        public static Pipe<T> But<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<Task> step) =>
            RunPipe(pipe, StepBut, title, step);

        /// <summary>
        /// <see cref="Step.But"/>
        /// </summary>
        public static Pipe<T> But<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Action<T> step) =>
            RunPipe(pipe, StepBut, title, step);

        /// <summary>
        /// <see cref="Step.But"/>
        /// </summary>
        public static Pipe<T> But<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Action step) =>
            RunPipe(pipe, StepBut, title, step);
    }
}
