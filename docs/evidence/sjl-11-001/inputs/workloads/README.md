<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The regular-expression area's measurement workloads

These files are the population of part E of bundle `sjl-11-001`'s
[area rule](../../decision-rule-area.md), this family's candidate-versus-control measurement of milestone SJL-11 of
the shared JavaScript libraries programme, which moves this family's regular expressions onto Broiler.Regex. The
verdict is
[JSD-0081](../../../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md),
whose decision D19 says how part E's verdict is recorded: it is not a gate clause. **No figure produced from these
files is retained anywhere in this repository except in bundle `sjl-11-001`'s `measurement/` sub-bundle.**

They were written, and committed with the rule, before any code of SJL-11 existed (JSD-0081 decision D17). They are
written for `broiler.javascript.wide`, the command-line root's manifest, and each program's completion value is its
answer. No build was taken at SJL-11.1, so this family first runs them at part E's base arm, where the rule's
condition checks every answer before and after every lane.

| Workload | Control | What it prices |
|---|---|---|
| `scan.js` | `scan.control.js` | A global search for a literal across a long ASCII text, and a search for a literal that first occurs at the text's end |
| `captures.js` | `captures.control.js` | `exec` with named groups and the `d` flag: five named groups over each line of a log, and a global pattern with one named group run again from each line's start |
| `replace.js` | `replace.control.js` | `replace` and `replaceAll` with a global pattern: a template with numbered references, a template with named references, and a replacement function |
| `split.js` | `split.control.js` | `split` by separators with white space around them, by a capturing pattern, and with a limit |
| `unicode.js` | `unicode.control.js` | Property classes under `u` (General_Category, Script, a complement), a class difference, a class intersection and a property of strings under `v`, and case-insensitive literals and ranges under `iu`, `iv` and `i`, over Latin with diacritics, Greek, Cyrillic, Han, ASCII and emoji |
| `backtrack.js` | `backtrack.control.js` | Bounded backtracking: a nested quantifier anchored at both ends, a starred alternation of overlapping choices, a repeated word through a back-reference, quoted strings closed by a back-reference behind a negative look-ahead, and a palindrome through named back-references |

**Each control is the same workload minus the thing measured**: the same strings, patterns, loops and accumulation,
with each regular-expression call replaced by a type test of its input. The difference between a workload and its
control is the cost of the calls; the rule retains it as attribution and decides nothing by it.

**Each answer counts the operations that completed, not their results.** SJL-3's measurement of this family failed
its condition because a correction moved a workload's answer; bundle `sjl-7-001`'s part C answered with counts for
that reason, and these do the same. A count is fixed by the loop bounds written in each file, so no correction of
either engine changes it.

**What the workloads stay away from**, so that every arm can run every one of them to its count:
- **the rows of the correspondence on which the engines' answers differ** ([`correspondence.md`](../../correspondence.md)):
  no group name is declared twice (S1), no `\k` stands in a class (S5), no lead-surrogate escape precedes a braced
  escape (S10), no `lastIndex` is set inside a surrogate pair (M4), no back-reference runs under `u` or `v` (M7), no
  pattern's source holds a line terminator (T3), no subject holds a line terminator for `^` or `$` (M3), and no
  case-insensitive pattern or subject holds U+0130, U+0131, U+212A, U+1F80, U+1F88, U+A7CE or U+A7CF (M2, M2a, M2b,
  U3);
- **the ceilings and the nesting limit** this family keeps (JSD-0081 decisions D7 and D10): no greedy quantifier over
  a single character or a group runs over a long subject (rows B1 and B6), and no pattern nests deeper than a few
  levels;
- **anything that is not this area's**: no `Intl`, no host capability and no output.

Every character outside ASCII is written as an escape, so each file is ASCII.
