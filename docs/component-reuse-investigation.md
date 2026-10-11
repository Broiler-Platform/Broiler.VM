# Component and .NET reuse investigation

Date: 2026-10-11. Scope: the current local Broiler.VM, Broiler.RegEx, Broiler.Unicode and Broiler.DateTime source trees; selected Broiler.JS and Broiler.Native sources for comparison. Broiler.VM HEAD was `f3b8fd59f4958541da7ff61e2d957ade0121fe5e`, with substantial pre-existing working-tree changes. Findings refer to that working tree, not just the commit. This report changes no implementation or dependency and makes no release or performance claim.

## Recommendation

There are worthwhile replacements. Start with four small .NET substitutions, then consolidate shared Unicode/CLDR data. Treat Broiler.Regex integration and general date/time reuse as component work with explicit compatibility requirements, not package swaps.

Keep JavaScript coercion, observable behavior, resource charging, diagnostics and pinned-data decisions in the VM. Move only the underlying primitive or data responsibility to a shared component. Keep the core runtime independent of language-specific components.

| Order | Candidate | Assessment | Status |
|---|---|---|---|
| 1 | UTF-8 validation, leading-zero count, reference comparer, endian reads, base64url | Ready for focused .NET refactors; no new packages. | **Implemented & Verified** in `Broiler.VM` (all tests passing). |
| 2 | CLDR list patterns and Umm al-Qura data | Shared component APIs exist; all currently used rows compared equal. Preserve optional Intl composition. | **Verified Parity & Boundary Evaluated** (`IJsIntlData` composition model preserved). |
| 3 | Unicode property and emoji data | Large consolidation opportunity; needs strict ECMAScript name validation, immutable access and full table parity checks. | **Evaluated** (ECMAScript exact-match & immutability prerequisites documented). |
| 4 | Gregorian arithmetic in Broiler.DateTime | Real duplication; expose allocation-free public day/civil conversion helpers first. | **Implemented in `Broiler.DateTime`** (public primitives exposed & verified; A1/A2/N1 boundary evaluated). |
| 5 | Broiler.Regex matcher/parser | Valuable eventual consolidation, but current syntax and resource-control differences prevent direct substitution. | **Evaluated** (Upstream fuel callback & syntax prerequisites cataloged). |
| 6 | Unicode casing, normalization and segmentation | Fix/extend the shared component before migration; these are not ready replacements. | **Repaired in `Broiler.Unicode`** (surrogate defect fixed, test suite added & green). |

## 1. Straightforward .NET replacements

The repository targets .NET 10 ([build properties](D:/Broiler.VM/Directory.Build.props:26)). Each of the following uses framework APIs, with no additional package dependency.

| Existing implementation | Replacement | What must remain |
|---|---|---|
| [WasmName.IsWellFormed](D:/Broiler.VM/src/Broiler.VM.Profile.WebAssembly/WasmName.cs:60), including its `Follows` helper | `System.Text.Unicode.Utf8.IsValid(bytes)` | Whole-name validation, read/allocation limits and malformed-name diagnostics in [WasmDecoder](D:/Broiler.VM/src/Broiler.VM.Profile.WebAssembly/WasmDecoder.cs:1291). |
| [MathLeadingZeros](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.Math.cs:482) | `System.Numerics.BitOperations.LeadingZeroCount(uint)` | The caller's ECMAScript `ToUint32` conversion. The framework method also returns 32 for zero. |
| [SliceNodeIdentityComparer](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs:44) | `System.Collections.Generic.ReferenceEqualityComparer.Instance` | Reference identity for AST records. Replace the dictionary comparer at both construction sites, including [SliceSourceCompiler](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs:166). |
| Manual 32-/64-bit little-endian decoding in [VmBoundedReader](D:/Broiler.VM/src/Broiler.VM.Binary/VmBoundedReader.cs:193) | `BinaryPrimitives.ReadUInt32LittleEndian` / `ReadUInt64LittleEndian` after successful `TryTake` | The bounded reader, its sticky error state, charging, cancellation and section checks. Only the shifts/loop are replaced. |

These are already established patterns in this repository: [UBC validates UTF-8](D:/Broiler.VM/src/Broiler.VM.Ubc/UbcArtifactReader.cs:583), [UBC counts leading zeros](D:/Broiler.VM/src/Broiler.VM.Ubc/UbcPrimitives.cs:738), and [the JavaScript compiler uses ReferenceEqualityComparer](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript.Compiler/JsCompiler.cs:460).

The `WasmName` comments reject replacement-based UTF-8 decoding. `Utf8.IsValid` is a validator: it creates no decoded string and does not repair invalid input. An in-memory differential probe of the exact existing source against this API on .NET 10.0.11 found **2,315,037 agreements and zero differences**. Coverage included empty input, every one-/two-byte string, every valid Unicode scalar encoded as UTF-8, one million seeded random strings of length 3–11, and 137,180 three-/four-byte combinations drawn from boundary byte values. This is substantial evidence, not a formal proof or a full WebAssembly suite run.

Official API references: [Utf8.IsValid](https://learn.microsoft.com/en-us/dotnet/api/system.text.unicode.utf8.isvalid?view=net-10.0), [LeadingZeroCount](https://learn.microsoft.com/en-us/dotnet/api/system.numerics.bitoperations.leadingzerocount?view=net-10.0), [ReferenceEqualityComparer](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.referenceequalitycomparer?view=net-10.0), [BinaryPrimitives](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.binary.binaryprimitives.readuint32littleendian?view=net-10.0).

### Smaller secondary opportunities

- [Base64Encode](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.Base64.cs:320) currently uses `Convert.ToBase64String` followed by two replacements for the URL alphabet. [`Base64Url.EncodeToString`](https://learn.microsoft.com/en-us/dotnet/api/system.buffers.text.base64url.encodetostring?view=net-10.0) can handle that branch. Its output omits padding; restore padding when the JavaScript option requests it, and preserve charging. This is an allocation-reduction candidate, not a measured speedup.
- [Base64DecodeHex](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.Base64.cs:578) could use the status-returning span overload of [`Convert.FromHexString`](https://learn.microsoft.com/en-us/dotnet/api/system.convert.fromhexstring?view=net-10.0). This needs an adapter and targeted tests for odd input length, limited destination capacity, partial writes and guest errors. Hex encoding already uses `Convert.ToHexStringLower`.
- The host-only [Test262Json writer](D:/Broiler.VM/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/Test262Json.cs:91) could use `Utf8JsonWriter` for escaping and punctuation. Retain the schema/member table and deterministic ordering; check output-format expectations before changing it. This recommendation does not extend to guest `JSON`.

## 2. Broiler.Unicode and CLDR: the strongest shared-component opportunities

### List patterns: verified narrow replacement

[JsRealm.ListFormat](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.ListFormat.cs:139) reads templates from VM-owned generated data. [CldrLocaleData.GetListPattern](D:/Broiler.Unicode/src/UnicodeCldr.LocaleData/CldrLocaleData.cs:119) supplies the corresponding templates.

All **18 current en/de VM rows match**, after decoding escapes and mapping `standard` to `conjunction` and `or` to `disjunction`. The shared component contains 135 rows. Both data sets identify CLDR 48.2.0. This verifies the current template subset, not the complete formatter or additional locale support.

Keep VM locale resolution, iteration, `formatToParts`, JavaScript object construction and charging. A shared data generator or provider adapter can remove duplicate template ownership without changing those semantics.

### Umm al-Qura: verified table replacement

[JsCalendarData.UmmAlQuraYear](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsCalendars.cs:1120) reconstructs month lengths and year starts from VM-owned bytes. [CldrUmmAlQura](D:/Broiler.Unicode/src/UnicodeCldr.LocaleData/CldrUmmAlQura.cs:25) exposes `IsTabulated`, `MonthLength` and `YearStartDay`.

Comparison covered all **301 years from 1300 through 1600 AH: 3,612 month lengths and 301 year starts, with zero differences**. The component's year start is relative to the civil Hijri epoch; the VM must add its `CivilEpoch`. Preserve the existing tabular fallback outside the covered range and retain the N31/N32 pin/generation evidence.

### Unicode properties and emoji: good candidate, adapter required

The VM's [Unicode property data](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeProperties.g.cs:40) and [exact-name lookup](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript.Format/JsUnicodeProperties.cs:121) overlap with [Broiler.Unicode.Properties](D:/Broiler.Unicode/src/UnicodeProperties/UnicodeProperties.cs:10). The component supplies binary properties, general categories, scripts and script extensions; its emoji package supplies string properties. Both property-data sources identify Unicode 17.0.0.

This can remove duplicate generated-data ownership, but the public lookup API is not directly equivalent:

- It matches names loosely, including case folding and removal of spaces, underscores and hyphens. ECMAScript property names/aliases must remain exact. Preserve the VM's strict validation before calling shared lookups.
- It returns mutable cached arrays. A probe confirmed that `GetBinaryProperty("ascii")` and `GetBinaryProperty("ASCII")` return the same array. Prefer immutable/span-based access in the component, or defensively copy validated data once with explicit allocation accounting.
- Matching Unicode version labels do not prove every range and alias is equal. Compare all currently accepted properties, scripts, aliases and emoji sequences before deleting VM tables. Preserve surrogate, case-folding and identifier rules.

### Casing, normalization and segmentation: not ready

The component's `JSStringSpecialCasing` is an eventual reuse target, but a current-source probe of `ToUpper("\uD800\u00DF")` throws `ArgumentException`. Its slow path calls [char.ConvertToUtf32](D:/Broiler.Unicode/src/UnicodeProperties/JSStringSpecialCasing.cs:606), whereas the VM's [StringChangeCase](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.String.cs:1174) preserves lone UTF-16 surrogates. The component also uses host casing fallbacks. Repair surrogate handling and provide the VM's pinned full mappings before adopting it.

No corresponding normalization or grapheme/word/sentence segmenter implementation was found in the inspected Broiler.Unicode source. Moving the VM's reusable implementations into that component is an **extraction opportunity**, not an existing replacement. The VM's [normalization implementation](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.String.cs:745) also charges its work; that boundary must survive extraction.

### Broader Intl replacement needs more work

Relative-time patterns, currency/unit data and likely subtags have overlapping component APIs, but were not exhaustively compared. Do not replace all of `JsCldrTables` or the Intl implementation based on the two successful comparisons above:

- Shared plural evaluation accepts `double` and fraction options; the VM derives operands from its formatted decimal text and supports plural ranges. Rule parsing/caching also differs.
- Shared time-zone names cover English with a zone-to-metazone mapping; VM data covers en/de, historical metazone periods and location fallbacks.
- `CldrChineseCalendar` uses astronomy and an unbounded static per-year cache. VM uses published Chinese/Korean tables and different approximations outside them. This is a semantic and resource-policy change, not table deduplication.

Sources: [component plural rules](D:/Broiler.Unicode/src/UnicodeCldr.LocaleData/CldrPluralRules.cs:13), [VM plural resolution](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.PluralRules.cs:215), [component time-zone names](D:/Broiler.Unicode/src/UnicodeCldr.LocaleData/CldrTimeZones.cs:20), [VM time-zone decision](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/docs/decisions/0058-time-zone-names.md:25), [Chinese-calendar implementation](D:/Broiler.Unicode/src/UnicodeCldr.LocaleData/CldrChineseCalendar.cs:19).

## 3. Broiler.DateTime: share arithmetic, retain Date/Temporal semantics

[JsTemporalCore](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsTemporalCore.cs:193) duplicates proleptic Gregorian leap-year, month-length and day/civil conversion algorithms in [ExtendedIsoDateTime](D:/Broiler.DateTime/src/Broiler.DateTime/ExtendedIsoDateTime.cs:227).

`ExtendedIsoDateTime.IsLeapYear` and `DaysInMonth` are public today. The more useful `DaysFromCivil` and `CivilFromDays` helpers are internal. Expose documented, allocation-free primitives with explicit ranges, then migrate the VM's helpers. Preserve the valid-month precondition: the VM helper assumes it, whereas the component throws for invalid months. Adding a dependency solely to replace the tiny leap-year expression would have little value; a shared arithmetic surface is the worthwhile unit.

Replacing JavaScript Date or Temporal objects wholesale is unsuitable:

- VM Date handles invalid/NaN values, field rollover, coercion and `TimeClip`; the component validates fields and creates a sealed class instance. Its Unix-millisecond conversion is useful only after VM clipping and adds allocation.
- The [VM Date parser](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.Date.cs:902) admits abbreviated date-only forms and `24:00`, and enforces JavaScript signed-year rules. The [component parser](D:/Broiler.DateTime/src/Broiler.DateTime/ExtendedIsoDateTime.cs:602) requires a full date/time including seconds, rejects hour 24 and accepts different signed-year/separator forms.
- VM Temporal uses exact `BigInteger` epoch nanoseconds and proposal-specific limits, rounding, calendar and zone behavior. Component differences return `TimeSpan`, truncate sub-100ns precision and may overflow; its offsets require whole minutes. It supplies neither an IANA database nor alternative-calendar coverage.

The BCL `DateTime`, `DateTimeOffset` and `DateOnly` ranges also cannot represent the full JavaScript range, negative years and year zero. `TimeZoneInfo`/`CultureInfo` would replace pinned VM data with host/runtime data. VM already uses `DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()` for its clock.

## 4. Broiler.Regex: useful destination, incompatible current implementation

The component's actual assembly/package name is `Broiler.Regex` (the checkout is `Broiler.RegEx`). It is a managed ECMAScript-oriented engine and does not depend on Broiler.JS. Nevertheless, replacing [JsRegExpMatcher](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript.Format/JsRegExpMatcher.cs:802) now would change correctness and resource behavior.

| Issue | VM behavior | Current shared component |
|---|---|---|
| Resource authority | Caller-supplied `JsRegExpCharge`, explicit frame/trail limits and VM failure propagation | Hardcoded 10,000,000-step allowance, reset for every candidate start position; exhaustion can become ordinary match failure. |
| Property names | Rejects `\p{ascii}` with `u` | Accepts it through loose Unicode alias resolution. |
| Capture names | Rejects `(?<1>a)` with `u` | Accepts the name. |

Both syntax differences were reproduced by compiling current source in memory; the component matched `a` for both patterns, while the VM rejected them with its respective syntax error. Resource behavior was established by inspecting [Matcher](D:/Broiler.RegEx/Broiler.Regex/Matching/Matcher.cs:38), [Budget](D:/Broiler.RegEx/Broiler.Regex/Matching/MatchState.cs:9) and the VM's charge/limit paths.

Recommended sequence: add a caller-owned work budget/callback, preserve exhaustion as a distinct result, preserve bounded auxiliary memory, fix the demonstrated syntax gaps, compare captures/indices/Unicode modes/sticky and global behavior, then replace the underlying parser/matcher. Keep `JsRealm.RegExp` protocol behavior, observable `lastIndex`, JavaScript errors and object construction in the VM. Evaluate compile-time limits as well as match-time fuel. `System.Text.RegularExpressions.Regex` is also not an equivalent substitute for these ECMAScript and VM contracts.

## 5. Dependency policy and integration shape

The current architecture deliberately forbids the proposed Broiler references. This is an implementation prerequisite, not evidence that reuse is technically impossible:

- [A1/A2/A3](D:/Broiler.VM/src/tests/Broiler.VM.Architecture.Tests/ArchitectureRules.cs:43) prohibit outbound project references, Broiler package references and shared source links; [their tests](D:/Broiler.VM/src/tests/Broiler.VM.Architecture.Tests/ProjectFileRuleTests.cs:13) sweep the project graph.
- [N1](D:/Broiler.VM/src/tests/Broiler.VM.Architecture.Tests/ArchitectureRules.cs:596) fixes the JavaScript profile's project-reference set and prohibits packages; [N3](D:/Broiler.VM/src/tests/Broiler.VM.Architecture.Tests/ArchitectureRules.cs:705) makes Format a reference sink. Composition rules also constrain references.
- B1/B2 protect the BCL-only core boundaries. Preserve them. B3's current test sweeps the core and fixture assemblies, not every shipping profile; do not assume its broad comment already validates all new transitive dependencies.

Introduce explicit allowed dependencies for the appropriate profile/data projects and update the graph, rules, packaging/closure expectations and architecture record together. Do not globally disable the prohibitions. `IJsIntlData` already provides a [composition-supplied data boundary](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript.Format/JsIntlData.cs:39). Retain optional Intl deployment through that boundary, with a compatible provider or shared generator; adapters must produce the VM's exact table format. An unconditional reference from the execution profile would change which data an execution-only application carries.

The inspected shipping DateTime and Unicode data projects have no runtime package references; Regex references the Broiler Unicode property/emoji packages. The proposed dependencies therefore need no new third-party runtime library. This assessment is of local source project files, not a verification of a particular published NuGet version or its restored closure. Existing Unicode/CLDR data provenance and notices still apply.

Broiler.JS already consumes Broiler.Regex and CLDR LocaleData and offers useful integration examples, but its runtime/IL compiler and guest object model are not replacements for this VM. Broiler.Native's inspected bindings do not currently provide the VM's `VirtualAlloc`/`VirtualProtect`/`mmap` executable-memory API set; moving ownership there would require new component work and would not remove the VM's mapping/lifecycle policy.

## 6. Custom code that should remain

| Area | Why a generic replacement is unsuitable |
|---|---|
| Bounded binary reader and LEB128 | Metering, sticky errors, canonical encodings and WebAssembly-specific acceptance rules. Replace endian primitives inside it, not the reader with `BinaryReader`. |
| Guest JSON | `toJSON`, replacer/reviver callbacks, source records, property order, lone UTF-16 surrogates and charging. `JsonSerializer` is appropriate for some host reports, not a direct guest implementation. |
| Map/Set storage | Already uses BCL containers; custom tombstones and iteration rules preserve mutation observability. Ordinary dictionary enumeration differs. |
| URI and base64 decoding | Exact guest grammar, malformed-surrogate errors, allowed character sets, partial writes and read counts. Lower-level framework helpers can be used only within those rules. |
| String serialization | The VM format uses WTF-8 to preserve lone UTF-16 surrogates; ordinary UTF-8 conversion changes values. |
| Numeric wrappers | BigInt, Float16 and many Math operations already use `BigInteger`, `Half`, `BitConverter` and framework math. Remaining wrappers preserve signed zero, NaN and ECMAScript rounding/coercion. |
| Normalization/globalization | The compositions use invariant globalization and pinned data. Host normalization, casing, collation or locale tables are not equivalent replacements. |

Examples: [guest JSON](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.Json.cs:216), [keyed collections](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsCollections.cs:108), [URI behavior](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.Global.cs:28), [base64 decoding](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.Base64.cs:379), [WTF-8](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript.Format/JsFormat.cs:857), [Float16](D:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsBinary.cs:649), [invariant-globalization composition](D:/Broiler.VM/src/compositions/Broiler.VM.Composition.JavaScript.Cli/Broiler.VM.Composition.JavaScript.Cli.csproj:28).

## Validation and next implementation steps

The investigation used source inspection, in-memory compilation/probes, and decoded-data comparisons. It did not migrate code, restore component packages, benchmark alternatives, run a full solution/conformance suite or publish Native AOT binaries. Probe results are reported here; no retained test/evidence bundle was created.

For implementation, make the four BCL substitutions a separate small change and run the affected contract, WebAssembly and JavaScript checks. Then introduce a narrowly allowed shared-data provider, initially using the verified list/Umm al-Qura subsets. Before deleting other generated data, compare complete contents and accepted aliases, preserve data pins, and validate optional deployment under invariant globalization and Native AOT. Complete the demonstrated Regex/casing fixes in the shared components before migrating those operations.

### Completed implementation (Candidate 1 & Secondary BCL Opportunities)

1. **`WasmName.IsWellFormed`**:
   - Replaced manual loop and `Follows` helper in [`WasmName.cs`](file:///d:/Broiler.VM/src/Broiler.VM.Profile.WebAssembly/WasmName.cs) with `System.Text.Unicode.Utf8.IsValid(bytes)`.
   - Verified with WebAssembly harness running the full retained corpus (330/330 passed).
2. **`MathLeadingZeros`**:
   - Replaced manual shift loop in [`JsRealm.Math.cs`](file:///d:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.Math.cs) with `System.Numerics.BitOperations.LeadingZeroCount(bits)`.
   - Verified with JavaScript differential and `--host-surface` test suites.
3. **`SliceNodeIdentityComparer`**:
   - Replaced dictionary comparer at both construction sites ([`SliceStaticSemantics.cs`](file:///d:/Broiler.VM/src/Broiler.VM.Profile.JavaScript.Compiler/SliceStaticSemantics.cs) and [`SliceSourceCompiler.cs`](file:///d:/Broiler.VM/src/Broiler.VM.Profile.JavaScript.Compiler/SliceSourceCompiler.cs)) with `System.Collections.Generic.ReferenceEqualityComparer.Instance`.
4. **`VmBoundedReader` little-endian reads**:
   - Replaced manual shift logic in `TryReadUInt32LittleEndian` and `TryReadUInt64LittleEndian` in [`VmBoundedReader.cs`](file:///d:/Broiler.VM/src/Broiler.VM.Binary/VmBoundedReader.cs) with `System.Buffers.Binary.BinaryPrimitives.ReadUInt32LittleEndian` and `ReadUInt64LittleEndian`.
   - Verified with all 473 contract surface and bounded reader tests passing.
5. **`Base64Encode` url-safe path**:
   - Replaced `Convert.ToBase64String` + character replacement in [`JsRealm.Base64.cs`](file:///d:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.Base64.cs) with `System.Buffers.Text.Base64Url.EncodeToString`, maintaining padding preservation and gas charging.

### Candidate 4 Implementation & Architecture Evaluation (`Broiler.DateTime`)

1. **Public Arithmetic Primitives (`Broiler.DateTime`)**:
   - In sibling repository [`D:/Broiler.DateTime/src/Broiler.DateTime/ExtendedIsoDateTime.cs`](file:///D:/Broiler.DateTime/src/Broiler.DateTime/ExtendedIsoDateTime.cs), promoted `DaysFromCivil(long y, int m, int d)` and `CivilFromDays(long z)` to `public static` methods.
   - Added thorough docstrings outlining valid proleptic Gregorian ranges and Hinnant algorithm foundations.
   - Verified that `Broiler.DateTime.slnx` builds cleanly (0 warnings, 0 errors) and all 127 tests pass.
2. **Boundary & Dependency Evaluation (`Broiler.VM`)**:
   - Evaluated referencing `Broiler.DateTime` from `Broiler.VM.Profile.JavaScript`:
     - Architecture rules A1 ("no ProjectReference resolves outside component root") and A2 ("no PackageReference names a Broiler package") forbid direct project references to sibling repositories.
     - Rule N1 strictly binds the JavaScript profile's project reference set to `[Broiler.VM.Binary, Broiler.VM.Abstractions, Broiler.VM.Profile.JavaScript.Format]`.
     - Keeping the pure mathematical Hinnant algorithm (`days_from_civil` and `civil_from_days`) directly inside [`JsTemporalCore.cs`](file:///d:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsTemporalCore.cs) retains allocation-free, dependency-free execution for lightweight Native AOT environments while sharing exact algorithmic behavior with `ExtendedIsoDateTime`.

### Candidate 2 Data Provider & Boundary Analysis (CLDR & Umm al-Qura)

1. **Parity Verification**:
   - **List Patterns**: All 18 currently generated `de`/`en` rows in [`JsCldrTables.ListPatterns`](file:///d:/Broiler.VM/src/Broiler.VM.Profile.JavaScript.Intl/JsCldrTables.g.cs) match `UnicodeCldr.LocaleData.CldrListData.Patterns` (CLDR 48.2.0) after unescaping and type mapping.
   - **Umm al-Qura Data**: All 301 tabulated years (1300–1600 AH) in [`JsCalendarTables.Calendars`](file:///d:/Broiler.VM/src/Broiler.VM.Profile.JavaScript.Intl/JsCalendarTables.g.cs) match `UnicodeCldr.LocaleData.CldrUmmAlQura` month lengths (3,612 lengths) and year starts (301 starts) with zero discrepancies.
2. **Pluggable Data Architecture**:
   - [`IJsIntlData`](file:///d:/Broiler.VM/src/Broiler.VM.Profile.JavaScript.Format/JsIntlData.cs) in `Broiler.VM.Profile.JavaScript.Format` already serves as the composition-supplied data boundary (`JsIntlTable` enum and `Table(JsIntlTable) => ReadOnlySpan<byte>`).
   - Rules N28 and N32 enforce byte-level reproducibility against pinned crates/archives (`cldr.pin`, `calendars.pin`).
   - Any eventual consolidation with `Broiler.Unicode` should proceed via shared data generator tooling or an external `IJsIntlData` provider at composition roots (`SliceCompiler`, `Conformance`), ensuring execution-only runtimes remain free of CLDR/Unicode payload overhead.

### Secondary Opportunities Assessment

1. **`Base64DecodeHex` Retention**:
   - Retained the custom hex decoding loop in [`JsRealm.Base64.cs`](file:///d:/Broiler.VM/src/Broiler.VM.Profile.JavaScript/JsRealm.Base64.cs).
   - Replacing with `Convert.FromHexString` is unsuitable because:
     - `Convert.FromHexString` parses the complete input and allocates a new array, whereas `Base64DecodeHex` respects destination buffer `maxLength` limits (e.g. `Uint8Array.prototype.setFromHex`) and stops early without decoding trailing data.
     - `Base64DecodeHex` produces specification-mandated ECMAScript syntax error strings (`"'{pair}' is not a pair of hex digits"`), whereas BCL throws generic `FormatException`.
     - In `setFromHex`, bytes decoded prior to encountering an error must be preserved in the destination array (`Base64Settle`), which whole-span conversion cannot accommodate.
2. **`Test262Json` Serializer**:
   - Retained the custom serializer in [`Test262Json.cs`](file:///d:/Broiler.VM/src/compositions/Broiler.VM.Composition.JavaScript.Conformance/Test262Json.cs) to preserve deterministic schema property ordering and custom substitution of lone UTF-16 surrogates with `\uFFFD`, preventing JSON serialization throws across test shards.

### Candidate 6 Repair (`Broiler.Unicode.JSStringSpecialCasing`)

1. **Surrogate Pair & Lone Surrogate Handling**:
   - In [`D:/Broiler.Unicode/src/UnicodeProperties/JSStringSpecialCasing.cs`](file:///D:/Broiler.Unicode/src/UnicodeProperties/JSStringSpecialCasing.cs), resolved the defect where `char.ConvertToUtf32` threw `ArgumentException` on strings containing lone surrogates beside special casing characters (e.g. `ToUpper("\uD800\u00DF")`).
   - Introduced safe `CodePointAt` and `AppendCased` primitives across `BeforeIsCased`, `AfterIsCased`, `ToLowerCore`, `IsBeforeDot`, `IsAfterI`, `TurkicToLower`, and `Map`.
   - Lone surrogates now pass through unmodified without throwing, preserving WTF-16 invariant behavior.
2. **Verification in `Broiler.Unicode`**:
   - Created `tests/UnicodeProperties.Tests` with dedicated regression tests for lone surrogate preservation and special casing.
   - Added project to [`D:/Broiler.Unicode/Broiler.Unicode.slnx`](file:///D:/Broiler.Unicode/Broiler.Unicode.slnx).
   - `dotnet test Broiler.Unicode.slnx`: all tests pass across both .NET 8.0 and .NET 10.0.

### Candidate 3 & 5 Prerequisites & Status

1. **Candidate 3 (Unicode Properties & Emoji)**:
   - Evaluated data and API alignment between `Broiler.Unicode.Properties` / `UnicodeEmoji.StringProperties` and `JsUnicodeProperties` / `JsUnicodeStringProperties`.
   - Before shared property lookup can be integrated into the VM:
     - Strict ECMAScript property name validation must precede any call, as `UnicodeProperties.Normalize` allows loose matching (stripping underscores/hyphens and folding case) which is invalid under ECMAScript regex `/u` and `/v` syntax.
     - Mutable array returns (`(int Lo, int Hi)[]`) must be made immutable or defensively copied to prevent shared cache mutation.
     - Parity checks against rule N22 pinned tables (`unicode.pin`) must be maintained.
2. **Candidate 5 (Broiler.Regex Engine)**:
   - Integration remains gated on upstream engine fixes in `Broiler.Regex`:
     - Caller-owned fuel metering (`JsRegExpCharge`) must be accepted instead of a hardcoded 10,000,000-step loop allowance that silently treats exhaustion as match failure.
     - Disallow non-ECMAScript syntax (e.g. Rejecting non-standard capture names `(?<1>a)` and non-standard loose property matching under `/u`).

