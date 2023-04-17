namespace BddPipe.FSharp

open BddPipe.FSharp.Model
open BddPipe.FSharp.StringExtensions
open BddPipe.FSharp.StepResultExtensions

module internal CtnExtensions =

    let map<'t, 'r> (map: 't -> 'r) (ctn: Ctn<'t>) =
        { ScenarioTitle = ctn.ScenarioTitle
          StepOutcomes = ctn.StepOutcomes
          Content = map(ctn.Content) }

    let mapAsync<'t, 'r>(map: 't -> Async<'r>) (ctn: Ctn<'t>) =
        async {
            let! content = map(ctn.Content)
            return { ScenarioTitle = ctn.ScenarioTitle
                     StepOutcomes = ctn.StepOutcomes
                     Content = content }
        }

    let toCtn<'t, 'r> (newContent: 'r, stepOutcome) (ctn: Ctn<'t>) =
        let outcomes = ctn.StepOutcomes |> List.append [stepOutcome]
        { ScenarioTitle = ctn.ScenarioTitle
          StepOutcomes = outcomes
          Content = newContent }

    let toResult<'t> (ctn: Ctn<'t>) =
        { Title = ctn.ScenarioTitle
          Description = ctn.ScenarioTitle |> withPrefix "Scenario:"
          StepResults = ctn.StepOutcomes |> toResults ctn.ScenarioTitle.IsSome }
