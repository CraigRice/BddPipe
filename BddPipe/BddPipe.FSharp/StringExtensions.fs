namespace BddPipe.FSharp

open System

module internal StringExtensions =
    let private startsWithIgnoreCase (text: string) prefix =
        text.IndexOf(prefix, StringComparison.InvariantCultureIgnoreCase) = 0

    let withPrefix prefix text =
        match text with
            | Some txt ->
                if (startsWithIgnoreCase txt prefix) then
                    txt
                else
                    (sprintf "%s %s" prefix txt).TrimEnd()
            | None -> prefix
