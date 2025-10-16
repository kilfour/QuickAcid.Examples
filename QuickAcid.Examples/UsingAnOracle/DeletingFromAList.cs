using QuickAcid;
using QuickFuzzr;
using QuickPulse.Explains;
using StringExtensionCombinators;

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
	[CodeExample]
	public class ListDeleter
	{
		// Removes only the first matching element.
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
    
    public IList<int> DoingMyThing(IList<int> theList, int iNeedToBeRemoved)
    {
        var result = theList.ToList();
        result.Remove(iNeedToBeRemoved);
        return result;
    }
}")]
	[DocHeader("The Acid Test")]
	[DocExample(typeof(DeletingFromAList), nameof(TheTest))]
	[DocHeader("The Report")]
	[DocCodeFile("DeletingFromAList.qr")]
	public void LetsAskTheOracle()
	{
		QState.Run(TheTest(), 1745533359)
			.Options(a => a with { FileAs = "DeletingFromAList" })
			.With(10.Runs())
			.And(10.ExecutionsPerRun());
	}


	public record DeleteFromList : Act
	{
		public record List : Input;
		public record ElementToRemove : Input;
		public record RemovesAllOccurences : Spec;
		public record DoesNotOverDelete : Spec;
		public record PreservesOrderOfSurvivors : Spec;
		public record IdempotentDelete : Spec;
	}

	[CodeSnippet]
	[CodeRemove("return")]
	private static QAcidScript<Acid> TheTest()
	{
		return
			from sut in Script.Stashed(() => new ListDeleter())

			let listGenerator =
				from listLength in Fuzz.Int(10, 20)
				from list in Fuzz.Int(0, 10).Many(listLength).ToList()
				select list

			from list in Script.Input<DeleteFromList.List>().With(listGenerator)
			from toRemove in Script.Input<DeleteFromList.ElementToRemove>().With(Fuzz.Int(0, 10))

			from output in Script.Act<DeleteFromList>().With(() => sut.DoingMyThing(list, toRemove))

			from expected in Script.Execute(() => list.Where(x => x != toRemove).ToList())

			from spec1 in Script.Spec<DeleteFromList.RemovesAllOccurences>(
				() => !output.Contains(toRemove))

			from spec2 in Script.Spec<DeleteFromList.DoesNotOverDelete>(
				() => output.Count == list.Count(x => x != toRemove))

			from spec3 in Script.Spec<DeleteFromList.PreservesOrderOfSurvivors>(
				() => output.SequenceEqual(expected))

			from twice in Script.Execute(
				() => new ListDeleter().DoingMyThing(output, toRemove))

			from specIdem in Script.Spec<DeleteFromList.IdempotentDelete>(
				() => twice.SequenceEqual(output))

			select Acid.Test;
	}
}
