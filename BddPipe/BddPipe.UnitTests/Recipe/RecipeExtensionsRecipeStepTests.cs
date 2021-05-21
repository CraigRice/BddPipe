using System;
using BddPipe.Model;
using BddPipe.Recipe;
using NUnit.Framework;
using static BddPipe.Recipe.RecipeExtensions;
using static BddPipe.Runner;

namespace BddPipe.UnitTests.Recipe
{
   // public class ScenarioInfo { }

    public static class Usage
    {
        public static RecipeStep<Unit, int> Hmmm()
        {
            return r => r.Map(u => 5).Step("hey", theInt => theInt + 1);
        }

        public static RecipeStep<int> Increment()
        {
            return r => r.Step("hey", theInt => theInt + 1);
        }

        public static RecipeStep<Unit, int> SeedThen(RecipeStep<int> recipeStep)
        {
            return r => recipeStep(r.Map(unit => 0));
        }


        public static void Go()
        {

            GivenRecipe(SeedThen(Increment()))
                .Run();

            GivenRecipe(Hmmm())
                .Run();

            GivenRecipe<int>(x => x.Map(u => 5).Step("hey", theInt => theInt + 1))
                .Run();

            GivenRecipe(SeedScenarioInfoU(SomeScenarioInfoStep))
                .Run();

            Scenario()
                .GivenRecipe(SeedScenarioInfo(SomeScenarioInfoStep))
                .AndRecipe(SomeScenarioInfoStep)
                .Run();

            Scenario()
                .Given("", () => { })
                .AndRecipe(StepUnitToInt())
                .AndRecipe(StepUnitToInt2())
                .AndRecipe(recipe => recipe.Step("", v => v.ToString()))
                .AndRecipe(pipe =>
                {
                    return pipe.And("", x => x.ToString());
                })
                .Run();
        }

        public static Pipe<ScenarioInfo> GivenScenarioRecipe(Recipe<Unit, ScenarioInfo> recipeStep, RecipeStep<ScenarioInfo> stepImpl)
        {
            return default;
            // return stepImpl(recipeStep.Map(u => new ScenarioInfo()));
        }

        public static RecipeStep<Scenario, int> StepUnitToInt()
        {
            return r => r.Step("", u => 5);
        }

        public static RecipeStep<int> StepUnitToInt2()
        {
            return r => r.Step("", theInt => 5);
        }

        public static RecipeStep<Unit, R> StartOff<R>(RecipeStep<ScenarioInfo, R> initialStep)
        {
            return step => initialStep(step.Map(u => new ScenarioInfo("a", "b")));
        }

        public static RecipeStep<Unit, ScenarioInfo> SeedScenarioInfoU(RecipeStep<ScenarioInfo> initialStep)
        {
            return step => initialStep(step.Map(u => new ScenarioInfo("a", "b")));
        }

        public static RecipeStep<Scenario, ScenarioInfo> SeedScenarioInfo(RecipeStep<ScenarioInfo> initialStep)
        {
            return step => initialStep(step.Map(u => new ScenarioInfo("a", "b")));
        }
        /*


                public static RecipeStep<TAnyOther, R> StartOff<TAnyOther, R>(RecipeStep<ScenarioInfo, R> initialStep)
                {
                    return step => initialStep(step.Map(u => new ScenarioInfo()));
                }


                */

        public static RecipeStep<ScenarioInfo> SomeScenarioInfoStep => recipe =>
            recipe.Step("some step", scenarioInfo =>
            {
                // do some things
                // update scenarioInfo
                return scenarioInfo;
            });
    }

    public class StepProxyGiven
    {

    }

    public class StepProxyAnd
    {
        // for each call... give me a pipe and ill call And on it.
        public Pipe<R> Call<T, R>(Pipe<T> pipe, string title, Func<T, R> step)
        {
            return pipe.And(title, step);
        }
    }

    // todo: pipe match arg null tests.
    // step arg null test on each method.
    // lightly test recipe ext. and adjust naming
    // finish testing.
}
