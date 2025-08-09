# QuickAcid.Examples
> Zen and the Art of Code Maintenance.
## Introduction
**QuickAcid.Examples** is a curated collection of property-based tests designed to show QuickAcid in action.  
Each example breaks something on purpose, sometimes in obvious ways,
sometimes in subtle, stateful, or downright mischievous ones,
so you can see how failures are found, minimized, and explained. 


Think of it as a workshop manual for QuickAcid: part demonstration, part exploration, part philosophy of testing.
## Bughousing
Bughousing is a set of intentionally fragile programs, each with its own peculiar way of failing.  
They range from simple state-based traps to intricate multi-method booby traps,
all designed to show how QuickAcid can uncover, shrink, and explain subtle bugs.
### Delayed Detonation
Throws only after exactly three total runs when the input value is 1.  
Demonstrates a simple stateful failure triggered by run count.
```csharp
return !(count++ == 2 && a == 1);
```
#### Minimal Fail Case:
- **Length:** 3 executions.
- **Example input:** `[42, 666, 1]`
```csharp
count = 0
Run( a = <any> ) // count = 1
Run( a = <any> ) // count = 2
Run( a = 1 ) // count == 2 && a == 1 → fail
```
#### Reports:
```
──────────────────────────────────────────────────
 Original failing run:    3 executions
 Minimal failing case:    3 executions (after 6 shrinks)
 Seed:                    1347683198
 ──────────────────────────────────────────────────
  Executed : BugHouse.Run (2 Times)
 ──────────────────────────────────────────────────
  Executed (2): BugHouse.Run
   - Input: a = 1
 ════════════════════════════════════════════════════════════════
  ❌ Spec Failed: Delayed Detonation: BugHouse.Run returns true
 ════════════════════════════════════════════════════════════════
 Passed Specs
  - Delayed Detonation: BugHouse.Run returns true: 2x
 ──────────────────────────────────────────────────
```
### Third Times the Harm
Throws as soon as the value 6 has been seen three times.  
A straightforward counter-based failure condition.
```csharp
if (a == 6) count++;
return !(count == 3);
```
#### Minimal Fail Case:
- **Length:** 3 executions.
- **Example input:** `[6, 6, 6]`
```csharp
count = 0
Run( a = 6 ) // count = 1
Run( a = 6 ) // count = 2
Run( a = 6 ) // count == 3 → fail
```
#### Reports:
```
──────────────────────────────────────────────────
 Original failing run:    22 executions
 Minimal failing case:    3 executions (after 25 shrinks)
 Seed:                    291715583
 ──────────────────────────────────────────────────
  Executed : BugHouse.Run (3 Times)
   - Input: a = 6
 ══════════════════════════════════════════════════════════════════
  ❌ Spec Failed: Third Times the Harm: BugHouse.Run returns true
 ══════════════════════════════════════════════════════════════════
 Passed Specs
  - Third Times the Harm: BugHouse.Run returns true: 21x
 ──────────────────────────────────────────────────
```
### Convoluted Countdown
Fails after a more complex sequence: specific inputs increment the counter in different ways, requiring a particular input dance to trigger the failure.
```csharp
if (a == 6 && count != 3) count++;
if (count >= 3) count++;
if (count == 5) return false; return true;
```
#### Minimal Fail Case:
- **Length:** 4 executions.
- **Example input:** `[6, 6, 6, 42]`
```csharp
count = 0
Run( a = 6 ) // +1 → 1
Run( a = 6 ) // +1 → 2
Run( a = 6 ) // +1 → 3, then >=3 → +1 → 4
Run( a != 6 ) // skip first inc, >=3 → +1 → 5 → fail
```
#### Reports:
```
──────────────────────────────────────────────────
 Original failing run:    36 executions
 Minimal failing case:    4 executions (after 40 shrinks)
 Seed:                    1221987654
 ──────────────────────────────────────────────────
  Executed : BugHouse.Run (3 Times)
   - Input: a = 6
 ──────────────────────────────────────────────────
  Executed (35): BugHouse.Run
 ══════════════════════════════════════════════════════════════════
  ❌ Spec Failed: Convoluted Countdown: BugHouse.Run returns true
 ══════════════════════════════════════════════════════════════════
 Passed Specs
  - Convoluted Countdown: BugHouse.Run returns true: 35x
 ──────────────────────────────────────────────────
```
### Multiple Methods
Exception occurs only from a specific interleaving of RunInt and RunString calls, each appending different markers to state. Demonstrates multi-method state interaction.  
**RunInt:**
```csharp
bug += "1";
if (bug.EndsWith("1221") && a == 6) return false;
return true;
```
**RunString:**
```csharp
bug += "2";
if (bug.EndsWith("122") && a == "p") return false;
return true;
```
#### Minimal Fail Case (one example):
- **Length:** 4 executions.
- **Example input sequence:**
```csharp
RunInt(<any>)        // bug = "1"
RunString(<any>)     // bug = "12"
RunString(<any>)     // bug = "122"
RunInt(6)            // bug ends with "1221" and `a == 6` → fail
```
#### Reports:
```
──────────────────────────────────────────────────
 Original failing run:    99 executions
 Minimal failing case:    4 executions (after 103 shrinks)
 Seed:                    1127921454
 ──────────────────────────────────────────────────
  Executed (95): BugHouse.RunInt
 ──────────────────────────────────────────────────
  Executed : BugHouse.RunString (2 Times)
 ──────────────────────────────────────────────────
  Executed (98): BugHouse.RunInt
   - Input: int = 6
 ═══════════════════════════════════════════════
  ❌ Spec Failed: BugHouse.RunInt returns true
 ═══════════════════════════════════════════════
 Passed Specs
  - BugHouse.RunString returns true: 45x
  - BugHouse.RunInt returns true: 53x
 ──────────────────────────────────────────────────
```
## Deleting From a List
When `Remove` only removes the first occurrence.

This example demonstrates how a property-based test can expose a subtle bug in list deletion logic:
`List<T>.Remove(value)` stops after the first match.
If the list contains duplicates, some remain, violating the intended behavior.
### The Buggy Implementation
```csharp
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
```
### The Acid Test
```csharp
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
```
### The Report
```
──────────────────────────────────────────────────
 Original failing run:    3 executions
 Minimal failing case:    1 execution (after 3 shrinks)
 Seed:                    1745533359
 ──────────────────────────────────────────────────
  Executed (2): ListDeleter.DoingMyThing
   - Input: List = [ 5, 5 ]
   - Input: Element to remove = 5
 ══════════════════════════════════════════
  ❌ Spec Failed: Removes all occurrences
 ══════════════════════════════════════════
 Passed Specs
  - Removes all occurrences: 2x
  - Does not over-delete: 2x
  - Preserves order of survivors: 2x
  - Idempotent delete: 2x
 ──────────────────────────────────────────────────
```
## Broadcaster: Copy-on-Write
> Iterating a list while (concurrently) mutating it is one way of banging your head against an `InvalidOperationException`.

This example models a simple broadcaster that keeps a list of connected clients and sends a notification to all of them.

* **Register** adds a client.
* **Broadcast** enumerates the `clients` list and calls `SendNotificationAsynchronously`.
* **On fault**, the client is unregistered.

The subtle bug: `Register` **mutates the list in place** (`clients.Add(client)`)
while `Broadcast` **enumerates without a lock**.
If a registration happens during a broadcast, enumeration can throw
`InvalidOperationException: Collection was modified; enumeration operation may not execute.`
### The Buggy Bits (an excerpt)
```csharp
// BUG: in-place mutation while others may be enumerating
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
}
```
### The **Passing** Unit Tests
```csharp
using QuickAcid.Examples.ZheBroadcasterExample.SimpleModel;

namespace QuickAcid.Examples.ZheBroadcasterExample;

public class UnitTests
{
    [Fact]
    public void Broadcaster_start_stop_no_exception()
    {
        var clientProxyFactory = new TestClientProxyFactory();
        var broadcaster = new Broadcaster(clientProxyFactory);
        var needler = new Needler();
        needler.Start(() => broadcaster.Broadcast(new Notification()));
        needler.Stop();
        Assert.Null(needler.ExceptionFromThread);
    }

    [Fact]
    public void Broadcaster_start_register_stop_no_exception()
    {
        var clientProxyFactory = new TestClientProxyFactory();
        var broadcaster = new Broadcaster(clientProxyFactory);
        var needler = new Needler();
        needler.Start(() => broadcaster.Broadcast(new Notification()));
        broadcaster.Register();
        needler.Stop();
        Assert.Null(needler.ExceptionFromThread);
    }

    [Fact]
    public void Broadcaster_register_start_register_stop_no_exception()
    {
        var clientProxyFactory = new TestClientProxyFactory();
        var broadcaster = new Broadcaster(clientProxyFactory);
        var needler = new Needler();
        broadcaster.Register();
        needler.Start(() => broadcaster.Broadcast(new Notification()));
        broadcaster.Register();
        needler.Stop();
        Assert.Null(needler.ExceptionFromThread);
    }
}
```
### The **Failing** Acid Test
```csharp
return
    from factory in "ClientProxyFactory".Stashed(() => new TestClientProxyFactory())
    from broadcaster in "Broadcaster".Stashed(() => new Broadcaster(factory))
    from needler in "Needler".Stashed(() => new Needler())
    from _ in "ops".Choose(
        // 1) Register
        from _a in "Register Client".Act(broadcaster.Register)
        from _s in "Client Exists In Collection".Spec(() =>
            GetBroadcastersClients(broadcaster).Contains(factory.CreatedClients.Last()))
        select Acid.Test,
        // 2) Remove on fault
        from faulty in "Faulty Client".Derived(
            Fuzz.ChooseFromWithDefaultWhenEmpty(GetBroadcastersClients(broadcaster)))
        from _b in "Registered Client Faults".ActIf(() => faulty != null,
            () => ((TestClientProxy)faulty!).Fault())
        from _sb in "Client Is Removed From Collection".Spec(() =>
            !GetBroadcastersClients(broadcaster).Contains(faulty))
        select Acid.Test,
        // 3) Start broadcast in background
        from _c in "Broadcast".ActIf(() => !needler.ThreadSwitch,
            () => needler.Start(() => broadcaster.Broadcast(new Notification())))
        from _sc in "Start Does Not Throw".Spec(() => needler.ExceptionFromThread == null)
        select Acid.Test,
        // 4) Stop broadcast
        from _d in "Stop Broadcasting".ActIf(() => needler.ThreadSwitch, needler.Stop)
        from _sd in "Stop Does Not Throw".Spec(() => needler.ExceptionFromThread == null)
        select Acid.Test)
    select Acid.Test;
```
### The Report
```
──────────────────────────────────────────────────
 Original failing run:    21 executions
 Minimal failing case:    6 executions (after 27 shrinks)
 Seed:                    1718881614
 ──────────────────────────────────────────────────
  Executed (16): Register Client
 ──────────────────────────────────────────────────
  Executed (17): Broadcast
 ──────────────────────────────────────────────────
  Executed (18): Register Client
 ──────────────────────────────────────────────────
  Executed (20): Stop Broadcasting
 ══════════════════════════════════════
  ❌ Spec Failed: Stop Does Not Throw
 ══════════════════════════════════════
 Passed Specs
  - Stop Does Not Throw: 4x
  - Client Exists In Collection: 8x
  - Start Does Not Throw: 4x
  - Client Is Removed From Collection: 4x
 ──────────────────────────────────────────────────
```
