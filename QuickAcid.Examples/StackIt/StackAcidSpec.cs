using QuickFuzzr;
using StringExtensionCombinators;

namespace QuickAcid.Examples.StackIt;


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
            from val in "pushval".Input(Fuzzr.Int(0, 100))
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