using QuickFuzzr;
using QuickPulse.Explains;

namespace QuickAcid.Examples.TheBugHouse.Tests;

[DocFile]
[DocContent("Exception occurs only from a specific " +
"interleaving of RunInt and RunString calls, " +
"each appending different markers to state. " +
"Demonstrates multi-method state interaction.  ")]
[DocContent("**RunInt:**")]
[DocCodeExample(typeof(BugHouse), nameof(BugHouse.RunInt))]
[DocContent("**RunString:**")]
[DocCodeExample(typeof(BugHouse), nameof(BugHouse.RunString))]
public class D_MultipleMethods
{
	public class BugHouse
	{
		private string? bug;

		[DocExample]
		public bool RunInt(int a)
		{
			bug += "1";
			if (bug.EndsWith("1221") && a == 6) return false;
			return true;
		}

		[DocExample]
		public bool RunString(string a)
		{
			bug += "2";
			if (bug.EndsWith("122") && a == "p") return false;
			return true;
		}
	}

	[Fact]
	[DocHeader("Minimal Fail Case (one example):")]
	[DocContent(
@"- **Length:** 4 executions.
- **Example input sequence:**
```csharp
RunInt(<any>)        // bug = ""1""
RunString(<any>)     // bug = ""12""
RunString(<any>)     // bug = ""122""
RunInt(6)            // bug ends with ""1221"" and `a == 6` → fail
```")]
	[DocHeader("Reports:")]
	[DocCodeFile("D__Report.qr")]
	public void AcidTest()
	{
		var script =
			from bughouse in "BugHouse".Stashed(() => new BugHouse())
			from funcOne in
				"Choose".Choose(
					from i in "int".Input(Fuzz.Int(0, 10))
					from runInt in "BugHouse.RunInt".Act(() => bughouse.RunInt(i))
					from specOne in "BugHouse.RunInt returns true".Spec(() => runInt)
					select Acid.Test,
					from str in "string".Input(Fuzz.String(1, 1))
					from runString in "BugHouse.RunString".Act(() => bughouse.RunString(str))
					from specTwo in "BugHouse.RunString returns true".Spec(() => runString)
					select Acid.Test)
			select Acid.Test;
		QState.Run(script)
			.Options(a => a with { FileAs = "MultipleMethods" })
			.With(100.Runs())
			.And(100.ExecutionsPerRun());
	}
}