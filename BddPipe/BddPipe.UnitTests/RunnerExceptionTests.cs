using System;
using FluentAssertions;
using NUnit.Framework;
using static BddPipe.Runner;

namespace ExceptionTestNamespace
{
    public class Person
    {
        public string Name { get; set; }
    }

    public class TestClass
    {
        private void SetPropertyOfNullInstance()
        {
            Person person = null;
            person.Name = "test name";
        }

        public void CauseNullReferenceException()
        {
            SetPropertyOfNullInstance();
        }
    }
}

namespace BddPipe.UnitTests
{
    [TestFixture]
    public class RunnerExceptionTests
    {
        private const string ExpectedStacktraceStart = "   at ExceptionTestNamespace.TestClass.SetPropertyOfNullInstance()";
        private const string StacktraceSectionMarker = "--- End of stack trace from previous location ---";

        private static string GetStackTraceUntilPreviousLocationMarker(string stackTrace)
        {
            if (stackTrace == null)
            {
                return null;
            }

            var indexOfMarker = stackTrace.IndexOf(StacktraceSectionMarker, StringComparison.InvariantCulture);
            return (indexOfMarker > -1)
                ? stackTrace.Substring(
                    0,
                    indexOfMarker + StacktraceSectionMarker.Length)
                : stackTrace;
        }

        [Test]
        public void Run_OnRethrowOfException_StackTraceStartsWithRaisedSource()
        {
            var pipeRaisingEx = Scenario("Test scenario")
                .Given(null, () => new {A = 5, B = 10})
                .And("a null reference exception is raised", () =>
                {
                    var testClass = new ExceptionTestNamespace.TestClass();
                    testClass.CauseNullReferenceException();
                });

            Action run = () => pipeRaisingEx.Run(result => { /*mute*/ });
            run.Should().ThrowExactly<NullReferenceException>()
                .Which
                .StackTrace.Should().StartWith(ExpectedStacktraceStart);
        }

        [Test]
        public void Run_OnRethrowOfException_StackTraceRefersToThrownLocation()
        {
            var pipeRaisingEx = Scenario("Test scenario")
                    .Given(null, () => new {A = 5, B = 10})
                    .And("a null reference exception is raised", () =>
                    {
                        var testClass = new ExceptionTestNamespace.TestClass();
                        testClass.CauseNullReferenceException();
                    })
                    .When("the numbers are summed", args => new {Result = args.A + args.B})
                    .Then("sum should be as expected", arg => { arg.Result.Should().Be(15); });

            var raisedExceptionStackTrace = pipeRaisingEx.Match(
                ctnValue => throw new InconclusiveException("Expecting an exception was raised by a step"),
                ctnError =>
                {
                    try
                    {
                        ctnError.Content.Throw();
                    }
                    catch (Exception ex)
                    {
                        return ex.StackTrace;
                    }

                    throw new Exception("Could not return stacktrace");
                });

            var exceptionStack = GetStackTraceUntilPreviousLocationMarker(raisedExceptionStackTrace);

            Console.WriteLine("===============");
            Console.WriteLine($"Expected stack trace to start with this initial stack trace:");
            Console.WriteLine(exceptionStack);
            Console.WriteLine("===============");

            var exceptionStackLines = exceptionStack.Split(Environment.NewLine);
            exceptionStackLines.Length.Should().BeGreaterThan(2);

            exceptionStackLines[0].Should().StartWith(ExpectedStacktraceStart);
            exceptionStackLines[^1].Should().Be(StacktraceSectionMarker);

            Action run = () => pipeRaisingEx.Run(result => { /*mute*/ });
            run.Should().ThrowExactly<NullReferenceException>()
                .Which
                .StackTrace.Should().StartWith(exceptionStack);
        }
    }
}
