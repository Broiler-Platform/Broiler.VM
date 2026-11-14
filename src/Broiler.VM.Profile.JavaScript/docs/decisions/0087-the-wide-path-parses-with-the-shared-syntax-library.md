<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0087 - The wide path parses with the shared syntax library: SJL-13.3's step 1, registry revision 19 and the frontend's admission

**Status:** Decided 2026-11-10. It applies [JSD-0083](0083-the-shared-syntax-library-and-the-wide-path.md), SJL-13's
area verdict, whose decision D3 asks for "a second record, the cut-over's, at step 1, recording registry revision 19
(governance P15) and the admission, with a dated note at step 2". **The code landed in the commits before this record**,
on the one branch that lands as step 1's one merge (JSD-0083 decision D35), so that the local pre-checks this record
cites could be taken at commits of the code it judges; this record and the other records of step 1 are the branch's
last commits, which change no product source.

**Date:** 2026-11-10.

**Owner:** the JavaScript profile owner. **Co-signers:** the conformance owner, as the holder of
[JSD-0009](0009-the-diagnostic-registry-and-the-position-encoding.md)'s diagnostic registry, which revision 19 revises;
the verification-boundary owner, as the holder of
[JSD-0014](0014-the-source-front-end-and-the-verification-boundary.md)'s decision 1, which JSD-0083's decision D33
reads; and the core architecture owner, for rule I5's register, the package allow-list and the dependency graph. **All
four roles are held by one person** (EX-30), and this record does not claim that anyone independent has read it.

**Milestone:** the shared JavaScript libraries programme's SJL-13
([the milestone](../../../../docs/javascript-shared-libraries/milestones/sjl-13-shared-syntax-and-vm.md)), work package
SJL-13.3's step 1 with SJL-13.4's admission. SJL-13 started before SJL-12, SJL-3 and SJL-7, which it waits on, met their
gates, and it may not meet its gate before they meet theirs; the ledger records both. **The owner's direction of
2026-10-18**, "Please do all recommended decisions and skip waiting for review.", is read as JSD-0083 reads it: the
choices this record makes within the records are its own, recorded below, and nothing in this record is accepted by
it. This record takes no owner's decision of its own; the owner's decisions it applies are JSD-0083's, each recorded
there by date, and the two readings accepted under the direction are recorded in JSD-0083's notes of 2026-11-10 on
decisions D21 and D20, not here.

---

## Context

JSD-0083 accepted all five sub-areas of the front end for the shared syntax library, `Broiler.ECMAScript.Frontend`,
built in Broiler.JS's repository at SJL-13.2 (Broiler-Platform/Broiler.JS#1096, `d4b330d8`, observed), and placed
this family's cut-over in two steps (decision D19). This record is step 1: the wide path parses with the library, an
adapter builds `JsSyntax` from the shared tree so that the lowering does not change, and `JsParser` is deleted with the
binder's copies of the checks the library now makes (decision D20). Step 2, which moves the lowering, the numeric
admission and the binder onto the shared tree and deletes the adapter and `JsSyntax`, files a dated note here.

The evidence that judges it is bundle [`sjl-13-001`](../../../../docs/evidence/sjl-13-001/README.md), under the area
rule SJL-13.1 committed before any of this code existed, as revised before any of its runs on 2026-11-02, 2026-11-04,
2026-11-08, 2026-11-09, later that day by
[`decision-rule-area-revised-2026-11-09b.md`](../../../../docs/evidence/sjl-13-001/decision-rule-area-revised-2026-11-09b.md),
and on 2026-11-10, with step 1's records, by
[`decision-rule-area-revised-2026-11-10.md`](../../../../docs/evidence/sjl-13-001/decision-rule-area-revised-2026-11-10.md),
which changes part D's clause on rule I5 alone (section 6).
Its runs are taken after the merge, at the base and the after commit (part A by the hosted workflow, parts B to J
locally), and none is taken or quoted here.

**The branch** is `sjl-13-3-step1d`, built on the default branch's `df75f8c` (Broiler-Platform/Broiler.VM#195, the
merge that filed the rule's revision of 2026-11-09b). It lands in one merge: its after commit is that merge, and its
base the merge's first parent. Its first commit is the registry commit, directly on the base (decision D35). **This
holds only while the default branch is still `df75f8c` when the branch merges**: if anything lands on it first, the
merge's first parent is not `df75f8c`, the registry commit is not directly on the base, and every commit this record
names is stale, so the branch is not merged as it stands but built again on the new tip, with its records. Its
commits, in order:

| Commit | What it does |
|---|---|
| `cbee7c4` | **The registry commit**: registry revision 19, the mapping revised, the emission sites renumbered, the retained source corpus's new cases (section 1) |
| `26b436f` | **The admission**: `Broiler.ECMAScript.Frontend` `0.1.0-preview.10` for the compiler alone, and the stream's three admitted packages moved to the same version (section 2) |
| `00a3359` | The package baseline, bundle JS-10-016's candidate pack collected at `26b436f` (rule N37) |
| `5528d6a` | The stage table's copy in bundle `sjl-13-001` gains its provenance |
| `6236d87` | `packages.md`'s algorithms bullet quotes what its move to `0.1.0-preview.10` replaced |
| `f786023` | The refusal audit's admitted direction refuses the codes that left 2104 too |
| `c090c9a` | **The cut-over**: the wide path parses through the library; `JsParser` and the rest of decision D20's list deleted (sections 3, 5 and 6) |
| `247fb60` | Two `--checks` rows: the tokenizers' agreement (decision D21) and a left operand's spine (decision D23) (section 7) |
| `b3d6a53` | Review fixes to the two rows, a text and the deletion's remarks |
| `5aeee48` | Bundle `sjl-13-001`'s README says when its corpus bullet was written again |
| `795ebd3` | The tokenizers-agreement row compares the sources the library accepts and lists those it refuses, as the rule's revision of 2026-11-09b asks |
| `b036f0f` | **One true text per code** (decision D11, the owner's decision of 2026-11-09) (section 4) |
| `0467a1b` | Review fixes to the texts, to a span's code point and to the agreement row's detail |

and then the records of step 1, which change no product source, in several commits (section 10): `bcdc9a3` (rule
I5's register), `09e7f9d` (bundle `sjl-11-001`'s rule revision), `7b76d1f` (this record, JSC-326 and the dated notes
on JSD-0083 and JSD-0081), `fc7959b` (bundle `sjl-13-001`'s README, its texts and EX-133) and `6ba54ed` (the ledger and
the programme's documents), and the review fixes to those records that follow them. **The branch was built three
times.** Its first
build (`sjl-13-3-step1`) and its second (`sjl-13-3-step1b`, `sjl-13-3-step1c`) were never pushed, and each stopped on a
question the owner then decided and the area rule recorded by a dated revision before any run; this branch is the
third, the first two stages' commits redone onto `df75f8c` and the round of 2026-11-09 added. The earlier branches are
kept outside this repository, and none of their commits is in this one.

## Decision

### 1. Registry revision 19 (JSD-0083 decision D12, as its notes of 2026-11-04 and 2026-11-08 read it)

The registry commit, `cbee7c4`, mints one new code for each meaning a split row of the front end's mapping left, and
no `split` relation remains. Every new code is a seam code; no core code moves and no code changes its meaning.
- **Reached by a retained source:** 2105 `NewTargetOutsideFunction` (it left 2101); 2106 `BigIntWiderThanTheFormat`
  (2104); 2212 `MisplacedSuper`, 2213 `CoverInitialiserOutsidePattern`, 2214 `DuplicateProtoProperty`, 2215
  `InvalidRegularExpression` and 2218 `ReturnOutsideFunction` (each 2101); 2216 `ArgumentsInClassInitialiser` and 2217
  `UndeclaredPrivateName` (2206); 2219 `ReturnInStaticBlock` (2207); 2407 `DuplicateImportAttributeKey` (2402); and, by
  the two refused requests the second tools commit made possible, 2307 `CompileRequestNotServed` (2104) and 2408
  `DuplicateModuleKey` (2201).
- **Defensive**, each for one catalogue row, as rule N7 and the mapping checker's check E13 admit since the second
  tools commit: 2304 `ConstructTheLoweringHasNoArmFor` (EX0024, 2104), 2305 `TemplateTooManyStrings` (EE0021, 2302)
  and 2306 `EvalScopeTooDeep` (EX0013, 2103).
- **No code for the three language rules coded 2104**, by the owner's decision "Mixed fix" of 2026-11-04: the parser
  refused each construct first, under the code its row maps to. 2211 stays retired.

The commit changed exactly the paths the area rule's revision of 2026-11-08 lists for it, and in each only what that
list allows: `registry.txt` and `SliceSourceDiagnosticCode`; `frontend-mapping.tsv`, its header naming revision 19, the
stage table's copy and its digest, and `sites-read-at` restated as the base; the emission sites of `JsParser`, the
binder and the lowering that serve one rule, message texts unchanged; the conformance root's `LanguageErrors` and
`Test262Run`, which grades each new code as it graded the code it left (2106, 2304 and 2307 unsupported, as 2104 was);
the retained source corpus's new cases and its two refused requests; the `--checks` rows and the CLI acceptance rows
that pin a renumbered code, each with a dated note; the comments and help text that named one; the assurance artefacts
and the profile family's API baseline (rule N10), which gains the new members and nothing else. The parse corpus's
report at the registry commit differs from the base's only in renumbered codes at the same places (part B's class R,
read at the registry commit by bundle `sjl-13-001`'s part B).

### 2. The admission (JSD-0083 decisions D29 and D30)

- **The package.** `Broiler.ECMAScript.Frontend` `0.1.0-preview.10`, which the owner published to nuget.org on
  2026-11-04 from Broiler.JS `29061d65` (tag `ecmascript-v0.1.0-preview.10`), the library publishing its token stream
  since Broiler-Platform/Broiler.JS#1104, is admitted for `Broiler.VM.Profile.JavaScript.Compiler` alone (rules I1 and
  A2), by the procedure this repository has used since `9d0f80e`: no package, pin or lock file in the tree, each
  archive downloaded twice, byte-identical, and hashed to the SHA-512 nuget.org's catalog publishes, the restored
  content hash recorded beside it in the allow-list.
- **The stream moves with it.** The three packages of the stream this family already pins, `Broiler.ECMAScript.Algorithms`,
  `Broiler.ECMAScript.Generation` (test-only) and `Broiler.ECMAScript.Intl`, move from `0.1.0-preview.9` to
  `0.1.0-preview.10` in the same commit: no file under any of the three projects changes between Broiler.JS
  `6f7284da` and `29061d65`, so their code is `0.1.0-preview.9`'s, as decision D30's publication check asks.
- **The records the admission moves:** `Directory.Packages.props`, the allow-list and rule I1's two witnesses that copy
  an entry; `graph.manifest.json`'s new edge (rule A7) with ADR 0001's dated revision; `eng/packages/README.md`'s row;
  the notices, which fold the library into the stream's row (rule N38); rule I3's register row, whose frontend clause
  has its subject from here, with a dated correction; and the package baseline, bundle
  [JS-10-016](../evidence/js-10-016/README.md)'s candidate pack, collected at `26b436f` by `00a3359` (rule N37), which
  `packages.md` and `support.md` section 9 name. At `26b436f` alone rule N37 fails until `00a3359` collects the
  baseline, the exception the owner accepted on 2026-11-08 ("Accept, as precedent").
- **The library's nuspec** depends on `Broiler.ECMAScript.Algorithms` `0.1.0-preview.10` and
  `Broiler.Unicode.Algorithms` `0.1.0-preview.6`, both admitted, and carries no third-party code and no data.

### 3. The wide path on the library (JSD-0083 decisions D7, D8, D10, D11, D15, D16 and D18)

| Part | Where | What it is |
|---|---|---|
| **The request** | `JsFrontend.cs` | `SliceParseOptions` stays this family's public options (rules N10 and I6), and the request is built from them internally (decision D16): the goal and the nesting bound; the eval flags as forced strictness and the inherited context; top-level `await` from the module options, for the Module goal only (decision D10); the first refusal alone (decision D11); no visible private names and no caller's lexical names. Tokens are retained for a module, whose anchors the tree does not carry. The parse runs on the stack `CompilationStack` declares, as every entry point does (rule N19) |
| **The providers** | `JsCompilerUnicode.cs`, `JsRegularExpressionValidator.cs` | The Unicode provider is `JsCompilerUnicode`'s Format tables, `IUnicodeTables`, required (decision D8). The regular-expression provider is `JsRegularExpressionValidator`, which makes the checks the binder's `CheckRegExp` made, in its order and with its texts: a flag the language does not define or given twice, `u` with `v`, then the pattern as this family's matcher compiles it; it answers invalid with EE0019's `regexp.literal.invalid` and the validator's message as its argument, and the library reports the refusal at the literal's span (decision D7) |
| **The adapter** | `JsFrontendAdapter.cs` and its parts `.Expressions`, `.Functions` and `.Modules` | Builds `JsSyntax` from the shared tree, reproducing every anchor and offset `JsParser` gave and every fact it computed into `JsSyntax`, each by `JsParser`'s rule: strictness from the library's side fact, function kinds, names and keys, parenthesisation, the chain wrappers, the desugarings and synthetic names, numeric-key spellings by `NumberText`, and the top-level-`await` fact by `JsParser`'s rule (decision D15, hand-off H19). The one exception is row P37's tree, which it builds as the specification reads it (the owner's decision of 2026-11-09). `JsParser`'s `DefaultBindingName` and `StarName` are `JsModuleNames`' in `JsSyntax.cs`, and `SawTopLevelAwait` the adapter's |
| **The diagnostics** | `JsFrontendDiagnostics.cs`, `JsFrontendDiagnostics.Texts.cs` | Each library diagnostic becomes the registry code the mapping's `code` row gives its shared code, placed at the span's start (decision D15), with one text per shared code (section 4) |
| **The admission walk** | `JsAdmissionWalk.cs` | After a parse that reported nothing, and before the numeric admission and the binder, on every entry point that parsed with `JsParser` (`JsCompiler.Compile`, eval code and `JsCompiler.Requests`), it makes the policy and limit refusals `JsParser` made during the parse: BigInt under a manifest that does not admit it (2104, EX0017), the BigInt width ceiling (2106, EX0028), and a module specifier that is not well-formed Unicode (EX0029, 2101). Its refusal stops the unit as the parse's did (decision D18) |

After the parse only the admission walk and the binder's deletions change (decision D19). The binder no longer makes
the checks of rows the stage table assigns `syntax`, which the library refuses before it runs, and keeps B16 (EE0018),
B21 (EE0054) and B33, which also refuses EE0171, whose rows are `semantics` (decision D20). Each `BinderAdmitted`
assertion of the lowering that mirrors a deleted check stays, as the lowering's guard. The slice path does not change:
`SliceParser` and `SliceTokenizer` keep it until SJL-15.4.

### 4. One true text per code (JSD-0083 decision D11, as its note of 2026-11-09, later that day, reads it)

`JsFrontendDiagnostics.Texts.cs` holds one frozen table: a text for every shared code the front end's mapping holds.
Each states its own code's condition, and where a code covers several of the deleted wide front end's sites its text is
a general wording true of each, as the owner decided ("One true text per code"); the texts `JsParser` gave each site
retire. A text of a manifest refusal ends with the suffix " is not admitted by the declared feature manifest". The
library does not change.
- **`b036f0f`** changed the texts that, taken from one site, were false of another, and wrote a text for every code that
  had rendered its message identifier, from the catalogue's description of its row. **`0467a1b`** changed again the
  texts of the codes the library also refuses strict `eval` and `arguments` under as assignment targets, which "is not a
  reference" did not state: JSE1324, JSE1325, JSE1329, JSE1330, JSE1331 and JSE1335, the codes of rows EE0028, EE0029,
  EE0033, EE0034, EE0035 and EE0039, now say the target "is not a valid assignment target (in strict code, neither
  `eval` nor `arguments` is one)". JSE1415, the code of row EE0054, which the binder's B21 refuses and the library does
  not report today (its row is `semantics`), was given the same wording, so that the codes of an assignment target
  agree. `0467a1b` also reworded JSE2202 and JSE9108, and made the code point a span names, for JSE1201 and JSE9101,
  the one its last backslash begins.
- **The codes that rendered their message identifier until `b036f0f`**, and gained a text there, are JSE1317, JSE1319,
  JSE1320, JSE1333, JSE1901, JSE1906, JSE1907, JSE1908, JSE2001 to JSE2003, JSE2201 to JSE2226, JSE7401, JSE8101 to
  JSE8105, JSE8107 and JSE9207. `b036f0f`'s message says of them that "none of the 45 is reported by the library in this
  family today". That is wrong, and history keeps it, since no commit is amended. The facts, as the stage's verifier
  read them: every one of them but JSE1317, JSE1906, JSE1907, JSE1908 and JSE9207 is a code of the library's rows;
  among those, JSE1319, JSE1320 and JSE1333 are reached by the library's parse, though no input of the parse corpus is
  refused first under one; and the regular-expression rows' codes (JSE2201 to JSE2226, JSE8101 to JSE8105 and JSE8107)
  are reached only through a validator's verdict that names their condition, while this family's validator names only
  `regexp.literal.invalid`, so it renders none of them today. `0467a1b`'s message and the table's own remark state the
  same, quoting the sentence.
- **JSE1315 keeps the validator's own message**, as its argument: decision D7 has the provider's answer carry it, so
  that this family renders the text its validator gave at the base. That one code is not one text per code, and
  JSD-0083's note of 2026-11-10 on decision D11 records the exception.
- **What pins a text moves with it**, each with a dated note quoting what it read: the CLI acceptance rows and the
  `--checks` rows that pin a text, the BigInt and template rows that pin code and place and print a text, and the
  remarks of the table, of `JsFrontendDiagnostics` and of `BinderChecks`. Every changed or new text's comment quotes
  what the code rendered before, with its provenance, or says it rendered its message identifier. No part of bundle
  `sjl-13-001`'s rule reads a text, as its revision of 2026-11-09b shows part by part, so no text moves a code, a
  place, a stage or an outcome. Bundle `sjl-13-001` retains every old and new text in `texts/changes.tsv`.

### 5. The corrections step 1 makes, by row (JSD-0083, "The corrections step 1 makes, and how each is shown")

Bundle `sjl-13-001`'s part A, kind (F), and part B, class F, name exactly these rows of the correspondence, each a
correction of this family that step 1 makes because the library parses as the specification reads:

| Row | What is corrected | How it is shown |
|---|---|---|
| T6, T7, T8 | `#!` and `-->` after a substitution's `${`; the template-end scan and HTML-like comments in a substitution; the template-end scan's regular-expression-or-division guess | the `syntax` vectors (part H) and the correspondence's probes |
| T13 | ASCII white space inside `\x` and `\u` escapes, on the wide path (the slice path's is hand-off H23) | the same |
| T16 | strict refusal of legacy octal and non-octal decimal literals in object keys and class element names | the same |
| T24d, T24e | a regular-expression literal after a `for await (…)` head, and after a prefix `++` or `--` | the same |
| T24f | a regular-expression literal after `await` used as an identifier | the same, and test262's `test/staging/sm/async-functions/await-in-arrow-parameters.js` (part A's kind (F)) |
| D32 | `new.target` in a class field initialiser at function depth 0 | the vectors and the probes |
| P31 | an assignment rest property whose target is an object or array literal | the same |
| P15b | a parenthesised nested target that is not a simple target, in a cover grammar | the parse corpus (part B's class F), and test262's `test/staging/sm/expressions/destructuring-pattern-parenthesized.js` |
| P05a, D40 | an import-attribute key that is an identifier name not starting with a letter; a reserved word as an import specifier written without `as` | the correspondence's probes only (only the Module goal reaches them) |
| P37 | a `new` whose operand begins with a `new` with an argument list and goes on to another argument list, which `JsParser` read with that list as a call inside the operand | the parse corpus (part B's class F, its artifact bytes): `T052945` |
| D64 | the module-request path's missing early errors: `JsCompiler.Requests` never ran the binder | the parse corpus (part B's class F, outcome moves in B5) |
| T33 | a regular-expression literal whose body ends with a backslash at the end of the text, on which this family's tokenizer threw `ArgumentOutOfRangeException` | test262's `test/staging/sm/RegExp/7.8.5-01.js` (part A's kind (F)) |
| P38 | an arrow function's concise body in a `for` statement's head, which `JsParser` read with `in` admitted | test262's `test/staging/sm/statements/arrow-function-in-for-statement-head.js` |
| P39 | a comma expression as a computed property name | test262's `test/staging/sm/class/compPropNames.js` |

Part A's kind (F) is expected to list the variants of the five test262 files above, each with its row, and no other
(the rule's revision of 2026-11-09b, item 2). The corrections of this family step 1 does not make are handed on and
admit nothing: P26 and D61 (H18), P32's item 10 (H19), D30 (H20), R19 (H21, SJL-11.4), and the slice path's two
defects, its half of row T33 and its acceptance of strict `\8` and `\08`, which bundle `sjl-13-001`'s part F lists as
`J000262` and `J001009` (H25, SJL-15.4).

*(Noted 2026-11-12, at SJL-13's work package 9.4, when this family's vectors pin moved to Broiler.JS's corpus revision
39.)* The table shows rows T6, T7, T8, T13, T16, T24d to T24f, D32 and P31 by "the `syntax` vectors (part H)", and row
T33 by test262's file alone. Corpus revision 39, merged in Broiler-Platform/Broiler.JS#1116 as `e4b6b49e`, retires the
`divergence.broiler-vm` fields those vectors carried, and row P15b's, which the table shows by the parse corpus and
test262 and which the `syntax` vectors carry too. By row T33's correction, the library's delimiting of a literal that
ends in a backslash at the end of the source, it also retires those of the regular-expression area's row S9b
([JSD-0081](0081-regular-expressions-move-to-broiler-regex-under-its-owner-conditions.md)), whose vectors that
correction reaches. This family's vectors pin moved to it in `b781ebb`, where every one of those vectors is a plain
match. The table is left as written.

### 6. Deleted, and rule I5 (JSD-0083 decision D20 and X6)

The cut-over, `c090c9a`, deletes decision D20's list. The rows were taken from the assurance manifests before the cut-over
(`f786023`) and at the head (`0467a1b`): every unit the branch no longer declares, registered in rule I5's
`retired-members.json` by type where its type went whole and by member where its type stays, each with what replaced
it and the milestone `SJL-13.3`.
- **Whole:** `JsParser.cs`, the wide parser, registered as the type `JsParser`; its parse is the library's
  `SyntaxParser`, behind `JsFrontend` and the adapter. The two members rule I5 registered at SJL-3.4,
  `JsParser.NumberText` and `JsParser.TryBigIntValue`, keep their rows; their replacements name the adapter and the
  admission walk since `c090c9a`.
- **`SliceTokenizer`'s wide-only members:** the structured-template mode (`StructuredTemplates`, `Templates`,
  `templates`, `streamTemplates`, `scanned`, `textStart`, the constructor that scanned a substitution, and
  `TokenizeSubstitutions`), its nesting limit (`TemplateNestingLimit`), the recording parts of `TemplateScan` (`Start`,
  `End`, `EndLine`, `EndLineStart`, `ChunkStart`, `Chunks`, `Substitutions`, `SubstitutionStart`, `SubstitutionLine` and
  `SubstitutionLineStart`), and the types `SliceTemplate` and `SliceSubstitution`. A constructor is not a name rule I5
  reads, so the substitution constructor is listed here and not registered. `SliceTokenizer` stays for the slice path
  and the census until SJL-15.4.
- **The binder:** `CheckRegExp`, which left for the provider (decision D7); `ResourceScoped`, the context the
  `using` check read; and the checks of `syntax` rows, the correspondence's B03 to B07, B10 to B15, B17 to B20, B26 to
  B31 and B34 (B25 is `CheckRegExp`), which were lines of members that stay, and the frames `Async`, `Body`,
  `Disposing`, `Function` and `StaticBlock` of `JsBinder.Frame` that only those checks read. An enumeration's member is
  not a name rule I5's reader reads, so the frames are listed here and not registered; `Bind`, `BindTree`, the binder's
  constructor and `Declares` stay, with fewer parameters. A check that was a line of a member that stays has no name
  rule I5 can register, so those checks are named here and in the register's paragraph by their rows. Decision D20
  said they are "named in X6 member by member and registered in I5", and part D's MET line asked the same; JSD-0083's
  note of 2026-11-10 on decision D20 reads it for the checks that were not members, and the area rule's revision of
  2026-11-10 writes that reading into part D before any run. Part D's control of "a re-added deleted binder member
  that checked a syntax row" is `CheckRegExp`, row B25, whose catalogue row EE0019 the stage table assigns `syntax`.

**What stays, and holds no parser:** the request, the providers, the adapter, the diagnostics and the admission walk of
section 3, which build, hand over, adapt, map and render, and make this family's policy refusals. Rule I5's register
gains a paragraph for SJL-13.3's step 1, its register row names it and this record with a dated revision, and its test
reads the files the registered members lived in, or where a file went whole the files that adapt its replacement:
`JsFrontend.cs`, `JsBinder.Walks.cs`, `JsBinder.EarlyErrors.cs` and `JsRegularExpressionValidator.cs` join the list,
beside `JsFrontendAdapter.Expressions.cs`, `JsAdmissionWalk.cs` and `SliceTokenizer.cs`, which `c090c9a` named.

### 7. The two `--checks` rows (JSD-0083 decisions D21 and D23)

- **"the shared and slice tokenizers agree on the slice path's inputs"** (part F): over the parse corpus's
  sub-population B4 and the slice compiler's accepted programs, the two streams are compared token by token through
  the field map decision D21 fixes. An input the slice path accepts and the library refuses is listed by name with the
  library's shared code, the catalogue row and the registry code, and not compared; hand-off H25 is named only for
  inputs refused under JSE1204 (EE0004), and the detail says whether those are exactly `J000262` and `J001009`. The row
  fails on a disagreement, on comparing nothing, and on a refused input whose shared code the mapping does not hold.
- **"a left operand's spine is charged to the chain it begins"** (part C's fifth row): the four shapes row R14 names,
  split into six, and the residual shapes of JSD-0083's note on decision D14, each past the tree-depth bound, are
  refused with the contract's tree-depth diagnostic, the process alive; a chain within the bound on each side compiles.

### 8. Evidence at this change

Local pre-checks, on binaries built at each commit of the branch that changes product source and against answers taken
at the base `df75f8c` on binaries built there, with the scripts and logs kept outside the tree. An independent verifier
took the base again from a clean archive of `df75f8c`, with the parse corpus and the census again from a clean archive
of the registry commit, and every pre-check again at `0467a1b` from a clean build. **They are not evidence of the
rule**, whose runs are taken after the merge, and no figure of them is quoted here.
- At each commit the stage checked (the base, the registry commit, the admission, each commit of the second round,
  `795ebd3`, `b036f0f` and `0467a1b`, the last the head of the code), the solution builds in Release with no warning and
  the writers change nothing; the architecture and contract tests pass at each but the admission, where rule N37 alone
  fails until the baseline's commit (section 2). The records' commits after `0467a1b` are checked the same way, and
  change no product source.
- The mapping checker passes at revision 19, with `--broiler-js` and its controls.
- The CLI acceptance table and the refusal audit answer as declared, the conformance root's js-3a and ingest-shape
  suites pass, and every `--checks` row passes at the head, the two new rows among them.
- The parse corpus's sub-populations B1 to B5, compared with the registry commit's report, hold no difference that
  part B's classes do not take, under a classifier kept in scratch; B4 has none; the construct census is
  byte-identical to the registry commit's over the parse corpus and over test262's `test/`.
- Whole-suite test262 in the bytecode form, locally, moved exactly the five files of section 5 from failing to passing,
  and no other verdict; the rows whose exhausted dimension moved were load effects, which each alone re-run, at the base
  and at the head, did not reproduce.
- The wide and slice fuzz sessions over the rule's seeds found nothing.

### 9. Rules and public API

- **I1, A2, A7, N37, N38, I3**: section 2. **I6**: no type of the library appears in the family's public API baseline;
  `docs/api/public-api.txt` gained revision 19's members at the registry commit and nothing since.
- **N5 to N9**: revision 19 and the mapping, checked by the mapping checker and by `DiagnosticRegistryRuleTests`.
- **N12**: the files the cut-over added hold frozen tables or are made per call; the rule's count of the lowering
  assembly's files moved with a dated comment in `c090c9a`, by the files added and the one deleted, and needs nothing
  here. **H3**: the review record's count moved in `c090c9a` the same way.
- **N19**: the library's parse runs on the declared stack; the tree-depth bound is the library's, equal to
  `SliceParseOptions.MaximumTreeDepth` (decision D14).
- **I5**: section 6, and part D's clause on it as the area rule's revision of 2026-11-10 reads it. **K4 and I7**:
  section 10's last item.

### 10. Records

- **Assurance.** Regenerated by the writers at each commit that changes a covered file; every human line stays
  PENDING. The parser's units that left this family's tree are the home's to cover: bundle `sjl-13-001` defines
  exclusion EX-133 (decision D31), as EX-125 and EX-127 to EX-132 did for the earlier areas.
- **Correction entry.** [JSC-326](../roadmap.corrections.md#jsc-326).
- **Dated notes**, each quoting what it reads: JSD-0083's status, and its decisions D11 (twice), D20 and D21; JSD-0081's
  decision D18 and its sub-area S's X2; the SJL-11 milestone's work package 11.4; the SJL-13 milestone's SJL-13.3 and
  SJL-13.4; the programme roadmap's section 11; `inventory.md`'s parser row and `areas/frontend.md`'s wide path;
  `governance.md`'s row P15, whose range of seam codes revision 19 passes; `architecture.md`'s section 7, which named
  `JsParser`'s copies; the profile's backends and parity records (JSC-326); bundle `sjl-13-001`'s and bundle
  `sjl-11-001`'s READMEs; and rule I5's register.
- **SJL-11's rule.** SJL-11.4 has not landed (Broiler.Regex's default branch is at its task 2.13, and SJL-11.4's
  landing branch is not cut), so decision D36's conditional applies, and
  [`decision-rule-area-revised-2026-11-10.md`](../../../../docs/evidence/sjl-11-001/decision-rule-area-revised-2026-11-10.md)
  revises bundle `sjl-11-001`'s rule before any of its runs, naming the provider's validator where its part B named
  the binder's `CheckRegExp`. A dated correction in it, in a later commit of the same change and before any run, gives
  the code `CheckRegExp` reported at the base as 2101, `UnexpectedToken`, where its context paragraph said 2102.
- **SJL-13's rule.** Bundle `sjl-13-001`'s rule is revised a sixth time, before any of its runs, by
  [`decision-rule-area-revised-2026-11-10.md`](../../../../docs/evidence/sjl-13-001/decision-rule-area-revised-2026-11-10.md),
  for part D's clause on rule I5 alone (section 6).
- **Not done on this branch: the composition register and the closures.** JSD-0083's decision D29 and SJL-13.4 ask for
  "K4's rows for SliceCompiler, Conformance, Cli and PolyglotCli" and the notices; the notices moved at `26b436f`, but
  `docs/compositions.md`'s shared-assemblies cells do not name the library, and no closure of a root carrying it is
  retained. The admission commit left them for the cut-over, because a trimmed publish keeps the library's assembly only
  once the compiler calls it, and the cut-over did not take them. Rule K4 holds each register row to the closures the
  bundle it names retains, and a closure must contain exactly the shared assemblies its row declares, so the cells
  cannot move before closures that carry the library are retained. Neither part D nor the merge settles it: at the
  after commit the four roots' rows still name bundle `sjl-9-001`'s closures, which do not carry the library, so rule
  K4 passes there and the rows stay as they are. **They move after the merge**, in SJL-13.3's work package 7: task
  7.2's collector publishes every root in three modes at the after commit, and task 7.3's records commit retains those
  closures in bundle `sjl-13-001` and moves `docs/compositions.md`'s shared-assemblies cells for SliceCompiler,
  Conformance, Cli and PolyglotCli, with the bundle each row names, in the same commit, so that rule K4 holds at every
  commit. Until then the register does not declare the library for those roots. SJL-9.4 moved its rows on its branch
  before its merge; step 1 does not. Section "Where the plan, JSD-0083 and the code disagree" records it (item 4).
  *(Noted 2026-11-10, after the merge, by the review of task 7.2 of step 1's evidence.)* This bullet reads "task 7.3's
  records commit retains those closures in bundle `sjl-13-001` and moves `docs/compositions.md`'s shared-assemblies
  cells" and "Until then the register does not declare the library for those roots." The closures and the four rows
  moved earlier, together, at task 7.2: commit `2c087c4` of the evidence branch `sjl-13-evidence` retains the closures
  published at the after commit in bundle `sjl-13-001`'s `after/publication/` and moves the four cells in the same
  commit, its own, as that stage's brief ordered, so rule K4 held at every commit. From that commit the register
  declares the library for SliceCompiler, Conformance, Cli and PolyglotCli; task 7.3's records commit reads the closures
  and moves no cell. Neither the closures nor the move is a verdict.

## Readings

- **The order of refusals.** `JsParser` made the BigInt refusals and EX0029's during the parse, in source order; the
  admission walk makes them after a parse that reported nothing, so such a refusal is absent where the parse refuses
  anywhere in the unit (part B's class S3), as decision D18 named before any run.
- **One diagnostic.** The library stops at its first refusal, so a refused unit's list is one diagnostic where
  `JsParser`'s held its cascade (part B's classes S7 and S8), and the one diagnostic may stand at the library's place or
  carry the code the mapping gives it (class N), as the owner decided on 2026-11-09.
- **The provider's refusal and the parse.** `CheckRegExp` ran in the binder after the parse; the library asks the
  provider when its parse reaches the literal, so a literal's refusal now precedes the binder's refusals of the same
  unit and follows the parse's earlier ones.

## Where the plan, JSD-0083 and the code disagree

JSD-0083 wins over the plan, and the code follows JSD-0083, but where this list says otherwise.
1. **The plan's commit 6.1** names "the code argument at each `JsParser` site that serves one relation-split row alone";
   JSD-0083's decision D12 adds the binder's and the parser's split other codes' single-rule sites, and the area rule's
   revision of 2026-11-08 the paths the registry commit may change. The registry commit follows the revision.
2. **The plan's decision D11** reads "Broiler.VM renders every message text it renders today for a code whose meaning
   stays"; JSD-0083's note of 2026-11-09, later that day, replaces it with one true text per code, and the code follows
   the note, but for JSE1315, which keeps the validator's message under decision D7 (section 4).
3. **The plan's commit 6.2** lists "every `packages.lock.json`"; this repository holds no lock file since `9d0f80e`, and
   rule I1 reads each project's restore metadata, so none moved.
4. **The plan's commit 6.2 and decision D29** name `docs/compositions.md`'s rows for the four roots; no commit of the
   branch moves them, and no closure with the library is retained. They move after the merge, with the closures
   published at the after commit, in task 7.3's records commit (section 10), so the after commit does not meet
   decision D29's K4 rows. Part D runs rule K4 there, which holds each row to the closures it names, so it does not
   show the gap; this item and section 10 record it.
   *(Noted 2026-11-10, after the merge, by the review of task 7.2 of step 1's evidence.)* This item reads "They move
   after the merge, with the closures published at the after commit, in task 7.3's records commit (section 10)". They
   moved at task 7.2, in commit `2c087c4` of the evidence branch, with the closures in the same commit, as section 10's
   note of this date records; the after commit itself still does not meet decision D29's K4 rows.
5. **Decision D20 lists `SliceTokenizer`'s wide-only members** as "the structured-template mode, its nesting limit,
   `SliceTemplate`, `SliceSubstitution` and their tables"; the code deleted those and `TemplateScan`'s recording fields
   and the frames of `JsBinder.Frame` that only the deleted checks read, which section 6 names.
6. **`b036f0f`'s message** misstates the codes that gained a text (section 4); history keeps it.
7. **JSD-0083's note of 2026-11-09, later that day, on decision D21** asked the owner to confirm its reading of "Falsified
   if"'s fourth clause; JSD-0083's note of 2026-11-10 records it as accepted under the owner's standing direction of
   2026-10-18, not as the owner's own dated answer.
8. **Decision D20 and part D's MET line** asked that the binder's checks of `syntax` rows be "registered in I5" and
   registered "member by member". Of those checks only `CheckRegExp` (B25) was a member of its own; the checks of rows
   B03 to B07, B10 to B15, B17 to B20, B26 to B31 and B34 were lines of members that stay, which rule I5, reading
   declared names, cannot register (section 6). JSD-0083's note of 2026-11-10 on decision D20 reads the decision for
   them as named by row, accepted under the owner's standing direction of 2026-10-18 and not as the owner's own dated
   answer, and the area rule's revision of 2026-11-10 writes that reading into part D's MET line before any run.

## Correction entries

[JSC-326](../roadmap.corrections.md#jsc-326): the profile's plan sentence that named `JsParser.Parse` in the wide
compile's order (`roadmap.backends.md`), the parity record's sentence that named `JsParser.cs`'s primary-expression
switch (`roadmap.parity.md`), and rule I5's register.

## Step 2: the dated note decision D3 asks for

*(Noted 2026-11-14, at SJL-13.3's step 2's records, on its branch, before any run of bundle `sjl-13-002`'s rule.)*
JSD-0083's decision D3 asks for this record "with a dated note at step 2", and the context above says "Step 2, which
moves the lowering, the numeric admission and the binder onto the shared tree and deletes the adapter and `JsSyntax`,
files a dated note here." This is that note, step 2's cut-over record. Nothing above it is edited.

**The branch** is `sjl-13-3-step2`, cut from the default branch's `389b55c` (Broiler-Platform/Broiler.VM#205), the
merge that moved the shared stream to `0.1.0-preview.11` and carried bundle `sjl-13-002`'s readings taken before any
run and its rule's revision of 2026-11-13. It lands in one merge, bundle `sjl-13-002`'s after commit, whose first
parent is the base. **This holds only while the default branch is still `389b55c` when the branch merges**: if
anything lands on it first, the merge's first parent is not `389b55c`, every commit this note names is stale, and the
branch is built again on the new tip, with its records, before it merges; that bundle's reading 1 holds the default
branch until its evidence merges. Its commits, in order:

| Commit | What it does |
|---|---|
| `db7a6f0` | **The admission walk walks the shared tree itself**: the BigInt and module-specifier refusals, the BigInt values and the top-level-`await` fact, by `JsParser`'s rule and in the adapter's visit order; the adapter stops making them |
| `fb909e4` | **`JsTreeFacts`**, the readings of the shared tree in one place, which the adapter calls |
| `f0b117d` | The numeric admission reads the shared tree, and the parse carries both trees |
| `f34494a` | The binder, its early errors and the lowering's walk read the shared tree, through a temporary map from each adapted node to its shared origin |
| `dcec5ec` | **The lowering reads the shared tree**, `JsFrontend` stops adapting, and the map is dropped |
| `812cbaf` | **`JsSyntax.cs` and `JsFrontendAdapter.cs` with its parts deleted**; rule N12's and the review record's counts and rule I5's list of files follow, each with a dated comment |
| `a71f01d` | Rule I5's register: the deleted types by type, the members types that stay lost by member, and the paragraph for SJL-13.3's step 2 |
| `c452294` | Review fixes to the register's wording |

and then the records of step 2, which change no product source, in several commits: bundle `sjl-13-002`'s README
with EX-135, and its note on bundle `sjl-13-001`'s EX-133; JSC-328; this note; JSD-0083's notes; the SJL-13
milestone's note; and the ledger. Every commit of the code builds in Release with no warning, passes the architecture
and contract tests with the assurance gate in its checking mode, and carries its own regeneration by the writers; every
new or changed unit carries its annotation lines, every human line PENDING.

**What step 2 does** (JSD-0083's decisions D15, D19 and D20, as its notes of 2026-11-13 and 2026-11-14 read them):
- **The facts.** The admission walk, `JsAdmissionWalk`, computes the top-level-`await` fact by `JsParser`'s rule: eval
  code inside a function counts as inside one, a non-arrow function and a static block raise the depth, an arrow and
  a field's initialiser do not, and an `await` inside an async arrow at a module's top level still counts (hand-off H19
  is unchanged); it is not the library's `ProgramNode.HasTopLevelAwait`. It computes the BigInt values beside the
  refusals, a BigInt key held to the width ceiling alone. A side table keyed by the shared tree's nodes holds each
  function's and the program's strictness, from the library's side fact (decision D10); field initialisers and static
  blocks are strict. `JsFrontend.Parsed` hands the library's tree on with those tables and the module's retained
  tokens.
- **The readings.** `JsTreeFacts` holds the readings of the shared tree the readers share, each by the adapter's rule:
  where a node begins and ends, the program's place, the end of the source and the token before a node; parentheses,
  unwrapped by a loop; the directive prologue, by the adapter's predicate; a key's spelling, by `NumberText` for a
  number; export names, the anonymous default export named `default`; a module specifier, through `SourceOrNone`, so
  that no specifier and the empty one read alike, as the adapter's tree held them; the import and export tuples; the
  names a pattern binds; the operator and declaration-kind mappings; and predicates for the shapes the adapter
  normalised, a simple-name target, a method entry, an if-clause function and the default export.
- **The binder and the walk.** The binder's tables are keyed by the shared tree's nodes, its facts and their order
  unchanged: a for-in statement's head, then the Annex B initialiser, then the right-hand side, then the body;
  a class's heritage, then each member; ties at one place by visit order. The lowering's `Walk` keeps the adapter's
  child map arm by arm: an arrow yields only its body, a try statement never its catch parameter, a for-of statement no
  initialiser; a shorthand's value counts as a mention, and `MakesClosure` never enters a function declaration. The
  if-clause function's one-statement block is keyed by its declaration and compiled at the function's place.
- **The lowering.** Every site reads the shared tree, at the place the adapter gave: the program's first retained token,
  a function at its container's span, a concise body's and a field's return through one `CompileReturn`, a static
  block's own source offsets, a shorthand at its identifier, a pattern leaf at its inner expression, a `for`
  initialiser at its span before unwrapping, the export spans, a namespace import at its `*` and `export * as` at its
  `as`, a dotted member's name span, an object literal's method at its entry, and a field's initialiser with no source
  span. The last directive's completion value is compiled only where the adapter's tree compiled it. One core
  `CompileFunction` takes a `FunctionNode`, a `FieldDefinition` or a `StaticBlock` as its key. The lowering's
  refusals of what it has no arm for stay as they were; only sites that replace the adapter's own refusal of an
  unexpected node throw.
- **Deleted**: `JsSyntax.cs`, and `JsFrontendAdapter.cs` with `JsFrontendAdapter.Expressions.cs`,
  `JsFrontendAdapter.Functions.cs` and `JsFrontendAdapter.Modules.cs`; `JsFrontend` keeps the request, the parse and
  the side tables. `JsModuleNames`' constants are `JsCompiler.DefaultBindingName` and `JsCompiler.StarName`, in the
  lowering's module code, `JsCompiler.Modules.cs`, as decision D20 says. `JsCompiler.FindConstructor` moved to
  `JsTreeFacts`, where it reads each member's key through `JsTreeFacts.Key`, and `JsCompiler.Unwrapped` with it; the
  plan asked only that the keys be read through `JsTreeFacts.Key`, so the move is a departure, recorded here and
  registered in rule I5.
- **Rules.** N19: no public static member is added, and the parse runs on the stack `CompilationStack` declares. A10
  and I6: no `InternalsVisibleTo`, and nothing new in any public API baseline. N12's count of the lowering assembly's
  files and the review record's count move with dated comments naming the files deleted and `JsTreeFacts.cs`. No path
  the development checks hold fixed changes: the slice compiler, the CLI and the conformance roots, `eng/`, the
  workflows, the corpora, `Directory.Packages.props` and every public API baseline.

**Decisions taken under the owner's standing direction of 2026-10-18** ("Please do all recommended decisions and skip
waiting for review"), each as the recommended choice and recorded as the owner's dated decision under it, with the
alternative not taken; none is the owner's own dated answer, asked and answered, and the owner may rule otherwise:
1. **The exclusion.** Bundle `sjl-13-002` defines EX-135, the adapted tree's and the adapter's units, which leave this
   component's assurance coverage, in its section 9, keeps that section's row of SJL-13.1, "(none)", with a dated
   note, and notes EX-133 in bundle `sjl-13-001`, whose list of what stays covered named the adapter. Not taken: "a
   note on EX-133 only".
2. **The library's version.** Step 2 moves no package: it is built on `Broiler.ECMAScript.Frontend`
   `0.1.0-preview.11`, which the default branch admits since `389b55c`, whose code is `0.1.0-preview.10`'s, its
   sources changing in comments only. The plan's recommendation, "stay on `0.1.0-preview.10`", is overtaken by the
   owner's publication of `0.1.0-preview.11` on 2026-11-13 and its move before step 2's base, which bundle
   `sjl-13-002`'s README records. Not taken: "a package move inside step 2", which would put a version change in the
   after commit.
3. **No view layer.** The lowering, the binder and the walks read the library's nodes as they are; the library's
   nodes have internal constructors, and nothing wraps them. Not taken: "a thin internal view over the shared tree".
4. **Operators.** The shared tree's operators map onto this family's `SliceTokenKind` in `JsTreeFacts`, and the
   lowering compiles them as before. Not taken: "the shared operator enumerations now", which is SJL-15's.
5. **Side tables.** Strictness and the BigInt values are tables keyed by a node's identity, handed on beside the tree.
   Not taken: "a fact carried by a wrapper of each node", which is the view layer not taken.
6. **One core `CompileFunction`.** Not taken: "a compile method per kind of function node".
7. **The cut-over record.** This dated note on JSD-0087, as decision D3 asks. Not taken: "a new record, JSD-0088".
8. **The enumerations** `JsPropertyKind`, `JsUsing`, `JsMethodKind` and `JsExportKind`: a shared enumeration where
   one maps value for value, else a new name, and none of those names declared again. `JsUsing` becomes the
   compiler's own `JsDisposal`, since the library's `DeclarationKind` does not map onto it value for value;
   `JsMethodKind` becomes the library's `MethodKind`, with its `FieldDefinition` and `StaticBlock` nodes;
   `JsPropertyKind` becomes the library's `PropertyKind`, with its `SpreadElement` node; and `JsExportKind` becomes the
   library's export declaration nodes, `ExportNamedDeclaration`, `ExportDefaultDeclaration` and `ExportAllDeclaration`.
   Not taken: "keep this family's enumerations under their names", which rule I5's type rows refuse.

**Rule I5** (decision D20, as JSD-0083's note of 2026-11-13 reads it, and bundle `sjl-13-002`'s rule as revised on
2026-11-13, part D):
- **Registered by type**, with the milestone `SJL-13.3`: every type `JsSyntax.cs` declared directly in a namespace,
  `JsModuleNames` among them, and `JsFrontendAdapter`. Each type's row refuses each of its members, since none can be
  declared again without its type: `JsMemberExpression.NameSpan`, `JsAssignmentExpression.ParenthesisedTarget`, the
  `SourceStart` and `SourceEnd` of `JsFunctionNode` and `JsClassNode`, `JsModuleNames.DefaultBindingName` and
  `JsModuleNames.StarName`, and every member of `JsFrontendAdapter`, `SawTopLevelAwait` among them.
- **Registered by member**, one row per distinct name: `JsCompiler.FindConstructor` and `JsCompiler.Unwrapped`.
- **Not registered, named by kind for each type deleted whole**, because rule I5's reader does not read their kind of
  name: the positional parameters of the records `JsNode`, `JsExpression`, `JsStatement`, `JsNumberLiteral`,
  `JsBigIntLiteral`, `JsStringLiteral`, `JsBooleanLiteral`, `JsNullLiteral`, `JsRegExpLiteral`, `JsIdentifier`,
  `JsThisExpression`, `JsArrayLiteral`, `JsSpreadElement`, `JsObjectEntry`, `JsObjectLiteral`, `JsParameter`,
  `JsFunctionNode`, `JsYieldExpression`, `JsAwaitExpression`, `JsFunctionExpression`, `JsUnaryExpression`,
  `JsUpdateExpression`, `JsBinaryExpression`, `JsLogicalExpression`, `JsAssignmentExpression`,
  `JsConditionalExpression`, `JsMemberExpression`, `JsCallExpression`, `JsChainExpression`, `JsTemplateLiteral`,
  `JsTaggedTemplate`, `JsNewTargetExpression`, `JsNewExpression`, `JsSequenceExpression`, `JsPattern`,
  `JsTargetPattern`, `JsPatternElement`, `JsArrayPattern`, `JsPatternProperty`, `JsObjectPattern`,
  `JsDestructuringAssignment`, `JsDeclarator`, `JsVariableStatement`, `JsExpressionStatement`, `JsBlockStatement`,
  `JsIfStatement`, `JsWithStatement`, `JsWhileStatement`, `JsDoWhileStatement`, `JsForStatement`, `JsForInStatement`,
  `JsForOfStatement`, `JsBreakStatement`, `JsContinueStatement`, `JsReturnStatement`, `JsThrowStatement`,
  `JsTryStatement`, `JsSwitchClause`, `JsSwitchStatement`, `JsLabelledStatement`, `JsEmptyStatement`,
  `JsDebuggerStatement`, `JsFunctionDeclaration`, `JsClassMember`, `JsClassNode`, `JsClassDeclaration`,
  `JsClassExpression`, `JsSuperMemberExpression`, `JsPrivateMemberExpression`, `JsPrivateInExpression`,
  `JsSuperCallExpression`, `JsImportCall`, `JsImportMeta`, `JsImportAttribute`, `JsImportSpecifier`,
  `JsImportDeclaration`, `JsExportSpecifier`, `JsExportDeclaration` and `JsProgramNode`; the members of the
  enumerations `JsPropertyKind`, `JsUsing`, `JsMethodKind` and `JsExportKind`; and `JsFrontendAdapter`'s constructor.
- **Not registered, named member by member for types that stay**: `JsFrontend.Parsed`'s positional parameter
  `Program`, the adapted tree, which its positional parameter `Tree`, the library's tree, replaces; and
  `JsAdmissionWalk`'s internal constructor of one parameter, whose walk the static `Walk` makes through a private
  constructor that also takes the function depth. The constructors of `JsBinder`, `JsBinder.Declarations` and
  `JsBinder.HoistingScope` take the library's nodes in place of the adapted tree's and are not deleted.
- The register's paragraph for SJL-13.3's step 2 names the same, with a dated note on step 1's paragraph, whose list
  of what stays named the adapter; the register's row in `rules.register.json` carries a dated revision; and rule
  I5's list of the files it reads replaces `JsFrontendAdapter.Expressions.cs` with `JsTreeFacts.cs` and adds
  `JsCompiler.Modules.cs`, with a dated comment. Part D's control, the type `JsMemberExpression` declared again with
  its member `NameSpan`, and `JsNumberLiteral` declared again, is taken after the merge.

**Evidence at this change.** Development checks, from clean archives of the base `389b55c` and of the branch's
commits, behind an idle gate, with the scripts and logs kept outside the tree. **They are not evidence of bundle
`sjl-13-002`'s rule**, whose runs are all taken after the merge, at the base and the after commit, and they decide
nothing (that bundle's reading 6); no figure of them is quoted here. They found no difference in any outcome,
artifact, specifier list, numeric admission or diagnostic, every checks row as at the base, the fuel twins and the
fuzz sessions as at the base; the test262 comparison found only Exhausted rows whose allowance moves between the wall
clock and fuel, as the base build's own re-runs do.

**Records**, each a dated note quoting what it reads: JSD-0083's status, X6 and decisions D15, D19 and D20; the SJL-13
milestone's SJL-13.3; [JSC-328](../roadmap.corrections.md#jsc-328), for the inventory's parser and tree row, the
frontend area's wide path and the backends plan; bundle `sjl-13-002`'s README, with EX-135, and bundle `sjl-13-001`'s
note on EX-133; the ledger. No rule file is edited, and the rule's revision of 2026-11-13 stands as filed.

## Settling condition

Settled in the change that files it. **This record is reopened only by a new dated record**; step 2 files the dated
note decision D3 asks for.

## What this record does not decide

- **Whether SJL-13 is met.** Its evidence is bundles `sjl-13-001` and `sjl-13-002`, read under their rules, and the
  ledger records the result.
- **Step 2**, which bundle `sjl-13-002`'s rule judges.
- **Broiler.JS's cut-over**, SJL-14's.
- **SJL-11.4**, which re-points this family's regular-expression provider under bundle `sjl-11-001`'s rule as revised
  on 2026-11-10.
- **Any publication** of the shared stream, which is the owner's own (decision D30).

## Falsified if

- a member rule I5 registers is declared again in the family's product sources;
- a source the wide path compiles is parsed by anything but the library, or a unit the library or the admission walk
  refused reaches the numeric admission or the binder;
- a shared code reaches two registry meanings, or a diagnostic carries a code other than the one the mapping's `code`
  row gives its shared code;
- a text this family renders for a shared code states something false of a source the library refuses under it;
- bundle `sjl-13-001`'s runs find a difference its rule's classes do not take.
