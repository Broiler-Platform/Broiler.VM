# Human Review: Broiler.VM

GENERATED - DO NOT EDIT MANUALLY. Regenerate with
`BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release`, which rewrites this file,
`CODE-ASSURANCE.md`, `assurance.manifest.json` and every generated source header from the
product tree.

> **Status: PENDING.** Human-reviewed: 0 of 2612 relevant units. No package
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
| Files scanned | 121 |
| Code units | 4734 |
| Relevant | 2612 |
| Exempt | 2122 |
| Assessed | 2612 of 2612 (100%) |
| Human reviewed | 0 of 2612 (0%) |
| Unverified | 2612 |
| Aliases naming a decision | 0 |

## 4. Review States

One row per state of the machine that reads the two lines. The states are computed from the
annotations and the current fingerprints; nothing stores them.

| State | Units |
|---|---:|
| NEW | 0 |
| AI_ASSESSED | 0 |
| HUMAN_PENDING | 2612 |
| HUMAN_APPROVED_PENDING_FINGERPRINT | 0 |
| VERIFIED | 0 |
| STALE | 0 |
| EXEMPT | 2122 |

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
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` | 249 | 166 | 83 | 166 | None | High | 7/6 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` | 141 | 124 | 17 | 124 | None | High | 2/2 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsSyntax.cs` | 91 | 78 | 13 | 78 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` | 7 | 7 | 0 | 7 | None | High | 3/3 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` | 61 | 6 | 55 | 6 | None | High | 4/4 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceControlFlow.cs` | 8 | 8 | 0 | 8 | None | Medium | 4/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceLowering.cs` | 22 | 22 | 0 | 22 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParseOptions.cs` | 14 | 9 | 5 | 9 | None | High | 3/2 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` | 65 | 59 | 6 | 59 | None | High | 20/20 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceProgramBuilder.cs` | 35 | 24 | 11 | 24 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` | 33 | 24 | 9 | 24 | None | High | 17/17 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceDiagnostics.cs` | 30 | 3 | 27 | 3 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourcePrograms.cs` | 11 | 10 | 1 | 10 | None | High | 6/4 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` | 40 | 24 | 16 | 24 | None | High | 14/14 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSyntax.cs` | 29 | 26 | 3 | 26 | None | Medium | 1/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` | 140 | 40 | 100 | 40 | None | High | 22/18 |
| `src/Broiler.VM.Profile.JavaScript.Format/AssemblyMarker.cs` | 1 | 1 | 0 | 1 | None | None | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptArtifactWriter.cs` | 20 | 17 | 3 | 17 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptFormat.cs` | 26 | 14 | 12 | 14 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptOpcode.cs` | 40 | 10 | 30 | 10 | None | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsArtifactWriter.cs` | 29 | 29 | 0 | 29 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsFormat.cs` | 56 | 25 | 31 | 25 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsOpcode.cs` | 147 | 23 | 124 | 23 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsRegExpMatcher.cs` | 213 | 120 | 93 | 120 | Medium | Medium | 1/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsSurfaces.cs` | 9 | 9 | 0 | 9 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/AssemblyMarker.cs` | 1 | 1 | 0 | 1 | None | None | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptDiagnostics.cs` | 71 | 8 | 63 | 8 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` | 35 | 18 | 17 | 18 | Low | High | 6/6 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptLanguageEdition.cs` | 13 | 13 | 0 | 13 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptPosition.cs` | 10 | 6 | 4 | 6 | None | Medium | 1/0 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` | 28 | 18 | 10 | 18 | Low | High | 7/7 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` | 28 | 20 | 8 | 20 | Low | High | 4/4 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` | 60 | 32 | 28 | 32 | Low | High | 12/12 |
| `src/Broiler.VM.Profile.JavaScript/JsArray.cs` | 19 | 14 | 5 | 14 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsBinary.cs` | 51 | 31 | 20 | 31 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsCollections.cs` | 87 | 51 | 36 | 51 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` | 126 | 112 | 14 | 112 | Low | High | 15/15 |
| `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` | 24 | 13 | 11 | 13 | Low | High | 2/2 |
| `src/Broiler.VM.Profile.JavaScript/JsFunction.cs` | 56 | 26 | 30 | 26 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JsGenerator.cs` | 67 | 17 | 50 | 17 | None | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript/JsModule.cs` | 38 | 13 | 25 | 13 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsNumberFormat.cs` | 19 | 19 | 0 | 19 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsObject.cs` | 59 | 31 | 28 | 31 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsProgram.cs` | 44 | 16 | 28 | 16 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsProxy.cs` | 44 | 37 | 7 | 37 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Array.cs` | 30 | 30 | 0 | 30 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.AsyncGenerator.cs` | 19 | 14 | 5 | 14 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Binary.cs` | 33 | 28 | 5 | 28 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Boolean.cs` | 4 | 4 | 0 | 4 | Low | Low | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.CollectionIterators.cs` | 4 | 4 | 0 | 4 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Collections.cs` | 33 | 27 | 6 | 27 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Date.cs` | 65 | 56 | 9 | 56 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` | 5 | 4 | 1 | 4 | Low | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Error.cs` | 6 | 6 | 0 | 6 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Function.cs` | 7 | 7 | 0 | 7 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Generator.cs` | 12 | 9 | 3 | 9 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Global.cs` | 25 | 25 | 0 | 25 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Json.cs` | 36 | 31 | 5 | 31 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Lexical.cs` | 10 | 5 | 5 | 5 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Math.cs` | 19 | 18 | 1 | 18 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Number.cs` | 24 | 24 | 0 | 24 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Object.cs` | 40 | 25 | 15 | 25 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Promise.cs` | 34 | 28 | 6 | 28 | Low | High | 2/2 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Proxy.cs` | 8 | 6 | 2 | 6 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Reflect.cs` | 6 | 6 | 0 | 6 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.RegExp.cs` | 49 | 39 | 10 | 39 | Medium | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.String.cs` | 15 | 15 | 0 | 15 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Symbol.cs` | 29 | 10 | 19 | 10 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.cs` | 37 | 18 | 19 | 18 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsSymbol.cs` | 6 | 2 | 4 | 2 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsThrow.cs` | 10 | 5 | 5 | 5 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsValue.cs` | 49 | 29 | 20 | 29 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` | 72 | 40 | 32 | 40 | Low | High | 1/1 |
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
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.ProtectSomething(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `EAD16F`, PENDING
  - Falsified if: a region is emitted whose start offset equals its end offset
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.CompileTemplate(JsTemplateLiteral)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `3E5E65`, PENDING
  - Falsified if: a substitution coerces through `valueOf` before `toString`, or a Symbol substitution does not throw
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.EmitToString(JsExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `A802C8`, PENDING
  - Falsified if: the two paths reach the call at different operand-stack heights
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.EmitTemplateStrings(JsTemplateLiteral)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `2E89AA`, PENDING
  - Falsified if: two evaluations of one call site produce two strings objects
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.CompileChain(JsChainExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `E0C92D`, PENDING
  - Falsified if: a link after a short-circuited one is evaluated, or the two paths meet at different heights
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.Shadowable(string, out int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `80888B`, PENDING
  - Falsified if: the bound reaches a record at or beyond the binding this name resolves to
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.TemplateReader` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, Spec=none cited, `09F45E`, PENDING
  - Falsified if: this cursor ends a substitution at a different character than the tokenizer did
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.TemplateReader.ScanSubstitution()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, Spec=none cited, `90E31B`, PENDING
  - Falsified if: a brace inside a string, a comment, a nested template or an object literal closes the substitution
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructCensus` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` - Security=High, Spec=none cited, `C0354E`, PENDING
  - Falsified if: a construct present in a source is not counted, or a count includes a construct the source does not contain
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructCensus.Take(System.Collections.Generic.IEnumerable<string>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` - Security=High, Spec=none cited, `B3C49F`, PENDING
  - Falsified if: a source that parses contributes no counts, or a source that does not parse is counted as containing nothing
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructCensus.Walk(SliceNode, System.Collections.Generic.Dictionary<SliceConstructKind, int>, System.Collections.Generic.HashSet<SliceConstructKind>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` - Security=High, Spec=none cited, `3DAB0E`, PENDING
  - Falsified if: a node reachable in the tree is not visited
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructExpression` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` - Security=High, Spec=none cited, `53E46E`, PENDING
  - Falsified if: a construct node drops a child the parser read, so a walk under it counts nothing
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructStatement` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` - Security=High, Spec=none cited, `44C22E`, PENDING
  - Falsified if: a construct node drops a child the parser read, so a walk under it counts nothing
- `Broiler.VM.Profile.JavaScript.Compiler.SliceManifest` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` - Security=High, Spec=none cited, `9ADE31`, PENDING
  - Falsified if: a construct this returns true for has no lowering, or one it returns false for is lowered anyway
- `Broiler.VM.Profile.JavaScript.Compiler.SliceManifest.Admits(SliceConstructKind)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` - Security=High, Spec=none cited, `18FE9D`, PENDING
  - Falsified if: this admits a kind for which no lowering exists
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParseOptions.MaximumSupportedNestingDepth` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParseOptions.cs` - Security=High, Spec=none cited, `2C4111`, PENDING
  - Falsified if: a source parsed at this bound terminates the process
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParseOptions.MaximumNestingDepth` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParseOptions.cs` - Security=High, Spec=none cited, `FA7632`, PENDING
  - Falsified if: a source nested deeper than this bound terminates the process instead of being refused
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `32D6FA`, PENDING
  - Falsified if: a nesting case terminates the process, a grammar switch is read from anywhere but the options value, or a construct is refused here rather than by the validation stage
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseProgram()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `2D78F6`, PENDING
  - Falsified if: a statement that is not an expression statement over a string literal is admitted into the directive prologue
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseStatement()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `13B3FB`, PENDING
  - Falsified if: a statement form the grammar has is not parsed into a node a walk can descend through
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseDeclarator()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `7C9919`, PENDING
  - Falsified if: a binding pattern is recorded as an identifier, so the validation stage cannot refuse it
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseFor()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `1364F1`, PENDING
  - Falsified if: a `for … in` or `for … of` head is parsed as a three-part head, or the reverse
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseFunction(SliceSourceSpan, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `29B058`, PENDING
  - Falsified if: a generator or an async function is counted as a plain function
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseClass(SliceSourceSpan)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `79DEE6`, PENDING
  - Falsified if: a class body's members are not walked, so what is inside a class is counted as nothing
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseMember(bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `236DBE`, PENDING
  - Falsified if: a property named `get`, `set`, `static` or `async` is read as an accessor or a modifier
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseAssignment(bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `7DFF55`, PENDING
  - Falsified if: an arrow function's head is parsed as a parenthesised expression, or a compound assignment is recorded as a plain one
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseBinary(int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `0FFC90`, PENDING
  - Falsified if: the tree this builds groups an operator differently from the language's precedence and associativity
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Combine(SliceSourceSpan, SliceTokenKind, SliceExpression, SliceExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `1E98FB`, PENDING
  - Falsified if: an operator outside the manifest is built as a precise node, so the validation stage never sees it
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Precedence(SliceTokenKind)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `793D6C`, PENDING
  - Falsified if: two operators the language separates share a level here, or the order differs from the language's
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseUnary()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `705846`, PENDING
  - Falsified if: a unary operator outside the manifest is built as a precise node, or its operand is not walked
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseCallChain()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `309170`, PENDING
  - Falsified if: a link of a chain drops its target, so a walk under it counts nothing
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParsePrimary()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `871855`, PENDING
  - Falsified if: a literal form the grammar has produces no node, so the construct it is goes uncounted
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ArrowFollows()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `4D3013`, PENDING
  - Falsified if: a parenthesised expression is parsed as a parameter list, or an arrow's head is parsed as an expression
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseArrow(SliceSourceSpan)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `1BA262`, PENDING
  - Falsified if: an arrow's parameters or body are dropped, so what is inside it counts as nothing
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ConsumeStatementTerminator()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `C160FB`, PENDING
  - Falsified if: a semicolon is inserted where the language does not insert one, or omitted where it does
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Enter()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `0D6A78`, PENDING
  - Falsified if: recursion continues after this answers false
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.StatementEndsAfterCurrent()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `CA6592`, PENDING
  - Falsified if: a string literal that is not a whole statement is admitted into the directive prologue
- `Broiler.VM.Profile.JavaScript.Compiler.SliceCompilation` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `A28A22`, PENDING
  - Falsified if: a result carries artifact bytes and a diagnostic at once
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `9A3F54`, PENDING
  - Falsified if: two compilations of one source under one options value differ by a byte, or an early error reaches the verifier as an artifact
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.Compile(string, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `4076ED`, PENDING
  - Falsified if: a stage runs over a tree the previous stage refused
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.Lower(SliceProgram, SliceBindingTable, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `C7F469`, PENDING
  - Falsified if: the operand stack is not empty at any statement boundary
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerStatement(SliceStatement, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `B5BD8A`, PENDING
  - Falsified if: any statement lowering leaves the operand stack at a different height than it entered with
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerIf(SliceIfStatement, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `7A389E`, PENDING
  - Falsified if: the two arms of a branch reach the join at different operand-stack heights
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerWhile(SliceWhileStatement, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `82D7DF`, PENDING
  - Falsified if: the body is reachable with the test false, or a `break` does not leave the loop
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerDoWhile(SliceDoWhileStatement, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `60245D`, PENDING
  - Falsified if: the body runs zero times, or a `continue` reaches the loop top rather than the test
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerFor(SliceForStatement, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `3F4366`, PENDING
  - Falsified if: a `continue` skips the update expression, which turns a counting loop into an endless one
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerExpression(SliceExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `2E1CD6`, PENDING
  - Falsified if: any expression lowering leaves other than exactly one value on the stack
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerLogical(SliceLogicalExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `CBC769`, PENDING
  - Falsified if: the value of either operator is coerced to Boolean, or the right operand is evaluated when the left short-circuits
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerConditional(SliceConditionalExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `6BD8B0`, PENDING
  - Falsified if: both arms are evaluated, or the two arms leave different heights at the join
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerAssignment(SliceAssignmentExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `302785`, PENDING
  - Falsified if: an assignment expression's value is not the value assigned
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.OpcodeFor(SliceTokenKind, SliceSourceSpan)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `40B5FC`, PENDING
  - Falsified if: a loose equality is lowered onto a strict one
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerIdentifierReference(SliceIdentifierReference)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `07B781`, PENDING
  - Falsified if: a reference whose binding is already initialised lowers to the fault, or one in the dead zone lowers to a read
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.InDeadZone(SliceIdentifierReference)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `5A4FA1`, PENDING
  - Falsified if: a `var` is reported in the dead zone, or a lexical binding is not reported before its initialiser is lowered
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.Position(SliceSourceSpan)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `804A28`, PENDING
  - Falsified if: this writes a row at an offset not greater than the previous row's, or a row with a zero coordinate
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourcePrograms.RefusedModules` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourcePrograms.cs` - Security=High, Spec=none cited, `DCB5D1`, PENDING
  - Falsified if: any program here is refused with a code other than the one recorded beside it
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourcePrograms.Accepted` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourcePrograms.cs` - Security=High, Spec=none cited, `11701F`, PENDING
  - Falsified if: any program here runs to a value other than the one recorded beside it
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourcePrograms.Refused` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourcePrograms.cs` - Security=High, Spec=none cited, `FEAC8D`, PENDING
  - Falsified if: any source here compiles, or is refused with a code other than the one recorded beside it
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourcePrograms.Nested(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourcePrograms.cs` - Security=High, Spec=none cited, `4CD3B4`, PENDING
  - Falsified if: this source terminates the process at any depth instead of being refused
- `Broiler.VM.Profile.JavaScript.Compiler.SliceNodeIdentityComparer` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `3BEC56`, PENDING
  - Falsified if: two distinct nodes that compare equal as records share one entry in a resolution table
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `47D4B5`, PENDING
  - Falsified if: an early error the manifest requires is reported anywhere but this stage, or this stage reads the source text
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.UseStrictRawForms` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `F4EF6A`, PENDING
  - Falsified if: a string whose value is `use strict` but whose raw text is not one of these two enables strict code
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.Scope` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `E072AC`, PENDING
  - Falsified if: a `var` name written inside a block is not recorded against that block's own scope
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.Validate(SliceProgram)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `7FE2B1`, PENDING
  - Falsified if: strictness is decided after a name or a literal has been ruled on against it
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.HoistVarBindings(System.Collections.Generic.IReadOnlyList<SliceStatement>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `EADB67`, PENDING
  - Falsified if: a `var` declared inside a block is not visible to a reference outside it
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.VarDeclaratorsWithin(System.Collections.Generic.IReadOnlyList<SliceStatement>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `D34BB0`, PENDING
  - Falsified if: a statement kind that can contain a `var` is walked by one caller and not the other
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.DeclareLexical(SliceDeclarator, SliceDeclarationKind)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `4497B3`, PENDING
  - Falsified if: a second lexical declaration of one name in one scope allocates a second slot instead of refusing
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.CheckVarLexicalIntersection(Scope)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `7AE000`, PENDING
  - Falsified if: a name declared both by `var` and by `let` in one scope is accepted
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.VisitStatement(SliceStatement)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `ED5060`, PENDING
  - Falsified if: a `break` or `continue` inside a loop body is reported as having no enclosing loop, or one outside every loop is not
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.VisitConstruct(SliceConstructKind, SliceSourceSpan, System.Collections.Generic.IReadOnlyList<SliceNode>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `B18B5E`, PENDING
  - Falsified if: a construct nested inside another is not reported, or an admitted construct is refused
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.VisitExpression(SliceExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `6C11BD`, PENDING
  - Falsified if: a subexpression is not visited, so an early error inside it goes unreported
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.VisitAssignmentTarget(SliceAssignmentExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `DB4749`, PENDING
  - Falsified if: an assignment to a `const` binding is accepted
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.Resolve(SliceIdentifierReference)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `725C01`, PENDING
  - Falsified if: a name with no binding is accepted, or a name shadowed in an inner scope resolves to the outer one
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `5006C2`, PENDING
  - Falsified if: a second call site for this type appears, or a consumer re-reads the source text
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.Tokenize()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `7C304A`, PENDING
  - Falsified if: a token is produced after a refusal, or the stream does not end with exactly one EndOfSource
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadToken(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `9013CA`, PENDING
  - Falsified if: a character that starts an identifier is read as a punctuator, or a numeric literal is read as an identifier
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadIdentifierEscape(System.Text.StringBuilder, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `380942`, PENDING
  - Falsified if: an escaped identifier and its unescaped spelling are different names
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.AppendScalar(System.Text.StringBuilder, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `F77C9C`, PENDING
  - Falsified if: an escape naming a lone surrogate throws, or a supplementary code point is not encoded as a surrogate pair
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadUnicodeEscapeValue(out int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `6142E3`, PENDING
  - Falsified if: either spelling produces a value the language does not give it
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadNumericLiteral(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `730FCD`, PENDING
  - Falsified if: the value this produces differs from the language's MV for the same literal text
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadStringLiteral(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `872F0C`, PENDING
  - Falsified if: a directive is recognised from the string's value rather than from its raw text
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.RegularExpressionIsAllowedHere()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `6559F9`, PENDING
  - Falsified if: a division after a value is read as a regular expression, or a literal after an operator is read as a division
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadRegularExpressionLiteral(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `C7126B`, PENDING
  - Falsified if: a `/` inside a character class ends the literal
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadTemplateLiteral(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `9ABBF8`, PENDING
  - Falsified if: a template, string, comment or object literal inside a substitution ends the outer literal
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ScanTemplateBody()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `BB535C`, PENDING
  - Falsified if: a substitution consumes the backtick that closes the template it belongs to
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ScanSubstitution()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `00BA21`, PENDING
  - Falsified if: a brace inside a string, comment, nested template or object literal closes the substitution
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ScanStringBody(char)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `98CF66`, PENDING
  - Falsified if: an escaped quote ends the string, or an unterminated one swallows the rest of the source
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ScanRegularExpressionBody()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `EB5032`, PENDING
  - Falsified if: a slash inside a character class ends the literal
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.StartsRegularExpression(char)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `17FBC0`, PENDING
  - Falsified if: a division after an identifier or a literal is taken for a regular expression
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadPunctuator(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `E57116`, PENDING
  - Falsified if: a shorter punctuator is matched where a longer one starting at the same character exists
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.Punctuators` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `3A0A57`, PENDING
  - Falsified if: this table is not in descending order of text length
- `Broiler.VM.Profile.JavaScript.Format.JavaScriptOpcodes` in `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptOpcode.cs` - Security=High, Spec=none cited, `7F5FC6`, PENDING
  - Falsified if: an opcode's declared width differs from what the encoder writes or the executor reads, or a declared stack effect differs from what the executor performs
- `Broiler.VM.Profile.JavaScript.Format.JavaScriptOpcodes.IsDefined(byte)` in `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptOpcode.cs` - Security=High, Spec=none cited, `B722F8`, PENDING
  - Falsified if: a byte this returns true for has no arm in the verifier's stack-effect switch or in the executor's dispatch
- `Broiler.VM.Profile.JavaScript.Format.JavaScriptOpcodes.OperandWidth(JavaScriptOpcode)` in `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptOpcode.cs` - Security=High, Spec=none cited, `5F48AB`, PENDING
  - Falsified if: the width returned here differs from the bytes the encoder emits for the same opcode
- `Broiler.VM.Profile.JavaScript.Format.JavaScriptOpcodes.PopCount(JavaScriptOpcode)` in `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptOpcode.cs` - Security=High, Spec=none cited, `DA56F1`, PENDING
  - Falsified if: an opcode pops a different number of values in the executor than this reports
- `Broiler.VM.Profile.JavaScript.Format.JavaScriptOpcodes.PushCount(JavaScriptOpcode)` in `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptOpcode.cs` - Security=High, Spec=none cited, `2E9BD0`, PENDING
  - Falsified if: an opcode pushes a different number of values in the executor than this reports
- `Broiler.VM.Profile.JavaScript.JavaScriptReadAdapter` in `src/Broiler.VM.Profile.JavaScript/JavaScriptDiagnostics.cs` - Security=High, Spec=none cited, `1DD7A4`, PENDING
  - Falsified if: a charge made through this adapter reaches a dimension other than the one named, or a released byte count is charged rather than released
- `Broiler.VM.Profile.JavaScript.JavaScriptInstance` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `E818FA`, PENDING
  - Falsified if: two instances over one shared handle observe each other's locals, or any instance state is reachable from that handle
- `Broiler.VM.Profile.JavaScript.JavaScriptContinuation` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `38E70F`, PENDING
  - Falsified if: this milestone constructs one, or a resume presented with one is answered as anything but a contract violation
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `E9D7AE`, PENDING
  - Falsified if: any input makes a member here throw, or an answer is produced that is not one of the five step kinds
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Instantiate(VmVerifiedArtifact, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `F2CD86`, PENDING
  - Falsified if: a handle this profile did not verify produces an instance
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Invoke(IVmInstanceState, in VmInvocationRequest, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `59C786`, PENDING
  - Falsified if: an unknown entry point is reported as anything but a language fault, or a foreign instance state runs
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Run(JavaScriptInstance, int)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `1C127C`, PENDING
  - Falsified if: the operand stack is sized from anything but the maximum the verifier computed, or an index used here was not proved in range before execution
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `EA198F`, PENDING
  - Falsified if: a second static accessor or an aggregate profile-listing type appears in this graph, or the descriptor accepts a manifest this build does not implement
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.SourceProviderCapability` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `9A3E06`, PENDING
  - Falsified if: this profile obtains executable bytes by any route but a provider registered under this identity
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.ResolveCapability` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `D7A399`, PENDING
  - Falsified if: this profile opens a file, follows a specifier, or honours a module request the host was not asked to rule on
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.DescriptorAdmitting(params VmFeatureManifestId[])` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `420B8F`, PENDING
  - Falsified if: a descriptor built here accepts an optional surface its caller did not name
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.Build(ImmutableArray<string>)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `864F45`, PENDING
  - Falsified if: a row here disagrees with decision JSD-0004 or JSD-0008 without a dated record of the correction
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.Defaults()` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `B1B19D`, PENDING
  - Falsified if: a default here is zero on a dimension this profile declares inapplicable, or any default exceeds its maximum
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.Matrix()` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `AAA8EB`, PENDING
  - Falsified if: a row says charged for a dimension no code path charges, or inapplicable for one that is reachable
- `Broiler.VM.Profile.JavaScript.JavaScriptValue.ToInt32()` in `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` - Security=High, Spec=none cited, `61E0F7`, PENDING
  - Falsified if: ToInt32 of 2147483648 is not -2147483648, or of NaN or an infinity is not 0
- `Broiler.VM.Profile.JavaScript.JavaScriptValue.ToUint32()` in `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` - Security=High, Spec=none cited, `983293`, PENDING
  - Falsified if: ToUint32 of -1 is not 4294967295, or of a value above 2^53 disagrees with the specification's modulo
- `Broiler.VM.Profile.JavaScript.JavaScriptValue.StrictlyEquals(JavaScriptValue)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` - Security=High, Spec=none cited, `4BA1F7`, PENDING
  - Falsified if: NaN is strictly equal to itself, or +0 is not strictly equal to -0, or 1 is strictly equal to true
- `Broiler.VM.Profile.JavaScript.JavaScriptValue.LessThan(JavaScriptValue, JavaScriptValue)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` - Security=High, Spec=none cited, `C91DA6`, PENDING
  - Falsified if: any relational comparison involving NaN answers true
- `Broiler.VM.Profile.JavaScript.JavaScriptProgram` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `7FCF90`, PENDING
  - Falsified if: anything reachable from this state can be mutated after verification returns, or two runtimes sharing one handle observe each other through it
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `6DB9F8`, PENDING
  - Falsified if: any input makes Verify throw, or a check this class performs can be reached for the first time during execution
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.surfaces` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `60EEF1`, PENDING
  - Falsified if: this set differs from the accepted feature manifests of the descriptor that carries this verifier
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.Verify(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, IVmVerificationContext, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `1557EA`, PENDING
  - Falsified if: a payload byte is read on a path that answers UnsupportedProfile
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.ReadAndCheckManifest(in VmArtifactDescriptor, ref VmBoundedReader)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `5533C9`, PENDING
  - Falsified if: an artifact naming an unaccepted manifest verifies, or the two mismatches report the same diagnostic code
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.ReadSection(ref VmBoundedReader, in VmReadBounds, JavaScriptReadAdapter, ref uint, ref SectionSet, uint, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `2FA2BD`, PENDING
  - Falsified if: a section body is read before its kind's order and uniqueness are checked, or an unknown kind is skipped rather than refused
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.ReadLimits(ref VmBoundedReader, ref SectionSet)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `521C64`, PENDING
  - Falsified if: a declared maximum is used before its ceiling comparison
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.ReadReserved(ref VmBoundedReader, JavaScriptDiagnosticCode)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `57B0F0`, PENDING
  - Falsified if: a reserved section carrying a non-zero count verifies
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.Link(ref SectionSet, in VmReadBounds, JavaScriptReadAdapter)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `B856CD`, PENDING
  - Falsified if: an artifact admitted here contains a jump to a non-boundary, a join whose two heights differ, an unreachable instruction, or a path that reaches the end of the code without returning
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.InvalidInCode(VmReason, JavaScriptDiagnosticCode, ulong, in SectionSet)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `B800EB`, PENDING
  - Falsified if: a code-section offset is reported with the artifact-relative section index, or a read-stage offset with a section index
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.Stopped(System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `2FAC64`, PENDING
  - Falsified if: a wall-clock exhaustion during verification is reported as a cancellation, or a cancellation as a resource exhaustion
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.FromReader(ref VmBoundedReader, ulong)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `B519B7`, PENDING
  - Falsified if: a ceiling breach is mapped onto an invalid artifact, or a framing failure onto a resource exhaustion
- `Broiler.VM.Profile.JavaScript.JsFinalizationRegistryObject` in `src/Broiler.VM.Profile.JavaScript/JsCollections.cs` - Security=High, Spec=none cited, `66E399`, PENDING
  - Falsified if: a cleanup callback registered here is ever invoked, or any guest code runs from a CLR finalizer
- `Broiler.VM.Profile.JavaScript.JsEngine.DrainJobs()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `541804`, PENDING
  - Falsified if: a job runs at a point the host did not ask for, or an endless queue is a hang rather than an exhaustion
- `Broiler.VM.Profile.JavaScript.JsEngine.Loader` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `67C7AE`, PENDING
  - Falsified if: a mediator is used outside the invocation that supplied it
- `Broiler.VM.Profile.JavaScript.JsEngine.Evaluate(JsValue[], bool, Format.JsFormat.FunctionFlags)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `AF4B33`, PENDING
  - Falsified if: guest source becomes executable bytes without passing through the mediator
- `Broiler.VM.Profile.JavaScript.JsEngine.MaximumCallDepth` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `321EE5`, PENDING
  - Falsified if: a program recursing past this bound terminates the process rather than throwing a catchable RangeError
- `Broiler.VM.Profile.JavaScript.JsEngine.reportingDepth` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `DA5D1D`, PENDING
  - Falsified if: this stays set after the refusal has been thrown, so a later recursion is unbounded
- `Broiler.VM.Profile.JavaScript.JsEngine.RunModuleGraph(JsProgram, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `7F9904`, PENDING
  - Falsified if: a module body runs twice in one realm, or a module body runs before every module's declarations are initialised
- `Broiler.VM.Profile.JavaScript.JsEngine.Step(JsProgram, System.Collections.Generic.List<int>, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `F0048D`, PENDING
  - Falsified if: a module body runs before a module it requested has finished awaiting
- `Broiler.VM.Profile.JavaScript.JsEngine.Confirm(JsProgram)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `4B22CD`, PENDING
  - Falsified if: a module request is honoured without the composition being asked, or a refusal is treated as an answer
- `Broiler.VM.Profile.JavaScript.JsEngine.ResumeGenerator(JsValue, JsResumeMode, JsValue, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `984439`, PENDING
  - Falsified if: a generator resumed while its own body is running re-enters that body, or a completed generator runs any instruction
- `Broiler.VM.Profile.JavaScript.JsEngine.ResumeAsync(JsAsyncCall, JsResumeMode, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `99B3DF`, PENDING
  - Falsified if: an async call whose body is already on the interpreter's stack is resumed again, or a program that awaits without end is a hang rather than an exhaustion
- `Broiler.VM.Profile.JavaScript.JsEngine.EnqueueAsyncGenerator(JsValue, JsResumeMode, JsValue, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `CB6E30`, PENDING
  - Falsified if: a call of `next`, `return` or `throw` on an async generator answers anything but a promise, or two calls made before the first settles are answered out of order
- `Broiler.VM.Profile.JavaScript.JsEngine.ResumeAsyncGenerator(JsAsyncGenerator, JsResumeMode, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `30F939`, PENDING
  - Falsified if: an async generator whose body is on the interpreter's stack is resumed again, or an `await` inside an async generator body settles a request the way a `yield` does
- `Broiler.VM.Profile.JavaScript.JsEngine.Delegate(JsFrame, JsValue[], ref int, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `E53DC7`, PENDING
  - Falsified if: a `return` or a `throw` that arrives while a `yield*` is suspended is not offered to the inner iterator first
- `Broiler.VM.Profile.JavaScript.JsEngine.DelegateAsync(JsFrame, JsValue[], ref int, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `197F48`, PENDING
  - Falsified if: an inner step of an async `yield*` reaches the outer body unawaited, or a `return` or a `throw` that arrives while one is suspended is not offered to the inner iterator first
- `Broiler.VM.Profile.JavaScript.JsEngine.ResolveName(System.Collections.Generic.List<JsEnvironment>, int, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `1AC0F4`, PENDING
  - Falsified if: this walk answers with anything but an object a `PushObjectScope` placed on the chain
- `Broiler.VM.Profile.JavaScript.JsInstance.Environment` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `1C7767`, PENDING
  - Falsified if: this environment is asked for a mediator outside an invocation it supplied one for
- `Broiler.VM.Profile.JavaScript.JsExecution.RunOnGuestStack(JsInstance, uint?)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `CDA795`, PENDING
  - Falsified if: guest code runs on the caller's stack, or an exception the guest raised does not reach the caller
- `Broiler.VM.Profile.JavaScript.JsEnvironment` in `src/Broiler.VM.Profile.JavaScript/JsFunction.cs` - Security=High, Spec=none cited, `D0AAD1`, PENDING
  - Falsified if: a lookup by name reaches a slot of a declarative record
- `Broiler.VM.Profile.JavaScript.JsFrame` in `src/Broiler.VM.Profile.JavaScript/JsGenerator.cs` - Security=High, Spec=none cited, `9F431B`, PENDING
  - Falsified if: a generator resumed after a suspension observes an operand stack, a scope chain or an instruction pointer other than the one it suspended with
- `Broiler.VM.Profile.JavaScript.JsForcedReturn` in `src/Broiler.VM.Profile.JavaScript/JsGenerator.cs` - Security=High, Spec=none cited, `4AB7F8`, PENDING
  - Falsified if: a `catch` clause in a generator body observes the value a `return()` forced, or a `finally` block does not run for one
- `Broiler.VM.Profile.JavaScript.JsGenerator` in `src/Broiler.VM.Profile.JavaScript/JsGenerator.cs` - Security=High, Spec=none cited, `FDED37`, PENDING
  - Falsified if: a generator whose body is on the interpreter's stack can be resumed again, or a completed generator runs any part of its body
- `Broiler.VM.Profile.JavaScript.JsAsyncCall` in `src/Broiler.VM.Profile.JavaScript/JsGenerator.cs` - Security=High, Spec=none cited, `17AAD5`, PENDING
  - Falsified if: an async call whose body is on the interpreter's stack is resumed again, or a suspended async call is reachable from anything the allowance is not already counting
- `Broiler.VM.Profile.JavaScript.JsAsyncGenerator` in `src/Broiler.VM.Profile.JavaScript/JsGenerator.cs` - Security=High, Spec=none cited, `F8D803`, PENDING
  - Falsified if: two requests made before the first settles are answered out of order, or a request reaches a body that is already on the interpreter's stack
- `Broiler.VM.Profile.JavaScript.JsRealm.GetAsyncIterator(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.AsyncGenerator.cs` - Security=High, Spec=none cited, `EFF362`, PENDING
  - Falsified if: a `for await` over an object carrying `Symbol.asyncIterator` reaches the synchronous wrapper, or one over an Array of promises answers the promises rather than their values
- `Broiler.VM.Profile.JavaScript.JsRealm` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, Spec=none cited, `60DD8D`, PENDING
  - Falsified if: anything in this file is built into a realm whose composition did not admit broiler.javascript.dynamic
- `Broiler.VM.Profile.JavaScript.JsRealm.EvalIntrinsic` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, Spec=none cited, `D093DD`, PENDING
  - Falsified if: this holds a function object the guest can reach under any other name
- `Broiler.VM.Profile.JavaScript.JsRealm.IsEvalIntrinsic(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, Spec=none cited, `B458DE`, PENDING
  - Falsified if: it answers true for a function object this realm did not build as its own eval
- `Broiler.VM.Profile.JavaScript.JsRealm.SetupDynamic()` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, Spec=none cited, `31E9B4`, PENDING
  - Falsified if: it installs a global that turns source into code without going through the mediator
- `Broiler.VM.Profile.JavaScript.JsRealm.FromSource(JsEngine, JsValue[])` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, Spec=none cited, `01B333`, PENDING
  - Falsified if: the assembled source is evaluated anywhere but the global scope
- `Broiler.VM.Profile.JavaScript.JsRealm.PromiseSchedule(JsEngine, JsPromiseReaction, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Promise.cs` - Security=High, Spec=none cited, `C89A73`, PENDING
  - Falsified if: a promise reaction runs before the synchronous continuation of whatever settled or observed the promise
- `Broiler.VM.Profile.JavaScript.JsRealm.AwaitOn(JsEngine, JsValue, System.Action<JsEngine, JsValue, bool>)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Promise.cs` - Security=High, Spec=none cited, `DB2999`, PENDING
  - Falsified if: an `await` of a value that is not a promise continues without yielding to the job queue
- `Broiler.VM.Profile.JavaScript.JsVerifier.LinkModules(Sections, JsCodeUnit[], IVmVerificationContext, JavaScriptReadAdapter, out JsModuleRecord[], out JsBinding[])` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `8255D8`, PENDING
  - Falsified if: linking recurses to a depth the payload chooses, or a cyclic export resolution is answered by spending an allowance
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

That is not a figure of speech. 2612 of the 2612 assessed units declare
`Origin=AI`, and the records this component implements were drafted the same way. An
adversarial pass over the work confirmed findings and they were corrected, which is a check
on it and not an independent judgement of it. Reading a declaration is the only thing that
makes it read.
