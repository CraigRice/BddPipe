using System;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace BddPipe.UnitTests
{
    [TestFixture]
    public class RunnerExtensionsTests
    {
        private const int DefaultInput = 5;
        private const string AnyStringArg = "any-arg";

        // Action<T> to Func<T, T>
        [Test]
        public void PipeFuncT_ActionT_RunsAndReturnsArg()
        {
            var result = 0;

            Action<int> action = i => { result = i; };
            Func<int, int> fn = action.PipeFunc<int>();

            var fnResult = fn(DefaultInput);
            fnResult.Should().Be(DefaultInput);
            result.Should().Be(DefaultInput);
        }

        // Action to Func<T, T>
        [Test]
        public void PipeFuncT_Action_RunsAndReturnsArg()
        {
            var result = 0;

            Action action = () => { result = DefaultInput; };
            Func<int, int> fn = action.PipeFunc<int>();

            var fnResult = fn(DefaultInput);
            fnResult.Should().Be(DefaultInput);
            result.Should().Be(DefaultInput);
        }

        // Func<Task> to Func<T, Task<T>>
        [Test]
        public async Task PipeFuncT_FuncTask_RunsAndReturnsArg()
        {
            var result = 0;

            Func<Task> funcTask = () =>
            {
                result = DefaultInput;
                return Task.CompletedTask;
            };

            Func<int, Task<int>> fn = funcTask.PipeFunc<int>();

            var fnResult = await fn(DefaultInput);
            fnResult.Should().Be(DefaultInput);
            result.Should().Be(DefaultInput);
        }

        // Func<T, Task> to Func<T, Task<T>>
        [Test]
        public async Task PipeFuncT_FuncTTask_RunsAndReturnsArg()
        {
            var result = 0;

            Func<int, Task> funcTask = i =>
            {
                result = i;
                return Task.CompletedTask;
            };

            Func<int, Task<int>> fn = funcTask.PipeFunc<int>();

            var fnResult = await fn(DefaultInput);
            fnResult.Should().Be(DefaultInput);
            result.Should().Be(DefaultInput);
        }

        // Func<R> to Func<T, R>
        [Test]
        public void PipeFuncTR_FuncR_RunsAndReturnsArg()
        {
            Func<int> funcTask = () => DefaultInput;

            Func<string, int> fn = funcTask.PipeFunc<string, int>();

            var fnResult = fn(AnyStringArg);
            fnResult.Should().Be(DefaultInput);
        }

        // Func<Task<R>> to Func<T, Task<R>>
        [Test]
        public async Task PipeFuncTR_FuncTaskR_RunsAndReturnsArg()
        {
            Func<Task<int>> funcTask = () => Task.FromResult(DefaultInput);

            Func<string, Task<int>> fn = funcTask.PipeFunc<string, int>();

            var fnResult = await fn(AnyStringArg);
            fnResult.Should().Be(DefaultInput);
        }
    }
}
