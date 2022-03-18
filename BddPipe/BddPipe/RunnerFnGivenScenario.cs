using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<R> Given<R>(this Pipe<Scenario> scenario, [AllowNull] string title, [DisallowNull] Func<Scenario, R> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<R> Given<R>(this Pipe<Scenario> scenario, [AllowNull] string title, [DisallowNull] Func<Scenario, Task<R>> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<R> Given<R>(this Pipe<Scenario> scenario, [AllowNull] string title, [DisallowNull] Func<R> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<R> Given<R>(this Pipe<Scenario> scenario, [AllowNull] string title, [DisallowNull] Func<Task<R>> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Scenario> Given(this Pipe<Scenario> scenario, [AllowNull] string title, [DisallowNull] Func<Scenario, Task> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Scenario> Given(this Pipe<Scenario> scenario, [AllowNull] string title, [DisallowNull] Func<Task> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Scenario> Given(this Pipe<Scenario> scenario, [AllowNull] string title, [DisallowNull] Action<Scenario> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Scenario> Given(this Pipe<Scenario> scenario, [AllowNull] string title, [DisallowNull] Action step) =>
            RunPipe(scenario, StepGiven, title, step);
    }
}
