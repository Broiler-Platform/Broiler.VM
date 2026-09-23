// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE EVALUATION OF ASYNC MODULES (JSeal I11-async).
//
// A module with a top-level `await` does not hold up the modules beside it: its siblings, and every
// module that does not depend on it, run while it is suspended (InnerModuleEvaluation). A module
// that waits on async dependencies runs when the last of them finishes, and the modules one
// completion releases run in the order they were first reached ([[AsyncEvaluationOrder]],
// AsyncModuleExecutionFulfilled). A rejection fails every module waiting on it and nothing else
// (AsyncModuleExecutionRejected). Each graph is imported on its own and logs what ran, in order,
// under `modules/siblings-*`. Each case prints its own number.
globalThis.siblingsLog = [];
let n = 0;
function out(r) { n++; print(n + " " + r); }
function outcome(promise) {
  return promise.then(function () { return "ok"; }, function (e) { return e.name + ":" + e.message; });
}
async function turns(count) { for (let i = 0; i < count; i++) { await null; } }
async function graph(specifier) {
  globalThis.siblingsLog = [];
  const settled = await outcome(import(specifier));
  await turns(10);
  return settled + ";" + globalThis.siblingsLog.join(",");
}

// --- a synchronous sibling runs before its async sibling resumes (the Test262 shape)
const checked = await import("./modules/siblings-check-root.mjs");
out(checked.check);

// --- two async siblings and a synchronous one all start before either async one resumes
out(await graph("./modules/siblings-two-root.mjs"));

// --- two synchronous parents of one async module run in the order they were reached
out(await graph("./modules/siblings-diamond-root.mjs"));

// --- a parent runs when its own dependency finishes, not in import order
out(await graph("./modules/siblings-race-root.mjs"));

// --- an async parent and a synchronous parent of one async module
out(await graph("./modules/siblings-mixed-root.mjs"));

// --- a rejection fails every parent; an unrelated async sibling still finishes
out(await graph("./modules/siblings-reject-root.mjs"));

// --- a synchronous throw beside a suspended async sibling rejects at once; the sibling finishes
out(await graph("./modules/siblings-sync-throw-root.mjs"));

// --- an async module that throws before its first await rejects its graph; its sibling finishes
out(await graph("./modules/siblings-early-root.mjs"));

// --- an awaiting cycle root, and a module that depends on its member, settle in that order
globalThis.siblingsLog = [];
const cycleRoot = import("./modules/siblings-cycle-a.mjs").then(function () { siblingsLog.push("A settled"); });
await turns(10);
const cycleDependant = import("./modules/siblings-cycle-c.mjs").then(function () { siblingsLog.push("C settled"); });
await turns(10);
siblingsLog.push("releasing");
globalThis.siblingsRelease();
await Promise.all([cycleRoot, cycleDependant]);
out(siblingsLog.join(","));

// --- a second import of a suspended async module waits for it, and its body ran once
globalThis.siblingsLog = [];
const first = import("./modules/siblings-again.mjs").then(function () { siblingsLog.push("first"); });
const second = import("./modules/siblings-again.mjs").then(function () { siblingsLog.push("second"); });
await Promise.all([first, second]);
out(siblingsLog.join(","));

// --- waiting imports of a cycle that waits on another import's module settle in the order the
// specification's async parents give: the root's evaluation, then its dependants', then the rest
globalThis.siblingsLog = [];
const splitSlow = import("./modules/siblings-split-slow.mjs").then(function () { siblingsLog.push("slow settled"); });
await turns(10);
const splitA = import("./modules/siblings-split-a.mjs").then(function () { siblingsLog.push("A settled"); });
await turns(10);
const splitB = import("./modules/siblings-split-b.mjs").then(function () { siblingsLog.push("B settled"); });
const splitC = import("./modules/siblings-split-c.mjs").then(function () { siblingsLog.push("C settled"); });
await turns(10);
siblingsLog.push("releasing");
globalThis.siblingsSplitRelease();
await Promise.all([splitSlow, splitA, splitB, splitC]);
out(siblingsLog.join(","));
