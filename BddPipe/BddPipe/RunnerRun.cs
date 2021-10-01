﻿using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        /// <summary>
        /// The last call to evaluate the result of calls made.
        /// </summary>
        /// <typeparam name="T">Type of the value represented when in a successful state.</typeparam>
        /// <param name="pipe">The state so far, containing the original exception or last returned result.</param>
        /// <param name="writeScenarioResult">Will output the result to console unless this optional handling is supplied.</param>
        /// <returns>Last returned type is returned from this function in the successful case, otherwise the exception previously raised is thrown.</returns>
        public static BddPipeResult<T> Run<T>(this Pipe<T> pipe, Action<ScenarioResult> writeScenarioResult = null)
        {
            var container = pipe.ToContainer();
            return ProcessRun(container, writeScenarioResult);
        }

        /// <summary>
        /// The last call to evaluate the result of calls made.
        /// </summary>
        /// <typeparam name="T">Type of the value represented when in a successful state.</typeparam>
        /// <param name="pipe">The state so far, containing the original exception or last returned result.</param>
        /// <param name="writeScenarioResult">Will output the result to console unless this optional handling is supplied.</param>
        /// <returns>Last returned type is returned from this function in the successful case, otherwise the exception previously raised is thrown.</returns>
        public static async Task<BddPipeResult<T>> RunAsync<T>(this Pipe<T> pipe, Action<ScenarioResult> writeScenarioResult = null)
        {
            var container = await pipe.ToContainerAsync().ConfigureAwait(false);
            return ProcessRun(container, writeScenarioResult);
        }

        private static BddPipeResult<T> ProcessRun<T>(Either<Ctn<ExceptionDispatchInfo>, Ctn<T>> container, Action<ScenarioResult> writeScenarioResult = null)
        {
            var scenarioResult = container.ToScenarioResult();
            LogResult(scenarioResult, writeScenarioResult);
            var content = container.ToContent();
            return AsBddPipeResult(content, scenarioResult);
        }

        private static BddPipeResult<T> AsBddPipeResult<T>(Either<ExceptionDispatchInfo, T> content, Some<ScenarioResult> scenarioResult) =>
            content.Match(
                t => new BddPipeResult<T>(t, scenarioResult),
                exceptionDispatchInfo =>
                {
                    exceptionDispatchInfo.Throw();
                    throw new Exception("Could not throw exception dispatch info", exceptionDispatchInfo.SourceException);
                }
            );

        private static void LogResult(Some<ScenarioResult> scenarioResult, Action<ScenarioResult> writeScenarioResult = null)
        {
            var logResult = writeScenarioResult ?? WriteOutput.ApplyLast(Console.WriteLine);

            logResult(scenarioResult.Value);
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
            if (result == null) { throw new ArgumentNullException(nameof(result)); }

            WriteOutput(result, writeLine ?? Console.WriteLine);
        }
    }
}
