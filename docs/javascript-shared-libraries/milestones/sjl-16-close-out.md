# Milestone SJL-16 — Close-out: records, support tables and the maintenance model

[← Roadmap](../roadmap.md) · [Ledger](../status.md)

- **Objective.** Every record in every repository describes the end state, and three things are
  confirmed against the trees:
  - every area is either shared or refused with its record;
  - no deletion list has a surviving member;
  - each engine's support documents say where its algorithms and data come from.

  The maintenance model is written. What the programme does not plan stays recorded as not planned.
- **Owner roles.** Every owner named in the programme. One person.
- **Repositories.** Every repository the programme touched.
- **Waits on.** Every milestone from SJL-0 to SJL-15, and SJL-17, is met or `Refused`.

---

## Work packages

### SJL-16.1 — The sweep

A script, retained in the bundle, checks every area against the trees at named commits:

- **The deletion list** is checked against Broiler.VM's sources through rule `I5`, against
  Broiler.JS's sources, and against the component repositories.
- **Every refused area's source pointers** still exist.
- **Every admitted package** is on the allow-list. Every allow-list entry is referenced.
- **Every shared library's `SL` rules** are active in its home.

### SJL-16.2 — The records

The following are revised to describe the end state, each with its superseded text quoted:

- **Broiler.VM:**
  - ADR 0014;
  - the register rows;
  - the profile's plans, through JSC entries;
  - `docs/compositions.md`.
- **Broiler.JS:**
  - `dependencies.md`;
  - `public-api.md`;
  - the shared-libraries plan pair, set to its final state.
- **The component repositories:** their roadmaps.

### SJL-16.3 — The support documents

These documents name the shared library or component behind each fixed surface, with the data
versions **as pins and not as figures**:

- Broiler.VM's `src/Broiler.VM.Profile.JavaScript/docs/support.md` (drafted, not issued, under
  `docs/mvp.md`);
- Broiler.JS's compliance documents.

No cell claims more than its evidence.

### SJL-16.4 — The maintenance model

One document in the home, linked from every repository, states:

- who owns each library;
- the release cadence;
- the procedure for a data-version move, recorded as a decision and followed by one change per
  engine;
- the security intake;
- the defect-routing rule of [governance.md](../governance.md#6-the-cross-repository-change-flow);
- the co-hosting policy;
- how a new area joins through the extraction gate.

### SJL-16.5 — Confirmation of what is not planned

Two decisions are confirmed in ADR 0014's revision and in Broiler.JS's plan, or reopened on new
evidence:

- [SJL-D-18](../decisions.md#sjl-d-18): whole-runtime unification;
- [SJL-D-17](../decisions.md#sjl-d-17): the non-standard extensions.

The responsibilities still present in only one engine are listed with a note on whether either
engine could adopt them later; the list is in [inventory.md](../inventory.md). Adopting them is
future work, not this programme's.

---

## Exit gate

1. The sweep is retained and reports no surviving deleted member, no missing pointer, no orphaned
   allow-list entry, and no inactive `SL` rule.
2. Every record of SJL-16.2 is revised, with its superseded text quoted.
3. The support documents name their sources. No cell claims beyond its evidence.
4. The maintenance model is filed and linked from every repository.
5. Decisions `SJL-D-17` and `SJL-D-18` are confirmed or reopened on the record.

---

## Evidence bundles

`docs/evidence/sjl-16-001/` in Broiler.VM and in Broiler.JS: the sweep script, its output and the
record diffs.

---

## Does not do

- **It publishes nothing and advertises nothing.**
- **It accepts nothing on review's behalf.**
- **It starts no new area.**
