# Human Review: Broiler.VM

GENERATED - DO NOT EDIT MANUALLY. Regenerate with
`BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release`, which rewrites this file,
`CODE-ASSURANCE.md`, `assurance.manifest.json` and every generated source header from the
product tree.

> **Status: PENDING.** Human-reviewed: 0 of 5190 relevant units. No package
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
| Files scanned | 211 |
| Code units | 8814 |
| Relevant | 5190 |
| Exempt | 3624 |
| Assessed | 5190 of 5190 (100%) |
| Human reviewed | 0 of 5190 (0%) |
| Unverified | 5190 |
| Aliases naming a decision | 0 |

## 4. Review States

One row per state of the machine that reads the two lines. The states are computed from the
annotations and the current fingerprints; nothing stores them.

| State | Units |
|---|---:|
| NEW | 0 |
| AI_ASSESSED | 0 |
| HUMAN_PENDING | 5190 |
| HUMAN_APPROVED_PENDING_FINGERPRINT | 0 |
| VERIFIED | 0 |
| STALE | 0 |
| EXEMPT | 3624 |

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
| `src/Broiler.VM.Abstractions/VmProfileContracts.cs` | 76 | 42 | 34 | 42 | Low | High | 9/8 |
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
| `src/Broiler.VM.Profile.JavaScript.Compiler/CompilationStack.cs` | 3 | 3 | 0 | 3 | Low | High | 2/2 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` | 54 | 45 | 9 | 45 | Low | High | 14/14 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` | 42 | 37 | 5 | 37 | Low | High | 14/14 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Walk.cs` | 31 | 18 | 13 | 18 | Low | High | 4/4 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` | 349 | 228 | 121 | 228 | Low | High | 19/18 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsNativeBackend.cs` | 23 | 12 | 11 | 12 | None | Medium | 4/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsNativeCompiler.cs` | 9 | 7 | 2 | 7 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` | 14 | 12 | 2 | 12 | None | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` | 185 | 162 | 23 | 162 | Low | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsSyntax.cs` | 99 | 83 | 16 | 83 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` | 11 | 9 | 2 | 9 | Low | High | 8/8 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` | 97 | 65 | 32 | 65 | Low | Critical | 20/20 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` | 34 | 29 | 5 | 29 | Low | High | 17/17 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64BaselineEmitter.cs` | 4 | 4 | 0 | 4 | None | Critical | 3/3 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64ValueEmitter.cs` | 9 | 9 | 0 | 9 | Low | Critical | 4/4 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Walk.cs` | 28 | 16 | 12 | 16 | Low | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` | 7 | 7 | 0 | 7 | None | High | 3/3 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` | 61 | 6 | 55 | 6 | None | High | 4/4 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceControlFlow.cs` | 8 | 8 | 0 | 8 | None | Medium | 4/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceLowering.cs` | 22 | 22 | 0 | 22 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParseOptions.cs` | 18 | 11 | 7 | 11 | None | High | 4/3 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` | 70 | 63 | 7 | 63 | None | High | 21/21 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceProgramBuilder.cs` | 35 | 24 | 11 | 24 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` | 35 | 26 | 9 | 26 | None | High | 19/19 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceDiagnostics.cs` | 34 | 4 | 30 | 4 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourcePrograms.cs` | 11 | 10 | 1 | 10 | None | High | 6/4 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` | 40 | 24 | 16 | 24 | None | High | 14/14 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSyntax.cs` | 29 | 26 | 3 | 26 | None | Medium | 1/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` | 142 | 42 | 100 | 42 | Low | High | 22/18 |
| `src/Broiler.VM.Profile.JavaScript.Format/AssemblyMarker.cs` | 1 | 1 | 0 | 1 | None | None | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptArtifactWriter.cs` | 20 | 17 | 3 | 17 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptFormat.cs` | 26 | 14 | 12 | 14 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptOpcode.cs` | 40 | 10 | 30 | 10 | None | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsArtifactWriter.cs` | 41 | 41 | 0 | 41 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` | 76 | 46 | 30 | 46 | Low | High | 19/17 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineFrame.cs` | 17 | 9 | 8 | 9 | None | Critical | 8/8 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsFormat.cs` | 111 | 52 | 59 | 52 | None | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` | 29 | 17 | 12 | 17 | Low | High | 6/6 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsNativeFrame.cs` | 17 | 3 | 14 | 3 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` | 54 | 25 | 29 | 25 | Low | Critical | 14/14 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` | 70 | 45 | 25 | 45 | Low | Critical | 11/11 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsNumericManifest.cs` | 5 | 5 | 0 | 5 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsOpcode.cs` | 168 | 23 | 145 | 23 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsRegExpMatcher.cs` | 224 | 127 | 97 | 127 | Medium | Medium | 1/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsSurfaces.cs` | 12 | 12 | 0 | 12 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeCaseFolding.cs` | 12 | 12 | 0 | 12 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeCaseFolding.g.cs` | 7 | 1 | 6 | 1 | Low | Low | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeLexical.cs` | 4 | 4 | 0 | 4 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeProperties.cs` | 17 | 15 | 2 | 15 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeProperties.g.cs` | 18 | 1 | 17 | 1 | Low | Low | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` | 28 | 22 | 6 | 22 | Low | Critical | 20/20 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` | 181 | 61 | 120 | 61 | Low | Critical | 40/40 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsWord.cs` | 33 | 33 | 0 | 33 | Low | High | 10/10 |
| `src/Broiler.VM.Profile.JavaScript/AssemblyMarker.cs` | 1 | 1 | 0 | 1 | None | None | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptDiagnostics.cs` | 80 | 8 | 72 | 8 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` | 37 | 18 | 19 | 18 | Low | High | 7/6 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptLanguageEdition.cs` | 13 | 13 | 0 | 13 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptPosition.cs` | 10 | 6 | 4 | 6 | None | Medium | 1/0 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` | 41 | 27 | 14 | 27 | Low | High | 13/13 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` | 28 | 20 | 8 | 20 | Low | High | 4/4 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` | 62 | 33 | 29 | 33 | Low | High | 14/14 |
| `src/Broiler.VM.Profile.JavaScript/JsArray.cs` | 19 | 14 | 5 | 14 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` | 202 | 201 | 1 | 201 | Low | Critical | 202/202 |
| `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` | 55 | 51 | 4 | 51 | Low | High | 24/24 |
| `src/Broiler.VM.Profile.JavaScript/JsBinary.cs` | 80 | 53 | 27 | 53 | Low | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript/JsClone.cs` | 92 | 30 | 62 | 30 | Low | High | 6/6 |
| `src/Broiler.VM.Profile.JavaScript/JsCollections.cs` | 87 | 51 | 36 | 51 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` | 14 | 12 | 2 | 12 | Low | Critical | 14/14 |
| `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` | 241 | 208 | 33 | 208 | Low | Critical | 83/81 |
| `src/Broiler.VM.Profile.JavaScript/JsEvalMap.cs` | 33 | 15 | 18 | 15 | Low | High | 3/3 |
| `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` | 44 | 26 | 18 | 26 | Low | High | 12/12 |
| `src/Broiler.VM.Profile.JavaScript/JsFunction.cs` | 64 | 29 | 35 | 29 | Low | High | 3/3 |
| `src/Broiler.VM.Profile.JavaScript/JsGenerator.cs` | 69 | 17 | 52 | 17 | None | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` | 32 | 17 | 15 | 17 | Low | Critical | 13/13 |
| `src/Broiler.VM.Profile.JavaScript/JsHostObject.cs` | 14 | 10 | 4 | 10 | Low | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` | 92 | 80 | 12 | 80 | Low | High | 41/41 |
| `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` | 133 | 55 | 78 | 55 | Low | High | 9/9 |
| `src/Broiler.VM.Profile.JavaScript/JsModule.cs` | 51 | 15 | 36 | 15 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsNativeAbi.cs` | 4 | 4 | 0 | 4 | Low | Critical | 4/4 |
| `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` | 37 | 9 | 28 | 9 | Low | Critical | 24/24 |
| `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` | 23 | 14 | 9 | 14 | Low | Critical | 18/18 |
| `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` | 12 | 4 | 8 | 4 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsNumberFormat.cs` | 19 | 19 | 0 | 19 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsObject.cs` | 69 | 38 | 31 | 38 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JsProgram.cs` | 64 | 18 | 46 | 18 | Low | Critical | 4/4 |
| `src/Broiler.VM.Profile.JavaScript/JsProxy.cs` | 44 | 37 | 7 | 37 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Array.cs` | 39 | 38 | 1 | 38 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.ArrayFromAsync.cs` | 23 | 14 | 9 | 14 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.AsyncGenerator.cs` | 19 | 14 | 5 | 14 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.BigInt.cs` | 6 | 5 | 1 | 5 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Binary.cs` | 45 | 38 | 7 | 38 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Boolean.cs` | 4 | 4 | 0 | 4 | Low | Low | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` | 53 | 36 | 17 | 36 | Low | High | 22/22 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.CollectionIterators.cs` | 4 | 4 | 0 | 4 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Collections.cs` | 33 | 27 | 6 | 27 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Date.cs` | 65 | 56 | 9 | 56 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Disposal.cs` | 57 | 36 | 21 | 36 | Low | High | 6/6 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` | 5 | 4 | 1 | 4 | Low | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Error.cs` | 9 | 8 | 1 | 8 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Function.cs` | 7 | 7 | 0 | 7 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Generator.cs` | 12 | 9 | 3 | 9 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Global.cs` | 25 | 25 | 0 | 25 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Iterator.cs` | 29 | 16 | 13 | 16 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.IteratorHelpers.cs` | 27 | 18 | 9 | 18 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Json.cs` | 42 | 36 | 6 | 36 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Lexical.cs` | 10 | 5 | 5 | 5 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Math.cs` | 19 | 18 | 1 | 18 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Number.cs` | 24 | 24 | 0 | 24 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Object.cs` | 50 | 33 | 17 | 33 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Promise.cs` | 36 | 30 | 6 | 30 | Low | High | 3/3 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Proxy.cs` | 8 | 6 | 2 | 6 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Reflect.cs` | 6 | 6 | 0 | 6 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.RegExp.cs` | 65 | 53 | 12 | 53 | Medium | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.String.cs` | 21 | 21 | 0 | 21 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.Symbol.cs` | 31 | 10 | 21 | 10 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsRealm.cs` | 39 | 19 | 20 | 19 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsSymbol.cs` | 6 | 2 | 4 | 2 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsThrow.cs` | 10 | 5 | 5 | 5 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsUnicodeNormalization.cs` | 27 | 18 | 9 | 18 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsUnicodeNormalization.g.cs` | 9 | 1 | 8 | 1 | Low | Low | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsValue.cs` | 53 | 32 | 21 | 32 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` | 428 | 427 | 1 | 427 | Low | Critical | 428/428 |
| `src/Broiler.VM.Profile.JavaScript/JsValueSlab.cs` | 14 | 11 | 3 | 11 | Low | Critical | 7/7 |
| `src/Broiler.VM.Profile.JavaScript/JsValueStack.cs` | 12 | 9 | 3 | 9 | Low | Critical | 7/7 |
| `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` | 14 | 14 | 0 | 14 | Low | Critical | 14/14 |
| `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` | 110 | 61 | 49 | 61 | Low | High | 17/17 |
| `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` | 49 | 39 | 10 | 39 | Low | Critical | 22/22 |
| `src/Broiler.VM.Profile.JavaScript/JsWordCodec.cs` | 5 | 5 | 0 | 5 | Low | Critical | 5/5 |
| `src/Broiler.VM.Profile.MachineCode/AssemblyMarker.cs` | 1 | 1 | 0 | 1 | None | None | 0/0 |
| `src/Broiler.VM.Profile.MachineCode/MachineCodeArtifactWriter.cs` | 2 | 2 | 0 | 2 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.MachineCode/MachineCodeExecutor.cs` | 19 | 9 | 10 | 9 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.MachineCode/MachineCodeFormat.cs` | 13 | 6 | 7 | 6 | Low | Low | 0/0 |
| `src/Broiler.VM.Profile.MachineCode/MachineCodeProfile.cs` | 9 | 5 | 4 | 5 | Low | Low | 0/0 |
| `src/Broiler.VM.Profile.MachineCode/MachineCodeProgram.cs` | 10 | 3 | 7 | 3 | Low | Low | 0/0 |
| `src/Broiler.VM.Profile.MachineCode/MachineCodeReadAdapter.cs` | 8 | 5 | 3 | 5 | Low | Low | 0/0 |
| `src/Broiler.VM.Profile.MachineCode/MachineCodeVerifier.cs` | 8 | 3 | 5 | 3 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.MachineCode/VmNativeFrame.cs` | 17 | 3 | 14 | 3 | Low | Low | 0/0 |
| `src/Broiler.VM.Profile.MachineCode/VmNativePage.Unix.cs` | 12 | 11 | 1 | 11 | Low | Critical | 12/12 |
| `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` | 13 | 13 | 0 | 13 | Low | Critical | 13/13 |
| `src/Broiler.VM.Profile.MachineCode/VmNativePage.cs` | 22 | 12 | 10 | 12 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.WebAssembly/AssemblyMarker.cs` | 1 | 1 | 0 | 1 | None | None | 0/0 |
| `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` | 63 | 39 | 24 | 39 | Low | Critical | 24/24 |
| `src/Broiler.VM.Profile.WebAssembly/WasmEntryPoint.cs` | 8 | 8 | 0 | 8 | Low | High | 5/5 |
| `src/Broiler.VM.Profile.WebAssembly/WasmFormat.cs` | 24 | 10 | 14 | 10 | Low | High | 2/2 |
| `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` | 57 | 30 | 27 | 30 | Low | Critical | 20/20 |
| `src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs` | 18 | 12 | 6 | 12 | Low | Critical | 6/6 |
| `src/Broiler.VM.Profile.WebAssembly/WasmMemory.cs` | 15 | 13 | 2 | 13 | Low | Critical | 6/6 |
| `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` | 94 | 50 | 44 | 50 | Low | Critical | 7/7 |
| `src/Broiler.VM.Profile.WebAssembly/WasmName.cs` | 3 | 3 | 0 | 3 | Low | High | 3/3 |
| `src/Broiler.VM.Profile.WebAssembly/WasmOpcode.cs` | 173 | 1 | 172 | 1 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.WebAssembly/WasmReadAdapter.cs` | 12 | 10 | 2 | 10 | Low | High | 6/6 |
| `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` | 27 | 18 | 9 | 18 | Low | Critical | 10/10 |
| `src/Broiler.VM.Profile.WebAssembly/WasmTrapKind.cs` | 16 | 2 | 14 | 2 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.WebAssembly/WasmTypes.cs` | 45 | 20 | 25 | 20 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` | 113 | 75 | 38 | 75 | Low | Critical | 52/52 |
| `src/Broiler.VM.Profile.WebAssembly/WasmValue.cs` | 16 | 12 | 4 | 12 | Low | High | 3/3 |
| `src/Broiler.VM.Profile.WebAssembly/WebAssemblyDiagnostics.cs` | 81 | 5 | 76 | 5 | Low | High | 3/3 |
| `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` | 16 | 14 | 2 | 14 | Low | Critical | 7/7 |
| `src/Broiler.VM.Profile.WebAssembly/WebAssemblyPayloads.cs` | 42 | 15 | 27 | 15 | Low | High | 2/2 |
| `src/Broiler.VM.Profile.WebAssembly/WebAssemblyProfile.cs` | 16 | 13 | 3 | 13 | Low | High | 4/4 |
| `src/Broiler.VM.Profile.WebAssembly/WebAssemblyVerifier.cs` | 11 | 7 | 4 | 7 | Low | Critical | 4/4 |
| `src/Broiler.VM.Runtime/VmAggregateBudget.cs` | 42 | 23 | 19 | 23 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmArtifactLoadMediator.cs` | 17 | 6 | 11 | 6 | Low | Medium | 1/0 |
| `src/Broiler.VM.Runtime/VmBudgetLevel.cs` | 16 | 12 | 4 | 12 | Low | Medium | 1/0 |
| `src/Broiler.VM.Runtime/VmCanonicalCatalogEncoding.cs` | 5 | 5 | 0 | 5 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmCapabilityBinding.cs` | 22 | 10 | 12 | 10 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmCatalog.cs` | 47 | 24 | 23 | 24 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmCatalogValidation.cs` | 47 | 5 | 42 | 5 | Low | Low | 0/0 |
| `src/Broiler.VM.Runtime/VmCeilingResolution.cs` | 4 | 4 | 0 | 4 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmDescriptorValidation.cs` | 10 | 10 | 0 | 10 | Low | Low | 0/0 |
| `src/Broiler.VM.Runtime/VmExecutionScope.cs` | 29 | 16 | 13 | 16 | Low | Medium | 2/0 |
| `src/Broiler.VM.Runtime/VmFuelPreAdmissions.cs` | 13 | 11 | 2 | 11 | Low | Medium | 10/0 |
| `src/Broiler.VM.Runtime/VmInstanceImplementation.cs` | 40 | 24 | 16 | 24 | Low | High | 6/2 |
| `src/Broiler.VM.Runtime/VmInstantiation.cs` | 15 | 9 | 6 | 9 | Low | Medium | 2/0 |
| `src/Broiler.VM.Runtime/VmLimitPrecedence.cs` | 3 | 3 | 0 | 3 | Low | High | 1/1 |
| `src/Broiler.VM.Runtime/VmMeter.cs` | 52 | 31 | 21 | 31 | Low | Medium | 20/0 |
| `src/Broiler.VM.Runtime/VmNativePipeline.cs` | 8 | 3 | 5 | 3 | Low | Low | 0/0 |
| `src/Broiler.VM.Runtime/VmOperation.cs` | 54 | 25 | 29 | 25 | Low | Medium | 1/0 |
| `src/Broiler.VM.Runtime/VmProfileRuntimeState.cs` | 15 | 6 | 9 | 6 | Low | Medium | 0/0 |
| `src/Broiler.VM.Runtime/VmRuntime.cs` | 62 | 36 | 26 | 36 | Low | High | 13/2 |
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
- `Broiler.VM.Profile.JavaScript.Compiler.CompilationStack` in `src/Broiler.VM.Profile.JavaScript.Compiler/CompilationStack.cs` - Security=High, Spec=none cited, `2C7737`, PENDING
  - Falsified if: a compilation walks a syntax tree on the caller's stack
- `Broiler.VM.Profile.JavaScript.Compiler.CompilationStack.Run<T>(System.Func<T>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/CompilationStack.cs` - Security=High, Spec=none cited, `97F713`, PENDING
  - Falsified if: the compilation runs on the calling thread, or an exception it raised does not reach the caller
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `ACB9B6`, PENDING
  - Falsified if: a word this type writes decodes as an instruction other than the one its method name states, or a branch it patched reaches an offset other than its bound label
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.ToArray()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `911DEE`, PENDING
  - Falsified if: the four bytes this method writes for a word are not that word's little-endian representation
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.Branch(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `56C339`, PENDING
  - Falsified if: a site this method records is left carrying its placeholder word after TryFix has answered true
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.BranchIf(JsArm64Condition, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `C4E165`, PENDING
  - Falsified if: the condition field of the word this method writes is not the condition it was given
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.TryFix(out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `CFE6F8`, PENDING
  - Falsified if: a displacement outside the encodable range is written into a branch word instead of being refused
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.Scaled(int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `1F7360`, PENDING
  - Falsified if: an offset outside the encodable range is written into an instruction word instead of raising
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.PairOffset(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `0B2388`, PENDING
  - Falsified if: an offset outside the signed seven-bit range is written into a pair instruction instead of raising
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.StorePairPreIndex(int, int, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `A28B68`, PENDING
  - Falsified if: the word this method writes moves the stack pointer by an amount other than the offset it was given
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.LoadPairPostIndex(int, int, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `835BE5`, PENDING
  - Falsified if: the word this method writes leaves the stack pointer at a value other than its entry value plus the offset
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.Return()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `BB9CCC`, PENDING
  - Falsified if: the word this method writes is anything but a plain return through the link register
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.SetIfCondition(int, JsArm64Condition)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `7A2EBF`, PENDING
  - Falsified if: the word this method writes sets its destination to one in the case where the condition it was given does not hold
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.LoadDouble(int, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `C29FF5`, PENDING
  - Falsified if: the word this method writes names an integer register rather than a floating-point one
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.StoreDouble(int, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `764A8D`, PENDING
  - Falsified if: the word this method writes names an integer register rather than a floating-point one
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Assembler.CompareDouble(int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` - Security=High, Spec=none cited, `437B58`, PENDING
  - Falsified if: the word this method writes compares the two registers in the opposite order
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Frame` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `79329A`, PENDING
  - Falsified if: a field offset here differs from the offset the runtime gives that field of JsNativeFrame
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Frame.StackFrameBytes` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `708AA7`, PENDING
  - Falsified if: the prologue leaves the stack pointer at an address that is not a multiple of sixteen
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Frame.UndefinedBits` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `279E6F`, PENDING
  - Falsified if: an admitted operator distinguishes this bit pattern from a NaN the emitted code could itself produce
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `5C4732`, PENDING
  - Falsified if: this component maps, protects or executes memory on behalf of an arm64 emission, or emits a template for an instruction whose JavaScript semantics that template does not implement
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.TryEmit(JsAssembledProgram, out JsNativeEmission, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `8956C1`, PENDING
  - Falsified if: this method answers true for a program containing a unit it emitted no entry point for, or answers true having emitted a template for an instruction it does not admit
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.EmitUnit(JsArm64Assembler, JsAssembledProgram, int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `60ADDC`, PENDING
  - Falsified if: an offset this pass treats as unreachable is the target of a branch it emitted
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.Prologue(JsArm64Assembler)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `9051BC`, PENDING
  - Falsified if: a register this prologue saves is not restored by the epilogue to the same stack slot
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.Epilogue(JsArm64Assembler)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `A20958`, PENDING
  - Falsified if: this epilogue leaves the stack pointer at a value other than its value on entry to the unit
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.Charge(JsArm64Assembler, int, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `92DD0E`, PENDING
  - Falsified if: a block charges a number of units other than the count of bytecode instructions it contains
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.EmitInstruction(JsArm64Assembler, JsAssembledProgram, JsArm64Walk, int, int[], int, int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `104389`, PENDING
  - Falsified if: a template here computes something other than what this profile's interpreter computes for the same instruction over Number operands
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.Binary(JsArm64Assembler, int, JsArm64Operation)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `1D962A`, PENDING
  - Falsified if: the operands are loaded in an order that makes a subtraction or a division compute the reverse
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.Compare(JsArm64Assembler, int, bool, JsArm64Condition)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `ACABB6`, PENDING
  - Falsified if: a comparison answers true for a pair in which either operand is a NaN
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.Falsy(JsArm64Assembler, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `ED52A4`, PENDING
  - Falsified if: this template answers false for a NaN operand
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.JumpIfTrue(JsArm64Assembler, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `2772AF`, PENDING
  - Falsified if: this template branches for an operand that is a zero or a NaN
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Walk` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Walk.cs` - Security=High, Spec=none cited, `BEBD3B`, PENDING
  - Falsified if: an offset this pass reports a height or a flat slot for is reached by a path on which that height or that slot is different
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Walk.LocalSlots` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Walk.cs` - Security=High, Spec=none cited, `108210`, PENDING
  - Falsified if: an emitted instruction addresses a local slot at or beyond this count
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Walk.TryTrace(out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Walk.cs` - Security=High, Spec=none cited, `338F05`, PENDING
  - Falsified if: this method answers true for a unit in which some reachable offset is reached with two different entry states
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Walk.Step(int, int[], bool[], int, System.Collections.Generic.Queue<(int At, int[] Scopes, bool[] Slots, int Height)>, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Walk.cs` - Security=High, Spec=none cited, `76E929`, PENDING
  - Falsified if: this method reports a frame effect for an instruction that differs from what this profile's interpreter does to its operand stack and scopes
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.TryReadProgramRequest(System.ReadOnlySpan<byte>, out JsScriptUnit)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `4CF8C0`, PENDING
  - Falsified if: a payload that begins with a reserved mark, or with the eval or script mark and no well-formed request, is answered as source to compile
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.SweepAgainstTheNumericManifest(JsAssembledProgram)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `375AB9`, PENDING
  - Falsified if: an artifact naming the numeric manifest is produced carrying an instruction that manifest does not admit
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.ScanAnnexB(System.Collections.Generic.IReadOnlyList<JsStatement>, System.Collections.Generic.HashSet<string>, System.Collections.Generic.List<string>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `8DEF5C`, PENDING
  - Falsified if: a declaration is admitted whose name a `var` of the same spelling could not be added under
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.ScanAnnexBStatement(JsStatement, System.Collections.Generic.HashSet<string>, System.Collections.Generic.List<string>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `643157`, PENDING
  - Falsified if: a statement that opens a lexical record forwards the enclosing blocking set unchanged
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.ScanAnnexBBlock(System.Collections.Generic.IReadOnlyList<JsStatement>, System.Collections.Generic.HashSet<string>, System.Collections.Generic.List<string>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `CEAD6A`, PENDING
  - Falsified if: a block's own lexical names reach the test its own declarations are judged by
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.EmitAnnexBAlias(string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `84DFE0`, PENDING
  - Falsified if: the write lands on the block's own binding rather than the hoisting scope's
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.BeginDisposal(SliceSourceSpan, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `83FAFA`, PENDING
  - Falsified if: a resource registered in the list can be left without its scope being disposed, by falling off the end, a jump, a return, a throw or a forced return
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.EndDisposal(Exit)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `FDF86E`, PENDING
  - Falsified if: the handler of a resource scope runs a disposer the normal path already ran, or re-raises anything other than the folded completion or the forced return it was entered with
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.EmitDisposal(Exit, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `5B2B12`, PENDING
  - Falsified if: an asynchronous disposal is emitted into a unit that may not await, or a rejected disposer's reason escapes without being folded into the completion
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.ProtectSomething(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `EAD16F`, PENDING
  - Falsified if: a region is emitted whose start offset equals its end offset
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.UnwindAbove(int, Exit?)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `8C6605`, PENDING
  - Falsified if: an exit that owns regions is unwound without its start being answered, or a region open at the jump is left with no instruction outside the answered starts
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.CompileTemplate(JsTemplateLiteral)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `3E5E65`, PENDING
  - Falsified if: a substitution coerces through `valueOf` before `toString`, or a Symbol substitution does not throw
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.EmitToString(JsExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `A802C8`, PENDING
  - Falsified if: the two paths reach the call at different operand-stack heights
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.EmitTemplateStrings(JsTemplateLiteral)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `28BE61`, PENDING
  - Falsified if: two evaluations of one call site produce two strings objects, or the cache is reachable from guest code
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.CompileChain(JsChainExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `E0C92D`, PENDING
  - Falsified if: a link after a short-circuited one is evaluated, or the two paths meet at different heights
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.Shadowable(string, out int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `B1DA0C`, PENDING
  - Falsified if: the bound reaches a record at or beyond the binding this name resolves to
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.Shadowable(string, out int, out bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `7112BF`, PENDING
  - Falsified if: the bound reaches a record at or beyond the binding this name resolves to, or a function that may hold eval variables inside the bound is not reported
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.UnitBuffer.Leave(int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `9DE8CE`, PENDING
  - Falsified if: a region of the statement at the given level, or of one nested in it, is closed with a row covering an instruction between start and the cursor at the call
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, Spec=none cited, `27925C`, PENDING
  - Falsified if: a construct outside the numeric manifest reaches the lowering unrefused, or a construct the manifest admits is refused
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission.Statement(JsStatement, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, Spec=none cited, `6BC92C`, PENDING
  - Falsified if: a statement kind this pass does not recognise reaches the lowering
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission.Function(JsFunctionDeclaration, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, Spec=none cited, `DBB3D5`, PENDING
  - Falsified if: a function body that can fall off its end is admitted, or a function that closes over an enclosing binding is
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission.Expression(JsExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, Spec=none cited, `44F93A`, PENDING
  - Falsified if: an expression kind this pass does not recognise reaches the lowering
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission.Call(JsCallExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, Spec=none cited, `87953E`, PENDING
  - Falsified if: a call whose callee is not a function this program declares is admitted
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.ParseArrowParameters()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, Spec=none cited, `7804D2`, PENDING
  - Falsified if: an arrow's parameter list is parsed with the enclosing `[Await]` context cleared
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.TryBigIntValue(SliceToken, out System.Numerics.BigInteger)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, Spec=none cited, `9C1B5C`, PENDING
  - Falsified if: a BigInt literal's value differs from the integer its text spells, or a literal wider than the constant ceiling is converted
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.Deepen(SliceSourceSpan, out JsExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, Spec=none cited, `FEEA6B`, PENDING
  - Falsified if: a source whose tree is deeper than this bound reaches a walk
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.TemplateReader` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, Spec=none cited, `09F45E`, PENDING
  - Falsified if: this cursor ends a substitution at a different character than the tokenizer did
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.TemplateReader.ScanSubstitution()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, Spec=none cited, `90E31B`, PENDING
  - Falsified if: a brace inside a string, a comment, a nested template or an object literal closes the substitution
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, Spec=none cited, `5B0412`, PENDING
  - Falsified if: a field of a row here differs from the calling convention that row names
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi.Windows` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, Spec=none cited, `9FC3BF`, PENDING
  - Falsified if: this row names a frame-pointer register, a shadow-space size or a callee-saved set the Windows x64 convention does not
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi.SystemV` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, Spec=none cited, `38DF78`, PENDING
  - Falsified if: this row names a frame-pointer register, a shadow-space size or a callee-saved set the System V AMD64 convention does not
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi.Host` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, Spec=none cited, `BD9956`, PENDING
  - Falsified if: this is consulted anywhere on a path that decides what bytes to emit
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi.FrameBytes` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, Spec=none cited, `715C84`, PENDING
  - Falsified if: this is not a multiple of sixteen, or it does not leave room for both the shadow space and the frame-pointer spill
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi.FramePointerSlot` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, Spec=none cited, `EB800D`, PENDING
  - Falsified if: this offset lands inside the shadow space a callee may overwrite
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi.SecondArgumentRegister` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, Spec=none cited, `5EE4FB`, PENDING
  - Falsified if: this names a register other than the one the row's convention passes its second integer argument in
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Abi.BaselineFrameBytes` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` - Security=High, Spec=none cited, `9E74A2`, PENDING
  - Falsified if: a baseline unit calls a handler with the stack pointer not sixteen-aligned, or with less shadow space below it than the convention requires
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `9E1CB2`, PENDING
  - Falsified if: a branch this encoder writes carries fewer than four bytes of displacement, or a byte sequence it emits differs from the architecture manual's encoding for the instruction it names
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.TryBind(int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `CE5E20`, PENDING
  - Falsified if: a patched displacement is not the distance from the end of the branch instruction to the bind point
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.TryFinish(out byte[], out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `DFEB4F`, PENDING
  - Falsified if: bytes are handed out while any branch site still holds an unpatched displacement
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.ModRmMemory(int, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `51DD3D`, PENDING
  - Falsified if: a memory operand based on RSP or R12 is encoded without the SIB byte the architecture requires
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.Ret()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `3376B8`, PENDING
  - Falsified if: an emitted return removes any argument bytes from the stack
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.AddMemory32Immediate32(JsX64Register, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `7539CE`, PENDING
  - Falsified if: this instruction writes more than four bytes of the frame
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.SubMemory32Immediate32(JsX64Register, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `F4ECE1`, PENDING
  - Falsified if: this instruction writes more than four bytes of the frame
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.Jump(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `E457CA`, PENDING
  - Falsified if: this emits the two-byte EB form
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.JumpIf(JsX64Condition, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `212DC1`, PENDING
  - Falsified if: this emits the two-byte 7x form
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.Call(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `767753`, PENDING
  - Falsified if: a call whose target is not a code unit of the same artifact is emitted
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.CallRegister(JsX64Register)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `4A18FF`, PENDING
  - Falsified if: the backend emits one of these
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.MovsdLoad(int, JsX64Register, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `07711B`, PENDING
  - Falsified if: a REX prefix is written ahead of a mandatory SSE prefix
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.Ucomisd(int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `D1BB8D`, PENDING
  - Falsified if: a comparison this backend emits answers true for an unordered pair
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.SubRspImm8(sbyte)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `0247BA`, PENDING
  - Falsified if: this emits bytes other than 48 83 EC followed by the immediate
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.AddRspImm8(sbyte)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `C87914`, PENDING
  - Falsified if: this emits bytes other than 48 83 C4 followed by the immediate
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.MovRbxFromR14()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `97690A`, PENDING
  - Falsified if: this emits bytes other than 49 8B 1E
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.CallRbxDisp32(int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=Critical, Spec=none cited, `94AF39`, PENDING
  - Falsified if: this emits an indirect call through any base other than RBX, or bytes other than FF 93 followed by the displacement
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.JccRel32(JsX64JumpCondition)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `D48702`, PENDING
  - Falsified if: this emits a short conditional form, or answers a site that is not the first byte of the four-byte displacement
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.JmpRel32()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `ED0ED4`, PENDING
  - Falsified if: this emits the two-byte EB form, or answers a site that is not the first byte of the displacement
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Assembler.PatchRel32(int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` - Security=High, Spec=none cited, `B3F5EE`, PENDING
  - Falsified if: the patched displacement is not the distance from the end of the branch instruction to the target
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Frame` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `5FA1CA`, PENDING
  - Falsified if: a field offset here differs from the offset the runtime gives that field of JsNativeFrame
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `35D721`, PENDING
  - Falsified if: an emitted unit answers a value the interpreter does not answer for the same bytecode and the same inputs
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.SemanticVersion` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `656FFA`, PENDING
  - Falsified if: a template in this file, or the baseline block partition or layout the baseline emitter encodes, changes without this number changing
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.TryEmit(JsNativeProgramImage, out byte[], out JsNativeSymbolRow[], out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `62A1BD`, PENDING
  - Falsified if: an emission is produced in which some code unit has no emitted entry point
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.TryReadBindings(JsNativeProgramImage, out System.Collections.Generic.Dictionary<int, int>, out System.Collections.Generic.HashSet<int>, out System.Collections.Generic.HashSet<int>, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `FD3618`, PENDING
  - Falsified if: a binding this sweep records as function-valued is written a Number by any instruction of the artifact
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitUnit(JsX64Assembler, JsNativeProgramImage, int, int[], System.Collections.Generic.Dictionary<int, int>, System.Collections.Generic.HashSet<int>, System.Collections.Generic.HashSet<int>, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `9A32C1`, PENDING
  - Falsified if: a path out of an emitted unit leaves the frame's slab pointer or remaining-slot count other than it found them
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitPrologue(JsX64Assembler, JsX64UnitPlan, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `DA1CCC`, PENDING
  - Falsified if: the prologue advances the slab pointer before it has checked that the region fits
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitFuelCharge(JsX64Assembler, JsX64UnitPlan)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `BF26B8`, PENDING
  - Falsified if: a unit entry or a back edge is emitted without this sequence
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitEpilogues(JsX64Assembler, JsX64UnitPlan, int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `9BCE24`, PENDING
  - Falsified if: any exit restores the stack pointer or the callee-saved register differently from the others
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitInstruction(JsX64Assembler, JsNativeProgramImage, JsX64Walk, JsX64UnitPlan, JsFunctionRow, int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `EB7312`, PENDING
  - Falsified if: a template here computes a different value from the interpreter's case for the same instruction
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitLoadGlobal(JsX64Assembler, JsX64UnitPlan, JsX64Value[], int, int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `64963B`, PENDING
  - Falsified if: a read of an uninitialised realm binding answers a value rather than a throw
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitStoreGlobal(JsX64Assembler, JsX64UnitPlan, JsX64Value[], int, int, JsOpcode, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `BDACE8`, PENDING
  - Falsified if: an assignment to a `const` realm binding is emitted rather than refused
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitArithmetic(JsX64Assembler, JsX64UnitPlan, JsX64Value[], int, JsOpcode, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `2D960A`, PENDING
  - Falsified if: an operand that is not a Number reaches one of these instructions
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitNot(JsX64Assembler, JsX64UnitPlan, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `455D61`, PENDING
  - Falsified if: this answers false for NaN or for negative zero
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitComparison(JsX64Assembler, JsX64UnitPlan, JsX64Value[], int, JsOpcode, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `86F63E`, PENDING
  - Falsified if: a relational comparison with a NaN operand answers true
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitBranch(JsX64Assembler, JsX64UnitPlan, int, int, int, int, JsOpcode)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `483910`, PENDING
  - Falsified if: a branch to an offset at or before this one is emitted without a fuel charge
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.EmitCall(JsX64Assembler, JsNativeProgramImage, JsX64UnitPlan, JsX64Value[], int, int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `A494F3`, PENDING
  - Falsified if: a call is emitted to a unit whose parameter count differs from the argument count
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64BaselineEmitter` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64BaselineEmitter.cs` - Security=Critical, Spec=none cited, `4CA3A6`, PENDING
  - Falsified if: an emitted unit transfers control to a handler other than the one for the opcode at the head it passes, lands on an offset the managed side did not answer, or emits bytes other than the encoding of JsBaselineBlocks.Layout for the same image
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64BaselineEmitter.TryEmit(JsNativeProgramImage, JsX64Abi, uint, out byte[], out JsNativeSymbolRow[], out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64BaselineEmitter.cs` - Security=Critical, Spec=none cited, `D227C8`, PENDING
  - Falsified if: an emission is produced in which some code unit has no entry point, some block head has no handler call, or the bytes exceed the format's native-code ceiling
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64BaselineEmitter.EmitUnit(JsX64Assembler, JsNativeProgramImage, int, System.ReadOnlySpan<uint>, JsX64Abi, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64BaselineEmitter.cs` - Security=Critical, Spec=none cited, `FF794B`, PENDING
  - Falsified if: a unit's call for a block head names a slot other than eight times that head's opcode or passes a program counter other than the head's offset, the unit's body is not one template per entry of JsBaselineBlocks.Layout in its order, or a branch site is left unpatched or patched to a position other than that of the entry its target names
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64ValueEmitter` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64ValueEmitter.cs` - Security=Critical, Spec=none cited, `7CE21F`, PENDING
  - Falsified if: an emitted value-form unit is not the encoding of JsValueLayout.Layout for the same image and residency, or writes a byte sequence the value table does not admit
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64ValueEmitter.TryEmit(JsNativeProgramImage, JsX64Abi, uint, out byte[], out JsNativeSymbolRow[], out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64ValueEmitter.cs` - Security=Critical, Spec=none cited, `D227C8`, PENDING
  - Falsified if: an emission is produced in which some code unit has no entry point, or the bytes exceed the format's native-code ceiling
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64ValueEmitter.EmitUnit(JsX64Assembler, JsNativeProgramImage, int, System.ReadOnlySpan<uint>, JsX64Abi, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64ValueEmitter.cs` - Security=Critical, Spec=none cited, `859ACC`, PENDING
  - Falsified if: the unit's body is not one template per entry of JsValueLayout.Layout in its order, or a branch site is left unpatched or patched to a position other than that of the entry its target names
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64ValueEmitter.Encode(JsX64Assembler, JsX64Abi, JsValueInstruction)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64ValueEmitter.cs` - Security=Critical, Spec=none cited, `3A42CF`, PENDING
  - Falsified if: an encoding here differs from the value-table row of the same name, or a branch answers a site that is not the first byte of its displacement
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64ValueKind` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Walk.cs` - Security=High, Spec=none cited, `BF16E6`, PENDING
  - Falsified if: a value of a kind other than Number reaches an arithmetic instruction this emitter wrote
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Walk` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Walk.cs` - Security=High, Spec=none cited, `6C19B6`, PENDING
  - Falsified if: a depth-and-index pair resolves here to a slot other than the one the interpreter's scope chain would reach
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Walk.TryResolveSlot(int, int, int, out int, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Walk.cs` - Security=High, Spec=none cited, `EDDD0C`, PENDING
  - Falsified if: an index outside the named scope's declared size resolves to a slot rather than a refusal
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Walk.TryTrace(out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Walk.cs` - Security=High, Spec=none cited, `63023E`, PENDING
  - Falsified if: an offset is marked reachable with a state some path to it does not produce
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Walk.Step(int, JsX64Value[], int[], System.Collections.Generic.Stack<(int, JsX64Value[], int[])>, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Walk.cs` - Security=High, Spec=none cited, `98D4B8`, PENDING
  - Falsified if: this walk's height for an instruction differs from the format's own stack-effect table
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructCensus` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` - Security=High, Spec=none cited, `C0354E`, PENDING
  - Falsified if: a construct present in a source is not counted, or a count includes a construct the source does not contain
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructCensus.Take(System.Collections.Generic.IEnumerable<string>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` - Security=High, Spec=none cited, `B3C49F`, PENDING
  - Falsified if: a source that parses contributes no counts, or a source that does not parse is counted as containing nothing
- `Broiler.VM.Profile.JavaScript.Compiler.SliceConstructCensus.Walk(SliceNode, System.Collections.Generic.Dictionary<SliceConstructKind, int>, System.Collections.Generic.HashSet<SliceConstructKind>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` - Security=High, Spec=none cited, `3B0990`, PENDING
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
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParseOptions.MaximumTreeDepth` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParseOptions.cs` - Security=High, Spec=none cited, `6DB2AB`, PENDING
  - Falsified if: a source whose tree is deeper than this bound is compiled rather than refused
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
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseBinary(int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `70C6A7`, PENDING
  - Falsified if: the tree this builds groups an operator differently from the language's precedence and associativity
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Combine(SliceSourceSpan, SliceTokenKind, SliceExpression, SliceExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `1E98FB`, PENDING
  - Falsified if: an operator outside the manifest is built as a precise node, so the validation stage never sees it
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Precedence(SliceTokenKind)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `793D6C`, PENDING
  - Falsified if: two operators the language separates share a level here, or the order differs from the language's
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseUnary()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `49555A`, PENDING
  - Falsified if: a unary operator outside the manifest is built as a precise node, or its operand is not walked
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseCallChain()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `267636`, PENDING
  - Falsified if: a link of a chain drops its target, so a walk under it counts nothing
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParsePrimary()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `871855`, PENDING
  - Falsified if: a literal form the grammar has produces no node, so the construct it is goes uncounted
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ArrowFollows()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `4D3013`, PENDING
  - Falsified if: a parenthesised expression is parsed as a parameter list, or an arrow's head is parsed as an expression
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseArrow(SliceSourceSpan)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `1BA262`, PENDING
  - Falsified if: an arrow's parameters or body are dropped, so what is inside it counts as nothing
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ConsumeStatementTerminator()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `C160FB`, PENDING
  - Falsified if: a semicolon is inserted where the language does not insert one, or omitted where it does
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Deepen(SliceSourceSpan, out SliceExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `E5C733`, PENDING
  - Falsified if: a source whose tree is deeper than this bound reaches a walk
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Enter()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `0D6A78`, PENDING
  - Falsified if: recursion continues after this answers false
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.StatementEndsAfterCurrent()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `CA6592`, PENDING
  - Falsified if: a string literal that is not a whole statement is admitted into the directive prologue
- `Broiler.VM.Profile.JavaScript.Compiler.SliceCompilation` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `A28A22`, PENDING
  - Falsified if: a result carries artifact bytes and a diagnostic at once
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `9A3F54`, PENDING
  - Falsified if: two compilations of one source under one options value differ by a byte, or an early error reaches the verifier as an artifact
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.Compile(string, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `CCBB5B`, PENDING
  - Falsified if: a stage runs over a tree the previous stage refused
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.CompileOnTheDeclaredStack(string, SliceParseOptions)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `95FA57`, PENDING
  - Falsified if: a stage below runs on the caller's stack rather than the declared one
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
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerExpression(SliceExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `441EF6`, PENDING
  - Falsified if: any expression lowering leaves other than exactly one value on the stack
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerBinary(SliceBinaryExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `D9C53B`, PENDING
  - Falsified if: operands are evaluated out of order, or an unhandled binary operator is emitted
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourceCompiler.LowerLogical(SliceLogicalExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` - Security=High, Spec=none cited, `FEA3D4`, PENDING
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
- `Broiler.VM.Profile.JavaScript.Compiler.SliceSourcePrograms.RefusedModules` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourcePrograms.cs` - Security=High, Spec=none cited, `C3A6FF`, PENDING
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
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.VisitExpression(SliceExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `07464B`, PENDING
  - Falsified if: a subexpression is not visited, so an early error inside it goes unreported
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.VisitAssignmentTarget(SliceAssignmentExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `DB4749`, PENDING
  - Falsified if: an assignment to a `const` binding is accepted
- `Broiler.VM.Profile.JavaScript.Compiler.SliceStaticSemantics.Resolve(SliceIdentifierReference)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` - Security=High, Spec=none cited, `725C01`, PENDING
  - Falsified if: a name with no binding is accepted, or a name shadowed in an inner scope resolves to the outer one
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `5006C2`, PENDING
  - Falsified if: a second call site for this type appears, or a consumer re-reads the source text
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.Tokenize()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `7C304A`, PENDING
  - Falsified if: a token is produced after a refusal, or the stream does not end with exactly one EndOfSource
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadToken(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `415EF1`, PENDING
  - Falsified if: a character that starts an identifier is read as a punctuator, or a numeric literal is read as an identifier
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadIdentifierEscape(System.Text.StringBuilder, bool, int, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `91F171`, PENDING
  - Falsified if: an escaped identifier and its unescaped spelling are different names, or an escape naming a code point the literal spelling would refuse is admitted
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.AppendScalar(System.Text.StringBuilder, int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `F77C9C`, PENDING
  - Falsified if: an escape naming a lone surrogate throws, or a supplementary code point is not encoded as a surrogate pair
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadUnicodeEscapeValue(out int)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `6142E3`, PENDING
  - Falsified if: either spelling produces a value the language does not give it
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadNumericLiteral(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `730FCD`, PENDING
  - Falsified if: the value this produces differs from the language's MV for the same literal text
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadStringLiteral(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `B80E71`, PENDING
  - Falsified if: a directive is recognised from the string's value rather than from its raw text
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.RegularExpressionIsAllowedHere()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `6559F9`, PENDING
  - Falsified if: a division after a value is read as a regular expression, or a literal after an operator is read as a division
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadRegularExpressionLiteral(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `C7126B`, PENDING
  - Falsified if: a `/` inside a character class ends the literal
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ReadTemplateLiteral(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `9ABBF8`, PENDING
  - Falsified if: a template, string, comment or object literal inside a substitution ends the outer literal
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ScanTemplateBody()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `0FABF8`, PENDING
  - Falsified if: a substitution consumes the backtick that closes the template it belongs to
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ScanSubstitution()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `00BA21`, PENDING
  - Falsified if: a brace inside a string, comment, nested template or object literal closes the substitution
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ScanStringBody(char)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `6DD49B`, PENDING
  - Falsified if: an escaped quote ends the string, or an unterminated one swallows the rest of the source
- `Broiler.VM.Profile.JavaScript.Compiler.SliceTokenizer.ScanRegularExpressionBody()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` - Security=High, Spec=none cited, `685B24`, PENDING
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
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `DD3F28`, PENDING
  - Falsified if: for some defined opcode RunsAlone, EndsBlock and StopsAfter disagree, or TryPlan, LayoutLength or Layout answers differently for the same image, unit and handler offsets
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.Table` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `58E210`, PENDING
  - Falsified if: for a defined byte the entry's width or either bit differs from JsOpcodes.InstructionWidth, EndsBlock or RunsAlone, or an undefined byte's entry lacks either bit
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.RunsAlone(JsOpcode)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `C25AED`, PENDING
  - Falsified if: an opcode of the call class is not named here, or an opcode not named here has an arm or an unconditionally called helper that reaches guest or embedder code other than behind the object test of ToPrimitive or Render, the getter or setter test of Lookup, GetSymbol, SetProperty, SetSymbol, SetWithReceiver, SetSuper, GetSymbolWithReceiver, SetSymbolWithReceiver, ReadPrivate or WritePrivate, the dispatch to JsProxy or JsHostObject, or InstanceOf's test for a Symbol.hasInstance method
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.EndsBlock(JsOpcode)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `4AA9FF`, PENDING
  - Falsified if: it answers false for an opcode that is terminal, has a code target or runs alone, or true for any other opcode
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.StopsAfter(byte[], int, int, int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `DD109B`, PENDING
  - Falsified if: for a defined byte at current it answers other than EndsBlock of that byte, or pc other than current plus that byte's width, or pc at or past unitEnd, or RunsAlone of the byte at pc - or it reads the byte at pc when pc is at or past unitEnd
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.GroupHandlerOffsets(JsNativeProgramImage)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `5CECB9`, PENDING
  - Falsified if: for some unit of the image the slice it answers differs from the handler offsets of the image's regions whose function index is that unit, in the order the image carries them
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.TryPlan(JsNativeProgramImage, int, System.ReadOnlySpan<uint>, out JsBaselineUnitPlan, out string)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `D47050`, PENDING
  - Falsified if: a plan it answers omits the unit's entry, a handler offset it was given, the successor of a Yield, Await or EnterBody or a YieldDelegate from its landings, names a head that is not an instruction start of the unit, gives a head a last instruction other than the first after which StopsAfter holds on its linear walk, or it answers differently for the same image, unit and handler offsets
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.TryPlanEachInstruction(JsNativeProgramImage, int, System.ReadOnlySpan<uint>, out JsBaselineUnitPlan, out string)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `580104`, PENDING
  - Falsified if: a plan it answers has a block that is not exactly one instruction, omits an instruction of the unit from its heads, or has landings, tails or refusals other than TryPlan's for the same image, unit and handler offsets
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.Plan(JsNativeProgramImage, int, System.ReadOnlySpan<uint>, bool, out JsBaselineUnitPlan, out string)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `358E53`, PENDING
  - Falsified if: with eachInstruction false it answers differently from the baseline partition TryPlan documents, or with it true some block is longer than one instruction
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.LayoutLength(JsBaselineUnitPlan)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `4C8676`, PENDING
  - Falsified if: for a plan TryPlan answered it differs from the length of the array Layout answers for the same plan
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.Layout(JsBaselineUnitPlan)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `CAE84F`, PENDING
  - Falsified if: for a plan TryPlan answered, the layout's dispatch sends a landing anywhere but its own head's call or another non-negative answer anywhere but the defect, a head's call names a pc other than the head or a slot other than eight times its opcode, a tail differs from the one its block's kind and target dictate, a branch resolves to an index other than the instruction it names, or the answer's length differs from LayoutLength
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.Lay<TSink>(JsBaselineUnitPlan, ref TSink)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `A0E4CD`, PENDING
  - Falsified if: for a plan TryPlan answered, it hands out an entry other than the one Layout writes at the index it names, hands out indices other than zero upward one at a time, goes on after the sink answers false, or answers true when it stopped before the last entry
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.Tree<TSink>(ref TSink, ref int, System.ReadOnlySpan<int>, int, int, int, System.ReadOnlySpan<JsBaselineBlock>, int[])` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `0D7902`, PENDING
  - Falsified if: a landing in the range is compared anywhere but on a path that branches to its own head, an offset outside the range reaches anything but the defect, it hands out other than TreeLength entries for the range, or it goes on after the sink answers false
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.TreeLength(int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `A8605C`, PENDING
  - Falsified if: for some count it differs from the number of entries Tree writes for a range of that many landings
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.BlockLength(JsBaselineBlock)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `AB6E97`, PENDING
  - Falsified if: for some block it differs from the number of entries Layout writes for that block's call and tail
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.HeadIndex(System.ReadOnlySpan<JsBaselineBlock>, int[], int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `E31FC6`, PENDING
  - Falsified if: it answers an index other than the first entry of the block whose head is at the offset, or an index when no block's head is there
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineBlocks.Build()` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineBlocks.cs` - Security=High, Spec=none cited, `107A7E`, PENDING
  - Falsified if: for some byte the entry it writes differs from the one the Table remark states
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineStatus` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineFrame.cs` - Security=High, Spec=none cited, `B08268`, PENDING
  - Falsified if: a handler or unit answers a negative value other than these three, or a non-negative value that is not the offset of an instruction start
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineFrame` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineFrame.cs` - Security=Critical, Spec=none cited, `654AD3`, PENDING
  - Falsified if: a field of this structure is, or contains, a reference the collector traces, or its offsets differ from the constants in JsBaselineAbi
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineAbi` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineFrame.cs` - Security=High, Spec=none cited, `5343D7`, PENDING
  - Falsified if: a constant here disagrees with the layout the runtime gives JsBaselineFrame, or with the reservation the emitted prologue makes
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineAbi.HandlersOffset` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineFrame.cs` - Security=High, Spec=none cited, `A0D518`, PENDING
  - Falsified if: the runtime places JsBaselineFrame.Handlers at any other offset
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineAbi.CookieOffset` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineFrame.cs` - Security=High, Spec=none cited, `5C496D`, PENDING
  - Falsified if: the runtime places JsBaselineFrame.Cookie at any other offset
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineAbi.FrameSize` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineFrame.cs` - Security=High, Spec=none cited, `168EAF`, PENDING
  - Falsified if: the runtime gives JsBaselineFrame any other size
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineAbi.HandlerSlots` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineFrame.cs` - Security=High, Spec=none cited, `7D95B4`, PENDING
  - Falsified if: an emitted call can index the table at or beyond this many slots
- `Broiler.VM.Profile.JavaScript.Format.JsBaselineAbi.FrameBytes(JsNativeArchitecture)` in `src/Broiler.VM.Profile.JavaScript.Format/JsBaselineFrame.cs` - Security=High, Spec=none cited, `ED86E7`, PENDING
  - Falsified if: a baseline unit calls a handler with the stack pointer not sixteen-aligned or with less shadow space than the convention requires
- `Broiler.VM.Profile.JavaScript.Format.JsFormat.ScriptRequestMark` in `src/Broiler.VM.Profile.JavaScript.Format/JsFormat.cs` - Security=High, Spec=none cited, `F9CFC2`, PENDING
  - Falsified if: a guest-initiated load can send a payload that begins with this byte
- `Broiler.VM.Profile.JavaScript.Format.JsNativeValues` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, Spec=none cited, `5F6922`, PENDING
  - Falsified if: an arithmetic operation over ordinary Numbers produces either of these bit patterns
- `Broiler.VM.Profile.JavaScript.Format.JsNativeValues.UninitialisedBits` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, Spec=none cited, `10ECDD`, PENDING
  - Falsified if: a realm binding is readable by emitted code before an initialiser stored to it
- `Broiler.VM.Profile.JavaScript.Format.JsNativeProgramImage.Tier` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, Spec=none cited, `6AB00A`, PENDING
  - Falsified if: the compiler and the verifier build images of one artifact with different tiers
- `Broiler.VM.Profile.JavaScript.Format.JsNativeProgramImage.ResidentBindings` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, Spec=none cited, `A94B56`, PENDING
  - Falsified if: the compiler and the verifier build images of one value-form artifact with different residency
- `Broiler.VM.Profile.JavaScript.Format.JsNativeAbiProbe` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, Spec=none cited, `68CB0A`, PENDING
  - Falsified if: a field offset here differs from the offset the trampoline that fills it computes
- `Broiler.VM.Profile.JavaScript.Format.JsNativeAbiProbeLayout` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, Spec=none cited, `A42497`, PENDING
  - Falsified if: an offset here differs from the offset the runtime gives that field
- `Broiler.VM.Profile.JavaScript.Format.JsNativeFrame` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeFrame.cs` - Security=High, Spec=none cited, `B132E7`, PENDING
  - Falsified if: a field of this structure holds a reference the collector traces, or its declared offsets differ from the layout the runtime gives it
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=High, Spec=none cited, `31ACE8`, PENDING
  - Falsified if: a payload this scan accepts carries a byte no backend of this build could have emitted
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.Scan(JsNativeArchitecture, byte[], JsNativeSymbolRow[], uint, System.Collections.Generic.ICollection<string>?)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=High, Spec=none cited, `61C6A9`, PENDING
  - Falsified if: a byte sequence no backend of this build can emit is accepted, or a sequence one of them emits is refused
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.Scan(JsNativeArchitecture, JsNativeTier, byte[], JsNativeSymbolRow[], uint, JsNativeProgramImage?, System.Collections.Generic.ICollection<string>?)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=Critical, Spec=none cited, `4933B4`, PENDING
  - Falsified if: a baseline x86-64 payload is accepted whose unit writes RBX or RSP outside one prologue and one epilogue, branches into either sequence, or makes an indirect call other than through the handler table at a defined opcode's slot, or whose unit body is not, instruction for instruction, the layout JsBaselineBlocks gives for the program's own partition
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.ValueLayout(JsNativeTemplate[], byte[], JsNativeSymbolRow[], JsNativeProgramImage?, System.Collections.Generic.List<(uint At, int Index)>, int[])` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=Critical, Spec=none cited, `9D805F`, PENDING
  - Falsified if: it accepts a value-form payload with no program, with a program whose units are not the symbols', or with a unit body that is not, instantiation for instantiation, the layout of that unit's value plan under the image's residency - or it refuses a body that is
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.ValueRefusal(JsValueInstruction, int, uint, string)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=High, Spec=none cited, `9E5B74`, PENDING
  - Falsified if: a differing guard answers other than GuardNotItsHelper, a differing debt entry other than DebtNotSettled, a differing inline entry other than InlineNotTheInstruction, a differing direct-call entry other than CallNotDirect, or a differing dispatch, call or tail other than the baseline clause's outcome
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.OwnedBy(JsNativeSymbolRow[], JsNativeProgramImage?)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=Critical, Spec=none cited, `633AEA`, PENDING
  - Falsified if: it accepts no program, a program with a unit count other than the symbols', or a symbol that names a function other than its own position
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.BlockLayout(JsNativeTemplate[], byte[], JsNativeSymbolRow[], JsNativeProgramImage?, System.Collections.Generic.List<(uint At, int Index)>, int[])` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=Critical, Spec=none cited, `892601`, PENDING
  - Falsified if: it accepts a payload with no program, with a program whose unit count or order is not the symbols', or with a unit body that is not, instruction for instruction, the layout of that unit's plan with the unit's own handler offsets - or it refuses a body that is
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.BaselineTemplateIndices(JsNativeTemplate[])` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=High, Spec=none cited, `E4A85A`, PENDING
  - Falsified if: for some JsBaselineTemplate it answers the index of a template whose name is not the one this file gives that member, or a non-negative index when the table has no template of that name
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.BaselineTemplateNames` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=High, Spec=none cited, `F15041`, PENDING
  - Falsified if: an entry names a template other than the one the JsBaselineTemplate member of its index documents
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.LayoutComparison` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=Critical, Spec=none cited, `A69F72`, PENDING
  - Falsified if: it goes on after an entry whose template, operand or branch destination differs from the instantiation at the same index of the body, or stops at one whose three agree
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.LayoutComparison.Take(int, JsBaselineInstruction)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=Critical, Spec=none cited, `D7A644`, PENDING
  - Falsified if: it answers true for an entry whose template, operand or branch destination differs from the instantiation at its index, or false for one whose three agree
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.LayoutComparison.Refusal(int, uint)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=High, Spec=none cited, `53B907`, PENDING
  - Falsified if: a differing dispatch entry answers other than DispatchNotTheLandings, a differing move answers other than CallsNotTheBlockHeads, a call through another slot answers other than HandlerSlotNotTheOpcode, another template where a call belongs answers other than CallsNotTheBlockHeads, or a differing tail entry answers other than TailNotTheBlockEnd
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.FrameShape(System.Collections.Generic.List<(uint At, int Index)>, int, int, int, int, uint, int, System.Collections.Generic.HashSet<uint>)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=Critical, Spec=none cited, `E73BFD`, PENDING
  - Falsified if: a baseline unit is accepted whose instantiations do not open with exactly the prologue, close with exactly the epilogue, or carry a prologue or epilogue template elsewhere
- `Broiler.VM.Profile.JavaScript.Format.JsNativeScan.FrameTargets(System.Collections.Generic.List<Branch>, System.Collections.Generic.HashSet<uint>)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeScan.cs` - Security=Critical, Spec=none cited, `6A1345`, PENDING
  - Falsified if: a baseline payload is accepted with a branch whose target is a prologue instruction, a pop or a return
- `Broiler.VM.Profile.JavaScript.Format.JsNativeTemplates` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` - Security=High, Spec=none cited, `14F65E`, PENDING
  - Falsified if: a template here differs from the bytes the encoder method it names emits, or a byte sequence a backend emits matches no template here
- `Broiler.VM.Profile.JavaScript.Format.JsNativeTemplates.For(JsNativeArchitecture, JsNativeTier)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` - Security=Critical, Spec=none cited, `F4C459`, PENDING
  - Falsified if: a numeric payload is judged against any table other than the one For(architecture) answers, or a baseline x86-64 payload against a table that admits a template the baseline emitter does not write
- `Broiler.VM.Profile.JavaScript.Format.JsNativeTemplates.IsX64Baseline(JsNativeTemplate[])` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` - Security=High, Spec=none cited, `8C4B1E`, PENDING
  - Falsified if: a baseline x86-64 table is scanned without the frame-shape clauses
- `Broiler.VM.Profile.JavaScript.Format.JsNativeTemplates.IsX64Value(JsNativeTemplate[])` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` - Security=High, Spec=none cited, `15D085`, PENDING
  - Falsified if: a value x86-64 payload is held to the baseline layout, or a baseline payload to the value form's
- `Broiler.VM.Profile.JavaScript.Format.JsNativeTemplates.Admits(JsNativeFieldKind, long)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` - Security=High, Spec=none cited, `86C513`, PENDING
  - Falsified if: a value no backend of this build asks an encoder for is admitted by one of these arms
- `Broiler.VM.Profile.JavaScript.Format.JsNativeTemplates.windowsBaseline` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` - Security=High, Spec=none cited, `86A9C7`, PENDING
  - Falsified if: an entry of this table differs from the bytes the baseline emitter writes for Windows x64
- `Broiler.VM.Profile.JavaScript.Format.JsNativeTemplates.systemVBaseline` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` - Security=High, Spec=none cited, `10255D`, PENDING
  - Falsified if: an entry of this table differs from the bytes the baseline emitter writes for System V
- `Broiler.VM.Profile.JavaScript.Format.JsNativeTemplates.windowsValue` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` - Security=High, Spec=none cited, `D4D2F5`, PENDING
  - Falsified if: an entry of this table differs from the bytes the value emitter writes for Windows x64
- `Broiler.VM.Profile.JavaScript.Format.JsNativeTemplates.systemVValue` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` - Security=High, Spec=none cited, `0BB002`, PENDING
  - Falsified if: an entry of this table differs from the bytes the value emitter writes for System V
- `Broiler.VM.Profile.JavaScript.Format.JsNativeTemplates.X64Value(JsNativeArchitecture)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` - Security=Critical, Spec=none cited, `52AD49`, PENDING
  - Falsified if: a template here writes memory other than a region word or the debt word, dereferences a register an inline template loads a word into, makes an indirect transfer other than through RBX at a defined opcode's slot or the settlement, prepare or finish slot or through the callee context's entry field, or is a byte sequence the value emitter does not write
- `Broiler.VM.Profile.JavaScript.Format.JsNativeTemplates.X64Baseline(JsNativeArchitecture)` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeTemplates.cs` - Security=Critical, Spec=none cited, `4BCDE9`, PENDING
  - Falsified if: a template here admits an indirect transfer other than the handler-table call, a memory write, or a byte sequence the baseline emitter does not write
- `Broiler.VM.Profile.JavaScript.Format.JsValueFrame` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=Critical, Spec=none cited, `5A1791`, PENDING
  - Falsified if: a field of this structure is, or contains, a reference the collector traces, or its offsets differ from the constants in JsValueAbi
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `D6B895`, PENDING
  - Falsified if: a constant here disagrees with the layout the runtime gives JsValueFrame, or with the offsets the value form's emitted code reads
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.HelpersOffset` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `D2E11C`, PENDING
  - Falsified if: the runtime places JsValueFrame.Helpers at any other offset, or it differs from JsBaselineAbi.HandlersOffset
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.CookieOffset` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `5C496D`, PENDING
  - Falsified if: the runtime places JsValueFrame.Cookie at any other offset
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.RegionOffset` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `604F4C`, PENDING
  - Falsified if: the runtime places JsValueFrame.Region at any other offset
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.DebtOffset` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `21C096`, PENDING
  - Falsified if: the runtime places JsValueFrame.Debt at any other offset
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.FrameSize` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `D34DD6`, PENDING
  - Falsified if: the runtime gives JsValueFrame any other size
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.EntryOffset` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `232D6F`, PENDING
  - Falsified if: the runtime places JsValueFrame.Entry at any other offset
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.EntryPcOffset` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `0828B6`, PENDING
  - Falsified if: the runtime places JsValueFrame.EntryPc at any other offset
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.HelperSlots` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `A738D5`, PENDING
  - Falsified if: an emitted call can index the helper table at or beyond this many slots
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.SettleSlot` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `32CE45`, PENDING
  - Falsified if: an opcode this format defines has this byte, or an emitted settlement calls any other slot
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.PrepareSlot` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `96F399`, PENDING
  - Falsified if: an opcode this format defines has this byte, or an emitted direct call site calls any other slot first
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.FinishSlot` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `19C3FB`, PENDING
  - Falsified if: an opcode this format defines has this byte, or an emitted direct call is followed by a call of any other slot
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.DirectCall` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `EAD516`, PENDING
  - Falsified if: this equals a JsBaselineStatus or can be an instruction offset
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.Returned` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `B914F5`, PENDING
  - Falsified if: this equals a JsBaselineStatus or DirectCall, or a unit answers it with any word but its returned value in its region's first word
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.CalleeOffset(JsNativeArchitecture)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `3500E3`, PENDING
  - Falsified if: the context overlaps the shadow space, the saved registers or the return address, or reaches past the frame the prologue reserves
- `Broiler.VM.Profile.JavaScript.Format.JsValueAbi.FrameBytes(JsNativeArchitecture)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `90A8DF`, PENDING
  - Falsified if: a value-form unit calls a helper or a direct callee with the stack pointer not sixteen-aligned, with less shadow space than the convention requires, or with a callee context that does not fit the frame
- `Broiler.VM.Profile.JavaScript.Format.JsNativeCodeHeader` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `062001`, PENDING
  - Falsified if: a header field Pack writes is read back by TryUnpack as another architecture, another form or another residency, a field with a nonzero upper half or a form byte other than zero and the value form's two is read at all, or a numeric or baseline header is written with a nonzero form byte
- `Broiler.VM.Profile.JavaScript.Format.JsNativeCodeHeader.Pack(JsNativeArchitecture, bool, bool)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `66AAF0`, PENDING
  - Falsified if: it writes a form byte other than zero for a section that is not the value form, or other than the value tier's number, with FlatBit exactly when residency is withheld, for one that is
- `Broiler.VM.Profile.JavaScript.Format.JsNativeCodeHeader.TryUnpack(uint, out JsNativeArchitecture, out bool, out bool)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueFrame.cs` - Security=High, Spec=none cited, `F3FE38`, PENDING
  - Falsified if: it answers true for a field whose upper sixteen bits are not zero or whose form byte is neither zero, the value tier's number, nor that number with FlatBit
- `Broiler.VM.Profile.JavaScript.Format.JsValueInline` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `231C64`, PENDING
  - Falsified if: a member other than None names an instruction outside the pure set, or an instruction the pure set holds has no member
- `Broiler.VM.Profile.JavaScript.Format.JsValueUnitPlan` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `DEAE80`, PENDING
  - Falsified if: a plan answers a height, a depth, a residency or an inline decision other than the walk and the analysis JsValueLayout.TryPlan documents compute for the same image, unit, handler offsets and residency flag
- `Broiler.VM.Profile.JavaScript.Format.JsValueUnitPlan.StackBase` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `2875C9`, PENDING
  - Falsified if: it answers a word other than the first one after the arguments and the resident bindings
- `Broiler.VM.Profile.JavaScript.Format.JsValueUnitPlan.Suspends` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `C89275`, PENDING
  - Falsified if: it answers false for a generator's or an async function's unit, or true for any other
- `Broiler.VM.Profile.JavaScript.Format.JsValueUnitPlan.HeightAt(int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `782F5C`, PENDING
  - Falsified if: it answers a height for an offset outside the unit or not an instruction start, or a height other than the verifier's abstract height there
- `Broiler.VM.Profile.JavaScript.Format.JsValueUnitPlan.DepthAt(int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `74F33E`, PENDING
  - Falsified if: it answers a depth other than the verifier's scope depth at a reached instruction
- `Broiler.VM.Profile.JavaScript.Format.JsValueUnitPlan.InlineAt(int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `9D43C3`, PENDING
  - Falsified if: it answers an inline kind for an instruction outside the pure set, for one the walk did not reach, or for a scoped instruction whose binding is not resident
- `Broiler.VM.Profile.JavaScript.Format.JsValueUnitPlan.ResidentBaseOf(int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `941DCD`, PENDING
  - Falsified if: two resident environments share a word, a resident word overlaps the arguments or the operand stack, or a depth a closure, an eval, a with, a name search or a mapped arguments object can reach answers a word
- `Broiler.VM.Profile.JavaScript.Format.JsValueUnitPlan.ResidentSlotsOf(int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `3D2019`, PENDING
  - Falsified if: it answers fewer words than an instance of the environment at that depth declares
- `Broiler.VM.Profile.JavaScript.Format.JsValueUnitPlan.OperandAt(int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `594A0E`, PENDING
  - Falsified if: it answers an operand other than the word, argument, region word or pick depth the instruction's inline kind reads
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `758365`, PENDING
  - Falsified if: an inline template's effect differs from its arm's for an operand its guards admit, a guard branches anywhere but its own instruction's helper stub, a backward inline branch carries no debt test, a binding is resident that a closure, an eval, a with, a name search or a mapped arguments object can reach, or TryPlan or Layout answers differently for the same image, unit, handler offsets and residency flag
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.DebtThreshold` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `A922D6`, PENDING
  - Falsified if: an emitted debt test compares against any other value
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.CeilingRegionWords` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `289852`, PENDING
  - Falsified if: a region an emitted template addresses holds more words than this
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Names` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `FF4199`, PENDING
  - Falsified if: an entry names a row other than the one the JsValueTemplate member of its index documents
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.IsSuspension(JsOpcode)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `BC9016`, PENDING
  - Falsified if: an arm of the dispatch loop can suspend its frame and its opcode is not one of these
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.TemplateName(JsValueTemplate)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `E26DA3`, PENDING
  - Falsified if: it answers a name other than the one of the row the member documents
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.TryPlan(JsNativeProgramImage, int, System.ReadOnlySpan<uint>, bool, out JsValueUnitPlan, out string)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `73F3D8`, PENDING
  - Falsified if: a plan it answers has a height or depth other than the verifier's at some instruction, a resident depth the remark refuses, or an inline decision for an instruction whose height is not known
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Layout(JsValueUnitPlan)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `8394EE`, PENDING
  - Falsified if: an entry of the answer differs from the template, operand and target the remark and the inline set document for its instruction, or a branch resolves to an index other than the entry it names
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Walk(JsNativeProgramImage, int, int, int, int[], int[], out string)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `7D275C`, PENDING
  - Falsified if: it answers a height or depth at an instruction other than the verifier's, or accepts a join of two different states
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Seed(int, int, int, int, int[], int[], System.Collections.Generic.Stack<int>, out string)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `732B2F`, PENDING
  - Falsified if: it records a state at an offset outside the unit, or accepts a second state that differs from the first
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Decide(JsNativeProgramImage, JsOpcode, uint, int, int[], int[], int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `DF103D`, PENDING
  - Falsified if: an instruction outside the pure set is given an inline kind, or a scoped instruction is inline for a binding that is not resident
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `B5C9F0`, PENDING
  - Falsified if: the layout it answers is not the one the remark on JsValueLayout.Layout documents for the plan it was handed
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.Build()` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `D62CB8`, PENDING
  - Falsified if: the entries it adds are not the dispatch, then one sequence per instruction in bytecode order, then the stubs those sequences asked for
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.Finish()` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `5FA585`, PENDING
  - Falsified if: an entry is answered with a target other than the index its label was bound at, or an unbound label resolves to anything but a refusal
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.Helper(JsBaselineBlock, bool, bool)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `C924A8`, PENDING
  - Falsified if: a helper call passes a program counter other than its instruction's or a slot other than eight times that opcode, is not preceded by a spill and a clear of the debt, or a stub's call does not first take its instruction's count back
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.DirectCall(JsBaselineBlock)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `AFB86F`, PENDING
  - Falsified if: a direct call site passes a program counter other than its instruction's, calls anything but the prepare slot first, calls the callee's entry without the prepare helper having answered the direct call, is not followed by the finish helper with the callee's status, or is not preceded by a spill and a clear of the debt
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.Tail(JsBaselineBlock, bool)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `B410A8`, PENDING
  - Falsified if: an answer goes anywhere but the instruction's target, its successor or the dispatch
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.Slow(JsBaselineBlock, ref int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `514138`, PENDING
  - Falsified if: a guard's stub calls any instruction's helper but its own, or one instruction is given two stubs
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.DebtTest(int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `EA8806`, PENDING
  - Falsified if: a debt test compares against any value but the threshold, or settles to resume anywhere but the offset it names
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.Settlement(int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `45CA33`, PENDING
  - Falsified if: a settlement stub resumes anywhere but its offset, or calls anything but the settlement slot
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.Transfer(int, int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `416307`, PENDING
  - Falsified if: a backward inline branch reaches its target without passing a debt test
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.Inline(JsBaselineBlock)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `FEE111`, PENDING
  - Falsified if: an inline template writes a word before its last guard, writes a word other than its arm's outputs, or computes a value other than its arm's for an operand its guards admit
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.Branch(JsBaselineBlock, int, ref int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `4495BA`, PENDING
  - Falsified if: a Boolean, a Number, undefined or null goes the other way from the arm's ToBoolean, or any other word goes anywhere but the helper
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.OneNumber(JsBaselineBlock, int, ref int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `CBCEEF`, PENDING
  - Falsified if: a word that is not a Number passes the guard
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.TwoNumbers(JsBaselineBlock, int, ref int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `1E6A99`, PENDING
  - Falsified if: a pair of which either word is not a Number passes the guards
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.Compare(JsOpcode, int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `51A6EB`, PENDING
  - Falsified if: a comparison answers true for an unordered pair where the arm answers false, or differs from the arm for any ordered pair
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.Slot(int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=Critical, Spec=none cited, `987B00`, PENDING
  - Falsified if: a stack slot resolves to a word outside the region's operand stack
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.Tree(System.ReadOnlySpan<int>, int, int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `1B4948`, PENDING
  - Falsified if: a landing is compared anywhere but on a path to its own instruction, or an offset that is no landing reaches anything but the defect
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.Guard(JsValueTemplate, int, int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `02B717`, PENDING
  - Falsified if: a guard is recorded with any role but Guard or any target but the stub it was handed
- `Broiler.VM.Profile.JavaScript.Format.JsValueLayout.Builder.LabelOf(int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsValueLayout.cs` - Security=High, Spec=none cited, `93754D`, PENDING
  - Falsified if: it answers the label of an instruction other than the one at the offset, or any label for an offset that is no instruction's
- `Broiler.VM.Profile.JavaScript.Format.JsWord` in `src/Broiler.VM.Profile.JavaScript.Format/JsWord.cs` - Security=High, Spec=none cited, `D89940`, PENDING
  - Falsified if: a word this type classifies as a Number is also classified as a tagged value, or a tagged word can be produced by IEEE-754 arithmetic on two Numbers
- `Broiler.VM.Profile.JavaScript.Format.JsWord.FirstTag` in `src/Broiler.VM.Profile.JavaScript.Format/JsWord.cs` - Security=High, Spec=none cited, `B77857`, PENDING
  - Falsified if: the default NaN of an SSE2 operation, or its sign flip, has top sixteen bits at or above this value
- `Broiler.VM.Profile.JavaScript.Format.JsWord.Empty` in `src/Broiler.VM.Profile.JavaScript.Format/JsWord.cs` - Security=High, Spec=none cited, `398351`, PENDING
  - Falsified if: an empty slot and undefined decode to the same value, so a read in the temporal dead zone is not refused
- `Broiler.VM.Profile.JavaScript.Format.JsWord.CanonicalNaN` in `src/Broiler.VM.Profile.JavaScript.Format/JsWord.cs` - Security=High, Spec=none cited, `D48E42`, PENDING
  - Falsified if: FromNumber answers for a NaN that sets a bit of NaNTagReach a word other than this one, or this word is not a quiet NaN
- `Broiler.VM.Profile.JavaScript.Format.JsWord.NaNTagReach` in `src/Broiler.VM.Profile.JavaScript.Format/JsWord.cs` - Security=High, Spec=none cited, `A3C52F`, PENDING
  - Falsified if: a NaN word with these bits clear reaches a tag under a sign flip, the setting of the quiet bit or both
- `Broiler.VM.Profile.JavaScript.Format.JsWord.IsNumber(ulong)` in `src/Broiler.VM.Profile.JavaScript.Format/JsWord.cs` - Security=High, Spec=none cited, `A20747`, PENDING
  - Falsified if: this answers true for a word carrying any tag from FirstTag upwards, or false for a word below it
- `Broiler.VM.Profile.JavaScript.Format.JsWord.IsHandle(ulong)` in `src/Broiler.VM.Profile.JavaScript.Format/JsWord.cs` - Security=High, Spec=none cited, `5FE785`, PENDING
  - Falsified if: this answers true for a special constant, a frame header, a reserved word or a Number
- `Broiler.VM.Profile.JavaScript.Format.JsWord.FromNumber(double)` in `src/Broiler.VM.Profile.JavaScript.Format/JsWord.cs` - Security=High, Spec=none cited, `FC491C`, PENDING
  - Falsified if: some double answers a word IsNumber rejects, a double that is not a NaN setting a bit of NaNTagReach does not answer its own bit pattern, or one that is answers other than CanonicalNaN
- `Broiler.VM.Profile.JavaScript.Format.JsWord.Handle(ushort, ushort, uint)` in `src/Broiler.VM.Profile.JavaScript.Format/JsWord.cs` - Security=High, Spec=none cited, `205016`, PENDING
  - Falsified if: the word answered does not carry the tag, the generation and the index it was given, or carries a tag outside the four handle tags
- `Broiler.VM.Profile.JavaScript.Format.JsWord.Header(int, int)` in `src/Broiler.VM.Profile.JavaScript.Format/JsWord.cs` - Security=High, Spec=none cited, `BC255B`, PENDING
  - Falsified if: a header answers a region or live length other than the ones it was built from, or is built with more live words than its region holds
- `Broiler.VM.Profile.JavaScript.JavaScriptReadAdapter` in `src/Broiler.VM.Profile.JavaScript/JavaScriptDiagnostics.cs` - Security=High, Spec=none cited, `1DD7A4`, PENDING
  - Falsified if: a charge made through this adapter reaches a dimension other than the one named, or a released byte count is charged rather than released
- `Broiler.VM.Profile.JavaScript.JavaScriptInstance` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `E818FA`, PENDING
  - Falsified if: two instances over one shared handle observe each other's locals, or any instance state is reachable from that handle
- `Broiler.VM.Profile.JavaScript.JavaScriptContinuation` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `38E70F`, PENDING
  - Falsified if: this milestone constructs one, or a resume presented with one is answered as anything but a contract violation
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `E9D7AE`, PENDING
  - Falsified if: any input makes a member here throw, or an answer is produced that is not one of the five step kinds
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Instantiate(VmVerifiedArtifact, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `C59E32`, PENDING
  - Falsified if: a handle this profile did not verify produces an instance
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Invoke(IVmInstanceState, in VmInvocationRequest, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `89BE7A`, PENDING
  - Falsified if: an unknown entry point is reported as anything but a language fault, or a foreign instance state runs
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Run(JavaScriptInstance, int)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `1C127C`, PENDING
  - Falsified if: the operand stack is sized from anything but the maximum the verifier computed, or an index used here was not proved in range before execution
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `EA198F`, PENDING
  - Falsified if: a second static accessor or an aggregate profile-listing type appears in this graph, or the descriptor accepts a manifest this build does not implement
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.TurnEntryPoint` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `E75436`, PENDING
  - Falsified if: a turn runs outside a step, or reaches an embedder no composition registered
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.HostSurfaceBindingIndex` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `F33DA9`, PENDING
  - Falsified if: a realm carries a host object in a composition that did not register this slot
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.SourceProviderCapability` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `0E9FDF`, PENDING
  - Falsified if: this profile obtains executable bytes by any route but a provider registered under this identity
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.ResolveCapability` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `D7A399`, PENDING
  - Falsified if: this profile opens a file, follows a specifier, or honours a module request the host was not asked to rule on
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.HostSurfaceCapability` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `FA1E2A`, PENDING
  - Falsified if: this profile invokes this binding rather than only asking whether it is bound
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.DescriptorAdmitting(params VmFeatureManifestId[])` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `A90981`, PENDING
  - Falsified if: a descriptor built here accepts an optional surface its caller did not name
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.DescriptorReEmittingWith(Format.IJsNativeEmitter, params VmFeatureManifestId[])` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `F076BD`, PENDING
  - Falsified if: a descriptor built here admits a native payload whose bytes its emitter does not reproduce
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.DescriptorHostingRealms(IJsHostSurface, params VmFeatureManifestId[])` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `11215D`, PENDING
  - Falsified if: a descriptor built here installs a surface in a runtime that registered no permission
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.DescriptorUnderHandleStress(IJsHostSurface?, params VmFeatureManifestId[])` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `A66C15`, PENDING
  - Falsified if: a descriptor built here runs a value-form instance whose table does not compact at every safepoint, or changes anything for an instance of another form
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.Build(ImmutableArray<string>, Format.IJsNativeEmitter?, IJsHostSurface?, bool)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `319B97`, PENDING
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
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.JavaScriptVerifier(VmProfileId, VmFeatureManifestId, System.Collections.Immutable.ImmutableArray<string>, Format.IJsNativeEmitter?)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `8BF62C`, PENDING
  - Falsified if: a native payload is admitted whose bytes this emitter, where there is one, does not reproduce
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.reEmitter` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `A13BA0`, PENDING
  - Falsified if: this is non-null in an image whose closure carries no code generator
- `Broiler.VM.Profile.JavaScript.JavaScriptVerifier.Verify(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, IVmVerificationContext, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` - Security=High, Spec=none cited, `2B5921`, PENDING
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
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=Critical, Spec=none cited, `20AD4A`, PENDING
  - Falsified if: an emitted call through this table reaches code other than one step of the dispatch loop or the refusing entry point
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.PerOpcodeSteps` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `6B18D3`, PENDING
  - Falsified if: the two settings produce a different JavaScript answer for any program
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Table` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=Critical, Spec=none cited, `FD6363`, PENDING
  - Falsified if: this is non-zero while a defined opcode's slot holds anything but that opcode's own entry point, or an undefined byte's slot holds anything but the refusing one
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.JsBaselineHandlers()` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=Critical, Spec=none cited, `63B4DD`, PENDING
  - Falsified if: the published table maps a defined opcode byte to an entry point built for another opcode, or an undefined byte to anything but the refusing entry point
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Sound(nint[], nint)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=Critical, Spec=none cited, `131DD4`, PENDING
  - Falsified if: this answers true for a table in which two defined opcodes share an entry point, a defined opcode has the refusing one, or an undefined byte has any other
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Undefined(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `DFD818`, PENDING
  - Falsified if: this reads or writes any state, or answers anything but the defect status
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Nop(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `5A6B2F`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Nop at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadUndefined(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `A2C00C`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadUndefined at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadNull(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `8B9C06`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadNull at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadTrue(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `2F4548`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadTrue at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadFalse(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `2931C8`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadFalse at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadConstant(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `248FC9`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadConstant at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadThis(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `449067`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadThis at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.NewArguments(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `C8A81C`, PENDING
  - Falsified if: this runs anything other than a block step starting at one NewArguments at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadNewTarget(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `BBD78E`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadNewTarget at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadArgument(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `6F7F6A`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadArgument at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.RestArguments(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `428012`, PENDING
  - Falsified if: this runs anything other than a block step starting at one RestArguments at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadScoped(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `1615E7`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadScoped at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StoreScoped(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `FDC027`, PENDING
  - Falsified if: this runs anything other than a block step starting at one StoreScoped at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.InitialiseScoped(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `4B8647`, PENDING
  - Falsified if: this runs anything other than a block step starting at one InitialiseScoped at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadGlobal(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `C3B020`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadGlobal at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StoreGlobal(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `B243F9`, PENDING
  - Falsified if: this runs anything other than a block step starting at one StoreGlobal at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadGlobalOrUndefined(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `D6C0AD`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadGlobalOrUndefined at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.PushScope(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `8F8213`, PENDING
  - Falsified if: this runs anything other than a block step starting at one PushScope at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.PopScope(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `00838C`, PENDING
  - Falsified if: this runs anything other than a block step starting at one PopScope at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.CopyScope(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `CDAF46`, PENDING
  - Falsified if: this runs anything other than a block step starting at one CopyScope at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DeclareGlobal(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `07D66A`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DeclareGlobal at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.PushObjectScope(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `08ABD6`, PENDING
  - Falsified if: this runs anything other than a block step starting at one PushObjectScope at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ResolveName(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `07F9F9`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ResolveName at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.NewObject(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `DBC04D`, PENDING
  - Falsified if: this runs anything other than a block step starting at one NewObject at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.NewArray(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `040661`, PENDING
  - Falsified if: this runs anything other than a block step starting at one NewArray at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.GetProperty(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `272DF4`, PENDING
  - Falsified if: this runs anything other than a block step starting at one GetProperty at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.SetProperty(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `963AD0`, PENDING
  - Falsified if: this runs anything other than a block step starting at one SetProperty at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.GetIndex(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `696E20`, PENDING
  - Falsified if: this runs anything other than a block step starting at one GetIndex at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.SetIndex(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `97167E`, PENDING
  - Falsified if: this runs anything other than a block step starting at one SetIndex at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DefineField(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `45AEC3`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DefineField at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DefineIndexed(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `D354DA`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DefineIndexed at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DeleteProperty(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `9D3D19`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DeleteProperty at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DeleteIndex(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `9536FC`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DeleteIndex at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DefineGetter(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `A4B9D5`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DefineGetter at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DefineSetter(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `AD625A`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DefineSetter at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DefineMethod(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `31CC88`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DefineMethod at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadSuperProperty(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `E6F89C`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadSuperProperty at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StoreSuperProperty(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `6F2DF2`, PENDING
  - Falsified if: this runs anything other than a block step starting at one StoreSuperProperty at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ArrayAppend(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `ACA39C`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ArrayAppend at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Closure(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `F5AF61`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Closure at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Call(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `8F4FE3`, PENDING
  - Falsified if: this runs any instruction other than one Call at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Construct(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `8CCB64`, PENDING
  - Falsified if: this runs any instruction other than one Construct at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Return(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `EB1C70`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Return at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ReturnUndefined(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `D3C995`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ReturnUndefined at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.CallEval(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `98714F`, PENDING
  - Falsified if: this runs any instruction other than one CallEval at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.SuperCall(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `B5B6F0`, PENDING
  - Falsified if: this runs any instruction other than one SuperCall at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.SuperCallForwarded(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `766C71`, PENDING
  - Falsified if: this runs any instruction other than one SuperCallForwarded at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.NewClass(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `E18E52`, PENDING
  - Falsified if: this runs anything other than a block step starting at one NewClass at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ArrayHoles(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `9F10E0`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ArrayHoles at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.SpreadArray(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `1A3C48`, PENDING
  - Falsified if: this runs any instruction other than one SpreadArray at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.SpreadObject(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `D1B99A`, PENDING
  - Falsified if: this runs anything other than a block step starting at one SpreadObject at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.CallSpread(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `4C237E`, PENDING
  - Falsified if: this runs any instruction other than one CallSpread at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ConstructSpread(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `A121AF`, PENDING
  - Falsified if: this runs any instruction other than one ConstructSpread at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.SuperCallSpread(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `574BF0`, PENDING
  - Falsified if: this runs any instruction other than one SuperCallSpread at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.SetPrototypeLiteral(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `AFD8FD`, PENDING
  - Falsified if: this runs anything other than a block step starting at one SetPrototypeLiteral at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Add(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `A8D14B`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Add at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Subtract(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `BE0031`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Subtract at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Multiply(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `E0CA13`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Multiply at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Divide(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `FA6A92`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Divide at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Remainder(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `4B3DED`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Remainder at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Exponent(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `AE7094`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Exponent at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Negate(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `EAEF1C`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Negate at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ToNumber(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `F97078`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ToNumber at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Not(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `4F9718`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Not at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.BitwiseNot(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `71BEDB`, PENDING
  - Falsified if: this runs anything other than a block step starting at one BitwiseNot at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LessThan(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `A7AB60`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LessThan at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LessThanOrEqual(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `04D328`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LessThanOrEqual at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.GreaterThan(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `08A3C4`, PENDING
  - Falsified if: this runs anything other than a block step starting at one GreaterThan at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.GreaterThanOrEqual(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `1AE0FB`, PENDING
  - Falsified if: this runs anything other than a block step starting at one GreaterThanOrEqual at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StrictEquals(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `6B05E8`, PENDING
  - Falsified if: this runs anything other than a block step starting at one StrictEquals at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StrictNotEquals(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `AB3870`, PENDING
  - Falsified if: this runs anything other than a block step starting at one StrictNotEquals at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LooseEquals(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `5B4EA1`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LooseEquals at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LooseNotEquals(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `F7564B`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LooseNotEquals at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.BitwiseOr(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `7D4EC8`, PENDING
  - Falsified if: this runs anything other than a block step starting at one BitwiseOr at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.BitwiseAnd(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `0CD898`, PENDING
  - Falsified if: this runs anything other than a block step starting at one BitwiseAnd at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.BitwiseXor(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `E196D4`, PENDING
  - Falsified if: this runs anything other than a block step starting at one BitwiseXor at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ShiftLeft(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `F8A8FE`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ShiftLeft at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ShiftRight(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `5BAD97`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ShiftRight at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ShiftRightUnsigned(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `F3C276`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ShiftRightUnsigned at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.TypeOf(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `E772FA`, PENDING
  - Falsified if: this runs anything other than a block step starting at one TypeOf at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.InstanceOf(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `0F49FC`, PENDING
  - Falsified if: this runs anything other than a block step starting at one InstanceOf at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.In(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `4065E6`, PENDING
  - Falsified if: this runs anything other than a block step starting at one In at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Void(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `C6A2E3`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Void at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.RequireCoercible(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `D09ED2`, PENDING
  - Falsified if: this runs anything other than a block step starting at one RequireCoercible at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ToPropertyKey(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `64B161`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ToPropertyKey at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.GetTemplateObject(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `680DFE`, PENDING
  - Falsified if: this runs anything other than a block step starting at one GetTemplateObject at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Jump(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `E36AB7`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Jump at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.JumpIfFalse(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `72F3C2`, PENDING
  - Falsified if: this runs anything other than a block step starting at one JumpIfFalse at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.JumpIfTrue(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `0EEE7F`, PENDING
  - Falsified if: this runs anything other than a block step starting at one JumpIfTrue at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Throw(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `7BE2F7`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Throw at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ForInStart(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `BDDE7C`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ForInStart at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ForInNext(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `D6C4A4`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ForInNext at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.IterateStart(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `A585E6`, PENDING
  - Falsified if: this runs any instruction other than one IterateStart at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.IterateNext(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `EECFBB`, PENDING
  - Falsified if: this runs any instruction other than one IterateNext at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.IterateRest(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `3CA5FC`, PENDING
  - Falsified if: this runs any instruction other than one IterateRest at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.IterateClose(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `9668EC`, PENDING
  - Falsified if: this runs any instruction other than one IterateClose at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Yield(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `F4A9AF`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Yield at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.YieldDelegate(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `526D32`, PENDING
  - Falsified if: this runs any instruction other than one YieldDelegate at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Await(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `994FB9`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Await at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadImport(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `9A9071`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadImport at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ThrowImmutable(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `2CCCD5`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ThrowImmutable at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DefineClassElement(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `94C9EB`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DefineClassElement at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Pop(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `286CE5`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Pop at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Duplicate(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `0BE055`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Duplicate at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DuplicateTwo(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `305D59`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DuplicateTwo at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Swap(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `CFE42C`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Swap at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Pick(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `9C2483`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Pick at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.NewPrivateName(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `A004F8`, PENDING
  - Falsified if: this runs anything other than a block step starting at one NewPrivateName at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadPrivate(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `7F2E6A`, PENDING
  - Falsified if: this runs anything other than a block step starting at one LoadPrivate at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StorePrivate(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `6D6450`, PENDING
  - Falsified if: this runs anything other than a block step starting at one StorePrivate at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.HasPrivate(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `12FE21`, PENDING
  - Falsified if: this runs anything other than a block step starting at one HasPrivate at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.RunStaticElements(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `E3D87E`, PENDING
  - Falsified if: this runs any instruction other than one RunStaticElements at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.IterateStartAsync(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `4FC94A`, PENDING
  - Falsified if: this runs any instruction other than one IterateStartAsync at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.IterateNextAsync(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `AE57F8`, PENDING
  - Falsified if: this runs any instruction other than one IterateNextAsync at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.IterateAwaitStep(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `D00330`, PENDING
  - Falsified if: this runs anything other than a block step starting at one IterateAwaitStep at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.IterateCloseAsync(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `048BF9`, PENDING
  - Falsified if: this runs any instruction other than one IterateCloseAsync at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.IterateCloseCheck(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `AD4CEC`, PENDING
  - Falsified if: this runs anything other than a block step starting at one IterateCloseCheck at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DeclareGlobalLet(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `3E7F80`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DeclareGlobalLet at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DeclareGlobalConst(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `505007`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DeclareGlobalConst at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.InitialiseGlobalLexical(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `654457`, PENDING
  - Falsified if: this runs anything other than a block step starting at one InitialiseGlobalLexical at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DeleteGlobalBinding(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `713F2C`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DeleteGlobalBinding at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.EnterBody(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `D249CF`, PENDING
  - Falsified if: this runs anything other than a block step starting at one EnterBody at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ImportCall(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `8B3573`, PENDING
  - Falsified if: this runs any instruction other than one ImportCall at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ImportMeta(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `BC6690`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ImportMeta at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.CallEvalSpread(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `949682`, PENDING
  - Falsified if: this runs any instruction other than one CallEvalSpread at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadEvalName(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `1D4439`, PENDING
  - Falsified if: this runs any instruction other than one LoadEvalName at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadEvalNameOrUndefined(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `A0FC49`, PENDING
  - Falsified if: this runs any instruction other than one LoadEvalNameOrUndefined at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StoreEvalName(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `856FEB`, PENDING
  - Falsified if: this runs any instruction other than one StoreEvalName at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.LoadEvalNameWithBase(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `64C206`, PENDING
  - Falsified if: this runs any instruction other than one LoadEvalNameWithBase at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DeleteEvalName(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `3CA5A8`, PENDING
  - Falsified if: this runs any instruction other than one DeleteEvalName at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.WithBaseObject(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `C6F53B`, PENDING
  - Falsified if: this runs anything other than a block step starting at one WithBaseObject at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StoreEvalVariable(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `247561`, PENDING
  - Falsified if: this runs any instruction other than one StoreEvalVariable at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DisposeScope(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `A60F4D`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DisposeScope at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DisposeAdd(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `3411DD`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DisposeAdd at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DisposeFold(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `F89672`, PENDING
  - Falsified if: this runs anything other than a block step starting at one DisposeFold at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.ToNumeric(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `EC258B`, PENDING
  - Falsified if: this runs anything other than a block step starting at one ToNumeric at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Increment(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `12C42C`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Increment at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.Decrement(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `57F69F`, PENDING
  - Falsified if: this runs anything other than a block step starting at one Decrement at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DisposeStep(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `098985`, PENDING
  - Falsified if: this runs any instruction other than one DisposeStep at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.DisposeEnd(JsBaselineFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `2C6414`, PENDING
  - Falsified if: this runs any instruction other than one DisposeEnd at the offset the managed side expects
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepCall` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `821976`, PENDING
  - Falsified if: this step names an opcode other than Call
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepCall.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `AEA809`, PENDING
  - Falsified if: this answers any opcode other than Call
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepConstruct` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `D2D494`, PENDING
  - Falsified if: this step names an opcode other than Construct
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepConstruct.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `AB9410`, PENDING
  - Falsified if: this answers any opcode other than Construct
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepCallEval` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `5E042F`, PENDING
  - Falsified if: this step names an opcode other than CallEval
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepCallEval.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `D5D4C4`, PENDING
  - Falsified if: this answers any opcode other than CallEval
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepSuperCall` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `D2FC1C`, PENDING
  - Falsified if: this step names an opcode other than SuperCall
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepSuperCall.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `716B4E`, PENDING
  - Falsified if: this answers any opcode other than SuperCall
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepSuperCallForwarded` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `82DB6C`, PENDING
  - Falsified if: this step names an opcode other than SuperCallForwarded
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepSuperCallForwarded.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `6D1190`, PENDING
  - Falsified if: this answers any opcode other than SuperCallForwarded
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepSpreadArray` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `0307E1`, PENDING
  - Falsified if: this step names an opcode other than SpreadArray
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepSpreadArray.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `10C694`, PENDING
  - Falsified if: this answers any opcode other than SpreadArray
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepCallSpread` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `A5D0B7`, PENDING
  - Falsified if: this step names an opcode other than CallSpread
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepCallSpread.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `B10BF3`, PENDING
  - Falsified if: this answers any opcode other than CallSpread
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepConstructSpread` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `41B0BC`, PENDING
  - Falsified if: this step names an opcode other than ConstructSpread
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepConstructSpread.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `A344AA`, PENDING
  - Falsified if: this answers any opcode other than ConstructSpread
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepSuperCallSpread` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `CBE67D`, PENDING
  - Falsified if: this step names an opcode other than SuperCallSpread
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepSuperCallSpread.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `935410`, PENDING
  - Falsified if: this answers any opcode other than SuperCallSpread
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateStart` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `74E822`, PENDING
  - Falsified if: this step names an opcode other than IterateStart
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateStart.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `0C1F4F`, PENDING
  - Falsified if: this answers any opcode other than IterateStart
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateNext` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `02BE85`, PENDING
  - Falsified if: this step names an opcode other than IterateNext
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateNext.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `E8268A`, PENDING
  - Falsified if: this answers any opcode other than IterateNext
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateRest` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `28105F`, PENDING
  - Falsified if: this step names an opcode other than IterateRest
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateRest.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `D63D37`, PENDING
  - Falsified if: this answers any opcode other than IterateRest
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateClose` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `167664`, PENDING
  - Falsified if: this step names an opcode other than IterateClose
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateClose.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `BF8374`, PENDING
  - Falsified if: this answers any opcode other than IterateClose
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepYieldDelegate` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `96D383`, PENDING
  - Falsified if: this step names an opcode other than YieldDelegate
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepYieldDelegate.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `BB6777`, PENDING
  - Falsified if: this answers any opcode other than YieldDelegate
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepRunStaticElements` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `4AD870`, PENDING
  - Falsified if: this step names an opcode other than RunStaticElements
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepRunStaticElements.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `276AB0`, PENDING
  - Falsified if: this answers any opcode other than RunStaticElements
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateStartAsync` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `B0B5E3`, PENDING
  - Falsified if: this step names an opcode other than IterateStartAsync
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateStartAsync.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `4D20AC`, PENDING
  - Falsified if: this answers any opcode other than IterateStartAsync
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateNextAsync` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `319122`, PENDING
  - Falsified if: this step names an opcode other than IterateNextAsync
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateNextAsync.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `021DE0`, PENDING
  - Falsified if: this answers any opcode other than IterateNextAsync
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateCloseAsync` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `664AB9`, PENDING
  - Falsified if: this step names an opcode other than IterateCloseAsync
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepIterateCloseAsync.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `3D3E70`, PENDING
  - Falsified if: this answers any opcode other than IterateCloseAsync
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepImportCall` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `ECF250`, PENDING
  - Falsified if: this step names an opcode other than ImportCall
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepImportCall.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `33B0A7`, PENDING
  - Falsified if: this answers any opcode other than ImportCall
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepCallEvalSpread` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `DAB9CA`, PENDING
  - Falsified if: this step names an opcode other than CallEvalSpread
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepCallEvalSpread.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `1FA07A`, PENDING
  - Falsified if: this answers any opcode other than CallEvalSpread
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepLoadEvalName` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `556454`, PENDING
  - Falsified if: this step names an opcode other than LoadEvalName
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepLoadEvalName.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `98D74C`, PENDING
  - Falsified if: this answers any opcode other than LoadEvalName
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepLoadEvalNameOrUndefined` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `E42664`, PENDING
  - Falsified if: this step names an opcode other than LoadEvalNameOrUndefined
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepLoadEvalNameOrUndefined.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `407A2F`, PENDING
  - Falsified if: this answers any opcode other than LoadEvalNameOrUndefined
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepStoreEvalName` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `E03E43`, PENDING
  - Falsified if: this step names an opcode other than StoreEvalName
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepStoreEvalName.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `8F0D6F`, PENDING
  - Falsified if: this answers any opcode other than StoreEvalName
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepLoadEvalNameWithBase` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `7DD129`, PENDING
  - Falsified if: this step names an opcode other than LoadEvalNameWithBase
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepLoadEvalNameWithBase.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `697B36`, PENDING
  - Falsified if: this answers any opcode other than LoadEvalNameWithBase
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepDeleteEvalName` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `21A508`, PENDING
  - Falsified if: this step names an opcode other than DeleteEvalName
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepDeleteEvalName.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `1EF902`, PENDING
  - Falsified if: this answers any opcode other than DeleteEvalName
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepStoreEvalVariable` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `ADB718`, PENDING
  - Falsified if: this step names an opcode other than StoreEvalVariable
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepStoreEvalVariable.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `CF3F82`, PENDING
  - Falsified if: this answers any opcode other than StoreEvalVariable
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepDisposeStep` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `CB0B02`, PENDING
  - Falsified if: this step names an opcode other than DisposeStep
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepDisposeStep.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `2ACA17`, PENDING
  - Falsified if: this answers any opcode other than DisposeStep
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepDisposeEnd` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `FAC970`, PENDING
  - Falsified if: this step names an opcode other than DisposeEnd
- `Broiler.VM.Profile.JavaScript.JsBaselineHandlers.StepDisposeEnd.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs` - Security=High, Spec=none cited, `3C4E84`, PENDING
  - Falsified if: this answers any opcode other than DisposeEnd
- `Broiler.VM.Profile.JavaScript.JsBigInt.JsBigInt(System.Numerics.BigInteger)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `4BD330`, PENDING
  - Falsified if: an instance holding a magnitude wider than MaximumBits is constructed
- `Broiler.VM.Profile.JavaScript.JsBigInt.FromConstant(bool, System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `2C9E91`, PENDING
  - Falsified if: a payload decodes to an integer other than the one its bytes spell
- `Broiler.VM.Profile.JavaScript.JsBigInt.ToDecimalString(System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `4CE1C6`, PENDING
  - Falsified if: a wider BigInt is converted to text for the same charge as a narrower one, or the text differs from the integer
- `Broiler.VM.Profile.JavaScript.JsBigInt.AppendDecimal(System.Text.StringBuilder, System.Numerics.BigInteger, int, int, System.Collections.Generic.List<System.Numerics.BigInteger>, System.Collections.Generic.List<int>, System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `8795FC`, PENDING
  - Falsified if: a digit is dropped, duplicated or unpadded where a split leaves a remainder with leading zeros
- `Broiler.VM.Profile.JavaScript.JsBigInt.LinearCost(int)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `EA891E`, PENDING
  - Falsified if: an operation linear in its operands is charged less for wider operands
- `Broiler.VM.Profile.JavaScript.JsBigInt.ProductCost(int, int)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `B7A09B`, PENDING
  - Falsified if: a multiplication or division of wider operands is charged less than one of narrower operands, or less than the product of their sizes over eight
- `Broiler.VM.Profile.JavaScript.JsBigInt.LeafCost(int)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `38E03F`, PENDING
  - Falsified if: a wider part is converted for a smaller charge
- `Broiler.VM.Profile.JavaScript.JsBigInt.Bounded(System.Numerics.BigInteger)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `81A77E`, PENDING
  - Falsified if: a value wider than MaximumBits is wrapped, or the constructor throws for a computed result
- `Broiler.VM.Profile.JavaScript.JsBigInt.Multiply(JsBigInt, JsBigInt, System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `531F83`, PENDING
  - Falsified if: a product whose operands already prove it wider than MaximumBits is computed, or a product is charged less than ProductCost of its operands
- `Broiler.VM.Profile.JavaScript.JsBigInt.Divide(JsBigInt, JsBigInt, System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `32A18C`, PENDING
  - Falsified if: a quotient is rounded other than toward zero, or a division is charged less than ProductCost of its operands
- `Broiler.VM.Profile.JavaScript.JsBigInt.Remainder(JsBigInt, JsBigInt, System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `EED233`, PENDING
  - Falsified if: a remainder takes the sign of the divisor, or is charged less than ProductCost of its operands
- `Broiler.VM.Profile.JavaScript.JsBigInt.Power(JsBigInt, JsBigInt, System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `A8C216`, PENDING
  - Falsified if: a power is computed without charging each multiplication, or a result past MaximumBits is answered, or a huge exponent of a trivial base is refused
- `Broiler.VM.Profile.JavaScript.JsBigInt.Shift(JsBigInt, System.Numerics.BigInteger, System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `CC5259`, PENDING
  - Falsified if: a left shift allocates a result its widths already prove wider than MaximumBits, a right shift rounds toward zero, or an oversized count reaches the base class library
- `Broiler.VM.Profile.JavaScript.JsBigInt.AsIntN(ulong, JsBigInt, System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `09DD62`, PENDING
  - Falsified if: a value that fits the signed width is changed, or a wrapped value takes the wrong sign
- `Broiler.VM.Profile.JavaScript.JsBigInt.AsUintN(ulong, JsBigInt, System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `443166`, PENDING
  - Falsified if: a non-negative value that fits the width is changed, a negative value is answered negative, or a modulus past MaximumBits is allocated
- `Broiler.VM.Profile.JavaScript.JsBigInt.FromIntegralNumber(double)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `861F87`, PENDING
  - Falsified if: an integral Number converts to an integer other than its exact value
- `Broiler.VM.Profile.JavaScript.JsBigInt.ToNumber(System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `A48858`, PENDING
  - Falsified if: a BigInt converts to a Number other than the nearest one with ties to even, or a value at or past 2 ** 1024 - 2 ** 970 converts to a finite Number
- `Broiler.VM.Profile.JavaScript.JsBigInt.CompareToNumber(double, System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `616D0D`, PENDING
  - Falsified if: a BigInt and a Number compare other than their mathematical values do, or either is rounded to compare
- `Broiler.VM.Profile.JavaScript.JsBigInt.TryParseStringInteger(string, System.Action<ulong>, out JsBigInt?, out int)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `7E3B41`, PENDING
  - Falsified if: a text outside StringIntegerLiteral answers a value, a text inside it answers an integer other than its mathematical value, or a value past MaximumBits is constructed
- `Broiler.VM.Profile.JavaScript.JsBigInt.TryParseRadix(System.ReadOnlySpan<char>, int, System.Action<ulong>, out JsBigInt?, out int)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `6AC22E`, PENDING
  - Falsified if: a digit outside the radix is admitted, or the packed bits spell an integer other than the digits do
- `Broiler.VM.Profile.JavaScript.JsBigInt.ParseDecimal(System.ReadOnlySpan<char>, System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `254B9F`, PENDING
  - Falsified if: a digit is dropped or misplaced in a split, or a join is not charged before it runs
- `Broiler.VM.Profile.JavaScript.JsBigInt.ParseDecimalPart(System.ReadOnlySpan<char>, int, System.Collections.Generic.List<System.Numerics.BigInteger>, System.Collections.Generic.List<int>, System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `C444EA`, PENDING
  - Falsified if: the high part is scaled by a power other than ten to the low part's length
- `Broiler.VM.Profile.JavaScript.JsBigInt.ToRadixString(int, System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `DEB859`, PENDING
  - Falsified if: a wider BigInt is converted for the same charge as a narrower one, or the digits differ from the integer in that radix
- `Broiler.VM.Profile.JavaScript.JsBigInt.AppendRadix(System.Text.StringBuilder, System.Numerics.BigInteger, int, int, int, ulong, int, System.Collections.Generic.List<System.Numerics.BigInteger>, System.Collections.Generic.List<int>, System.Action<ulong>)` in `src/Broiler.VM.Profile.JavaScript/JsBigInt.cs` - Security=High, Spec=none cited, `75C725`, PENDING
  - Falsified if: a digit is dropped, duplicated or unpadded where a split leaves a remainder with leading zeros
- `Broiler.VM.Profile.JavaScript.JsElements.ReadValue(byte[], int, JsElementKind, bool)` in `src/Broiler.VM.Profile.JavaScript/JsBinary.cs` - Security=High, Spec=none cited, `BAB170`, PENDING
  - Falsified if: a BigInt64 element reads as anything but the two's-complement integer of its eight bytes, a BigUint64 element as anything but their unsigned integer, or either as a Number
- `Broiler.VM.Profile.JavaScript.JsElements.WriteValue(byte[], int, JsElementKind, JsValue, bool)` in `src/Broiler.VM.Profile.JavaScript/JsBinary.cs` - Security=High, Spec=none cited, `D893D2`, PENDING
  - Falsified if: a BigInt element stores anything but its value modulo 2**64, or a value of the other content type is stored rather than refused
- `Broiler.VM.Profile.JavaScript.JsElements.Low64(System.Numerics.BigInteger)` in `src/Broiler.VM.Profile.JavaScript/JsBinary.cs` - Security=High, Spec=none cited, `33AFD0`, PENDING
  - Falsified if: the answer differs from the value modulo 2**64 for any integer, negative ones included
- `Broiler.VM.Profile.JavaScript.JsElements.ElementOf(JsElementKind, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsBinary.cs` - Security=High, Spec=none cited, `50F640`, PENDING
  - Falsified if: a BigInt kind stores a value converted from a Number, a String or an object on this engine-less path
- `Broiler.VM.Profile.JavaScript.JsArrayBuffer.TryReplaceStorage(byte[])` in `src/Broiler.VM.Profile.JavaScript/JsBinary.cs` - Security=High, Spec=none cited, `B340C3`, PENDING
  - Falsified if: a resize is committed on a fixed-length or detached buffer, past the maximum, or loses a byte of the common prefix or exposes a stale byte past it
- `Broiler.VM.Profile.JavaScript.JsCloneCarrier` in `src/Broiler.VM.Profile.JavaScript/JsClone.cs` - Security=High, Spec=none cited, `D07D0E`, PENDING
  - Falsified if: a carrier holds a JsObject, a JsValue or any other reference into the realm that produced it
- `Broiler.VM.Profile.JavaScript.JsCloneCarrier.MaxEntries` in `src/Broiler.VM.Profile.JavaScript/JsClone.cs` - Security=High, Spec=none cited, `8888D4`, PENDING
  - Falsified if: a serialization completes with more entries than this
- `Broiler.VM.Profile.JavaScript.JsCloneCarrier.MaxBytes` in `src/Broiler.VM.Profile.JavaScript/JsClone.cs` - Security=High, Spec=none cited, `7362FC`, PENDING
  - Falsified if: a serialization completes holding more bytes than this
- `Broiler.VM.Profile.JavaScript.JsCloneCarrier.ChargeFor(long, long)` in `src/Broiler.VM.Profile.JavaScript/JsClone.cs` - Security=High, Spec=none cited, `ECD920`, PENDING
  - Falsified if: the charge does not grow with the carrier's entries and copied bytes
- `Broiler.VM.Profile.JavaScript.JsCloneCarrier.TryClaimMoved()` in `src/Broiler.VM.Profile.JavaScript/JsClone.cs` - Security=High, Spec=none cited, `D7D2C4`, PENDING
  - Falsified if: two adoptions of one carrier holding moved bytes both succeed
- `Broiler.VM.Profile.JavaScript.JsHostCloneCarrier` in `src/Broiler.VM.Profile.JavaScript/JsClone.cs` - Security=High, Spec=none cited, `B2EC01`, PENDING
  - Falsified if: a carrier exposes its graph, holds a reference into a realm, or is adopted by a realm of another profile build
- `Broiler.VM.Profile.JavaScript.JsFinalizationRegistryObject` in `src/Broiler.VM.Profile.JavaScript/JsCollections.cs` - Security=High, Spec=none cited, `66E399`, PENDING
  - Falsified if: a cleanup callback registered here is ever invoked, or any guest code runs from a CLR finalizer
- `Broiler.VM.Profile.JavaScript.JsEngine` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `7BBE7E`, PENDING
  - Falsified if: emitted code is entered from anywhere but this file
- `Broiler.VM.Profile.JavaScript.JsEngine.RunNative(JsProgram, int, JsEnvironment?, JsValue, JsValue[], JsScriptFunction?, JsValue, JsCell?, JsFrame?)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `8C7329`, PENDING
  - Falsified if: emitted code runs while its activation or its page is unreachable from a managed root, or this answers a value for a status other than exit
- `Broiler.VM.Profile.JavaScript.JsEngine.ValueStack` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `431645`, PENDING
  - Falsified if: a value-form engine has none, or two engines share one
- `Broiler.VM.Profile.JavaScript.JsEngine.ValueHandles` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `582BBE`, PENDING
  - Falsified if: a value-form engine has none, two engines share one, or its compactions scan any words but this engine's value stack
- `Broiler.VM.Profile.JavaScript.JsEngine.RunValue(JsProgram, int, JsEnvironment?, JsValue, JsValue[], JsScriptFunction?, JsValue, JsCell?, JsFrame?)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `22D6B8`, PENDING
  - Falsified if: emitted code runs while its activation or its page is unreachable from a managed root, runs with no region or with a region some other activation holds, a region outlives the call that opened it, or this answers a value for a status other than exit
- `Broiler.VM.Profile.JavaScript.JsEngine.Returned(JsNativeActivation, long)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `696FCF`, PENDING
  - Falsified if: it answers any value but the decoding of the region's first word, decodes after a safepoint, or answers before the debt is charged
- `Broiler.VM.Profile.JavaScript.JsEngine.OpenValue(JsProgram, int, JsEnvironment?, JsValue, JsValue[], JsScriptFunction?, JsValue, JsCell?, JsFrame?)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `FB7536`, PENDING
  - Falsified if: it answers an activation whose region is not the top of the value stack, is not encoded and published at the activation's height, or whose plan is not its unit's; or a failure leaves a region pushed
- `Broiler.VM.Profile.JavaScript.JsEngine.CloseValue(JsNativeActivation)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `C5E740`, PENDING
  - Falsified if: a region is popped other than in the reverse order regions were pushed, or an activation keeps a segment or a plan after its region is closed
- `Broiler.VM.Profile.JavaScript.JsEngine.Frame(JsNativeActivation, JsValueFrame*)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `22D8FA`, PENDING
  - Falsified if: the context names a cookie other than the activation's, a region other than its own first word, a debt other than zero, or an entry
- `Broiler.VM.Profile.JavaScript.JsEngine.TryDirectCallee(JsNativeActivation, int, out JsScriptFunction)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `35645C`, PENDING
  - Falsified if: it answers true for a callee that is not a plain script function of the caller's program, or changes a word, a handle or the meter
- `Broiler.VM.Profile.JavaScript.JsEngine.BeginDirectCall(JsNativeActivation, int, int, JsScriptFunction, JsValueFrame*)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `0002A2`, PENDING
  - Falsified if: a direct call charges, checks, binds or opens other than the interpreter's call of the same function would, fills an entry other than the callee unit's in the caller's own payload, or a failure leaves a depth, a referrer or a region taken
- `Broiler.VM.Profile.JavaScript.JsEngine.EndDirectCall(JsNativeActivation)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `7C9518`, PENDING
  - Falsified if: a direct callee's region, referrer or depth outlives its return, or is given back twice
- `Broiler.VM.Profile.JavaScript.JsEngine.NativePageOf(JsProgram)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `D236B5`, PENDING
  - Falsified if: a page this returns is not armed, or a program's published page is replaced or released while the program is reachable
- `Broiler.VM.Profile.JavaScript.JsEngine.RequireInstanceForm(JsProgram)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.Baseline.cs` - Security=Critical, Spec=none cited, `D4C7D8`, PENDING
  - Falsified if: a guest-loaded program of the other form, of another architecture, or of the numeric manifest runs in a baseline instance
- `Broiler.VM.Profile.JavaScript.JsEngine.nativeForm` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `05B362`, PENDING
  - Falsified if: an engine built for one form runs a program of the other form
- `Broiler.VM.Profile.JavaScript.JsEngine.valueForm` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `921708`, PENDING
  - Falsified if: an engine built for the value form runs a baseline program's emitted code, or the reverse
- `Broiler.VM.Profile.JavaScript.JsEngine.DrainJobs()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `4B5EB1`, PENDING
  - Falsified if: a job runs at a point the host did not ask for, or an endless queue is a hang rather than an exhaustion
- `Broiler.VM.Profile.JavaScript.JsEngine.StepOneJob(out JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `FB280D`, PENDING
  - Falsified if: more than one job runs in a step, or a step reports a queue state the queue does not have
- `Broiler.VM.Profile.JavaScript.JsEngine.DropPendingJobs()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `459593`, PENDING
  - Falsified if: a queued job runs during an unwind
- `Broiler.VM.Profile.JavaScript.JsEngine.Loader` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `67C7AE`, PENDING
  - Falsified if: a mediator is used outside the invocation that supplied it
- `Broiler.VM.Profile.JavaScript.JsEngine.Evaluate(JsValue[], bool, Format.JsFormat.FunctionFlags)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `F0977B`, PENDING
  - Falsified if: guest source becomes executable bytes without passing through the mediator
- `Broiler.VM.Profile.JavaScript.JsEngine.EvaluateDirect(JsProgram, int, int, System.Collections.Generic.List<JsEnvironment>, JsValue[], JsValue, JsCell?, JsValue, JsScriptFunction?)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `C613DA`, PENDING
  - Falsified if: a direct eval with no site row at a module's top level, or in a script body whose current record is not its entry record, evaluates the source instead of throwing an EvalError
- `Broiler.VM.Profile.JavaScript.JsEngine.EvaluateGlobal(string, Format.JsFormat.EvalRequestFlags, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `C997CA`, PENDING
  - Falsified if: guest source becomes executable bytes without passing through the mediator, or a global evaluation's lexical declaration becomes a binding of the realm
- `Broiler.VM.Profile.JavaScript.JsEngine.EvaluateScript(string, string, bool)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `1248A2`, PENDING
  - Falsified if: a host script's source becomes executable bytes without passing through the mediator, or an answer that is not a script compiled under the requested strictness runs
- `Broiler.VM.Profile.JavaScript.JsEngine.LoadEvalCode(Format.JsFormat.EvalRequestFlags, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `541E1E`, PENDING
  - Falsified if: guest source becomes executable bytes without passing through the mediator, or an answer that is not eval code compiled under the requested flags is returned
- `Broiler.VM.Profile.JavaScript.JsEngine.InstantiateEvalDeclarations(JsEvalView, JsEnvironment?, JsEvalDeclaration)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `053159`, PENDING
  - Falsified if: a binding is created in the caller's scope or on the global object before a later check of the same evaluation throws, or a declared name is made a binding past a lexical binding of the same name that is not a catch parameter
- `Broiler.VM.Profile.JavaScript.JsEngine.InstantiateGlobalDeclarations(JsScriptDeclaration)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `C0B3E2`, PENDING
  - Falsified if: a binding is created on the global object or in the global lexical environment before a later check of the same script throws, or a lexical declaration is admitted over an existing global lexical declaration or a non-configurable global property
- `Broiler.VM.Profile.JavaScript.JsEngine.WriteEvalVariable(JsEnvironment, string, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `19B50E`, PENDING
  - Falsified if: the write reaches a record other than the evaluation's variable environment, or writes a name its instantiation did not declare
- `Broiler.VM.Profile.JavaScript.JsEngine.EvaluateAtSite(JsProgram, JsEvalSite, JsEnvironment, string, JsValue, JsCell?, JsValue, JsScriptFunction?)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `9109EA`, PENDING
  - Falsified if: guest source becomes executable bytes without passing through the mediator, or an answer that is not eval code compiled under the site's flags runs
- `Broiler.VM.Profile.JavaScript.JsEngine.EvalBoundary(System.Collections.Generic.List<JsEnvironment>, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `BFC809`, PENDING
  - Falsified if: it answers a record that was not entered as an eval boundary
- `Broiler.VM.Profile.JavaScript.JsEngine.ResolveEvalName(JsEnvironment, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `7216D6`, PENDING
  - Falsified if: a name is answered from a slot the view's site row chain does not name, or from a record past a declarative binding of the same name
- `Broiler.VM.Profile.JavaScript.JsEngine.EvalPrivateNameDeclared(JsEnvironment, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `01D083`, PENDING
  - Falsified if: it asks a with object or runs guest code, or answers true for a name no declarative row on the site's row chain names
- `Broiler.VM.Profile.JavaScript.JsEngine.WriteEvalName(JsEnvironment, string, JsValue, bool)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `290144`, PENDING
  - Falsified if: a write through the map reaches a slot the map does not name, or succeeds on an immutable or uninitialised binding
- `Broiler.VM.Profile.JavaScript.JsEngine.EndHostStep()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `39C37F`, PENDING
  - Falsified if: an abort latched at the seam is dropped when the step ends
- `Broiler.VM.Profile.JavaScript.JsEngine.MaximumCallDepth` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `321EE5`, PENDING
  - Falsified if: a program recursing past this bound terminates the process rather than throwing a catchable RangeError
- `Broiler.VM.Profile.JavaScript.JsEngine.reportingDepth` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `DA5D1D`, PENDING
  - Falsified if: this stays set after the refusal has been thrown, so a later recursion is unbounded
- `Broiler.VM.Profile.JavaScript.JsEngine.StackBackstopReached()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `94E6AA`, PENDING
  - Falsified if: the stack backstop produces a result naming no dimension, or a refused charge commits anything
- `Broiler.VM.Profile.JavaScript.JsEngine.ChargeText(int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `626201`, PENDING
  - Falsified if: a text operation's charge does not grow with the input the guest controls
- `Broiler.VM.Profile.JavaScript.JsEngine.ChargeHostCrossing(ulong)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `62A22B`, PENDING
  - Falsified if: a crossing of the host surface completes without charging HostCalls
- `Broiler.VM.Profile.JavaScript.JsEngine.ChargeDebt(long)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `6EA938`, PENDING
  - Falsified if: a debt is charged other than through Charge, or a zero debt charges or polls anything
- `Broiler.VM.Profile.JavaScript.JsEngine.RetainOrAbort(ulong)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `13932D`, PENDING
  - Falsified if: a retention the LiveBytes ceiling refuses returns normally or leaves the allocation to happen
- `Broiler.VM.Profile.JavaScript.JsEngine.ToNumberFromText(string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `07B791`, PENDING
  - Falsified if: a longer numeric string is read for the same charge as a shorter one
- `Broiler.VM.Profile.JavaScript.JsEngine.ToNumeric(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `5C1B6B`, PENDING
  - Falsified if: an object operand is converted to a primitive twice, or a BigInt reaches ToNumber through this operation
- `Broiler.VM.Profile.JavaScript.JsEngine.BigIntText(JsBigInt)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `E0D910`, PENDING
  - Falsified if: a wider BigInt is converted to text for the same charge as a narrower one
- `Broiler.VM.Profile.JavaScript.JsEngine.ToBigInt(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `66E934`, PENDING
  - Falsified if: a Number, undefined, null or a Symbol converts to a BigInt, or a String outside StringIntegerLiteral answers anything but a SyntaxError
- `Broiler.VM.Profile.JavaScript.JsEngine.PrimitiveToBigInt(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `29E42B`, PENDING
  - Falsified if: a Number, undefined, null or a Symbol converts to a BigInt
- `Broiler.VM.Profile.JavaScript.JsEngine.StringToBigIntOrThrow(string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `60A446`, PENDING
  - Falsified if: a text outside StringIntegerLiteral answers a value, or a value past the ceiling is answered
- `Broiler.VM.Profile.JavaScript.JsEngine.NumberToBigInt(double)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `9A4F6D`, PENDING
  - Falsified if: a Number that is not an integer converts to a BigInt, or an integral one converts to any integer but its own
- `Broiler.VM.Profile.JavaScript.JsEngine.ToElementValue(JsElementKind, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `AB31B4`, PENDING
  - Falsified if: a BigInt kind accepts a Number, a Number kind accepts a BigInt, or either converts with the other content type's operation
- `Broiler.VM.Profile.JavaScript.JsEngine.BigIntToNumber(JsBigInt)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `1D0F36`, PENDING
  - Falsified if: a BigInt converts to a Number other than the nearest one with ties to even
- `Broiler.VM.Profile.JavaScript.JsEngine.EnterCall()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `6C4D20`, PENDING
  - Falsified if: a call that passes it has not been charged, has not been probed, is past the counted bound without a RangeError, or has not taken one depth that LeaveCall gives back
- `Broiler.VM.Profile.JavaScript.JsEngine.DirectCallCharge` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `5DBA5E`, PENDING
  - Falsified if: a direct call is charged other than the instruction's unit and the call's four over its debt
- `Broiler.VM.Profile.JavaScript.JsEngine.EnterDepth()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `19672B`, PENDING
  - Falsified if: a call that passes it has not been probed, is past the counted bound without a RangeError, or has not taken one depth that LeaveCall gives back
- `Broiler.VM.Profile.JavaScript.JsEngine.LeaveCall()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `957E56`, PENDING
  - Falsified if: it is run other than once for each EnterCall that returned
- `Broiler.VM.Profile.JavaScript.JsEngine.instanced` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `9CFED6`, PENDING
  - Falsified if: two instances of one module key exist in one realm
- `Broiler.VM.Profile.JavaScript.JsEngine.RunModuleGraph(JsProgram, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `6350BB`, PENDING
  - Falsified if: a module body runs twice in one realm, or a module body runs before every module's declarations are initialised
- `Broiler.VM.Profile.JavaScript.JsEngine.TemplateObject(JsProgram, int, JsValue[], int, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `CEE75C`, PENDING
  - Falsified if: two evaluations of one site answer different objects, two sites answer one object, or the answer is observably not frozen
- `Broiler.VM.Profile.JavaScript.JsEngine.Evaluated(JsProgram, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `CE0B69`, PENDING
  - Falsified if: a module body runs twice in one realm, or the entry graph's failure is reported nowhere
- `Broiler.VM.Profile.JavaScript.JsEngine.Instantiate(JsProgram)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `6D69E9`, PENDING
  - Falsified if: a module body runs before every module of its artifact has its declarations initialised
- `Broiler.VM.Profile.JavaScript.JsEngine.EvaluateModule(JsModuleInstance)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `BC6F73`, PENDING
  - Falsified if: a module body runs twice in one realm, runs before a module it requests has finished, or a module that does not depend on an awaiting module waits for it
- `Broiler.VM.Profile.JavaScript.JsEngine.InnerModuleEvaluation(JsModuleInstance, System.Collections.Generic.List<JsModuleInstance>, ref int, out JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `91CD77`, PENDING
  - Falsified if: a module body runs before every module it requests has run or started, or a module waiting on async dependencies runs before they finish
- `Broiler.VM.Profile.JavaScript.JsEngine.AsyncModuleExecutionFulfilled(JsModuleInstance)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `42DB0E`, PENDING
  - Falsified if: the modules one completion releases run in an order other than the one the walk reached them in, or a module runs while something it waits on has not finished
- `Broiler.VM.Profile.JavaScript.JsEngine.AsyncModuleExecutionRejected(JsModuleInstance, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `140497`, PENDING
  - Falsified if: a module that waits on one whose async evaluation threw later runs, or a module not waiting on it is failed
- `Broiler.VM.Profile.JavaScript.JsEngine.Confirm(JsProgram)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `4B22CD`, PENDING
  - Falsified if: a module request is honoured without the composition being asked, or a refusal is treated as an answer
- `Broiler.VM.Profile.JavaScript.JsEngine.DynamicImport(JsProgram, string, JsValue, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `880ABC`, PENDING
  - Falsified if: this throws instead of rejecting, or a specifier reaches bytes without passing through the mediator or the artifact's own records
- `Broiler.VM.Profile.JavaScript.JsEngine.EvaluateInto(JsProgram, int, JsPromiseObject, bool)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `50B9DF`, PENDING
  - Falsified if: the promise settles before every module of the graph has finished, or a failure escapes as a throw
- `Broiler.VM.Profile.JavaScript.JsEngine.LinkAndEvaluate(JsProgram, int, JsPromiseObject, bool)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `F6359B`, PENDING
  - Falsified if: the promise settles other than through the job queue, or a failure escapes as a throw
- `Broiler.VM.Profile.JavaScript.JsEngine.LinkHostModule(string, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `85EF5C`, PENDING
  - Falsified if: a module body runs before a host asks for an evaluation, or a module request is answered without the mediator
- `Broiler.VM.Profile.JavaScript.JsEngine.CompleteImport(string, string, JsPromiseObject)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `DD5620`, PENDING
  - Falsified if: a deferred import settles other than through the job queue, or a failure escapes as a throw
- `Broiler.VM.Profile.JavaScript.JsEngine.TryOwnRequest(JsProgram, string, string, out (JsProgram Program, int Index))` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `8970EB`, PENDING
  - Falsified if: one module key is evaluated twice in one realm
- `Broiler.VM.Profile.JavaScript.JsEngine.MediatedModule(string, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `006C6A`, PENDING
  - Falsified if: a module request is answered by an artifact whose root is not a module body
- `Broiler.VM.Profile.JavaScript.JsEngine.BindParameters(JsProgram, JsCodeUnit, JsScriptFunction, JsFrame, JsValue, JsValue[])` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `E1EDFC`, PENDING
  - Falsified if: a generator over a non-simple parameter list reports a binding failure at its first resumption rather than at its call
- `Broiler.VM.Profile.JavaScript.JsEngine.ResumeGenerator(JsValue, JsResumeMode, JsValue, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `984439`, PENDING
  - Falsified if: a generator resumed while its own body is running re-enters that body, or a completed generator runs any instruction
- `Broiler.VM.Profile.JavaScript.JsEngine.ResumeAsync(JsAsyncCall, JsResumeMode, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `04C185`, PENDING
  - Falsified if: an async call whose body is already on the interpreter's stack is resumed again, or a program that awaits without end is a hang rather than an exhaustion
- `Broiler.VM.Profile.JavaScript.JsEngine.EnqueueAsyncGenerator(JsValue, JsResumeMode, JsValue, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `CB6E30`, PENDING
  - Falsified if: a call of `next`, `return` or `throw` on an async generator answers anything but a promise, or two calls made before the first settles are answered out of order
- `Broiler.VM.Profile.JavaScript.JsEngine.ResumeAsyncGenerator(JsAsyncGenerator, JsResumeMode, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `30F939`, PENDING
  - Falsified if: an async generator whose body is on the interpreter's stack is resumed again, or an `await` inside an async generator body settles a request the way a `yield` does
- `Broiler.VM.Profile.JavaScript.JsEngine.Execute(JsProgram, int, JsEnvironment?, JsValue, JsValue[], JsScriptFunction?, JsValue, JsCell?, JsFrame?, string?)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `AAA100`, PENDING
  - Falsified if: a program whose form differs from the engine's reaches ExecuteCore or emitted code
- `Broiler.VM.Profile.JavaScript.JsEngine.ExecuteCore<TMode>(JsProgram, int, JsEnvironment?, JsValue, JsValue[], JsScriptFunction?, JsValue, JsCell?, JsFrame?, JsNativeActivation?)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `22A2EC`, PENDING
  - Falsified if: an instantiation over a per-opcode step mode runs more or fewer than one charged instruction per call, the block instantiation stops anywhere but at the first boundary after its first instruction at which JsBaselineBlocks.StopsAfter holds, or the interpreted instantiation behaves differently from the loop before it was made generic
- `Broiler.VM.Profile.JavaScript.JsEngine.Land(System.Collections.Generic.List<JsEnvironment>, JsValue[], ref int, ref int, JsRegion, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `47E854`, PENDING
  - Falsified if: a landing keeps a scope deeper than its region's, leaves the stack at any height but the region's plus the value, or resumes anywhere but the region's handler
- `Broiler.VM.Profile.JavaScript.JsEngine.TryLand(JsNativeActivation, int, System.Exception)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=Critical, Spec=none cited, `74C4B4`, PENDING
  - Falsified if: it lands an exception the interpreter's filter would not land at the same instruction, or lands one elsewhere than the interpreter would
- `Broiler.VM.Profile.JavaScript.JsEngine.Delegate(JsFrame, JsValue[], ref int, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `E53DC7`, PENDING
  - Falsified if: a `return` or a `throw` that arrives while a `yield*` is suspended is not offered to the inner iterator first
- `Broiler.VM.Profile.JavaScript.JsEngine.DelegateAsync(JsFrame, JsValue[], ref int, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `197F48`, PENDING
  - Falsified if: an inner step of an async `yield*` reaches the outer body unawaited, or a `return` or a `throw` that arrives while one is suspended is not offered to the inner iterator first
- `Broiler.VM.Profile.JavaScript.JsEngine.ResolveName(System.Collections.Generic.List<JsEnvironment>, int, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `E98FC8`, PENDING
  - Falsified if: this walk answers with anything but an object a `PushObjectScope` placed on the chain
- `Broiler.VM.Profile.JavaScript.JsEngine.BigIntBinary(JsOpcode, JsValue, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `346910`, PENDING
  - Falsified if: a Number operand is mixed into a BigInt result, a BigInt is read as a Number, a zero divisor or negative exponent answers anything but a RangeError, or >>> answers a value
- `Broiler.VM.Profile.JavaScript.JsEngine.NumericStep(JsValue, bool)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `27D8C7`, PENDING
  - Falsified if: a BigInt operand is stepped through a double or answers a Number, or a Number operand answers anything but x + 1 or x - 1
- `Broiler.VM.Profile.JavaScript.JsEngine.BigIntResult(JsBigInt?)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `FAB1EA`, PENDING
  - Falsified if: an operation whose result is past JsBigInt.MaximumBits answers a value or escapes as anything but a RangeError
- `Broiler.VM.Profile.JavaScript.JsEngine.BigIntOrder(JsValue, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `309DC2`, PENDING
  - Falsified if: a BigInt and a Number or String are ordered other than by their mathematical values, or either is read through a double
- `Broiler.VM.Profile.JavaScript.JsEngine.OrderAgainstNumber(JsBigInt, double)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `5CB7FA`, PENDING
  - Falsified if: a BigInt is ordered against a Number other than by their mathematical values
- `Broiler.VM.Profile.JavaScript.JsEngine.ChargeComparison(JsValue, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `8F73E3`, PENDING
  - Falsified if: comparing two long equal strings costs what comparing two short ones costs
- `Broiler.VM.Profile.JavaScript.JsEngine.BigIntEqualsText(JsBigInt, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `783A53`, PENDING
  - Falsified if: a String that is not an integer, or spells another integer, is answered equal
- `Broiler.VM.Profile.JavaScript.IJsExecutionMode` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `7F0A56`, PENDING
  - Falsified if: a mode's opcode is read by the dispatch loop for a mode that reads its opcode from the code
- `Broiler.VM.Profile.JavaScript.IJsExecutionMode.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `A70DF0`, PENDING
  - Falsified if: a per-opcode step answers an opcode other than the one its handler was installed for
- `Broiler.VM.Profile.JavaScript.JsInterpreted` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `7881B9`, PENDING
  - Falsified if: the loop instantiated over this mode stops before a return, a suspension or an escaping exception
- `Broiler.VM.Profile.JavaScript.JsNativeEntry` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `9020C2`, PENDING
  - Falsified if: the loop instantiated over this mode charges for or runs an instruction
- `Broiler.VM.Profile.JavaScript.JsStepBlock` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `0FD013`, PENDING
  - Falsified if: the loop instantiated over this mode stops at its first boundary, runs an instruction after a later boundary at which JsBaselineBlocks.StopsAfter holds, or stops at a later boundary at which it does not
- `Broiler.VM.Profile.JavaScript.JsEvalMap` in `src/Broiler.VM.Profile.JavaScript/JsEvalMap.cs` - Security=High, Spec=none cited, `DE8935`, PENDING
  - Falsified if: a site, shape or declaration row reaches the executor without the verifier having checked it against the code and function tables
- `Broiler.VM.Profile.JavaScript.JsEvalView` in `src/Broiler.VM.Profile.JavaScript/JsEvalMap.cs` - Security=High, Spec=none cited, `6138EF`, PENDING
  - Falsified if: a name resolved through a view reaches a record other than the ones the view's own site row describes
- `Broiler.VM.Profile.JavaScript.JsEvalVariables` in `src/Broiler.VM.Profile.JavaScript/JsEvalMap.cs` - Security=High, Spec=none cited, `60C215`, PENDING
  - Falsified if: an eval-variables object becomes reachable from guest code, as a receiver, a property value or through a prototype
- `Broiler.VM.Profile.JavaScript.JsPause` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `6E5458`, PENDING
  - Falsified if: anything reachable from the realm is published through this payload
- `Broiler.VM.Profile.JavaScript.JsContinuation` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `231B03`, PENDING
  - Falsified if: a continuation is honoured against an instance that did not produce it
- `Broiler.VM.Profile.JavaScript.JsInstance.Environment` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `1C7767`, PENDING
  - Falsified if: this environment is asked for a mediator outside an invocation it supplied one for
- `Broiler.VM.Profile.JavaScript.JsExecution.RunHostTurn(VmProfileId, JsInstance, IJsHostSurface)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `A28546`, PENDING
  - Falsified if: a turn touches the realm outside the step bracket it opens
- `Broiler.VM.Profile.JavaScript.JsExecution.InstallHostSurface(JsEngine, IJsHostSurface)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `AA8194`, PENDING
  - Falsified if: an embedder that throws leaves an instance a caller can obtain
- `Broiler.VM.Profile.JavaScript.JsExecution.StepEntryPoint` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `815A20`, PENDING
  - Falsified if: a guest pause creates a core suspension, or a step runs more than one job
- `Broiler.VM.Profile.JavaScript.JsExecution.StepJobs(VmProfileId, JsInstance, int)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `B5AF9E`, PENDING
  - Falsified if: a step parks with an empty queue, or completes with a job still due
- `Broiler.VM.Profile.JavaScript.JsExecution.Resume(VmProfileId, IVmInstanceState, IVmProfileContinuation)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `5DF4EF`, PENDING
  - Falsified if: a continuation is honoured against an instance that did not produce it
- `Broiler.VM.Profile.JavaScript.JsExecution.Unwind(IVmProfileContinuation)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `CE785F`, PENDING
  - Falsified if: guest code runs during an unwind
- `Broiler.VM.Profile.JavaScript.JsExecution.RunOnGuestStack(System.Action)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `63D0AD`, PENDING
  - Falsified if: host-supplied setup runs on the caller's stack
- `Broiler.VM.Profile.JavaScript.JsExecution.RunOnGuestStack(JsInstance, uint?)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `D9C81E`, PENDING
  - Falsified if: guest code runs on the caller's stack, or an exception the guest raised does not reach the caller
- `Broiler.VM.Profile.JavaScript.JsExecution.RunOneJobOnGuestStack(JsInstance)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `94AAA9`, PENDING
  - Falsified if: a job runs on the caller's stack, or a job that throws ends the stepping
- `Broiler.VM.Profile.JavaScript.JsEnvironment` in `src/Broiler.VM.Profile.JavaScript/JsFunction.cs` - Security=High, Spec=none cited, `D0AAD1`, PENDING
  - Falsified if: a lookup by name reaches a slot of a declarative record
- `Broiler.VM.Profile.JavaScript.JsEnvironment.JsEnvironment(int, JsEnvironment?, JsEvalView)` in `src/Broiler.VM.Profile.JavaScript/JsFunction.cs` - Security=High, Spec=none cited, `F9EB3D`, PENDING
  - Falsified if: a boundary record is created with a parent other than the calling frame's current record
- `Broiler.VM.Profile.JavaScript.JsEnvironment.EvalVariables` in `src/Broiler.VM.Profile.JavaScript/JsFunction.cs` - Security=High, Spec=none cited, `FF611F`, PENDING
  - Falsified if: a lookup by name reaches a slot of this record rather than a binding an evaluation introduced
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
- `Broiler.VM.Profile.JavaScript.JsHandleTable` in `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` - Security=Critical, Spec=none cited, `1FF218`, PENDING
  - Falsified if: TryResolve answers an object for a word whose entry was released since the word was issued, or a compaction releases an entry some live slab word, permanent entry or nursery handle names
- `Broiler.VM.Profile.JavaScript.JsHandleTable.Free` in `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` - Security=High, Spec=none cited, `560BED`, PENDING
  - Falsified if: a slot in this state holds a value or is reissued while another slot answers its generation
- `Broiler.VM.Profile.JavaScript.JsHandleTable.Live` in `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` - Security=High, Spec=none cited, `008B26`, PENDING
  - Falsified if: a slot in this state is released while a root names it
- `Broiler.VM.Profile.JavaScript.JsHandleTable.Permanent` in `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` - Security=High, Spec=none cited, `4F1233`, PENDING
  - Falsified if: a slot in this state is released before the table is
- `Broiler.VM.Profile.JavaScript.JsHandleTable.Retired` in `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` - Security=High, Spec=none cited, `08FF61`, PENDING
  - Falsified if: a slot in this state is reissued
- `Broiler.VM.Profile.JavaScript.JsHandleTable.JsHandleTable(IJsWordRoots, bool, int)` in `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` - Security=High, Spec=none cited, `622C5F`, PENDING
  - Falsified if: a table is created whose compactions scan any slab other than its own instance's
- `Broiler.VM.Profile.JavaScript.JsHandleTable.HandleFor(object, ushort, bool)` in `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` - Security=Critical, Spec=none cited, `48DA47`, PENDING
  - Falsified if: two live entries hold the same object, or the word answered is released by a compaction before the next safepoint
- `Broiler.VM.Profile.JavaScript.JsHandleTable.TryResolve(ulong, out object?)` in `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` - Security=Critical, Spec=none cited, `6E1598`, PENDING
  - Falsified if: this answers true for a word whose index, generation or tag differs from a holding entry's, or answers an object other than the one the word was issued for
- `Broiler.VM.Profile.JavaScript.JsHandleTable.Safepoint()` in `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` - Security=Critical, Spec=none cited, `32CA5F`, PENDING
  - Falsified if: under handle-stress a safepoint returns without a compaction having run
- `Broiler.VM.Profile.JavaScript.JsHandleTable.Compact()` in `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` - Security=Critical, Spec=none cited, `A9A332`, PENDING
  - Falsified if: an entry named by a live slab word, a nursery handle or a permanent entry is released, or an entry named by none of them survives
- `Broiler.VM.Profile.JavaScript.JsHandleTable.MarkRoot(ulong)` in `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` - Security=Critical, Spec=none cited, `0860FA`, PENDING
  - Falsified if: a live handle word whose entry does not hold a value is passed over without a defect
- `Broiler.VM.Profile.JavaScript.JsHandleTable.Allocate()` in `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` - Security=High, Spec=none cited, `767A29`, PENDING
  - Falsified if: a slot is answered that is live, permanent or retired
- `Broiler.VM.Profile.JavaScript.JsHandleTable.Release(int)` in `src/Broiler.VM.Profile.JavaScript/JsHandleTable.cs` - Security=Critical, Spec=none cited, `68B984`, PENDING
  - Falsified if: a released slot keeps its generation, keeps its reference, or is reissued after its generation wrapped to zero
- `Broiler.VM.Profile.JavaScript.JsHostObject` in `src/Broiler.VM.Profile.JavaScript/JsHostObject.cs` - Security=High, Spec=none cited, `5BF8DC`, PENDING
  - Falsified if: a handler is consulted for a name this object's own storage already holds
- `Broiler.VM.Profile.JavaScript.JsHostObject.TryGetOwnProperty(string, out JsProperty)` in `src/Broiler.VM.Profile.JavaScript/JsHostObject.cs` - Security=High, Spec=none cited, `0A2ACE`, PENDING
  - Falsified if: the handler answers a key the base found
- `Broiler.VM.Profile.JavaScript.JsHostObject.SetOwnProperty(string, JsProperty)` in `src/Broiler.VM.Profile.JavaScript/JsHostObject.cs` - Security=High, Spec=none cited, `539B64`, PENDING
  - Falsified if: an assignment reaches the handler for a key this object's storage already holds
- `Broiler.VM.Profile.JavaScript.JsHostObject.OfferNamedWrite(string, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsHostObject.cs` - Security=High, Spec=none cited, `60F60B`, PENDING
  - Falsified if: an exception the handler raises unwinds through interpreter frames untranslated
- `Broiler.VM.Profile.JavaScript.JsHostObject.DeleteOwnProperty(string)` in `src/Broiler.VM.Profile.JavaScript/JsHostObject.cs` - Security=High, Spec=none cited, `889091`, PENDING
  - Falsified if: an index key reaches the deletion hook, or the ordinary deletion is skipped after the hook returned
- `Broiler.VM.Profile.JavaScript.JsHostRealm` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `88DEB7`, PENDING
  - Falsified if: a crossing runs outside a step, on another thread, or without charging HostCalls
- `Broiler.VM.Profile.JavaScript.JsHostRealm.EndStep()` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `EF50C7`, PENDING
  - Falsified if: an operation whose allowance was spent completes because host code caught the abort
- `Broiler.VM.Profile.JavaScript.JsHostRealm.DefineIndex(JsHostValue, uint, JsHostValue, JsHostPropertyFlags)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `19C400`, PENDING
  - Falsified if: an index defined here is not found by an operation that walks length
- `Broiler.VM.Profile.JavaScript.JsHostRealm.TryReadArrayBuffer(JsHostValue, out byte[])` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `9302B3`, PENDING
  - Falsified if: a guest-writable property or function decides the answer, or the realm's own storage is answered
- `Broiler.VM.Profile.JavaScript.JsHostRealm.TryReadArrayBuffer(JsHostValue, System.Span<byte>, out int)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `780A25`, PENDING
  - Falsified if: a destination is written when the answer is not Copied, or is written before the charge is admitted
- `Broiler.VM.Profile.JavaScript.JsHostRealm.NewArrayBuffer(System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `E9CCCF`, PENDING
  - Falsified if: a buffer is answered after its fuel or live-bytes charge was refused, or a later write to the caller's memory changes it
- `Broiler.VM.Profile.JavaScript.JsHostRealm.BufferBytes(JsHostValue, out byte[]?)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `78A87E`, PENDING
  - Falsified if: an object that is not the realm's own buffer type answers Copied
- `Broiler.VM.Profile.JavaScript.JsHostRealm.DetachArrayBuffer(JsHostValue)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `D9D821`, PENDING
  - Falsified if: a buffer this detaches can still be read or written through any view, or a guest reaches this without a function an embedder installed
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Invoke(JsHostValue, JsHostValue, System.ReadOnlySpan<JsHostValue>)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `790C56`, PENDING
  - Falsified if: a guest call from host code skips the call-depth charge or lets a JsAbort escape as a guest throw
- `Broiler.VM.Profile.JavaScript.JsHostRealm.EvaluateScript(string, string, bool)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `634ACD`, PENDING
  - Falsified if: a host script runs outside a step, without charging HostCalls, or lets a JsAbort escape as a guest throw
- `Broiler.VM.Profile.JavaScript.JsHostRealm.EnqueueJob(System.Action)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `749324`, PENDING
  - Falsified if: a host job and a guest job run in an order neither queue decided
- `Broiler.VM.Profile.JavaScript.JsHostRealm.DrainJobs(int)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `0D75AE`, PENDING
  - Falsified if: a drain runs more than its limit, or a throwing job discards the queue behind it
- `Broiler.VM.Profile.JavaScript.JsHostRealm.NewPromiseCapability()` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `33371B`, PENDING
  - Falsified if: the promise is built through a binding a guest can replace
- `Broiler.VM.Profile.JavaScript.JsHostRealm.ResolvePromise(JsHostPromiseCapability, JsHostValue)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `402C27`, PENDING
  - Falsified if: a second resolution changes the promise or queues a reaction
- `Broiler.VM.Profile.JavaScript.JsHostRealm.RejectPromise(JsHostPromiseCapability, JsHostValue)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `586445`, PENDING
  - Falsified if: a rejection after a resolution changes the promise or queues a reaction
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Settle(JsHostPromiseCapability, JsHostValue, bool)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `531055`, PENDING
  - Falsified if: a capability another realm minted settles a promise in this one
- `Broiler.VM.Profile.JavaScript.JsHostRealm.LoadModule(string, string)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `B6FBE2`, PENDING
  - Falsified if: a module reaches the realm without the artifact provider, a module body runs, or one key answers two handles
- `Broiler.VM.Profile.JavaScript.JsHostRealm.EvaluateModule(JsHostModule)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `418E05`, PENDING
  - Falsified if: two calls answer two promises, a module body runs twice, the promise fulfils while a module of the graph is still suspended, or it fulfils for a graph holding an errored module
- `Broiler.VM.Profile.JavaScript.JsHostRealm.CompleteModuleRequest(JsHostModuleRequest)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `1CDAD4`, PENDING
  - Falsified if: a request settles twice, settles outside a step of its own realm, or settles other than through the job queue
- `Broiler.VM.Profile.JavaScript.JsHostRealm.FailModuleRequest(JsHostModuleRequest, JsHostErrorKind, string)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `882B8B`, PENDING
  - Falsified if: a failed request settles twice, or settles outside a step of its own realm
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Claim(JsHostModuleRequest)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `36AF6C`, PENDING
  - Falsified if: a request another realm offered, or one still being offered, is settled here
- `Broiler.VM.Profile.JavaScript.JsHostRealm.OfferModuleRequest(string, string, JsPromiseObject)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `7999FB`, PENDING
  - Falsified if: a loader's JsHostThrowException unwinds through interpreter frames untranslated, or a request the realm answered can be completed again
- `Broiler.VM.Profile.JavaScript.JsHostRealm.OfferDeletion(IJsHostExoticDeletion, string)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `20E4E7`, PENDING
  - Falsified if: a handler's JsHostThrowException unwinds through interpreter frames untranslated
- `Broiler.VM.Profile.JavaScript.JsHostRealm.HookRaised(System.Exception)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `7B7F0D`, PENDING
  - Falsified if: a hook's JsHostThrowException or JsHostSurfaceException unwinds through interpreter frames untranslated
- `Broiler.VM.Profile.JavaScript.JsHostRealm.TryEnterHook()` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `9046ED`, PENDING
  - Falsified if: an exotic hook runs without charging HostCalls, outside a step, or after the realm latched an abort
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Raised(JsHostThrowException)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `9154C2`, PENDING
  - Falsified if: a host's thrown value that this realm refuses escapes as a JsHostSurfaceException
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Wrap(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `5F3135`, PENDING
  - Falsified if: two calls over one guest object answer two different JsHostRef instances
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Unwrap(JsHostValue)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `F7AD59`, PENDING
  - Falsified if: a value minted by another realm resolves rather than being refused
- `Broiler.VM.Profile.JavaScript.JsHostRealm.AdmitBigInt(System.Numerics.BigInteger)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `EABD9A`, PENDING
  - Falsified if: a BigInt wider than JsBigInt.MaximumBits reaches the realm, or one reaches a realm whose composition declined the surface
- `Broiler.VM.Profile.JavaScript.JsHostRealm.UnwrapAtCrossing(JsHostValue)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `6B2764`, PENDING
  - Falsified if: a JsThrow or a JsAbort escapes a public crossing untranslated
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Installing` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `DBCA8A`, PENDING
  - Falsified if: a member the realm installs reaches an exotic handler as an assignment
- `Broiler.VM.Profile.JavaScript.JsHostRealm.BindConstructor(JsHostFunction)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `1A9B56`, PENDING
  - Falsified if: a nested construction leaves the outer body reading the inner target
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Bind(JsHostFunction)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `9EA8DF`, PENDING
  - Falsified if: a host body's CLR exception unwinds through interpreter frames uncaught
- `Broiler.VM.Profile.JavaScript.JsHostRealm.CloneSerialize(JsHostValue, long, long)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `BCF1B2`, PENDING
  - Falsified if: a serialization runs outside a step, or a refusal reaches host code as anything but a guest throw
- `Broiler.VM.Profile.JavaScript.JsHostRealm.CloneSerializeWithTransfer(JsHostValue, JsHostValue[], long, long)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `2B215E`, PENDING
  - Falsified if: a refused transfer leaves any listed buffer detached, or a completed one leaves any attached
- `Broiler.VM.Profile.JavaScript.JsHostRealm.CloneDeserialize(JsCloneCarrier)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `28EF4E`, PENDING
  - Falsified if: a deserialization runs outside a step, or answers an object built on another realm's intrinsics
- `Broiler.VM.Profile.JavaScript.JsHostRealm.DetachClone(JsHostValue, System.ReadOnlySpan<JsHostValue>)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `37F58D`, PENDING
  - Falsified if: a carrier is made outside a step, holds an object of this realm, or leaves a listed buffer attached after returning
- `Broiler.VM.Profile.JavaScript.JsHostRealm.AdoptClone(object)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `1EC4C5`, PENDING
  - Falsified if: an adoption answers an object on another realm's intrinsics, accepts a foreign carrier, or adopts a single-use carrier twice
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Enter(ulong)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `48F054`, PENDING
  - Falsified if: a crossing proceeds while the realm is outside a step or on another thread
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Thrown(JsThrow)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `871C26`, PENDING
  - Falsified if: a guest throw of any value reaches host code as anything but a JsHostThrowException or the latched termination
- `Broiler.VM.Profile.JavaScript.JsHostRealm.Latch(JsAbort)` in `src/Broiler.VM.Profile.JavaScript/JsHostRealm.cs` - Security=High, Spec=none cited, `F6EE30`, PENDING
  - Falsified if: an abort reaches host code as anything a catch clause can clear
- `Broiler.VM.Profile.JavaScript.JsHostRef` in `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` - Security=High, Spec=none cited, `5A4DC0`, PENDING
  - Falsified if: two reads of one guest object answer two instances, or one instance names two guest objects
- `Broiler.VM.Profile.JavaScript.JsHostTerminatedException` in `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` - Security=High, Spec=none cited, `CECBE0`, PENDING
  - Falsified if: an operation whose allowance was spent completes normally because host code caught this
- `Broiler.VM.Profile.JavaScript.IJsHostSurface` in `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` - Security=High, Spec=none cited, `8FEF7F`, PENDING
  - Falsified if: a realm is handed to a surface a composition did not register, or outside a step
- `Broiler.VM.Profile.JavaScript.IJsHostExotic` in `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` - Security=High, Spec=none cited, `FADADB`, PENDING
  - Falsified if: a handler is consulted for a name the object's own storage already holds
- `Broiler.VM.Profile.JavaScript.IJsHostExoticDeletion` in `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` - Security=High, Spec=none cited, `7EB180`, PENDING
  - Falsified if: an index key or a symbol reaches the hook, or one deletion offers a name twice
- `Broiler.VM.Profile.JavaScript.JsHostPromiseCapability` in `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` - Security=High, Spec=none cited, `5E7572`, PENDING
  - Falsified if: a capability settles a promise twice, or settles one outside a step of its own realm
- `Broiler.VM.Profile.JavaScript.JsHostModule` in `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` - Security=High, Spec=none cited, `E66A7A`, PENDING
  - Falsified if: two handles, or two namespaces, exist for one module key in one realm
- `Broiler.VM.Profile.JavaScript.JsHostModuleRequest` in `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` - Security=High, Spec=none cited, `F7D8EF`, PENDING
  - Falsified if: a request settles its promise twice, or settles it outside a step of its own realm
- `Broiler.VM.Profile.JavaScript.IJsHostModuleLoader` in `src/Broiler.VM.Profile.JavaScript/JsHostValue.cs` - Security=High, Spec=none cited, `F28A83`, PENDING
  - Falsified if: a deferred import reaches the realm without the artifact provider and the core's verification
- `Broiler.VM.Profile.JavaScript.JsNativeAbiObservation` in `src/Broiler.VM.Profile.JavaScript/JsNativeAbi.cs` - Security=High, Spec=none cited, `AB829C`, PENDING
  - Falsified if: a field here reports something the trampoline did not record
- `Broiler.VM.Profile.JavaScript.JsNativeAbi` in `src/Broiler.VM.Profile.JavaScript/JsNativeAbi.cs` - Security=Critical, Spec=none cited, `2AA164`, PENDING
  - Falsified if: any product execution path calls this, or a mapping it makes is writable and executable at once
- `Broiler.VM.Profile.JavaScript.JsNativeAbi.Run(System.ReadOnlySpan<byte>, uint, uint, int, int, double[], long, int)` in `src/Broiler.VM.Profile.JavaScript/JsNativeAbi.cs` - Security=Critical, Spec=none cited, `AAE7EE`, PENDING
  - Falsified if: this reports a stack pointer the trampoline did not record, or it returns without releasing the mapping
- `Broiler.VM.Profile.JavaScript.JsNativeAbi.RunBaseline(System.ReadOnlySpan<byte>, uint, uint, uint, System.Span<long>)` in `src/Broiler.VM.Profile.JavaScript/JsNativeAbi.cs` - Security=Critical, Spec=none cited, `1F95D2`, PENDING
  - Falsified if: this records a handler stack pointer the stub did not write, sends a slot anywhere but the stub, or returns without releasing the mapping
- `Broiler.VM.Profile.JavaScript.JsNativeActivation` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `7BCAA8`, PENDING
  - Falsified if: a handler reaches an activation other than the one whose emitted frame is innermost on its thread, or an object a handler touches is reachable only from an emitted frame
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.cookies` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=High, Spec=none cited, `2F3773`, PENDING
  - Falsified if: two activations are ever given the same cookie
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.current` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `4421A1`, PENDING
  - Falsified if: this names an activation while no emitted code of that activation is on this thread's stack
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.JsNativeActivation(JsEngine, JsProgram, int, JsEnvironment?, JsValue, JsValue[], JsScriptFunction?, JsValue, JsCell?, JsFrame?)` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=High, Spec=none cited, `3F3C6C`, PENDING
  - Falsified if: an activation is created with a cookie another activation already holds, or with entry values other than the ones the dispatch loop was called with
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Code` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=High, Spec=none cited, `975847`, PENDING
  - Falsified if: this is any array other than the program's own code section
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Cookie` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=High, Spec=none cited, `73BB14`, PENDING
  - Falsified if: a handler acts on this activation for a frame whose cookie differs
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Stack` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=High, Spec=none cited, `5C6FE1`, PENDING
  - Falsified if: a step runs over a stack other than the one the entry built or borrowed from the frame
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Scopes` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=High, Spec=none cited, `0E6121`, PENDING
  - Falsified if: a step runs over a scope list other than the one the entry built or borrowed from the frame
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Sp` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=High, Spec=none cited, `1FED1E`, PENDING
  - Falsified if: a step starts at a height other than the one the previous step or the entry stopped at
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Pc` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `4E6E02`, PENDING
  - Falsified if: a handler starts a step at any offset other than this one
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Exited` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=High, Spec=none cited, `05335F`, PENDING
  - Falsified if: a handler runs an instruction after the activation exited
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Segment` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `61730A`, PENDING
  - Falsified if: a value step reads or writes words of a segment other than the one the activation's region was opened in, or runs after the region was closed
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.SlabFrame` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `A73741`, PENDING
  - Falsified if: this names any header other than the one the entry opened for this activation
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Plan` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `3EC17C`, PENDING
  - Falsified if: a value step of this activation reads a plan other than the one of its own unit the template scan held the payload to
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Landed` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=High, Spec=none cited, `3B3559`, PENDING
  - Falsified if: a step that landed leaves this false, or one that did not land leaves it true
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Pending` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=High, Spec=none cited, `AE8477`, PENDING
  - Falsified if: an exception a step caught is dropped rather than raised by the managed frame that entered the emitted code
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Caller` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `275E20`, PENDING
  - Falsified if: a direct callee runs with no caller named, or names one other than the activation whose call site entered it
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Current` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `859662`, PENDING
  - Falsified if: this is written anywhere but around the one call that enters emitted code, or is not restored when that call returns
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.Step<TMode>(JsBaselineFrame*, int, JsOpcode)` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `EAD7FE`, PENDING
  - Falsified if: a step starts at an offset, with an opcode or for an activation other than what the managed side computed, or an exception escapes into emitted code
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.StepValue<TMode>(JsValueFrame*, int, JsOpcode)` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `D0E8EF`, PENDING
  - Falsified if: a value step starts at an offset that is not a reached instruction start of its activation's unit, with an opcode or for an activation other than the emitted code's own, runs with no open region, runs before its debt is charged, leaves a word the arm wrote unencoded or unpublished, or lets an exception escape into emitted code
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.SettleValue(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `2A7F11`, PENDING
  - Falsified if: a settlement answers an offset other than the one it was handed, runs an instruction, writes a word, or leaves a debt it was handed uncharged without parking the refusal
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.PrepareCall<TMode>(JsValueFrame*, int, JsValueFrame*)` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `F68147`, PENDING
  - Falsified if: a direct call is prepared for a call site whose checks fail, for a callee the ordinary helper should call, without its debt charged, or answers the direct call with the thread slot naming anything but the prepared callee; or anything but the direct call escapes into emitted code
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.FinishCall(JsValueFrame*, int, JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `C2D3A2`, PENDING
  - Falsified if: it acts for a callee or a caller whose cookie is not its context's, leaves the thread slot naming the callee, leaves the callee's region, depth or referrer taken, writes the answer anywhere but the call's own output slot, or lets an exception escape into emitted code
- `Broiler.VM.Profile.JavaScript.JsNativeActivation.RaiseAt(JsNativeActivation, int, int, int, System.Exception)` in `src/Broiler.VM.Profile.JavaScript/JsNativeActivation.cs` - Security=Critical, Spec=none cited, `8E4FA9`, PENDING
  - Falsified if: an exception lands where the interpreter's filter would not land it, one it would land is parked, it is thrown again to land, or a landing's words are left unencoded or unpublished
- `Broiler.VM.Profile.JavaScript.JsNativeInstance` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `7ABDB0`, PENDING
  - Falsified if: anything reachable from this instance holds a managed reference emitted code can dereference
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.OperandSlabSlots` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `A12001`, PENDING
  - Falsified if: an emitted unit can be entered when fewer slots remain than its region needs
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.OutgoingArgumentHeadroom` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `9C90B6`, PENDING
  - Falsified if: a call's argument store reaches past the end of the operand slab's array
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.FuelPerInvocation` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `5FA9CD`, PENDING
  - Falsified if: an invocation runs past this many charged events without the meter being consulted
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.JsNativeInstance(JsProgram, IVmExecutionEnvironment, JsNativePage, double[], double[], double[], long[])` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `361FFF`, PENDING
  - Falsified if: an instance is constructed around a mapping that is not armed
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Operands` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `489648`, PENDING
  - Falsified if: this slab is not pinned, so the collector can move it while emitted code holds its address
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Bindings` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `75149F`, PENDING
  - Falsified if: this slab is not pinned, or a slot of it is readable by emitted code before an initialiser stored to it
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Fuel` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `5C814B`, PENDING
  - Falsified if: this slab is not pinned, so the collector can move it while emitted code is decrementing it
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.TryCreate(JsProgram, IVmExecutionEnvironment)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `030FF4`, PENDING
  - Falsified if: an instance is produced whose mapping is not armed
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Dispose()` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `4FEDBC`, PENDING
  - Falsified if: a mapping outlives the instance that owns it, or is released while an invocation is still inside it
- `Broiler.VM.Profile.JavaScript.JsNativeExecution` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `65CC7E`, PENDING
  - Falsified if: the form an invocation runs under differs from the form its handle carried when it was minted
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.CarriesEmittedCode(JsProgram)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `2492DE`, PENDING
  - Falsified if: this answers true for an artifact emitted for a convention this process does not use
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.HostArchitecture` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `658D99`, PENDING
  - Falsified if: this names a convention other than the one this process actually uses
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.Instantiate(JsProgram, IVmExecutionEnvironment)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `DC113E`, PENDING
  - Falsified if: an artifact emitted for another architecture is instantiated
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.Invoke(VmProfileId, JsNativeInstance, in VmInvocationRequest)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `660C65`, PENDING
  - Falsified if: a value is reported that the emitted code did not leave in the frame's first operand slot
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.TryFindSymbol(JsProgram, uint, out uint)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `B97A49`, PENDING
  - Falsified if: an offset is returned for a unit the symbol table does not name
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.Render(double)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `A768C5`, PENDING
  - Falsified if: a completion of undefined is reported as a Number, or a Number as undefined
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.TypeOf(double)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `0FC6E8`, PENDING
  - Falsified if: this disagrees with what the interpreter's typeof answers for the same value
- `Broiler.VM.Profile.JavaScript.JsMappedArguments` in `src/Broiler.VM.Profile.JavaScript/JsObject.cs` - Security=High, Spec=none cited, `0C7FAE`, PENDING
  - Falsified if: a mapped index answers a value other than its parameter's current binding, or a deleted, accessor-replaced or non-writable index still writes or reads the parameter
- `Broiler.VM.Profile.JavaScript.JsProgram.NativeValueImage` in `src/Broiler.VM.Profile.JavaScript/JsProgram.cs` - Security=High, Spec=none cited, `BC2C12`, PENDING
  - Falsified if: this differs from the image the verifier scanned and re-emitted the value-form payload against
- `Broiler.VM.Profile.JavaScript.JsProgram.ValuePlan(int)` in `src/Broiler.VM.Profile.JavaScript/JsProgram.cs` - Security=Critical, Spec=none cited, `6F3F18`, PENDING
  - Falsified if: it answers a plan other than the one the template scan held the unit's payload to, or replaces a published plan
- `Broiler.VM.Profile.JavaScript.JsProgram.NativeValueForm` in `src/Broiler.VM.Profile.JavaScript/JsProgram.cs` - Security=High, Spec=none cited, `942D8A`, PENDING
  - Falsified if: this differs from the form byte of the emitted code section the verifier scanned
- `Broiler.VM.Profile.JavaScript.JsProgram.NativePage` in `src/Broiler.VM.Profile.JavaScript/JsProgram.cs` - Security=Critical, Spec=none cited, `F05E06`, PENDING
  - Falsified if: this is written more than once for one program, or holds a mapping that is not armed
- `Broiler.VM.Profile.JavaScript.JsRealm.GetAsyncIterator(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.AsyncGenerator.cs` - Security=High, Spec=none cited, `EFF362`, PENDING
  - Falsified if: a `for await` over an object carrying `Symbol.asyncIterator` reaches the synchronous wrapper, or one over an Array of promises answers the promises rather than their values
- `Broiler.VM.Profile.JavaScript.JsRealm.BigIntWrap(JsEngine, JsValue[], bool)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.BigInt.cs` - Security=High, Spec=none cited, `2F9DBC`, PENDING
  - Falsified if: the value is converted before the width, a Number value is accepted, or a result past MaximumBits is answered
- `Broiler.VM.Profile.JavaScript.JsRealm.BinaryResizeBuffer(JsEngine, JsArrayBuffer, int)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Binary.cs` - Security=High, Spec=none cited, `A87CC7`, PENDING
  - Falsified if: a resize the fuel meter, the live-bytes ceiling or the allocator refuses leaves the buffer at a different length or with different bytes
- `Broiler.VM.Profile.JavaScript.JsRealm` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `60DD8D`, PENDING
  - Falsified if: a clone graph carries a reference into the realm that produced it
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneSerialize(JsValue, long, long)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `33240C`, PENDING
  - Falsified if: a serialization completes for a function, a symbol, a Proxy or an unlisted brand, or its work is not charged
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneSerializeWithTransfer(JsValue, JsValue[], long, long)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `8C1269`, PENDING
  - Falsified if: a refused transfer leaves any listed buffer detached, or a completed one leaves any listed buffer attached
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneDeserialize(JsCloneCarrier)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `185FAE`, PENDING
  - Falsified if: a rebuilt object has a prototype that is not this realm's, or two adoptions share a buffer
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneBuffer(JsCloneRecord)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `A67638`, PENDING
  - Falsified if: the destination buffer shares its byte array with the carrier or another adoption
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneView(JsCloneRecord, JsObject?[])` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `36BE86`, PENDING
  - Falsified if: a view is built over bytes outside its buffer, or two views of one source buffer see two buffers
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneBigInt(JsCloneSlot)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `4CF6A0`, PENDING
  - Falsified if: a BigInt slot is rebuilt uncharged, or a slot of another kind is read as a BigInt
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `499643`, PENDING
  - Falsified if: one source object produces two records, or a bound is passed without a refusal
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.maxEntries` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `88E752`, PENDING
  - Falsified if: this bound is larger than JsCloneCarrier.MaxEntries
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.maxBytes` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `5AF476`, PENDING
  - Falsified if: this bound is larger than JsCloneCarrier.MaxBytes
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.Run(JsValue, JsValue[])` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `549634`, PENDING
  - Falsified if: getters run in an order other than HTML's recursive depth-first order
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.CheckTransferList(JsValue[])` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `D25FD2`, PENDING
  - Falsified if: a transfer list naming a non-ArrayBuffer or one buffer twice is admitted
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.Validate(System.Collections.Generic.List<(JsArrayBuffer Buffer, JsCloneRecord Record)>)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `E91B98`, PENDING
  - Falsified if: a detached buffer or one past the byte bound reaches the commit
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.Slot(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `DF1346`, PENDING
  - Falsified if: a Symbol value serializes rather than being refused
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.Record(JsObject)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `1A9F4B`, PENDING
  - Falsified if: an object whose brand is not in the matrix is recorded as any kind at all
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.RefuseDetachedView(bool)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `551994`, PENDING
  - Falsified if: a typed array or DataView over a detached buffer serializes, whether or not its buffer was recorded first
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.RefuseResizable(JsArrayBuffer)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `22B2B9`, PENDING
  - Falsified if: a resizable ArrayBuffer, or a typed array or DataView over one, serializes
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.Buffer(JsObject, JsArrayBuffer)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `BF8169`, PENDING
  - Falsified if: the carrier holds the source buffer's own byte array, or a detached buffer serializes
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.New(JsObject, JsCloneKind, out JsCloneRecord)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `E54E31`, PENDING
  - Falsified if: an object is entered in the memory map after its contents are serialized
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.Count(long)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `CDB11E`, PENDING
  - Falsified if: an entry is added past the bound without a refusal
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.CountText(string)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `2BB13D`, PENDING
  - Falsified if: distinct string text is carried past the byte bound without a refusal
- `Broiler.VM.Profile.JavaScript.JsRealm.CloneWriter.CountBigInt(JsBigInt)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Clone.cs` - Security=High, Spec=none cited, `0651DE`, PENDING
  - Falsified if: a BigInt is carried past the byte bound without a refusal, or carried uncharged
- `Broiler.VM.Profile.JavaScript.JsRealm.DisposeScopeFold(JsEngine, JsDisposeScope, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Disposal.cs` - Security=High, Spec=none cited, `6F7B69`, PENDING
  - Falsified if: a second throw during one scope's disposal replaces the first instead of suppressing it, or a forced return is reported as a thrown value
- `Broiler.VM.Profile.JavaScript.JsRealm.DisposeScopeStep(JsEngine, JsDisposeScope, out JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Disposal.cs` - Security=High, Spec=none cited, `F3F6F6`, PENDING
  - Falsified if: an entry of one scope is called twice, a synchronous entry's result is awaited, or a nullish await using owes more or fewer awaits than DisposeResources performs
- `Broiler.VM.Profile.JavaScript.JsRealm.DisposeScopeEnd(JsEngine, JsDisposeScope, bool)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Disposal.cs` - Security=High, Spec=none cited, `2AD8C7`, PENDING
  - Falsified if: a scope whose disposal threw completes normally, or a scope disposed twice runs any entry twice
- `Broiler.VM.Profile.JavaScript.JsRealm.JsDisposalRun` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Disposal.cs` - Security=High, Spec=none cited, `E99113`, PENDING
  - Falsified if: an entry of one disposeAsync() run is called twice, or the run settles its promise before every entry's awaited result has settled
- `Broiler.VM.Profile.JavaScript.JsRealm.JsDisposalRun.Continue(JsEngine)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Disposal.cs` - Security=High, Spec=none cited, `A97F1B`, PENDING
  - Falsified if: a guest throw from a disposer escapes disposeAsync() synchronously instead of rejecting its promise
- `Broiler.VM.Profile.JavaScript.JsDisposeScope.From(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Disposal.cs` - Security=High, Spec=none cited, `119C32`, PENDING
  - Falsified if: a value that is not a disposal scope is treated as one
- `Broiler.VM.Profile.JavaScript.JsRealm` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, Spec=none cited, `60DD8D`, PENDING
  - Falsified if: anything in this file is built into a realm whose composition did not admit broiler.javascript.dynamic
- `Broiler.VM.Profile.JavaScript.JsRealm.EvalIntrinsic` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, Spec=none cited, `D093DD`, PENDING
  - Falsified if: this holds a function object the guest can reach under any other name
- `Broiler.VM.Profile.JavaScript.JsRealm.IsEvalIntrinsic(JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, Spec=none cited, `B458DE`, PENDING
  - Falsified if: it answers true for a function object this realm did not build as its own eval
- `Broiler.VM.Profile.JavaScript.JsRealm.SetupDynamic()` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, Spec=none cited, `209E66`, PENDING
  - Falsified if: it installs a global that turns source into code without going through the mediator
- `Broiler.VM.Profile.JavaScript.JsRealm.FromSource(JsEngine, JsValue[])` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Dynamic.cs` - Security=High, Spec=none cited, `01B333`, PENDING
  - Falsified if: the assembled source is evaluated anywhere but the global scope
- `Broiler.VM.Profile.JavaScript.JsRealm.PromiseSchedule(JsEngine, JsPromiseReaction, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Promise.cs` - Security=High, Spec=none cited, `C89A73`, PENDING
  - Falsified if: a promise reaction runs before the synchronous continuation of whatever settled or observed the promise
- `Broiler.VM.Profile.JavaScript.JsRealm.NewHostPromise(JsEngine)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Promise.cs` - Security=High, Spec=none cited, `D45325`, PENDING
  - Falsified if: a host promise is built from a prototype a guest assignment to Promise can replace
- `Broiler.VM.Profile.JavaScript.JsRealm.AwaitOn(JsEngine, JsValue, System.Action<JsEngine, JsValue, bool>)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.Promise.cs` - Security=High, Spec=none cited, `6F23A9`, PENDING
  - Falsified if: an `await` of a value that is not a promise continues without yielding to the job queue
- `Broiler.VM.Profile.JavaScript.JsRealm.NormalizeText(JsEngine, string, bool, bool)` in `src/Broiler.VM.Profile.JavaScript/JsRealm.String.cs` - Security=High, Spec=none cited, `E5D2CC`, PENDING
  - Falsified if: a guest string makes normalize allocate or loop over an expansion it was not charged for, or answer other than the pinned NormalizationTest.txt vectors
- `Broiler.VM.Profile.JavaScript.JsValueHelpers` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=Critical, Spec=none cited, `D64011`, PENDING
  - Falsified if: an emitted call through this table reaches code other than one value step of the instruction its slot is named for or the refusing entry point
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Table` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=Critical, Spec=none cited, `FD6363`, PENDING
  - Falsified if: this is non-zero while a defined opcode's slot holds anything but that opcode's own entry point, or an undefined byte's slot holds anything but the refusing one
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.JsValueHelpers()` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=Critical, Spec=none cited, `2BED28`, PENDING
  - Falsified if: the published table maps a defined opcode byte to an entry point built for another opcode, or an undefined byte to anything but the refusing entry point
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Sound(nint[], nint)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=Critical, Spec=none cited, `16E5DF`, PENDING
  - Falsified if: this answers true for a table in which two defined opcodes or two fixed slots share an entry point, a defined opcode has the refusing one or a fixed slot's, a fixed slot is a defined opcode's or holds the refusing entry or nothing, or another undefined byte has anything but the refusing one
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Settle(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=Critical, Spec=none cited, `83F643`, PENDING
  - Falsified if: this runs an instruction, or does anything but the checked settlement of JsNativeActivation.SettleValue
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Prepare(JsValueFrame*, int, JsValueFrame*)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=Critical, Spec=none cited, `798F00`, PENDING
  - Falsified if: this does anything but the checked preparation of JsNativeActivation.PrepareCall over the Call opcode's own arm
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Finish(JsValueFrame*, int, JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=Critical, Spec=none cited, `A4B1AF`, PENDING
  - Falsified if: this does anything but the checked finish of JsNativeActivation.FinishCall
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Undefined(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `91A42A`, PENDING
  - Falsified if: this reads or writes any state, or answers anything but the defect status
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Nop(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `CDB853`, PENDING
  - Falsified if: this runs anything other than one Nop at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadUndefined(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B00E19`, PENDING
  - Falsified if: this runs anything other than one LoadUndefined at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadNull(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4904CD`, PENDING
  - Falsified if: this runs anything other than one LoadNull at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadTrue(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `38BA56`, PENDING
  - Falsified if: this runs anything other than one LoadTrue at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadFalse(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `47DC63`, PENDING
  - Falsified if: this runs anything other than one LoadFalse at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadConstant(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `031E57`, PENDING
  - Falsified if: this runs anything other than one LoadConstant at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadThis(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B6B8A3`, PENDING
  - Falsified if: this runs anything other than one LoadThis at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.NewArguments(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `982568`, PENDING
  - Falsified if: this runs anything other than one NewArguments at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadNewTarget(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `CE25FB`, PENDING
  - Falsified if: this runs anything other than one LoadNewTarget at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadArgument(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `90B8BA`, PENDING
  - Falsified if: this runs anything other than one LoadArgument at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.RestArguments(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4EC35C`, PENDING
  - Falsified if: this runs anything other than one RestArguments at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadScoped(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E221CE`, PENDING
  - Falsified if: this runs anything other than one LoadScoped at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.StoreScoped(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `88719E`, PENDING
  - Falsified if: this runs anything other than one StoreScoped at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.InitialiseScoped(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `186E60`, PENDING
  - Falsified if: this runs anything other than one InitialiseScoped at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadGlobal(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `711B90`, PENDING
  - Falsified if: this runs anything other than one LoadGlobal at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.StoreGlobal(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D37A84`, PENDING
  - Falsified if: this runs anything other than one StoreGlobal at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadGlobalOrUndefined(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C2AE3F`, PENDING
  - Falsified if: this runs anything other than one LoadGlobalOrUndefined at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.PushScope(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1022FA`, PENDING
  - Falsified if: this runs anything other than one PushScope at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.PopScope(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `04792F`, PENDING
  - Falsified if: this runs anything other than one PopScope at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.CopyScope(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `AD07A1`, PENDING
  - Falsified if: this runs anything other than one CopyScope at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DeclareGlobal(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4F774B`, PENDING
  - Falsified if: this runs anything other than one DeclareGlobal at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.PushObjectScope(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `861D59`, PENDING
  - Falsified if: this runs anything other than one PushObjectScope at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ResolveName(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F05111`, PENDING
  - Falsified if: this runs anything other than one ResolveName at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.NewObject(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1992AB`, PENDING
  - Falsified if: this runs anything other than one NewObject at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.NewArray(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `8CA964`, PENDING
  - Falsified if: this runs anything other than one NewArray at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.GetProperty(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `0A6252`, PENDING
  - Falsified if: this runs anything other than one GetProperty at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.SetProperty(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6AD29A`, PENDING
  - Falsified if: this runs anything other than one SetProperty at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.GetIndex(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C28AD4`, PENDING
  - Falsified if: this runs anything other than one GetIndex at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.SetIndex(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4C1583`, PENDING
  - Falsified if: this runs anything other than one SetIndex at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DefineField(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `92A728`, PENDING
  - Falsified if: this runs anything other than one DefineField at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DefineIndexed(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `793E92`, PENDING
  - Falsified if: this runs anything other than one DefineIndexed at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DeleteProperty(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D086DA`, PENDING
  - Falsified if: this runs anything other than one DeleteProperty at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DeleteIndex(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `80806E`, PENDING
  - Falsified if: this runs anything other than one DeleteIndex at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DefineGetter(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F610DF`, PENDING
  - Falsified if: this runs anything other than one DefineGetter at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DefineSetter(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B1706E`, PENDING
  - Falsified if: this runs anything other than one DefineSetter at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DefineMethod(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `FF497C`, PENDING
  - Falsified if: this runs anything other than one DefineMethod at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadSuperProperty(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `FF75AB`, PENDING
  - Falsified if: this runs anything other than one LoadSuperProperty at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.StoreSuperProperty(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `37FE7A`, PENDING
  - Falsified if: this runs anything other than one StoreSuperProperty at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArrayAppend(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `3B0245`, PENDING
  - Falsified if: this runs anything other than one ArrayAppend at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Closure(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B09C7D`, PENDING
  - Falsified if: this runs anything other than one Closure at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Call(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `FF3AE6`, PENDING
  - Falsified if: this runs anything other than one Call at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Construct(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `338957`, PENDING
  - Falsified if: this runs anything other than one Construct at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Return(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `55103C`, PENDING
  - Falsified if: this runs anything other than one Return at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ReturnUndefined(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `7C7641`, PENDING
  - Falsified if: this runs anything other than one ReturnUndefined at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.CallEval(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `CB067F`, PENDING
  - Falsified if: this runs anything other than one CallEval at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.SuperCall(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F46B10`, PENDING
  - Falsified if: this runs anything other than one SuperCall at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.SuperCallForwarded(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `067599`, PENDING
  - Falsified if: this runs anything other than one SuperCallForwarded at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.NewClass(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B9D87F`, PENDING
  - Falsified if: this runs anything other than one NewClass at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArrayHoles(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `033A04`, PENDING
  - Falsified if: this runs anything other than one ArrayHoles at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.SpreadArray(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `34D25B`, PENDING
  - Falsified if: this runs anything other than one SpreadArray at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.SpreadObject(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4B990D`, PENDING
  - Falsified if: this runs anything other than one SpreadObject at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.CallSpread(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B9BE8F`, PENDING
  - Falsified if: this runs anything other than one CallSpread at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ConstructSpread(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A041D3`, PENDING
  - Falsified if: this runs anything other than one ConstructSpread at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.SuperCallSpread(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `DABE25`, PENDING
  - Falsified if: this runs anything other than one SuperCallSpread at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.SetPrototypeLiteral(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2F03D8`, PENDING
  - Falsified if: this runs anything other than one SetPrototypeLiteral at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Add(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `234207`, PENDING
  - Falsified if: this runs anything other than one Add at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Subtract(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `046109`, PENDING
  - Falsified if: this runs anything other than one Subtract at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Multiply(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `72E6D0`, PENDING
  - Falsified if: this runs anything other than one Multiply at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Divide(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5BA206`, PENDING
  - Falsified if: this runs anything other than one Divide at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Remainder(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `480C60`, PENDING
  - Falsified if: this runs anything other than one Remainder at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Exponent(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D97834`, PENDING
  - Falsified if: this runs anything other than one Exponent at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Negate(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `71D864`, PENDING
  - Falsified if: this runs anything other than one Negate at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ToNumber(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D0051A`, PENDING
  - Falsified if: this runs anything other than one ToNumber at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Not(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5F7F50`, PENDING
  - Falsified if: this runs anything other than one Not at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.BitwiseNot(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1306C3`, PENDING
  - Falsified if: this runs anything other than one BitwiseNot at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LessThan(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B9DCBE`, PENDING
  - Falsified if: this runs anything other than one LessThan at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LessThanOrEqual(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `008E9A`, PENDING
  - Falsified if: this runs anything other than one LessThanOrEqual at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.GreaterThan(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A6A4A2`, PENDING
  - Falsified if: this runs anything other than one GreaterThan at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.GreaterThanOrEqual(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2D471C`, PENDING
  - Falsified if: this runs anything other than one GreaterThanOrEqual at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.StrictEquals(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `EC89ED`, PENDING
  - Falsified if: this runs anything other than one StrictEquals at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.StrictNotEquals(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `0E59FD`, PENDING
  - Falsified if: this runs anything other than one StrictNotEquals at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LooseEquals(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `21CF78`, PENDING
  - Falsified if: this runs anything other than one LooseEquals at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LooseNotEquals(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `45C69B`, PENDING
  - Falsified if: this runs anything other than one LooseNotEquals at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.BitwiseOr(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `19A5A0`, PENDING
  - Falsified if: this runs anything other than one BitwiseOr at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.BitwiseAnd(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2D8C62`, PENDING
  - Falsified if: this runs anything other than one BitwiseAnd at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.BitwiseXor(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `3EE95E`, PENDING
  - Falsified if: this runs anything other than one BitwiseXor at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ShiftLeft(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F97573`, PENDING
  - Falsified if: this runs anything other than one ShiftLeft at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ShiftRight(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `65B054`, PENDING
  - Falsified if: this runs anything other than one ShiftRight at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ShiftRightUnsigned(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `CA92BB`, PENDING
  - Falsified if: this runs anything other than one ShiftRightUnsigned at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.TypeOf(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `01A086`, PENDING
  - Falsified if: this runs anything other than one TypeOf at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.InstanceOf(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `248605`, PENDING
  - Falsified if: this runs anything other than one InstanceOf at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.In(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5FF055`, PENDING
  - Falsified if: this runs anything other than one In at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Void(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `87AD5F`, PENDING
  - Falsified if: this runs anything other than one Void at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.RequireCoercible(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `3B0113`, PENDING
  - Falsified if: this runs anything other than one RequireCoercible at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ToPropertyKey(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E13AB6`, PENDING
  - Falsified if: this runs anything other than one ToPropertyKey at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.GetTemplateObject(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1F9053`, PENDING
  - Falsified if: this runs anything other than one GetTemplateObject at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Jump(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `270671`, PENDING
  - Falsified if: this runs anything other than one Jump at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.JumpIfFalse(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C4DE38`, PENDING
  - Falsified if: this runs anything other than one JumpIfFalse at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.JumpIfTrue(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `591FF8`, PENDING
  - Falsified if: this runs anything other than one JumpIfTrue at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Throw(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5BEC33`, PENDING
  - Falsified if: this runs anything other than one Throw at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ForInStart(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `9C9F99`, PENDING
  - Falsified if: this runs anything other than one ForInStart at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ForInNext(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `246D7B`, PENDING
  - Falsified if: this runs anything other than one ForInNext at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.IterateStart(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `7E6547`, PENDING
  - Falsified if: this runs anything other than one IterateStart at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.IterateNext(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `47D48A`, PENDING
  - Falsified if: this runs anything other than one IterateNext at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.IterateRest(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `EBF67B`, PENDING
  - Falsified if: this runs anything other than one IterateRest at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.IterateClose(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D26D42`, PENDING
  - Falsified if: this runs anything other than one IterateClose at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Yield(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `08D312`, PENDING
  - Falsified if: this runs anything other than one Yield at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.YieldDelegate(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F8F00E`, PENDING
  - Falsified if: this runs anything other than one YieldDelegate at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Await(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `707DD0`, PENDING
  - Falsified if: this runs anything other than one Await at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadImport(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `DE9194`, PENDING
  - Falsified if: this runs anything other than one LoadImport at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ThrowImmutable(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `EC4BFE`, PENDING
  - Falsified if: this runs anything other than one ThrowImmutable at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DefineClassElement(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B7008F`, PENDING
  - Falsified if: this runs anything other than one DefineClassElement at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Pop(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `14DC3D`, PENDING
  - Falsified if: this runs anything other than one Pop at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Duplicate(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `730659`, PENDING
  - Falsified if: this runs anything other than one Duplicate at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DuplicateTwo(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `038CB0`, PENDING
  - Falsified if: this runs anything other than one DuplicateTwo at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Swap(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `516106`, PENDING
  - Falsified if: this runs anything other than one Swap at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Pick(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `647F94`, PENDING
  - Falsified if: this runs anything other than one Pick at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.NewPrivateName(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F77E53`, PENDING
  - Falsified if: this runs anything other than one NewPrivateName at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadPrivate(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C5D3E1`, PENDING
  - Falsified if: this runs anything other than one LoadPrivate at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.StorePrivate(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `DA5D2D`, PENDING
  - Falsified if: this runs anything other than one StorePrivate at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.HasPrivate(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6A21D4`, PENDING
  - Falsified if: this runs anything other than one HasPrivate at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.RunStaticElements(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E4808B`, PENDING
  - Falsified if: this runs anything other than one RunStaticElements at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.IterateStartAsync(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `45D829`, PENDING
  - Falsified if: this runs anything other than one IterateStartAsync at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.IterateNextAsync(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `930D4D`, PENDING
  - Falsified if: this runs anything other than one IterateNextAsync at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.IterateAwaitStep(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `035850`, PENDING
  - Falsified if: this runs anything other than one IterateAwaitStep at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.IterateCloseAsync(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C9E9E1`, PENDING
  - Falsified if: this runs anything other than one IterateCloseAsync at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.IterateCloseCheck(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B72045`, PENDING
  - Falsified if: this runs anything other than one IterateCloseCheck at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DeclareGlobalLet(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A8C10D`, PENDING
  - Falsified if: this runs anything other than one DeclareGlobalLet at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DeclareGlobalConst(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `05F0F2`, PENDING
  - Falsified if: this runs anything other than one DeclareGlobalConst at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.InitialiseGlobalLexical(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `21210C`, PENDING
  - Falsified if: this runs anything other than one InitialiseGlobalLexical at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DeleteGlobalBinding(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `54FF86`, PENDING
  - Falsified if: this runs anything other than one DeleteGlobalBinding at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.EnterBody(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `82546F`, PENDING
  - Falsified if: this runs anything other than one EnterBody at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ImportCall(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C79CBD`, PENDING
  - Falsified if: this runs anything other than one ImportCall at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ImportMeta(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5EEE8F`, PENDING
  - Falsified if: this runs anything other than one ImportMeta at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.CallEvalSpread(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E83DB8`, PENDING
  - Falsified if: this runs anything other than one CallEvalSpread at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadEvalName(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1D2E83`, PENDING
  - Falsified if: this runs anything other than one LoadEvalName at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadEvalNameOrUndefined(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `993E16`, PENDING
  - Falsified if: this runs anything other than one LoadEvalNameOrUndefined at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.StoreEvalName(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `9B8EE2`, PENDING
  - Falsified if: this runs anything other than one StoreEvalName at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.LoadEvalNameWithBase(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `010F0B`, PENDING
  - Falsified if: this runs anything other than one LoadEvalNameWithBase at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DeleteEvalName(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `BB023C`, PENDING
  - Falsified if: this runs anything other than one DeleteEvalName at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.WithBaseObject(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2E9C39`, PENDING
  - Falsified if: this runs anything other than one WithBaseObject at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.StoreEvalVariable(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B8225B`, PENDING
  - Falsified if: this runs anything other than one StoreEvalVariable at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DisposeScope(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `411816`, PENDING
  - Falsified if: this runs anything other than one DisposeScope at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DisposeAdd(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `26C59F`, PENDING
  - Falsified if: this runs anything other than one DisposeAdd at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DisposeFold(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `74383E`, PENDING
  - Falsified if: this runs anything other than one DisposeFold at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DisposeStep(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `48ADC8`, PENDING
  - Falsified if: this runs anything other than one DisposeStep at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.DisposeEnd(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F5AD61`, PENDING
  - Falsified if: this runs anything other than one DisposeEnd at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ToNumeric(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `777668`, PENDING
  - Falsified if: this runs anything other than one ToNumeric at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Increment(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `FE0117`, PENDING
  - Falsified if: this runs anything other than one Increment at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.Decrement(JsValueFrame*, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C88297`, PENDING
  - Falsified if: this runs anything other than one Decrement at the offset the managed side expects, through its own step
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNop` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `EE2A74`, PENDING
  - Falsified if: this mode names an opcode other than Nop
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNop.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D7D8F6`, PENDING
  - Falsified if: this answers any opcode other than Nop
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadUndefined` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `FA6D6B`, PENDING
  - Falsified if: this mode names an opcode other than LoadUndefined
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadUndefined.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E0D46D`, PENDING
  - Falsified if: this answers any opcode other than LoadUndefined
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadNull` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `902CD4`, PENDING
  - Falsified if: this mode names an opcode other than LoadNull
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadNull.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `8F90FD`, PENDING
  - Falsified if: this answers any opcode other than LoadNull
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadTrue` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1B4E9F`, PENDING
  - Falsified if: this mode names an opcode other than LoadTrue
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadTrue.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `3D4366`, PENDING
  - Falsified if: this answers any opcode other than LoadTrue
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadFalse` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `38E162`, PENDING
  - Falsified if: this mode names an opcode other than LoadFalse
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadFalse.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `289B9B`, PENDING
  - Falsified if: this answers any opcode other than LoadFalse
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadConstant` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D4298D`, PENDING
  - Falsified if: this mode names an opcode other than LoadConstant
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadConstant.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A1257B`, PENDING
  - Falsified if: this answers any opcode other than LoadConstant
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadThis` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2639BE`, PENDING
  - Falsified if: this mode names an opcode other than LoadThis
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadThis.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `93E9E1`, PENDING
  - Falsified if: this answers any opcode other than LoadThis
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNewArguments` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6D576B`, PENDING
  - Falsified if: this mode names an opcode other than NewArguments
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNewArguments.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A8B424`, PENDING
  - Falsified if: this answers any opcode other than NewArguments
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadNewTarget` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `FFC187`, PENDING
  - Falsified if: this mode names an opcode other than LoadNewTarget
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadNewTarget.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D1003C`, PENDING
  - Falsified if: this answers any opcode other than LoadNewTarget
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadArgument` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C87458`, PENDING
  - Falsified if: this mode names an opcode other than LoadArgument
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadArgument.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A879E1`, PENDING
  - Falsified if: this answers any opcode other than LoadArgument
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmRestArguments` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E31A74`, PENDING
  - Falsified if: this mode names an opcode other than RestArguments
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmRestArguments.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `CC6674`, PENDING
  - Falsified if: this answers any opcode other than RestArguments
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadScoped` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1B43C8`, PENDING
  - Falsified if: this mode names an opcode other than LoadScoped
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadScoped.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `DA48FC`, PENDING
  - Falsified if: this answers any opcode other than LoadScoped
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStoreScoped` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `06A9E1`, PENDING
  - Falsified if: this mode names an opcode other than StoreScoped
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStoreScoped.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `3F1707`, PENDING
  - Falsified if: this answers any opcode other than StoreScoped
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmInitialiseScoped` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2267FE`, PENDING
  - Falsified if: this mode names an opcode other than InitialiseScoped
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmInitialiseScoped.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F14691`, PENDING
  - Falsified if: this answers any opcode other than InitialiseScoped
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadGlobal` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `0C59C5`, PENDING
  - Falsified if: this mode names an opcode other than LoadGlobal
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadGlobal.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A7AB7B`, PENDING
  - Falsified if: this answers any opcode other than LoadGlobal
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStoreGlobal` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6069F2`, PENDING
  - Falsified if: this mode names an opcode other than StoreGlobal
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStoreGlobal.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F87126`, PENDING
  - Falsified if: this answers any opcode other than StoreGlobal
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadGlobalOrUndefined` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C81635`, PENDING
  - Falsified if: this mode names an opcode other than LoadGlobalOrUndefined
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadGlobalOrUndefined.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `35BA98`, PENDING
  - Falsified if: this answers any opcode other than LoadGlobalOrUndefined
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmPushScope` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `62441E`, PENDING
  - Falsified if: this mode names an opcode other than PushScope
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmPushScope.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6F0172`, PENDING
  - Falsified if: this answers any opcode other than PushScope
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmPopScope` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `517D6C`, PENDING
  - Falsified if: this mode names an opcode other than PopScope
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmPopScope.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `115BB4`, PENDING
  - Falsified if: this answers any opcode other than PopScope
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmCopyScope` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4140DB`, PENDING
  - Falsified if: this mode names an opcode other than CopyScope
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmCopyScope.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `BDDF52`, PENDING
  - Falsified if: this answers any opcode other than CopyScope
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeclareGlobal` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6FC9D1`, PENDING
  - Falsified if: this mode names an opcode other than DeclareGlobal
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeclareGlobal.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `8DA33D`, PENDING
  - Falsified if: this answers any opcode other than DeclareGlobal
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmPushObjectScope` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D5898B`, PENDING
  - Falsified if: this mode names an opcode other than PushObjectScope
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmPushObjectScope.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2FB524`, PENDING
  - Falsified if: this answers any opcode other than PushObjectScope
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmResolveName` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `79343F`, PENDING
  - Falsified if: this mode names an opcode other than ResolveName
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmResolveName.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `EFF26A`, PENDING
  - Falsified if: this answers any opcode other than ResolveName
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNewObject` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `218F4B`, PENDING
  - Falsified if: this mode names an opcode other than NewObject
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNewObject.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A62141`, PENDING
  - Falsified if: this answers any opcode other than NewObject
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNewArray` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D98C06`, PENDING
  - Falsified if: this mode names an opcode other than NewArray
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNewArray.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `CD07BC`, PENDING
  - Falsified if: this answers any opcode other than NewArray
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmGetProperty` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F15954`, PENDING
  - Falsified if: this mode names an opcode other than GetProperty
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmGetProperty.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5835F4`, PENDING
  - Falsified if: this answers any opcode other than GetProperty
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSetProperty` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `761E3B`, PENDING
  - Falsified if: this mode names an opcode other than SetProperty
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSetProperty.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F1EBDB`, PENDING
  - Falsified if: this answers any opcode other than SetProperty
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmGetIndex` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `FFA5E9`, PENDING
  - Falsified if: this mode names an opcode other than GetIndex
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmGetIndex.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `37F5C2`, PENDING
  - Falsified if: this answers any opcode other than GetIndex
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSetIndex` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D1E0A0`, PENDING
  - Falsified if: this mode names an opcode other than SetIndex
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSetIndex.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `19F768`, PENDING
  - Falsified if: this answers any opcode other than SetIndex
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDefineField` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C2B8D8`, PENDING
  - Falsified if: this mode names an opcode other than DefineField
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDefineField.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `BC936A`, PENDING
  - Falsified if: this answers any opcode other than DefineField
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDefineIndexed` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `FE311B`, PENDING
  - Falsified if: this mode names an opcode other than DefineIndexed
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDefineIndexed.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `83BF75`, PENDING
  - Falsified if: this answers any opcode other than DefineIndexed
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeleteProperty` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B7FA0C`, PENDING
  - Falsified if: this mode names an opcode other than DeleteProperty
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeleteProperty.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `CCED5A`, PENDING
  - Falsified if: this answers any opcode other than DeleteProperty
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeleteIndex` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B5D4B8`, PENDING
  - Falsified if: this mode names an opcode other than DeleteIndex
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeleteIndex.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2C1D9C`, PENDING
  - Falsified if: this answers any opcode other than DeleteIndex
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDefineGetter` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C15C85`, PENDING
  - Falsified if: this mode names an opcode other than DefineGetter
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDefineGetter.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2316AF`, PENDING
  - Falsified if: this answers any opcode other than DefineGetter
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDefineSetter` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B0359A`, PENDING
  - Falsified if: this mode names an opcode other than DefineSetter
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDefineSetter.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E0C41F`, PENDING
  - Falsified if: this answers any opcode other than DefineSetter
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDefineMethod` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `04784B`, PENDING
  - Falsified if: this mode names an opcode other than DefineMethod
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDefineMethod.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `00E07A`, PENDING
  - Falsified if: this answers any opcode other than DefineMethod
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadSuperProperty` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `DC6EEE`, PENDING
  - Falsified if: this mode names an opcode other than LoadSuperProperty
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadSuperProperty.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `3430FE`, PENDING
  - Falsified if: this answers any opcode other than LoadSuperProperty
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStoreSuperProperty` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B1D68B`, PENDING
  - Falsified if: this mode names an opcode other than StoreSuperProperty
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStoreSuperProperty.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `9AB412`, PENDING
  - Falsified if: this answers any opcode other than StoreSuperProperty
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmArrayAppend` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `857B55`, PENDING
  - Falsified if: this mode names an opcode other than ArrayAppend
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmArrayAppend.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D01310`, PENDING
  - Falsified if: this answers any opcode other than ArrayAppend
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmClosure` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `3C8014`, PENDING
  - Falsified if: this mode names an opcode other than Closure
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmClosure.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B9B08E`, PENDING
  - Falsified if: this answers any opcode other than Closure
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmCall` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D1D7BE`, PENDING
  - Falsified if: this mode names an opcode other than Call
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmCall.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `AEA809`, PENDING
  - Falsified if: this answers any opcode other than Call
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmConstruct` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `67FA34`, PENDING
  - Falsified if: this mode names an opcode other than Construct
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmConstruct.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `AB9410`, PENDING
  - Falsified if: this answers any opcode other than Construct
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmReturn` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F7DDAF`, PENDING
  - Falsified if: this mode names an opcode other than Return
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmReturn.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `EF4A69`, PENDING
  - Falsified if: this answers any opcode other than Return
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmReturnUndefined` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F3794E`, PENDING
  - Falsified if: this mode names an opcode other than ReturnUndefined
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmReturnUndefined.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `7D9B6E`, PENDING
  - Falsified if: this answers any opcode other than ReturnUndefined
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmCallEval` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A69582`, PENDING
  - Falsified if: this mode names an opcode other than CallEval
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmCallEval.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D5D4C4`, PENDING
  - Falsified if: this answers any opcode other than CallEval
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSuperCall` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4449B4`, PENDING
  - Falsified if: this mode names an opcode other than SuperCall
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSuperCall.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `716B4E`, PENDING
  - Falsified if: this answers any opcode other than SuperCall
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSuperCallForwarded` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4B4F91`, PENDING
  - Falsified if: this mode names an opcode other than SuperCallForwarded
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSuperCallForwarded.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6D1190`, PENDING
  - Falsified if: this answers any opcode other than SuperCallForwarded
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNewClass` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B05163`, PENDING
  - Falsified if: this mode names an opcode other than NewClass
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNewClass.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `254D0F`, PENDING
  - Falsified if: this answers any opcode other than NewClass
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmArrayHoles` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `8D4218`, PENDING
  - Falsified if: this mode names an opcode other than ArrayHoles
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmArrayHoles.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A0D86F`, PENDING
  - Falsified if: this answers any opcode other than ArrayHoles
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSpreadArray` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `8E0559`, PENDING
  - Falsified if: this mode names an opcode other than SpreadArray
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSpreadArray.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `10C694`, PENDING
  - Falsified if: this answers any opcode other than SpreadArray
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSpreadObject` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6BF9F0`, PENDING
  - Falsified if: this mode names an opcode other than SpreadObject
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSpreadObject.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `90CFF0`, PENDING
  - Falsified if: this answers any opcode other than SpreadObject
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmCallSpread` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `319A52`, PENDING
  - Falsified if: this mode names an opcode other than CallSpread
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmCallSpread.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B10BF3`, PENDING
  - Falsified if: this answers any opcode other than CallSpread
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmConstructSpread` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `69BDA6`, PENDING
  - Falsified if: this mode names an opcode other than ConstructSpread
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmConstructSpread.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A344AA`, PENDING
  - Falsified if: this answers any opcode other than ConstructSpread
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSuperCallSpread` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `000FCD`, PENDING
  - Falsified if: this mode names an opcode other than SuperCallSpread
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSuperCallSpread.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `935410`, PENDING
  - Falsified if: this answers any opcode other than SuperCallSpread
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSetPrototypeLiteral` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `DE8A44`, PENDING
  - Falsified if: this mode names an opcode other than SetPrototypeLiteral
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSetPrototypeLiteral.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F076A9`, PENDING
  - Falsified if: this answers any opcode other than SetPrototypeLiteral
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmAdd` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `0F9A51`, PENDING
  - Falsified if: this mode names an opcode other than Add
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmAdd.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5AF05B`, PENDING
  - Falsified if: this answers any opcode other than Add
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSubtract` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5C6988`, PENDING
  - Falsified if: this mode names an opcode other than Subtract
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSubtract.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5768D1`, PENDING
  - Falsified if: this answers any opcode other than Subtract
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmMultiply` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `BC373B`, PENDING
  - Falsified if: this mode names an opcode other than Multiply
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmMultiply.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E0007F`, PENDING
  - Falsified if: this answers any opcode other than Multiply
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDivide` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2AA909`, PENDING
  - Falsified if: this mode names an opcode other than Divide
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDivide.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `84FD41`, PENDING
  - Falsified if: this answers any opcode other than Divide
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmRemainder` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `3482F4`, PENDING
  - Falsified if: this mode names an opcode other than Remainder
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmRemainder.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `129811`, PENDING
  - Falsified if: this answers any opcode other than Remainder
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmExponent` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `10A908`, PENDING
  - Falsified if: this mode names an opcode other than Exponent
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmExponent.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `914BD7`, PENDING
  - Falsified if: this answers any opcode other than Exponent
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNegate` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `7210AB`, PENDING
  - Falsified if: this mode names an opcode other than Negate
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNegate.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C79B7B`, PENDING
  - Falsified if: this answers any opcode other than Negate
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmToNumber` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `83BA2E`, PENDING
  - Falsified if: this mode names an opcode other than ToNumber
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmToNumber.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C36BA6`, PENDING
  - Falsified if: this answers any opcode other than ToNumber
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNot` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `0ADF30`, PENDING
  - Falsified if: this mode names an opcode other than Not
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNot.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `75DF62`, PENDING
  - Falsified if: this answers any opcode other than Not
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmBitwiseNot` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `9A46F3`, PENDING
  - Falsified if: this mode names an opcode other than BitwiseNot
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmBitwiseNot.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F715CA`, PENDING
  - Falsified if: this answers any opcode other than BitwiseNot
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLessThan` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `FDB8F9`, PENDING
  - Falsified if: this mode names an opcode other than LessThan
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLessThan.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `EEF32A`, PENDING
  - Falsified if: this answers any opcode other than LessThan
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLessThanOrEqual` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `9FBAF0`, PENDING
  - Falsified if: this mode names an opcode other than LessThanOrEqual
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLessThanOrEqual.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F41DF2`, PENDING
  - Falsified if: this answers any opcode other than LessThanOrEqual
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmGreaterThan` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A68416`, PENDING
  - Falsified if: this mode names an opcode other than GreaterThan
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmGreaterThan.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E44031`, PENDING
  - Falsified if: this answers any opcode other than GreaterThan
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmGreaterThanOrEqual` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `991D86`, PENDING
  - Falsified if: this mode names an opcode other than GreaterThanOrEqual
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmGreaterThanOrEqual.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6DF1AF`, PENDING
  - Falsified if: this answers any opcode other than GreaterThanOrEqual
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStrictEquals` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `AB39EF`, PENDING
  - Falsified if: this mode names an opcode other than StrictEquals
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStrictEquals.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A8BF70`, PENDING
  - Falsified if: this answers any opcode other than StrictEquals
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStrictNotEquals` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `8A7F32`, PENDING
  - Falsified if: this mode names an opcode other than StrictNotEquals
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStrictNotEquals.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1F0424`, PENDING
  - Falsified if: this answers any opcode other than StrictNotEquals
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLooseEquals` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1C2C46`, PENDING
  - Falsified if: this mode names an opcode other than LooseEquals
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLooseEquals.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4B4670`, PENDING
  - Falsified if: this answers any opcode other than LooseEquals
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLooseNotEquals` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `717825`, PENDING
  - Falsified if: this mode names an opcode other than LooseNotEquals
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLooseNotEquals.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4EE2CC`, PENDING
  - Falsified if: this answers any opcode other than LooseNotEquals
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmBitwiseOr` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2A9347`, PENDING
  - Falsified if: this mode names an opcode other than BitwiseOr
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmBitwiseOr.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `28A30B`, PENDING
  - Falsified if: this answers any opcode other than BitwiseOr
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmBitwiseAnd` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6453A1`, PENDING
  - Falsified if: this mode names an opcode other than BitwiseAnd
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmBitwiseAnd.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D7BEA3`, PENDING
  - Falsified if: this answers any opcode other than BitwiseAnd
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmBitwiseXor` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B01B87`, PENDING
  - Falsified if: this mode names an opcode other than BitwiseXor
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmBitwiseXor.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B60E97`, PENDING
  - Falsified if: this answers any opcode other than BitwiseXor
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmShiftLeft` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B23F8D`, PENDING
  - Falsified if: this mode names an opcode other than ShiftLeft
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmShiftLeft.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `0FC726`, PENDING
  - Falsified if: this answers any opcode other than ShiftLeft
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmShiftRight` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `CEFB22`, PENDING
  - Falsified if: this mode names an opcode other than ShiftRight
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmShiftRight.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `58841A`, PENDING
  - Falsified if: this answers any opcode other than ShiftRight
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmShiftRightUnsigned` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `AF9597`, PENDING
  - Falsified if: this mode names an opcode other than ShiftRightUnsigned
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmShiftRightUnsigned.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A421C0`, PENDING
  - Falsified if: this answers any opcode other than ShiftRightUnsigned
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmTypeOf` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `9D6D91`, PENDING
  - Falsified if: this mode names an opcode other than TypeOf
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmTypeOf.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `EA8853`, PENDING
  - Falsified if: this answers any opcode other than TypeOf
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmInstanceOf` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `882998`, PENDING
  - Falsified if: this mode names an opcode other than InstanceOf
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmInstanceOf.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `7A23DA`, PENDING
  - Falsified if: this answers any opcode other than InstanceOf
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIn` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `58E843`, PENDING
  - Falsified if: this mode names an opcode other than In
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIn.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2A1104`, PENDING
  - Falsified if: this answers any opcode other than In
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmVoid` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `F86808`, PENDING
  - Falsified if: this mode names an opcode other than Void
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmVoid.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `172D6A`, PENDING
  - Falsified if: this answers any opcode other than Void
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmRequireCoercible` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `BD7E84`, PENDING
  - Falsified if: this mode names an opcode other than RequireCoercible
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmRequireCoercible.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `DD736E`, PENDING
  - Falsified if: this answers any opcode other than RequireCoercible
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmToPropertyKey` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `0C60CA`, PENDING
  - Falsified if: this mode names an opcode other than ToPropertyKey
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmToPropertyKey.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E6C46D`, PENDING
  - Falsified if: this answers any opcode other than ToPropertyKey
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmGetTemplateObject` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `824432`, PENDING
  - Falsified if: this mode names an opcode other than GetTemplateObject
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmGetTemplateObject.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `448C7D`, PENDING
  - Falsified if: this answers any opcode other than GetTemplateObject
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmJump` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1A3FEF`, PENDING
  - Falsified if: this mode names an opcode other than Jump
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmJump.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `FDAF9E`, PENDING
  - Falsified if: this answers any opcode other than Jump
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmJumpIfFalse` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `775656`, PENDING
  - Falsified if: this mode names an opcode other than JumpIfFalse
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmJumpIfFalse.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5DB205`, PENDING
  - Falsified if: this answers any opcode other than JumpIfFalse
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmJumpIfTrue` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `9157F5`, PENDING
  - Falsified if: this mode names an opcode other than JumpIfTrue
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmJumpIfTrue.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5D29ED`, PENDING
  - Falsified if: this answers any opcode other than JumpIfTrue
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmThrow` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2D7568`, PENDING
  - Falsified if: this mode names an opcode other than Throw
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmThrow.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `BAACFC`, PENDING
  - Falsified if: this answers any opcode other than Throw
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmForInStart` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `8FB7C6`, PENDING
  - Falsified if: this mode names an opcode other than ForInStart
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmForInStart.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6C92A5`, PENDING
  - Falsified if: this answers any opcode other than ForInStart
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmForInNext` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1F0B61`, PENDING
  - Falsified if: this mode names an opcode other than ForInNext
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmForInNext.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `40210D`, PENDING
  - Falsified if: this answers any opcode other than ForInNext
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateStart` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `119212`, PENDING
  - Falsified if: this mode names an opcode other than IterateStart
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateStart.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `0C1F4F`, PENDING
  - Falsified if: this answers any opcode other than IterateStart
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateNext` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `56A32E`, PENDING
  - Falsified if: this mode names an opcode other than IterateNext
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateNext.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E8268A`, PENDING
  - Falsified if: this answers any opcode other than IterateNext
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateRest` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `3F2315`, PENDING
  - Falsified if: this mode names an opcode other than IterateRest
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateRest.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D63D37`, PENDING
  - Falsified if: this answers any opcode other than IterateRest
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateClose` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `95888B`, PENDING
  - Falsified if: this mode names an opcode other than IterateClose
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateClose.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `BF8374`, PENDING
  - Falsified if: this answers any opcode other than IterateClose
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmYield` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `7B7F34`, PENDING
  - Falsified if: this mode names an opcode other than Yield
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmYield.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `0E5713`, PENDING
  - Falsified if: this answers any opcode other than Yield
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmYieldDelegate` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `85183A`, PENDING
  - Falsified if: this mode names an opcode other than YieldDelegate
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmYieldDelegate.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `BB6777`, PENDING
  - Falsified if: this answers any opcode other than YieldDelegate
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmAwait` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A49AA3`, PENDING
  - Falsified if: this mode names an opcode other than Await
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmAwait.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2F7951`, PENDING
  - Falsified if: this answers any opcode other than Await
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadImport` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6D0504`, PENDING
  - Falsified if: this mode names an opcode other than LoadImport
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadImport.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `957D8D`, PENDING
  - Falsified if: this answers any opcode other than LoadImport
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmThrowImmutable` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `8FBEAC`, PENDING
  - Falsified if: this mode names an opcode other than ThrowImmutable
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmThrowImmutable.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C977D7`, PENDING
  - Falsified if: this answers any opcode other than ThrowImmutable
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDefineClassElement` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4F173E`, PENDING
  - Falsified if: this mode names an opcode other than DefineClassElement
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDefineClassElement.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `56A2D2`, PENDING
  - Falsified if: this answers any opcode other than DefineClassElement
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmPop` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `EE5544`, PENDING
  - Falsified if: this mode names an opcode other than Pop
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmPop.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A61E2D`, PENDING
  - Falsified if: this answers any opcode other than Pop
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDuplicate` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C37345`, PENDING
  - Falsified if: this mode names an opcode other than Duplicate
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDuplicate.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `3E8968`, PENDING
  - Falsified if: this answers any opcode other than Duplicate
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDuplicateTwo` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5025B9`, PENDING
  - Falsified if: this mode names an opcode other than DuplicateTwo
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDuplicateTwo.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `435C7A`, PENDING
  - Falsified if: this answers any opcode other than DuplicateTwo
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSwap` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `406123`, PENDING
  - Falsified if: this mode names an opcode other than Swap
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmSwap.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E79E80`, PENDING
  - Falsified if: this answers any opcode other than Swap
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmPick` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `913279`, PENDING
  - Falsified if: this mode names an opcode other than Pick
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmPick.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A6A5C6`, PENDING
  - Falsified if: this answers any opcode other than Pick
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNewPrivateName` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `CF0B2C`, PENDING
  - Falsified if: this mode names an opcode other than NewPrivateName
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmNewPrivateName.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4B324A`, PENDING
  - Falsified if: this answers any opcode other than NewPrivateName
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadPrivate` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E6151A`, PENDING
  - Falsified if: this mode names an opcode other than LoadPrivate
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadPrivate.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E3F8D0`, PENDING
  - Falsified if: this answers any opcode other than LoadPrivate
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStorePrivate` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6315E4`, PENDING
  - Falsified if: this mode names an opcode other than StorePrivate
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStorePrivate.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E161C3`, PENDING
  - Falsified if: this answers any opcode other than StorePrivate
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmHasPrivate` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1158D1`, PENDING
  - Falsified if: this mode names an opcode other than HasPrivate
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmHasPrivate.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `468A35`, PENDING
  - Falsified if: this answers any opcode other than HasPrivate
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmRunStaticElements` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `70289D`, PENDING
  - Falsified if: this mode names an opcode other than RunStaticElements
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmRunStaticElements.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `276AB0`, PENDING
  - Falsified if: this answers any opcode other than RunStaticElements
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateStartAsync` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D94B0A`, PENDING
  - Falsified if: this mode names an opcode other than IterateStartAsync
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateStartAsync.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4D20AC`, PENDING
  - Falsified if: this answers any opcode other than IterateStartAsync
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateNextAsync` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `9EDB63`, PENDING
  - Falsified if: this mode names an opcode other than IterateNextAsync
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateNextAsync.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `021DE0`, PENDING
  - Falsified if: this answers any opcode other than IterateNextAsync
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateAwaitStep` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `489DEF`, PENDING
  - Falsified if: this mode names an opcode other than IterateAwaitStep
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateAwaitStep.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `988400`, PENDING
  - Falsified if: this answers any opcode other than IterateAwaitStep
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateCloseAsync` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `8734A8`, PENDING
  - Falsified if: this mode names an opcode other than IterateCloseAsync
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateCloseAsync.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `3D3E70`, PENDING
  - Falsified if: this answers any opcode other than IterateCloseAsync
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateCloseCheck` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `E0A0CC`, PENDING
  - Falsified if: this mode names an opcode other than IterateCloseCheck
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIterateCloseCheck.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `940D59`, PENDING
  - Falsified if: this answers any opcode other than IterateCloseCheck
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeclareGlobalLet` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `8CC6A1`, PENDING
  - Falsified if: this mode names an opcode other than DeclareGlobalLet
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeclareGlobalLet.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `FFF17F`, PENDING
  - Falsified if: this answers any opcode other than DeclareGlobalLet
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeclareGlobalConst` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `A2EC49`, PENDING
  - Falsified if: this mode names an opcode other than DeclareGlobalConst
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeclareGlobalConst.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `C6B233`, PENDING
  - Falsified if: this answers any opcode other than DeclareGlobalConst
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmInitialiseGlobalLexical` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `CE3114`, PENDING
  - Falsified if: this mode names an opcode other than InitialiseGlobalLexical
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmInitialiseGlobalLexical.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2F78D2`, PENDING
  - Falsified if: this answers any opcode other than InitialiseGlobalLexical
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeleteGlobalBinding` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `4D296D`, PENDING
  - Falsified if: this mode names an opcode other than DeleteGlobalBinding
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeleteGlobalBinding.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `BFB32C`, PENDING
  - Falsified if: this answers any opcode other than DeleteGlobalBinding
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmEnterBody` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5D36F6`, PENDING
  - Falsified if: this mode names an opcode other than EnterBody
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmEnterBody.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `BD1929`, PENDING
  - Falsified if: this answers any opcode other than EnterBody
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmImportCall` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `ED7F43`, PENDING
  - Falsified if: this mode names an opcode other than ImportCall
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmImportCall.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `33B0A7`, PENDING
  - Falsified if: this answers any opcode other than ImportCall
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmImportMeta` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D72C5E`, PENDING
  - Falsified if: this mode names an opcode other than ImportMeta
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmImportMeta.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2803F5`, PENDING
  - Falsified if: this answers any opcode other than ImportMeta
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmCallEvalSpread` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B149F0`, PENDING
  - Falsified if: this mode names an opcode other than CallEvalSpread
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmCallEvalSpread.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1FA07A`, PENDING
  - Falsified if: this answers any opcode other than CallEvalSpread
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadEvalName` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `53DAD0`, PENDING
  - Falsified if: this mode names an opcode other than LoadEvalName
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadEvalName.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `98D74C`, PENDING
  - Falsified if: this answers any opcode other than LoadEvalName
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadEvalNameOrUndefined` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2AFD77`, PENDING
  - Falsified if: this mode names an opcode other than LoadEvalNameOrUndefined
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadEvalNameOrUndefined.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `407A2F`, PENDING
  - Falsified if: this answers any opcode other than LoadEvalNameOrUndefined
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStoreEvalName` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `B76858`, PENDING
  - Falsified if: this mode names an opcode other than StoreEvalName
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStoreEvalName.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `8F0D6F`, PENDING
  - Falsified if: this answers any opcode other than StoreEvalName
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadEvalNameWithBase` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5B722F`, PENDING
  - Falsified if: this mode names an opcode other than LoadEvalNameWithBase
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmLoadEvalNameWithBase.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `697B36`, PENDING
  - Falsified if: this answers any opcode other than LoadEvalNameWithBase
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeleteEvalName` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D3D865`, PENDING
  - Falsified if: this mode names an opcode other than DeleteEvalName
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDeleteEvalName.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `1EF902`, PENDING
  - Falsified if: this answers any opcode other than DeleteEvalName
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmWithBaseObject` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `47CE85`, PENDING
  - Falsified if: this mode names an opcode other than WithBaseObject
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmWithBaseObject.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `ED1AB4`, PENDING
  - Falsified if: this answers any opcode other than WithBaseObject
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStoreEvalVariable` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `908837`, PENDING
  - Falsified if: this mode names an opcode other than StoreEvalVariable
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmStoreEvalVariable.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `CF3F82`, PENDING
  - Falsified if: this answers any opcode other than StoreEvalVariable
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDisposeScope` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `750866`, PENDING
  - Falsified if: this mode names an opcode other than DisposeScope
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDisposeScope.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `72A086`, PENDING
  - Falsified if: this answers any opcode other than DisposeScope
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDisposeAdd` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `85EA4F`, PENDING
  - Falsified if: this mode names an opcode other than DisposeAdd
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDisposeAdd.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `5DE6AA`, PENDING
  - Falsified if: this answers any opcode other than DisposeAdd
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDisposeFold` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `401FE0`, PENDING
  - Falsified if: this mode names an opcode other than DisposeFold
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDisposeFold.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `12D9B0`, PENDING
  - Falsified if: this answers any opcode other than DisposeFold
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDisposeStep` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2C51D1`, PENDING
  - Falsified if: this mode names an opcode other than DisposeStep
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDisposeStep.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2ACA17`, PENDING
  - Falsified if: this answers any opcode other than DisposeStep
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDisposeEnd` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `6EAB71`, PENDING
  - Falsified if: this mode names an opcode other than DisposeEnd
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDisposeEnd.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `3C4E84`, PENDING
  - Falsified if: this answers any opcode other than DisposeEnd
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmToNumeric` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `FD9D65`, PENDING
  - Falsified if: this mode names an opcode other than ToNumeric
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmToNumeric.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `D773B4`, PENDING
  - Falsified if: this answers any opcode other than ToNumeric
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIncrement` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `0E339A`, PENDING
  - Falsified if: this mode names an opcode other than Increment
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmIncrement.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2E6F4F`, PENDING
  - Falsified if: this answers any opcode other than Increment
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDecrement` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `2E3696`, PENDING
  - Falsified if: this mode names an opcode other than Decrement
- `Broiler.VM.Profile.JavaScript.JsValueHelpers.ArmDecrement.Opcode` in `src/Broiler.VM.Profile.JavaScript/JsValueHelpers.cs` - Security=High, Spec=none cited, `92C7FE`, PENDING
  - Falsified if: this answers any opcode other than Decrement
- `Broiler.VM.Profile.JavaScript.JsValueSlab` in `src/Broiler.VM.Profile.JavaScript/JsValueSlab.cs` - Security=High, Spec=none cited, `778523`, PENDING
  - Falsified if: a live word of some frame is not visited by Scan, or a word past a frame's published live length or past the top is visited
- `Broiler.VM.Profile.JavaScript.JsValueSlab.Headroom` in `src/Broiler.VM.Profile.JavaScript/JsValueSlab.cs` - Security=High, Spec=none cited, `02A8CF`, PENDING
  - Falsified if: a call's argument stores for a region that does not fit can reach past the end of the array
- `Broiler.VM.Profile.JavaScript.JsValueSlab.TryPushFrame(int, out int)` in `src/Broiler.VM.Profile.JavaScript/JsValueSlab.cs` - Security=High, Spec=none cited, `408AE9`, PENDING
  - Falsified if: a frame is opened whose header and region together reach past the advertised capacity
- `Broiler.VM.Profile.JavaScript.JsValueSlab.Publish(int, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueSlab.cs` - Security=High, Spec=none cited, `276FB2`, PENDING
  - Falsified if: a live length larger than the frame's region is published
- `Broiler.VM.Profile.JavaScript.JsValueSlab.PopFrame(int)` in `src/Broiler.VM.Profile.JavaScript/JsValueSlab.cs` - Security=High, Spec=none cited, `F0AF85`, PENDING
  - Falsified if: a frame other than the innermost one is closed, or the top moves anywhere but to that frame's header
- `Broiler.VM.Profile.JavaScript.JsValueSlab.Scan(JsHandleTable)` in `src/Broiler.VM.Profile.JavaScript/JsValueSlab.cs` - Security=Critical, Spec=none cited, `3A33CD`, PENDING
  - Falsified if: a scan completes over a slab whose frame chain does not end exactly at the top, or skips a published live word of any frame
- `Broiler.VM.Profile.JavaScript.JsValueSlab.HeaderAt(int)` in `src/Broiler.VM.Profile.JavaScript/JsValueSlab.cs` - Security=High, Spec=none cited, `CC2D9B`, PENDING
  - Falsified if: a word that is not a frame header is answered as one
- `Broiler.VM.Profile.JavaScript.IJsWordRoots` in `src/Broiler.VM.Profile.JavaScript/JsValueStack.cs` - Security=Critical, Spec=none cited, `DD2034`, PENDING
  - Falsified if: an implementation's Scan passes over a published live word of some frame it holds
- `Broiler.VM.Profile.JavaScript.IJsWordRoots.Scan(JsHandleTable)` in `src/Broiler.VM.Profile.JavaScript/JsValueStack.cs` - Security=Critical, Spec=none cited, `6B1144`, PENDING
  - Falsified if: a published live word of a frame held here is not marked
- `Broiler.VM.Profile.JavaScript.JsValueStack` in `src/Broiler.VM.Profile.JavaScript/JsValueStack.cs` - Security=Critical, Spec=none cited, `671496`, PENDING
  - Falsified if: a frame spans two segments, a segment holding a live frame is dropped or reused, a frame is popped out of call order without a defect, or Scan passes over a live word of any open frame
- `Broiler.VM.Profile.JavaScript.JsValueStack.MaximumWords` in `src/Broiler.VM.Profile.JavaScript/JsValueStack.cs` - Security=High, Spec=none cited, `02C436`, PENDING
  - Falsified if: the chain's segments together advertise more words than this
- `Broiler.VM.Profile.JavaScript.JsValueStack.TryPush(int, out JsValueSlab, out int)` in `src/Broiler.VM.Profile.JavaScript/JsValueStack.cs` - Security=Critical, Spec=none cited, `D8C559`, PENDING
  - Falsified if: a frame is opened across two segments, in a segment below one holding an open frame, or past MaximumWords advertised in all
- `Broiler.VM.Profile.JavaScript.JsValueStack.Pop(JsValueSlab, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueStack.cs` - Security=Critical, Spec=none cited, `E84434`, PENDING
  - Falsified if: a frame of a segment other than the current one is closed without a defect, or the chain keeps more than one empty segment above the current one
- `Broiler.VM.Profile.JavaScript.JsValueStack.Scan(JsHandleTable)` in `src/Broiler.VM.Profile.JavaScript/JsValueStack.cs` - Security=Critical, Spec=none cited, `9B1956`, PENDING
  - Falsified if: a live word of an open frame in any segment up to the current one is not marked, or a segment above the current one is scanned
- `Broiler.VM.Profile.JavaScript.JsValueWindows` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=Critical, Spec=none cited, `3F70A3`, PENDING
  - Falsified if: an arm reads a stack slot below its read window, a suspending unit's helper leaves a slot of its stack undecoded, a word the arm wrote is left unencoded or unpublished at the step's end, a word outside the write window is overwritten, or a height is published after a safepoint that could compact
- `Broiler.VM.Profile.JavaScript.JsValueWindows.Open(JsNativeActivation, JsHandleTable)` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=Critical, Spec=none cited, `FF1B22`, PENDING
  - Falsified if: an argument word, a resident word or a slot below the activation's height is not encoded into its region before the region is published at that height
- `Broiler.VM.Profile.JavaScript.JsValueWindows.Enter(JsNativeActivation, int, out int)` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=Critical, Spec=none cited, `3D076D`, PENDING
  - Falsified if: a slot of the read window is left holding anything but its word's decoding, a word is decoded before the safepoint, the height the safepoint scans is not the plan's, or the activation's program counter and height are not the instruction's when the arm starts
- `Broiler.VM.Profile.JavaScript.JsValueWindows.Leave(JsNativeActivation, int, int, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=Critical, Spec=none cited, `65BD6E`, PENDING
  - Falsified if: a slot from the lowest one the step could write up to the height it stopped at is not encoded before that height is published, a slot below that lowest one is written, or a resident environment a PushScope opened keeps a word of an earlier record
- `Broiler.VM.Profile.JavaScript.JsValueWindows.Suspend(JsNativeActivation, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=Critical, Spec=none cited, `145CDD`, PENDING
  - Falsified if: a resident word of an environment the frame is inside is not in that environment's record when the frame suspends, or a word of an environment it is not inside is written anywhere
- `Broiler.VM.Profile.JavaScript.JsValueWindows.Returned(JsNativeActivation)` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=Critical, Spec=none cited, `F0E783`, PENDING
  - Falsified if: it decodes any word but the region's first, or decodes after the region closed
- `Broiler.VM.Profile.JavaScript.JsValueWindows.ReadDepth(byte[], int)` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=Critical, Spec=none cited, `5C2997`, PENDING
  - Falsified if: for some opcode the arm reads a slot deeper than the depth this answers
- `Broiler.VM.Profile.JavaScript.JsValueWindows.Depth(JsOpcode, uint, int)` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=Critical, Spec=none cited, `790494`, PENDING
  - Falsified if: for some opcode the arm reads a slot deeper than the depth this answers
- `Broiler.VM.Profile.JavaScript.JsValueWindows.Pops(byte[], int)` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=High, Spec=none cited, `0FA930`, PENDING
  - Falsified if: it answers other than JsOpcodes.TryDescribe's pops for the instruction and its operand
- `Broiler.VM.Profile.JavaScript.JsValueWindows.FixedPops` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=High, Spec=none cited, `FF9330`, PENDING
  - Falsified if: an entry other than minus one differs from TryDescribe's pops for some operand of that opcode
- `Broiler.VM.Profile.JavaScript.JsValueWindows.FixedReads` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=High, Spec=none cited, `63C412`, PENDING
  - Falsified if: an entry other than minus one differs from the read depth the slow path answers for some operand of that opcode
- `Broiler.VM.Profile.JavaScript.JsValueWindows.BuildFixed(bool)` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=High, Spec=none cited, `D2B3E8`, PENDING
  - Falsified if: an opcode whose count varies with its operand gets an entry other than minus one
- `Broiler.VM.Profile.JavaScript.JsValueWindows.Pops(JsOpcode, uint)` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=High, Spec=none cited, `BA50DF`, PENDING
  - Falsified if: it answers a count for an opcode TryDescribe does not describe
- `Broiler.VM.Profile.JavaScript.JsValueWindows.Operand(byte[], int, JsOpcode)` in `src/Broiler.VM.Profile.JavaScript/JsValueWindows.cs` - Security=High, Spec=none cited, `B4C746`, PENDING
  - Falsified if: for some operand shape it answers other than the verifier's decoding of the same bytes
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReadManifest(in VmArtifactDescriptor, ref VmBoundedReader, Sections)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `7CFF0B`, PENDING
  - Falsified if: an artifact naming a manifest this build does not accept is admitted, or the descriptor and the payload are allowed to name different ones
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReadBigInt(ref VmBoundedReader, Sections, out JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `07C246`, PENDING
  - Falsified if: a BigInt constant wider than the format ceiling, or spelled non-canonically, is admitted
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReadNativeCode(ref VmBoundedReader, ulong, Sections)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `61C027`, PENDING
  - Falsified if: a declared length that disagrees with the bytes present is accepted, or an architecture value this build cannot name is
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReadNativeSymbols(ref VmBoundedReader, Sections)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `F1C392`, PENDING
  - Falsified if: an offset outside the emitted blob is accepted, or two rows naming one code unit are
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReadEvalScopes(ref VmBoundedReader, JavaScriptReadAdapter, Sections)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `D1DE97`, PENDING
  - Falsified if: a row of a kind, flag or refusal this build does not define is read as one it does
- `Broiler.VM.Profile.JavaScript.JsVerifier.IsEvalCall(byte[], JsCodeUnit, uint, JavaScriptReadAdapter)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `D3432C`, PENDING
  - Falsified if: it answers true for an offset inside an instruction, past the unit, or holding anything but CallEval or CallEvalSpread
- `Broiler.VM.Profile.JavaScript.JsVerifier.LinkEvalScopes(Sections, JsCodeUnit[], JavaScriptReadAdapter, out JsEvalMap?)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `BF07DB`, PENDING
  - Falsified if: an eval scope map row that names a non-name constant, a unit, an offset or a row it may not name reaches the executor
- `Broiler.VM.Profile.JavaScript.JsVerifier.LinkScriptDeclarations(Sections, JsCodeUnit[], JavaScriptReadAdapter, out System.Collections.Generic.Dictionary<int, JsScriptDeclaration>?)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `A345C7`, PENDING
  - Falsified if: a script-declarations row that names a unit other than a script body, or a constant that is not an interned name, reaches the executor
- `Broiler.VM.Profile.JavaScript.JsVerifier.LinkScriptReferrers(Sections, JsCodeUnit[], JavaScriptReadAdapter, out System.Collections.Generic.Dictionary<int, string>?)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `806351`, PENDING
  - Falsified if: a script-referrers row that names a unit other than a script body, or a constant that is not a non-empty interned name, reaches the executor
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReachesRoot(Sections, JsEvalShape[], JsEvalSiteRow, JsCodeUnit)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `0AFF44`, PENDING
  - Falsified if: it answers true for a chain whose row at the declared depth is not the root kind the unit's flags call for
- `Broiler.VM.Profile.JavaScript.JsVerifier.LinkNative(Sections, JsCodeUnit[], IJsNativeEmitter?)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `F4A6EC`, PENDING
  - Falsified if: an artifact whose symbol table names fewer units than the function table is admitted, or a symbol offset outside the emitted blob is
- `Broiler.VM.Profile.JavaScript.JsVerifier.NativeImage(Sections, JsNativeTier)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `4CBD0B`, PENDING
  - Falsified if: the image differs from the artifact's own code, function rows, constant pool, or - for the baseline tier - exception regions in their order, or its tier differs from the one the manifest selects
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReEmit(Sections, byte[], JsNativeSymbolRow[], IJsNativeEmitter?, JsNativeProgramImage)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `5982A5`, PENDING
  - Falsified if: an artifact whose emitted bytes differ from this image's own emission of its bytecode is admitted while an emitter is present
- `Broiler.VM.Profile.JavaScript.JsVerifier.LinkModules(Sections, JsCodeUnit[], IVmVerificationContext, JavaScriptReadAdapter, out JsModuleRecord[], out JsBinding[])` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `8255D8`, PENDING
  - Falsified if: linking recurses to a depth the payload chooses, or a cyclic export resolution is answered by spending an allowance
- `Broiler.VM.Profile.JavaScript.JsVerifier.Sections.NativeValueForm` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `991AC8`, PENDING
  - Falsified if: this is true for a section whose form byte was zero, or false for one whose form byte named the value form
- `Broiler.VM.Profile.JavaScript.JsVerifier.Sections.NativeResidentBindings` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `765990`, PENDING
  - Falsified if: it differs from the residency the value-form payload's form byte states
- `Broiler.VM.Profile.JavaScript.JsVerifier.Sections.NativeValueImage` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `AD9182`, PENDING
  - Falsified if: it is set for any payload but a value-form one, or to an image other than the one that payload was scanned against
- `Broiler.VM.Profile.JavaScript.JsWordChecks.Fuzz(int, int, bool)` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=High, Spec=none cited, `628167`, PENDING
  - Falsified if: a run passes in which a live word decoded to a value other than its shadow's, a retired word decoded to a different object, or an entry survived the final compaction
- `Broiler.VM.Profile.JavaScript.JsWordChecks.EveryTagPrefixHasExactlyOneClass()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=High, Spec=none cited, `0B402D`, PENDING
  - Falsified if: this passes while some top sixteen bits classify as two classes, as none, or as a class other than JSD-0035's table gives
- `Broiler.VM.Profile.JavaScript.JsWordChecks.TheHardwareNaNsAreNumbers()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=High, Spec=none cited, `733DE9`, PENDING
  - Falsified if: this passes while a NaN the hardware computes is classified as a tagged word
- `Broiler.VM.Profile.JavaScript.JsWordChecks.EveryNaNPayloadEncodesCanonically()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=High, Spec=none cited, `16554C`, PENDING
  - Falsified if: this passes while some NaN that could reach a tag, including one whose bits carry a tag, encodes as anything but the canonical NaN, another NaN encodes as anything but itself, a word either answers reaches a tag under a sign flip or quieting, or a word decodes as a non-NaN or as other bits
- `Broiler.VM.Profile.JavaScript.JsWordChecks.EveryKindRoundTrips()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=High, Spec=none cited, `7E78F8`, PENDING
  - Falsified if: this passes while a value of some kind decodes to another kind, another reference or other Number bits
- `Broiler.VM.Profile.JavaScript.JsWordChecks.OneObjectHasOneWord()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=High, Spec=none cited, `174F1F`, PENDING
  - Falsified if: this passes while one object is answered two words, or two objects one
- `Broiler.VM.Profile.JavaScript.JsWordChecks.WordsThatNameNoValueAreRefused()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=Critical, Spec=none cited, `9A83D0`, PENDING
  - Falsified if: this passes while a header, a reserved word, an undefined special, a handle with a foreign index, generation or tag, or a released handle decodes to a value
- `Broiler.VM.Profile.JavaScript.JsWordChecks.AWrappingSlotIsRetired()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=Critical, Spec=none cited, `5234F5`, PENDING
  - Falsified if: this passes while generation zero is issued, a slot is reissued after its generation wrapped, or a word from an earlier generation of a reissued slot decodes
- `Broiler.VM.Profile.JavaScript.JsWordChecks.PermanentAndNurseryEntriesSurvive()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=Critical, Spec=none cited, `71804B`, PENDING
  - Falsified if: this passes while a permanent entry or a handle answered since the last safepoint is released, or a nursery handle survives once the safepoint has passed
- `Broiler.VM.Profile.JavaScript.JsWordChecks.AnAllocationCompactionKeepsTheNursery()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=Critical, Spec=none cited, `BC0C53`, PENDING
  - Falsified if: this passes while a compaction an allocation triggers releases a handle answered since the last safepoint
- `Broiler.VM.Profile.JavaScript.JsWordChecks.OnlyPublishedWordsAreRoots()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=Critical, Spec=none cited, `60AB8E`, PENDING
  - Falsified if: this passes while a published live word's entry is released, or a word past the published length or past the top roots its entry
- `Broiler.VM.Profile.JavaScript.JsWordChecks.HandleStressRefusesAnUnrootedWord()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=Critical, Spec=none cited, `190D9D`, PENDING
  - Falsified if: this passes while handle-stress lets a stored but unpublished word decode after its safepoint
- `Broiler.VM.Profile.JavaScript.JsWordChecks.AReleasedLiveWordStopsTheScan()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=Critical, Spec=none cited, `0B4BDC`, PENDING
  - Falsified if: this passes while a compaction completes over a live word that names a released handle
- `Broiler.VM.Profile.JavaScript.JsWordChecks.AMalformedFrameChainStopsTheScan()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=Critical, Spec=none cited, `6EF373`, PENDING
  - Falsified if: this passes while a scan completes over a frame chain with a missing header or a header stating more live words than its region
- `Broiler.VM.Profile.JavaScript.JsWordChecks.FramesCloseInOrderAndRefuseToOverflow()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=High, Spec=none cited, `286879`, PENDING
  - Falsified if: this passes while an outer frame closes over an open inner one, or a frame past the advertised capacity opens
- `Broiler.VM.Profile.JavaScript.JsWordChecks.TheValueStackOpensASegmentAndScansEveryOne()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=High, Spec=none cited, `048F07`, PENDING
  - Falsified if: this passes while a frame spans two segments, a compaction releases a handle a published word in the second segment names, or the chain does not return to its first segment
- `Broiler.VM.Profile.JavaScript.JsWordChecks.TheValueStackClosesFramesInCallOrder()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=High, Spec=none cited, `095AB5`, PENDING
  - Falsified if: this passes while an outer frame, in its own segment or an earlier one, closes over an open frame
- `Broiler.VM.Profile.JavaScript.JsWordChecks.EveryHelperWindowIsTheVerifiersCount()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=High, Spec=none cited, `3FEEF1`, PENDING
  - Falsified if: this passes while a helper's pop count differs from TryDescribe's for some instruction, or its read depth is below that count
- `Broiler.VM.Profile.JavaScript.JsWordChecks.FuzzModel` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=High, Spec=none cited, `A69F70`, PENDING
  - Falsified if: the model's shadow of a live slot differs from the value last stored there
- `Broiler.VM.Profile.JavaScript.JsWordChecks.FuzzModel.Run(int)` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=High, Spec=none cited, `E24C01`, PENDING
  - Falsified if: an operation breaks an invariant and the run still answers null
- `Broiler.VM.Profile.JavaScript.JsWordChecks.FuzzModel.CheckLive(int)` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=High, Spec=none cited, `04318B`, PENDING
  - Falsified if: a live word that does not decode to its shadow value is passed
- `Broiler.VM.Profile.JavaScript.JsWordChecks.FuzzModel.CheckDropped()` in `src/Broiler.VM.Profile.JavaScript/JsWordChecks.cs` - Security=Critical, Spec=none cited, `DCE26B`, PENDING
  - Falsified if: a word that fell out of use decodes to an object other than the one it was issued for and the check passes
- `Broiler.VM.Profile.JavaScript.JsWordCodec` in `src/Broiler.VM.Profile.JavaScript/JsWordCodec.cs` - Security=Critical, Spec=none cited, `3AF53F`, PENDING
  - Falsified if: decoding the encoding of a value answers a value of another kind, another reference, or a Number whose bits differ other than by a NaN's payload
- `Broiler.VM.Profile.JavaScript.JsWordCodec.Encode(in JsValue, JsHandleTable)` in `src/Broiler.VM.Profile.JavaScript/JsWordCodec.cs` - Security=Critical, Spec=none cited, `C0F227`, PENDING
  - Falsified if: a value of some kind is encoded under another kind's tag, or a referenced value is encoded without a handle the table holds
- `Broiler.VM.Profile.JavaScript.JsWordCodec.TryDecode(ulong, JsHandleTable, out JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsWordCodec.cs` - Security=Critical, Spec=none cited, `B6DDAF`, PENDING
  - Falsified if: this answers true for a frame header, a reserved word, an undefined special payload or a handle the table refuses, or answers a referenced value of a kind other than the word's tag
- `Broiler.VM.Profile.JavaScript.JsWordCodec.Decode(ulong, JsHandleTable)` in `src/Broiler.VM.Profile.JavaScript/JsWordCodec.cs` - Security=High, Spec=none cited, `708398`, PENDING
  - Falsified if: a word TryDecode refuses answers a value here rather than a defect
- `Broiler.VM.Profile.JavaScript.JsWordCodec.TryDecodeSpecial(ulong, out JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsWordCodec.cs` - Security=High, Spec=none cited, `D684DB`, PENDING
  - Falsified if: a payload at or past SpecialCount answers a value
- `Broiler.VM.Profile.MachineCode.VmNativePage` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Unix.cs` - Security=Critical, Spec=none cited, `BDA384`, PENDING
  - Falsified if: this half is reached on a system it was not written for
- `Broiler.VM.Profile.MachineCode.VmNativePage.ProtReadWrite` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Unix.cs` - Security=Critical, Spec=none cited, `A3E0E6`, PENDING
  - Falsified if: this value admits an execute
- `Broiler.VM.Profile.MachineCode.VmNativePage.ProtReadExecute` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Unix.cs` - Security=Critical, Spec=none cited, `4E4A17`, PENDING
  - Falsified if: a protection value naming both write and execute permission appears in any source file this component compiles, or outside the stored witness inputs rule X1's negative controls are made of
- `Broiler.VM.Profile.MachineCode.VmNativePage.MapPrivateAnonymous` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Unix.cs` - Security=Critical, Spec=none cited, `913A18`, PENDING
  - Falsified if: this names a shared or a file-backed mapping
- `Broiler.VM.Profile.MachineCode.VmNativePage.MapPrivateAnonymousBsd` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Unix.cs` - Security=Critical, Spec=none cited, `60B009`, PENDING
  - Falsified if: this names a shared or a file-backed mapping on the systems it is chosen for
- `Broiler.VM.Profile.MachineCode.VmNativePage.MapFailed` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Unix.cs` - Security=Critical, Spec=none cited, `BAFAB4`, PENDING
  - Falsified if: this differs from the value the platform answers a failed mapping with, so a failure is read as an address
- `Broiler.VM.Profile.MachineCode.VmNativePage.MapUnix(nuint)` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Unix.cs` - Security=Critical, Spec=none cited, `61768A`, PENDING
  - Falsified if: this passes any protection other than the readable-and-writable one
- `Broiler.VM.Profile.MachineCode.VmNativePage.ArmUnix(byte*, nuint)` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Unix.cs` - Security=Critical, Spec=none cited, `7C06EF`, PENDING
  - Falsified if: this passes any protection that admits a write
- `Broiler.VM.Profile.MachineCode.VmNativePage.ReleaseUnix(byte*, nuint)` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Unix.cs` - Security=Critical, Spec=none cited, `2252AF`, PENDING
  - Falsified if: this unmaps an address or a length the mapping does not own
- `Broiler.VM.Profile.MachineCode.VmNativePage.Map(void*, nuint, int, int, int, nint)` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Unix.cs` - Security=Critical, Spec=none cited, `491B84`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.MachineCode.VmNativePage.Protect(void*, nuint, int)` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Unix.cs` - Security=Critical, Spec=none cited, `70F3E8`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.MachineCode.VmNativePage.Unmap(void*, nuint)` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Unix.cs` - Security=Critical, Spec=none cited, `5B2FE6`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.MachineCode.VmNativePage` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` - Security=Critical, Spec=none cited, `BDA384`, PENDING
  - Falsified if: this half is reached on a system it was not written for
- `Broiler.VM.Profile.MachineCode.VmNativePage.MemCommitAndReserve` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` - Security=Critical, Spec=none cited, `724619`, PENDING
  - Falsified if: this names anything but reserving and committing, or it carries a protection bit
- `Broiler.VM.Profile.MachineCode.VmNativePage.MemRelease` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` - Security=Critical, Spec=none cited, `95117F`, PENDING
  - Falsified if: this names a free that leaves the reservation standing
- `Broiler.VM.Profile.MachineCode.VmNativePage.PageReadWrite` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` - Security=Critical, Spec=none cited, `9DE8C2`, PENDING
  - Falsified if: this value admits an execute
- `Broiler.VM.Profile.MachineCode.VmNativePage.PageExecuteRead` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` - Security=Critical, Spec=none cited, `056695`, PENDING
  - Falsified if: a protection value naming both write and execute permission appears in any source file this component compiles, or outside the stored witness inputs rule X1's negative controls are made of
- `Broiler.VM.Profile.MachineCode.VmNativePage.MapWindows(nuint)` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` - Security=Critical, Spec=none cited, `5C64B1`, PENDING
  - Falsified if: this passes any protection other than the readable-and-writable one
- `Broiler.VM.Profile.MachineCode.VmNativePage.ArmWindows(byte*, nuint)` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` - Security=Critical, Spec=none cited, `10433E`, PENDING
  - Falsified if: this passes any protection that admits a write
- `Broiler.VM.Profile.MachineCode.VmNativePage.ReleaseWindows(byte*)` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` - Security=Critical, Spec=none cited, `7E36F0`, PENDING
  - Falsified if: this releases an address the mapping does not own, or releases one twice
- `Broiler.VM.Profile.MachineCode.VmNativePage.VirtualAlloc(void*, nuint, uint, uint)` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` - Security=Critical, Spec=none cited, `634738`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.MachineCode.VmNativePage.VirtualProtect(void*, nuint, uint, uint*)` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` - Security=Critical, Spec=none cited, `3BA5CB`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.MachineCode.VmNativePage.VirtualFree(void*, nuint, uint)` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` - Security=Critical, Spec=none cited, `65CC8A`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.MachineCode.VmNativePage.FlushInstructionCache(void*, void*, nuint)` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` - Security=Critical, Spec=none cited, `030701`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.MachineCode.VmNativePage.GetCurrentProcess()` in `src/Broiler.VM.Profile.MachineCode/VmNativePage.Windows.cs` - Security=Critical, Spec=none cited, `FCD535`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.WebAssembly.WasmDecoder` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=Critical, Spec=none cited, `399155`, PENDING
  - Falsified if: a buffer is sized from a count that has not cleared its ceiling, or a ceiling breach is reported as a malformed artifact
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.WasmDecoder(System.ReadOnlySpan<byte>, in VmReadBounds, WasmReadAdapter, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `7A55F8`, PENDING
  - Falsified if: any field is left uninitialised so a failed decode hands back an array nothing filled
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecode(out WasmModule?, out VmVerifierOutcome)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=Critical, Spec=none cited, `E26D08`, PENDING
  - Falsified if: a module is returned while any section body did not consume exactly its declared length, or a non-custom section repeated or ran out of canonical order
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadPreamble()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `BED23F`, PENDING
  - Falsified if: a payload whose first eight bytes are not the magic and version 1 reaches the section loop
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadOneSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=Critical, Spec=none cited, `5D296A`, PENDING
  - Falsified if: the order and duplicate rules are applied after the section body is decoded rather than before
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryAdmitSectionPosition(WasmSectionId, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `DA3A9F`, PENDING
  - Falsified if: it decides order by comparing section identifiers rather than order ranks
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeTypeSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `8D16A4`, PENDING
  - Falsified if: a parameter or result vector is allocated from a count that has not cleared the declared-count ceiling
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeImportSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `14DFDD`, PENDING
  - Falsified if: a module declaring an import verifies, or the refusal is reported as a malformed artifact
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeTableSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `3A047B`, PENDING
  - Falsified if: a second table is accepted at a format version that defines only one
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeMemorySection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `BC38E0`, PENDING
  - Falsified if: a memory declaring more pages than a 32-bit address space holds is accepted
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeExportSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `54567B`, PENDING
  - Falsified if: a name that is not well formed under this format's own UTF-8 rule is accepted
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeElementSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `07BA66`, PENDING
  - Falsified if: a segment encoding form this format version does not define is decoded as though it were the classic form
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeCodeSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=Critical, Spec=none cited, `4E1120`, PENDING
  - Falsified if: a body's byte count is taken from anywhere but its declared size, or the expanded local count is not held to the declared-count ceiling
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryDecodeDataSection()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `077959`, PENDING
  - Falsified if: a segment encoding form this format version does not define is decoded as though it were the classic form
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryCheckSectionAgreement()` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `AEF1C9`, PENDING
  - Falsified if: a module whose function and code counts differ is decoded successfully
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadValueTypeVector(out WasmValueType[])` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `D547B9`, PENDING
  - Falsified if: the vector is sized before its count has cleared the declared-count ceiling
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadValueType(out WasmValueType)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `6D564B`, PENDING
  - Falsified if: an unadmitted value type and an undefined byte produce the same diagnostic code
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadLimits(out WasmLimits)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `ADE89C`, PENDING
  - Falsified if: a limit whose minimum is above its maximum is accepted
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadName(out byte[])` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `1BD6B8`, PENDING
  - Falsified if: the platform's UTF-8 decoder is consulted, or the bytes are retained before the rule has admitted them
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadConstantExpression(out WasmConstantExpression)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `C2310C`, PENDING
  - Falsified if: an expression not closed by the end opcode is accepted, or an instruction outside the constant set is decoded
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadLocals(out WasmValueType[])` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=Critical, Spec=none cited, `A839E9`, PENDING
  - Falsified if: the expanded array is sized before the accumulated total has cleared the declared-count ceiling
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryReadCount(out uint)` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `D4C69D`, PENDING
  - Falsified if: it answers true for a count that has not been both compared and charged
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryAllocateValues<TElement>(uint, out TElement[])` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=High, Spec=none cited, `3E19A9`, PENDING
  - Falsified if: an array is created on a path where the allocator refused
- `Broiler.VM.Profile.WebAssembly.WasmDecoder.TryAllocateReferences<TElement>(uint, out TElement[])` in `src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs` - Security=Critical, Spec=ADR-0007 s6, `155D19`, PENDING
  - Falsified if: the array is created before the reservation returns true, or the byte-count product is unchecked
- `Broiler.VM.Profile.WebAssembly.WasmEntryPoint` in `src/Broiler.VM.Profile.WebAssembly/WasmEntryPoint.cs` - Security=High, Spec=none cited, `A39A6F`, PENDING
  - Falsified if: two different (name, argument) pairs encode to the same text, or a float literal does not round-trip its exact bit pattern
- `Broiler.VM.Profile.WebAssembly.WasmEntryPoint.TryParse(System.ReadOnlySpan<byte>, System.Span<WasmValue>, System.Span<WasmValueType>, out int, out int, out int, out WebAssemblyEntryPointProblem)` in `src/Broiler.VM.Profile.WebAssembly/WasmEntryPoint.cs` - Security=High, Spec=none cited, `B56F30`, PENDING
  - Falsified if: a byte count is trusted past the end of the text, or a malformed literal is read as a value
- `Broiler.VM.Profile.WebAssembly.WasmEntryPoint.TryReadCount(System.ReadOnlySpan<byte>, ref int, out uint)` in `src/Broiler.VM.Profile.WebAssembly/WasmEntryPoint.cs` - Security=High, Spec=none cited, `7C643C`, PENDING
  - Falsified if: a count is accumulated past what a 32-bit length can hold
- `Broiler.VM.Profile.WebAssembly.WasmEntryPoint.TryReadLiteral(WasmValueType, System.ReadOnlySpan<byte>, out WasmValue)` in `src/Broiler.VM.Profile.WebAssembly/WasmEntryPoint.cs` - Security=High, Spec=none cited, `DEABD5`, PENDING
  - Falsified if: a float literal of the wrong digit count is accepted, or an integer literal is accepted with a trailing byte
- `Broiler.VM.Profile.WebAssembly.WasmEntryPoint.TryReadInteger(System.ReadOnlySpan<byte>, out ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmEntryPoint.cs` - Security=High, Spec=none cited, `247F89`, PENDING
  - Falsified if: a literal with no digits or with a trailing non-digit is accepted
- `Broiler.VM.Profile.WebAssembly.WasmFormat` in `src/Broiler.VM.Profile.WebAssembly/WasmFormat.cs` - Security=High, Spec=none cited, `ABAE0A`, PENDING
  - Falsified if: section order is decided anywhere in this assembly by comparing identifier values rather than by this table
- `Broiler.VM.Profile.WebAssembly.WasmFormat.OrderRankOf(WasmSectionId)` in `src/Broiler.VM.Profile.WebAssembly/WasmFormat.cs` - Security=High, Spec=none cited, `0C2C57`, PENDING
  - Falsified if: the tag section does not rank between memory and global, or the data count section does not rank before code
- `Broiler.VM.Profile.WebAssembly.WasmLabel` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, Spec=none cited, `A993B6`, PENDING
  - Falsified if: a branch scans the body for a matching end rather than reading a target from here
- `Broiler.VM.Profile.WebAssembly.WasmFrame` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, Spec=none cited, `7EAD83`, PENDING
  - Falsified if: guest call depth grows the CLR stack, or a frame outlives the run that made it
- `Broiler.VM.Profile.WebAssembly.WasmPacing` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, Spec=none cited, `A58F73`, PENDING
  - Falsified if: fuel charged between two polls can exceed the declared uncharged-work bound
- `Broiler.VM.Profile.WebAssembly.WasmPacing.TryReserve(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, Spec=none cited, `624E86`, PENDING
  - Falsified if: it returns true while the bound could still be crossed by the charge it was asked about
- `Broiler.VM.Profile.WebAssembly.WasmPacing.TryCharge(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, Spec=none cited, `4DB319`, PENDING
  - Falsified if: a charge is committed without the poll that its size demanded
- `Broiler.VM.Profile.WebAssembly.WasmPacing.Observe(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, Spec=none cited, `6ABDC7`, PENDING
  - Falsified if: a charge made elsewhere never reaches this counter
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, Spec=none cited, `03489B`, PENDING
  - Falsified if: any input makes a member here throw, or a trap leaves as anything but a return code, or an operand is read past the height validation computed
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.Call(int, System.ReadOnlySpan<WasmValue>, out int)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, Spec=none cited, `9994B7`, PENDING
  - Falsified if: it answers completed while the operand stack does not hold exactly the declared results
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.TryPushFrame(int, out WasmRunStatus)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, Spec=none cited, `CBF744`, PENDING
  - Falsified if: a frame is pushed without a call-depth charge taking, or arguments are read below the caller's own base
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.PopFrame(WasmFrame)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, Spec=none cited, `99DB03`, PENDING
  - Falsified if: it leaves anything on the stack other than exactly the declared results
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.Run()` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, Spec=none cited, `105A25`, PENDING
  - Falsified if: an opcode validation admits is not handled here, or a handled opcode reads a different immediate shape than validation read
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.Branch(WasmFrame, int, ref int)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, Spec=none cited, `A49C19`, PENDING
  - Falsified if: a branch to a loop closes the loop's own label, or a branch leaves values below the label's height
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.MemoryAccess(byte, System.ReadOnlySpan<byte>, ref int)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, Spec=none cited, `666BB8`, PENDING
  - Falsified if: an effective address is computed in 32-bit arithmetic, or a width here disagrees with the one validation typed
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.ReadU32(System.ReadOnlySpan<byte>, ref int)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, Spec=none cited, `BE279D`, PENDING
  - Falsified if: it reads past the end of the body, or shifts a payload byte past the width
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.ReadS64(System.ReadOnlySpan<byte>, ref int)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, Spec=none cited, `FA830B`, PENDING
  - Falsified if: it reads past the end of the body, or sign-extends from the wrong bit
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.TryNumeric(byte, out WasmTrapKind?)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, Spec=none cited, `0123F7`, PENDING
  - Falsified if: an opcode inside these ranges answers false, or one outside them answers true
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.Integer(byte, ref WasmTrapKind?)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, Spec=none cited, `23CF43`, PENDING
  - Falsified if: a signed remainder by minus one traps, or a shift count is used unmasked
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.Float(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, Spec=none cited, `A93738`, PENDING
  - Falsified if: a float operation here raises a trap, or a minimum of a negative and a positive zero answers the positive one
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.FloatComparison(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=High, Spec=none cited, `9BCC47`, PENDING
  - Falsified if: a comparison against a NaN answers anything but false, or an inequality against a NaN answers false
- `Broiler.VM.Profile.WebAssembly.WasmInterpreter.Convert(byte, ref WasmTrapKind?)` in `src/Broiler.VM.Profile.WebAssembly/WasmInterpreter.cs` - Security=Critical, Spec=none cited, `A0EBA3`, PENDING
  - Falsified if: a value exactly on a bound is refused, or a value one unit past it is accepted
- `Broiler.VM.Profile.WebAssembly.WasmLeb128` in `src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs` - Security=High, Spec=none cited, `839507`, PENDING
  - Falsified if: any member here calls one of the core's canonical variable-length readers, or a padded encoding inside its byte budget is refused
- `Broiler.VM.Profile.WebAssembly.WasmLeb128.TryReadVarU32(ref VmBoundedReader, out uint, out WasmVarIntStatus)` in `src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs` - Security=High, Spec=none cited, `D83660`, PENDING
  - Falsified if: a five-byte encoding whose terminal byte is zero is refused, or a terminal byte above 0x0F is accepted
- `Broiler.VM.Profile.WebAssembly.WasmLeb128.TryReadVarS32(ref VmBoundedReader, out int, out WasmVarIntStatus)` in `src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs` - Security=High, Spec=none cited, `8D4328`, PENDING
  - Falsified if: a terminal byte that is not a sign extension of the value's top bit is accepted
- `Broiler.VM.Profile.WebAssembly.WasmLeb128.TryReadBoundedCount(ref VmBoundedReader, WasmReadAdapter, out uint, out WasmVarIntStatus)` in `src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs` - Security=Critical, Spec=ADR-0007 s6, `A89DDB`, PENDING
  - Falsified if: a count is returned before both the ceiling comparison and the charge have succeeded
- `Broiler.VM.Profile.WebAssembly.WasmLeb128.TryReadUnsigned(ref VmBoundedReader, int, int, out ulong, out WasmVarIntStatus)` in `src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs` - Security=Critical, Spec=none cited, `650D29`, PENDING
  - Falsified if: two encodings inside the budget decode to different values, or the shift reaches 64 and wraps
- `Broiler.VM.Profile.WebAssembly.WasmLeb128.TryReadSigned(ref VmBoundedReader, int, int, out long, out WasmVarIntStatus)` in `src/Broiler.VM.Profile.WebAssembly/WasmLeb128.cs` - Security=Critical, Spec=none cited, `31C498`, PENDING
  - Falsified if: a sign extension that disagrees with the value's top bit is accepted, or a short encoding is not sign-extended
- `Broiler.VM.Profile.WebAssembly.WasmMemoryInstance` in `src/Broiler.VM.Profile.WebAssembly/WasmMemory.cs` - Security=Critical, Spec=none cited, `985077`, PENDING
  - Falsified if: an access reaches a byte outside the current size, or a growth refused by the profile maximum spends a core allowance, or a growth allocates before its charge is taken
- `Broiler.VM.Profile.WebAssembly.WasmMemoryInstance.ProfileMaximumPages` in `src/Broiler.VM.Profile.WebAssembly/WasmMemory.cs` - Security=High, Spec=none cited, `F9C3B1`, PENDING
  - Falsified if: this bound is derived from a limit vector rather than declared here, or refusing against it charges anything
- `Broiler.VM.Profile.WebAssembly.WasmMemoryInstance.TryLoad(ulong, int, out ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmMemory.cs` - Security=Critical, Spec=none cited, `8389FD`, PENDING
  - Falsified if: it reads a byte at or past the current size, or the range check is performed in 32-bit arithmetic
- `Broiler.VM.Profile.WebAssembly.WasmMemoryInstance.TryStore(ulong, int, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmMemory.cs` - Security=Critical, Spec=none cited, `A3FB78`, PENDING
  - Falsified if: it writes a byte at or past the current size, or the range check is performed in 32-bit arithmetic
- `Broiler.VM.Profile.WebAssembly.WasmMemoryInstance.TryInitialise(ulong, System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Profile.WebAssembly/WasmMemory.cs` - Security=High, Spec=none cited, `FA5507`, PENDING
  - Falsified if: a prefix of a segment that does not fit is written, or a segment that does fit is refused
- `Broiler.VM.Profile.WebAssembly.WasmMemoryInstance.Grow(uint, IVmMeter)` in `src/Broiler.VM.Profile.WebAssembly/WasmMemory.cs` - Security=Critical, Spec=none cited, `859A66`, PENDING
  - Falsified if: an array is allocated before the allocation charge returns true, or a refusal against the profile ceiling reaches the meter
- `Broiler.VM.Profile.WebAssembly.WasmJumpTarget` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=High, Spec=none cited, `4D9BAD`, PENDING
  - Falsified if: an offset here is assigned from a value read out of the payload rather than from a position validation reached
- `Broiler.VM.Profile.WebAssembly.WasmFunctionBody` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=Critical, Spec=none cited, `7D49AB`, PENDING
  - Falsified if: either bound is assigned from a number the payload carried, or a body is sealed twice, or a buffer is sized from a bound while it reads the sentinel
- `Broiler.VM.Profile.WebAssembly.WasmFunctionBody.TrySeal(int, int, WasmJumpTarget[])` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=Critical, Spec=none cited, `0C343E`, PENDING
  - Falsified if: a second call changes any field, or a caller reaches these fields by any other route
- `Broiler.VM.Profile.WebAssembly.WasmFunctionBody.TryFindJumpTarget(int, out WasmJumpTarget)` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=High, Spec=none cited, `3EE527`, PENDING
  - Falsified if: the table is searched as though sorted while validation appends out of offset order
- `Broiler.VM.Profile.WebAssembly.WasmModule` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=Critical, Spec=none cited, `25C6A2`, PENDING
  - Falsified if: anything reachable from this state can be mutated after verification returns, or a caller reads one of these fields as though a validation pass had run
- `Broiler.VM.Profile.WebAssembly.WasmModule.ExecutionBoundsComputed` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=High, Spec=none cited, `A79497`, PENDING
  - Falsified if: it reports true while any body still carries the not-computed sentinel
- `Broiler.VM.Profile.WebAssembly.WasmModule.TryCopyExportName(int, System.Span<byte>, out int)` in `src/Broiler.VM.Profile.WebAssembly/WasmModule.cs` - Security=High, Spec=none cited, `82B42B`, PENDING
  - Falsified if: it writes past the destination, or hands back a view onto the array the module keeps
- `Broiler.VM.Profile.WebAssembly.WasmName` in `src/Broiler.VM.Profile.WebAssembly/WasmName.cs` - Security=High, Spec=none cited, `88A0F6`, PENDING
  - Falsified if: an overlong form, a surrogate code point, or a value above U+10FFFF is reported well formed
- `Broiler.VM.Profile.WebAssembly.WasmName.IsWellFormed(System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Profile.WebAssembly/WasmName.cs` - Security=High, Spec=none cited, `683B7D`, PENDING
  - Falsified if: a sequence this method accepts decodes to a different scalar value than the specification assigns it
- `Broiler.VM.Profile.WebAssembly.WasmName.Follows(System.ReadOnlySpan<byte>, int, int, byte, byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmName.cs` - Security=High, Spec=none cited, `240E15`, PENDING
  - Falsified if: it reads past the end of the span, or reports true for a sequence that runs off the end of the name
- `Broiler.VM.Profile.WebAssembly.WasmReadAdapter` in `src/Broiler.VM.Profile.WebAssembly/WasmReadAdapter.cs` - Security=High, Spec=none cited, `7779B6`, PENDING
  - Falsified if: a charge made through this adapter reaches a dimension other than the one named, or a released byte count is charged rather than released
- `Broiler.VM.Profile.WebAssembly.WasmReadAdapter.Ceilings` in `src/Broiler.VM.Profile.WebAssembly/WasmReadAdapter.cs` - Security=High, Spec=none cited, `1BB2CD`, PENDING
  - Falsified if: a reader is built from ceilings other than these, so two passes over one artifact run under different bounds
- `Broiler.VM.Profile.WebAssembly.WasmReadAdapter.WasmReadAdapter(IVmMeter, VmLimitVector)` in `src/Broiler.VM.Profile.WebAssembly/WasmReadAdapter.cs` - Security=High, Spec=none cited, `035C43`, PENDING
  - Falsified if: a payload byte is read before this constructor has run
- `Broiler.VM.Profile.WebAssembly.WasmReadAdapter.TryChargeDeclaredCount(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmReadAdapter.cs` - Security=High, Spec=none cited, `1A2D6D`, PENDING
  - Falsified if: a declared count is charged anywhere but through this member, or its refusal is reported as a malformed artifact
- `Broiler.VM.Profile.WebAssembly.WasmReadAdapter.TryChargeStructuralDepth(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmReadAdapter.cs` - Security=High, Spec=none cited, `1D9970`, PENDING
  - Falsified if: a level charged here is not released when it closes, so nesting is counted as a running total
- `Broiler.VM.Profile.WebAssembly.WasmReadAdapter.ReleaseStructuralDepth(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmReadAdapter.cs` - Security=High, Spec=none cited, `0E44B7`, PENDING
  - Falsified if: it releases more levels than were charged, so a ceiling admits a nesting it should refuse
- `Broiler.VM.Profile.WebAssembly.WasmTableInstance` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, Spec=none cited, `6C840E`, PENDING
  - Falsified if: a slot is read without its index being compared against the entry count first
- `Broiler.VM.Profile.WebAssembly.WasmTableInstance.TryRead(uint, out int)` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, Spec=none cited, `895FB1`, PENDING
  - Falsified if: an index at or past the entry count is read rather than refused
- `Broiler.VM.Profile.WebAssembly.WasmTableInstance.TryInitialise(ulong, System.ReadOnlySpan<uint>)` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, Spec=none cited, `C8FAE1`, PENDING
  - Falsified if: a prefix of a segment that does not fit is written
- `Broiler.VM.Profile.WebAssembly.WasmStore` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, Spec=none cited, `F2E105`, PENDING
  - Falsified if: anything here becomes reachable from an artifact's state once verification has returned
- `Broiler.VM.Profile.WebAssembly.WasmStore.TryAllocate(WasmModule, IVmMeter, out WasmStore?, out bool)` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=Critical, Spec=none cited, `9C0CD5`, PENDING
  - Falsified if: an array is allocated before its charge returns true, or a declared minimum above the profile ceiling is allocated
- `Broiler.VM.Profile.WebAssembly.WasmStore.ReleasePartial(WasmMemoryInstance?[], WasmTableInstance?[], IVmMeter)` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, Spec=none cited, `EC74DD`, PENDING
  - Falsified if: a refused allocation leaves any byte reported retained and never released
- `Broiler.VM.Profile.WebAssembly.WasmStore.ProfileMaximumTableEntries` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, Spec=none cited, `0F97A2`, PENDING
  - Falsified if: this bound is derived from a limit vector rather than declared here
- `Broiler.VM.Profile.WebAssembly.WasmStore.Release(IVmMeter)` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, Spec=none cited, `8C390E`, PENDING
  - Falsified if: any guest instruction is dispatched on this path
- `Broiler.VM.Profile.WebAssembly.WasmInstance` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, Spec=none cited, `95661E`, PENDING
  - Falsified if: an instance is published while any segment or the start function refused
- `Broiler.VM.Profile.WebAssembly.WasmContinuation` in `src/Broiler.VM.Profile.WebAssembly/WasmStore.cs` - Security=High, Spec=none cited, `56FA69`, PENDING
  - Falsified if: any path in this assembly constructs one of these and hands it to the core
- `Broiler.VM.Profile.WebAssembly.WasmTrapKind` in `src/Broiler.VM.Profile.WebAssembly/WasmTrapKind.cs` - Security=High, Spec=none cited, `FD59E7`, PENDING
  - Falsified if: a member here is raised by throwing, or a resource exhaustion is reported as one of these
- `Broiler.VM.Profile.WebAssembly.WasmFuncType` in `src/Broiler.VM.Profile.WebAssembly/WasmTypes.cs` - Security=High, Spec=none cited, `33E22E`, PENDING
  - Falsified if: either array reaches a caller, so a state two runtimes share can be mutated through it
- `Broiler.VM.Profile.WebAssembly.WasmControlFrame` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `E68A6D`, PENDING
  - Falsified if: a frame's recorded height is taken after its start types were pushed rather than before
- `Broiler.VM.Profile.WebAssembly.WasmValidator` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `566927`, PENDING
  - Falsified if: a pop at a frame's own height answers the bottom type while the frame is not unreachable, or a module reaches an interpreter with an index, an opcode, a block or an operand stack this pass did not check
- `Broiler.VM.Profile.WebAssembly.WasmValidator.BottomType` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `B6F1EE`, PENDING
  - Falsified if: the binary format assigns this byte to a value type, so a payload can name the bottom type
- `Broiler.VM.Profile.WebAssembly.WasmValidator.InstructionsBetweenPolls` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `154EC0`, PENDING
  - Falsified if: the work accumulated between two polls can exceed the uncharged-work bound the descriptor declares
- `Broiler.VM.Profile.WebAssembly.WasmValidator.WasmValidator(WasmModule, WasmReadAdapter, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `E88F05`, PENDING
  - Falsified if: any field is left uninitialised, so a failed validation reads a buffer nothing filled
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidate(out VmVerifierOutcome)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `E081FE`, PENDING
  - Falsified if: a scratch reservation or a charged level of nesting survives this call on any path
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateModule()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `210294`, PENDING
  - Falsified if: a function body is walked before the type indices it reads have been held to the type space
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateFunctionTypes()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `52A978`, PENDING
  - Falsified if: a function whose type index addresses no type reaches the body walk
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateStartFunction()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `3E5D87`, PENDING
  - Falsified if: a start function that takes a parameter or returns a result is accepted
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateExports()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `18FD2F`, PENDING
  - Falsified if: two exports publishing one name are accepted, or the check is quadratic in a guest-chosen count
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateGlobals()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `4F956A`, PENDING
  - Falsified if: an initializer whose type differs from the global it initialises is accepted
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateElementSegments()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `4ECC1D`, PENDING
  - Falsified if: a segment writing a function index that addresses no function is accepted
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateDataSegments()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `55202D`, PENDING
  - Falsified if: a segment naming a memory the module does not declare is accepted
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryConstantExpressionType(WasmConstantExpression, out WasmValueType)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `C58248`, PENDING
  - Falsified if: a constant expression reading a module's own global is admitted
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryValidateBody(int)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `074C19`, PENDING
  - Falsified if: a body is sealed while the control stack is not empty, or bytes past the closing end are accepted
- `Broiler.VM.Profile.WebAssembly.WasmValidator.OpenReaderOver(System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `0947DE`, PENDING
  - Falsified if: the reader it builds outlives the ceilings value it was built from
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TrySealBody(WasmFunctionBody)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `29B0E7`, PENDING
  - Falsified if: a body is handed a jump table longer than the entries this walk resolved
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryStep()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `B028D1`, PENDING
  - Falsified if: an opcode byte reaches a typing rule that is not the one the specification gives it
- `Broiler.VM.Profile.WebAssembly.WasmValidator.RefuseOpcode(byte, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `EFA3C4`, PENDING
  - Falsified if: an unadmitted instruction and a byte naming no instruction produce the same diagnostic code
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryOpenBlock(byte, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `89CB8B`, PENDING
  - Falsified if: a loop's label offset is anywhere but the first instruction of its own body
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryElse(ulong)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `DDE1A2`, PENDING
  - Falsified if: an else where no if is open is accepted, or the if's else offset is not the first byte of this arm
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryEnd()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `005AC3`, PENDING
  - Falsified if: a conditional with a result and no alternative arm is accepted, or an end resolves an entry it did not close
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryBranch(bool)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `398466`, PENDING
  - Falsified if: an unconditional branch does not make the rest of its frame unreachable, or a conditional one does
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryBranchTable()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `A59028`, PENDING
  - Falsified if: two labels of one table are admitted with different arities, or the label vector is read before its count cleared its ceiling
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryReturn()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `4E2268`, PENDING
  - Falsified if: it pops anything but the function's own result types
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryCall()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `25372F`, PENDING
  - Falsified if: a call whose index addresses no function is accepted
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryCallIndirect()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `35A3F9`, PENDING
  - Falsified if: an indirect call is admitted in a module that declares no table
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryLocal(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `46392A`, PENDING
  - Falsified if: an index past the parameters and locals together is accepted
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryGlobal(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `55CCEC`, PENDING
  - Falsified if: a write to a global the module declared immutable is accepted
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryMemoryQuery(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `378D70`, PENDING
  - Falsified if: either is admitted in a module that declares no memory
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryMemoryAccess(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `3B8775`, PENDING
  - Falsified if: an alignment above the natural alignment of the access reaches an interpreter, or a store pops its address before its value
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryNumeric(byte)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `632427`, PENDING
  - Falsified if: an opcode inside one of these runs is given a signature the specification does not give it
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryNumericSignature(byte, out WasmValueType, out int, out WasmValueType)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `1801B1`, PENDING
  - Falsified if: a run's boundary here differs from the specification's own opcode table
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TrySelect()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `7A8CBE`, PENDING
  - Falsified if: the two candidates are compared as concrete types inside unreachable code
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PushOperand(WasmValueType)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `5EFE15`, PENDING
  - Falsified if: the recorded maximum is below a height the stack actually reached
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PopOperand(WebAssemblyDiagnosticCode, out WasmValueType)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `6991A4`, PENDING
  - Falsified if: a pop at the frame's own height answers the bottom type while the frame is reachable, or answers an error while it is not
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PopOperand(WasmValueType, WebAssemblyDiagnosticCode, WebAssemblyDiagnosticCode, out WasmValueType)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `886224`, PENDING
  - Falsified if: a concrete type is admitted where a different concrete type was expected
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PopOperands(WasmTypeVector)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `7A7064`, PENDING
  - Falsified if: the run is popped in declaration order rather than in reverse
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PopOperandSpan(System.ReadOnlySpan<WasmValueType>)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `410518`, PENDING
  - Falsified if: the run is popped in declaration order rather than in reverse
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PopThenPush(WasmTypeVector)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `199BCD`, PENDING
  - Falsified if: it pushes back the expected types rather than the ones the pops answered
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PushControl(byte, WasmTypeVector, WasmTypeVector, int, int)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `22C66A`, PENDING
  - Falsified if: the frame's height is recorded after its start types were pushed, or a level is opened without being charged
- `Broiler.VM.Profile.WebAssembly.WasmValidator.PopControl(out WasmControlFrame)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `9EA35D`, PENDING
  - Falsified if: a frame closes while operands remain above the height it was entered at, or a closed level is not released
- `Broiler.VM.Profile.WebAssembly.WasmValidator.LabelTypes(in WasmControlFrame)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `69C68C`, PENDING
  - Falsified if: a loop's label types are its end types, or any other frame's are its start types
- `Broiler.VM.Profile.WebAssembly.WasmValidator.MarkUnreachable()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=none cited, `C0EE44`, PENDING
  - Falsified if: it truncates to anything but the frame's own recorded height
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryReadBlockType(out WasmTypeVector)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `308BB4`, PENDING
  - Falsified if: a padded encoding of a one-byte block type is refused, or a type index is decoded as a value type
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryReadReservedZero()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `B0F2F9`, PENDING
  - Falsified if: a non-zero reserved byte is accepted, so a later revision's meaning is decoded as this one's
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryReadCount(out uint)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `2F7DBE`, PENDING
  - Falsified if: it answers true for a count that has not been both compared and charged
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryReserveScratch()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `D26998`, PENDING
  - Falsified if: a buffer is sized from a count that has not cleared the declared-count ceiling
- `Broiler.VM.Profile.WebAssembly.WasmValidator.ReleaseScratch()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `1AEC57`, PENDING
  - Falsified if: a reservation this pass took survives it, or more levels are released than were charged
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryGrow<TElement>(ref TElement[], ref ulong, int, ulong, VmBudgetDimension)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=Critical, Spec=ADR-0007 s6, `A11BA9`, PENDING
  - Falsified if: a buffer grows past the ceiling named for it, or a reservation is released before its replacement is taken
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryAppendJump(int, int, int, int, out int)` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=none cited, `C3C9D8`, PENDING
  - Falsified if: an entry is appended at an offset below the one before it, so the table is not sorted
- `Broiler.VM.Profile.WebAssembly.WasmValidator.TryPoll()` in `src/Broiler.VM.Profile.WebAssembly/WasmValidator.cs` - Security=High, Spec=ADR-0007, `BB7926`, PENDING
  - Falsified if: the work accumulated between two calls of this member can exceed the uncharged-work bound the descriptor declares
- `Broiler.VM.Profile.WebAssembly.WasmValue` in `src/Broiler.VM.Profile.WebAssembly/WasmValue.cs` - Security=High, Spec=none cited, `EFC50D`, PENDING
  - Falsified if: a slot here carries a type tag, or an interpreter path reads the payload as a type validation did not prove it to be
- `Broiler.VM.Profile.WebAssembly.WasmValue.FromF32(float)` in `src/Broiler.VM.Profile.WebAssembly/WasmValue.cs` - Security=High, Spec=none cited, `44A97E`, PENDING
  - Falsified if: a stored NaN reads back with a different payload than it was written with
- `Broiler.VM.Profile.WebAssembly.WasmValue.FromF64(double)` in `src/Broiler.VM.Profile.WebAssembly/WasmValue.cs` - Security=High, Spec=none cited, `5FFF79`, PENDING
  - Falsified if: a stored NaN reads back with a different payload than it was written with
- `Broiler.VM.Profile.WebAssembly.WasmRefusal` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyDiagnostics.cs` - Security=High, Spec=ADR-0005, `686934`, PENDING
  - Falsified if: a ceiling breach is mapped onto an invalid artifact, or a framing failure onto a resource exhaustion
- `Broiler.VM.Profile.WebAssembly.WasmRefusal.FromReader(VmBoundedReadStatus, VmSourcePosition)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyDiagnostics.cs` - Security=High, Spec=ADR-0005, `A30C39`, PENDING
  - Falsified if: a ceiling breach is mapped onto an invalid artifact, or a framing failure onto a resource exhaustion
- `Broiler.VM.Profile.WebAssembly.WasmRefusal.FromVarInt(WasmVarIntStatus, VmBoundedReadStatus, VmSourcePosition)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyDiagnostics.cs` - Security=High, Spec=none cited, `F268C0`, PENDING
  - Falsified if: two of the three integer malformations report the same diagnostic code
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=Critical, Spec=none cited, `ED8401`, PENDING
  - Falsified if: any input makes a member of this type throw, or produces an answer that is not one of the five execution-step kinds
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor.Instantiate(VmVerifiedArtifact, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=High, Spec=none cited, `B6295D`, PENDING
  - Falsified if: an instance is published after a segment or the start function refused, or an exception escapes this member
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor.Invoke(IVmInstanceState, in VmInvocationRequest, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=High, Spec=none cited, `196678`, PENDING
  - Falsified if: an exception escapes this member, or a trap leaves as anything but a typed payload
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor.Unwind(IVmProfileContinuation, ulong)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=High, Spec=none cited, `8FAAB7`, PENDING
  - Falsified if: any guest instruction is dispatched on this path
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor.InstantiateCore(VmVerifiedArtifact, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=Critical, Spec=none cited, `9A4768`, PENDING
  - Falsified if: a store outlives a refused instantiation without being released
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor.InvokeCore(IVmInstanceState, in VmInvocationRequest)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=Critical, Spec=none cited, `41C410`, PENDING
  - Falsified if: an entry point resolves to a function whose parameters the arguments do not match
- `Broiler.VM.Profile.WebAssembly.WebAssemblyExecutor.Faulted(WasmTrapKind, int, int)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyExecutor.cs` - Security=High, Spec=none cited, `98D997`, PENDING
  - Falsified if: a trap kind reaches a caller without the diagnostic code its registry row names
- `Broiler.VM.Profile.WebAssembly.WebAssemblyValue` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyPayloads.cs` - Security=High, Spec=none cited, `378F47`, PENDING
  - Falsified if: a value crossing this type is quieted, rounded or re-encoded
- `Broiler.VM.Profile.WebAssembly.WebAssemblyTrap` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyPayloads.cs` - Security=High, Spec=none cited, `211660`, PENDING
  - Falsified if: a trap reaches a caller as a CLR exception rather than as one of these
- `Broiler.VM.Profile.WebAssembly.WebAssemblyProfile.MaxUnchargedWork` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyProfile.cs` - Security=High, Spec=none cited, `D58353`, PENDING
  - Falsified if: the descriptor's uncharged-work row and this constant disagree
- `Broiler.VM.Profile.WebAssembly.WebAssemblyProfile.Build()` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyProfile.cs` - Security=High, Spec=none cited, `FDF7A7`, PENDING
  - Falsified if: a row here states a capability, a guest load, a manifest or a format version this assembly does not implement without the surrounding text saying so
- `Broiler.VM.Profile.WebAssembly.WebAssemblyProfile.Defaults()` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyProfile.cs` - Security=High, Spec=none cited, `00D0CA`, PENDING
  - Falsified if: a default here is zero on a dimension this profile declares inapplicable, or any default exceeds its maximum
- `Broiler.VM.Profile.WebAssembly.WebAssemblyProfile.Matrix()` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyProfile.cs` - Security=High, Spec=none cited, `18E0B0`, PENDING
  - Falsified if: a row says charged for a dimension no code path in this assembly charges or polls against, or a dimension this assembly charges is declared inapplicable
- `Broiler.VM.Profile.WebAssembly.WebAssemblyVerifier` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyVerifier.cs` - Security=Critical, Spec=none cited, `9EEB68`, PENDING
  - Falsified if: any input makes a member of this type throw, or a state it hands back is treated as though a validation pass had run over it
- `Broiler.VM.Profile.WebAssembly.WebAssemblyVerifier.PollGranularity` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyVerifier.cs` - Security=High, Spec=none cited, `A489E3`, PENDING
  - Falsified if: it exceeds the uncharged-work bound this profile's descriptor declares
- `Broiler.VM.Profile.WebAssembly.WebAssemblyVerifier.Verify(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, IVmVerificationContext, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyVerifier.cs` - Security=Critical, Spec=none cited, `2B99FE`, PENDING
  - Falsified if: a payload byte is read on a path that answers UnsupportedProfile, or an exception escapes this method
- `Broiler.VM.Profile.WebAssembly.WebAssemblyVerifier.VerifyCore(in VmArtifactDescriptor, System.ReadOnlySpan<byte>, IVmVerificationContext, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.WebAssembly/WebAssemblyVerifier.cs` - Security=Critical, Spec=none cited, `20FD2C`, PENDING
  - Falsified if: a payload byte is read before the effective ceilings have been projected into the reader's bounds
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

That is not a figure of speech. 5073 of the 5190 assessed units declare
`Origin=AI`, and the records this component implements were drafted the same way. An
adversarial pass over the work confirmed findings and they were corrected, which is a check
on it and not an independent judgement of it. Reading a declaration is the only thing that
makes it read.
