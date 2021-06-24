using System;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<R> Given<R>(this Pipe<Scenario> scenario, string title, Func<Scenario, R> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<R> Given<R>(this Pipe<Scenario> scenario, string title, Func<Scenario, Task<R>> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<R> Given<R>(this Pipe<Scenario> scenario, string title, Func<R> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<R> Given<R>(this Pipe<Scenario> scenario, string title, Func<Task<R>> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Scenario> Given(this Pipe<Scenario> scenario, string title, Func<Scenario, Task> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Scenario> Given(this Pipe<Scenario> scenario, string title, Func<Task> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Scenario> Given(this Pipe<Scenario> scenario, string title, Action<Scenario> step) =>
            RunPipe(scenario, StepGiven, title, step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Scenario> Given(this Pipe<Scenario> scenario, string title, Action step) =>
            RunPipe(scenario, StepGiven, title, step);
    }
}
