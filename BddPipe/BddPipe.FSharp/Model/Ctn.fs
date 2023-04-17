namespace BddPipe.FSharp.Model

type internal Ctn<'t> = {
    ScenarioTitle: string option
    StepOutcomes: StepOutcome list
    Content: 't
}

