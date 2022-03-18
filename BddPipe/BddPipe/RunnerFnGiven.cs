using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepGiven = Step.Given;

        /// <summary>
        /// <see cref="Step.Given"/>. Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<R> Given<R>([AllowNull] string title, [DisallowNull] Func<Unit, R> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// <see cref="Step.Given"/>. Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<R> Given<R>([AllowNull] string title, [DisallowNull] Func<Unit, Task<R>> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// <see cref="Step.Given"/>. Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<R> Given<R>([AllowNull] string title, [DisallowNull] Func<R> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// <see cref="Step.Given"/>. Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<R> Given<R>([AllowNull] string title, [DisallowNull] Func<Task<R>> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// <see cref="Step.Given"/>. Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<Unit> Given([AllowNull] string title, [DisallowNull] Func<Unit, Task> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// <see cref="Step.Given"/>. Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<Unit> Given([AllowNull] string title, [DisallowNull] Func<Task> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// <see cref="Step.Given"/>. Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<Unit> Given([AllowNull] string title, [DisallowNull] Action<Unit> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// <see cref="Step.Given"/>. Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<Unit> Given([AllowNull] string title, [DisallowNull] Action step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);
    }
}
