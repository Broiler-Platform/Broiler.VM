<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The wide shapes the baseline form's granularity is measured over

These compile under **`broiler.javascript.wide`**, which is the manifest this host runs by default,
and they run in **both output forms**: the bytecode form, and the baseline native form over the same
surface. `eng/measure-baseline-granularity.py` is the only thing that drives them.

They are not the kernels beside them. `../README.md`'s six compile under
`broiler.javascript.numeric` and drive the *numeric* native form, which refuses `%`, the bitwise
operators, objects, strings and closures by name. Nothing in this folder is refused by the wide
surface, which is the point: the question these shapes exist to ask is what the baseline form's
**step granularity** costs, and that question is only interesting over programs written the way
programs are written.

**No figure produced from these shapes is retained anywhere in this repository except in evidence
bundle `jsb-11-002`,** under `roadmap.gates.md` section 17 and `docs/baselines.md` rule L1. A figure
from them in a roadmap, a ledger, a support table, a decision record, a commit message or a code
remark is a defect in the commit that put it there. The one exception is the fuel figures in this
file, and section 3 says exactly why those are a different kind of thing.

## 1. What each shape prices

Every shape is deterministic, takes its iteration count as a **literal**, and ends in an expression
statement whose value is the answer. Ten of them are the measured population; `fresh-small.js` is
not (section 5).

| Shape | What it prices |
|---|---|
| `numeric-loop.js` | Arithmetic between transfers: the most instructions per branch of anything here, and so the shape most exposed to how often emitted code calls back into the dispatch. |
| `property-loop.js` | Ordinary data-property reads and writes — no accessor, no Proxy — which is what most programs are mostly made of. |
| `call-loop.js` | One call per iteration into a callee that does nothing: short runs between dispatches. |
| `fib.js` | Recursion, where the call *is* the work. 7,049,155 calls, and almost nothing between them. |
| `string-loop.js` | Text, whose charge grows with its input. A **control on the set**: a form that moved every shape by the same proportion would be moving something other than the dispatch. |
| `closure-loop.js` | A call through a variable, into a body whose state lives in an enclosing scope. |
| `try-loop.js` | An exception region entered and left per iteration, with nothing thrown. |
| `throw-loop.js` | A throw caught per iteration, across a call boundary: the one path where control leaves an instruction for a handler rather than for its successor. |
| `array-loop.js` | Indexed reads over a dense thousand-element array, in a nested loop. |
| `generator-loop.js` | `for`-`of` over a generator: the iterator protocol, and a resumption at a point that is not a function's start. |

## 2. The literals, and the argument for them

**The floor is fixed once, for every shape, and it is stated as a count of charged work rather than
as a time: a run must charge at least 100,000,000 units of fuel.** The argument is a ratio between
two kinds of work and mentions no clock:

- The measured quantity is a run lane minus a check lane. What the subtraction removes is everything
  the two lanes share — starting the process, reading the file, parsing, lowering, emitting,
  verifying — and all of that is bounded by the **size** of the program, which is under a few hundred
  instructions for every shape here.
- So a run that charges 10<sup>8</sup> units performs something like six orders of magnitude more
  work than the compiling and verifying the subtraction removes. What a subtraction cannot remove is
  that shared part's **variance**, which is not expressible as a count of work at all — it is why the
  A/A lane is measured at all, and why a difference smaller than it is reported as below resolution
  rather than as a result.
- Nothing above was chosen after a trial run. `docs/baselines.md` admits **no pilot phase and no
  adaptive iteration count**, and section 17 of `roadmap.gates.md` inherits that; a literal sized by
  "until it takes about a second" is exactly the policy those rules exclude.

| Shape | Charged fuel per iteration | Literal | Charged fuel for the run |
|---|---|---|---|
| `numeric-loop.js` | 24 | 5,000,000 iterations | 120,000,019 |
| `property-loop.js` | 30 | 4,000,000 iterations | 120,000,026 |
| `call-loop.js` | 29 | 4,000,000 iterations | 116,000,022 |
| `fib.js` | 16.5 per call | `fib(32)`, which is 7,049,155 calls | 116,311,065 |
| `string-loop.js` | 27, and 4 more on each 65th iteration, when the string is reset | 4,000,000 iterations | 108,246,172 |
| `closure-loop.js` | 32 | 4,000,000 iterations | 128,000,039 |
| `try-loop.js` | 29 | 4,000,000 iterations | 116,000,019 |
| `throw-loop.js` | 35 | 3,000,000 iterations | 105,000,022 |
| `array-loop.js` | 25,023 per pass (a thousand inner iterations) | 4,000 passes | 100,118,032 |
| `generator-loop.js` | 41 | 3,000,000 iterations | 123,000,071 |

## 3. Where those figures come from, and what kind of figure they are

**A fuel figure is exact, machine-independent and a property of the program.** It is the same in
both output forms and on every machine, it is what the fuel-parity condition of the harness bisects,
and it is not a measurement of speed. That is why it may be read and written down here while no
wall-clock figure may.

Each per-iteration figure was obtained by **bisecting the smallest allowance the program completes
under** at two small literals and differencing them, then **verifying the model at a third literal**,
where the prediction had to be exact rather than close. Nine of the ten are exactly linear and were
confirmed again at the twin's literal in section 4. `string-loop.js` is the tenth: its reset branch
is taken once every 65 iterations, so its charge per iteration is 27 with 4 more on each reset, which
is why its twin costs 27,080 rather than 27,020. The run figures in section 2 are derived from those
per-iteration figures and the literal; they are not separately bisected, because bisecting a
hundred-million-unit run would mean running it forty times to learn something arithmetic already
says.

**Two departures from the design this folder was written from, stated rather than left to be found:**

1. The design asks for **the instruction count** one iteration lowers to, *read from a compiled
   bytecode listing*. **No tool in this tree prints one.** The CLI has no listing option, and the
   check that would produce one — a plain width walk over a golden artifact's code section — belongs
   to the change this measurement is collected to weigh, which lands after this folder if it lands at
   all. So the figure stated here is **charged fuel**.
2. Fuel is **one unit per instruction plus extra units** for the families whose cost grows with their
   input or whose work is larger than an instruction's (`JsEngine.ChargeText` and the flat charges
   beside it). So a per-iteration fuel figure is charged *work*, and an **upper bound** on the
   instruction count rather than the count itself. It is the right unit for sizing a workload either
   way, because what a literal has to be large enough to dominate is work.

## 4. The twins, and the fuel-parity condition

Each shape has a `.small.js` twin: the same program with a smaller literal. The harness bisects the
twin's smallest completing allowance under the control's bytecode lane and then requires **all four
arm-and-form combinations to complete at that one figure and to refuse with `AllowanceExhausted on
Fuel` at one less**. Two builds that charged differently would be two different workloads, and this
is the condition that says they are not.

| Twin | Literal | Smallest completing allowance |
|---|---|---|
| `numeric-loop.small.js` | 1,000 | 24,019 |
| `property-loop.small.js` | 1,000 | 30,026 |
| `call-loop.small.js` | 1,000 | 29,022 |
| `fib.small.js` | `fib(12)`, which is 465 calls | 7,680 |
| `string-loop.small.js` | 1,000 | 27,080 |
| `closure-loop.small.js` | 1,000 | 32,039 |
| `try-loop.small.js` | 1,000 | 29,019 |
| `throw-loop.small.js` | 1,000 | 35,022 |
| `array-loop.small.js` | 2 passes | 76,078 |
| `generator-loop.small.js` | 1,000 | 41,071 |

Every one of those figures was the same under both forms when this folder was written, which is what
the condition re-establishes on the two arms before any of them is timed.

## 5. `fresh-small.js` is not in the population

It is the fresh-process shape, and the harness reaches it only under `--fresh`. **No decision rule
judges it**: what it asks is what a form costs to *start*, which is the one question the run-minus-
check subtraction deliberately removes from every other shape here.

## 6. No answer is written in a comment

`../README.md`'s kernels state theirs, and these do not. The reason is that nothing checks a comment:
the harness's condition establishes that **all four arm-and-form combinations print the same thing**
and then holds every timed lane to that, so a wrong figure in a comment here would be a sentence no
run could contradict. The values are large and not hand-derivable for most of these shapes, and a
plausible wrong one is worse than none.

## 7. What these shapes are not

- **One convention.** The native lane is `x86-64-win64`, which is the convention this host arms.
  `arm64-aapcs64` emits and is never armed, so it cannot be a lane.
- **Not a benchmark suite.** There is no framework here, no warm-up policy of their own, no outlier
  policy, and no claim about any other engine or component in either direction.
- **Not a gate.** Nothing in a release gate reads these, and no test asserts anything about their
  answers.
