using QuickAcid;
using QuickFuzzr;
using QuickPulse.Explains;

namespace QuickAcid.Examples.UsingAnOracle;

[DocFile]
[DocFileHeader("Deleting From a List")]
[DocContent(
@"When `Remove` only removes the first occurrence.

This example demonstrates how a property-based test can expose a subtle bug in list deletion logic:
`List<T>.Remove(value)` stops after the first match.
If the list contains duplicates, some remain, violating the intended behavior.")]

public class DeletingFromAList
{
	public class ListDeleter
	{
		public IList<int> DoingMyThing(IList<int> theList, int iNeedToBeRemoved)
		{
			var result = theList.ToList();
			result.Remove(iNeedToBeRemoved);
			return result;
		}
	}

	[Fact]
	[DocHeader("The Buggy Implementation")]
	[DocCode(
@"public class ListDeleter
{
    // Removes only the first matching element.
    public IList<int> DoingMyThing(IList<int> theList, int iNeedToBeRemoved)
    {
        var result = theList.ToList();
        result.Remove(iNeedToBeRemoved);
        return result;
    }
}")]
	[DocHeader("The Acid Test")]
	[DocCodeExample(typeof(DeletingFromAList), nameof(TheTest))]
	[DocHeader("The Report")]
	[DocCodeFile("DeletingFromAList.qr")]
	public void LetsAskTheOracle()
	{
		QState.Run(TheTest(), 1745533359)
			.Options(a => a with { FileAs = "DeletingFromAList" })
			.With(10.Runs())
			.And(10.ExecutionsPerRun());
	}

	[DocExample]
	public QAcidScript<Acid> TheTest()
	{
		return
			from sut in "ListDeleter".Stashed(() => new ListDeleter())
			let listGenerator =
				from listLength in Fuzz.Int(10, 20)
				from list in Fuzz.Int(0, 10).Many(listLength).ToList()
				select list
			from list in "List".Input(listGenerator)
			from toRemove in "Element to remove".Input(Fuzz.Int(0, 10))
			from output in "ListDeleter.DoingMyThing".Act(() => sut.DoingMyThing(list, toRemove))
			from expected in "Oracle".Derived(() => list.Where(x => x != toRemove).ToList())
			from spec1 in "Removes all occurrences".Spec(() => !output.Contains(toRemove))
			from spec2 in "Does not over-delete".Spec(() => output.Count == list.Count(x => x != toRemove))
			from spec3 in "Preserves order of survivors".Spec(() => output.SequenceEqual(expected))
			from twice in "Apply twice".Derived(() => new ListDeleter().DoingMyThing(output, toRemove))
			from specIdem in "Idempotent delete".Spec(() => twice.SequenceEqual(output))
			select Acid.Test;
	}
}
