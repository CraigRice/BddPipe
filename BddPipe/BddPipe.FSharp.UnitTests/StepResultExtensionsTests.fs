namespace BddPipe.FSharp.UnitTests

open NUnit.Framework
open BddPipe.FSharp.StepResultExtensions
open BddPipe.FSharp.Model
open FsUnit

[<TestFixture>]
module StepResultExtensionsTests =

    [<Test>]
    let WithLatestStepOutcomeAsFail_StepOutcomesEmpty_ReturnsEmpty() =
        let outcomes = []
        let result = outcomes |> withLatestStepOutcomeAs Fail
        result |> should be Empty

    [<Test>]
    let WithLatestStepOutcomeAsFail_StepOutcomesSingleFail_ReturnsSingleFail() =
        let outcomes = [{Step = Given; Outcome = Fail; Text = Some "step-text"}]
        let result = outcomes |> withLatestStepOutcomeAs Fail
        result |> should not' (equal null)
        result.Length |> should equal 1
        result.[0].Step |> should equal Given
        result.[0].Outcome |> should equal Fail
        result.[0].Text |> should equal (Some "step-text")

    [<Test>]
    let WithLatestStepOutcomeAsFail_TextIsNone_ReturnsTextAsNone() =
        let outcomes = [{Step = Given; Outcome = Fail; Text = None}]
        let result = outcomes |> withLatestStepOutcomeAs Fail
        result |> should not' (equal null)
        result.Length |> should equal 1
        result.[0].Step |> should equal Given
        result.[0].Outcome |> should equal Fail
        result.[0].Text |> should equal None

    [<Test>]
    let WithLatestStepOutcomeAsFail_StepOutcomesLastOfTwoIsFail_ReturnsLastOfTwoFail() =
        let outcomes = [
            {Step = Given; Outcome = Pass; Text = Some "given-text"}
            {Step = When; Outcome = Fail; Text = Some "when-text"}
        ]
        let result = outcomes |> withLatestStepOutcomeAs Fail
        result |> should not' (equal null)
        result.Length |> should equal 2
        result.[0].Step |> should equal Given
        result.[0].Outcome |> should equal Pass
        result.[0].Text |> should equal (Some "given-text")
        result.[1].Step |> should equal When
        result.[1].Outcome |> should equal Fail
        result.[1].Text |> should equal (Some "when-text")

    [<Test>]
    let WithLatestStepOutcomeAsFail_StepOutcomesSinglePass_ReturnsSingleFail() =
        let outcomes = [{Step = Given; Outcome = Pass; Text = Some "step-text"}]
        let result = outcomes |> withLatestStepOutcomeAs Fail
        result |> should not' (equal null)
        result.Length |> should equal 1
        result.[0].Step |> should equal Given
        result.[0].Outcome |> should equal Fail
        result.[0].Text |> should equal (Some "step-text")

    [<Test>]
    let WithLatestStepOutcomeAsFail_StepOutcomesLastOfTwoIsPass_ReturnsLastOfTwoFail() =
        let outcomes = [
            {Step = Given; Outcome = Pass; Text = Some "given-text"}
            {Step = When; Outcome = Pass; Text = Some "when-text"}
        ]
        let result = outcomes |> withLatestStepOutcomeAs Fail
        result |> should not' (equal null)
        result.Length |> should equal 2
        result.[0].Step |> should equal Given
        result.[0].Outcome |> should equal Pass
        result.[0].Text |> should equal (Some "given-text")
        result.[1].Step |> should equal When
        result.[1].Outcome |> should equal Fail
        result.[1].Text |> should equal (Some "when-text")

    [<Test>]
    let ToOutcome_Exception_Fail() =
        let ex = System.Exception "some error"
        ex |> toOutcome |> should equal Fail

    [<Test>]
    let ToOutcome_FormatException_Fail() =
        let ex = System.FormatException "some error"
        ex |> toOutcome |> should equal Fail

    [<Test>]
    let ToOutcome_InconclusiveException_Inconclusive() =
        let ex = InconclusiveException "some error"
        ex |> toOutcome |> should equal Inconclusive

    [<Test>]
    let ToTitle_NoneTitleString_ReturnsCorrectTitle() =
        let result = None |> toTitle And
        result.Step |> should equal And
        result.Text |> should equal None

    [<Test>]
    let ToTitle_TitleString_ReturnsCorrectTitle() =
        let titleText = Some "title text"
        let result = titleText |> toTitle And
        result.Step |> should equal And
        result.Text |> should equal titleText


    [<Test>]
    let ToStepOutcome_TitleAndSuppliedOutcome_ReturnsMappedStepOutcome() =
        let titleText = Some "title text"
        let title = { Step = And; Text = titleText }
        let stepOutcome = title |> toStepOutcome Inconclusive
        stepOutcome.Step |> should equal And
        stepOutcome.Outcome |> should equal Inconclusive
        stepOutcome.Text |> should equal titleText

    [<Test>]
    let ToStepOutcome_NoneTitleAndSuppliedOutcome_ReturnsMappedStepOutcome() =
        let titleText = None
        let title = { Step = And; Text = titleText }
        let stepOutcome = title |> toStepOutcome Inconclusive
        stepOutcome.Step |> should equal And
        stepOutcome.Outcome |> should equal Inconclusive
        stepOutcome.Text |> should equal titleText

    [<Test>]
    let ToResults_Empty_ReturnsEmpty() =
        let stepResults = []
        let results = stepResults |> toResults false
        results |> should be Empty

    [<Test>]
    let ToResults_TitleWithValue_MapsTitle() =
        let title = Some "step title"
        let stepResults = [{Step = But; Outcome = Pass; Text = title}]
        let results = stepResults |> toResults false
        results.Length |> should equal 1
        results.[0].Title |> should equal title

    [<Test>]
    let ToResults_TitleWithNone_MapsTitle() =
        let title = None
        let stepResults = [{Step = But; Outcome = Pass; Text = title}]
        let results = stepResults |> toResults false
        results.Length |> should equal 1
        results.[0].Title |> should equal title


    type StepResultSource = {
        HasScenario: bool
        Step: Step
        Outcome: Outcome
        StepTitle: string
        Expected: string
    }

    [<Literal>]
    let private stepTitle = "step title"

    let stepResultsSource() =
        [
            { HasScenario = false; Step = Given; Outcome = Fail; StepTitle = stepTitle; Expected = "Given step title [Failed]" }
            { HasScenario = false; Step = Given; Outcome = Inconclusive; StepTitle = stepTitle; Expected = "Given step title [Inconclusive]" }
            { HasScenario = false; Step = Given; Outcome = NotRun; StepTitle = stepTitle; Expected = "Given step title [not run]" }
            { HasScenario = false; Step = Given; Outcome = Pass; StepTitle = stepTitle; Expected = "Given step title [Passed]" }

            { HasScenario = false; Step = When; Outcome = Fail; StepTitle = stepTitle; Expected = "When step title [Failed]" }
            { HasScenario = false; Step = When; Outcome = Inconclusive; StepTitle = stepTitle; Expected = "When step title [Inconclusive]" }
            { HasScenario = false; Step = When; Outcome = NotRun; StepTitle = stepTitle; Expected = "When step title [not run]" }
            { HasScenario = false; Step = When; Outcome = Pass; StepTitle = stepTitle; Expected = "When step title [Passed]" }

            { HasScenario = false; Step = Then; Outcome = Fail; StepTitle = stepTitle; Expected = "Then step title [Failed]" }
            { HasScenario = false; Step = Then; Outcome = Inconclusive; StepTitle = stepTitle; Expected = "Then step title [Inconclusive]" }
            { HasScenario = false; Step = Then; Outcome = NotRun; StepTitle = stepTitle; Expected = "Then step title [not run]" }
            { HasScenario = false; Step = Then; Outcome = Pass; StepTitle = stepTitle; Expected = "Then step title [Passed]" }

            { HasScenario = false; Step = And; Outcome = Fail; StepTitle = stepTitle; Expected = "  And step title [Failed]" }
            { HasScenario = false; Step = And; Outcome = Inconclusive; StepTitle = stepTitle; Expected = "  And step title [Inconclusive]" }
            { HasScenario = false; Step = And; Outcome = NotRun; StepTitle = stepTitle; Expected = "  And step title [not run]" }
            { HasScenario = false; Step = And; Outcome = Pass; StepTitle = stepTitle; Expected = "  And step title [Passed]" }

            { HasScenario = false; Step = But; Outcome = Fail; StepTitle = stepTitle; Expected = "  But step title [Failed]" }
            { HasScenario = false; Step = But; Outcome = Inconclusive; StepTitle = stepTitle; Expected = "  But step title [Inconclusive]" }
            { HasScenario = false; Step = But; Outcome = NotRun; StepTitle = stepTitle; Expected = "  But step title [not run]" }
            { HasScenario = false; Step = But; Outcome = Pass; StepTitle = stepTitle; Expected = "  But step title [Passed]" }


            { HasScenario = true; Step = Given; Outcome = Fail; StepTitle = stepTitle; Expected = "  Given step title [Failed]" }
            { HasScenario = true; Step = Given; Outcome = Inconclusive; StepTitle = stepTitle; Expected = "  Given step title [Inconclusive]" }
            { HasScenario = true; Step = Given; Outcome = NotRun; StepTitle = stepTitle; Expected = "  Given step title [not run]" }
            { HasScenario = true; Step = Given; Outcome = Pass; StepTitle = stepTitle; Expected = "  Given step title [Passed]" }

            { HasScenario = true; Step = When; Outcome = Fail; StepTitle = stepTitle; Expected = "  When step title [Failed]" }
            { HasScenario = true; Step = When; Outcome = Inconclusive; StepTitle = stepTitle; Expected = "  When step title [Inconclusive]" }
            { HasScenario = true; Step = When; Outcome = NotRun; StepTitle = stepTitle; Expected = "  When step title [not run]" }
            { HasScenario = true; Step = When; Outcome = Pass; StepTitle = stepTitle; Expected = "  When step title [Passed]" }

            { HasScenario = true; Step = Then; Outcome = Fail; StepTitle = stepTitle; Expected = "  Then step title [Failed]" }
            { HasScenario = true; Step = Then; Outcome = Inconclusive; StepTitle = stepTitle; Expected = "  Then step title [Inconclusive]" }
            { HasScenario = true; Step = Then; Outcome = NotRun; StepTitle = stepTitle; Expected = "  Then step title [not run]" }
            { HasScenario = true; Step = Then; Outcome = Pass; StepTitle = stepTitle; Expected = "  Then step title [Passed]" }

            { HasScenario = true; Step = And; Outcome = Fail; StepTitle = stepTitle; Expected = "    And step title [Failed]" }
            { HasScenario = true; Step = And; Outcome = Inconclusive; StepTitle = stepTitle; Expected = "    And step title [Inconclusive]" }
            { HasScenario = true; Step = And; Outcome = NotRun; StepTitle = stepTitle; Expected = "    And step title [not run]" }
            { HasScenario = true; Step = And; Outcome = Pass; StepTitle = stepTitle; Expected = "    And step title [Passed]" }

            { HasScenario = true; Step = But; Outcome = Fail; StepTitle = stepTitle; Expected = "    But step title [Failed]" }
            { HasScenario = true; Step = But; Outcome = Inconclusive; StepTitle = stepTitle; Expected = "    But step title [Inconclusive]" }
            { HasScenario = true; Step = But; Outcome = NotRun; StepTitle = stepTitle; Expected = "    But step title [not run]" }
            { HasScenario = true; Step = But; Outcome = Pass; StepTitle = stepTitle; Expected = "    But step title [Passed]" }
        ]

    [<TestCaseSource("stepResultsSource")>]
    let ToResults_IndividualForEachStepAndOutcomeAndScenario_MapsToExpected (stepResultSource: StepResultSource) =
        let stepResults = [{Step = stepResultSource.Step; Outcome = stepResultSource.Outcome; Text = Some stepResultSource.StepTitle }]
        let results = stepResults |> toResults stepResultSource.HasScenario

        results.Length |> should equal 1
        results.[0].Title |> should equal  (Some stepResultSource.StepTitle)
        results.[0].Outcome |> should equal stepResultSource.Outcome
        results.[0].Description |> should equal stepResultSource.Expected
        results.[0].Step |> should equal stepResultSource.Step
