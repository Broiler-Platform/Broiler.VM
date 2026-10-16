<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Bundle SJL-7-001 - the Unicode area: Broiler.VM's verdict, rules and evidence

**Milestone:** SJL-7 of the shared JavaScript libraries programme,
[`sjl-7-unicode.md`](../../javascript-shared-libraries/milestones/sjl-7-unicode.md).

**Rule:** [`decision-rule-area.md`](decision-rule-area.md), written at SJL-7.1 before any code of the milestone
existed, with its probes and workloads in `src/tests/forms/unicode`.

**Records:** [JSD-0074](../../../src/Broiler.VM.Profile.JavaScript/docs/decisions/0074-unicode-moves-to-broiler-unicode-under-the-owner-conditions.md),
whose input is [`correspondence.md`](correspondence.md).

**Owner and reviewer:** one person holds every role (EX-30), and nobody independent has read this bundle.

**State:** rules and correspondence only, 2026-10-16, at SJL-7.1. **Nothing here is a verdict, and nothing has
been taken.**
- The base of all three parts is taken at the commit that carries the rule, before any code of SJL-7 lands.
- The after runs follow SJL-7.4's cut-over.
- A collector joins this bundle when there is a run to take.

The owner's clauses are judged in Broiler.Unicode's bundle of the same name, and Broiler.JS's in its own. This
bundle reads neither's runs.

## 1. SJL-7's area rule and its correspondence (SJL-7.1)

**The correspondence.** It was read from code in both engines, in Broiler.Unicode and in Broiler.Regex, at the
revisions it names. It numbers every difference the area verdict classifies (P, I, W, F, C, N, S, G), and every
behavioural cell its rows rest on was run in both engines. **This family's own corrections in the area are
three:**
- its casing ceiling is checked on the whole result;
- so is its normalization ceiling;
- its segmenter charges every code point it scans.

The rest of the differences correct Broiler.JS, or are SJL-11's.

**The rule has three parts.**
- **Part A** takes test262 in both forms with the hosted workflow at the base and after the cut-over. It
  admits a moved exhaustion verdict as class C2 on `test/intl402/Segmenter/` alone, where JSD-0074's decision 4
  changes segmentation's charging units, and nowhere else.
- **Part B** holds the wide twins and the Unicode probes to their base figures, with no class.
- **Part C** is the measurement that answers JSD-0031's fifth reason for this engine. Its three workloads
  answer with counts of completed operations, so that no correction can fail its condition.
