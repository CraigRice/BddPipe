using System;
using BddPipe.Model;
using NUnit.Framework;
using static BddPipe.Runner;

namespace BddPipe.UnitTests
{
    public abstract class RunnerFnTestBase
    {
        protected const string StringValue = "string-value";
        protected const string ScenarioText = "scenario-text";
        protected const string GivenTitle = "given-text";
        protected const int GivenValue = 12;
        protected const string AnyStringArg = "any-arg";

        protected Exception GetTestException() =>
            new ApplicationException("test exception message");

        protected Exception GetInconclusiveException() =>
            new InconclusiveException("test inconclusive message");

        protected Pipe<int> RunnerWithGivenStep() =>
            Scenario(ScenarioText).Given(GivenTitle, () => GivenValue);
    }
}
