using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using QuickFuzzr;

namespace QuickAcid.Examples;

// ====================
// FsCheck Version
// ====================
public enum StackOp { Push, Pop }

public class FsCheckStackSpec
{
    [Property]
    public Property StackShouldBehaveCorrectly()
    {
        var stackOps = Gen.ListOf(
            Gen.OneOf(
                from v in Gen.Choose(0, 100)
                select (StackOp.Push, v), Gen.Constant((StackOp.Pop, 0)))
        );

        return Prop.ForAll(stackOps.ToArbitrary(), ops =>
        {
            var stack = new Stack<int>();
            var pushed = new List<int>();
            var popped = new List<int>();
            var expectedPops = new List<int>();

            foreach (var (op, val) in ops)
            {
                if (op == StackOp.Push)
                {
                    stack.Push(val);
                    pushed.Add(val);
                }
                else if (stack.Count > 0)
                {
                    expectedPops.Add(stack.Peek());
                    popped.Add(stack.Pop());
                }
            }

            var countOkay = stack.Count == (pushed.Count - popped.Count);
            var poppedReverse = popped.SequenceEqual(expectedPops);
            return countOkay && poppedReverse;
        });
    }
}


// ====================
// QuickAcid Version
// ====================
public class StackAcidSpec
{
    [Fact]
    public void StackBehavesCorrectly()
    {
        var script =
            from stack in "stack".Tracked(() => new Stack<int>())
            from pushed in "pushed".Tracked(() => new List<int>())
            from expectedPops in "expectedPops".Tracked(() => new List<int>())
            from popped in "popped".Tracked(() => new List<int>())
            from val in "pushval".Input(Fuzz.Int(0, 100))
            from action in Script.Choose(
                "push".Act(() => { stack.Push(val); pushed.Add(val); }),
                "pop".ActIf(
                    () => stack.Count > 0,
                    () => { expectedPops.Add(stack.Peek()); popped.Add(stack.Pop()); }))
            from finalCountOkay in "FinalCountOkay".Spec(
                    () => stack.Count == pushed.Count - popped.Count)
            from popsInReverse in "PopsInReverse".Spec(
                () => popped.SequenceEqual(expectedPops))
            select Acid.Test;
        QState.Run(script).With(10.Runs()).And(20.ExecutionsPerRun());
    }
}