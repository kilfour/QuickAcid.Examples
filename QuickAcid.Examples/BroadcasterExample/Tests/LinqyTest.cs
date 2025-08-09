using System.Reflection;
using QuickAcid.Examples.BroadcasterExample.SimpleModel;
using QuickFuzzr;

namespace QuickAcid.Examples.BroadcasterExample;

public partial class LinqyTest
{

    [Fact(Skip = "WIP")]
    //[Fact]
    public void AllInOne()
    {
        var script =
            from clientProxyFactory in "ClientProxyFactory".Stashed(() => new TestClientProxyFactory())
            from broadcaster in "Broadcaster".Stashed(() => new Broadcaster(clientProxyFactory))
            from needler in "Needler".Stashed(() => new Needler())
            from _o1 in "ops".Choose(
                from _a1 in "Register Client".Act(broadcaster.Register)
                from _s1 in "Client Exists In Collection".Spec(() => GetBroadcastersClients(broadcaster).Contains(clientProxyFactory.CreatedClients.Last()))
                select Acid.Test,
                from faultyClient in "Faulty Client".Derived(Fuzz.ChooseFromWithDefaultWhenEmpty(GetBroadcastersClients(broadcaster)))
                from _a2 in "Registered Client Faults".ActIf(() => faultyClient != null, () => ((TestClientProxy)faultyClient).Fault())
                from _s2 in "Client Is Removed From Collection".Spec(() => !GetBroadcastersClients(broadcaster).Contains(faultyClient))
                select Acid.Test,
                from _a3 in "Broadcast".ActIf(() => !needler.ThreadSwitch, () => needler.Start(() => broadcaster.Broadcast(new Notification())))
                from _s3 in "Start Does Not Throw".Spec(() => needler.ExceptionFromThread == null)
                select Acid.Test,
                from _a4 in "Stop Broadcasting".ActIf(() => needler.ThreadSwitch, needler.Stop)
                from _s4 in "Stop Does Not Throw".Spec(() => needler.ExceptionFromThread == null)
                select Acid.Test
            )
            select Acid.Test;

        QState.Run(script).With(10.Runs()).And(50.ExecutionsPerRun());
    }

    private static List<IClientProxy> GetBroadcastersClients(Broadcaster caster)
    {
        var clientsFieldInfo =
            typeof(Broadcaster).GetField("clients", BindingFlags.NonPublic | BindingFlags.Instance);
        return (List<IClientProxy>)clientsFieldInfo!.GetValue(caster)!;
    }
}

