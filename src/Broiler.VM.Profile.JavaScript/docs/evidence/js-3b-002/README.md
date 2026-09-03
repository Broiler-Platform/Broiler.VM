# Bundle JS-3B-002 — three repairs a real suite asked for, and what each one moved

**Collected:** 2026-09-03. **Milestone:** JS-3b. **Owner:** profile architecture owner.
**Reviewer:** none.

**What this bundle is.** The three defects
[Bundle JS-3B-001](../js-3b-001/README.md) found by pointing real material at this component,
repaired, with the numbers each repair moved. It is a re-collection over the same pinned suite
rather than a new measurement: the ref, the tarball and its digest are unchanged.

**What this bundle is not.** Not a conformance score, not a floor, not an accepted anything. The
pin is still over a **transient checkout** — retrieved, hashed, read and left in a temporary
directory — and section 3 asks for material retrieved, hashed **and archived**. No suite file is in
this repository.

---

## 1. What was found, and what is repaired

| Found in JS-3B-001 | Repaired here |
|---|---|
| A loop whose body always breaks emits a continuation nothing reaches — 13 artifact refusals | **yes** ([JSC-60](../../roadmap.corrections.md#jsc-60)) |
| `#!` opening a source text is a comment and the tokenizer refused it — 6 failures | **yes** ([JSC-61](../../roadmap.corrections.md#jsc-61)) |
| The temporal dead zone answered `undefined` — 8 failures | **yes** ([JSC-62](../../roadmap.corrections.md#jsc-62)) |
| Two files the dialect reader could not read | **yes** ([JSC-59](../../roadmap.corrections.md#jsc-59)) |

---

## 2. The temporal dead zone needed the format to grow, and that is the decision in this bundle

Reading a `let` or `const` before its initialiser has run is a **runtime `ReferenceError`**. This
profile answered `undefined`, and the reason is worth stating exactly: **reading a slot that had not
been written yet was indistinguishable from reading one holding `undefined`** — which is precisely
the distinction the dead zone exists to draw.

**And nothing in the format could express the difference.** Division by zero is `Infinity` here and
every other instruction is total, so **format version 1 had no instruction that could fail at all.**
There was no lowering to write.

One opcode was added — `ThrowUninitializedBinding` (`0x71`) — and roadmap
[section 7](../../roadmap.md#7-the-bytecode-format-and-the-verifier) sanctions that in its own
words: format version 1 "grows with the interpreter", with compatibility promised only when a
persisted-artifact version is accepted, which no milestone grants.

**Two properties of the opcode are decisions rather than details.**

- **It declares a push of one and never pushes.** It stands exactly where a `LoadLocal` would have
  stood, so declaring that height leaves every join, every bound and every reachability answer
  identical to the program with no dead zone in it. The frame is abandoned before the push happens,
  so the declared height describes a state no execution observes.
- **It carries no operand**, so the message names no binding. Naming one needs an interned name, and
  the constant pool's interned-name tag is reserved from version 1 and admitted by no manifest yet.
  The error *kind* is what a conformance test matches on.

**The detection is in the lowering and is exact for this manifest — and only for this manifest.**
The lowering walks the tree in the order the program runs, so a set of slots whose initialiser has
been lowered answers the question directly. That works because there is no function, no closure, no
`eval` and no label here, so nothing can re-enter the middle of a block or defer a read past its
lexical position. **In a manifest with any of those it would be a runtime question and this would be
wrong.**

---

## 3. What each repair moved

`test262-sweep.log` and `test262-harness.log`, over the same 53,469 files at the same ref.

**The harness:**

| | before | after |
|---|---:|---:|
| Executed | 1,188 | 1,205 |
| Passed | 1,170 | **1,201** |
| Failed | 18 | **4** |
| Declined as unscorable | 7,366 | 7,367 |

**The host's own sweep moved in four columns, each for a reason named above:** 103 → **117**
completed; 13 → **0** artifact refusals; 53,337 → **53,332** refused at the source seam, as five
hashbang files began to parse; and **4 faults where there were none** — programs that now throw the
`ReferenceError` the dead zone requires.

**One of the six hashbang files is now declined rather than passed**, and that is the honest
outcome: it parses past the hashbang and then needs a construct this manifest does not admit, so
the refusal is not a language answer and [JSC-54](../../roadmap.corrections.md#jsc-54)'s rule
reports it unscorable.

**The four remaining failures are not a gap in this component.** They are `using` declarations — a
proposal — scored because the run passed no feature filter. The suite carries the metadata to
exclude them; what decides whether such a test applies is the **language edition**, which section 3
records as unpinned. That row now has a cost attached to it rather than only a description.

---

## 4. Two safety properties, checked rather than argued

- **The retained corpus regenerated byte-identical** after every repair. No artifact whose bytes
  are pinned was touched.
- **The reachability analysis answers "reachable" wherever it is unsure**, which is what the
  lowering did unconditionally before, so the only bytes that could move belonged to programs the
  verifier was already refusing.

**One defect was caught by that first check and not by any test.** The first draft of the dead-zone
lowering emitted a position row on *every* identifier read rather than only on the fault. Nothing
failed — but the corpus stopped regenerating identically, and that is what named it.

---

## 5. Exclusions — what this bundle does not show

- **What is repaired is narrower than what is named.** A **write** before initialisation
  (`x = 1; let x;`) is a `ReferenceError` in the language and is not one here. **Dead code after a
  `break` inside a block** is still emitted, because `var` is hoisted and a slot the executor never
  wrote is a state this profile does not guarantee. And a **loop with no exit at all** is still
  refused. Each is recorded with its reason rather than folded into a change that would have had to
  guess at it.
- **The dead-zone detection is sound for this manifest and would not be for a larger one.** It is a
  lowering-order set, not a runtime check; a manifest with functions, closures or `eval` needs the
  latter. This is stated here and in the code rather than left for a later reader to discover.
- **The pin is over a transient checkout.** Retrieved, hashed, read, left. Section 3 wants
  archiving too, so **no floor is set over any figure here** and the dependency does not close.
- **One machine, one RID.** `win-x64`. The lane runs the host and both suites on every cell from
  the previous change onward.
- **Nothing is accepted**, no milestone advances, and the advertised set is still empty.
