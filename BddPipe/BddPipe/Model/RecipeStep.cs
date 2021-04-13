using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace BddPipe.Model
{
    /// <summary>
    /// Recipe defines a proxy for Step calls against the original <see cref="Pipe{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="R"></typeparam>
    public sealed class Recipe<T, R>
    {
        private readonly Pipe<T> _pipe;
        private readonly Step _step;
        private readonly StepTR<T, R> _stepTr;
        private readonly StepTTaskR<T, R> _stepTTaskR;
        private readonly StepR<R> _stepR;
        private readonly StepTaskR<R> _stepTaskR;
        private readonly StepTTask<T, T> _stepTTask;
        private readonly StepTask<T, T> _stepTask;
        private readonly StepActionT<T, T> _stepActionT;
        private readonly StepAction<T, T> _stepAction;

        internal Recipe(Pipe<T> pipe, Step step)
        {
            _pipe = pipe;
            _step = step;

            switch (_step)
            {
                case BddPipe.Step.Given:
                    _stepTr = (title, stepImpl) => Runner.RunPipe(_pipe, step, title, stepImpl);
                    _stepTTaskR = (title, stepImpl) => Runner.RunPipe(_pipe, step, title, stepImpl);
                    _stepR = (title, stepImpl) => Runner.RunPipe(_pipe, step, title, stepImpl);
                    _stepTaskR = (title, stepImpl) => Runner.RunPipe(_pipe, step, title, stepImpl);
                    _stepTTask = (title, stepImpl) => Runner.RunPipe(_pipe, step, title, stepImpl);
                    _stepTask = (title, stepImpl) => Runner.RunPipe(_pipe, step, title, stepImpl);
                    _stepActionT = (title, stepImpl) => Runner.RunPipe(_pipe, step, title, stepImpl);
                    _stepAction = (title, stepImpl) => Runner.RunPipe(_pipe, step, title, stepImpl);
                    break;
                case BddPipe.Step.And:
                    _stepTr = (title, stepImpl) => _pipe.And(title, stepImpl);
                    _stepTTaskR = (title, stepImpl) => _pipe.And(title, stepImpl);
                    _stepR = (title, stepImpl) => _pipe.And(title, stepImpl);
                    _stepTaskR = (title, stepImpl) => _pipe.And(title, stepImpl);
                    _stepTTask = (title, stepImpl) => _pipe.And(title, stepImpl);
                    _stepTask = (title, stepImpl) => _pipe.And(title, stepImpl);
                    _stepActionT = (title, stepImpl) => _pipe.And(title, stepImpl);
                    _stepAction = (title, stepImpl) => _pipe.And(title, stepImpl);
                    break;
                case BddPipe.Step.Then:
                    _stepTr = (title, stepImpl) => _pipe.Then(title, stepImpl);
                    _stepTTaskR = (title, stepImpl) => _pipe.Then(title, stepImpl);
                    _stepR = (title, stepImpl) => _pipe.Then(title, stepImpl);
                    _stepTaskR = (title, stepImpl) => _pipe.Then(title, stepImpl);
                    _stepTTask = (title, stepImpl) => _pipe.Then(title, stepImpl);
                    _stepTask = (title, stepImpl) => _pipe.Then(title, stepImpl);
                    _stepActionT = (title, stepImpl) => _pipe.Then(title, stepImpl);
                    _stepAction = (title, stepImpl) => _pipe.Then(title, stepImpl);
                    break;
                default:
                    throw new InvalidEnumArgumentException($"Step type '{step}' can not be used with recipe step");
            }
        }

        /// <summary>
        /// Projects from one value to another.
        /// <remarks>A failure to map will impact the current step as if this happened in the step itself.</remarks>
        /// </summary>
        /// <typeparam name="T2">Type of the resulting value</typeparam>
        /// <param name="map">A function to map the current value to its new value.</param>
        /// <returns>A new <see cref="Recipe{T2, R}"/> instance of the destination type</returns>
        public Recipe<T2, R> Map<T2>(Func<T, T2> map)
        {
            // todo: test a map failure before the first step... all steps should be in fail state
            return new Recipe<T2, R>(_pipe.Map(map), _step);
        }

        public Pipe<R> Step(string title, Func<T, R> step) => _stepTr(title, step);
        public Pipe<R> Step(string title, Func<T, Task<R>> step) => _stepTTaskR(title, step);
        public Pipe<R> Step(string title, Func<R> step) => _stepR(title, step);
        public Pipe<R> Step(string title, Func<Task<R>> step) => _stepTaskR(title, step);
        public Pipe<T> Step(string title, Func<T, Task> step) => _stepTTask(title, step);
        public Pipe<T> Step(string title, Func<Task> step) => _stepTask(title, step);
        public Pipe<T> Step(string title, Action<T> step) => _stepActionT(title, step);
        public Pipe<T> Step(string title, Action step) => _stepAction(title, step);
    }
}
