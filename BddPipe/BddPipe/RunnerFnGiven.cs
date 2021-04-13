using System;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepGiven = Step.Given;

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<R> Given<R>(string title, Func<Unit, R> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<R> Given<R>(string title, Func<Unit, Task<R>> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<R> Given<R>(string title, Func<R> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<R> Given<R>(string title, Func<Task<R>> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<Unit> Given(string title, Func<Unit, Task> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<Unit> Given(string title, Func<Task> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<Unit> Given(string title, Action<Unit> step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<Unit> Given(string title, Action step) =>
            RunPipe(CreatePipe(), StepGiven, title, step);
    }
}
