<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The frontend area's compile workloads

These files are the population of part J of bundle `sjl-13-001`'s [area rule](../../decision-rule-area.md), this
family's compile measurement of milestone SJL-13 of the shared JavaScript libraries programme, which moves this
family's wide path onto the shared syntax library. Part F of bundle `sjl-13-002`'s
[rule](../../../sjl-13-002/decision-rule-area.md) reads the same files. The verdict is
[JSD-0083](../../../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0083-the-shared-syntax-library-and-the-wide-path.md),
whose decision D24 says how part J's verdict is recorded: it is not a gate clause. **No figure produced from these
files is retained anywhere in this repository except in the bundles' `measurement/` sub-bundles.**

They were written, and committed with the rule, before any code of SJL-13 existed: no shared library, no tools commit
and no measurement harness. The slice compiler's `--measure-compile` mode, which the tools commit adds, compiles each
of them through `JsCompiler.Compile` under its goal, in the bytecode form, and times the compilation, not an
execution. No build was taken at SJL-13.1, so this family first compiles them at part J's base arm, where the rule's
condition checks every outcome and artifact hash before and after every lane.

| Workload | Control | Goal | What it prices |
|---|---|---|---|
| `expressions.js` | `expressions.control.js` | Script | Operators of every precedence level, conditionals, logical and nullish operators, short member chains, optional chaining, object and array literals, compound assignment |
| `declarations.js` | `declarations.control.js` | Script | Functions with default and rest parameters, array and object destructuring with initialisers, arrow functions, generators, async functions, block-scoped declarations |
| `classes.js` | `classes.control.js` | Script | Classes with public and private fields and methods, static fields and static blocks, accessors, computed method names, derived classes with `super` calls |
| `statements.js` | `statements.control.js` | Script | `if`, `switch`, `for`, `for-in`, `for-of`, `while` and `do-while`, a labelled loop with `break`, `try`, `catch` and `finally`, blocks with lexical declarations |
| `literals.js` | `literals.control.js` | Script | String literals with escapes, template literals with simple substitutions, numeric literals in every radix with separators and exponents, regular-expression literals, nested object and array literals |
| `module.js` | `module.control.js` | Module | Exported function and `const` declarations, classes with private fields and accessors, export lists with renaming |

**Each control is the same text as one block comment**, of the same goal: the same characters reach the tokenizer,
and no construct reaches the parser or the lowering. The difference between a workload and its control is the cost
of the constructs; the rule retains it as attribution and decides nothing by it.

**Each workload repeats one block of source with its names numbered**, so that it compiles long enough to time, and
stays well inside the format's ceilings (the constant pool, the locals of a frame) and the nesting bounds.

**What the workloads stay away from**, so that the base and the after commit give one outcome and one artifact for
each, which the rule's condition requires:
- **the rows of the correspondence ([`correspondence.md`](../../correspondence.md)) where step 1 changes an outcome or
  a byte**: no comment, hashbang, HTML-like comment or regular expression inside a template substitution (T6, T7, T8);
  no white space inside an escape's digits (T13); no legacy octal literal (T16); no regular expression after a
  `for await (…)` head, a prefix update or an `await` used as an identifier (T24d to T24f); no import attribute (P05a);
  no parenthesised nested assignment target and no rest property whose target is a literal (P15b, P31); no
  `new.target` (D32); no import at all (D40);
- **the rows step 1 leaves a difference in, or hands on**: no `await` inside an async arrow at a module's top level
  (P32's item 10), no label set (D30), no regular-expression pattern on which the two validators disagree (R19, R20),
  and no dynamic function (P26);
- **the limits**: no chain or nesting near the tree-depth or nesting bound, and none of the four left-operand shapes
  row R14 names;
- **anything the admission walk refuses or a manifest declines**: no BigInt literal, no module specifier, no
  decorator, `accessor` or import phase, no `1m`.

Every file is ASCII; the one character outside it a workload names is written as an escape.
