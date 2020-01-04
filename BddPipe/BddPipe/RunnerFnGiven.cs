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
        public static Pipe<R> Given<R>(string title, Func<R> step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), None)), title.ToTitle(StepGiven), unit => step());

        /// <summary>
        /// Starts the runner with a 'Given' step and no scenario
        /// <remarks>Start with a call to Scenario(...) if you wish to have a scenario in the output</remarks>
        /// </summary>
        public static Pipe<Unit> Given(string title, Action step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), None)), title.ToTitle(StepGiven), step.PipeFunc<Unit>());

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
        public static Pipe<Unit> Given(string title, Func<Task> step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), None)), title.ToTitle(StepGiven), step.PipeFunc<Unit>());

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<R> Given<R>(this Scenario scenario, string title, Func<R> step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), scenario.Title)), title.ToTitle(StepGiven), unit => step());

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Unit> Given(this Scenario scenario, string title, Action step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), scenario.Title)), title.ToTitle(StepGiven), step.PipeFunc<Unit>());

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<R> Given<R>(this Scenario scenario, string title, Func<Task<R>> step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), scenario.Title)), title.ToTitle(StepGiven), step.PipeFunc<Unit, R>());

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Unit> Given(this Scenario scenario, string title, Func<Task> step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), scenario.Title)), title.ToTitle(StepGiven), step.PipeFunc<Unit>());
    }
}
