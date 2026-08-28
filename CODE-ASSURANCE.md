# Broiler.VM Code Assurance

GENERATED - DO NOT EDIT MANUALLY. Regenerate with
`BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release`, which rewrites this file,
`HUMAN_REVIEW.md`, `assurance.manifest.json` and every generated source header from the
product tree.

**Nothing in this component has been reviewed by a human.** This report records that
absence precisely. It is not a claim that the code is reviewed, assured or safe, and the
figures below are the measurement of how far from that claim the component is.

## Summary

| Metric | Value |
|---|---:|
| Files scanned | 45 |
| Files carrying an annotation | 45 |
| Code units | 1592 |
| Relevant | 689 |
| Exempt by predicate | 903 |
| Annotated | 689 of 689 (100%) |
| Human reviewed | 0 of 689 (0%) |
| Unverified | 689 |

## Review states

| State | Count |
|---|---:|
| NEW | 0 |
| AI_ASSESSED | 0 |
| HUMAN_PENDING | 689 |
| HUMAN_APPROVED_PENDING_FINGERPRINT | 0 |
| VERIFIED | 0 |
| STALE | 0 |
| EXEMPT | 903 |

## IP risk

| Value | Units |
|---|---:|
| None | 21 |
| Low | 668 |
| Medium | 0 |
| High | 0 |
| Unknown | 0 |
| *not annotated* | 0 |

## Security risk

| Value | Units |
|---|---:|
| None | 2 |
| Low | 385 |
| Medium | 258 |
| High | 44 |
| Critical | 0 |
| *not annotated* | 0 |

## Resource impact

| Metric | Value |
|---|---:|
| Maximum | 8 / 10 |
| Average over annotated units | 0.7 / 10 |
| Units scored | 689 |

## High-security review areas

- `Broiler.VM.IVmVerifiedState` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmInstanceState` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmProfileContinuation` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.VmInvocationRequest` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmProfileVerifier` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmProfileVerifier.Verify(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, IVmVerificationContext, System.Threading.CancellationToken)` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.VmExecutionStepKind` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.VmExecutorFactory` in `src/Broiler.VM.Abstractions/VmProfileContracts.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId.TryParse(System.ReadOnlySpan<char>, out VmProfileId)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId.Parse(System.ReadOnlySpan<char>)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId.TryValidateGrammar(System.ReadOnlySpan<char>, int, int, int, int, out byte)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId.TryValidate(System.ReadOnlySpan<char>, out byte)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId.IsAsciiLetter(char)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.VmProfileId.IsAsciiAlphanumeric(char)` in `src/Broiler.VM.Abstractions/VmProfileId.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmBoundedAllocationMeter` in `src/Broiler.VM.Binary/IVmBoundedAllocationMeter.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmBoundedAllocationMeter.TryReserve(ulong)` in `src/Broiler.VM.Binary/IVmBoundedAllocationMeter.cs` - Security=High, human line PENDING
- `Broiler.VM.IVmBoundedAllocationMeter.TryChargeWork(ulong)` in `src/Broiler.VM.Binary/IVmBoundedAllocationMeter.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedAllocator` in `src/Broiler.VM.Binary/VmBoundedAllocator.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedAllocator.TryAllocate<T>(in VmReadBounds, IVmBoundedAllocationMeter, uint, out T[])` in `src/Broiler.VM.Binary/VmBoundedAllocator.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedAllocator.TryAllocateExact<T>(in VmReadBounds, IVmBoundedAllocationMeter, ulong, out T[])` in `src/Broiler.VM.Binary/VmBoundedAllocator.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.VmBoundedReader(System.ReadOnlySpan<byte>, in VmReadBounds, IVmBoundedAllocationMeter)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.Remaining` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadByte(out byte)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadUInt32LittleEndian(out uint)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadUInt64LittleEndian(out ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadVarUInt32(out uint)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadVarUInt64(out ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadDeclaredCount(out uint)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadBytes(ulong, out System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryEnterSection(ulong, out VmSectionFrame)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryExitSection(in VmSectionFrame)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TrySkipSectionBody(in VmSectionFrame)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryChargeWork(ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryTake(ulong, out System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryConsume(ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.TryReadVarUInt64Core(int, out ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmBoundedReader.ChargeWork(ulong)` in `src/Broiler.VM.Binary/VmBoundedReader.cs` - Security=High, human line PENDING
- `Broiler.VM.VmRuntime` in `src/Broiler.VM.Runtime/VmRuntime.cs` - Security=High, human line PENDING
- `Broiler.VM.VmRuntime.Verify(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, System.Threading.CancellationToken)` in `src/Broiler.VM.Runtime/VmRuntime.cs` - Security=High, human line PENDING
- `Broiler.VM.VmRuntime` in `src/Broiler.VM.Runtime/VmVerification.cs` - Security=High, human line PENDING
- `Broiler.VM.VmRuntime.VerifyCore(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, System.Threading.CancellationToken, VmDiagnostics, VmArtifactOrigin, VmMeter?)` in `src/Broiler.VM.Runtime/VmVerification.cs` - Security=High, human line PENDING
- `Broiler.VM.VmRuntime.RunVerifier(VmProfileDescriptor, in VmArtifactDescriptor, System.ReadOnlySpan<byte>, System.Threading.CancellationToken, VmDiagnostics, VmArtifactOrigin, VmMeter?)` in `src/Broiler.VM.Runtime/VmVerification.cs` - Security=High, human line PENDING

## Falsification criteria

| Metric | Value |
|---|---:|
| Units carrying a criterion | 69 |
| Units required to carry one | 44 |
| Required and missing | 0 |

A `Broiler-Falsified-If:` line states, at the declaration, the observation that would make
the unit wrong. `Security=High` says a unit is risky, which is a set and not a test; the
criterion is the test. It is required where `Security` is `High` or `Critical`, permitted
elsewhere, and rule J10 names every unit that owes one and carries none.

The line is a comment, so it is outside every fingerprint by construction: rewording a
criterion moves no recorded value here, in a file header or in
`assurance.manifest.json`, and invalidates nothing. That is the intended reading - a
criterion is an instruction to whoever reads the unit, not part of what a review is bound to.

This third line is a local extension. The owner's policy defines two lines and not three,
and it is added here because the two cannot carry a falsification criterion at all, and
because the line numbers a separate worksheet cited rotted the moment the annotations moved
the code: an annotation travels with its declaration and a citation does not. Exclusion
EX-74 records that this is an extension to the policy rather than an implementation of it,
and that the owner may reject it.

## Exemption

Exemption is decided by one predicate in `AssuranceScanner.ExemptionFor`, not per unit, so
that the rule is reviewable in one place rather than in several hundred.

| Case | Units |
|---|---:|
| TrivialPropertyOrAccessor | 324 |
| ParameterAssigningConstructor | 63 |
| TrivialExpressionBodiedMember | 16 |
| CompilerSuppliedRecordOrEnumMember | 0 |
| DelegatingOverrideOrOperator | 88 |
| InsideAssemblyMarker | 0 |
| FieldDeclaringStorage | 135 |
| EnumMemberOfADeclaredVocabulary | 277 |
| DeclaredInSource | 0 |

## Per-unit exemptions

| Metric | Value |
|---|---:|
| Per-unit exemptions | 0 |

A per-unit `EXEMPT=<reason>` line exempts one unit by a reason a human wrote, for what the
predicate cannot see. Nothing mechanical checks that the reason is true, that it describes
the unit it sits on, or that it says anything at all, so every use is counted and named
here. `Broiler.VM.Binary` is closed to it entirely: that assembly reads untrusted
input, and a unit there is assessed or it is not shipped. Rule J1 asserts both halves.

No unit in this component states a per-unit exemption.

## Change detection

`assurance.manifest.json` lists **every** code unit in the three product assemblies -
1592 of them, exempt and relevant alike - with the fingerprint of its declaration.
This manifest is a change-detection record, not a review. A unit listed there is watched, not reviewed:
the entry records what the declaration's tokens hashed to when the generator last ran, and
nothing else. Exempt units still need no annotation and carry none, and no human line in
this component has moved off `PENDING`. What the manifest adds is that a unit the exemption
predicate treats as trivial is no longer invisible: a semantic change to one moves a value
in a generated file the gate compares byte for byte. Rule J7 holds the manifest to the tree.

Beside the units it lists **every covered file** - 45 of them - with a
fingerprint over the complete token stream of its compilation unit. A unit entry exists only
for a declaration kind the scanner enumerates, and an enumeration is a whitelist: an
`[assembly: ...]` attribute is a member of nothing and can be in no unit at all.
Nothing in a covered file can change without something moving here, whatever kind of declaration it is. Comments are outside the stream, because a token's
text is its own characters, so the generated header above and the annotation lines below move
no file fingerprint - which is what lets one generation be a fixed point.

## Verification

The generator and the gate are the same code, run as a test in the architecture suite. Two
lanes under `.github/workflows/` compel it rather than leaving it to whoever remembers: the
review lane regenerates every artefact on a pull request and commits what moved, and the
publish lane runs the release mode below and refuses to pack while anything is unresolved.
Exclusion EX-45 still records one RID and one machine for the Native AOT evidence, which no
lane reproduces.

| Mode | Command | Effect |
|---|---|---|
| Generate | `BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release` | Fills every `Fingerprint=TBF`, refreshes a decision the code has outrun into `STALE; Previous=...`, rewrites the generated headers, `HUMAN_REVIEW.md`, `assurance.manifest.json` and this file. |
| Gate | `dotnet test Broiler.VM.slnx -c Release` | Asserts every generated artefact is byte-identical to what the generator would produce. |
| Release | `BROILER_ASSURANCE_RELEASE=1 dotnet test Broiler.VM.slnx -c Release` | The gate, and additionally: no relevant unit left in a state that blocks a release, no annotation this system cannot read, no fingerprint out of date, no unit at the top of the security vocabulary without a criterion. |

The fingerprint is six hex characters - 24 bits - of SHA-256 over the declaration's token
texts, joined by single spaces. Trivia is excluded because a token's text is its own
characters and never the comments or whitespace around it, so `dotnet format` moves no
fingerprint and an annotation is never part of what it describes. The value answers whether a
unit changed since it was reviewed. It is not a collision-free identifier across units and it
is not a cryptographic commitment.
