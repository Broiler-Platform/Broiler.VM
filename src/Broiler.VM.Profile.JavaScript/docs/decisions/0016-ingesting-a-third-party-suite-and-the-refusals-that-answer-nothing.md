# JSD-0016 - Ingesting a third-party suite: the dialect, the strictness readings, and the refusals that answer nothing

**Status:** Accepted for JS-3a's ingestion path. **The path is built and exercised; no third-party
suite is pinned, retrieved or held**, and nothing here changes that.

**Date:** 2026-09-03

**Owner:** verification-boundary owner. **Co-signer:** the profile runtime owner. **Both roles are
held by one person** and this record does not claim the co-signature is independent.

**Milestone:** JS-3a owns the harness; JS-3b's exit gate is what needs the path, because the clause
it cannot close is "the parse-and-early-error slice scored on JS-3a's harness against the ratchet".

## What was open

[JSD-0015](0015-the-conformance-oracle-and-what-it-refuses-to-score.md) built the harness and said
of its metadata reader that the shape is "deliberately the one the conformance suite uses … so that
the day a pinned suite is retrieved, this reader is pointed at it rather than replaced".

**That was false, and it was measured false rather than argued false.** Five files written in the
real dialect — a nested `negative` mapping, a folded description, an `info` block, no `expected`
key — were put through the harness on 2026-09-03. All five were refused with "declares no readable
expectation", and a suite is read whole, so the run scored nothing at all. Pointing the harness at
a real checkout would have produced a `HarnessDefect` exit and one complaint per file, tens of
thousands of them.

The gap was not only the reader. Three further questions had no answer anywhere:

- A suite declares its negative expectations as **JavaScript error types**. This front end refuses
  source with **its own diagnostic codes**. Nothing said which of those codes stand for which error.
- A file that declares neither strictness is defined to be run **twice**. The harness had one
  reading of a file and one identity for it.
- Both vocabularies use a flag spelled `raw`, **and they do not mean the same thing by it.**

Four decisions follow. The third is the one that matters; the others are what it needs to exist.

---

## 1. A suite is read in a declared dialect, and the run is told which

`--dialect native|ingested` selects the reader. `native` is the default and is this component's own
fixtures, which declare a verdict in this build's vocabulary. `ingested` is the dialect a
third-party suite writes.

**Rejected: sniffing the dialect per file.** The two are distinguishable in practice — one has an
`expected` key the other never writes — but a reader that guessed would change how a file is scored
the day somebody added a key, and it would be least reliable on exactly the malformed files where
the answer matters most. A suite is in one dialect and the caller knows which.

**Rejected: replacing the native dialect with the ingested one.** The ingested dialect cannot
express a positive expectation without an assertion library, and this manifest has no function for
one to be written in. A fixture whose whole body is `1 + 2` can only be scored by declaring the
value, so this component keeps a dialect in which it can.

**A consequence, recorded because it is not obvious:** the self-check is read in the suite's
dialect, and a third-party checkout does not contain this component's fixtures. `--selfcheck <dir>`
therefore points at the fixtures this repository holds while the tests come from the checkout.
Section 14 requires the self-check before every shard; against an ingested suite that is the only
way to have both.

---

## 2. A file that declares no strictness becomes two cases, and they are named apart

The suite defines a file carrying neither `onlyStrict` nor `noStrict` (nor `raw`, nor `module`) to
be run twice: once as written, once with `"use strict";` prepended. This harness does that, and the
two readings are `path#sloppy` and `path#strict`.

**The prologue is the one alteration this harness makes to what it runs**, and it is the suite's
own rule rather than a convenience of ours: the alteration is part of what the file means. The
sloppy reading is still the bytes on disk. There is no third form.

**Rejected: one reading per file.** Two of this front end's early errors —
`ReservedWordAsBinding` and `LegacyOctalInStrictCode` — exist only in strict code. A harness with
one reading would score half of them and would report the other half as an engine that failed to
refuse.

**Rejected: a suffix only where a file yields two cases.** Then `onlyStrict` would give a bare
path and a file with no flags would give two suffixed ones, and a reader could not tell which
strictness an unsuffixed path was read under. A script-goal case always says.

---

## 3. A refusal answers a question about the language only when it was a language answer

**This is the decision the rest of the path exists to support.**

An engine that refuses almost everything and a suite whose negative tests almost all declare a
refusal will agree on the observable outcome nearly every time they meet — and the agreement means
nothing, because the two facts have no connection. `broiler.javascript.slice` admits no function,
no object, no string value and no property access, so the refusal a real suite would provoke over
and over is `ConstructOutsideManifest`: **the source is valid JavaScript and this profile declined
the construct, without ever reaching the thing the test was about.** Scoring that agreement would
turn a manifest that admits almost nothing into a near-perfect conformance total, and it would do
so silently, at scale, in the direction that flatters.

Every source-refusal code therefore carries a declared class, and only one of the four may answer a
suite's expectation:

| Class | What it means | May it score? |
|---|---|---|
| **Early error** | The source is not JavaScript; every conforming engine must refuse it | **Yes** |
| **Outside manifest** | The source *is* JavaScript; this profile's manifest declines the construct | No |
| **Divergence** | This profile answers where the language answers differently, or later | No |
| **Implementation limit** | A ceiling the specification permits an engine to have | No |

Of the twenty-four codes: seventeen are early errors, one is the manifest, two are divergences —
`AssignmentToConstant`, which the language throws as a **runtime** `TypeError`, and
`UnresolvableIdentifier`, which the language throws as a **runtime** `ReferenceError` and whose own
declaration already records the divergence — and four are implementation limits.

**A case whose refusal cannot score is reported unscorable, and is neither a pass nor a failure.**
Not a pass, because the engine did not earn one. Not a failure, because nothing here is a defect
and the failure manifest is a repair queue. The rule runs **ahead of** the comparison rather than
inside it, which is what makes it a rule instead of a special case: no declaration can be written
that gets past it, and it holds in the positive direction too — a positive test the manifest
declines is unscorable for the same reason a negative one is.

**Rejected: four classes collapsed into a "can this score" flag.** They have four different
futures. *Outside manifest* disappears as the manifest grows and is a scope input; *divergence* is
permanent until a decision retires it and belongs in the published exclusions; *implementation
limit* is neither a defect nor an answer. A reader triaging a large unscorable total needs to know
which of the four is dominating it, and for a real suite the answer is expected to be the manifest
by a wide margin.

**Rejected: applying the rule to this component's own fixtures.** A native fixture declaring
`refused-by-source ConstructOutsideManifest` asked whether *this front end* refuses a construct
outside its manifest — a question the refusal answers exactly. It is scored, and scored as a pass.
The rule is carried on the test rather than inferred from its expectation kind, because inferring
it at each site is how one of the two suites eventually acquires the other one's rule.

**A refusal naming no code this build knows is a FAILURE and not an unscorable case.** A front end
that refused without saying why is a defect in this component, and filing it under "out of scope"
would hide a growing count of real defects inside a count of things the manifest does not admit.

---

## 4. The suite's `raw` flag is source, and this harness's raw mode is bytes

The two vocabularies spell one flag the same way and mean different things by it. In the ingested
dialect `raw` means *prepend no harness file and take no strictness variant* — the file is still
source. In this harness `raw` means the test carries **artifact bytes** that no front end lowers.

The adapter never maps one onto the other. Carrying it across would route source into the mode
reserved for bytecode, where the verifier would be handed a JavaScript file, and every such test
would fail for a reason nobody wrote down. **An ingested suite holds no artifact of this format at
all**, so the reader does not look for one there either.

---

## What this path refuses to do

- **It fetches nothing and holds nothing.** It handles a metadata dialect and a set of flag names,
  which are a format. No test, path, expectation or revision from any suite is embedded here, and
  this repository holds no suite file. Retrieving, hashing and archiving a suite remains the human
  action the [ledger](../roadmap.status.md#3-open-external-dependencies)'s section 3 records as
  open.
- **It does not prepend the assertion library**, because this manifest admits no call with which to
  load one. That is why almost every positive test in a real suite is declined by name rather than
  run: the suite prepends its library to every file without the `raw` flag, whatever that file's
  `includes` line says.
- **It does not carry a total across from anywhere.** Section 14's refusal is unchanged and this
  path adds no way around it.

## What it is for, stated plainly

**One arm makes a slice scorable at all**: a negative test whose declared phase is `parse` never
executes, so the assertion library it would have needed is never reached, and the question it asks
— is this source a syntax error — is one this front end genuinely answers. That is the
parse-and-early-error slice
[JS-3b's exit gate](../roadmap.delivery.md#js-3b-static-semantics-as-one-verification-stage-and-the-lowering)
names, and it is why that gate names that slice and not another.

**What it does not do is close that gate.** The clause says the slice is scored *on JS-3a's harness
against the ratchet*, and the ratchet is over a pinned suite. A suite nobody has retrieved has no
revision, and a run pointed at a directory carrying none reports `MissingSuiteRevision` rather than
a smaller total. The path is now the only part of that clause that is not waiting on a person.
