using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepWhen = Step.When;

        /// <summary>
        /// <see cref="Step.When"/>
        /// </summary>
        public static Pipe<R> When<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<T, R> step) =>
            RunPipe(pipe, StepWhen, title, step);

        /// <summary>
        /// <see cref="Step.When"/>
        /// </summary>
        public static Pipe<R> When<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<T, Task<R>> step) =>
            RunPipe(pipe, StepWhen, title, step);

        /// <summary>
        /// <see cref="Step.When"/>
        /// </summary>
        public static Pipe<R> When<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<R> step) =>
            RunPipe(pipe, StepWhen, title, step);

        /// <summary>
        /// <see cref="Step.When"/>
        /// </summary>
        public static Pipe<R> When<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<Task<R>> step) =>
            RunPipe(pipe, StepWhen, title, step);

        /// <summary>
        /// <see cref="Step.When"/>
        /// </summary>
        public static Pipe<T> When<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<T, Task> step) =>
            RunPipe(pipe, StepWhen, title, step);

        /// <summary>
        /// <see cref="Step.When"/>
        /// </summary>
        public static Pipe<T> When<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<Task> step) =>
            RunPipe(pipe, StepWhen, title, step);

        /// <summary>
        /// <see cref="Step.When"/>
        /// </summary>
        public static Pipe<T> When<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Action<T> step) =>
            RunPipe(pipe, StepWhen, title, step);

        /// <summary>
        /// <see cref="Step.When"/>
        /// </summary>
        public static Pipe<T> When<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Action step) =>
            RunPipe(pipe, StepWhen, title, step);
    }
}
