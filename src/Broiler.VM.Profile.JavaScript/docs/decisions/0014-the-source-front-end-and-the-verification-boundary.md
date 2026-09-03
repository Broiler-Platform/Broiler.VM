# JSD-0014 - The source front end: where the verification boundary falls, where strict mode is ruled on, and what a nesting case gets

**Status:** Accepted for JS-3b.

**Date:** 2026-09-03

**Owner:** verification-boundary owner. **Co-signer:** the profile runtime owner. **Both roles are
held by one person** and this record does not claim the co-signature is independent.

**Milestone:** JS-3b.

## What was open, and why these answers had to be taken together

Roadmap [section 9](../roadmap.md#9-the-semantic-front-end-and-lowering) names five things a front
end for this profile has to decide and leaves three of them explicitly open. Until this milestone
the profile had **no front end at all** — no tokenizer, no parser, no static-semantic stage — so
every one of the five was a paragraph rather than a decision, and the only thing that turned
source into an artifact was a hand-written builder a human drove instruction by instruction.

The five are answered here in one record because they are one design. Where strict mode is ruled
on determines what the tokenizer may know; what the tokenizer may know determines whether the
re-scans can be deleted; whether the verifier re-derives early errors determines what the lowering
is allowed to emit; and the nesting answer determines whether any of the three passes may recurse
at all. Splitting them across five records would have produced five decisions each of which reads
as though the other four were already settled.

**One thing this record does not do is claim the front end is finished.** It compiles
`broiler.javascript.slice`, which admits numbers, booleans, `undefined`, local bindings, the
operators the format has opcodes for, and structured control flow. It has no functions, no
objects, no strings as values, no `try`, no modules and no regular expressions. Every decision
below is a decision about the shape of a front end for this profile; each states what would
falsify it when the manifest grows.

## Decision 1: static semantics is one stage over the tree, and it is the only place an early error is reported

The seed splits early-error responsibility across four places in two assemblies. Section 9 rejects
that split on one ground — a verifier that must answer totally, in one pass, with one diagnostic
per rejection, cannot be built over it — and this component's front end has exactly one such
place: `SliceStaticSemantics`, one pass over a parsed tree, one diagnostic list.

The parser rules on nothing. A duplicate lexical name, an assignment to a constant, a legacy octal
in strict code, a `break` outside a loop, a name that resolves to no binding: every one of them
parses into a tree and is reported by the stage. What the parser refuses is what is not a tree.

**The cost, stated:** the parser accepts programs that are not programs, so a reader of the parser
alone cannot tell what this profile admits. That is the price of having one place to look, and the
alternative is the seed's four places.

**Falsified if:** an early error the manifest requires is reported anywhere but that stage.

## Decision 2: strict mode is *recognised* by the tokenizer and *ruled on* by the validator

Section 9 asks for this ownership to be named. The seed's parser deliberately tracks no strict
mode and the ruling is part of the four-way split; this component's answer is a split of a
different kind, along recognition and ruling rather than along passes.

The tokenizer records **facts** and asks no questions: that a numeric literal had a legacy-octal
shape, and that a string literal had this exact raw text. It never asks whether either is allowed,
because that depends on a directive prologue it has not reached and on a goal symbol it is not
told. The validator holds the ruling: it reads the goal, reads the prologue, decides strictness
before it looks at a single name, and then rules.

**Why not the parser.** Putting the ruling there would need the parser to know the goal and to have
finished the prologue before tokenizing the rest of the source — which is the seed's ambient parse
state wearing a different name, and decision 4 removes exactly that.

**What this buys, and it is the concrete half of the decision: both of the seed's source re-scans
are deleted rather than reimplemented.** The seed re-tokenizes raw source text in two places, and
section 9 asks for the facts they recover to be carried on the tree instead. They are:

| The seed's re-scan | The fact carried instead |
|---|---|
| re-tokenizing to tell a directive from a string expression of the same value | `SliceToken.RawText`, so `"use strict"` is a directive and a literal spelling the same characters with an escape is not |
| re-tokenizing to tell a legacy octal from a decimal literal | `SliceToken.IsLegacyOctal`, recorded by the pass that had the characters |

A third would have been needed and never existed in the seed's shape: automatic semicolon
insertion is a question about the whitespace *between* two tokens, and `PrecededByLineTerminator`
answers it without the parser looking back at the text.

**Falsified if:** the validation stage reads the source text, or a directive is recognised from a
string's value rather than from its raw text.

## Decision 3: the verifier does not re-derive early errors, because there are no bytes to derive them from

This is the first of section 9's two open questions, and the answer is **no**.

Source carrying an early error never becomes an artifact. It is refused at the seam, with a code
from the registry's `embedder-seam` half, and nothing is emitted for a verifier to read. The two
stages therefore check disjoint things:

- **The front end** checks what a *program* can be wrong about: a name with no binding, a constant
  assigned, a construct outside the manifest.
- **The verifier** checks what *bytes* can be wrong about: framing, limits, opcodes, stack
  discipline, reachability — over every artifact whatever produced it, because an artifact does not
  have to come from this lowering and a verifier that trusted one producer would be a verifier for
  one producer.

Neither repeats the other, which is what the rejected alternative would have cost: a front-end
contract returning "a validated tree the lowering consumes" while the verifier re-derives the same
early errors is two implementations of one ruling, and the second one is the one nobody reads.

**Falsified if:** a compilation result ever carries artifact bytes and a diagnostic at once.

## Decision 4: parse options are a value, and no front-end state outlives a call

The seed reads its two most consequential grammar switches — the module-versus-script goal and the
top-level-await permission — out of ambient async-local state in a different assembly. Section 9
rejects this for three reasons and this component replaces it with `SliceParseOptions`, a readonly
record struct passed in.

Section 9 states the gate as a runtime test: two parses with different goals running concurrently
in one process, each goal-appropriate, failing when the options are replaced by a shared static.
That test exists and the producer composition runs it, 200 concurrent script-and-module pairs over
a source the two goals answer differently.

**It is the weaker half, and this record adds the stronger one.** A concurrency test can only fail
over a static the two parses it runs actually reach; a switch moved into a third construct nobody
wrote a case for would leave it green. **Rule N12** scans every declaration in the lowering
assembly for anything that could outlive a call — a mutable static, a settable static property, a
`[ThreadStatic]`, an `AsyncLocal` or `ThreadLocal` — and there is none. A `static readonly` table
is not its subject and is not reported, because nothing can write one.

**Falsified if:** any grammar or strictness decision reads a value that did not arrive through the
options.

## Decision 5: deep nesting is refused, and the bound is explicit rather than a worklist

Section 9 makes a process termination on a nesting case a **blocking** failure. `CallDepth` bounds
guest frames and reaches none of the three compile-time recursions; the seed mitigates the same
problem with stack segmentation and an oversized thread, neither of which this component has.

The answer is an explicit depth bound carried in the parse options, defaulting to 64, and exceeding
it is a refusal carrying `NestingTooDeep`.

**Why not the worklist rewrite the section offers as the alternative.** Three reasons, in order of
weight. The three recursions are over three different shapes, so a worklist rewrite is three
rewrites. A worklist that survives arbitrarily deep input *answers* a case the section wants
**refused**, so it would satisfy the letter of "does not terminate the process" while failing what
the clause is for. And a bound is a number a reader can see, where a worklist's real limit is
whatever the machine had.

**Why the default is 64 and not a measurement.** A bound derived from a measured stack is a number
that moves with the runtime, and the point of the bound is that the refusal is the same answer
everywhere.

**The cost, stated as a conformance exclusion:** a program nested deeper than the bound is refused
by this profile and accepted by every other JavaScript implementation. JS-3a's harness has to score
it as an exclusion rather than as a failure, and this is the row it reads.

**Falsified if:** a source nested past the bound terminates the process instead of being refused.

## What a doubly-bad input gets, and why it is not a phase-order tie-break

Section 9's second open question is what an artifact both malformed in framing and invalid in
static semantics is answered with. **It gets the framing answer**, and the reason is stronger than
an ordering: static semantics is a property of a **tree**, and a doubly-bad *artifact* has no tree,
because its bytes were never source.

The phases cannot fuse. They live in two assemblies that do not reference each other, they take two
different input types, and they run in runs that need not share a process — the execution-only
composition carries a verifier and no front end at all, and publishes and runs that way. Section 9
asks for a named case that fails when the phases are fused; the case is that an early error never
produces bytes, and fusing the phases — lowering the bad program and letting the verifier catch
it — makes it fail.

## Three conformance exclusions this front end creates, recorded here rather than discovered later

Each is a place where this profile answers differently from the language. None is a defect and all
three become defects if the manifest grows without them being revisited.

**An unresolvable name is an early error here and a runtime `ReferenceError` in the language.** In
the language a free name might be a property of the global object, so the answer waits for run
time. This manifest declares no global object and no property access at all, so a name that
resolves to nothing can never resolve at run time either, and deferring would move the same refusal
to a stage with less to say about it. *It becomes a real divergence the day the manifest grows a
global*, and on that day the answer has to move.

**An identifier is ASCII plus `$` and `_`.** The language's answer is the Unicode `ID_Start` and
`ID_Continue` properties, which need the Unicode data this component has not acquired — an open
dependency with a named holder, opened at JS-0 and consumed at JS-6. A non-ASCII identifier is
refused as an unexpected character, which is a refusal rather than a silent acceptance.

**A loop whose exit is reachable from nothing does not compile.** `while (true) { }` with no
`break` lowers to a tail the verifier refuses as unreachable code. That is the format's answer
rather than the lowering's — the format has no way to declare a program that cannot return — and it
is recorded because it is a real program that other implementations run.

## The free-name analysis is exact here, and its soundness contract still stands

Section 9 asks for the analysis's contract to be carried verbatim rather than paraphrased:
**over-approximation is safe and under-approximation is a miscompile.** The analysis in this front
end is exact, and it is worth saying why that is not a reason to drop the sentence.

It is exact only because the three constructs that can reach a binding never mentioned at all — a
direct `eval`, a `with`, and a `debugger` — are each outside this manifest and refused by name. The
moment one is admitted, the analysis must over-approximate or it is wrong, and the sentence is what
tells whoever admits it which direction to err in.

## What this decision does not settle

- **The lowering that JS-4 deletes.** JS-1's hand-written builder is still here and still writes
  half the corpus. This front end uses its instruction buffer and label patching rather than
  replacing it; JS-4's gate carries the deletion of the hand-written *programs*, and the builder
  itself becomes this front end's back end. That is a re-scope of JS-4's clause and is recorded as
  [JSC-45](../roadmap.corrections.md#jsc-45).
- **Modules.** The goal symbol is carried, `SliceGoal.Module` is honoured for strictness, and no
  `import` or `export` parses — a module goal today changes strictness and nothing else.
- **Error recovery.** The parser reports the first syntax error and stops. A tree that parsed is a
  real program and every early error in it is reported; a source that did not parse gets one
  answer, because recovering produces diagnostics about a program the source never described.
- **Any conformance claim whatsoever.** There is no oracle. The 25 accepted programs and 29 refused
  ones are this component's own claims about JavaScript, written by the same party that wrote the
  code they judge, and the roadmap's rule that a manifest name is not a conformance claim is
  unchanged by their existing.
