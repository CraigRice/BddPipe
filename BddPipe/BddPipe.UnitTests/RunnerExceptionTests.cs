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
                    .Then("sum should be as expected", arg => { arg.Result.Should().Be(15); })
            ;

            var raisedExceptionStackTrace = pipeRaisingEx.Match(
                ctnValue => throw new InconclusiveException("Expecting an exception was raised by a step"),
                ctnError => ctnError.Content.StackTrace
            );

            Console.WriteLine("===============");
            Console.WriteLine($"Expected stack trace to start with this initial stack trace: {raisedExceptionStackTrace}");
            Console.WriteLine("===============");

            Action run = () => pipeRaisingEx.Run(result => { });
            run.Should().ThrowExactly<NullReferenceException>()
                .Which
                .StackTrace.Should().StartWith(raisedExceptionStackTrace);
        }
    }
}
