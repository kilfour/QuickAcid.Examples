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
