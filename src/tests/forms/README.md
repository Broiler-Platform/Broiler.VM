<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The numeric kernels the two output forms are compared over

`broiler.javascript.numeric` is a feature manifest and not an optimisation setting: it admits
Number values, the arithmetic and comparison and unary operators, `let` and `const` bindings of
numbers, the ordinary statements, and functions of numbers called by name — and it refuses
everything else **by name, at compile time**. The native output forms admit a smaller surface
still. Six kernels live here because nothing this repository already holds can be compiled to
both forms.

**Octane cannot be used for this comparison, and that is a fact about Octane rather than a gap in
the harness.** Every one of the fifteen benchmarks is written in the whole language: objects,
arrays, strings, closures, prototypes, `try`/`catch`, regular expressions. `richards.js` reaches a
`new` expression in its first executable statement, and assigns to a constructor's prototype by
line 126 *(corrected 2026-09-08: this sentence read "`richards.js` declares a constructor and
assigns to its prototype in its first twenty lines", and the first twenty lines of that file are
the V8 copyright header with no code in them at all — its first statement is `var Richards = new
BenchmarkSuite(...)` at line 38 and its first prototype assignment is
`Scheduler.prototype.addIdleTask` at line 126. The demonstration below has always printed
`2104:ConstructOutsideManifest at 38:16` first, so the sentence and the run it introduces named
different lines; the corrected halves are both what the run names)*. The numeric manifest refuses
each of those constructs by name at compile time, so there is no version of "Octane under the
native form" to run — not a slow one, not a partial one. `eng/compare-forms.py` demonstrates this
rather than asserting it: it compiles a named Octane file under the numeric manifest and prints
the host's own refusal.

**What the native x86-64 backends additionally refuse**, measured against these kernels on
2026-09-07 and stated here so a reader adding a kernel is not surprised:

| Construct | Bytecode form | Native `x86-64-*` |
|---|---|---|
| `&&`, `\|\|`, `?:` | refused by the manifest | refused by the manifest |
| `%` | admitted | refused — the backend emits no template for a floating-point remainder |
| `\|`, `&`, `^`, `<<`, `>>`, `>>>`, `~` | admitted | refused — `ToInt32` is a modular reduction the one available conversion does not perform |
| a top-level `var` binding | admitted | refused — it holds `undefined` before its assignment and the frame is a slab of doubles |

`arm64-aapcs64` refuses all six kernels at `DeclareGlobal`, before any of the above: it is the
**emitting-only** backend, it is never armed, and an artifact carrying its bytes refuses to
instantiate. It is not a lane in this comparison and cannot be one.

Every kernel is deterministic, takes its own iteration count as a literal, ends in an expression
statement whose value is the answer the comparison checks, and is sized so that the bytecode lane
spends roughly three quarters of a second — long enough that the process floor is not the
measurement, short enough that ten repetitions of six kernels is a coffee.

**No figure produced from these kernels is retained anywhere in this repository.** There is no
baseline register row for either output form; creating one is a bundle-collection act
`docs/mvp.md` defers.
