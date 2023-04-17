namespace BddPipe.FSharp.UnitTests

open NUnit.Framework
open BddPipe.FSharp.StringExtensions
open FsUnit

[<TestFixture>]
module StringExtensionsTests =

    [<Literal>]
    let DefaultValue = "test";

    [<Literal>]
    let DefaultPrefix = "Given";

    [<Literal>]
    let DefaultExpectation = "Given test";

    [<Test>]
    let WithPrefix_DoesNotHavePrefix_PrefixAdded() =
        let result = Some DefaultValue |> withPrefix DefaultPrefix
        result |> should equal DefaultExpectation

    [<Test>]
    let WithPrefix_AlreadyHasPrefix_PrefixNotAdded() =
        let result = Some DefaultExpectation |> withPrefix DefaultPrefix
        result |> should equal DefaultExpectation

    [<Test>]
    let WithPrefix_AlreadyHasPrefixCaseLower_PrefixNotAdded() =
        let result = Some "given something" |> withPrefix DefaultPrefix
        result |> should equal "given something"

    [<Test>]
    let WithPrefix_AlreadyHasPrefixCaseUpper_PrefixNotAdded() =
        let result = Some "GIVEN something" |> withPrefix DefaultPrefix
        result |> should equal "GIVEN something"

    [<Test>]
    let WithPrefix_AlreadyHasPrefixCaseMixed_PrefixNotAdded() =
        let result = Some "GiVeN something" |> withPrefix DefaultPrefix
        result |> should equal "GiVeN something"

    [<Test>]
    let WithPrefix_None_PrefixReturned() =
        let result = None |> withPrefix DefaultPrefix
        result |> should equal DefaultPrefix

    [<Test>]
    let WithPrefix_Empty_PrefixReturned() =
        let result = Some "" |> withPrefix DefaultPrefix
        result |> should equal DefaultPrefix

    [<Test>]
    let WithPrefix_Whitespace_PrefixReturned() =
        let result = Some " " |> withPrefix DefaultPrefix
        result |> should equal DefaultPrefix
