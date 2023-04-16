namespace BddPipe.FSharp.UnitTests

open NUnit.Framework
open BddPipe.FSharp.StringExtensions

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
        Assert.AreEqual(DefaultExpectation, result)

    [<Test>]
    let WithPrefix_AlreadyHasPrefix_PrefixNotAdded() =
        let result = Some DefaultExpectation |> withPrefix DefaultPrefix
        Assert.AreEqual(DefaultExpectation, result)

    [<Test>]
    let WithPrefix_AlreadyHasPrefixCaseLower_PrefixNotAdded() =
        let result = Some "given something" |> withPrefix DefaultPrefix
        Assert.AreEqual("given something", result)

    [<Test>]
    let WithPrefix_AlreadyHasPrefixCaseUpper_PrefixNotAdded() =
        let result = Some "GIVEN something" |> withPrefix DefaultPrefix
        Assert.AreEqual("GIVEN something", result)

    [<Test>]
    let WithPrefix_AlreadyHasPrefixCaseMixed_PrefixNotAdded() =
        let result = Some "GiVeN something" |> withPrefix DefaultPrefix
        Assert.AreEqual("GiVeN something", result)

    [<Test>]
    let WithPrefix_None_PrefixReturned() =
        let result = None |> withPrefix DefaultPrefix
        Assert.AreEqual(DefaultPrefix, result)

    [<Test>]
    let WithPrefix_Empty_PrefixReturned() =
        let result = Some "" |> withPrefix DefaultPrefix
        Assert.AreEqual(DefaultPrefix, result)

    [<Test>]
    let WithPrefix_Whitespace_PrefixReturned() =
        let result = Some " " |> withPrefix DefaultPrefix
        Assert.AreEqual(DefaultPrefix, result)
