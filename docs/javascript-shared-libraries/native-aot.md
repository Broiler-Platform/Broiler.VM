# Native AOT compatibility of the decisions and the plan

[← README](README.md) · [Roadmap](roadmap.md) · [Decisions](decisions.md) · [Architecture](architecture.md)

**Date:** 2026-10-11. **Status:** assessment, with the corrections to the plan it led to.

**Question.** Is every decision taken on 2026-10-11 ([decisions.md](decisions.md)), and every
milestone of the programme, compatible with Native AOT?

**Method.**

- Source and build files were read in Broiler.VM (`88d8b51d`), Broiler.JS (`44a214dc`),
  Broiler.Unicode (`2affa78f`) and Broiler.RegEx (`c1422f4d`).
- In scratch clones outside every repository, the libraries were built with the trim, AOT and
  single-file analyzers on. Small Native AOT applications over Broiler.Regex, Broiler.Unicode and
  Broiler.JS's portable path were published and run, with SDK 10.0.401 and ILCompiler 10.0.12, for
  `win-x64`.
- **These are observations of repository state.** They explain the verdicts below and meet no
  gate. Each milestone collects its own evidence, and no figure from these runs appears here.

---

## 1. Verdict

**Yes. Every decision and every milestone is compatible with Native AOT. The plan had gaps through
which an incompatibility could have passed unseen; this change closes them (section 5).**

- **No decision needs dynamic code.** None needs code generated at run time, reflection over a type
  named at run time, or data loaded by name. The data shape of `SJL-D-6` (c) fits Native AOT
  better than any of its alternatives: embedded tables over constant data, handed in by the caller.
- **Broiler.VM already enforces it.** Its JavaScript roots publish with Native AOT and full
  trimming, with `InvariantGlobalization` and `UseSystemResourceKeys` set and warnings as errors.
  ILC applies that to every assembly it compiles, packages included. **A shared package with a trim
  or AOT warning in code a root reaches fails Broiler.VM's AOT lane.**
- **The code the plan moves is AOT-clean.**
  - What moves out of Broiler.VM uses no dynamic code.
  - The plan never moves Broiler.JS's AOT-unsafe or ICU-dependent members; it replaces them. These
    are its reflection-based JSON deserialization, its regular-expression-based Date and Temporal
    parsers, and its ICU-based Intl, normalization and casing.
- **Both component libraries already publish and run under Native AOT with no warning**, with output
  equal to their JIT runs. Two things are missing:
  - neither declares `IsAotCompatible` or `IsTrimmable`;
  - Broiler.Regex's parser recurses without a guard, so a deeply nested pattern terminates the
    process.
- **Three things stay outside Native AOT by design** (section 6): Broiler.JS's main engine,
  Broiler.JS's host providers under invariant globalization, and the Mono-based Android root.

---

## 2. Where Native AOT is enforced

### 2.1 Broiler.VM

**The roots.** These JavaScript composition roots are published with Native AOT by the CI lane and
by `eng/collect-js-evidence.py`:

| Root | Carries the compiler | Carries `.Intl` |
|---|---|---|
| JavaScript.ExecutionOnly | no | no |
| JavaScript.SliceCompiler | yes | yes |
| JavaScript.Conformance | yes | yes |
| JavaScript.Cli | yes | no |

PolyglotCli is published with Native AOT only by bundle `ubc-4-002`'s collector. JavaScript.Android
runs on Mono with neither trimming nor AOT compilation.

**The settings.**

- Every desktop root sets `PublishTrimmed`, `TrimMode` full, `InvariantGlobalization`,
  `UseSystemResourceKeys`, `TreatWarningsAsErrors` and `IsAotCompatible`, with `TrimmerSingleWarn`
  off.
- `PublishAot` is passed on the command line.
- ILCompiler's `IlcTreatWarningsAsErrors` defaults to `TreatWarningsAsErrors`, so a trim or AOT
  warning anywhere in what ILC compiles fails the publish.
- **The compiler-bearing roots are Native AOT images.** The shared frontend therefore runs inside
  them, even though its boundary is called "compile time" (it is the *JavaScript* compiler's).

**The three modes.**

- The CI lane (`.github/workflows/broiler-vm-lane.yml`, job `publish`) publishes the roots with
  Native AOT only, runs them, and runs Octane and test262 subtrees from the AOT images.
- `eng/collect-js-evidence.py` publishes under JIT, trimming and Native AOT.
- A pristine consumer of the JavaScript **packages** is published with Native AOT and run (bundle
  JS-10-002). Packages admitted later are exercised there too.

**What Broiler.VM's rules can see.**

- Rules B5, B5b and B5c read the compiled assemblies of the solution's own projects:
  - B5: dynamic loading, reflection invocation, `Reflection.Emit`, `System.Runtime.Loader`,
    `LambdaExpression.Compile`, native-library and function-pointer marshalling, and method
    preparation;
  - B5b: module initializers;
  - B5c: platform invokes.

  **A package's assembly is never read by them.** Until rule `I2` exists, a package's code is
  checked only by ILC at publish, and only where a root reaches it.
- Rule N36 inventories warning suppressions in the family's own sources.
- **In Native AOT mode, the published directory holds no managed assembly.** The collectors list
  managed DLLs, so an AOT closure report is empty by construction, and rule K4 accepts that. The
  only AOT-side statement of an image's contents is the catalog the binary prints for `--closure`
  (rule K3). **As planned, rule `I3` would therefore have been blind to a shared assembly inside a
  Native AOT image** (gap G3).

**Data and binding.**

- Every generated table is a `ReadOnlySpan<byte>` property over constant data: a byte array or a
  UTF-8 literal. There are no embedded resources.
- The `.Intl` data reaches the profile explicitly: roots pass `JsCldrData.Instance` through
  `JsComposition`. There is no reflection and no assembly scanning.

**Stack and input.**

- Compilation runs on a dedicated large-stack thread (`CompilationStack`, rule N19). The interpreter
  runs guest code on a large-stack thread with a call-depth bound. The regular-expression matcher
  backtracks on an explicit stack, and pattern nesting is capped.
- The Conformance root, which runs as a Native AOT image, reads its inputs with hand-written readers
  and `JsonDocument`. No reflection-based JSON serializer is in any published image.

### 2.2 The home repository

The plan makes the shared libraries meet the bar Broiler.VM's roots set:

- contract convention 12 ([architecture.md](architecture.md#4-contract-conventions));
- rules SL3 and SL5 ([governance.md](governance.md#32-shared-libraries-group-sl-in-their-home-repository));
- the folder settings of SJL-2.10;
- the Native AOT probe of SJL-2.14.

Section 5 records what this assessment corrected in each.

### 2.3 The component repositories

| | Broiler.RegEx (`c1422f4d`) | Broiler.Unicode (`2affa78f`) |
|---|---|---|
| Declares `IsAotCompatible`, `IsTrimmable` | Neither | Neither |
| Analyzer build with `IsAotCompatible` on | No trim or AOT warning | No trim or AOT warning, on both its frameworks |
| Native AOT probe | Published with no warning; output equal to the JIT run | Published with no warning; output equal to the JIT run |
| Dynamic code, resources, P/Invoke | None. Patterns compile into delegate closures. One `GetType().Name` appears in an exception message. | None |
| Recursion | **The parser recurses with no depth guard.** A deeply nested pattern overflows the stack and terminates the process, at different depths under JIT and Native AOT. The only guard is in the quantifier driver. | Bounded: the emoji trie walk, by the longest sequence |
| Culture | None | Locale casing (`JSStringSpecialCasing.ToLocaleUpper` and `ToLocaleLower`) goes through the caller's `CultureInfo.TextInfo` |
| Data | Generated hex text, parsed into arrays at type initialisation | Generated dictionaries of hex text parsed on first use, generated arrays, and large static constructors in the CLDR package; an unbounded Chinese-calendar cache under a global lock |
| CI | Build, test, pack; no AOT lane | Build, test, pack; no AOT lane |

Neither package carries the `IsTrimmable` marker, so a consumer's trimming treats it as not
annotated. Native AOT's default full trimming trimmed both anyway.

### 2.4 Broiler.JS

- **Its Native AOT target is the portable interpreter alone.** `Broiler.JavaScript.Portable` is a
  stack interpreter over doubles, with no strings, objects, `Math`, `Date`, `RegExp`, Intl or JSON.
  Its sample, `samples/Broiler.JavaScript.NativeAotSample`, sets `PublishAot` and
  `InvariantGlobalization` and references nothing else.
  - The sample published and ran.
  - `Portable.Compiler` runs offline under JIT and emits C# for the sample.
  - The AOT lane (`performance-baseline.yml`, the Phase 5 step) runs only on manual request.
- **Its main engine is not a Native AOT target.** It emits IL, through `DynamicMethod` or a dynamic
  assembly, and compiles expression trees.
  - A Native AOT publish of its startup host terminates before `Main`: `KeyStrings`' static
    constructor writes initonly static fields through reflection, which Native AOT refuses.
  - No analyzer flags this.
- **Analyzer build.**
  - Parser, Ast, Storage, Compiler, Portable and Portable.Compiler have no trim or AOT warning.
  - Runtime has reflection helpers, and generic instantiation over types known only at run time.
  - BuiltIns' only warnings come from `JSJsonParser`: reflection-based
    `JsonSerializer.Deserialize<JSValue>`, with no `JsonSerializerContext`.
- **No shared library reaches Broiler.JS's Native AOT image.** The shared libraries reach its JIT
  engine.

---

## 3. The decisions

| Decision | Taken | Under Native AOT | Verdict |
|---|---|---|---|
| `SJL-D-1` homes | (b) | A home changes who builds a library, not how it compiles. Each home builds under convention 12. The component homes adopt it at SJL-7 and SJL-11. | Compatible |
| `SJL-D-2` packages | (a) | One package per boundary keeps the frontend out of compiler-free images. `IsTrimmable` marks each package for trimming. | Compatible |
| `SJL-D-3` versions | Separate stream, minimum ranges, lock files | Native AOT fixes one version of each package at publish, and ILC compiles against it. A member removed by a later version fails at publish, not at run time, so binary compatibility within a major version matters here too. Lock files make the compiled version the tested one. | Compatible |
| `SJL-D-4` sources | (c) | Where a package comes from does not change what ILC compiles | Not affected |
| `SJL-D-5` assurance | (a) | (none) | Not affected |
| `SJL-D-6` data | (c) | Tables embedded as `ReadOnlySpan<byte>` over constant data compile into the image as read-only data, with no loading, no parsing and no reflection, and the caller hands them in. The generators run at build and test time under JIT, never in an image. The cost: an image that hosts both engines carries both engines' tables. | Compatible; the best fit |
| `SJL-D-7` JS-2 | (a) | Not taking the seed copy keeps IL-emitting code out of Broiler.VM | Compatible |
| `SJL-D-8`, `SJL-D-9` | (a); the policy | (none) | Not affected |
| `SJL-D-10` meter | (a) | A delegate is invoked, never generated. (b) would have been equally safe. | Compatible |
| `SJL-D-11` diagnostics | Codes and identifiers | Codes and message identifiers are data. Default English text is a constant string, never a satellite resource, so trimming and `UseSystemResourceKeys` cannot change it (convention 3, corrected). | Compatible |
| `SJL-D-12` tree | (a) with (i) | The immutable tree needs no reflection. Broiler.JS's adapter packages serve its JIT engine only. | Compatible |
| `SJL-D-13` slice path | (a) | The rebuilt slice path runs on the shared frontend inside Native AOT images, so the frontend must meet convention 12 (gap G1) | Compatible, with G1 closed |
| `SJL-D-14` regular expressions | (a) with (i) | Broiler.Regex's delegate-closure design is AOT-safe, and it publishes under Native AOT today. It must declare the flags and bound its nesting without CLR recursion (requirement 4). A limit set by the stack gives different answers under JIT and Native AOT. | Compatible, through SJL-11's requirements |
| `SJL-D-15` BigInt width | (a) | `BigInteger` is AOT-safe | Compatible |
| `SJL-D-16` corrections | (a) | (none) | Not affected |
| `SJL-D-17` extensions | They stay | Broiler.JS's culture-dependent extensions stay outside the shared libraries, which therefore behave identically under `InvariantGlobalization` | Compatible |
| `SJL-D-18` unification | Not planned | (none) | Not affected |
| `SJL-D-19` host policy | Providers | Providers are interfaces handed in by the caller, and nothing is discovered. Broiler.JS's host providers depend on host data that an invariant-globalization image lacks (section 6). | Compatible; host providers degrade by design |
| `SJL-D-20`, `SJL-D-21` | Broiler.VM's policy; ICU4X sources | Data only. `SJL-D-21` also avoids the .NET calendar classes. | Compatible |
| `SJL-D-22` locale data | (a) with (c) | The shared locale data is embedded. Under `InvariantGlobalization`, Broiler.JS's host provider sees only the invariant culture, and `CultureInfo.GetCultureInfo` throws for a named culture. The provider answers "unavailable" and never throws (now in the area design). | Compatible, with that rule |
| `SJL-D-23` pilot | (a) | (none) | Not affected |
| `SJL-D-24` collation | (c) | `UcaCollator` reads embedded tables through the provider | Compatible |
| `SJL-D-25` dispositions | Forwarders | Forwarding members and type forwarders resolve at compile time | Compatible |

---

## 4. The areas

| Area | Where the shared implementation comes from | AOT state of that source | What the plan does about the rest |
|---|---|---|---|
| Numbers (SJL-3) | The specification's algorithms; Broiler.VM's `JsNumberFormat` and Broiler.JS's number code as reference arms | Both clean: invariant-culture parsing and formatting only | (none needed) |
| Numerics (SJL-4) | Broiler.VM's `JsBigInt` design and Math kernels | Clean | (none needed) |
| Text (SJL-5) | Broiler.VM's hand-written JSON reader, quoting and codecs | Clean | **Deletes `JSJsonParser`'s reflection-based `System.Text.Json` deserialization**, BuiltIns' only AOT and trim warnings |
| Date (SJL-6) | Broiler.VM's grammars and arithmetic | Clean | `DateParser`'s compiled regular expressions and current-culture fallback stay behind Broiler.JS's dialect (`SJL-D-17`), outside shared code |
| Unicode (SJL-7) | The owner's algorithms; tables embedded per engine | The owner publishes under Native AOT with no warning | Requires the flags and an AOT probe of the owner (SJL-7.2). Its culture-based locale casing is replaced (SJL-7.2 item 6). |
| Time zones and calendars (SJL-8) | Broiler.VM's compiler and generators, at build time; tables embedded | Clean | Deletes Broiler.JS's resource-loaded `tzdata.bin` and its .NET calendars |
| Locale data and Intl (SJL-9); collation (SJL-17) | Broiler.VM's algorithms | Clean. One generic `Enum.Parse<TEnum>` in `JsSegmenter`'s break data is analyzer-clean and runs today inside the AOT image's checks. | Deletes Broiler.JS's ICU-based code; its host provider stays |
| Temporal (SJL-10) | Broiler.VM's `JsTemporalCore` and linear grammar | Clean | Deletes Broiler.JS's regular-expression parsers per type |
| Regular expressions (SJL-11) | Broiler.Regex, fixed | AOT-clean; unbounded parser recursion | Requirement 4, and now the flags with an AOT probe (SJL-11.2) |
| Frontend and static semantics (SJL-12 to SJL-15) | A new tree seeded from Broiler.VM's design | Broiler.VM's front ends are clean. Broiler.JS's parser is analyzer-clean but reads ambient `AsyncLocal` state, which SJL-12 removes. | Convention 12 now names the frontend (G1) |
| Vectors (SJL-1) | The corpus and its reader | Not built yet | The reader may use no reflection-based serializer, because the AOT probe and Broiler.VM's AOT harness run it (G4) |

---

## 5. Gaps found, and the corrections made

| # | Gap | Correction |
|---|---|---|
| G1 | **Convention 12 covered "every runtime library".** The frontend's boundary is called "compile time", so a reader could exempt it, but it runs inside Broiler.VM's Native AOT images. Test code the AOT probe and Broiler.VM's harness run was not covered either. | Convention 12 and rule SL5 now cover every shared library, the frontend included, and the test code the probe or a Broiler.VM harness root runs ([architecture.md](architecture.md#4-contract-conventions), [governance.md](governance.md#32-shared-libraries-group-sl-in-their-home-repository), SJL-2.10). |
| G2 | **Rule `I2`'s list was narrower than B5's, and no rule mirrored B5 for the shared libraries.** B5, B5b and B5c never read a package. Convention 8 and rule SL3 also left out loading by name and generic instantiation over a type known only at run time. | Convention 8 and rule SL3 now ban everything on B5's, B5b's and B5c's lists, generic instantiation over a run-time `Type`, `Enum.GetValues(Type)`, `dynamic` and reflection-based serialization. Providers are bound by the caller, never discovered. Rule `I2` applies B5's, B5b's and B5c's lists to every admitted assembly (governance section 3.1, SJL-2.6). |
| G3 | **Rule `I3` could not see inside a Native AOT image.** Its published directory holds no managed assembly. | For the Native AOT mode, `I3` reads ILC's own record of what it compiled: the mstat file that `IlcGenerateMstatFile` writes beside the intermediate output, which the collector copies into the bundle (governance section 3.1, SJL-2.6). |
| G4 | **The probe's settings and coverage, the vector reader and warning suppressions were unstated.** | The Native AOT probe publishes with Broiler.VM's root settings and runs the whole corpus, the frontend's and the data-backed libraries' over embedded tables included (convention 12, SJL-2.14). The vector reader uses no reflection-based serializer ([areas/conformance-vectors.md](areas/conformance-vectors.md#4-runners)). Reference arms stay in the JIT test project ([validation.md](validation.md#3-reference-arms)). No trim or AOT warning may be suppressed: there is an inventory in the shape of Broiler.VM's N36 (rule SL5). |
| G5 | **The component and Broiler.JS obligations were implicit.** Neither component declares the flags or runs an AOT lane, and Broiler.JS's AOT sample was evidence at SJL-14 but in no clause. | SJL-7.2 and SJL-11.2 require both flags and a Native AOT probe in the component's CI (governance items C1 and C3). SJL-14 gains a clause: the sample's program regenerates identically, and the sample publishes and runs. Architecture section 6.2 says which Broiler.JS image is a Native AOT target. |

**Smaller statements made explicit:**

- Default English text is a constant string (convention 3).
- A limit is never the stack (convention 5).
- Generated tables are `ReadOnlySpan<byte>` over constant data (convention 13).
- JavaScript.Android runs on Mono (architecture section 6.1).
- An image that hosts both engines carries both engines' tables (architecture section 6.3).
- Broiler.JS's host provider never throws when the host has no culture data (the locale-data
  design, section 3.3).

---

## 6. Outside Native AOT, by design

- **Broiler.JS's main engine.** It generates IL at run time and is not a Native AOT target, and the
  programme does not change that. Its Native AOT image is the portable interpreter, which no shared
  library reaches.
- **Broiler.JS's host providers and culture-dependent extensions** (`SJL-D-19`, `SJL-D-22` (c),
  `SJL-D-17`) under `InvariantGlobalization`, which Native AOT applications commonly set:
  - named cultures are unavailable, and `CultureInfo.GetCultureInfo` throws for one;
  - normalization through the platform does nothing;
  - the host zone's IANA identifier cannot be resolved, so Temporal's default zone falls back to
    UTC.

  These are Broiler.JS policy, and they stay outside the shared libraries. The libraries depend on
  no host globalization data (convention 9), so they answer the same in every mode.
- **JavaScript.Android** runs on Mono, with neither trimming nor AOT compilation. The shared
  libraries rely on nothing that differs there.

---

## 7. A note on the build machine

On the owner's Windows machine, a Native AOT link needs the Visual Studio installer directory on
`PATH`, because the toolchain's setup script calls `vswhere` by name. Broiler.VM's
`eng/collect-js-evidence.py` arranges this, and the probes behind this assessment did the same.
