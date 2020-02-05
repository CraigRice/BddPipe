using System;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    /// <summary>
    /// BddPipe scenario runner
    /// </summary>
    public static partial class Runner
    {
        private static Pipe<R> RunStep<T, R>(this Pipe<T> pipe, Some<Title> title, Func<T, Task<R>> step) =>
            RunStep(pipe, title, tValue =>
                Task.Run(() => step(tValue))
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult()
                );

        private static Pipe<R> RunStep<T, R>(this Pipe<T> pipe, Some<Title> title, Func<T, R> step) =>
            pipe.BiBind(
                tValue =>
                    step.Apply(tValue.Content)
                    .TryRun()
                    .Match<Pipe<R>>(
                        r => tValue.ToCtn(r,  title.ToStepOutcome(Outcome.Pass)),
                        ex => tValue.ToCtn(ex, title.ToStepOutcome(new Some<Exception>(ex).ToOutcome()))),
                err => 
                    err.ToCtn(err.Content, title.ToStepOutcome(Outcome.NotRun))
            );

        /// <summary>
        /// The last call to evaluate the result of calls made.
        /// </summary>
        /// <typeparam name="T">Type of the value represented when in a successful state.</typeparam>
        /// <param name="pipe">The state so far, containing the original exception or last returned result.</param>
        /// <param name="writeScenarioResult">Will output the result to console unless this optional handling is supplied.</param>
        /// <returns>Last returned type is returned from this function in the successful case, otherwise the exception previously raised is thrown.</returns>
        public static BddPipeResult<T> Run<T>(this Pipe<T> pipe, Action<ScenarioResult> writeScenarioResult = null)
        {
            var result = pipe.Match(
                ctnValue => ctnValue.ToResult(),
                ctnError => ctnError.ToResult()
            );

            var logResult = writeScenarioResult ?? WriteOutput.ApplyLast(Console.WriteLine);

            logResult(result);

            return pipe.Match(
                ctnValue => new BddPipeResult<T>(ctnValue.Content, result),
                ctnError =>
                {
                    var exception = ctnError.Content;

                    exception.TryPreserveStackTrace();
                    throw exception;
                });
        }

        private static Action<ScenarioResult, Action<string>> WriteOutput => (scenarioResult, writeLine) =>
        {
            if (!string.IsNullOrWhiteSpace(scenarioResult.Title))
            {
                writeLine(scenarioResult.Description);
            }

            foreach (var stepResult in scenarioResult.StepResults)
            {
                writeLine(stepResult.Description);
            }
        };

        /// <summary>
        /// Writes the scenario title and step results to console
        /// </summary>
        /// <param name="result">The scenario result gives a detailed output for each step outcome</param>
        /// <param name="writeLine">Optionally provide an implementation for each write line call</param>
        public static void WriteLogsToConsole(ScenarioResult result, Action<string> writeLine = null)
        {
            WriteOutput(result, writeLine ?? Console.WriteLine);
        }
    }
}
