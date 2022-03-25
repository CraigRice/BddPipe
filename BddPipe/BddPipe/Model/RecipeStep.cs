using System.Diagnostics.CodeAnalysis;

namespace BddPipe.Model
{
    /// <summary>
    /// Call the step method on the recipe to return the next pipe instance.
    /// </summary>
    /// <typeparam name="T">The initial <see cref="Pipe{T}"/> type</typeparam>
    /// <typeparam name="R">The projected <see cref="Pipe{T}"/> type</typeparam>
    /// <param name="recipe"></param>
    /// <returns>The next <see cref="Pipe{T}"/> instance as returned from the recipe implementation.</returns>
    public delegate Pipe<R> RecipeStep<T, R>([DisallowNull] Recipe<T, R> recipe);

    /// <summary>
    /// Call the step method on the recipe to return the next pipe instance.
    /// </summary>
    /// <typeparam name="T">The initial <see cref="Pipe{T}"/> type which is not altered in the step implementation.</typeparam>
    /// <param name="recipe"></param>
    /// <returns>The next <see cref="Pipe{T}"/> instance as returned from the recipe implementation.</returns>
    public delegate Pipe<T> RecipeStep<T>([DisallowNull] Recipe<T, T> recipe);
}
