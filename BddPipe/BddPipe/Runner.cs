using System;
using System.Threading.Tasks;

namespace BddPipe
{
    /// <summary>
    /// BddPipe scenario runner
    /// </summary>
    public static partial class Runner
    {
        private static Either<Ctn<Exception>, Ctn<R>> Pipe<T, R>(this Either<Ctn<Exception>, Ctn<T>> t, Some<Title> title, Func<T, Task<R>> step) =>
            Pipe(t, title, tValue =>
            {
                return Task.Run(() => step(tValue)).Result;
            });

        private static Either<Ctn<Exception>, Ctn<R>> Pipe<T, R>(this Either<Ctn<Exception>, Ctn<T>> t, Some<Title> title, Func<T, R> step)
        {
            return t.BiBind(
                tValue =>
                    step.Apply(tValue.Content)
                    .TryRun()
                    .Match<Either<Ctn<Exception>, Ctn<R>>>(
                        r => tValue.ToCtn(r,  title.ToStepOutcome(Outcome.Pass)),
                        ex => tValue.ToCtn(ex, title.ToStepOutcome(new Some<Exception>(ex).ToOutcome()))),
                err => 
                    err.ToCtn(err.Content, title.ToStepOutcome(Outcome.NotRun))
            );
        }

        /// <summary>
        /// The last call to evaluate the result of calls made.
        /// </summary>
        /// <typeparam name="T">Last returned type</typeparam>
        /// <param name="t">The state so far, containing the original exception or last returned result.</param>
        /// <param name="scenarioResult">Will output the result to console unless this optional handling is supplied.</param>
        /// <returns>Last returned type is returned from this function in the successful case, otherwise the exception previously raised is thrown.</returns>
        public static BddPipeResult<T> Run<T>(this Either<Ctn<Exception>, Ctn<T>> t, Action<ScenarioResult> scenarioResult = null)
        {
            var result = t.Match(
                r => r.ToResult(),
                e => e.ToResult()
            );

            var logger = scenarioResult ?? WriteOutput.ApplyLast(Console.WriteLine);

            logger(result);

            return t.Match(
                r => new BddPipeResult<T>(r.Content, result),
                ex => throw ex.Content);
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
