<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The Unicode area's fuel-parity probes and measurement workloads

These files are the populations of parts B and C of bundle
[`sjl-7-001`](../../../../docs/evidence/sjl-7-001/decision-rule-area.md)'s area rule. Milestone SJL-7 of the
shared JavaScript libraries programme moves this family's Unicode algorithms to Broiler.Unicode:
- the property and lexical lookups;
- case folding and case mapping;
- normalization;
- the properties of strings;
- segmentation.

The verdict is
[JSD-0074](../../../Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md).
The family keeps charging fuel for normalization and casing through its own meter, in its own units. **No
figure produced from these files is retained anywhere in this repository except in bundle `sjl-7-001`.**

They were written, and committed with the rule, before any code of SJL-7 existed. Each compiles under
`broiler.javascript.wide`, the command-line root's manifest, and gives the same answer in both output forms.

## The probes (part B)

**They exist to show that the cut-over moves no charge.** Each probe's smallest completing fuel allowance is
bisected at the milestone's base commit. After the cut-over, every probe must complete at that figure and
refuse at one less, in both output forms.

| Probe | What it charges |
|---|---|
| `normalize.small.js` | `normalize` in its four forms and its default, over canonical and compatibility decompositions, composition with blocking, canonical reordering, Hangul both ways, composition exclusions, singletons, supplementary-plane mappings, Unicode 16's decompositions, ASCII and lone surrogates |
| `casing.small.js` | `toUpperCase`, `toLowerCase`, `toLocaleUpperCase` and `toLocaleLowerCase` over one-to-one and one-to-many mappings, title-case letters, final sigma in and out of its context, supplementary-plane letters, Unicode 16 and 17's mappings and lone surrogates |
| `properties.small.js` | the matcher's property escapes by General_Category, Script, Script_Extensions and binary property, a set operation and a property of strings under `v`, folding under `iu` and the canonicalisation under `i`, and identifiers outside ASCII compiled through the Function constructor |

**Every probe's answer is one the cut-over does not change.** The family's three corrections in the area
verdict are of no answer these probes reach:
- its casing and normalization ceilings, checked on the whole result, matter only at 2^24 code units, and no
  probe comes near that;
- its segmenter's charging is not reached: `Intl.Segmenter`, like `Intl` and the locale's effect on casing, is
  outside `broiler.javascript.wide`.

So the casing probe's `toLocale*Case` calls take the realm-without-Intl path, which reads no locale. Every
property name is written exactly as the specification's tables give it. A probe whose answer changed after the
cut-over would be a defect of the probe or of the owner's code, and part B reads it as a moved verdict.

## The workloads (part C)

| Workload | Control | What it prices |
|---|---|---|
| `normalize.js` | `normalize.control.js` | `normalize` in its four forms over strings in and out of each form: Latin with marks, Hangul, compatibility characters, Greek, supplementary-plane mappings and ASCII |
| `casing.js` | `casing.control.js` | `toUpperCase`, `toLowerCase`, and `toLocaleUpperCase` and `toLocaleLowerCase` with a Turkic and a Lithuanian locale, over scripts with one-to-many mappings, final sigma and supplementary-plane letters |
| `identifiers.js` | `identifiers.control.js` | the compiler's identifier scanning outside ASCII: a body declaring and reading identifiers in six scripts, with Other_ID_Start and Other_ID_Continue characters, a joiner and supplementary-plane letters, compiled again on every iteration through the Function constructor |

**Each control is the same workload minus the thing measured:**
- for the runtime workloads, the same strings and loops with each operation replaced by a type test of its
  input;
- for the identifiers, the same body compiled the same number of times with every identifier in ASCII of the
  same length.

**Each answer counts the operations that completed, not their results.** SJL-3's measurement of this family
failed its condition because a correction moved its answer. Counting completed operations keeps the answer
the same at the base and after, and a lane that throws or stops early still fails it.

**The probes and workloads are not changed after they are committed.** A change would change the population
the rule was written over, and the rule's own section on revisions says what that costs.
