using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace BddPipe.Model
{
    /// <summary>
    /// Recipe defines a proxy for Step calls against the original <see cref="Pipe{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    public sealed class Recipe<T, R>
    {
        private readonly Pipe<T> _pipe;
        private readonly Step _step;

        internal Recipe(Pipe<T> pipe, Step step)
        {
            _pipe = pipe;
            _step = step;
        }

        /// <summary>
        /// Projects from one value to another.
        /// <remarks>A failure to map will impact the current step as if this happened in the step itself.</remarks>
        /// </summary>
        /// <typeparam name="T2">Type of the resulting value</typeparam>
        /// <param name="map">A function to map the current value to its new value.</param>
        /// <returns>A new <see cref="Recipe{T2, R}"/> instance of the destination type</returns>
        [return: NotNull]
        public Recipe<T2, R> Map<T2>([DisallowNull] Func<T, T2> map)
        {
            if (map == null) { throw new ArgumentNullException(nameof(map)); }
            return new Recipe<T2, R>(_pipe.Map(map), _step);
        }

        /// <summary>
        /// Run the step for the recipe.
        /// <remarks>The step type is already determined</remarks>
        /// </summary>
        public Pipe<R> Step([AllowNull] string title, [DisallowNull] Func<T, R> step) => Runner.RunPipe(_pipe, _step, title, step);

        /// <summary>
        /// Run the step for the recipe.
        /// <remarks>The step type is already determined</remarks>
        /// </summary>
        public Pipe<R> Step([AllowNull] string title, [DisallowNull] Func<T, Task<R>> step) => Runner.RunPipe(_pipe, _step, title, step);

        /// <summary>
        /// Run the step for the recipe.
        /// <remarks>The step type is already determined</remarks>
        /// </summary>
        public Pipe<R> Step([AllowNull] string title, [DisallowNull] Func<R> step) => Runner.RunPipe(_pipe, _step, title, step);

        /// <summary>
        /// Run the step for the recipe.
        /// <remarks>The step type is already determined</remarks>
        /// </summary>
        public Pipe<R> Step([AllowNull] string title, [DisallowNull] Func<Task<R>> step) => Runner.RunPipe(_pipe, _step, title, step);

        /// <summary>
        /// Run the step for the recipe.
        /// <remarks>The step type is already determined</remarks>
        /// </summary>
        public Pipe<T> Step([AllowNull] string title, [DisallowNull] Func<T, Task> step) => Runner.RunPipe(_pipe, _step, title, step);

        /// <summary>
        /// Run the step for the recipe.
        /// <remarks>The step type is already determined</remarks>
        /// </summary>
        public Pipe<T> Step([AllowNull] string title, [DisallowNull] Func<Task> step) => Runner.RunPipe(_pipe, _step, title, step);

        /// <summary>
        /// Run the step for the recipe.
        /// <remarks>The step type is already determined</remarks>
        /// </summary>
        public Pipe<T> Step([AllowNull] string title, [DisallowNull] Action<T> step) => Runner.RunPipe(_pipe, _step, title, step);

        /// <summary>
        /// Run the step for the recipe.
        /// <remarks>The step type is already determined</remarks>
        /// </summary>
        public Pipe<T> Step([AllowNull] string title, [DisallowNull] Action step) => Runner.RunPipe(_pipe, _step, title, step);
    }
}
