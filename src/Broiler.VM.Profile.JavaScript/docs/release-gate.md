<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# The JavaScript profile's release gate

**Verdict: REFUSED.**

This is the register of what the release gate refuses on, under proposed
[JSD-0062](decisions/0062-the-release-gate-that-refuses.md), phase F9's slice R4. The gate,
`JsReleaseGate` in the architecture test assembly, reads roadmap
[section 22](roadmap.gates.md#22-release-gates)'s gates 1 to 13 from the checkout; rule **N38** holds
this register to the blockers the gate names, in both directions, and to its verdict. Clearing a row
here without clearing it in the checkout fails the rule, and so does leaving a row the checkout has
cleared.

**Running it.** `BROILER_RELEASE_GATE_REPORT=<path> dotnet test src/tests/Broiler.VM.Architecture.Tests
--filter N38_The_Gate_Report` writes the gate's report, every blocker with its declaration, to the
path. The count of undecided units is in that report, read live, and not here.

**It cannot be argued past.** A human decision is counted from each relevant unit's own
`// Broiler-Human:` line and from nothing else; a headline or a table that claims more is itself a
blocker, and rule N38's two witnesses show the gate refusing both.

## The gates

| Gate | State | Held by |
|---|---|---|
| 1. Support truth | Blocked | N34, N35, N36 |
| 2. Graph and registration | Held | A7, N1, N2, B5, B5b |
| 3. Correctness and safety | Blocked: unread | V9, N5, N6, N7, N11 in part |
| 4. Lifecycle and results | Blocked: unread | V8 in part |
| 5. Guest loads and policy | Blocked: unread | - |
| 6. Host boundary | Blocked: unread | - |
| 7. Native AOT | Blocked | K1 to K5 |
| 8. Packages and consumers | Held | N37, A14 |
| 9. Conformance | Blocked | N15, and release-candidate bundle [JS-10-003](evidence/js-10-003/README.md) read file by file |
| 10. Measurement honesty | Held | N33 |
| 11. Human review | Blocked | J3, J4, J11 |
| 12. Licence and attribution | Blocked | N22, N27 |
| 13. Operations | Blocked | - |

## The blockers

| Blocker | Gate | Declaration | What clears it |
|---|---|---|---|
| `G1-not-issued` | 1 | [`support.md`](support.md), its banner | The owner issuing the table, after gate 11 |
| `G3-unread` | 3 | Gate 3's corpus, fuzz and post-verification clauses | A mechanism reading the bundles that show them, or the review of gate 11 reading them |
| `G4-unread` | 4 | Gate 4's step-kind, exception, suspension and stack-overflow clauses | The same |
| `G5-unread` | 5 | Gate 5's three clauses | The same |
| `G6-unread` | 6 | Gate 6's five clauses | The same |
| `G7-no-composition-advertised` | 7 | [`docs/compositions.md`](../../../docs/compositions.md) section 1 | The owner advertising a composition |
| `G7-no-rid-claimed` | 7 | [`support.md`](support.md) section 8 | The owner claiming a RID a retained bundle published and ran, after gate 11 |
| `G9-wide-native-ratchet` | 9 | [`evidence/js-10-003/wide-native.floor.log`](evidence/js-10-003/wide-native.floor.log): the native floor was set in the `x86-64-win64` form and this run is `x86-64-sysv`, so the two were not compared | A native run on `win-x64` held to the floor, or the owner admitting a floor for the System V form |
| `G11-units-undecided` | 11 | Every relevant unit's `// Broiler-Human:` line | A named human recording a decision on each, bound to its fingerprint |
| `G11-published-before-review` | 11 | [`support.md`](support.md) section 9; ADR 0001's revision of 2026-09-28 | The owner's answer to the two versions published before review |
| `G12-unconfirmed-broiler.vm.profile.javascript` | 12 | [`THIRD_PARTY_NOTICES.md`](../../../THIRD_PARTY_NOTICES.md), the specification row | The release owner co-signing the scoping |
| `G12-unconfirmed-broiler.vm.profile.javascript-broiler.vm.profile.javascript.format` | 12 | The same file, the Unicode Character Database row | The same |
| `G12-unconfirmed-broiler.vm.composition.javascript.cli` | 12 | The same file, the Octane row | The same |
| `G12-no-row-broiler.vm.profile.javascript.intl` | 12 | The same file's ingesting-components table, which has no CLDR or tzdb row | A row for the Intl assembly, confirmed |
| `G13-vacant-diagnostics` | 13 | [`support.md`](support.md) section 10 | The owner naming a holder |
| `G13-vacant-cancellation` | 13 | The same | The same |
| `G13-vacant-rollback` | 13 | The same | The same |
| `G13-vacant-format-version-rejection` | 13 | The same | The same |
| `G13-vacant-corpus-drift` | 13 | The same | The same |
| `G13-vacant-suite-revision-drift` | 13 | The same | The same |
| `G13-no-security-intake` | 13 | The same, the vulnerability-response row | A security contact and an intake channel |

## What this register does not say

- **That the held gates are reviewed.** Held means the named rules are `Active` and pass; no human has
  read what they hold.
- **That clearing every row is a release.** It is what this check reads. Release gate 11's reviewer
  reads the rest.
