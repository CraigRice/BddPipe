﻿using System;
using System.Threading.Tasks;
using BddPipe.Model;

namespace BddPipe
{
    public static partial class Runner
    {
        private const Step StepAnd = Step.And;

        public static Pipe<R> And<T, R>(this Pipe<T> t, string title, Func<T, R> step) =>
            RunStep(t, title.ToTitle(StepAnd), step);

        public static Pipe<R> And<T, R>(this Pipe<T> t, string title, Func<T, Task<R>> step) =>
            RunStep(t, title.ToTitle(StepAnd), step);

        public static Pipe<R> And<T, R>(this Pipe<T> t, string title, Func<R> step) =>
            RunStep(t, title.ToTitle(StepAnd), step.PipeFunc<T, R>());

        public static Pipe<R> And<T, R>(this Pipe<T> t, string title, Func<Task<R>> step) =>
            RunStep(t, title.ToTitle(StepAnd), step.PipeFunc<T, R>());

        public static Pipe<T> And<T>(this Pipe<T> t, string title, Func<T, Task> step) =>
            RunStep(t, title.ToTitle(StepAnd), step.PipeFunc());

        public static Pipe<T> And<T>(this Pipe<T> t, string title, Func<Task> step) =>
            RunStep(t, title.ToTitle(StepAnd), step.PipeFunc<T>());

        public static Pipe<T> And<T>(this Pipe<T> t, string title, Action<T> step) =>
            RunStep(t, title.ToTitle(StepAnd), step.PipeFunc());

        public static Pipe<T> And<T>(this Pipe<T> t, string title, Action step) =>
            RunStep(t, title.ToTitle(StepAnd), step.PipeFunc<T>());
    }
}
