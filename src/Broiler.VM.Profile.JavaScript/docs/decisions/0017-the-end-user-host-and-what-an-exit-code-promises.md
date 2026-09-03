# JSD-0017 - The end-user host: what it runs, what an exit code promises, and why it is not advertised

**Status:** Accepted for JS-3b's composition half. **The composition exists, is published and is
run on one RID**; the gate's "every claimed RID" is a collection nobody has made and this record
does not claim it.

**Date:** 2026-09-03

**Owner:** verification-boundary owner. **Co-signer:** Broiler.VM architecture and
developer-experience owner. **Both roles are held by one person** and this record does not claim
the co-signature is independent.

**Milestone:** JS-3b.

## What was open

`docs/compositions.md` recorded that no root here held the `narrow-runtime-compiler` label, that
the slice-compiler root "is only shaped like one", and that "JS-3b claims the label with a
publish-and-run gate of its own". The [ledger](../roadmap.status.md)'s JS-3b row carried the
consequence as an open clause.

**What was missing turned out to be narrower than the register's own sentence.** The register said
the label waited on a source surface, and JS-3b wrote one. What it actually waited on was a
composition **handed source from outside its own image**: every JavaScript root here reads its
input from inside one, and a root lowering a builder it also wrote cannot demonstrate what the
label means. [JSC-56](../roadmap.corrections.md#jsc-56) records that.

Four decisions follow, and the second is the one a consumer would notice first.

---

## 1. It is a host, not a toolchain, and the core roadmap's refusal is untouched

The core roadmap's section 10 says there is "no command-line tool, build integration, or packaged
toolchain anywhere on that path", and defers a toolchain component with a named trigger: a product
that must ship precompiled artifacts **with no compiler in its image**.

**That refusal is about a compiler you invoke to produce artifacts for a different host, and this
is not one.** This root compiles and runs in the same process, which is precisely what the
`narrow-runtime-compiler` label describes and what a browser does. It emits no artifact, writes no
file, and has no `--out`. Nothing here is a step towards a packaged SDK and the trigger the core
names is still unmet.

**Rejected: a `--compile` mode writing an artifact to disk.** It would be useful, it would take an
hour, and it would be exactly the toolchain the core defers - with a consumer invented to justify
it. The corpus writer that legitimately needs to emit bytes already exists in the slice-compiler
root, where it is a demonstration and says so.

---

## 2. The exit codes are a contract, and two of them accuse this component

A host whose only codes are zero and one makes *your program has a syntax error* indistinguishable
from *this host is broken*, and a script driving it over a corpus cannot tell them apart. There are
seven:

| Code | Meaning | Whose fault |
|---:|---|---|
| 0 | every file ran and completed | — |
| 1 | a program threw and nothing caught it | the program |
| 2 | the command line is not one this host understands | the caller |
| 3 | a source was refused before it became an artifact | the program, or this manifest |
| 4 | **an artifact was refused by the verifier** | **this component** |
| 5 | a program spent its instruction allowance | the program |
| 6 | a named file could not be read as source | the caller |
| 7 | **this host did something wrong** | **this component** |

**Over several files the worst code wins, and the order is by whose fault it is rather than by how
bad it sounds.** Code 4 outranks code 6 even though an unreadable file sounds worse: an artifact
this host's own lowering produced and its own verifier then refused is a **defect here**, and a
defect must not report under a code that reads as a property of the input. A sweep of a thousand
files that turns up one lowering defect among nine hundred ordinary refusals reports the defect.

**Rejected: reporting the LAST file's code.** It is what a naive loop does and it makes the answer
depend on filename order.

**Rejected: folding refusals into code 1.** A refused source and a thrown error are the two
outcomes a person most needs to tell apart, because one is fixed by editing the program and the
other by reading it.

---

## 3. Reading a file honestly is a decision, and it has two halves

Every sibling root gets its source from inside its own image, so two problems arrive here for the
first time.

**A leading byte-order mark is removed.** The language defines U+FEFF as format-control whitespace
a source text may open with, and a tokenizer handed one would refuse an ordinary file saved by an
ordinary editor with an unexpected-character diagnostic naming a character the author never typed.

**Bytes that are not UTF-8 are refused, not decoded.** The convenient decoder substitutes U+FFFD
for a bad byte, and a replacement character **changes the program**. A host that silently ran a
different program than the file contains is worse than one that declines, so the file reports code
6 with the offending byte offset named.

**Rejected: a `--encoding` option.** It would be a way to ask this host to guess, and the guess
would be wrong silently.

---

## 4. It states no ceiling of its own, and registers nothing

The instruction allowance is **the profile's own declared default** unless a caller passes
`--fuel`. A host with an opinion about how long a program may run is a host imposing a policy the
profile did not declare, and the fifteen declared defaults are the profile's per
[JSD-0004](0004-limit-defaults-hard-maxima-and-the-budget-matrix.md). What the default buys is
that a program which never terminates ends in a **bounded number of instructions rather than
never**, and ends the same way on a busy machine, because fuel is charged per instruction and not
per second.

**One exception, and it is a caller's option rather than a default.** The parse options' nesting
bound of 64 refuses two files of the Octane benchmark before the manifest is consulted, with
`NestingTooDeep` - a ceiling the specification permits and not a statement about the language. The
host takes `--max-depth` so the two readings are measurable;
[JSC-57](../roadmap.corrections.md#jsc-57) records the disagreement between this default and the
construct census, which reads at the largest bound the parser supports. **Whether 64 is the right
default is not decided here** - it now has a measurement behind it and still needs an owner.

**No capability and no artifact provider are registered**, so every guest-initiated load is refused
deterministically. That is this host's content policy, and it is the only one a manifest with no
`eval`, no `Function` constructor and no dynamic `import()` could have.

---

## What it is not, stated because this is the root a reader will assume otherwise

- **It is not advertised and not packable.** It is the first root here a person would expect to be
  shipped, and it is not. A tool advertised as a JavaScript host has to be able to run JavaScript;
  this manifest admits no function, no object, no string value and no property access, and pointed
  at the Octane benchmark the host refuses all twenty-four files. Section 1 of the composition
  register stays as it is.
- **It is not a JavaScript implementation** and its `--help` says so in those words, because a
  help text is where that claim would otherwise be made by omission.
- **It is not a conformance oracle.** It reports what happened to each file; it compares nothing
  against a declared expectation, has no pinned suite, no self-check and no ratchet. The harness
  roadmap [section 14](../roadmap.md#14-the-conformance-oracle) specifies is a different
  composition and stays one.
- **It does not close JS-3b's clause.** The composition exists and runs; "on every claimed RID" is
  a collection, and one machine is not a matrix.

## How it is judged, and why not by a test method

`src/tests/cli/` holds input files and `eng/run-cli-acceptance.py` drives the **built binary** over
eighteen command lines, judging exit codes and output. Nothing is injected: no source of this
component is patched to make a case fail, no internal type is reached for, and no JavaScript is
embedded in a test. **A control here is an input.**

That is not a stylistic preference. The subject is a binary's argument parsing, its exit codes,
which of its two streams carries which message, and what it does with a file that is not UTF-8 -
and none of it is reachable from a test project, because rule A11 forbids a test project to
reference a profile assembly, so a test could not compose this host at all. A test calling an
internal type would be judging something a person cannot invoke.

**The driver takes `--expected` so that it can be shown to report a mismatch.** Pointed at a table
of deliberately wrong rows it fails and names what differed, which is the check a driver whose
every row passes cannot otherwise offer.
