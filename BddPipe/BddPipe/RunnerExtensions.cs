using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("BddPipe.UnitTests")]

namespace BddPipe
{
    internal static class RunnerExtensions
    {
        public static Func<T, T> PipeFunc<T>(this Action<T> action) =>
            arg =>
            {
                action(arg);
                return arg;
            };

        public static Func<T, T> PipeFunc<T>(this Action action) =>
            arg =>
            {
                action();
                return arg;
            };

        public static Func<T, Task<T>> PipeFunc<T>(this Func<Task> fn) =>
            async arg =>
            {
                await fn();
                return arg;
            };

        public static Func<T, R> PipeFunc<T, R>(this Func<R> fn) =>
            arg => fn();

        public static Func<T, Task<T>> PipeFunc<T>(this Func<T, Task> fn) =>
            async arg =>
            {
                await fn(arg);
                return arg;
            };

        public static Func<Unit, Unit> PipeFunc(this Func<Task> fn) =>
            unit =>
            {
                Task.Run(fn).Wait();
                return unit;
            };

        public static Func<Unit, Unit> PipeFunc(this Action fn) =>
            unit =>
            {
                fn();
                return unit;
            };
    }
}
