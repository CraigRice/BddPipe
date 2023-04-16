namespace BddPipe.FSharp.Model

type StepResult = {
    Step: Step
    Outcome: Outcome
    Title: string option
    Description: string }
