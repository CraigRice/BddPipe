using System;
using System.Threading.Tasks;
using BddPipe.Model;
using static BddPipe.F;

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
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), None)), title.ToTitle(StepGiven), step);

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<R> Given<R>(string title, Func<Unit, Task<R>> step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), None)), title.ToTitle(StepGiven), step);

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<R> Given<R>(string title, Func<R> step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), None)), title.ToTitle(StepGiven), unit => step());

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<R> Given<R>(string title, Func<Task<R>> step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), None)), title.ToTitle(StepGiven), step.PipeFunc<Unit, R>());

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<Unit> Given(string title, Func<Unit, Task> step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), None)), title.ToTitle(StepGiven), step.PipeFunc());

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<Unit> Given(string title, Func<Task> step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), None)), title.ToTitle(StepGiven), step.PipeFunc<Unit>());

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<Unit> Given(string title, Action<Unit> step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), None)), title.ToTitle(StepGiven), step.PipeFunc());

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<Unit> Given(string title, Action step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), None)), title.ToTitle(StepGiven), step.PipeFunc<Unit>());
    }
}
