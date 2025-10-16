using QuickFuzzr;
using QuickPulse.Explains;
using StringExtensionCombinators;

namespace QuickAcid.Examples.TheBugHouse.Tests;

[DocFile]
[DocContent("Fails after a more complex sequence: " +
"specific inputs increment the counter in " +
"different ways, requiring a particular " +
"input dance to trigger the failure.")]
[DocExample(typeof(BugHouse), nameof(BugHouse.Run))]
public class C_ConvolutedCountdown
{
	public class BugHouse
	{
		private int count;

		[CodeSnippet]
		public bool Run(int a)
		{
			if (a == 6 && count != 3) count++;
			if (count >= 3) count++;
			if (count == 5) return false; return true;
		}
	}

	[Fact]
	[DocHeader("Minimal Fail Case:")]
	[DocContent(
@"- **Length:** 4 executions.
- **Example input:** `[6, 6, 6, 42]`
```csharp
count = 0
Run( a = 6 ) // +1 → 1
Run( a = 6 ) // +1 → 2
Run( a = 6 ) // +1 → 3, then >=3 → +1 → 4
Run( a != 6 ) // skip first inc, >=3 → +1 → 5 → fail
```")]
	[DocHeader("Reports:")]
	[DocCodeFile("C__Report.qr")]
	public void AcidTest()
	{
		var script =
			from a in "a".Input(Fuzz.Int(0, 10))
			from bughouse in "BugHouse".Stashed(() => new BugHouse())
			from output in "BugHouse.Run".Act(() => bughouse.Run(a))
			from spec in "Convoluted Countdown: BugHouse.Run returns true".Spec(() => output)
			select Acid.Test;

		QState.Run(script, 1221987654)
			.Options(a => a with { FileAs = "ConvolutedCountdown" })
			.With(100.Runs())
			.And(100.ExecutionsPerRun());
	}
}