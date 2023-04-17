namespace BddPipe.FSharp.Model

open System.Runtime.ExceptionServices

type internal EitherCtn<'t> =
| CtnOk of Ctn<'t>
| CtnError of Ctn<ExceptionDispatchInfo>
