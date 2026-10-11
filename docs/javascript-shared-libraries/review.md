# Review of the feasibility report

**Date:** 2026-10-11. **Subject:**
[`docs/javascript-shared-libraries-feasibility.md`](../javascript-shared-libraries-feasibility.md)
(commit `88d8b51d`).

**Scope of the review.** Every claim was checked against the source, at these revisions:

| Repository | Revision |
|---|---|
| Broiler.VM | `88d8b51d` |
| Broiler.JS | `44a214dc` |
| Broiler.Unicode | `2affa78f` |
| Broiler.RegEx | `c1422f4d` |
| Broiler.DateTime | `8de12cc3` |
| Broiler.JSeal | `6ca0ddd`; it moved during the review |

**Method.** The review read source, records and build files only. Nothing was built, run or modified
in any repository. A statement about what a script would observe is a reading of the code, to be
confirmed by the vectors of SJL-1.

**Later on 2026-10-11** the owner took every decision point this review led to
([decisions.md](decisions.md)). The review's text is unchanged. Where it calls a decision
recommended or a question open, decisions.md records the choice.

**Verdict.** **The report's direction holds.** Its central recommendations survive the review: share
language-level algorithms and data, keep execution machinery per engine, start with number
conversion, design the front end as a contract before moving code, and validate against the
specification and test262 rather than against each other. **Its catalogue of obstacles is
incomplete.** It names the architecture rules but not the plan-level positions, prior decisions,
contract-bearing refusals and deadlines that would stop the programme first. It does not know three
facts that shape the plan: published component libraries already own two of its areas, a third
engine-neutral layer constrains package names, and the two engines' data sets are measurably
different. This document lists each claim with its verdict, then what the report left out, then
what the review found stale elsewhere.

---

## 1. Claims, one by one

| # | Claim (short quote) | Verdict | Evidence and consequence |
|---|---|---|---|
| 1 | "Broiler.JS can become the home of reusable JavaScript libraries, with its current engine remaining an independently usable public reference implementation" | **Supported, with conditions** | Technically yes. But Broiler.JS's own records say "No Broiler.VM integration is planned for this component, and none changes that statement" (`docs/public-api.md`), and Broiler.VM's records say the reverse of every edge the programme needs. Both must be revised first (SJL-0). Two of the report's areas already have published owners outside Broiler.JS: Broiler.Regex for regular expressions, and Broiler.Unicode for Unicode and CLDR data. The recommended homes are mixed (decision `SJL-D-1`). |
| 2 | "shared language processing and algorithms, with each engine retaining its own execution machinery" | **Confirmed** | Most of Broiler.VM's algorithm code already references no engine type. Number formatting, Temporal's core, calendars, time zones, locale tags, plural rules and the pattern generators take or return no engine value, or use the engine only to raise errors or charge fuel. |
| 3 | "Broiler.JS already separates its parser, AST, runtime, compiler, built-ins and distribution packages" | **True of packaging; not of layering** | The packages are separate. But `Parser` references `Runtime` and reads its internals through `InternalsVisibleTo`. Every assembly rests on `Expressions`. `Compiler` and `BuiltIns` reference `Engine`. Number formatting reaches the runtime through a delegate the built-ins install. |
| 4 | "Its limited portable bytecode compiler reuses `FastParser` and the AST" | **True** | `PortableCompiler.Compile` constructs `FastParser` directly. Two qualifications: it does so outside any `CoreScript` scope, so it always parses as a script; and it **runs no early-error validation**. Its closure includes `Runtime`. |
| 5 | "Its architecture document also proposes a shared frontend … That proposed separation is not yet complete." | **True, and understated** | The proposed `FrontEnd/Semantics` boundary is "Shared by IL and bytecode", meaning **Broiler.JS's own** two back ends. It is a hypothesis pending its own MOD-M2 work, not started. The same document says nothing there plans a Broiler.VM integration. `Assemblies.md`, `Roadmap.md` and `ModernizationDelivery.md` still plan one, which contradicts it. |
| 6 | "Broiler.VM separately implements tokenization, parsing, syntax trees, JavaScript semantics and standard-library algorithms" | **True** | Both of Broiler.VM's front ends were written in its own checkout. The planned snapshot copy of Broiler.JS (its milestone JS-2) **was never taken**. |
| 7 | Extraction-candidate table | **Broadly confirmed; incomplete** | It misses four areas: BigInt, Math and Float16 (SJL-4); the text algorithms (URI, base64 and hex, JSON lexing, substitution templates; SJL-5); the Intl *algorithms*, as distinct from Intl *data* (SJL-9); and the duplication inside each engine. The rows on regular expressions and Unicode should have named their existing owners. |
| 8 | "VM's `JsNumberFormat` already operates on ordinary CLR values and uses only `System` dependencies" | **True of that file**, with four qualifications | The class is `internal`, so the compiler carries its own copy, `JsParser.NumberText`. `toFixed`, `toExponential` and `toPrecision` are not in it; they are private statics of `JsRealm.Number`. Its results depend on the .NET runtime's `"R"` format and on the runtime's Unicode data for the `Zs` category. It has no meter inside. |
| 9 | "Broiler.JS implements equivalent responsibilities in `NumberParser` and methods inside `JSNumber`"; "`ToJsString(double)` … `ToECMAString(double)`" | **Partly** | The members exist. The responsibility is also spread across `JSNumberPrototype`, the parser's own `NumberCoercion`, two `parseInt`/`parseFloat` wrapper pairs, the `JSValue.NumberToECMAString` delegate, and several whitespace sets. "Equivalent" holds for responsibility, not for algorithm. |
| 10 | "The algorithms are not identical today" | **True** | They differ in shortest digits, radix fractions, the fixed, exponential and precision rounding route, and parsing beyond 2^53. Readings suggest defects on **both** sides; see [areas/numbers.md](areas/numbers.md#4-differences-already-visible). |
| 11 | "Neutral `string` or `ReadOnlySpan<char>` inputs would also avoid importing Broiler.JS's AST-owned `StringSpan`" | **True** | `StringSpan` is defined in the `Broiler.JavaScript.Ast` assembly and appears in public `NumberParser` signatures. |
| 12 | "Its project currently references Broiler.JS Runtime, and parsing reads some options through ambient `CoreScript` state" | **True; there is more** | The parser uses exactly three `Runtime` types: `CoreScript`, `CancellableDisposableAction` and `RegExpUnicodeValidator`. It also names desugaring temporaries from a **process-wide static counter**, and writes a shared static end-of-file token on every parse, which looks like a cross-parse data race. |
| 13 | "Early-error checks are distributed between parsing and compilation" | **True, in both engines** | Broiler.JS: the parser, `SyntaxValidation` (which re-tokenises the source), `FastCompiler.EarlyErrors`, `Visit*` methods and `DirectEvalSupport`. **Broiler.VM's wide path also splits them**, between `JsParser` and the lowering. |
| 14 | "Some AST information is mutable and adjusted during compilation" | **True** | Also: the parser **desugars `for` loops while parsing**. |
| 15 | "VM has separate slice and wide parsers, sharing a tokenizer but using different trees" | **True** | They also share span, token, declaration-kind, diagnostic and option types. The wide parser re-tokenises template substitutions. |
| 16 | "VM carries explicit nesting limits, syntax-tree limits, feature admission and structured diagnostics" | **True** | Add rule N12 (no ambient state in the compiler) and rule N19 (the tree-depth bound is a constant the host cannot raise). Both bind the shared front end's design. |
| 17 | "`JsCompiler.Hoisting.cs`, where declaration analysis and instruction emission currently coexist" | **True** | The same holds in the evaluation, function, class, exception and statement parts. The wide path's hoisting order relies on dictionary insertion order, which backend stage JSB-1 records. |
| 18 | "Broiler.JS represents values through a public `JSValue` class hierarchy … VM uses an internal tagged `JsValue` struct" | **True** | |
| 19 | "This arrangement preserves VM's existing ability to deploy an execution-only composition without a compiler" | **True only if a rule enforces it** | Today's closure collectors filter by name prefix only (`System.`, `Microsoft.`). An admitted package would arrive unexamined. The programme adds rule `I3` on published closures. |
| 20 | "VM uses pinned Unicode tables, while Broiler.JS currently calls platform normalization" | **True; the picture is mixed on both sides** | Broiler.VM still consults the runtime for `Zs` in its number whitespace and `RegExp.escape`, and for letters in one function-name check. Broiler.JS's regular-expression properties already come from Broiler.Unicode's tables, which are UCD 17.0.0 and identical to Broiler.VM's apart from one script alias. Broiler.JS also normalises through the platform in its collator. |
| 21 | "Equivalence of the current time-zone datasets was not established" | **Now established: they are not equivalent** | The datasets differ in release (2026e against 2025b); backzone; extrapolation beyond the last listed transition (Broiler.JS freezes after 2037); primary identifiers; and how the data is produced (pinned and compiled in Broiler.VM, copied from a build host in Broiler.JS). See [areas/time-zones-and-calendars.md](areas/time-zones-and-calendars.md). |
| 22 | "Broiler.JS already consumes standalone Regex and Unicode/CLDR packages" | **True, partly in practice** | `Broiler.Regex` handles only `u`- and `v`-flag patterns and a few gap shapes. Every other pattern goes through a .NET translator, with a silent fallback when Broiler.Regex refuses a pattern. `Broiler.Unicode.Properties` and the emoji package arrive only **transitively**, although Broiler.JS calls them directly. `Broiler.DateTime` was dropped on 2026-09-18. |
| 23 | "VM's Temporal parser accepts a `JsEngine`, while Broiler.JS's Temporal string helpers include `JSValue` coercion and runtime-created errors" | **True as worded; misleading on both sides** | Broiler.VM's parser uses its engine **only to construct `RangeError`s**; its input is a string and its output a neutral record. `TemporalIsoString` is **not** Broiler.JS's real parser. Each Temporal type has its own private regular-expression parser, which returns engine objects and throws; one reads options part-way through parsing. |
| 24 | "VM's BigInt implementation already demonstrates a useful pattern" | **True** | The remark reads: "The charge goes to a delegate so that the arithmetic holds no engine and can be judged on its own". |
| 25 | "VM currently rejects external project references, Broiler package dependencies and externally linked source files. Its profile rules also constrain the permitted reference set." | **True, and stronger** | Rule N1 rejects **any** package reference in the profile, not only Broiler ones. Rules A12 and A13 do the same for composition roots and consumer profiles. |
| 26 | The cited rules: "A1, A2 and A3, plus the JavaScript profile reference rules" | **Incomplete** | Rules A2 and N1 fail at once. Rules K4 and N37, and the pristine consumer's restore, fail at the next evidence collection. Rules B3, B4, B5, B5b, B5c, K5, J1 to J12, N12, N18, N19 and N23 would **pass silently**, because they scan only Broiler.VM's own sources and binaries, so code moved into a package escapes them. The data-pin rules N22, N27, N28 and N29 to N32 lose their subjects. (Under `SJL-D-6` (c), taken later that day, they keep them.) Rules A7 and the graph manifest cannot express a package edge. See [governance.md](governance.md#31-broilervm). |
| 27 | "These are current design decisions, rather than technical impossibilities" | **True, with one exception** | Some are written into a **contract-bearing** record. ADR 0011's standing refusals include "A shared syntax tree or parser — Refused permanently". Editing that would be an amendment, and the amendment procedure is unexecutable while one person holds every co-signing role (EX-15). The programme takes a scope reading instead (decision `SJL-D-8`). |
| 28 | "Broiler.JS's architecture documentation explicitly describes the repositories as independent" | **True** | Also in `public-api.md` and the roadmap README ("The bytecode/VM plan is retired from this component"). Contradicted by residual text in three roadmap documents. |
| 29 | "Versioned packages would provide a clearer cross-repository contract than copying or linking source files" | **True** | Three facts it should add. Both repositories restore from nuget.org only. A process hosting both engines resolves **one** version of each shared package. Broiler.JS's packing gives every package one version today. |
| 30 | "Its `All` and `Minimal` packages already serve as compositions" | **True** | Both are dependency-only meta-packages. Broiler.JS documents `Minimal` as non-conformant. |
| 31 | "Moving public types between assemblies would need a separate compatibility decision" | **True** | Broiler.JS has no type forwarding anywhere, no API baseline and no package validation. Its own Modernization invariant 7 and MOD-M0-4 leave the question open (decisions `SJL-D-12`, `SJL-D-25`). |
| 32 | "VM already has a differential harness that can run probes against Broiler.JS" | **True, with qualifications** | It runs whole programs in separate processes and compares their printed output, so it cannot test an algorithm in isolation. It is run by hand; CI runs only its smoke fixtures, against Node. Its Broiler.JS declarations are pinned to older Broiler.JS commits, and its last Broiler.JS run left many findings unadjudicated. It is a cross-check, never an oracle, which the report itself says. |
| 33 | "Focused measurements of allocations, execution speed, parsing, startup and deployed size" | **Needs a constraint** | Broiler.VM's JavaScript profile forbids "comparison against any other engine or component, in either direction" (release gate 10). Every measurement must compare one engine with itself (validation principle 6). |
| 34 | "No percentage reduction or delivery estimate is justified by this inspection" | **Confirmed, and kept** | The roadmap carries no figure, under Broiler.VM's rule for plans. |

---

## 2. What the report left out

1. **The plan-level positions that forbid the programme.**
   - Broiler.VM's roadmap, sections 5, 9, 12, 14, 15 and 16: "Broiler.VM references no legacy Broiler
     component, in either direction"; "fixes do not flow across the fork"; "Copying is allowed;
     depending is not".
   - The JavaScript profile's invariant 11, "The fork is one-way and mechanical", its gate 2, and its
     stop row.
   - The core ledger's "none of its milestones wait for it".
   - The README's "Relationship to Broiler.JS".

   These are changed by records, not by rules ([governance.md](governance.md#2-positions-this-programme-reverses-or-narrows)).
2. **Decisions already taken against sharing.**
   - JSD-0031 §4(b) refused `Broiler.Unicode.Properties` for five reasons.
   - JSD-0027 §4 recorded `UnicodeCldr.LocaleData` as "Candidate input, not accepted".
   - JSD-0014 rejected the seed's split early errors and ambient parse state.
   - JSD-0011 rejected the seed's value hierarchy.
   - JSD-0005 set the seed snapshot's stop condition.

   Each is revisited by a named milestone, never ignored.
3. **The JS-2 deadline.** Broiler.VM's profile planned to *copy* Broiler.JS. JSD-0005 takes that copy
   by default on 2026-11-30. Sharing makes the copy pointless, so the re-scope (decision `SJL-D-7`)
   is time-critical.
4. **The existing component libraries** and their readiness gaps.
   - Broiler.Regex: a budget reset per start position; exhaustion read as "no match", or as a shorter
     match; no cancellation; recursion without guards; loose names.
   - Broiler.Unicode: loose names; mutable shared arrays; unpinned, unhashed inputs; no
     normalization, segmentation or folding; host fallbacks in casing.
   - Broiler.DateTime: no consumer, and internal primitives.
5. **Broiler.JSeal.** It is a host-facing engine abstraction *above* both engines. Its package gate
   forbids either engine's package family appearing in the other's graph, which constrains how
   shared packages may be named (decision `SJL-D-2`).
6. **Co-hosting.** NuGet's single-version resolution in a process that hosts both engines makes
   binary compatibility mandatory. It also makes **data-version drift** a real hazard.
7. **Concrete data-version skew:**
   - the tz database, 2026e against 2025b;
   - normalization from the host's ICU, or a no-op under invariant globalization;
   - CLDR coverage: a small set of complete locales against a wider set of partial languages plus host cultures.
8. **Duplication inside each engine.** It is part of the value:
   - Broiler.VM's compiler copies of number rendering, BigInt parsing and JSON reading;
   - Broiler.VM's separate Gregorian implementations;
   - Broiler.JS's number conversion in its parser, runtime and built-ins;
   - Broiler.JS's per-type Temporal helper copies;
   - Broiler.JS's two regular-expression engines.
9. **Assurance.** Code that leaves Broiler.VM leaves its J rules. Broiler.JS's equivalent scheme is
   configured but partial and unenforced.
10. **Licensing provenance.** Some Broiler.JS units cite a third-party engine or a Stack Overflow
    answer, and its notices name only Yantra JS.
11. **Security.** Every shared parser, matcher and codec processes untrusted input and must be
    metered and bounded.
12. **The cross-engine performance-comparison prohibition** (claim 33).
13. **The universal bytecode programme.** Profile decision JSD-0036 already says "the front end
    returns a validated tree; the lowering consumes it and emits universal bytecode".
14. **The two test262 runners are not comparable.** They pin the same revision but differ in
    variants, negative tests, admitted proposals, host hooks and allowances.
15. **Diagnostics.** Broiler.VM's conformance runner grades refusals by registry code, and its guests
    see a generic `SyntaxError`. Broiler.JS's guests see V8-style text. A shared front end must
    carry codes **and** message identifiers.

---

## 3. Findings about the sibling report

[`docs/component-reuse-investigation.md`](../component-reuse-investigation.md), of the same date, is
used by this programme where its findings hold:

- the list-pattern and Umm al-Qura parity checks;
- the Broiler.Regex syntax and budget findings;
- the dependency-policy summary.

Four of its findings do not hold, or need correcting:

1. **"Implemented in `Broiler.DateTime` (public primitives exposed & verified)" is false for every
   committed state.** `DaysFromCivil` and `CivilFromDays` are `internal` at Broiler.DateTime's head
   and on every branch. The change that makes them public exists only in an uncommitted stash of
   that checkout.
2. **Its line numbers match older checkouts** of Broiler.DateTime and Broiler.RegEx. The behaviour it
   describes is unchanged at head.
3. **It does not mention Broiler.VM's JSD-0031 and JSD-0027**, which had already evaluated and
   declined the same components.
4. **Broiler.Unicode's casing repair is real and published** in preview.3. The other half of the
   report's recommendation, removing the host casing fallbacks, is not done.

---

## 4. Stale or contradictory records noticed during the review

These were seen in passing. The programme fixes them only where a milestone touches them anyway.
They are listed so that nobody plans against them.

**Broiler.VM**

- Rules N29 to N32 (tz database and calendars) have tests but **no register rows**.
- The publish workflow does not run the assurance release gate that `CODE-ASSURANCE.md`,
  `HUMAN_REVIEW.md` and rule J11 describe. The workflow that ran it was removed on 2026-09-18, and
  the full-lane workflow's header still names it.
- ADR 0001's argument that a single-source `NuGet.config` makes a Broiler package "unreachable" is
  already false, because Broiler.JS publishes to nuget.org.
- `graph.manifest.json` records some packable projects as not packable. Rule N10's statement says the
  family has three assemblies where it lists four, and the profile's API baseline header says the
  same.
- Comments are stale in several places:
  - the test262 harness, on `CanBlockIsFalse`;
  - the regular-expression code, on the `v` flag, the nesting depth and SpecialCasing;
  - `JsParser` and `JsSyntax`, on features "refused" that are parsed;
  - `JsRealm.Date`, on refused date forms.
- In the profile's records:
  - the conformance README says no third-party suite is pinned;
  - an orphan identifier, `JSN-1`, appears in the diagnostic registry;
  - the decision number JSD-0026 is reused in one bundle's rule;
  - the corrections index covers only its first entries;
  - several ledger summary lines are stale.

**Broiler.JS**

- `Assemblies.md`, `Roadmap.md`, `ModernizationDelivery.md` and MOD-M2-7 contradict
  `dependencies.md` and `public-api.md` about a Broiler.VM consumer.
- Broken links: the README links a `Phase-6.md` that does not exist, and `Component.md` links a
  nested regular-expression path that no longer exists.
- `scripts/packaging/test_pristine_consumer.py` expects dependencies BuiltIns no longer has.
- The `Network` and `NodePollyfill` projects do not compile, though `All`'s description promises them.
- The README says conformance is tracked continuously in CI; `CONTRIBUTING.md` says the workflow is
  not automatic.
- `builtins-vs-clr-types.md` still describes a vendored Broiler.DateTime.
- The module-initializer document's table omits one assembly.

**Component repositories**

- Broiler.Unicode's README and build properties say emoji 16.0; its data and assembly carry 17.0.
- Broiler.RegEx's README and publishing notes name an older Broiler.Unicode floor.
- Several repositories' workflows read a secret named `NUGET_TOKEN` where their documents say
  `NUGET_API_KEY`.
- The aggregate checkout's root solution files still reference submodule paths Broiler.JS removed.
- Broiler.JSeal's VM-feature roadmap rows predate Broiler.VM's Intl assembly.
