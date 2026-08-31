# JSD-0004 - The fifteen limit defaults, the fifteen hard maxima, and the budget declaration matrix

**Status:** Accepted for JS-0 as the **intended** declaration. Four numbers are marked
`MEASURED` and are not chosen by this record. Nothing here is in a descriptor: JS-1 lands the
vectors and may correct them with a dated record, but may not drift.

**Date:** 2026-08-31

**Owner:** profile architecture owner. **Milestone:** JS-0.

## Why this is written before there is a descriptor

A matrix nobody wrote down is a matrix assembled from whatever made the catalog stop refusing.
The declaration matrix has no default row - a dimension this profile does not charge must say
`NotApplicable`, and the catalog checks that answer against the structural consequences of the
rest of the descriptor - so the answers have to be reasoned about once, in a place a reviewer can
read, rather than discovered one refusal at a time.

## The two vectors say different things, and one of them reaches other profiles

**A maximum is a statement about this profile.** It is the most this profile would tolerate a
host granting, and it binds **this profile's own artifacts and nobody else's**: verification
intersects the host's ceiling with the maxima of the profile the artifact names. A neighbour's
maxima do not reach this profile's artifacts and this profile's do not reach a neighbour's. That
was not always true - the core clamped every runtime ceiling to the tightest maximum in the
catalog until the defect was removed on 2026-08-31 - and the correction is why this record does
not carry the obligation an earlier draft of the roadmap did, to publish an unconstrained maximum
on a dimension declared inapplicable.

**A default is a statement about this profile's neighbours.** A host that adopts profile defaults
rather than stating numbers gets the **tightest default in the catalog, per dimension**, because
at runtime creation no profile has been selected and there is no other safe answer. So a stingy
default on a dimension this profile barely uses is what constrains a profile composed beside it.

Two consequences are binding on the numbers below. **A default is chosen as what this profile
actually needs, never as a way of expressing an opinion about a dimension it does not use** - a
zero there is a claim about every neighbour that adopts defaults. And **reconciling two profiles'
defaults belongs to whichever component composes both**; that component does not exist and the
status ledger carries it as an unopened dependency with no holder.

A profile's defaults may not be `Unconstrained`; a profile's maxima may be.

## The declaration matrix and the two vectors

`Charged` / `NotApplicable` is the matrix. Every dimension is charged: this profile executes
guest code, allocates, calls hosts, frames artifacts and declares guest-initiated loads, so
there is no dimension it can honestly disclaim. That is a different answer from the calculator
consumer profile's, which disclaims five, and the difference is the point of the matrix.

| Dimension | Matrix | Default | Hard maximum | Why |
|---|---|---|---:|---|
| `Fuel` | Charged | 50,000,000 | 1,099,511,627,776 | Every instruction dispatched, plus the proportional families. A page's script does real work; the default is a script budget, not a calculator's |
| `WallClock` | Charged | 10,000 ms | 3,600,000 ms | Core-metered; this profile polls often enough for it to bite |
| `AllocatedBytes` | Charged | 67,108,864 | 4,294,967,296 | Verification buffers, constant pool, environments, objects, and the storage layer's transitions |
| `LiveBytes` | Charged | 33,554,432 | 2,147,483,648 | Retained realm and heap state, reported on retention and released on instance disposal |
| `HostCalls` | Charged | 1,000,000 | 4,294,967,295 | Every call into an imported capability, including every artifact-provider request |
| `CallDepth` | Charged | **MEASURED** | **MEASURED** | Every interpreter frame. See below - this number is derived from a measurement, not chosen |
| `VerifierWork` | Charged | 100,000,000 | 1,099,511,627,776 | Required by the catalog. Decode, structural validation, and the static-semantic stage |
| `ArtifactBytes` | Charged | 33,554,432 | 536,870,912 | Enforced by the core's reader over the payload |
| `SectionCount` | Charged | 64 | 1,024 | This format's sections are literal and framed, so the dimension has a direct referent |
| `DeclaredCount` | Charged | 4,194,304 | 4,294,967,295 | Constant-pool, code, exception-region and position-table counts, through the guarded count reader |
| `StructuralDepth` | Charged | 256 | 4,096 | Section and exception-region nesting inside one artifact. **Ceiling-class**: charged on entry, released on exit |
| `NestedLoadDepth` | Charged | 4 | 64 | Declared because this profile declares guest-initiated loads. At core contract version 1 nesting is bounded at one by construction; the default is not 1, for the reason below |
| `NestedLoadFanOut` | Charged | 4,096 | 16,777,216 | Every admitted mediator request. **This is the counter an `eval` chain actually consumes** |
| `NestedLoadBytes` | Charged | 16,777,216 | 536,870,912 | Provider-returned bytes for one operation |
| `LiveRuntimes` | Charged | 64 | 4,096 | Core-metered; the agent model adds nothing beyond it |

**`NestedLoadDepth` is deliberately not 1.** At core contract version 1 a nested load hands back
a verified handle with no path to a nested core instantiation and a provider is mandatorily
non-reentrant, so nesting is bounded at one by construction and a default of 1 would cost this
profile nothing. It would cost a **neighbour** that adopts defaults everything, because the
tightest default in the catalog is what such a host gets. Four is the smallest number that leaves
room for a neighbour with a real module graph, and this is exactly the reasoning the section
above requires the numbers to obey.

**Allowances accumulate; ceilings occupy.** Seven dimensions are allowance-class and are consumed
monotonically with no refund; the other eight are ceiling-class, bound a live measure, and are
reported as retained and released. A depth counter is therefore an ordinary refusable high-water
bound and needs no amendment to express.

**No budget refusal is guest-observable on the current contract, and this profile must not design
one that assumes otherwise.** A refused charge at any scope latches exhaustion on the meter and
the core rewrites the completed step as `ResourceExhaustion` whatever the profile does with the
`false` it was handed; a ceiling-class retention report returns nothing at all. This profile has
no construct that needs a catchable budget refusal - a JavaScript allocation failure is a
host-level condition, not a value the language reads back - and that is a property to preserve
rather than a coincidence to rely on.

## Four numbers are measured, not chosen

`CallDepth`, `MaxUnchargedWork`, `ChargingGranularity` and `CancellationPollBound` are each
derived from a retained, reproducible measurement and recorded with it. Writing a round figure
here would be inventing the answer the measurement exists to produce.

**`CallDepth` is the sharp one.** A recursing program must be refused as `ResourceExhaustion`
naming `CallDepth`, on every claimed RID, under Native AOT - **rather than terminating the
process**. A stack overflow is not translatable into a result, so claiming to handle deep
recursion without a measured bound would be an untruthful capability claim. The default is
derived from a measured native frame cost per interpreter frame on each claimed RID, and a
recursion case proves the refusal on each. **JS-5 owns the measurement and the number**; JS-1
marks the descriptor row provisional and names JS-5.

## What this record does not decide

- **No descriptor carries these values.** They are in no assembly: JS-0 lands no product code,
  and a vector written into an assembly nothing reads is a claim with no gate over it. JS-1 lands
  them with the descriptor that uses them, and the two-profile catalog test JS-0's exit gate
  names is carried to JS-1 with them.
- **The proportional charging families are not enumerated here.** The rule that work be charged
  as a monotone non-decreasing function of its input, at least the ceiling of that function over
  the declared granularity, is the core's obligation CO-1 in ADR 0007. What this profile owns is
  the family list, the functions and the fixtures, and JS-5 owns them: string concatenation and
  comparison, array copy and sort, property enumeration, regular-expression matching, numeric
  conversion of large values, structured cloning. **An operation family without a proportionality
  fixture does not ship in the increment.**
