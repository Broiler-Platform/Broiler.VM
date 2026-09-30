<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# Record JSP-9-001

**What this is:** a working record of the JavaScript profile. It retains the evidence for
[the parity roadmap](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.parity.md)'s JSP-9. That
stage asks that section 4.9's catalogue of what must not be taken from the comparison engine be kept
as declared divergences. The driver must report each as declared, and report it stale once it stops
holding. The poisoned global store must also be recorded where a person taking a comparison run
will meet it.

**It moves no row of [the profile's ledger](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.status.md).**
JSP-9 is not a ledger row, it has no owner, and a stage's gate met in a working tree is not
acceptance.

**Collected:** 2026-09-30, at commit `7d26f0d`, from a clean tree, by `collect.py` in this directory.
- **The change:** `a7732d8`.
- **This directory's tooling:** `7d26f0d`. It changes no product, harness or test file.
- **The retaining commit** adds this record's logs and README.

## 1. Identity

| Field | Value |
|---|---|
| Record | JSP-9-001 |
| Stage touched | JSP-9 (every clause of the gate) |
| Correction | [JSC-235](../../../src/Broiler.VM.Profile.JavaScript/docs/roadmap.corrections.md#jsc-235) |
| Instrument | `eng/run-differential.py` and the probes under `src/tests/differential` |
| Comparison engines | Broiler.JS at commit `c249764` (`c24976423c2b5938bbb06c11f9310ff45c9275c8`), built Release from a read-only clone; Node `v22.22.2` for the probes this change added or changed |
| RID and mode | `linux-x64`, framework-dependent JIT only |
| Owner | the JavaScript profile owner; one person holds every role |
| Reviewer | none |

## 2. What each run shows

| Log | What it shows |
|---|---|
| `build.log` | the solution built with warnings as errors, with the commits of this checkout and of Broiler.JS |
| `tooling.log` | the driver's own tests, every one passing, including the six added for a run declaration |
| `retained.log` | every probe against its retained answers, exit 0: the two new probes and the rewritten case 74 answer what was retained |
| `broiler-js.log` | every probe against Broiler.JS: each declared divergence printed with its reason, a declared run with its failure beside it, and the findings outside the catalogue |
| `catalogue.log` | that run judged. **Every one of the 58 `broiler-js` declarations in the answer files was reported**, as a declared divergence or as not checked under a declared run, and **none was stale.** It also lists the findings outside the catalogue by probe |
| `node.log` | the catalogue probe, the recursive built-ins and the async family against Node, each exit 0: the host and Node agree on every case declared against Broiler.JS there |
| `tests.log` | the solution's test projects, every test passing |

**The run against Broiler.JS still exits 1, and that is its expected answer.** It reports 430 findings
in 46 probes that the catalogue does not cover. About half are in the generated Unicode
normalisation probes, most of them cases Broiler.JS never prints. Most of the rest are in surfaces
added to this realm after the survey section 4.9 records.
Nobody has adjudicated them against the language, and some may be this host's. The declarations
claim only what section 4.9 claims.

The run declarations cover five probes:
- `the-with-statement.js`, whose cases the poisoned store numbers again;
- `the-recursive-built-ins.js`, where the process ends with a stack overflow;
- the two module probes that time out on the pending-promise hang;
- the asynchronous iteration probe and the async family, whose cases settle in another order.

The last two are still compared case by case, which the log shows.

## 3. The controls

Each control is a patch in this directory. `control-<name>.log` applies it and runs the probe it
touches against Broiler.JS. It checks that the driver exits non-zero with the answer named below,
then reverts the patch.

| Control | What it changes | What the driver reported |
|---|---|---|
| `control-case-declaration-removed` | the declaration for the catalogue's case 8, the first poisoned-store case | `undeclared divergence broiler-js/8` |
| `control-run-declaration-removed` | the run declaration on `the-with-statement.js` | `duplicate case 54`, the poisoned store numbering the cases again |
| `control-stale-case-declaration` | a declaration for a case both engines answer alike | `stale divergence broiler-js/1` |
| `control-stale-run-declaration` | a run declaration for a run that completes in the host's order | `stale divergence broiler-js/run` |
| `control-driver-without-run` | the driver's reading of `run` removed | `use #diverges <engine> <case> <reason>`: the run declaration is refused as malformed |

`after-controls.log` runs the three controlled probes again once every patch is reverted, each exit 0.

## 4. What this record does not demonstrate

- **Acceptance, an owner, a reviewer.** Nothing here was read by anyone but its author, and no row of
  the ledger moves.
- **The findings outside the catalogue.** They are counted, not adjudicated.
- **Any other commit of Broiler.JS.** The declarations hold against `c249764`. A later commit may make
  any of them stale, and the driver would say so.
- **The platform-scoped entry.** `JSON.stringify`'s line ending is the platform's in Broiler.JS by
  its source, which on `linux-x64` is the language's. It was not observed and is not declared.
- **Other platforms.** Framework-dependent JIT on `linux-x64` only.

## 5. What was run

```text
python3 docs/evidence/jsp-9-001/collect.py --broiler-js <BroilerJS apphost> \
  --broiler-js-root <Broiler.JS checkout at c249764> --node <node>
```

The Broiler.JS apphost was built with
`dotnet build Broiler.JS/Broiler.JavaScript/Broiler.JavaScript.csproj -c Release`.

## 6. Environment

- Ubuntu 24.04 on `x86_64`;
- the .NET SDK `10.0.401`;
- Python 3;
- Node `v22.22.2`.
