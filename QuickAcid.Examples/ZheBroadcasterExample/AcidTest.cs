using System.Reflection;
using QuickAcid.Examples.ZheBroadcasterExample.SimpleModel;
using QuickFuzzr;
using QuickPulse.Explains;

namespace QuickAcid.Examples.ZheBroadcasterExample;

[DocFile]
[DocFileHeader("Broadcaster: Copy-on-Write")]
[DocContent(
@"> Iterating a list while (concurrently) mutating it is one way of banging your head against an `InvalidOperationException`.

This example models a simple broadcaster that keeps a list of connected clients and sends a notification to all of them.

* **Register** adds a client.
* **Broadcast** enumerates the `clients` list and calls `SendNotificationAsynchronously`.
* **On fault**, the client is unregistered.

The subtle bug: `Register` **mutates the list in place** (`clients.Add(client)`)
while `Broadcast` **enumerates without a lock**.
If a registration happens during a broadcast, enumeration can throw
`InvalidOperationException: Collection was modified; enumeration operation may not execute.`")]
public partial class AcidTest
{
    [Fact]
    [DocHeader("The Buggy Bits (an excerpt)")]
    [DocCode(
@"// BUG: in-place mutation while others may be enumerating
private void AddClientToRegisteredClients(IClientProxy client)
{
    lock (theDoor)
    {
        // Correct (copy-on-write) would be:
        // clients = new List<IClientProxy>(clients) { client };
        clients.Add(client);
    }
}

// Correct removal uses copy-on-write (safe during enumeration)
private void RemoveClientFromRegisteredClients(IClientProxy client)
{
    lock (theDoor)
    {
        var next = new List<IClientProxy>(clients);
        next.Remove(client);
        clients = next;
    }
}")]
    [DocHeader("The **Passing** Unit Tests")]
    [DocCodeFile("UnitTests.cs", "csharp")]
    [DocHeader("The **Failing** Acid Test")]
    [DocExample(typeof(AcidTest), nameof(TheTest))]
    [DocHeader("The Report")]
    [DocCodeFile("Broadcaster.qr")]
    public void AllInOne()
    {
        QState.Run(TheTest(), 1718881614)
            .Options(a => a with { FileAs = "Broadcaster" })
            .With(10.Runs())
            .And(50.ExecutionsPerRun());
    }

    public record RegisterClient : Act
    {
        public record ClientExistsInCollection : Spec;
    }

    public record RegisteredClientFaults : Act
    {
        public record ClientIsRemovedFromCollection : Spec;
    }

    public record Broadcast : Act
    {
        public record DoesNotThrow : Spec;
    }

    public record StopBroadcasting : Act
    {
        public record DoesNotThrow : Spec;
    }

    [CodeSnippet]
    public QAcidScript<Acid> TheTest()
    {
        return
            from factory in Script.Stashed(() => new TestClientProxyFactory())
            from broadcaster in Script.Stashed(() => new Broadcaster(factory))
            from needler in Script.Stashed(() => new Needler())
            from _ in Script.Choose(

                // 1) Register
                from _a in Script.Act<RegisterClient>(broadcaster.Register)
                from _s in Script.Spec<RegisterClient.ClientExistsInCollection>(() =>
                    GetBroadcastersClients(broadcaster).Contains(factory.CreatedClients.Last()))
                select Acid.Test,

                // 2) Remove on fault
                from faulty in Script.Execute(
                    Fuzz.ChooseFromWithDefaultWhenEmpty(GetBroadcastersClients(broadcaster)))
                from _b in Script.ActIf<RegisteredClientFaults>(() => faulty != null,
                    () => ((TestClientProxy)faulty!).Fault())
                from _sb in Script.Spec<RegisteredClientFaults.ClientIsRemovedFromCollection>(() =>
                    !GetBroadcastersClients(broadcaster).Contains(faulty))
                select Acid.Test,

                // 3) Start broadcast in background
                from _c in Script.ActIf<Broadcast>(() => !needler.ThreadSwitch,
                    () => needler.Start(() => broadcaster.Broadcast(new Notification())))
                from _sc in Script.Spec<Broadcast.DoesNotThrow>(() => needler.ExceptionFromThread == null)
                select Acid.Test,

                // 4) Stop broadcast
                from _d in Script.ActIf<StopBroadcasting>(() => needler.ThreadSwitch, needler.Stop)
                from _sd in Script.Spec<StopBroadcasting.DoesNotThrow>(() => needler.ExceptionFromThread == null)
                select Acid.Test)
            select Acid.Test;
    }

    private static List<IClientProxy> GetBroadcastersClients(Broadcaster caster)
    {
        var clientsFieldInfo =
            typeof(Broadcaster).GetField("clients", BindingFlags.NonPublic | BindingFlags.Instance);
        return (List<IClientProxy>)clientsFieldInfo!.GetValue(caster)!;
    }
}

