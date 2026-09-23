# The unwinding a jump inlines, and `await` in a static block: JSeal slice F21 (completion)

Date: 2026-09-21. Owner: JavaScript profile. Reviewer: none.
This is **local implementation validation, not accepted milestone evidence** or a conformance score.
It closes the two acceptance items the F21 record left unmet
([jseal-f21-f22](../jseal-f21-f22/README.md), "Not exercised, or known wrong"). Nothing here marks a
JSP/JSW stage or a milestone complete.

Checkout: a detached worktree at `484f389` of Broiler.VM plus the merged wave 1-3 patch
(`base-vm-w4`). Release build, Windows 11, .NET SDK 10.0.401. Node v24.17.0 was used only as a
diagnostic comparison. Pinned suite: `test262-ccaac100ff49d81e9ff47a75ff4c60e0bd3f262e`.

## The defect

A `break`, `continue` or `return` that leaves a `try … finally`, a `for … of` or a resource scope
runs that statement's unwinding inline, where the jump is: the finaliser's body, the iterator's
`return`, the scope's disposal. That code was emitted inside every protected range around the jump,
including the ranges of the statements it had already left. So what it threw re-entered their
handlers:

- `try { return 1; } finally { throw 2; }` ran the finaliser twice (`fin,fin`);
- a disposer that threw after a passed `finally` re-ran that `finally` (`fin,a,fin`);
- an iterator whose `return` threw on a `return` path re-ran a passed `finally`, and was caught
  by a `catch` of a `try` the `return` had left (`ret,wrong`);
- a generator suspended at a `yield` inside such an inlined finaliser re-ran it on `return()`,
  and a `throw()` there reached the left `try`'s `catch`.

[probe-before.txt](probe-before.txt) is the new probe on the base build: 32 of its 46 cases differ
from the retained answers, which agree with Node. Each difference is one of the above, a
static-block early error (cases 25-30 and 32-35), or a forced return that left a `for … of` without
closing its iterator (cases 43-44).

## What changed

- **Split exception regions** (`JsCompiler`, lowering only). Each statement that owns regions (a
  `try`, a `for … of`, a resource scope) has a nesting level. The unwinding sequence of a jump
  records, per statement it leaves, a hole from where that statement's unwinding begins to the end
  of the jump. When a region is closed, the holes of its own level or a shallower one are cut out
  of it, and the region is written as the rows that remain, in order and at the position the whole
  range had. Nothing else changed: no instruction, no row format, no verifier rule, no native entry
  point. A region can now be several rows with one handler. The verifier already seeds a handler
  once and requires every row naming it to agree on height and depth. `JsBaselineBlocks` already
  marks each landing once. A range that would lose every instruction gets a `Nop` first, for the
  same reason `ProtectSomething` exists. Hole lookup is a binary search over a list ordered by end
  offset, so closing a region costs the holes inside it.
- **Forced return through a synchronous `for … of`.** The loop now records a `finally`-kind region
  after its catch region, as an array pattern already does. A generator's `return()` at a `yield`
  in the body now closes the iterator. Before, it left without calling `return`. `for await` is
  unchanged (see below).
- **`await` in a class static block** (`JsParser`). A static block's own statement list is
  `[+Await]` and may not contain an await. So `await` is neither a name nor the operator there:
  `using await = null`, `let`, `const`, `var`, `class`, `function` declarations, catch parameters,
  patterns, labels, references, `extends await`, a computed key and `${await}` are all syntax
  errors. A function boundary clears the rule: a nested function, method, accessor, field
  initialiser or arrow body may use the word. An arrow's parameters and a function declaration's
  name inherit it. The verdict of each of 41 shapes was compared with Node, and all 41 agree.
- **Records.** Proposed decision JSD-0034 now says both inherited gaps are closed and how inline
  disposal is kept out of the regions it leaves. `FinallyMatrixChecks` gained four rows. A new
  differential probe, `the-unwinding-through-finally.js`, has 46 cases.

No opcode, diagnostic code, decision number, product file, public member or global was added. Rule
H3's count, the API baseline, rule N17 and `globals.txt` are untouched.

## Executed on Windows (win-x64, Release, bytecode form)

- **Gates.** Base (before any change): `dotnet build Broiler.VM.slnx -c Release`, then
  `dotnet test Broiler.VM.slnx -c Release --no-build`: Contract 267/267, Architecture 256/256.
  After: build, `BROILER_ASSURANCE_WRITE=1 dotnet test …`, build, gate-mode test: Contract
  267/267, Architecture 256/256.
- **Slice-compiler checks** (`… SliceCompiler.exe --checks`). After: 331 checks pass and 2 are
  not run on this machine (x86-64-sysv); the base figure was 327 and 2. On the base tree with only
  the four new matrix rows added, the four new rows fail and nothing else does
  ([matrix-before.txt](matrix-before.txt)).
- **CLI acceptance** (`python eng/run-cli-acceptance.py --binary-directory …`): 219 of 219.
  **Host-surface lane** (`… Cli.exe --host-surface`): every check passed.
- **Differential.** `python eng/run-differential.py --only the-unwinding-through-finally --against
  node --timeout 20` exits 0 with no declared divergence; `the-using-declarations` against Node
  also exits 0, unchanged. The full retained lane differs only in `the-later-library-methods` case
  36 (cube root), which is pre-existing.
- **Test262, focused selection** (`try`, `for-of`, `using`, `await-using`, both `class`
  directories, generators and their prototypes, async generators, `for-in`, `for`, `labeled`,
  `break`, `continue`, `return`, `for-await-of`, `switch`, `block`, `while`, `do-while`, `yield`,
  `let`, `const` and `staging/explicit-resource-management`; 13560 files, 26640 variants). The pass
  count moves from 26287 to 26301 and no variant regressed. The 14 variants that now pass are
  `static-init-await-binding-invalid.js` under `statements/{class, const, let, try, using}`,
  `expressions/class/static-init-await-binding.js` and
  `try/completion-values-fn-finally-abrupt.js`, each in both modes.
  `staging/explicit-resource-management` stays at 103 of 105.
- **Test262, all of `test/language` and `test/built-ins`** (`--dir test/language --dir
  test/built-ins --shards 6 --jobs 6`; 47450 files, 86224 variants). The pass count moves from
  73363 to 73391; unsupported (1617), exhausted (16) and skipped (5593) are unchanged, and no
  variant that passed before fails after. The 28 variants that now pass are the 14 above and the
  static-block `await` tests under `expressions/arrow-function`, `expressions/object`,
  `identifier-resolution`, `statements/function` and `statements/variable` (including its two
  destructuring cases). The per-variant lists are in
  [test262-comparison.txt](test262-comparison.txt).

## Not exercised, or known wrong

- **`for await` under a generator's forced return.** An async generator suspended in a `for await`
  body still leaves on `return()` without calling the iterator's `return` (Node calls it). The
  asynchronous close suspends, and this lowering does not park a forced return across an await.
  This is pre-existing and not fixed.
- **Native form** not run (`ProfileFault/UnsatisfiedHostAssumption` on this host, pre-existing). The
  baseline plan and scan checks in `--checks` pass over the split regions.
- Linux was not run. The Broiler.JS comparison was not run.
- Test262 has no test that turns on the new forced-return close of a synchronous `for … of`; it is
  covered only by probe cases 23 and 43-46 and was compared with Node.
- The focused "before" run used `--shards 1 --jobs 4` and every other run `--shards 6 --jobs 6`;
  a shard count changes how files are partitioned, not a verdict.
