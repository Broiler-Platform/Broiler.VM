<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0037 - The source-text section, and what `Function.prototype.toString` answers from it

**Status:** Proposed, 2026-10-04. **Owner decision pending.** The code this record describes is in
the tree: an optional artifact section, its writer, its verifier pass and its one reader. Taking the
record accepts the section, the default that every compilation carries it, and the departure from
the delivery plan's identity line recorded in section 5; not taking it leaves a section an artifact
may omit and a request flag that omits it.

**Owner:** MaiRat. Not yet signed. **Co-signer:** none. If the record is taken, **both roles are
held by one person**, and it does not claim the co-signature is independent.

**Milestone:** none of this profile's. It is the record that opens phase F3 of
[section 26](../roadmap.delivery.md#26-the-road-to-a-full-featured-profile), whose first delivery
is "`Function.prototype.toString` returning the source text a function was defined from", and whose
"Made of" asks for "an artifact section holding source spans, which manifests carry it, and what a
size-sensitive composition may drop". Stacks are the phase's second record, not this one.

**Context.** Until this record the artifact carried no source, and `Function.prototype.toString`
answered the NativeFunction form `function name() { [native code] }` for every function, written
in the guest or not (roadmap section 6's reopened table, *JSC-249*). The edition requires the
source text for a function defined by source (ECMA-262 `Function.prototype.toString`, step 2: "If
func is an ECMAScript function object ... return CodePointsToString(func.[[SourceText]])").
**The suite did not show the gap**: `test/built-ins/Function/prototype/toString` passed 158 of its
160 variants in the 2026-10-04 whole run, because its harness's `assertToStringOrNativeFunction`
accepts the NativeFunction form wherever it would accept the source. The two failures were a
computed method key whose rendering read `undefined`. With the section every one of the 160
variants passes on the source-text arm of that assertion, which is the arm that checks the text.

---

## 1. What was true before, verified

| Question | Answer | Where |
|---|---|---|
| Does a compiled artifact hold the source it was compiled from? | No. It holds constants, code, a position table mapping code offsets to line and column, and no text | `JsCompiler.cs` `Assemble` |
| Does a parse node know where its text begins and ends? | Only as a line and column of its first token; no node records an end, and no token an offset | `SliceTokenizer.cs`, `JsParser.cs` |
| What did `toString` answer for `function f(a) { return a; }`? | `function f() { [native code] }` | `JsRealm.Function.cs` |
| Is an unknown section kind refused or skipped by the verifier? | Refused, `1101 UnknownSectionKind`: a build that does not know a kind does not read past it | `JsVerifier.cs` |
| Are optional sections added under an existing format version? | Yes: sections 13 (eval scopes), 14 (script declarations) and 15 (script referrers) were each added under format version 2 with "its absence means ..." semantics | `JsFormat.cs` `SectionKind` |

## 2. The decision

1. **One optional section, kind 16, `SourceText`.** Its rows are `(unit, text, start, length)`,
   ascending by unit, each naming a code unit, a String constant, and a non-empty span of that
   constant. **Its absence means "no text"**, and a function with no row renders as the
   NativeFunction form, which is what every artifact written before it answers.
2. **One String constant per source, shared by every row from it.** A script, a module and a
   `Function` constructor's synthesised source are each interned once, whole, and every unit
   compiled from them names a span of that one constant. A program with a hundred functions pays
   for its text once and for four small integers per function, not for a hundred substrings.
3. **The span is the edition's.** A unit's span is the source text of the production that defined
   it, as the edition's static semantics fixes it:
   - a function or generator declaration or expression, from `function` or `async` to its `}`;
   - a method, getter, setter or generator method, from the start of its `MethodDefinition` - the
     `get`, `set`, `async`, `*` or name - to its `}`, so a static method's text does **not**
     include `static`;
   - an arrow, from its parameters to the end of its body;
   - a class's constructor unit, the whole `ClassDeclaration` or `ClassExpression`;
   - a `Function` constructor's result, the text the edition synthesises
     (`function anonymous(` parameters `\n) {\n` body `\n}`).
   Comments and whitespace inside the span are kept. Nothing outside it is.
4. **Every compilation carries it by default; a composition may drop it.** `JsCompileRequest`
   gains a public `KeepsSourceText`, `true` unless set. A size-sensitive composition sets it
   `false` and its artifacts carry no section and no text constants: the one observable effect
   is that `toString` answers the NativeFunction form for every function. **That is a departure
   from the edition the composition chooses**, and it is stated here so that no composition can
   claim conformance on `toString` while dropping the text.
5. **The text grants nothing.** It is a String any program could have built; reading it back
   reaches no capability and no host seam. Rows are charged one unit of verifier work each, and the
   constants they name are counted by the pool's existing ceilings, so the section adds no limit
   and no budget dimension.
6. **One code covers every structural refusal: `1633 MalformedSourceText`**, a core-result code
   with reason `InconsistentStructure`, registry revision 18. Its clauses are a unit past the
   table, a unit out of order or named twice, a text that is not a String constant, an empty span,
   and a span past the end of its text. The retained `source-text-*` entries break one clause each,
   and two entries verify and run.

## 3. What reads it

`Function.prototype.toString` (`JsRealm.Function.cs`) answers the row's span when the unit has
one, and the NativeFunction form otherwise. Built-ins, bound functions and host functions have no
unit and no row, so they keep the NativeFunction form the edition requires of them. The
interpreter, the baseline native form and the value form read one linked table, built by the
verifier from the section, so the three forms answer the same text.

## 4. What it costs

- **Artifact size.** An artifact grows by its source text once and four variable-length integers
  per function. Seven of the retained corpus's compiled entries changed bytes for this reason and
  no other; no retained hash outside the corpus manifest names them.
- **Identity of an artifact.** An artifact now changes when a comment changes. A cache keyed by
  the artifact's hash is keyed by its source text too; one keyed by the source text was already.
- **Memory.** The linked table holds a reference to each text constant, which the pool held
  already, and one entry per row.

## 5. The departure from the delivery plan

Section 26's F3 says: "**Identity:** a format-version increment if the record puts source in the
artifact." **This record puts source in the artifact and does not increment the format version.**
An increment exists so that a reader cannot misread bytes whose meaning changed. Nothing changed
meaning here: an artifact without the section reads exactly as before, and a build that predates
the section refuses an artifact carrying it with `1101 UnknownSectionKind` rather than skipping it.
Sections 13, 14 and 15 were added the same way under format version 2. The correction entry
[JSC-259](../roadmap.corrections.md#jsc-259) records the departure against the plan's text.

## 6. What this record does not decide

- **`Error.prototype.stack`.** F3's second record chooses its shape from the position table.
- **Whether a shipped composition drops the text.** No composition in the tree sets
  `KeepsSourceText` to `false`; one that does names this record.
- **HostHasSourceTextAvailable.** A host hook letting an embedder hide a function's text is a
  proposal, not part of the pinned edition, and nothing here implements it.
