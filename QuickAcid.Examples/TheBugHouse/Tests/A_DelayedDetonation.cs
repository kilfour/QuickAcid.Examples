using QuickFuzzr;
using QuickPulse.Explains;
using StringExtensionCombinators;

namespace QuickAcid.Examples.TheBugHouse.Tests;


[DocFile]
[DocContent(
@"Throws only after exactly three total runs when the input value is 1.  
Demonstrates a simple stateful failure triggered by run count.")]
[DocExample(typeof(BugHouse), nameof(BugHouse.Run))]
public class A_DelayedDetonation
{
    public class BugHouse
    {
        private int count;
        [CodeSnippet]
        public bool Run(int a)
        {
            return !(count++ == 2 && a == 1);
        }
    }

    [Fact]
    [DocHeader("Minimal Fail Case:")]
    [DocContent(
@"- **Length:** 3 executions.
- **Example input:** `[42, 666, 1]`
```csharp
count = 0
Run( a = <any> ) // count = 1
Run( a = <any> ) // count = 2
Run( a = 1 ) // count == 2 && a == 1 → fail
```")]
    [DocHeader("Reports:")]
    [DocCodeFile("A__Report.qr")]
    public void AcidTest()
    {
        var script =
            from a in "a".Input(Fuzz.Int(0, 10))
            from bughouse in "BugHouse".Stashed(() => new BugHouse())
            from output in "BugHouse.Run".Act(() => bughouse.Run(a))
            from spec in "Delayed Detonation: BugHouse.Run returns true".Spec(() => output)
            select Acid.Test;

        QState.Run(script, 1347683198)
            .Options(a => a with { FileAs = "DelayedDetonation" })
            .WithOneRun()
            .And(5.ExecutionsPerRun());
    }
}
