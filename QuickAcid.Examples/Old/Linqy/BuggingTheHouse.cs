using QuickFuzzr;

namespace QuickAcid.Examples.Old.Linqy
{
	public class BuggingTheHouse
	{
		public class BugHouse1
		{
			private int count;
			public bool Run(int a)
			{
				if (count++ == 2 && a == 1) throw new Exception(); return true;
			}
		}

		[Fact(Skip = "Explicit")]
		public void BugHouse1Error()
		{
			var script =
				from a in "a".Input(Fuzz.Int(0, 10))
				from bughouse in "bughouse".Tracked(() => new BugHouse1())
				from output in "bughouse.Run".Act(() => bughouse.Run(a))
				from spec in "returns true".Spec(() => output)
				select Acid.Test;
			QState.Run(script).With(100.Runs()).And(100.ExecutionsPerRun());
		}

		public class BugHouse2
		{
			private int count;
			public bool Run(int a)
			{
				if (a == 6) count++;
				if (count == 3) throw new Exception(); return true;
			}
		}

		[Fact(Skip = "explicit")]
		public void BugHouse2Error()
		{
			var script =
				from a in "a".Input(Fuzz.Int(0, 10))
				from bughouse in "bughouse".Tracked(() => new BugHouse2())
				from output in "bughouse.Run".Act(() => bughouse.Run(a))
				from spec in "returns true".Spec(() => output)
				select Acid.Test;
			QState.Run(script).WithOneRun().And(50.ExecutionsPerRun());
		}

		public class BugHouse3
		{
			private int count;
			public bool Run(int a)
			{
				if (a == 6 && count != 3) count++;
				if (count >= 3) count++;
				if (count == 5) throw new Exception(); return true;
			}
		}

		[Fact(Skip = "Explicit")]
		public void BugHouse3Error()
		{
			var script =
				from a in "a".Input(Fuzz.Int(0, 10))
				from bughouse in "bughouse".Tracked(() => new BugHouse3())
				from output in "bughouse.Run".Act(() => bughouse.Run(a))
				from spec in "returns true".Spec(() => output)
				select Acid.Test;
			QState.Run(script).With(100.Runs()).And(100.ExecutionsPerRun());
		}
	}
}