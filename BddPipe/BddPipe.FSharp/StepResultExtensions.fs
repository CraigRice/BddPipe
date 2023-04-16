namespace BddPipe.FSharp

open System
open BddPipe.FSharp.Model
open BddPipe.FSharp.StringExtensions

module internal StepResultExtensions =

    [<Literal>]
    let private DefaultIndentSize = 2

    let toOutcometext outcome =
        match outcome with
            | Pass -> "Passed"
            | Fail -> "Failed"
            | Inconclusive -> "Inconclusive"
            | NotRun -> "not run"

    let withIndendation step hasScenario prefixedStep =
        let toIndentSizeStepAndBut step =
            match step with
            | And | But -> DefaultIndentSize
            | _ -> 0

        let toIndentSizeStep hasScenario =
            if hasScenario then DefaultIndentSize else 0

        let indentSize = toIndentSizeStep hasScenario + toIndentSizeStepAndBut step
        let indent = new string(' ', indentSize)
        sprintf "%s%s" indent prefixedStep

    let withOutcomeDescribed outcome prefixedStep =
        sprintf "%s [%s]" prefixedStep (toOutcometext outcome)

    let private toPrefix (step: Step) =
        step.ToString()

    let toDescription (stepResult: StepResult) =
        None
            |> withPrefix (toPrefix stepResult.Step)
            |> withIndendation stepResult.Step false
            |> withOutcomeDescribed stepResult.Outcome

    let private toStepOutcomeDescription hasScenario (stepOutcome: StepOutcome) =
        stepOutcome.Text
            |> withPrefix (toPrefix stepOutcome.Step)
            |> withIndendation stepOutcome.Step hasScenario
            |> withOutcomeDescribed stepOutcome.Outcome

    let toResults (outcomes: StepOutcome list) hasScenario =
        outcomes
            |> Seq.map (fun o -> {
                Step = o.Step
                Outcome = o.Outcome
                Title = o.Text
                Description = o |> toStepOutcomeDescription hasScenario })
            |> Seq.toList

    let private toStepOutcomeOfOutcome (stepOutcome: StepOutcome) outcome = {
        Step = stepOutcome.Step
        Outcome = outcome
        Text = stepOutcome.Text
    }

    let withLatestStepOutcomeAs (outcomes: StepOutcome list) outcome =
        outcomes |> Seq.mapi (fun index stepOutcome ->
                    match index with
                        | i when i = outcomes.Length - 1 -> toStepOutcomeOfOutcome stepOutcome outcome
                        | _ -> stepOutcome
                    )
                 |> Seq.toList

    let toStepOutcome title outcome = {
        Step = title.Step
        Outcome = outcome
        Text = title.Text
    }

    let toTitle step text = {
        Step = step
        Text = text
    }

    let private exceptionTypeNameIsInconclusive exceptionTypeName =
        String.Equals(exceptionTypeName, "InconclusiveException", StringComparison.InvariantCultureIgnoreCase) ||
        String.Equals(exceptionTypeName, "AssertInconclusiveException", StringComparison.InvariantCultureIgnoreCase) ||
        (exceptionTypeName.StartsWith("Skippable") && exceptionTypeName.EndsWith("Exception"))

    let private exceptionIsInconclusive (ex: Exception) =
        exceptionTypeNameIsInconclusive (ex.GetType().Name)

    let toOutcome (ex: Exception) =
        match exceptionIsInconclusive ex with
            | true -> Inconclusive
            | false -> Fail
