# Risks and stop conditions

**Date:** 2026-10-11. **Status:** plan. Each risk names where it bites, its mitigation, and its
**stop condition**: the observation that ends a milestone's progress, or the programme's, until it
is resolved. A risk without a stop condition is one the programme accepts, and it says so.

**Revised later on 2026-10-11, when every decision point was taken.** G1, G2, D7, D8, R1 and L2 now
cite the choice taken. D10, D11 and R5 are new: they are the risks of the choices themselves. D7's
stop of the programme is withdrawn, and its row quotes it.

---

## Governance

| # | Risk | Mitigation | Stop condition |
|---|---|---|---|
| G1 | **The core-contract owner rules that ADR 0011's standing refusals cover this programme.** Broiler.VM's amendment procedure is unexecutable while one person holds the co-signing roles (EX-15). | Decision `SJL-D-8`, taken on 2026-10-11, takes the scope reading as a route with its alternative named. A later ruling that reverses it, for example by a second holder, blocks SJL-0, naming the core as the holder. | **Stop:** code written while SJL-0 is blocked on `SJL-D-8` |
| G2 | **JSD-0005's stop condition takes the Broiler.JS seed snapshot by default on 2026-11-30.** The programme would then share code with a fork of itself. **Taking `SJL-D-7` does not prevent this; only its filed JSD does.** | `SJL-D-7` was taken on 2026-10-11 (re-scope). Its JSD is filed by SJL-0.4, and the ledger carries the deadline. If that JSD cannot be filed in time, a JSD revising the stop condition is filed instead. | **Stop:** the date passes with neither the re-scope nor a revised stop condition recorded |
| G3 | **Records made false by implication.** A roadmap sentence, an invariant or an architecture document still says "no edge in either direction" after an edge exists. | Governance's lists V, P, J and C. Register rows are filed `Proposed` at SJL-0 and `Applied` at SJL-2. Every correction quotes the text it replaces. | **Stop:** an admitted package while any row of governance section 2 is unfiled |
| G4 | **One person holds every role** (EX-30), so no gate is independently confirmed | Every record says so. Nothing is accepted, published or claimed on its strength. Under Broiler.VM's MVP terms, unreviewed work may be built and merged, not released. | None. The programme accepts the cost and states it in every bundle. |
| G5 | **Broiler.JS's documents contradict each other** about a Broiler.VM consumer: `dependencies.md` and `public-api.md` say no integration; `Assemblies.md`, `Roadmap.md` and `ModernizationDelivery.md` still plan one. | Reconciled at SJL-0 (governance item J5) | **Stop:** SJL-2 begins with the contradiction unreconciled |
| G6 | **Stale claims in sibling reports mislead planning.** The component-reuse investigation says Broiler.DateTime's civil-day primitives were made public; they are internal in every committed state. | [review.md](review.md) lists every claim found stale. The plan relies on source, not on reports. | None |

## Architecture

| # | Risk | Mitigation | Stop condition |
|---|---|---|---|
| A1 | **A shared library references an engine, or anything in Broiler.VM.** For example, it reuses `IVmMeter` for charging. | Rules SL1 and D1; contract convention 4 (a neutral meter) | **Stop:** any such reference |
| A2 | **A deployment boundary is breached:** the frontend in a compiler-free root, or data in a root without its surface | Rule `I3` on published closures; the composition register's shared-library column | **Stop:** rule `I3` red on a landed change |
| A3 | **Platform behaviour creeps into shared code:** normalization, the platform's regular-expression engine, culture casing or comparison, `TimeZoneInfo`, runtime Unicode categories | Rules SL4 and `I2`, a metadata scan of the shipped DLLs | **Stop:** either rule red |
| A4 | **Ambient or mutable static state in shared code**, which is Broiler.JS's `CoreScript` pattern | Rule SL2, the shape of Broiler.VM's N12 | **Stop:** SL2 red |
| A5 | **The wrong abstraction:** a "shared" API that needs an engine identity, or a growing set of engine-specific switches | Extraction-gate condition X3. Engine policy enters only as a limit, a policy record or a provider. | **Stop:** an API parameter whose meaning is "which engine". The area is refused instead. |
| A6 | **Hidden coupling through types:** a shared type appears in a Broiler.VM public signature (rule B4's intent), or Broiler.JS's `StringSpan` leaks into a shared signature | Rule `I6`; neutral span types in every signature | **Stop:** rule `I6` red |
| A7 | **The two programmes collide.** The universal bytecode programme's UBC-3 changes Broiler.VM's JavaScript lowering while SJL-13 to SJL-15 change its front end. | Neither waits on the other. The frontend's output is the validated tree JSD-0036 names. Each cut-over lands on whichever lowering exists then, with its own artifact comparison. | **Stop:** one change that alters both the front end and the lowering, which would make the artifact comparison unattributable |

## Delivery

| # | Risk | Mitigation | Stop condition |
|---|---|---|---|
| D1 | **Duplication retained:** the shared library lands beside two surviving implementations | Deletion lists in every area; Broiler.VM's rule `I5`; Broiler.JS's deletion search; the "deletion in the same change" obligation | **Stop:** a cut-over that leaves a listed member, a switch or a fallback copy |
| D2 | **A cut-over loses a verdict** | Bases at named commits; predeclared rules; every difference classified | **Stop:** a verdict worse than the rule admits, with no rule revision written before the runs that showed it |
| D3 | **Broiler.VM's fuel or exhaustion verdicts move**, because shared code charges in different units | Charging units are specified from Broiler.VM's current charges; the fuel-parity twins are a gate clause; class C2 needs a dated decision | **Stop:** an exhaustion verdict changed without that decision |
| D4 | **Broiler.VM's artifacts stop being deterministic** after the frontend moves | Rules SL9 and T6; SJL-12 fixes the dictionary-order reliance first | **Stop:** two compilations of one source differ |
| D5 | **Diagnostic codes shift** and regrade Broiler.VM's conformance results | The diagnostic mapping of SJL-12.3; a registry revision per meaning change; per-variant comparison of early-error verdicts | **Stop:** a code whose meaning changed without a new number, which JSD-0009 forbids |
| D6 | **Broiler.JS's public API breaks downstream:** Broiler.JSeal, Broiler.HtmlBridge, users of `Ast` and `Parser` | Decision `SJL-D-25` dispositions; `SJL-D-12` (i) adapter packages; candidate-feed runs of the downstream suites | **Stop:** a public member removed without its recorded disposition |
| D7 | **The pilot shows no simplification, and the programme continues anyway**, because `SJL-D-23` was taken before the pilot: continue | The decision's criteria are SJL-3's exit clauses, so SJL-3 cannot be met while one fails. Reopening the decision on the pilot's evidence takes a new dated record. *(Noted 2026-10-20, which reopens nothing: SJL-3's clause 7, its clause 13 while it stays open, and SJL-4's clause 9 are unmet, and the dated note of that date under [`SJL-D-23`](decisions.md#sjl-d-23) names when the next dated choice is taken: when a controlled lane exists, or when those are the only unmet clauses of SJL-3 to SJL-15 and SJL-17.)* | None for the programme: SJL-3 stays `In progress` while a criterion fails. *(Revised 2026-10-11: the stop read "**Stop of the programme** if `SJL-D-23` records (c)".)* |
| D8 | **A component owner does not adopt the conventions** (Broiler.Unicode, Broiler.Regex) | Broiler.Unicode: decision `SJL-D-1`'s fallback to the Broiler.JS repository, recorded with its reason. Broiler.Regex: decision `SJL-D-14` (a) has no fallback home, so SJL-11 stays `In progress`, or the area is refused with its record, or a new dated choice reopens `SJL-D-14`. | None. The area moves or is refused, and no programme stop follows. |
| D9 | **Releases stall.** Broiler.VM waits for a shared package to be published. | Decision `SJL-D-4` (c): a committed hash-pinned feed during development | None |
| D10 | **Fixing Broiler.Regex costs more than planned** under `SJL-D-14` (a): bounded nesting without CLR recursion may need more than a repair of its closure-based design | Every requirement of SJL-11.2 is a component clause, and neither engine moves before all hold. Broiler.VM's matcher stays the reference for resource behaviour until then. | **Stop:** an engine cut over to Broiler.Regex before every requirement of SJL-11.2 holds |
| D11 | **The slice path's rebuild changes what it does** under `SJL-D-13` (a): its census counts, slot order or refusals | The slice corpus, byte for byte before and after, is a gate clause of SJL-15 | **Stop:** a rebuilt slice path landed with a slice-corpus difference |

## Data

| # | Risk | Mitigation | Stop condition |
|---|---|---|---|
| R1 | **Data drift in a co-hosting process.** A host resolves a newer data package than the one Broiler.VM pins, so Broiler.VM's behaviour changes with no pin move. | Decision `SJL-D-6`, taken as (c): no package carries data an engine reads, and each engine embeds the tables the shared generators produce. The risk is closed while (c) holds; a later decision to publish data packages reopens it. | **Stop:** a data package admitted under a policy that lets this happen silently |
| R2 | **Data budgets are exceeded** as tables move or locales are added. Broiler.VM's Unicode and CLDR budgets have little headroom. | Every data decision restates its budget, by citation, in Broiler.VM's bundle | **Stop:** a budget rule red |
| R3 | **Locale coverage regresses for Broiler.JS** when its host cultures stop answering | An optional host provider (`SJL-D-22` (c)) | None |
| R4 | **The time-zone policy changes Broiler.JS's historical answers:** release, backzone and primaries | Decision `SJL-D-20`; class C5 changes listed | None |
| R5 | **An engine's embedded tables drift from the shared generator or pins**, because under `SJL-D-6` (c) each engine regenerates its own | Rule `I4` holds Broiler.VM's pins equal to the home's; rules N22, N28, N30 and N32 regenerate Broiler.VM's tables byte for byte; rule SL7 does the same in the home | **Stop:** rule `I4`, or a byte-for-byte rule, red on a landed change |

## Security and supply chain

| # | Risk | Mitigation | Stop condition |
|---|---|---|---|
| S1 | **Unbounded work on hostile input** in a shared parser, matcher or codec | Contract conventions 4 and 5; fuzzing; the adversarial regular-expression corpus | **Stop:** a hostile input that completes without charging, or that hangs |
| S2 | **Stack exhaustion** in a shared parser or matcher. Broiler.Regex's parser and compiler recurse without a guard today. | Depth limits checked before the stack is; explicit stacks; Broiler.VM's large compilation thread kept | **Stop:** a stack overflow on any fuzz input |
| S3 | **A tampered or confused package** | Package source mapping; lock files with content hashes; rule `I1`; the upstream commit recorded (rule `I7`). *(Corrected 2026-10-12: the list began "Prefix reservation on nuget.org", which the owner withdrew when decision `SJL-D-2` was revised.)* | **Stop:** a restore whose content hash differs from the pin |
| S4 | **A vulnerability in a shared library** reaches both engines at once | ADR 0012's R4 covers admitted packages; a security intake in each home; coordinated releases | None. Responding is R4's process. |

## Licensing and assurance

| # | Risk | Mitigation | Stop condition |
|---|---|---|---|
| L1 | **Code of share-alike or unclear provenance enters a shared library.** Broiler.JS's radix routine cites a Stack Overflow answer; other units cite third-party engines. | Provenance checked unit by unit (governance section 9); such code is rewritten from the specification or deleted, never moved | **Stop:** a moved unit without a recorded origin and licence |
| L2 | **Assurance coverage shrinks.** Code leaves Broiler.VM's J rules and is covered by nothing. | Decision `SJL-D-5`, taken as (a): home assurance gates from a library's first commit; an exclusion in the moving bundle | **Stop:** code moved out of Broiler.VM while its home has no active assurance gate |

## Evidence

| # | Risk | Mitigation | Stop condition |
|---|---|---|---|
| E1 | **Agreement between the engines is mistaken for correctness** | Validation principle 1; the differential harness is a cross-check only | **Stop:** a clause met by agreement alone |
| E2 | **A cross-engine performance comparison enters a record** | Validation principle 6 | **Stop:** any such figure, which Broiler.VM's release gate 10 forbids |
| E3 | **A figure enters a plan, ledger, decision or support document** | Section 6 of the roadmap; the sweep at SJL-16 | **Stop:** any such figure, which is an untruthful claim under Broiler.VM's MVP terms |
| E4 | **The two test262 runners' totals are compared** | Validation layer L5 says they are not comparable | **Stop:** any document comparing them |
| E5 | **Unreviewed work accumulates** | Every bundle names the review debt it adds | None. Broiler.VM's MVP terms permit building unreviewed work, and the ledger states the cost. |
