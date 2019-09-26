## About BddPipe

This project was created to describe test steps.  It has been developed to replace similar existing libraries as a more succinct and simpler to use alternative.

### Goals

- let each step _define and return to the next step_ all the state it needs to proceed. 
- _avoid declaring variables outside test step functions_ as a way to store/retreive data between steps
- avoid having to set defaults for these declared variables before they are ready to be assigned
- have better control of output if desired - the outcome is detailed and the default console output can be captured or replaced.

## Getting started
### Add the using statement:
```C#
using static BddPipe.Runner;
```

### Basic example:

```C#
Given("two numbers", () => new { A = 5, B = 10 })
.When("the numbers are summed", args => new { Result = args.A + args.B })
.Then("sum should be as expected", arg =>
{
    arg.Result.Should().Be(15);
})
.Run();
```

> **Steps must end with a call to .Run() or the result is not evaluated.**

### Scenario:

It is optional to start with `Scenario()` for a scenario describing your method name, or `Scenario("Your scenario title")`
        
```C#
// This example will use the method name in the output
Scenario()
    .Given(...)

// This example will use "Your scenario title" in the output
Scenario("Your scenario title")
    .Given(...)
```

The scenario will then be at the top of the test output:

```
Scenario: Your scenario title  
  Given two numbers [Passed]  
  When the numbers are summed [Passed]  
  Then sum should be as expected [Passed]  
```

There are a number of overloads for the method steps: _Given_, _When_, _Then_, _And_, _But_.

When implementing steps, you can return from them whatever you like - including anonymous types or tuples.
If something is returned from one of these steps then **all following steps using a parameter will have a parameter of this instance, until a different instance is returned from a step**.  This also allows Action steps to be placed between the step returning something and the step that will later make use of it.

### Async:

Async steps can be provided via an async delegate implementation - you can provide a method returning `Task` or `Task<T>`:

```C#
    When("The numbers are summed", async args =>
    {
        await Task.Delay(10);
        return new { Result = args.A + args.B};
    })
```

### Output:

By default the output is written to console.  Each step will be marked as passed, failed, inconclusive or not run.
Steps after a failed or inconclusive step will not be run.

```
Given two numbers [Passed]  
When the numbers are summed [Passed]  
Then sum should be as expected [Passed]  
```

You can instead be given this output by supplying an action to the final Run() call.
This will no longer console write the output by default and will give the result to your implementation.

`.Run(output => { /*handle output*/ });`

It's still possible to call `Runner.WriteLogsToConsole(...)` from your implementation to use the default console logging.

You can also get the details of the output from the result of the final Run() call as an instance of `BddPipeResult<T>`.

### Actions:

It's also possible to use only actions for all steps and never return from them, but state then has to be maintained outside of the pipeline. This project has been built to _avoid the need for this approach_ but it is still possible. 

```C#
    int a = 0, b = 0;
    int result = 0;

    Given("two numbers", () =>
    {
        a = 6;
        b = 20;
    })
    ...
```        
