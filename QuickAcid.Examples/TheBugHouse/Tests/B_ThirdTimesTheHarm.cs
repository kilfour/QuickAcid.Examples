using QuickFuzzr;
using QuickPulse.Explains;
using StringExtensionCombinators;

namespace QuickAcid.Examples.TheBugHouse.Tests;

[DocFile]
[DocFileHeader("Third Times the Harm")]
[DocContent(
@"Throws as soon as the value 6 has been seen three times.  
A straightforward counter-based failure condition.")]
[DocExample(typeof(BugHouse), nameof(BugHouse.Run))]
public class B_ThirdTimesTheHarm
{
    public class BugHouse
    {
        private int count;
        [CodeSnippet]
        public bool Run(int a)
        {
            if (a == 6) count++;
            return !(count == 3);
        }
    }

    [Fact]
    [DocHeader("Minimal Fail Case:")]
    [DocContent(
@"- **Length:** 3 executions.
- **Example input:** `[6, 6, 6]`
```csharp
count = 0
Run( a = 6 ) // count = 1
Run( a = 6 ) // count = 2
Run( a = 6 ) // count == 3 → fail
```")]
    [DocHeader("Reports:")]
    [DocCodeFile("B__Report.qr")]
    public void AcidTest()
    {
        var script =
            from a in "a".Input(Fuzzr.Int(0, 10))
            from bughouse in "BugHouse".Stashed(() => new BugHouse())
            from output in "BugHouse.Run".Act(() => bughouse.Run(a))
            from spec in "Third Times the Harm: BugHouse.Run returns true".Spec(() => output)
            select Acid.Test;

        QState.Run(script, 291715583)
            .Options(a => a with { FileAs = "ThirdTimesTheHarm" })
            .WithOneRun()
            .And(50.ExecutionsPerRun());
    }
}