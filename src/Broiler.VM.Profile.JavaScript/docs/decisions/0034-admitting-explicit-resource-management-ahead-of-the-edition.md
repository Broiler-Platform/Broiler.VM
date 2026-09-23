<!-- SPDX-FileCopyrightText: 2026 Broiler Platform contributors -->
<!-- SPDX-License-Identifier: Apache-2.0 -->

# JSD-0034 - Admitting explicit resource management ahead of the pinned edition

**Status:** Proposed, owner decision pending. 2026-09-21. The implementation it describes is in the
tree (JSeal F18-F22). Nobody has signed the record, so it claims no approval. Approvals are deferred
under the MVP terms.

**Owner:** JavaScript profile owner. **Co-signer:** verification-boundary owner. **Both roles are held
by one person**, and this record does not claim the co-signature is independent.

**Milestone:** none of this profile's. It answers cards F21 and F22 of the Broiler.JSeal
cross-repository plan (`docs/roadmap.vm-features.md` in that repository). It also settles the record
the F18-F20 status said was owed before those globals are advertised.

## What was open

[JSD-0019](0019-the-pinned-language-edition-and-what-two-of-three-actions-buy.md) pins ES2026 and
confirms that it contains no `UsingDeclaration`, `DisposableStack` or `SuppressedError`.
[JSD-0018](0018-which-tests-are-about-this-language-and-who-decides.md) lets the suite's own
`features.txt` decide what a proposal is, and skips every test that claims one. JSD-0018 was right:
in 2026-09-03 this front end had no production for `using`, so its refusals "passed" 117 negative
tests for the wrong reason.

F18-F20 then published `Symbol.dispose`, `Symbol.asyncDispose`, `SuppressedError`, `DisposableStack`
and `AsyncDisposableStack` in the wide realm without a record. Their Test262 files were scored only
by a separate script. F21-F22 add the syntax. Three questions followed:

1. May the profile implement a proposal the pinned edition does not contain?
2. If so, which revision of the proposal is the oracle?
3. How does the harness score the proposal's tests without reopening what JSD-0018 closed?

## Decision

1. **Explicit resource management is admitted as a whole, ahead of the edition.** This covers the
   two symbols, `SuppressedError`, both stacks, the two iterator disposers, `using` and
   `await using`. It is admitted as one feature because the proposal is one feature. The runtime
   half is not a complete answer without the syntax, and the syntax cannot be tested without the
   runtime half. No part of it is gated behind a surface identity. Its globals already are in the
   wide realm's published set (`docs/realm/globals.txt`, rule N17), and a syntax nobody can reach
   through a global needs no declaration in the artifact.
2. **The revision relied on is `tc39/proposal-explicit-resource-management` at commit
   `38c13295dc20c2273ba0a6ed82555f1fabb37764` (2024-06-15, Stage 3).** That commit includes
   `e37aa9a3`, which introduced the `needsAwait`/`hasAwaited` rule of `DisposeResources`, and
   `fd4da064`, which added the `for (using of …` lookahead restriction. Both are observable, and both
   are what the pinned Test262 (`ccaac100`) tests. The commit was identified on 2026-09-21 from the
   repository's commit listing. **The proposal text is not archived here.** Archiving it, or
   replacing this pin with the edition that absorbs the proposal, is a condition of acceptance, as
   section 24 of the gates roadmap requires for the edition.
3. **The `--test262` command scores the proposal's tests; every other proposal stays skipped.**
   `SuiteFeatures.AdmittedProposals` names `explicit-resource-management` beside this record's
   number. A test claiming it is scored like any other test, and a test claiming any other proposed
   flag is still skipped. The transcript's `features` line names the admitted flag. The
   ingested-dialect command (`--suite`) is unchanged. Its fixture suite's flag list is a format
   exercise, and two of its fixtures exist to prove that a proposal is excluded.

## What the implementation commits to

- **Grammar and static semantics are those of the pinned revision.** `using` and `await using` are
  accepted in blocks, function bodies, class static blocks, modules and `for`/`for-of` heads.
  `await using` is accepted only where `await` is the operator. They are syntax errors at the top
  level of a script or an eval, directly in a case clause, as the body of a single-statement
  position or a label, in a `for-in` head, with a binding pattern, without an initialiser outside an
  enumerating head, when they bind `let`, and when written `export using`. `using` stays an ordinary
  identifier wherever no name follows on the same line. An escaped `using` is not the keyword.
  Two static-semantics gaps were inherited from the front end and were not closed by F21: a strict
  reserved word was not refused as a lexical binding name (`"use strict"; const static = null` was
  accepted, and so was `using static = null`), and `await` was not reserved inside a class static
  block (`let await` and `using await` were accepted there). The first was closed by the front-end
  repairs (VM-FIX-D); the second was closed on 2026-09-21 by F21's completion
  (`docs/evidence/jseal-finally-lowering`), which makes `await` neither a name nor the operator in a
  static block's own statement list.
- **The lowering is five instructions over a hidden scope value** (`DisposeScope` `0xA0`,
  `DisposeAdd` `0xA1`, `DisposeFold` `0xA2`, `DisposeStep` `0xA3`, `DisposeEnd` `0xA4`). Their
  verifier rules, executor arms and baseline native entry points are specified in `JsOpcode`. A
  normal exit, `break`, `continue` and `return` dispose inline, in code that no exception region
  of the statements the jump leaves covers (a region is written as several rows around that code),
  so a disposer that throws there cannot re-enter a `finally` the jump already ran. A throw and a
  generator's forced return dispose from a `finally`-kind region. Completions combine through `SuppressedError` with
  the payloads unchanged.
- **Asynchronous disposal awaits through the ordinary `Await`.** Each await costs the turn the
  specification's `Await` costs, and nothing blocks. A synchronous scope never suspends.
- **VM policy for budgets.** Registration charges fuel and live bytes per resource, as a stack
  entry does. Unwinding charges one fuel unit per entry. Only a guest `throw` is folded into the
  completion. An allowance that runs out or a cancellation during disposal ends the invocation with
  the remaining disposers **not run**. No cleanup runs after the budget is spent, unmetered or
  otherwise, which is the same policy F19/F20 recorded for the stacks.

## What this rejects

- **A named optional surface (`broiler.javascript.resources`).** A surface is how a composition
  declines globals it does not want. Nobody has asked to decline this one. A surface would also
  gate the syntax on a declaration the artifact would have to carry. That would be a format change
  that buys nothing, and a composition could decline half of one feature.
- **Admitting by a command-line switch.** JSD-0018 rejected a switch for exclusion because forgetting
  it over-scores. For admission, forgetting it would under-report and hide failures. The record and
  the code entry move together or not at all.
- **Lowering `using` as `const` plus a hidden `try`/`finally` built from existing instructions.** A
  `finally` block cannot see the value that was thrown. `SuppressedError` needs that value. The
  card also excludes parsing `using` and lowering it as an ordinary declaration.
- **Settling async disposal through an internal promise** (reusing `disposeAsync`'s run). That adds
  one turn at every scope exit, and a program counting ticks against its own `then` chains can see
  it.

## What this does not decide

- It accepts no milestone, and it adds no module capability claim for JSeal (card I13 owns that).
- It does not make the feature filter edition-aware. Any other proposal still needs a record of its
  own.
- The slice manifest's front end still has no production for either declaration and refuses them
  as it always did. The numeric manifest refuses `using` by name.
