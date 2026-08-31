# Human Review: Broiler.VM

GENERATED - DO NOT EDIT MANUALLY. Regenerate with
`BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release`, which rewrites this file,
`CODE-ASSURANCE.md`, `assurance.manifest.json` and every generated source header from the
product tree.

> **Status: PENDING.** Human-reviewed: 0 of 725 relevant units. No package
> may be published from this component, no RID claimed and no milestone accepted until every
> relevant unit carries a decision, which is update rule 8 in the status ledger.

## 1. How To Use This File

This section is the canonical mark legend for the component. The evidence bundles and the
status ledger link here rather than repeating the tables. There are two vocabularies, they are
different kinds of thing, and they must never be mixed. Both are closed sets, and rule H1
refuses a mark in any review document that this section does not publish.

### Evidence verdicts - stated about a piece of evidence

| Mark | Meaning |
|---|---|
| `[MET]` | Demonstrated. An execution, artefact or log in a retained bundle shows it. |
| `[PART]` | Partly demonstrated. What is not shown is named on the same row. |
| `[UNMET]` | Not discharged. The condition is stated and not satisfied. |
| `[N/A]` | Not claimed at this milestone. The milestone that owns it is named. |

### Review verdicts - stated in an evidence bundle about a gate clause

| Mark | Meaning |
|---|---|
| `[ ]` | Not yet read. |
| `[A]` | Accepted as stated. |
| `[C]` | Accepted with a condition. The condition is recorded beside it. |
| `[R]` | Rejected. The defect is recorded. |
| `[?]` | Cannot be judged from what is here. What is missing is named. |

**No verdict in this file is a mark.** A decision about a code unit is the
`// Broiler-Human:` line on that unit's declaration, and every table below is read out of
those lines. There is nothing here to fill in and nothing here to leave blank.

## 2. How A Review Is Recorded

In one place: the `// Broiler-Human:` line of the assurance annotation that sits on the
declaration being read. Nothing in this file is edited by hand, no second document carries a
per-item checklist, and no list of permitted aliases exists to be added to.

```csharp
// Broiler-AI:           Origin=AI; Spec=ADR-0007 s6; IP=Low; Security=High; Resources=7; Fingerprint=630EF7
// Broiler-Falsified-If: new T[] is reached before TryReserve returns true
// Broiler-Human:        PENDING
```

The last line has four shapes. A human writes three of them; the generator writes the fourth
and may never invent an alias, which rule J4 asserts in both directions.

| Line | Meaning |
|---|---|
| `PENDING` | Nobody has recorded a decision for this unit. The generator leaves it exactly as it stands. |
| `<alias>` | A human states their own alias and leaves the machine field to the generator, which fills it with the declaration's fingerprint at the next run. |
| `<alias>; Fingerprint=<six hex>` | A decision bound to one exact version of one declaration. |
| `STALE; Previous=<alias>@<fingerprint>` | Written by the generator when the code moved after a decision. Only a human clears it, by stating their alias again. |

A human may state their own `IP=`, `Security=` and `Resources=` assessment beside their alias,
which is how a reader disagrees with the machine assessment on the line above: an assessment is
a comment and moves no fingerprint, so there is nowhere else to say it.

**No branch, commit or tag is recorded in this file.** Each decision names the fingerprint of
the declaration it was made against, and the state machine compares that value with the
declaration as it now stands. A commit says a tree moved; a fingerprint says whether this unit
did, which is the narrower and the more useful of the two.

This file is produced on every pull request by the review lane in `.github/workflows/`, and the
publish lane refuses to run while any relevant unit is unresolved, any fingerprint is out of
date, any annotation is malformed or any generated artefact is stale.

## 3. Summary

| Metric | Value |
|---|---:|
| Files scanned | 50 |
| Code units | 1652 |
| Relevant | 725 |
| Exempt | 927 |
| Assessed | 725 of 725 (100%) |
| Human reviewed | 0 of 725 (0%) |
| Unverified | 725 |
| Aliases naming a decision | 0 |

## 4. Review States

One row per state of the machine that reads the two lines. The states are computed from the
annotations and the current fingerprints; nothing stores them.

| State | Units |
|---|---:|
| NEW | 0 |
| AI_ASSESSED | 0 |
| HUMAN_PENDING | 725 |
| HUMAN_APPROVED_PENDING_FINGERPRINT | 0 |
| VERIFIED | 0 |
| STALE | 0 |
| EXEMPT | 927 |

## 5. Aliases In The Tree

No alias appears on a human line anywhere in the product tree. Nobody has recorded a
decision about any unit of this component.

## 6. Coverage By File

One row per covered file, carrying that file's generated header. `Unverified` counts the
relevant units in a state that blocks a release.

| File | Units | Relevant | Exempt | Unverified | IP risk | Security risk | Criteria |
|---|---:|---:|---:|---:|---|---|---:|
| `src/Broiler.VM.Abstractions/VmArtifactDescriptor.cs` | 13 | 4 | 9 | 4 | Low | Low | 0/0 |
| `src/Broiler.VM.Abstractions/VmBudgetVocabulary.cs` | 38 | 14 | 24 | 14 | Low | Low | 0/0 |
| `src/Broiler.VM.Abstractions/VmControlResult.cs` | 24 | 12 | 12 | 12 | Low | Low | 0/0 |
| `src/Broiler.VM.Abstractions/VmCoreContract.cs` | 3 | 3 | 0 | 3 | None | Medium | 0/0 |
| `src/Broiler.VM.Abstractions/VmDescriptorValues.cs` | 38 | 14 | 24 | 14 | Low | Low | 0/0 |
| `src/Broiler.VM.Abstractions/VmDiagnostics.cs` | 120 | 36 | 84 | 36 | Low | Medium | 0/0 |
| `src/Broiler.VM.Abstractions/VmFeatureManifestId.cs` | 19 | 14 | 5 | 14 | Low | Medium | 0/0 |
| `src/Broiler.VM.Abstractions/VmGuestLoad.cs` | 31 | 11 | 20 | 11 | Low | Medium | 0/0 |
| `src/Broiler.VM.Abstractions/VmHostCapabilityDescriptor.cs` | 56 | 19 | 37 | 19 | Low | Medium | 0/0 |
| `src/Broiler.VM.Abstractions/VmIdentityPrimitives.cs` | 97 | 47 | 50 | 47 | Low | Medium | 0/0 |
| `src/Broiler.VM.Abstractions/VmLifecycleObjects.cs` | 66 | 23 | 43 | 23 | Low | Medium | 0/0 |
| `src/Broiler.VM.Abstractions/VmLimitPolicy.cs` | 34 | 18 | 16 | 18 | Low | Medium | 0/0 |
| `src/Broiler.VM.Abstractions/VmLimitVector.cs` | 51 | 27 | 24 | 27 | Low | Medium | 0/0 |
| `src/Broiler.VM.Abstractions/VmOutcome.cs` | 25 | 7 | 18 | 7 | Low | Medium | 0/0 |
| `src/Broiler.VM.Abstractions/VmProfileContracts.cs` | 72 | 39 | 33 | 39 | Low | High | 9/8 |
| `src/Broiler.VM.Abstractions/VmProfileDescriptor.cs` | 35 | 3 | 32 | 3 | None | Medium | 0/0 |
| `src/Broiler.VM.Abstractions/VmProfileId.cs` | 30 | 23 | 7 | 23 | Low | High | 9/7 |
| `src/Broiler.VM.Abstractions/VmReason.cs` | 95 | 8 | 87 | 8 | Low | Low | 0/0 |
| `src/Broiler.VM.Abstractions/VmStageResults.cs` | 106 | 65 | 41 | 65 | Low | Medium | 0/0 |
| `src/Broiler.VM.Abstractions/VmTransferTypes.cs` | 24 | 8 | 16 | 8 | Low | Low | 0/0 |
| `src/Broiler.VM.Abstractions/VmVerifiedArtifact.cs` | 55 | 17 | 38 | 17 | Low | Medium | 4/0 |
| `src/Broiler.VM.Binary/IVmBoundedAllocationMeter.cs` | 5 | 5 | 0 | 5 | Low | High | 3/3 |
| `src/Broiler.VM.Binary/VmBoundedAllocator.cs` | 3 | 3 | 0 | 3 | Low | High | 3/3 |
| `src/Broiler.VM.Binary/VmBoundedReadStatus.cs` | 10 | 1 | 9 | 1 | Low | Low | 0/0 |
| `src/Broiler.VM.Binary/VmBoundedReader.cs` | 36 | 22 | 14 | 22 | Low | High | 20/19 |
| `src/Broiler.VM.Binary/VmReadBounds.cs` | 11 | 3 | 8 | 3 | Low | Low | 0/0 |
| `src/Broiler.VM.Binary/VmSectionFrame.cs` | 10 | 3 | 7 | 3 | Low | Low | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/AssemblyMarker.cs` | 1 | 1 | 0 | 1 | None | None | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/AssemblyMarker.cs` | 1 | 1 | 0 | 1 | None | None | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/AssemblyMarker.cs` | 1 | 1 | 0 | 1 | None | None | 0/0 |
| `src/Broiler.VM.Runtime/VmAggregateBudget.cs` | 42 | 23 | 19 | 23 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmArtifactLoadMediator.cs` | 17 | 6 | 11 | 6 | Low | Medium | 1/0 |
| `src/Broiler.VM.Runtime/VmBudgetLevel.cs` | 15 | 11 | 4 | 11 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmCanonicalCatalogEncoding.cs` | 5 | 5 | 0 | 5 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmCapabilityBinding.cs` | 22 | 10 | 12 | 10 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmCatalog.cs` | 47 | 24 | 23 | 24 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmCatalogValidation.cs` | 47 | 5 | 42 | 5 | Low | Low | 0/0 |
| `src/Broiler.VM.Runtime/VmCeilingResolution.cs` | 4 | 4 | 0 | 4 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmDescriptorValidation.cs` | 10 | 10 | 0 | 10 | Low | Low | 0/0 |
| `src/Broiler.VM.Runtime/VmExecutionScope.cs` | 26 | 16 | 10 | 16 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmInstanceImplementation.cs` | 40 | 24 | 16 | 24 | Low | High | 6/2 |
| `src/Broiler.VM.Runtime/VmInstantiation.cs` | 15 | 9 | 6 | 9 | Low | Medium | 2/0 |
| `src/Broiler.VM.Runtime/VmLimitPrecedence.cs` | 3 | 3 | 0 | 3 | Low | High | 1/1 |
| `src/Broiler.VM.Runtime/VmMeter.cs` | 38 | 20 | 18 | 20 | Low | Medium | 10/0 |
| `src/Broiler.VM.Runtime/VmOperation.cs` | 54 | 25 | 29 | 25 | Low | Medium | 1/0 |
| `src/Broiler.VM.Runtime/VmProfileRuntimeState.cs` | 15 | 6 | 9 | 6 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmRuntime.cs` | 57 | 34 | 23 | 34 | Low | High | 10/2 |
| `src/Broiler.VM.Runtime/VmRuntimeCreationOptions.cs` | 55 | 22 | 33 | 22 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmRuntimeState.cs` | 18 | 8 | 10 | 8 | Low | Low | 0/0 |
| `src/Broiler.VM.Runtime/VmVerification.cs` | 12 | 8 | 4 | 8 | Low | High | 3/3 |

## 7. Decisions Recorded

No unit in this component carries a decision on its human line. Every one of them reads
`PENDING`.

## 8. Decisions The Code Has Outrun

No unit carries a decision that the code has since moved past.

## 9. Where A Decision Is Required First

The units at the top of the security vocabulary, with the observation that would show each
one wrong and the human line it carries. The set is read from the assessments rather than
written out, so a unit that becomes `High` joins it at the next generation.

- `Broiler.VM.IVmVerifiedState` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, Spec=none cited, `9B3EE1`, PENDING
  - Falsified if: the core calls anything on a stored state, or a state reachable from a shared handle can be mutated
- `Broiler.VM.IVmInstanceState` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, Spec=none cited, `FC8DAD`, PENDING
  - Falsified if: the core reads an instance state, or one reaches an executor other than the profile that made it
- `Broiler.VM.IVmProfileContinuation` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, Spec=none cited, `9DB83C`, PENDING
  - Falsified if: the core inspects a continuation, or one is handed back to an operation it was not captured from
- `Broiler.VM.VmInvocationRequest` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, Spec=none cited, `B2651B`, PENDING
  - Falsified if: the core decodes, re-encodes or trims the entry-point bytes rather than carrying them verbatim
- `Broiler.VM.IVmProfileVerifier` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, Spec=none cited, `8ED829`, PENDING
  - Falsified if: a verifier whose declared identity differs from the descriptor naming it is admitted to a catalog
- `Broiler.VM.IVmProfileVerifier.Verify(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, IVmVerificationContext, System.Threading.CancellationToken)` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, Spec=ADR-0006 s6, `ED6BA8`, PENDING
  - Falsified if: the payload arrives as anything but a span, or a second member here can answer a verification
- `Broiler.VM.VmExecutionStepKind` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, Spec=none cited, `15A717`, PENDING
  - Falsified if: a member's numeric value changes, or a kind exists the core's step-to-stage mapping has no arm for
- `Broiler.VM.VmExecutorFactory` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, Spec=none cited, `CC8727`, PENDING
  - Falsified if: an executor is created on a path that does not instantiate, or its type is rooted by reflection
- `Broiler.VM.VmProfileId` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, Spec=none cited, `CB7A1C`, PENDING
  - Falsified if: Equals, ==, CompareTo or GetHashCode folds case, or a stored id is not the text the caller supplied
- `Broiler.VM.VmProfileId.TryParse(System.ReadOnlySpan<char>, out VmProfileId)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, Spec=ADR-0002 s1, `745304`, PENDING
  - Falsified if: an id is returned for a candidate the grammar rejects, or it stores anything but the candidate
- `Broiler.VM.VmProfileId.Parse(System.ReadOnlySpan<char>)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, Spec=ADR-0002 s1, `AA7040`, PENDING
  - Falsified if: it accepts a candidate TryParse refuses, or its message names bounds the constants above do not
- `Broiler.VM.VmProfileId.TryValidateGrammar(System.ReadOnlySpan<char>, int, int, int, int, out byte)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, Spec=ADR-0002 s1, `A4118A`, PENDING
  - Falsified if: a label count, label length or total length outside its bound validates, or an empty label does
- `Broiler.VM.VmProfileId.TryValidate(System.ReadOnlySpan<char>, out byte)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, Spec=ADR-0002 s1, `7D6A16`, PENDING
  - Falsified if: it passes bounds other than the five constants above, or the label count is not dots plus one
- `Broiler.VM.VmProfileId.IsAsciiLetter(char)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, Spec=ADR-0002 s1, `C1992B`, PENDING
  - Falsified if: true is returned for a character outside A-Z and a-z - try the neighbours '@', '[', '`' and '{'
- `Broiler.VM.VmProfileId.IsAsciiAlphanumeric(char)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, Spec=ADR-0002 s1, `204317`, PENDING
  - Falsified if: it accepts a non-ASCII letter or digit, or rejects one of 0-9; the neighbours are '/' and ':'
- `Broiler.VM.IVmBoundedAllocationMeter` in `src/Broiler.VM.Binary/IVmBoundedAllocationMeter.cs` - Security=High, Spec=none cited, `A95709`, PENDING
  - Falsified if: a member that can refuse has no way to say so in its return value, so refusal must be thrown
- `Broiler.VM.IVmBoundedAllocationMeter.TryReserve(ulong)` in `src/Broiler.VM.Binary/IVmBoundedAllocationMeter.cs` - Security=High, Spec=none cited, `612753`, PENDING
  - Falsified if: a false return has already charged the allowance, or a true one reserves nothing
- `Broiler.VM.IVmBoundedAllocationMeter.TryChargeWork(ulong)` in `src/Broiler.VM.Binary/IVmBoundedAllocationMeter.cs` - Security=High, Spec=none cited, `E14B16`, PENDING
  - Falsified if: false is returned here for a cancellation, which the caller latches as a spent work allowance
- `Broiler.VM.VmBoundedAllocator` in `src/Broiler.VM.Binary/VmBoundedAllocator.cs` - Security=High, Spec=none cited, `30CDFD`, PENDING
  - Falsified if: a member here allocates without taking both a bounds value and a meter
- `Broiler.VM.VmBoundedAllocator.TryAllocate<T>(in VmReadBounds, IVmBoundedAllocationMeter, uint, out T[])` in `src/Broiler.VM.Binary/VmBoundedAllocator.cs` - Security=High, Spec=ADR-0007 s6, `630EF7`, PENDING
  - Falsified if: the count is sized before its bound comparison, or the element-size product is not checked
- `Broiler.VM.VmBoundedAllocator.TryAllocateExact<T>(in VmReadBounds, IVmBoundedAllocationMeter, ulong, out T[])` in `src/Broiler.VM.Binary/VmBoundedAllocator.cs` - Security=High, Spec=ADR-0007 s6, `5185F6`, PENDING
  - Falsified if: new T[] is reached before TryReserve returns true, or a failed allocation keeps its reservation
- `Broiler.VM.VmBoundedReader` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `CCF177`, PENDING
  - Falsified if: a public member examines bytes or advances position while Status is not Ok
- `Broiler.VM.VmBoundedReader.VmBoundedReader(System.ReadOnlySpan<byte>, in VmReadBounds, IVmBoundedAllocationMeter)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `CB0FB0`, PENDING
  - Falsified if: it forwards a granularity other than 1, so an existing caller's poll cadence changes
- `Broiler.VM.VmBoundedReader.VmBoundedReader(System.ReadOnlySpan<byte>, in VmReadBounds, IVmBoundedAllocationMeter, ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=ADR-0007, `83214B`, PENDING
  - Falsified if: a source longer than MaxArtifactBytes leaves Status Ok, or a granularity below 1 stops the reader polling at all
- `Broiler.VM.VmBoundedReader.Remaining` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `D3559E`, PENDING
  - Falsified if: position can exceed bytes.Length, so the subtraction wraps to a remainder larger than the span
- `Broiler.VM.VmBoundedReader.TryReadByte(out byte)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `EADE4A`, PENDING
  - Falsified if: bytes is indexed on a path where TryConsume(1) returned false, or the (int) index leaves the span
- `Broiler.VM.VmBoundedReader.TryReadUInt32LittleEndian(out uint)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `E8E77F`, PENDING
  - Falsified if: a window shorter than four bytes reaches the shifts, or the assembly is not little-endian
- `Broiler.VM.VmBoundedReader.TryReadUInt64LittleEndian(out ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `BF1BC9`, PENDING
  - Falsified if: a window shorter than eight bytes reaches the loop, or the descending loop is not little-endian
- `Broiler.VM.VmBoundedReader.TryReadVarUInt32(out uint)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `A13073`, PENDING
  - Falsified if: two distinct byte sequences both return true with the same value, or the (uint) cast drops bits
- `Broiler.VM.VmBoundedReader.TryReadVarUInt64(out ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `69F550`, PENDING
  - Falsified if: two distinct byte sequences both return true with one value; shift 63 is the case to try
- `Broiler.VM.VmBoundedReader.TryReadDeclaredCount(out uint)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `D8A056`, PENDING
  - Falsified if: a count is returned before its comparison with MaxDeclaredCount, or no path here calls TryReserve
- `Broiler.VM.VmBoundedReader.TryReadBytes(ulong, out System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `58DE6E`, PENDING
  - Falsified if: window is set on a path where TryTake returned false, or its length is not the length asked for
- `Broiler.VM.VmBoundedReader.TryEnterSection(ulong, out VmSectionFrame)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `2E1BF5`, PENDING
  - Falsified if: a frame is minted before the length, section-count and depth bounds have all been compared
- `Broiler.VM.VmBoundedReader.TryExitSection(in VmSectionFrame)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `D4F2B9`, PENDING
  - Falsified if: a frame this reader never minted reaches here and Start + DeclaredLength wraps
- `Broiler.VM.VmBoundedReader.TrySkipSectionBody(in VmSectionFrame)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `E174C9`, PENDING
  - Falsified if: the end sum wraps past the bytes.Length test, or position advances on a refused ChargeWork
- `Broiler.VM.VmBoundedReader.TryChargeWork(ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `D3A8E1`, PENDING
  - Falsified if: the meter is charged while Status is not Ok, so a spent reader keeps spending the allowance
- `Broiler.VM.VmBoundedReader.TryTake(ulong, out System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `6D9975`, PENDING
  - Falsified if: the (int) casts narrow an index or length TryConsume allowed, so the slice leaves the span
- `Broiler.VM.VmBoundedReader.TryConsume(ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `7DE9F2`, PENDING
  - Falsified if: position advances past a failed bound test or a refused ChargeWork, or the addition is unchecked
- `Broiler.VM.VmBoundedReader.TryReadVarUInt64Core(int, out ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `DE9CB5`, PENDING
  - Falsified if: an over-long encoding is accepted: a group past maxBits, an overflowing tail, a zero continuation
- `Broiler.VM.VmBoundedReader.ChargeWork(ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, Spec=none cited, `04F760`, PENDING
  - Falsified if: WorkBudgetExhausted is latched for a Poll that returned false under cancellation, not exhaustion, or a charge is batched, or work accumulates past the granularity without a poll
- `Broiler.VM.VmInstanceImplementation.Dispose(System.TimeSpan)` in `src/Broiler.VM.Runtime/VmInstanceImplementation.cs` - Security=High, Spec=ADR-0004, `3B4D81`, PENDING
  - Falsified if: disposal returns while stepsInFlight is above zero and still releases the lease
- `Broiler.VM.VmInstanceImplementation.LeaveStep()` in `src/Broiler.VM.Runtime/VmInstanceImplementation.cs` - Security=High, Spec=ADR-0004, `E8A45C`, PENDING
  - Falsified if: a step returns without decrementing, so a later disposal waits its whole budget
- `Broiler.VM.VmLimitPrecedence.TryApply(VmBudgetScope, ulong[], VmLimitOverrides, out ulong[], out VmBudgetDimension, out VmReason)` in `src/Broiler.VM.Runtime/VmLimitPrecedence.cs` - Security=High, Spec=ADR-0007, `C06726`, PENDING
  - Falsified if: a refused set leaves one dimension of the caller's inherited array changed
- `Broiler.VM.VmRuntime` in `src/Broiler.VM.Runtime/VmRuntime.cs` - Security=High, Spec=none cited, `C1CA36`, PENDING
  - Falsified if: a second member verifies bytes into a handle, or a member here returns a task or awaitable
- `Broiler.VM.VmRuntime.Verify(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, System.Threading.CancellationToken)` in `src/Broiler.VM.Runtime/VmRuntime.cs` - Security=High, Spec=none cited, `D50EA5`, PENDING
  - Falsified if: a call from inside a bound non-reentrant capability is refused, though the record permits it
- `Broiler.VM.VmRuntime` in `src/Broiler.VM.Runtime/VmVerification.cs` - Security=High, Spec=none cited, `910045`, PENDING
  - Falsified if: a guest-initiated load is admitted while a profile verifier frame is on the stack
- `Broiler.VM.VmRuntime.VerifyCore(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, System.Threading.CancellationToken, VmDiagnostics, VmArtifactOrigin, VmMeter?)` in `src/Broiler.VM.Runtime/VmVerification.cs` - Security=High, Spec=none cited, `4166AF`, PENDING
  - Falsified if: cancellation is decided after an input is examined, or an unknown profile answers InvalidArtifact
- `Broiler.VM.VmRuntime.RunVerifier(VmProfileDescriptor, in VmArtifactDescriptor, System.ReadOnlySpan<byte>, System.Threading.CancellationToken, VmDiagnostics, VmArtifactOrigin, VmMeter?)` in `src/Broiler.VM.Runtime/VmVerification.cs` - Security=High, Spec=none cited, `D433B9`, PENDING
  - Falsified if: an escaping verifier exception is answered as a category, or both effective ceilings are one vector, or a cancelled or poll-bound-violating verification is answered as resource exhaustion

## 10. What This Record Does Not Say

It is not an approval of the component, and a full table above would not be one either. It
records which declarations somebody stated a decision about, and against which version of
each. It does not record what they read, how long they spent, or whether they were right.

Broiler.VM has one person in every role: architecture owner, core-contract owner, security
owner and reader are the same individual, so **no second pair of eyes has seen this work.**
That is a property of the project's size rather than a defect in this component, and it is why
the tables above have room for as many aliases as the tree names rather than one signature
line.

A fingerprint is six hex characters of SHA-256 over a declaration's token texts. It answers
whether a unit changed since a decision was recorded against it. It is not a collision-free
identifier across units and it is not a cryptographic commitment, so it detects a change and
does not resist a forger with commit access.

The assessments the decisions are recorded beside are machine-written and unread: an
assessment is a comment, so downgrading one moves no fingerprint anywhere, which exclusions
EX-65 and EX-76 record.

That is not a figure of speech. 725 of the 725 assessed units declare
`Origin=AI`, and the records this component implements were drafted the same way. An
adversarial pass over the work confirmed findings and they were corrected, which is a check
on it and not an independent judgement of it. Reading a declaration is the only thing that
makes it read.
