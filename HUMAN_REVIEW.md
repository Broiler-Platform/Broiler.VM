# Human Review: Broiler.VM

GENERATED - DO NOT EDIT MANUALLY. Regenerate with
`BROILER_ASSURANCE_WRITE=1 dotnet test Broiler.VM.slnx -c Release`, which rewrites this file,
`CODE-ASSURANCE.md`, `assurance.manifest.json` and every generated source header from the
product tree.

> **Status: PENDING.** Human-reviewed: 0 of 3375 relevant units. No package
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
| Files scanned | 160 |
| Code units | 6159 |
| Relevant | 3375 |
| Exempt | 2784 |
| Assessed | 3375 of 3375 (100%) |
| Human reviewed | 0 of 3375 (0%) |
| Unverified | 3375 |
| Aliases naming a decision | 0 |

## 4. Review States

One row per state of the machine that reads the two lines. The states are computed from the
annotations and the current fingerprints; nothing stores them.

| State | Units |
|---|---:|
| NEW | 0 |
| AI_ASSESSED | 0 |
| HUMAN_PENDING | 3375 |
| HUMAN_APPROVED_PENDING_FINGERPRINT | 0 |
| VERIFIED | 0 |
| STALE | 0 |
| EXEMPT | 2784 |

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
| `src/Broiler.VM.Profile.JavaScript.Compiler/CompilationStack.cs` | 3 | 3 | 0 | 3 | Low | High | 2/2 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Assembler.cs` | 54 | 45 | 9 | 45 | Low | High | 14/14 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` | 42 | 37 | 5 | 37 | Low | High | 14/14 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Walk.cs` | 31 | 18 | 13 | 18 | Low | High | 4/4 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` | 279 | 188 | 91 | 188 | None | High | 12/11 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsNativeBackend.cs` | 19 | 12 | 7 | 12 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` | 14 | 12 | 2 | 12 | None | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` | 169 | 149 | 20 | 149 | None | High | 4/4 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsSyntax.cs` | 94 | 81 | 13 | 81 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Abi.cs` | 9 | 7 | 2 | 7 | Low | High | 6/6 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Assembler.cs` | 79 | 53 | 26 | 53 | Low | High | 13/13 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` | 34 | 29 | 5 | 29 | Low | High | 17/17 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Walk.cs` | 28 | 16 | 12 | 16 | Low | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructCensus.cs` | 7 | 7 | 0 | 7 | None | High | 3/3 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceConstructs.cs` | 61 | 6 | 55 | 6 | None | High | 4/4 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceControlFlow.cs` | 8 | 8 | 0 | 8 | None | Medium | 4/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceLowering.cs` | 22 | 22 | 0 | 22 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParseOptions.cs` | 15 | 10 | 5 | 10 | None | High | 4/3 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` | 70 | 63 | 7 | 63 | None | High | 21/21 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceProgramBuilder.cs` | 35 | 24 | 11 | 24 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs` | 34 | 25 | 9 | 25 | None | High | 18/18 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceDiagnostics.cs` | 32 | 3 | 29 | 3 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourcePrograms.cs` | 11 | 10 | 1 | 10 | None | High | 6/4 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs` | 40 | 24 | 16 | 24 | None | High | 14/14 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceSyntax.cs` | 29 | 26 | 3 | 26 | None | Medium | 1/0 |
| `src/Broiler.VM.Profile.JavaScript.Compiler/SliceTokenizer.cs` | 140 | 40 | 100 | 40 | None | High | 22/18 |
| `src/Broiler.VM.Profile.JavaScript.Format/AssemblyMarker.cs` | 1 | 1 | 0 | 1 | None | None | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptArtifactWriter.cs` | 20 | 17 | 3 | 17 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptFormat.cs` | 26 | 14 | 12 | 14 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JavaScriptOpcode.cs` | 40 | 10 | 30 | 10 | None | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsArtifactWriter.cs` | 32 | 32 | 0 | 32 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsFormat.cs` | 64 | 31 | 33 | 31 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` | 26 | 17 | 9 | 17 | Low | High | 4/4 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsNativeFrame.cs` | 17 | 3 | 14 | 3 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsNumericManifest.cs` | 5 | 5 | 0 | 5 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsOpcode.cs` | 150 | 23 | 127 | 23 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsRegExpMatcher.cs` | 213 | 120 | 93 | 120 | Medium | Medium | 1/0 |
| `src/Broiler.VM.Profile.JavaScript.Format/JsSurfaces.cs` | 10 | 10 | 0 | 10 | None | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/AssemblyMarker.cs` | 1 | 1 | 0 | 1 | None | None | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptDiagnostics.cs` | 74 | 8 | 66 | 8 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` | 35 | 18 | 17 | 18 | Low | High | 6/6 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptLanguageEdition.cs` | 13 | 13 | 0 | 13 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptPosition.cs` | 10 | 6 | 4 | 6 | None | Medium | 1/0 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` | 34 | 22 | 12 | 22 | Low | High | 8/8 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptValue.cs` | 28 | 20 | 8 | 20 | Low | High | 4/4 |
| `src/Broiler.VM.Profile.JavaScript/JavaScriptVerifier.cs` | 62 | 33 | 29 | 33 | Low | High | 14/14 |
| `src/Broiler.VM.Profile.JavaScript/JsArray.cs` | 19 | 14 | 5 | 14 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsBinary.cs` | 51 | 31 | 20 | 31 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsCollections.cs` | 87 | 51 | 36 | 51 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` | 142 | 127 | 15 | 127 | Low | High | 27/27 |
| `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` | 38 | 21 | 17 | 21 | Low | High | 9/9 |
| `src/Broiler.VM.Profile.JavaScript/JsFunction.cs` | 56 | 26 | 30 | 26 | Low | High | 1/1 |
| `src/Broiler.VM.Profile.JavaScript/JsGenerator.cs` | 68 | 17 | 51 | 17 | None | High | 5/5 |
| `src/Broiler.VM.Profile.JavaScript/JsModule.cs` | 40 | 13 | 27 | 13 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsNativeAbi.cs` | 3 | 3 | 0 | 3 | Low | Critical | 3/3 |
| `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` | 22 | 13 | 9 | 13 | Low | Critical | 18/18 |
| `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` | 11 | 10 | 1 | 10 | Low | Critical | 11/11 |
| `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` | 13 | 13 | 0 | 13 | Low | Critical | 13/13 |
| `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` | 16 | 8 | 8 | 8 | Low | Critical | 12/12 |
| `src/Broiler.VM.Profile.JavaScript/JsNumberFormat.cs` | 19 | 19 | 0 | 19 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsObject.cs` | 59 | 31 | 28 | 31 | Low | Medium | 0/0 |
| `src/Broiler.VM.Profile.JavaScript/JsProgram.cs` | 50 | 16 | 34 | 16 | Low | Medium | 0/0 |
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
| `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` | 85 | 47 | 38 | 47 | Low | High | 6/6 |
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
- `Broiler.VM.Profile.JavaScript.Compiler.JsArm64Backend.TryEmit(JsAssembledProgram, out JsNativeEmission, out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsArm64Backend.cs` - Security=High, Spec=none cited, `C3B8A0`, PENDING
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
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.SweepAgainstTheNumericManifest(JsAssembledProgram)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `375AB9`, PENDING
  - Falsified if: an artifact naming the numeric manifest is produced carrying an instruction that manifest does not admit
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.ScanAnnexB(System.Collections.Generic.IReadOnlyList<JsStatement>, System.Collections.Generic.HashSet<string>, System.Collections.Generic.List<string>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `8DEF5C`, PENDING
  - Falsified if: a declaration is admitted whose name a `var` of the same spelling could not be added under
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.ScanAnnexBStatement(JsStatement, System.Collections.Generic.HashSet<string>, System.Collections.Generic.List<string>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `643157`, PENDING
  - Falsified if: a statement that opens a lexical record forwards the enclosing blocking set unchanged
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.ScanAnnexBBlock(System.Collections.Generic.IReadOnlyList<JsStatement>, System.Collections.Generic.HashSet<string>, System.Collections.Generic.List<string>)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `CEAD6A`, PENDING
  - Falsified if: a block's own lexical names reach the test its own declarations are judged by
- `Broiler.VM.Profile.JavaScript.Compiler.JsCompiler.EmitAnnexBAlias(string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs` - Security=High, Spec=none cited, `DE88FF`, PENDING
  - Falsified if: the write lands on the block's own binding rather than the hoisting scope's
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
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, Spec=none cited, `27925C`, PENDING
  - Falsified if: a construct outside the numeric manifest reaches the lowering unrefused, or a construct the manifest admits is refused
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission.Statement(JsStatement, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, Spec=none cited, `4954C3`, PENDING
  - Falsified if: a statement kind this pass does not recognise reaches the lowering
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission.Function(JsFunctionDeclaration, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, Spec=none cited, `DBB3D5`, PENDING
  - Falsified if: a function body that can fall off its end is admitted, or a function that closes over an enclosing binding is
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission.Expression(JsExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, Spec=none cited, `3DC8F3`, PENDING
  - Falsified if: an expression kind this pass does not recognise reaches the lowering
- `Broiler.VM.Profile.JavaScript.Compiler.JsNumericAdmission.Call(JsCallExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsNumericAdmission.cs` - Security=High, Spec=none cited, `87953E`, PENDING
  - Falsified if: a call whose callee is not a function this program declares is admitted
- `Broiler.VM.Profile.JavaScript.Compiler.JsParser.ParseArrowParameters()` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsParser.cs` - Security=High, Spec=none cited, `7804D2`, PENDING
  - Falsified if: an arrow's parameter list is parsed with the enclosing `[Await]` context cleared
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
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Frame` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `5FA1CA`, PENDING
  - Falsified if: a field offset here differs from the offset the runtime gives that field of JsNativeFrame
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `35D721`, PENDING
  - Falsified if: an emitted unit answers a value the interpreter does not answer for the same bytecode and the same inputs
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.SemanticVersion` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `B49274`, PENDING
  - Falsified if: a template in this file changes without this number changing
- `Broiler.VM.Profile.JavaScript.Compiler.JsX64Backend.TryEmit(JsNativeProgramImage, out byte[], out JsNativeSymbolRow[], out string)` in `src/Broiler.VM.Profile.JavaScript.Compiler/JsX64Backend.cs` - Security=High, Spec=none cited, `86EFA0`, PENDING
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
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseBinary(int, bool)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `B46ED5`, PENDING
  - Falsified if: the tree this builds groups an operator differently from the language's precedence and associativity
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Combine(SliceSourceSpan, SliceTokenKind, SliceExpression, SliceExpression)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `1E98FB`, PENDING
  - Falsified if: an operator outside the manifest is built as a precise node, so the validation stage never sees it
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.Precedence(SliceTokenKind)` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `793D6C`, PENDING
  - Falsified if: two operators the language separates share a level here, or the order differs from the language's
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseUnary()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `49555A`, PENDING
  - Falsified if: a unary operator outside the manifest is built as a precise node, or its operand is not walked
- `Broiler.VM.Profile.JavaScript.Compiler.SliceParser.ParseCallChain()` in `src/Broiler.VM.Profile.JavaScript.Compiler/SliceParser.cs` - Security=High, Spec=none cited, `42B016`, PENDING
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
- `Broiler.VM.Profile.JavaScript.Format.JsNativeValues` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, Spec=none cited, `5F6922`, PENDING
  - Falsified if: an arithmetic operation over ordinary Numbers produces either of these bit patterns
- `Broiler.VM.Profile.JavaScript.Format.JsNativeValues.UninitialisedBits` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, Spec=none cited, `10ECDD`, PENDING
  - Falsified if: a realm binding is readable by emitted code before an initialiser stored to it
- `Broiler.VM.Profile.JavaScript.Format.JsNativeAbiProbe` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, Spec=none cited, `68CB0A`, PENDING
  - Falsified if: a field offset here differs from the offset the trampoline that fills it computes
- `Broiler.VM.Profile.JavaScript.Format.JsNativeAbiProbeLayout` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeEmitter.cs` - Security=High, Spec=none cited, `A42497`, PENDING
  - Falsified if: an offset here differs from the offset the runtime gives that field
- `Broiler.VM.Profile.JavaScript.Format.JsNativeFrame` in `src/Broiler.VM.Profile.JavaScript.Format/JsNativeFrame.cs` - Security=High, Spec=none cited, `B132E7`, PENDING
  - Falsified if: a field of this structure holds a reference the collector traces, or its declared offsets differ from the layout the runtime gives it
- `Broiler.VM.Profile.JavaScript.JavaScriptReadAdapter` in `src/Broiler.VM.Profile.JavaScript/JavaScriptDiagnostics.cs` - Security=High, Spec=none cited, `1DD7A4`, PENDING
  - Falsified if: a charge made through this adapter reaches a dimension other than the one named, or a released byte count is charged rather than released
- `Broiler.VM.Profile.JavaScript.JavaScriptInstance` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `E818FA`, PENDING
  - Falsified if: two instances over one shared handle observe each other's locals, or any instance state is reachable from that handle
- `Broiler.VM.Profile.JavaScript.JavaScriptContinuation` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `38E70F`, PENDING
  - Falsified if: this milestone constructs one, or a resume presented with one is answered as anything but a contract violation
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `E9D7AE`, PENDING
  - Falsified if: any input makes a member here throw, or an answer is produced that is not one of the five step kinds
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Instantiate(VmVerifiedArtifact, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `8F63E1`, PENDING
  - Falsified if: a handle this profile did not verify produces an instance
- `Broiler.VM.Profile.JavaScript.JavaScriptExecutor.Invoke(IVmInstanceState, in VmInvocationRequest, System.Threading.CancellationToken)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptExecutor.cs` - Security=High, Spec=none cited, `89BE7A`, PENDING
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
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.DescriptorReEmittingWith(Format.IJsNativeEmitter, params VmFeatureManifestId[])` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `A32D4C`, PENDING
  - Falsified if: a descriptor built here admits a native payload whose bytes its emitter does not reproduce
- `Broiler.VM.Profile.JavaScript.JavaScriptProfile.Build(ImmutableArray<string>, Format.IJsNativeEmitter?)` in `src/Broiler.VM.Profile.JavaScript/JavaScriptProfile.cs` - Security=High, Spec=none cited, `8F3F86`, PENDING
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
- `Broiler.VM.Profile.JavaScript.JsFinalizationRegistryObject` in `src/Broiler.VM.Profile.JavaScript/JsCollections.cs` - Security=High, Spec=none cited, `66E399`, PENDING
  - Falsified if: a cleanup callback registered here is ever invoked, or any guest code runs from a CLR finalizer
- `Broiler.VM.Profile.JavaScript.JsEngine.DrainJobs()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `541804`, PENDING
  - Falsified if: a job runs at a point the host did not ask for, or an endless queue is a hang rather than an exhaustion
- `Broiler.VM.Profile.JavaScript.JsEngine.StepOneJob(out JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `D67CCF`, PENDING
  - Falsified if: more than one job runs in a step, or a step reports a queue state the queue does not have
- `Broiler.VM.Profile.JavaScript.JsEngine.DropPendingJobs()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `459593`, PENDING
  - Falsified if: a queued job runs during an unwind
- `Broiler.VM.Profile.JavaScript.JsEngine.Loader` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `67C7AE`, PENDING
  - Falsified if: a mediator is used outside the invocation that supplied it
- `Broiler.VM.Profile.JavaScript.JsEngine.Evaluate(JsValue[], bool, Format.JsFormat.FunctionFlags)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `AF4B33`, PENDING
  - Falsified if: guest source becomes executable bytes without passing through the mediator
- `Broiler.VM.Profile.JavaScript.JsEngine.MaximumCallDepth` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `321EE5`, PENDING
  - Falsified if: a program recursing past this bound terminates the process rather than throwing a catchable RangeError
- `Broiler.VM.Profile.JavaScript.JsEngine.reportingDepth` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `DA5D1D`, PENDING
  - Falsified if: this stays set after the refusal has been thrown, so a later recursion is unbounded
- `Broiler.VM.Profile.JavaScript.JsEngine.StackBackstopReached()` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `94E6AA`, PENDING
  - Falsified if: the stack backstop produces a result naming no dimension, or a refused charge commits anything
- `Broiler.VM.Profile.JavaScript.JsEngine.ChargeText(int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `626201`, PENDING
  - Falsified if: a text operation's charge does not grow with the input the guest controls
- `Broiler.VM.Profile.JavaScript.JsEngine.ToNumberFromText(string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `07B791`, PENDING
  - Falsified if: a longer numeric string is read for the same charge as a shorter one
- `Broiler.VM.Profile.JavaScript.JsEngine.instanced` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `9CFED6`, PENDING
  - Falsified if: two instances of one module key exist in one realm
- `Broiler.VM.Profile.JavaScript.JsEngine.RunModuleGraph(JsProgram, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `6350BB`, PENDING
  - Falsified if: a module body runs twice in one realm, or a module body runs before every module's declarations are initialised
- `Broiler.VM.Profile.JavaScript.JsEngine.Evaluated(JsProgram, int)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `3D2BD8`, PENDING
  - Falsified if: a module body runs twice in one realm
- `Broiler.VM.Profile.JavaScript.JsEngine.Instantiate(JsProgram)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `A1564F`, PENDING
  - Falsified if: a module body runs before every module of its artifact has its declarations initialised
- `Broiler.VM.Profile.JavaScript.JsEngine.Step(JsProgram, JsModuleInstance[], System.Collections.Generic.List<int>, int, System.Action<JsEngine, JsValue, bool>?)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `DFDCE7`, PENDING
  - Falsified if: a module body runs before a module it requested has finished awaiting
- `Broiler.VM.Profile.JavaScript.JsEngine.Confirm(JsProgram)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `4B22CD`, PENDING
  - Falsified if: a module request is honoured without the composition being asked, or a refusal is treated as an answer
- `Broiler.VM.Profile.JavaScript.JsEngine.DynamicImport(JsProgram, string, JsValue, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `166A0A`, PENDING
  - Falsified if: this throws instead of rejecting, or a specifier reaches bytes without passing through the mediator or the artifact's own records
- `Broiler.VM.Profile.JavaScript.JsEngine.ImportedModule(JsProgram, string, string)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `0713ED`, PENDING
  - Falsified if: one module key is evaluated twice in one realm
- `Broiler.VM.Profile.JavaScript.JsEngine.BindParameters(JsProgram, JsCodeUnit, JsScriptFunction, JsFrame, JsValue, JsValue[])` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `E1EDFC`, PENDING
  - Falsified if: a generator over a non-simple parameter list reports a binding failure at its first resumption rather than at its call
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
- `Broiler.VM.Profile.JavaScript.JsEngine.ChargeComparison(JsValue, JsValue)` in `src/Broiler.VM.Profile.JavaScript/JsEngine.cs` - Security=High, Spec=none cited, `B5D7E9`, PENDING
  - Falsified if: comparing two long equal strings costs what comparing two short ones costs
- `Broiler.VM.Profile.JavaScript.JsPause` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `6E5458`, PENDING
  - Falsified if: anything reachable from the realm is published through this payload
- `Broiler.VM.Profile.JavaScript.JsContinuation` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `231B03`, PENDING
  - Falsified if: a continuation is honoured against an instance that did not produce it
- `Broiler.VM.Profile.JavaScript.JsInstance.Environment` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `1C7767`, PENDING
  - Falsified if: this environment is asked for a mediator outside an invocation it supplied one for
- `Broiler.VM.Profile.JavaScript.JsExecution.StepEntryPoint` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `815A20`, PENDING
  - Falsified if: a guest pause creates a core suspension, or a step runs more than one job
- `Broiler.VM.Profile.JavaScript.JsExecution.StepJobs(VmProfileId, JsInstance, int)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `B5AF9E`, PENDING
  - Falsified if: a step parks with an empty queue, or completes with a job still due
- `Broiler.VM.Profile.JavaScript.JsExecution.Resume(VmProfileId, IVmInstanceState, IVmProfileContinuation)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `5DF4EF`, PENDING
  - Falsified if: a continuation is honoured against an instance that did not produce it
- `Broiler.VM.Profile.JavaScript.JsExecution.Unwind(IVmProfileContinuation)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `CE785F`, PENDING
  - Falsified if: guest code runs during an unwind
- `Broiler.VM.Profile.JavaScript.JsExecution.RunOnGuestStack(JsInstance, uint?)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `CDA795`, PENDING
  - Falsified if: guest code runs on the caller's stack, or an exception the guest raised does not reach the caller
- `Broiler.VM.Profile.JavaScript.JsExecution.RunOneJobOnGuestStack(JsInstance)` in `src/Broiler.VM.Profile.JavaScript/JsExecution.cs` - Security=High, Spec=none cited, `6D52BB`, PENDING
  - Falsified if: a job runs on the caller's stack, or a job that throws ends the stepping
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
- `Broiler.VM.Profile.JavaScript.JsNativeAbiObservation` in `src/Broiler.VM.Profile.JavaScript/JsNativeAbi.cs` - Security=High, Spec=none cited, `AB829C`, PENDING
  - Falsified if: a field here reports something the trampoline did not record
- `Broiler.VM.Profile.JavaScript.JsNativeAbi` in `src/Broiler.VM.Profile.JavaScript/JsNativeAbi.cs` - Security=Critical, Spec=none cited, `2AA164`, PENDING
  - Falsified if: any product execution path calls this, or a mapping it makes is writable and executable at once
- `Broiler.VM.Profile.JavaScript.JsNativeAbi.Run(System.ReadOnlySpan<byte>, uint, uint, int, int, double[], long, int)` in `src/Broiler.VM.Profile.JavaScript/JsNativeAbi.cs` - Security=Critical, Spec=none cited, `AAE7EE`, PENDING
  - Falsified if: this reports a stack pointer the trampoline did not record, or it returns without releasing the mapping
- `Broiler.VM.Profile.JavaScript.JsNativeInstance` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `7ABDB0`, PENDING
  - Falsified if: anything reachable from this instance holds a managed reference emitted code can dereference
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.OperandSlabSlots` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `A12001`, PENDING
  - Falsified if: an emitted unit can be entered when fewer slots remain than its region needs
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.FuelPerInvocation` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `5FA9CD`, PENDING
  - Falsified if: an invocation runs past this many charged events without the meter being consulted
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.JsNativeInstance(JsProgram, JsNativePage, IVmExecutionEnvironment, double[], double[], double[], long[])` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `E54A21`, PENDING
  - Falsified if: an instance is constructed around a mapping that is not armed
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Page` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `574100`, PENDING
  - Falsified if: this hands out a mapping that is not armed
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Operands` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `489648`, PENDING
  - Falsified if: this slab is not pinned, so the collector can move it while emitted code holds its address
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Bindings` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `75149F`, PENDING
  - Falsified if: this slab is not pinned, or a slot of it is readable by emitted code before an initialiser stored to it
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Fuel` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `5C814B`, PENDING
  - Falsified if: this slab is not pinned, so the collector can move it while emitted code is decrementing it
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.TryCreate(JsProgram, IVmExecutionEnvironment)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `CA4D68`, PENDING
  - Falsified if: an instance is produced whose mapping is not armed
- `Broiler.VM.Profile.JavaScript.JsNativeInstance.Dispose()` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `5D9DD4`, PENDING
  - Falsified if: a mapping outlives the instance that owns it, or is released while an invocation is still inside it
- `Broiler.VM.Profile.JavaScript.JsNativeExecution` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `65CC7E`, PENDING
  - Falsified if: the form an invocation runs under differs from the form its handle carried when it was minted
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.CarriesEmittedCode(JsProgram)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `2492DE`, PENDING
  - Falsified if: this answers true for an artifact emitted for a convention this process does not use
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.HostArchitecture` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `658D99`, PENDING
  - Falsified if: this names a convention other than the one this process actually uses
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.Instantiate(JsProgram, IVmExecutionEnvironment)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `DC113E`, PENDING
  - Falsified if: an artifact emitted for another architecture is instantiated
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.Invoke(VmProfileId, JsNativeInstance, in VmInvocationRequest)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=Critical, Spec=none cited, `09609E`, PENDING
  - Falsified if: a value is reported that the emitted code did not leave in the frame's first operand slot
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.TryFindSymbol(JsProgram, uint, out uint)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `B97A49`, PENDING
  - Falsified if: an offset is returned for a unit the symbol table does not name
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.Render(double)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `A768C5`, PENDING
  - Falsified if: a completion of undefined is reported as a Number, or a Number as undefined
- `Broiler.VM.Profile.JavaScript.JsNativeExecution.TypeOf(double)` in `src/Broiler.VM.Profile.JavaScript/JsNativeExecution.cs` - Security=High, Spec=none cited, `0FC6E8`, PENDING
  - Falsified if: this disagrees with what the interpreter's typeof answers for the same value
- `Broiler.VM.Profile.JavaScript.JsNativePage` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, Spec=none cited, `1954A2`, PENDING
  - Falsified if: this half is reached on a system it was not written for
- `Broiler.VM.Profile.JavaScript.JsNativePage.ProtReadWrite` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, Spec=none cited, `A3E0E6`, PENDING
  - Falsified if: this value admits an execute
- `Broiler.VM.Profile.JavaScript.JsNativePage.ProtReadExecute` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, Spec=none cited, `4E4A17`, PENDING
  - Falsified if: a protection value naming both write and execute permission appears in any source file this component compiles, or outside the stored witness inputs rule X1's negative controls are made of
- `Broiler.VM.Profile.JavaScript.JsNativePage.MapPrivateAnonymous` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, Spec=none cited, `913A18`, PENDING
  - Falsified if: this names a shared or a file-backed mapping
- `Broiler.VM.Profile.JavaScript.JsNativePage.MapFailed` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, Spec=none cited, `BAFAB4`, PENDING
  - Falsified if: this differs from the value the platform answers a failed mapping with, so a failure is read as an address
- `Broiler.VM.Profile.JavaScript.JsNativePage.MapUnix(nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, Spec=none cited, `31576D`, PENDING
  - Falsified if: this passes any protection other than the readable-and-writable one
- `Broiler.VM.Profile.JavaScript.JsNativePage.ArmUnix(byte*, nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, Spec=none cited, `7C06EF`, PENDING
  - Falsified if: this passes any protection that admits a write
- `Broiler.VM.Profile.JavaScript.JsNativePage.ReleaseUnix(byte*, nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, Spec=none cited, `2252AF`, PENDING
  - Falsified if: this unmaps an address or a length the mapping does not own
- `Broiler.VM.Profile.JavaScript.JsNativePage.Map(void*, nuint, int, int, int, nint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, Spec=none cited, `491B84`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.JavaScript.JsNativePage.Protect(void*, nuint, int)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, Spec=none cited, `70F3E8`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.JavaScript.JsNativePage.Unmap(void*, nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Unix.cs` - Security=Critical, Spec=none cited, `5B2FE6`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.JavaScript.JsNativePage` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, Spec=none cited, `1954A2`, PENDING
  - Falsified if: this half is reached on a system it was not written for
- `Broiler.VM.Profile.JavaScript.JsNativePage.MemCommitAndReserve` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, Spec=none cited, `724619`, PENDING
  - Falsified if: this names anything but reserving and committing, or it carries a protection bit
- `Broiler.VM.Profile.JavaScript.JsNativePage.MemRelease` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, Spec=none cited, `95117F`, PENDING
  - Falsified if: this names a free that leaves the reservation standing
- `Broiler.VM.Profile.JavaScript.JsNativePage.PageReadWrite` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, Spec=none cited, `9DE8C2`, PENDING
  - Falsified if: this value admits an execute
- `Broiler.VM.Profile.JavaScript.JsNativePage.PageExecuteRead` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, Spec=none cited, `056695`, PENDING
  - Falsified if: a protection value naming both write and execute permission appears in any source file this component compiles, or outside the stored witness inputs rule X1's negative controls are made of
- `Broiler.VM.Profile.JavaScript.JsNativePage.MapWindows(nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, Spec=none cited, `5C64B1`, PENDING
  - Falsified if: this passes any protection other than the readable-and-writable one
- `Broiler.VM.Profile.JavaScript.JsNativePage.ArmWindows(byte*, nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, Spec=none cited, `10433E`, PENDING
  - Falsified if: this passes any protection that admits a write
- `Broiler.VM.Profile.JavaScript.JsNativePage.ReleaseWindows(byte*)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, Spec=none cited, `7E36F0`, PENDING
  - Falsified if: this releases an address the mapping does not own, or releases one twice
- `Broiler.VM.Profile.JavaScript.JsNativePage.VirtualAlloc(void*, nuint, uint, uint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, Spec=none cited, `634738`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.JavaScript.JsNativePage.VirtualProtect(void*, nuint, uint, uint*)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, Spec=none cited, `3BA5CB`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.JavaScript.JsNativePage.VirtualFree(void*, nuint, uint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, Spec=none cited, `65CC8A`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.JavaScript.JsNativePage.FlushInstructionCache(void*, void*, nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, Spec=none cited, `030701`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.JavaScript.JsNativePage.GetCurrentProcess()` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.Windows.cs` - Security=Critical, Spec=none cited, `FCD535`, PENDING
  - Falsified if: this signature differs from the one the platform exports
- `Broiler.VM.Profile.JavaScript.JsNativePageState` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, Spec=none cited, `89CCAB`, PENDING
  - Falsified if: a mapping this type produced is both writable and executable at any instant
- `Broiler.VM.Profile.JavaScript.JsNativePage` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, Spec=none cited, `7457A9`, PENDING
  - Falsified if: any mapping this type creates is executable while it is writable, or an entry pointer is handed out for a mapping that is not armed
- `Broiler.VM.Profile.JavaScript.JsNativePage.address` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, Spec=none cited, `82E226`, PENDING
  - Falsified if: this holds an address this type did not map, or it is read after the mapping was released
- `Broiler.VM.Profile.JavaScript.JsNativePage.length` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, Spec=none cited, `8F7E00`, PENDING
  - Falsified if: this differs from the number of bytes the mapping was actually made with
- `Broiler.VM.Profile.JavaScript.JsNativePage.state` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, Spec=none cited, `EF3CDD`, PENDING
  - Falsified if: this says Armed for a mapping the operating system still admits a write to, or Writable for one it admits an execute from
- `Broiler.VM.Profile.JavaScript.JsNativePage.JsNativePage(byte*, nuint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, Spec=none cited, `29EF7E`, PENDING
  - Falsified if: a mapping is created with any execute permission
- `Broiler.VM.Profile.JavaScript.JsNativePage.State` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, Spec=none cited, `77BB7D`, PENDING
  - Falsified if: this reports a state the mapping is not actually in
- `Broiler.VM.Profile.JavaScript.JsNativePage.TryMap(System.ReadOnlySpan<byte>)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, Spec=none cited, `E8DB67`, PENDING
  - Falsified if: a caller can write to a mapping this method returned after Arm has run
- `Broiler.VM.Profile.JavaScript.JsNativePage.Arm()` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, Spec=none cited, `AE60F1`, PENDING
  - Falsified if: this leaves the mapping writable, or it succeeds without removing write permission
- `Broiler.VM.Profile.JavaScript.JsNativePage.Entry(uint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, Spec=none cited, `41FDB2`, PENDING
  - Falsified if: an entry pointer is produced for a mapping that is not armed
- `Broiler.VM.Profile.JavaScript.JsNativePage.At(uint)` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, Spec=none cited, `FEC06D`, PENDING
  - Falsified if: an address is produced for a mapping that is not armed
- `Broiler.VM.Profile.JavaScript.JsNativePage.Dispose()` in `src/Broiler.VM.Profile.JavaScript/JsNativePage.cs` - Security=Critical, Spec=none cited, `A80C3A`, PENDING
  - Falsified if: a second call releases a mapping a second time
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
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReadManifest(in VmArtifactDescriptor, ref VmBoundedReader, Sections)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `7CFF0B`, PENDING
  - Falsified if: an artifact naming a manifest this build does not accept is admitted, or the descriptor and the payload are allowed to name different ones
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReadNativeCode(ref VmBoundedReader, ulong, Sections)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `FA7FCC`, PENDING
  - Falsified if: a declared length that disagrees with the bytes present is accepted, or an architecture value this build cannot name is
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReadNativeSymbols(ref VmBoundedReader, Sections)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `F1C392`, PENDING
  - Falsified if: an offset outside the emitted blob is accepted, or two rows naming one code unit are
- `Broiler.VM.Profile.JavaScript.JsVerifier.LinkNative(Sections, JsCodeUnit[], IJsNativeEmitter?)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `282F69`, PENDING
  - Falsified if: an artifact whose symbol table names fewer units than the function table is admitted, or a symbol offset outside the emitted blob is
- `Broiler.VM.Profile.JavaScript.JsVerifier.ReEmit(Sections, byte[], JsNativeSymbolRow[], IJsNativeEmitter?)` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `001336`, PENDING
  - Falsified if: an artifact whose emitted bytes differ from this image's own emission of its bytecode is admitted while an emitter is present
- `Broiler.VM.Profile.JavaScript.JsVerifier.LinkModules(Sections, JsCodeUnit[], IVmVerificationContext, JavaScriptReadAdapter, out JsModuleRecord[], out JsBinding[])` in `src/Broiler.VM.Profile.JavaScript/JsVerifier.cs` - Security=High, Spec=none cited, `8255D8`, PENDING
  - Falsified if: linking recurses to a depth the payload chooses, or a cyclic export resolution is answered by spending an allowance
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

That is not a figure of speech. 3261 of the 3375 assessed units declare
`Origin=AI`, and the records this component implements were drafted the same way. An
adversarial pass over the work confirmed findings and they were corrected, which is a check
on it and not an independent judgement of it. Reading a declaration is the only thing that
makes it read.
