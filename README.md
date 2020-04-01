## About BddPipe ##

This project was created to describe test steps.  It has been developed to replace similar existing libraries as a more succinct and simpler to use alternative.

#### See full details on the Wiki ####
- [Home](https://github.com/CraigRice/BddPipe/wiki)
- [Getting Started](https://github.com/CraigRice/BddPipe/wiki/Getting-Started)
- [Implementing Steps](https://github.com/CraigRice/BddPipe/wiki/Implementing-Steps)
- [Recipes](https://github.com/CraigRice/BddPipe/wiki/Recipes) (step reuse)
- [Map](https://github.com/CraigRice/BddPipe/wiki/Recipes-and-Pipe-Map-Function)
- [Test Output](https://github.com/CraigRice/BddPipe/wiki/Test-Output)
- [Version Release Notes](https://github.com/CraigRice/BddPipe/wiki/Version-Release-Notes)

### Goals ###

- Let each step _define and return to the next step_ all the state it needs to proceed. 
- _Avoid declaring variables outside test step functions_ as a way to store/retreive data between steps.
- Avoid having to set defaults for these declared variables before they are ready to be assigned.
- Provide a way to reuse steps.
- Have better control of output if desired - the outcome is detailed and the default console output can be captured or replaced.
- The steps are documented in greater detail than regular tests due to the step type and title. Tests with documented steps can be easier to understand and maintain.


## Getting started ##
### Add the using statement: ###
```C#
using static BddPipe.Runner;
```

### Basic example: ###

```C#
Scenario()
  .Given("two numbers", () => new { A = 5, B = 10 })
  .When("the numbers are summed", setup => setup.A + setup.B)
  .Then("sum should be as expected", result =>
  {
    Assert.AreEqual(15, result);
  })
  .Run();
```

> **Steps must end with a call to .Run() or the result is not evaluated.**

### Scenario: ###

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

### Async: ###

Async steps can be provided via an async delegate implementation - you can provide a method returning `Task` or `Task<T>`:

```C#
When("The numbers are summed", async args =>
{
    await Task.Delay(10);
    return new { Result = args.A + args.B };
})
```

### Output: ###

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
