using QuickFuzzr;

namespace QuickAcid.Examples
{
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

		[Fact(Skip = "Explicit")]
		public void ReportsError()
		{
			var intBetweenZeroAndTen = Fuzz.Int(0, 10);

			var ints =
				from numberOfInts in intBetweenZeroAndTen
				from list in intBetweenZeroAndTen.Many(numberOfInts).ToList()
				select list;

			var listDeleter = new ListDeleter();

			var script =
				from list in "input list".Input(ints)
				from toRemove in "to remove".Input(intBetweenZeroAndTen)
				from output in "listDeleter.DoingMyThing".Act(() => listDeleter.DoingMyThing(list, toRemove))
				from spec in "int removed".Spec(() => !output.Contains(toRemove))
				select Acid.Test;
			QState.Run(script).With(10.Runs()).And(10.ExecutionsPerRun());
		}
	}
}
