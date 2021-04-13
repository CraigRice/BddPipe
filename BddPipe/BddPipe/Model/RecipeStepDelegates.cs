using System;
using System.Threading.Tasks;

namespace BddPipe.Model
{
    /// <summary>
    /// Call the step method on the recipe to return the next pipe instance.
    /// </summary>
    /// <typeparam name="T">The initial <see cref="Pipe{T}"/> type</typeparam>
    /// <typeparam name="R">The projected <see cref="Pipe{T}"/> type</typeparam>
    /// <param name="recipe"></param>
    /// <returns>The next <see cref="Pipe{T}"/> instance as returned from the recipe implementation.</returns>
    public delegate Pipe<R> RecipeStep<T, R>(Recipe<T, R> recipe);

    /// <summary>
    /// Call the step method on the recipe to return the next pipe instance.
    /// </summary>
    /// <typeparam name="T">The initial <see cref="Pipe{T}"/> type which is not altered in the step implementation.</typeparam>
    /// <param name="recipe"></param>
    /// <returns>The next <see cref="Pipe{T}"/> instance as returned from the recipe implementation.</returns>
    public delegate Pipe<T> RecipeStep<T>(Recipe<T, T> recipe);

    internal delegate Pipe<R> StepTR<T, R>(string title, Func<T, R> step);
    internal delegate Pipe<R> StepTTaskR<T, R>(string title, Func<T, Task<R>> step);
    internal delegate Pipe<R> StepR<R>(string title, Func<R> step);
    internal delegate Pipe<R> StepTaskR<R>(string title, Func<Task<R>> step);
    internal delegate Pipe<T> StepTTask<T, R>(string title, Func<T, Task> step);
    internal delegate Pipe<T> StepTask<T, R>(string title, Func<Task> step);
    internal delegate Pipe<T> StepActionT<T, R>(string title, Action<T> step);
    internal delegate Pipe<T> StepAction<T, R>(string title, Action step);
}
