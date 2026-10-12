# Bundle sjl-2-001: the admission path, Broiler.VM's half

**Milestone:** SJL-2 of the shared JavaScript libraries programme,
[`milestones/sjl-2-the-admission-path.md`](../../javascript-shared-libraries/milestones/sjl-2-the-admission-path.md):
its Broiler.VM clauses 1 to 6, work packages SJL-2.1 to SJL-2.9. **Subject:** the commit that carries
this bundle's rules, records and tooling, named in [`manifest.json`](manifest.json). **Collected:**
2026-10-12. **Owner:** the architecture owner, the JavaScript profile owner and release owner R6, all
one person. **Reviewer:** none; nothing here was read by a second person.

SJL-2 has no predeclared decision rule (SJL-0.9 filed rules for the milestones that compare engines),
so each clause is read against the milestone file's exit gate as written. **No evidence from
Broiler.JS is cited**: the probe package is restored here, from this repository's committed feed, and
every run below is this repository's.

## Clauses

| Clause | Reading | What the bundle holds |
|---|---|---|
| **1.** Every rule of SJL-2.1 to SJL-2.6 is in the register, with its test and witnesses, each watched failing on injection and passing on revert. Rule J12 holds over the new rows. | **Met** | [`register.diff`](register.diff): the rows minted (I1, I2, I3, I6, I7) and revised (A2, A7, B3, E4, N1, N37). [`injections/`](injections/): each of those eleven rules watched failing on a defect injected into the real tree and passing after the revert, one log per rule. [`tests.log`](tests.log): the architecture suite over the commit, every test passed, the J12 tests and the register's own checks among them. |
| **2.** The admission probe passes I1, I2 and I6, and every negative control of SJL-2.9 fails. | **Met** | [`admission-probe.log`](admission-probe.log): the probe package restored through this repository's NuGet.config into an isolated package folder, again in locked mode, and built; I1, I2 and I6 report nothing; the probe referenced from `Broiler.VM.Runtime`, the lock file's hash changed (I1, and locked mode's NU1403) and a build of the probe referencing `System.Text.RegularExpressions` (I2) are each refused. |
| **3.** The ADR 0001 and ADR 0012 revisions are filed with what each section said before. | **Met** | ADR 0001, revision "2026-10-12 - shared packages admitted, the legacy boundary restated, SJL-2": the outbound boundary, the NuGet.config rationale (already false, and what holds the boundary now), the no-chaining reason, and the package-edge budget by citation of `{graph:package-edges}`. ADR 0012, revision "2026-10-12 - admitted shared packages are inside R4 and R6, SJL-2". |
| **4.** Every register row filed at SJL-0 is `Applied`, and its sentence is revised in place with the superseded text. | **Met** | ADR 0003 section 11, rows 24 to 41 `Applied at SJL-2`, with a fourth editorial revision; `docs/roadmap.md`, each of the eighteen rows' sentences revised in place with its superseded text and the row's number, dated (row 40's three new risk rows added, rows 26 to 28 a scope note beside an unchanged verdict). |
| **5.** A restore through each configured source succeeds, and an unmapped identifier's restore fails. The collectors record I7 identity. | **Met, the collectors' half by exercise** | [`restore-sources.log`](restore-sources.log), from `eng/verify-sources.py`: the committed feed, its lock file's hash equal to the pin; nuget.org; and an identifier outside the shared prefix, packed into a copy of the committed feed, refused with NU1101 and "PackageSourceMapping is enabled, the following source(s) were not considered: broiler-shared". [`collector-identity.log`](collector-identity.log): the identity record the collectors write, from the function they call. **No full collection was run**: no composition carries an admitted package at SJL-2, so a collection would write no record; the first that carries one will. |
| **6.** No product project references a shared package. | **Met** | Rule I1's sweep over every non-test project, in [`tests.log`](tests.log); the allow-list's one entry is admitted test-only, for the probe's scratch project alone; and [`no-product-reference.txt`](no-product-reference.txt), a search of every project file of the checkout for the shared prefix, which finds none. |

## What a reader should know

- **I3 and I7 are `Active`, and each states the clause that has no subject yet.** The programme's
  governance lists I3 as "SJL-2, active from SJL-3", because no frontend package exists to put in a
  closure. The register's activation-milestone check compares milestones within this repository's
  own series (`VM-n`), so a row cannot name `SJL-3` there. I3's other clause, optional data only in a
  root that declares it, is decided over real closures today, and I3 is `Active` for that clause,
  with its row stating that the frontend clause cannot fail before SJL-3. I7 is in the same position:
  it reads every retained bundle with closure listings, none of which carries an admitted package
  before SJL-3, and its row says so. Both are shown refusing their witnesses and their injections.
- **I7's first injection failed for the wrong reason, and the log is the rerun.** Its test keyed a
  bundle's closures by root, and the injected listing named a root the bundle already had, so the test
  crashed instead of reporting. The test now keys by listing, and the rerun reports the missing
  identity as it should.
- **B3 is `Active`, and its permanence reason is withdrawn**, not merely superseded: it rested on a
  single-source NuGet.config making a foreign Broiler package unresolvable, which was false before this
  milestone, because Broiler.JS publishes to nuget.org. The claim is dropped from B3's row and from
  `AssemblyMetadataRuleTests`' remarks; the milestone file and governance row V19 named
  `LegacyBoundaryTests` as holding it, which it never did, and each carries a dated correction.
- **`samples/NuGet.config` is not edited.** It lists nuget.org alone since 2026-10-11, when the samples
  moved to the published packages, so the consumer's isolated, local-only configuration is now written
  by `eng/collect-js-packages.py` for its runs, which also places every admitted package on
  `artifacts/feed`. The milestone file and governance row V20 carry dated corrections.
- **Two rows of ADR 0003's register were incomplete as filed at SJL-0.** Rows 26 and 27 end at "is
  neither". They are not edited; the applied scope notes complete them, and the fourth editorial
  revision says how.
- **The committed feed holds one package**, `Broiler.ECMAScript.Algorithms` `0.0.1-probe.1`, packed in
  Broiler.JS from its commit `a84a0b4` and pinned by `eng/packages/*.pin`. Its nuspec names that
  commit, which is what I7 records. It is test-only and is never published.
- **Rule N37's revision is filed through JSD-0064 and JSC-296**, which revise the package baseline's
  "No foreign dependency" paragraph. No family package declares a shared dependency.
- **Lock files and locked mode.** No product project references an admitted package, so none carries
  a lock file yet; CI's new step restores every project with one in locked mode and says so when there
  is none. The admission probe restores with a lock file and in locked mode, and locked mode refuses
  its changed hash.

## Not in this bundle

Broiler.JS's clauses 7 to 10 are that repository's bundle `docs/evidence/sjl-2-001/`, observed and not
cited. SJL-2.15 was not needed: Broiler.JSeal is unchanged.
