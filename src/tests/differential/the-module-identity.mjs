// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER MODULE IDENTITY ACROSS STATIC AND DYNAMIC IMPORT (JSeal I11 upstream).
//
// One key is one module in one realm: a static namespace import, an `import()` of the same file and
// two `import()`s started in the same turn all answer one namespace, and the body runs once. A
// module that is still awaiting when a second graph that depends on it is started is waited for,
// not read early. A missing module rejects, and rejects again. A cycle reached only dynamically
// meets the cross-module dead zone, and a top-level `await` that throws rejects the import. A module
// whose evaluation failed stays failed: every later import of it, or of a module that depends on
// it, rejects with the identical value and no body runs again. A member of a cycle whose root is
// still awaiting is waited for, and so is one whose root is still waiting on another import's module
// (ES2026 Evaluate step 3); when that root then throws, the member and a module depending on it
// reject with the root's value and the dependant's body never runs.
// Dependencies are under `modules/`, so the runner does not treat them as probes.
import * as depStatic from "./modules/identity-dep.mjs";
import { bump } from "./modules/identity-dep.mjs";

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }

// --- a static namespace import and an import() of the same file are one module
var dynamic = await import("./modules/identity-dep.mjs");
p(function () { return dynamic === depStatic; });

var both = await Promise.all([import("./modules/identity-dep.mjs"), import("./modules/identity-dep.mjs")]);
p(function () { return (both[0] === both[1]) + ":" + (both[0] === depStatic) + ":" + globalThis.identityDepRuns; });

bump();
p(function () { return dynamic.count + ":" + depStatic.count; });

// --- two import()s of a module nothing imported statically, started in the same turn
var fresh = await Promise.all([import("./modules/identity-fresh.mjs"), import("./modules/identity-fresh.mjs")]);
p(function () { return (fresh[0] === fresh[1]) + ":" + globalThis.identityFreshRuns; });

var freshAgain = await import("./modules/identity-fresh.mjs");
p(function () { return (freshAgain === fresh[0]) + ":" + globalThis.identityFreshRuns; });

// --- a missing module rejects, with an Error, every time it is asked for
var missing = await import("./modules/identity-nowhere.mjs").then(
  function () { return "resolved"; }, function (e) { return "rejected:" + (e instanceof Error); });
p(function () { return missing; });

var missingAgain = await import("./modules/identity-nowhere.mjs").then(
  function () { return "resolved"; }, function (e) { return "rejected:" + (e instanceof Error); });
p(function () { return missingAgain; });

// --- a graph started while a module it depends on is still awaiting waits for that module
var first = import("./modules/identity-slow.mjs");
var second = import("./modules/identity-after-slow.mjs");
var settled = await Promise.all([first, second]);
p(function () { return settled[1].seen + ":" + settled[0].v; });

// --- a cycle reached only by import()
var cycle = await import("./modules/identity-cycle-a.mjs");
p(function () { return cycle.fromB + ":" + cycle.late + ":" + cycle.a; });

// --- a top-level await that throws rejects the import with what was thrown
var thrown = await import("./modules/identity-tla-throws.mjs").then(
  function () { return "resolved"; }, function (e) { return e.name + ":" + e.message; });
p(function () { return thrown; });

// --- a module whose last statement is a pending promise still finishes evaluating
var lastPromise = await import("./modules/identity-last-promise.mjs");
p(function () { return lastPromise.done; });

// --- an errored module stays errored: a second import rejects with the identical value
function outcome(promise) {
  return promise.then(function () { return { ok: true }; }, function (e) { return { ok: false, e: e }; });
}

var boom1 = await outcome(import("./modules/identity-throws.mjs"));
var boom2 = await outcome(import("./modules/identity-throws.mjs"));
p(function () { return boom1.ok + ":" + boom2.ok + ":" + (boom1.e === boom2.e) + ":" + boom1.e.message + ":" + globalThis.identityThrowsRuns; });

// --- a module that depends on an errored module rejects with the dependency's value and never runs
var onBoom = await outcome(import("./modules/identity-on-throws.mjs"));
p(function () { return onBoom.ok + ":" + (onBoom.e === boom1.e) + ":" + globalThis.identityOnThrowsRan; });

// --- the same for a module whose top-level await rejected
var tla1 = await outcome(import("./modules/identity-tla-throws.mjs"));
var tla2 = await outcome(import("./modules/identity-on-tla-throws.mjs"));
p(function () { return tla1.ok + ":" + tla2.ok + ":" + (tla1.e === tla2.e) + ":" + globalThis.identityOnTlaThrowsRan; });

// --- a top-level-await module that throws before its first await, imported twice
var early1 = await outcome(import("./modules/identity-tla-sync-throws.mjs"));
var early2 = await outcome(import("./modules/identity-tla-sync-throws.mjs"));
p(function () { return early1.ok + ":" + early2.ok + ":" + (early1.e === early2.e) + ":" + early1.e.name + ":" + globalThis.identityTlaSyncRuns; });

// --- a cycle whose root throws after the other member ran: both members are errored
var cycA = await outcome(import("./modules/identity-errcycle-a.mjs"));
var cycB = await outcome(import("./modules/identity-errcycle-b.mjs"));
p(function () { return cycA.ok + ":" + cycB.ok + ":" + (cycA.e === cycB.e) + ":" + globalThis.identityErrCycleBRuns; });

// --- a cycle member whose own body finished waits for its still-awaiting cycle root
globalThis.identityAwaitLog = [];
var rootImport = import("./modules/identity-await-a.mjs");
for (var spin = 0; spin < 20 && !globalThis.identityReleaseA; spin++) { await null; }
var memberImport = import("./modules/identity-await-b.mjs").then(function () { identityAwaitLog.push("B settled"); });
for (var spin2 = 0; spin2 < 20; spin2++) { await null; }
identityAwaitLog.push("releasing");
globalThis.identityReleaseA();
await Promise.all([rootImport, memberImport]);
p(function () { return identityAwaitLog.join(","); });

// --- a cycle member that ran while its root still waits on another import's module waits for the root
globalThis.identitySplitLog = [];
var slowImport = import("./modules/identity-split-slow.mjs");
for (var spin3 = 0; spin3 < 20 && !globalThis.identitySplitRelease; spin3++) { await null; }
var splitRoot = import("./modules/identity-split-a.mjs").then(function () { identitySplitLog.push("A settled"); });
for (var spin4 = 0; spin4 < 20; spin4++) { await null; }
var splitMember = import("./modules/identity-split-b.mjs").then(function () { identitySplitLog.push("B settled"); });
for (var spin5 = 0; spin5 < 20; spin5++) { await null; }
identitySplitLog.push("releasing");
globalThis.identitySplitRelease();
await Promise.all([slowImport, splitRoot, splitMember]);
p(function () { return identitySplitLog.join(","); });

// --- a cycle member that ran, and a module depending on it, fail with their root's later error
globalThis.identityRootFailLog = [];
var rfSlow = outcome(import("./modules/identity-rootfail-slow.mjs"));
for (var spin6 = 0; spin6 < 20 && !globalThis.identityRootFailRelease; spin6++) { await null; }
var rfA = outcome(import("./modules/identity-rootfail-a.mjs"));
for (var spin7 = 0; spin7 < 20; spin7++) { await null; }
var rfB = outcome(import("./modules/identity-rootfail-b.mjs"));
var rfC = outcome(import("./modules/identity-rootfail-c.mjs"));
for (var spin8 = 0; spin8 < 20; spin8++) { await null; }
identityRootFailLog.push("releasing");
globalThis.identityRootFailRelease();
var rf = await Promise.all([rfSlow, rfA, rfB, rfC]);
var rfB2 = await outcome(import("./modules/identity-rootfail-b.mjs"));
p(function () {
  return identityRootFailLog.join(",") + ";" + rf[0].ok + ":" + rf[1].ok + ":" + rf[2].ok + ":" + rf[3].ok + ":" + rfB2.ok + ";" +
    (rf[1].e === rf[2].e) + ":" + (rf[1].e === rf[3].e) + ":" + (rf[1].e === rfB2.e) + ":" + rf[1].e.message;
});
