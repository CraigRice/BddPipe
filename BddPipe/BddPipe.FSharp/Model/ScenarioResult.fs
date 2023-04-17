namespace BddPipe.FSharp.Model

open BddPipe.FSharp.Model

type ScenarioResult = {
    Title: string option
    Description: string
    StepResults: StepResult list
}
