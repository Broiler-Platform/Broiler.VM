# Area: shared conformance vectors

**Delivered by:** [SJL-1](../milestones/sjl-1-shared-conformance-vectors.md), then extended by every
area milestone. **Home:** the Broiler.JS repository (decision [SJL-D-1](../decisions.md#sjl-d-1)).
Broiler.VM receives the corpus as a pinned archive. **Status:** design.

---

## 1. Purpose and limits

A vector pins **one operation's answer to one input**, with the provenance of that answer. The
corpus serves four purposes:

1. **It tells each engine where it is wrong today**, before anything moves (SJL-1).
2. **It is the library-level oracle** for each shared implementation, together with the
   specification and the reference arms.
3. **It is the engine-level regression check** at every cut-over.
4. **It is the place a defect is pinned first**, before its fix, in the cross-repository change
   flow ([governance.md](../governance.md#6-the-cross-repository-change-flow)).

**It is not test262.** It complements test262 where test262 is thin, where an operation's whole
input space matters, or where an algorithm is tested in isolation from its built-in. **It never
replaces test262 as the oracle**, and agreement between the engines on a vector is not evidence that
the vector is right.

---

## 2. Existing assets that seed it

| Asset | Where | Use |
|---|---|---|
| Broiler.VM's differential probes | `src/tests/differential/*.js`, among them number formatting, BigInt, Float16, raw JSON, Date and JSON, and normalisation | Inputs and cases. Their retained answers are Broiler.VM's own, so **they are not used as expected values**. |
| Broiler.VM's Unicode probes generated from `NormalizationTest.txt` | `src/tests/differential/the-unicode-normalization-*.js` (held by rule N22) | The library-level oracle is the test file itself |
| The UCD and CLDR test files Broiler.VM pins | `src/tests/unicode/pins/ucd-17.0.0/` (normalisation, case folding, the break tests); `src/tests/cldr/pins/…/uca/` (collation) | The library-level oracles for SJL-7, SJL-9 and, for collation, SJL-17 |
| Broiler.VM's Intl and Temporal reference datasets | `src/tests/cldr/<area>/` (answers from an independent ICU, with divergences declared by clause); `src/tests/temporal/` (the proposal's reference polyfill at the pinned revision) | Engine-level acceptance data for both engines, at SJL-8 to SJL-10, and at SJL-17 for `orderings` |
| Broiler.JS's inline expression tests | for example its number round-trip, Temporal string-limit and `Date.parse` tests | Inputs. Expected values are re-derived from an independent source. |
| Broiler.JS's focused test262 path lists | `scripts/compliance/test262-*.txt` | Pointers into test262 per area |
| test262 at the pin | Both repositories pin `ccaac100…` | Vectors derived from test262 tests carry its licence notice |

**The gap the corpus fills.** Neither repository has an engine-neutral number-formatting or
date-parsing vector file today. Broiler.VM's profile is internal, and rule A11 keeps its test
projects from referencing it, so every Broiler.VM vector travels today as a guest program.

---

## 3. The format

The format is line-oriented and versioned (`schema: 1`). One vector is one record:

| Field | Content |
|---|---|
| `id` | A stable identifier, never reused |
| `area` | `numbers`, `numerics`, `text`, `date`, `unicode`, `time-zones`, `intl`, `temporal`, `regexp`, `syntax`, `semantics` |
| `op` | The specification operation or built-in, for example `Number::toString` or `StringToNumber` |
| `in` | Typed inputs: `f64:0x…` (bit pattern), `str:"…"` (JSON-style escapes; lone surrogates allowed), `int:…`, `bigint:…`, `bool:…` |
| `out` | `value: …`, `status: …` (a shared-library status), or `error: RangeError` and the like (engine level) |
| `prov` | `spec:<clause>`, `test262:<path>`, `ref:<generator>@<seed>` (an independent computation), or `ucd:<file>:<line>` |
| `class` | `normative`; `impl-defined` with `per-engine: {…}`; or `divergence` with `engine:` and a `reason:` linking that engine's plan |
| `script` | The expression an engine-level runner evaluates, under the shared rendering rule |
| `since` | The corpus revision that added it |

**Rules of the corpus:**

1. **No expected value is taken from either engine.** The validator refuses a vector whose
   provenance is only engine-observed.
2. **A vector an engine fails is never deleted to make it pass.** It is classified.
3. **A classification change is a dated corpus entry.**
4. **Every area adds its vectors before its shared code lands.**
5. **Doubles are written as bit patterns**, so a vector cannot depend on any decimal formatter,
   including the one under test.

---

## 4. Runners

| Runner | Where | Calls |
|---|---|---|
| Library level | The home's test project, and the Native AOT probe | The shared API directly |
| Engine level, Broiler.JS | A Broiler.JS test project | The engine, through the `script` field, in a fresh realm |
| Engine level, Broiler.VM | A test-only harness root reading the pinned archive (rule N13) | The JavaScript profile, through the `script` field |

**The reader is Native AOT-safe.** It is hand-written over the line format, using `JsonDocument`
or `Utf8JsonReader` at most for the escaped strings, and never a reflection-based serializer. The
Native AOT probe runs it, and so does Broiler.VM's harness root, which is published with Native AOT
like every root.

**One rendering rule for both engine-level runners:** how a value prints, how an error prints, how
`-0` and `NaN` print. A difference between the runners is therefore never a difference in rendering.

---

## 5. Broiler.VM's pin

The archive and its `.pin` file record:

- the corpus's source commit;
- the archive's hash;
- the content hash.

It follows the shape of `src/tests/conformance/pins/test262.pin`. **A pin moves only by a recorded
decision**, in the change that re-runs the engine-level runner. Rule N13 keeps the directory out of
every product project.
