namespace BddPipe.FSharp

open BddPipe.FSharp.Model
open BddPipe.FSharp.StepResultExtensions
open BddPipe.FSharp.CtnExtensions
open System.Runtime.ExceptionServices

module Runner =

    let private toStepResult<'t, 'r> (tValue: Ctn<'t>, title: Title) (result: Result<'r, ExceptionDispatchInfo>) =
        match result with
            | Ok r ->
                CtnOk (tValue |> toCtn (r, title |> toStepOutcome Pass))
            | Error ex ->
                CtnError (tValue |> toCtn (ex, title |> toStepOutcome (toOutcome ex.SourceException)))

    let private toStepErrorState<'t>(title: Title) (ctnError: Ctn<ExceptionDispatchInfo>) : EitherCtn<'t> =
        CtnError (ctnError |> toCtn(ctnError.Content, title |> toStepOutcome NotRun))

    let private tryRun<'t> (fn: unit -> 't) : Result<'t, ExceptionDispatchInfo> =
        try
            let result = fn()
            Ok result
        with | ex ->
            Error (ExceptionDispatchInfo.Capture(ex))

    let private processStep<'t,'r> (title: Title, source: EitherCtn<'t>, stepFn: 't -> 'r) : EitherCtn<'r> =
        match source with
            | CtnOk tValue ->
                tryRun (fun () -> stepFn tValue.Content)
                |> toStepResult (tValue, title)
            | CtnError err -> err |> toStepErrorState<'r> title
