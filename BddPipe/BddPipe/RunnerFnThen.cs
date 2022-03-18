using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepThen = Step.Then;

        /// <summary>
        /// <see cref="Step.Then"/>
        /// </summary>
        public static Pipe<R> Then<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<T, R> step) =>
            RunPipe(pipe, StepThen, title, step);

        /// <summary>
        /// <see cref="Step.Then"/>
        /// </summary>
        public static Pipe<R> Then<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<T, Task<R>> step) =>
            RunPipe(pipe, StepThen, title, step);

        /// <summary>
        /// <see cref="Step.Then"/>
        /// </summary>
        public static Pipe<R> Then<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<R> step) =>
            RunPipe(pipe, StepThen, title, step);

        /// <summary>
        /// <see cref="Step.Then"/>
        /// </summary>
        public static Pipe<R> Then<T, R>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<Task<R>> step) =>
            RunPipe(pipe, StepThen, title, step);

        /// <summary>
        /// <see cref="Step.Then"/>
        /// </summary>
        public static Pipe<T> Then<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<T, Task> step) =>
            RunPipe(pipe, StepThen, title, step);

        /// <summary>
        /// <see cref="Step.Then"/>
        /// </summary>
        public static Pipe<T> Then<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Func<Task> step) =>
            RunPipe(pipe, StepThen, title, step);

        /// <summary>
        /// <see cref="Step.Then"/>
        /// </summary>
        public static Pipe<T> Then<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Action<T> step) =>
            RunPipe(pipe, StepThen, title, step);

        /// <summary>
        /// <see cref="Step.Then"/>
        /// </summary>
        public static Pipe<T> Then<T>(this Pipe<T> pipe, [AllowNull] string title, [DisallowNull] Action step) =>
            RunPipe(pipe, StepThen, title, step);
    }
}
