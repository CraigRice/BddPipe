using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepAnd = Step.And;

        /// <summary>
        /// <see cref="Step.And"/>
        /// </summary>
        public static Pipe<R> And<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<T, R> step) =>
            RunPipe(pipe, StepAnd, title, step);

        /// <summary>
        /// <see cref="Step.And"/>
        /// </summary>
        public static Pipe<R> And<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<T, Task<R>> step) =>
            RunPipe(pipe, StepAnd, title, step);

        /// <summary>
        /// <see cref="Step.And"/>
        /// </summary>
        public static Pipe<R> And<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<R> step) =>
            RunPipe(pipe, StepAnd, title, step);

        /// <summary>
        /// <see cref="Step.And"/>
        /// </summary>
        public static Pipe<R> And<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<Task<R>> step) =>
            RunPipe(pipe, StepAnd, title, step);

        /// <summary>
        /// <see cref="Step.And"/>
        /// </summary>
        public static Pipe<T> And<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<T, Task> step) =>
            RunPipe(pipe, StepAnd, title, step);

        /// <summary>
        /// <see cref="Step.And"/>
        /// </summary>
        public static Pipe<T> And<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<Task> step) =>
            RunPipe(pipe, StepAnd, title, step);

        /// <summary>
        /// <see cref="Step.And"/>
        /// </summary>
        public static Pipe<T> And<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Action<T> step) =>
            RunPipe(pipe, StepAnd, title, step);

        /// <summary>
        /// <see cref="Step.And"/>
        /// </summary>
        public static Pipe<T> And<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Action step) =>
            RunPipe(pipe, StepAnd, title, step);
    }
}
