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
        public static Pipe<R> Given<R>(this Scenario scenario, string title, Func<Scenario, R> step) =>
            RunStep(new Pipe<Scenario>(new Ctn<Scenario>(scenario, scenario.Title)), title.ToTitle(StepGiven), step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<R> Given<R>(this Scenario scenario, string title, Func<Scenario, Task<R>> step) =>
            RunStep(new Pipe<Scenario>(new Ctn<Scenario>(scenario, scenario.Title)), title.ToTitle(StepGiven), step);

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<R> Given<R>(this Scenario scenario, string title, Func<R> step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), scenario.Title)), title.ToTitle(StepGiven), unit => step());

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<R> Given<R>(this Scenario scenario, string title, Func<Task<R>> step) =>
            RunStep(new Pipe<Unit>(new Ctn<Unit>(new Unit(), scenario.Title)), title.ToTitle(StepGiven), step.PipeFunc<Unit, R>());

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Scenario> Given(this Scenario scenario, string title, Func<Scenario, Task> step) =>
            RunStep(new Pipe<Scenario>(new Ctn<Scenario>(scenario, scenario.Title)), title.ToTitle(StepGiven), step.PipeFunc());

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Scenario> Given(this Scenario scenario, string title, Func<Task> step) =>
            RunStep(new Pipe<Scenario>(new Ctn<Scenario>(scenario, scenario.Title)), title.ToTitle(StepGiven), step.PipeFunc<Scenario>());

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Scenario> Given(this Scenario scenario, string title, Action<Scenario> step) =>
            RunStep(new Pipe<Scenario>(new Ctn<Scenario>(scenario, scenario.Title)), title.ToTitle(StepGiven), step.PipeFunc());

        /// <summary>
        /// Specify the Given step implementation following the Scenario
        /// </summary>
        public static Pipe<Scenario> Given(this Scenario scenario, string title, Action step) =>
            RunStep(new Pipe<Scenario>(new Ctn<Scenario>(scenario, scenario.Title)), title.ToTitle(StepGiven), step.PipeFunc<Scenario>());
    }
}
