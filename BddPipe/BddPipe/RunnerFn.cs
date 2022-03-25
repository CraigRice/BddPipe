using System;
using System.Threading.Tasks;
using BddPipe.Model;
using static BddPipe.F;

namespace BddPipe
{
    public static partial class Runner
    {
        internal static Pipe<Unit> CreatePipe()
        {
            return new Pipe<Unit>(new Ctn<Unit>(new Unit(), None));
        }

        internal static Pipe<Scenario> CreatePipe(Scenario scenario)
        {
            if (scenario == null) { throw new ArgumentNullException(nameof(scenario)); }
            return new Pipe<Scenario>(new Ctn<Scenario>(scenario, scenario.Title));
        }

        internal static Pipe<R> RunPipe<T, R>(in Pipe<T> pipe, Step stepType, string title, Func<T, R> step)
        {
            if (step == null) { throw new ArgumentNullException(nameof(step)); }
            return RunStep(pipe, title.ToTitle(stepType), step);
        }

        internal static Pipe<R> RunPipe<T, R>(in Pipe<T> pipe, Step stepType, string title, Func<T, Task<R>> step)
        {
            if (step == null) { throw new ArgumentNullException(nameof(step)); }
            return RunStep(pipe, title.ToTitle(stepType), step);
        }

        internal static Pipe<R> RunPipe<T, R>(in Pipe<T> pipe, Step stepType, string title, Func<R> step)
        {
            if (step == null) { throw new ArgumentNullException(nameof(step)); }
            return RunStep(pipe, title.ToTitle(stepType), step.PipeFunc<T, R>());
        }

        internal static Pipe<R> RunPipe<T, R>(in Pipe<T> pipe, Step stepType, string title, Func<Task<R>> step)
        {
            if (step == null) { throw new ArgumentNullException(nameof(step)); }
            return RunStep(pipe, title.ToTitle(stepType), step.PipeFunc<T, R>());
        }

        internal static Pipe<T> RunPipe<T>(in Pipe<T> pipe, Step stepType, string title, Func<T, Task> step)
        {
            if (step == null) { throw new ArgumentNullException(nameof(step)); }
            return RunStep(pipe, title.ToTitle(stepType), step.PipeFunc());
        }

        internal static Pipe<T> RunPipe<T>(in Pipe<T> pipe, Step stepType, string title, Func<Task> step)
        {
            if (step == null) { throw new ArgumentNullException(nameof(step)); }
            return RunStep(pipe, title.ToTitle(stepType), step.PipeFunc<T>());
        }

        internal static Pipe<T> RunPipe<T>(in Pipe<T> pipe, Step stepType, string title, Action<T> step)
        {
            if (step == null) { throw new ArgumentNullException(nameof(step)); }
            return RunStep(pipe, title.ToTitle(stepType), step.PipeFunc());
        }

        internal static Pipe<T> RunPipe<T>(in Pipe<T> pipe, Step stepType, string title, Action step)
        {
            if (step == null) { throw new ArgumentNullException(nameof(step)); }
            return RunStep(pipe, title.ToTitle(stepType), step.PipeFunc<T>());
        }
    }
}
