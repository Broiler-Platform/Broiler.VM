// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER TWO MODULE-GOAL REPAIRS FOUND DURING JSEAL WAVES 1-2 (VM-FIX-D).
//
// Several top-level destructuring declarations each bind the names in their own pattern; and a
// module whose evaluation threw is finished with that error: every later import of it, or of a
// module that depends on it, rejects with the identical value and runs no body again, while a
// module the failed walk never reached is still evaluated by a later import. An import made while
// the module is still evaluating waits for it and settles as that evaluation does, and a module of
// a cycle that finished before another member threw is given the same error; an async module whose
// last statement's value is a promise still finishes. Cases 1-4 print as they run; cases 5-21
// print once the chain of dynamic imports has settled, in chain order.
const source = { a: 1, b: 2, c: 3, d: 4 };
const { a } = source;
const { b } = source;
const [first, { c }] = [5, source];
const { d, ...rest } = { d: 6, e: 7 };
export const { exported } = { exported: 8 };

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }

// --- top-level destructuring declarations (1-4)
p(function () { return a + " " + b; });
p(function () { return first + " " + c + " " + d + " " + rest.e; });
p(function () { return exported; });
p(function () { a = 9; return "wrote"; });

// --- a module whose evaluation threw (5-13)
var log = [];
function note(line) { log.push(line); }
var thrown;
var late;
var early;
import("./modules/evaluation-dependent.mjs")
  .then(function () { note("resolved"); }, function (e) {
    thrown = e;
    note(e.tag + " runs=" + globalThis.evaluationThrowsRuns + " dependentRan=" + globalThis.evaluationDependentRan);
    return import("./modules/evaluation-throws.mjs");
  })
  .then(function () { note("resolved"); }, function (e) {
    note((e === thrown) + " runs=" + globalThis.evaluationThrowsRuns);
    return import("./modules/evaluation-dependent.mjs");
  })
  .then(function () { note("resolved"); }, function (e) {
    note((e === thrown) + " dependentRan=" + globalThis.evaluationDependentRan);
    return import("./modules/evaluation-both.mjs");
  })
  .then(function () { note("resolved"); }, function (e) {
    note((e === thrown) + " siblingRuns=" + globalThis.evaluationSiblingRuns);
    return import("./modules/evaluation-sibling.mjs");
  })
  .then(function (ns) {
    note(ns.sibling + " siblingRuns=" + globalThis.evaluationSiblingRuns);
    return import("./modules/evaluation-late.mjs");
  })
  .then(function () { note("resolved"); }, function (e) {
    late = e;
    note(e.name + " " + e.message + " runs=" + globalThis.evaluationLateRuns);
    return import("./modules/evaluation-late.mjs");
  })
  .then(function () { note("resolved"); }, function (e) {
    note((e === late) + " runs=" + globalThis.evaluationLateRuns);
    return import("./modules/evaluation-early.mjs");
  })
  .then(function () { note("resolved"); }, function (e) {
    early = e;
    note(e.name + " " + e.message + " runs=" + globalThis.evaluationEarlyRuns);
    return import("./modules/evaluation-early.mjs");
  })
  .then(function () { note("resolved"); }, function (e) {
    note((e === early) + " runs=" + globalThis.evaluationEarlyRuns);
  })
  // --- imports made while the module is still evaluating, and a cycle (14-21)
  .then(function () {
    return Promise.allSettled([
      import("./modules/evaluation-concurrent-throws.mjs"),
      import("./modules/evaluation-concurrent-throws.mjs")]);
  })
  .then(function (r) {
    note(r[0].status + " " + r[1].status + " " + (r[0].reason === r[1].reason) +
      " runs=" + globalThis.evaluationConcurrentRuns);
    var one = import("./modules/evaluation-concurrent-value.mjs");
    var two = import("./modules/evaluation-concurrent-value.mjs");
    return two.then(function (ns) { note("second " + t(function () { return ns.value; })); return one; });
  })
  .then(function (ns) {
    note("first " + ns.value);
    return import("./modules/evaluation-cycle-first.mjs");
  })
  .then(function () { note("resolved"); }, function (e) {
    thrown = e;
    note(e.message);
    return import("./modules/evaluation-cycle-second.mjs");
  })
  .then(function () { note("resolved"); }, function (e) {
    note(e === thrown);
    var base = import("./modules/evaluation-awaited.mjs");
    return import("./modules/evaluation-awaited-dependent.mjs").then(function (ns) { return base.then(function () { return ns; }); });
  })
  .then(function (ns) {
    note("dependent " + ns.dependent);
    return import("./modules/evaluation-self.mjs");
  })
  .then(function () { return globalThis.evaluationSelf; })
  .then(function (late) {
    note("self " + late);
    return import("./modules/evaluation-self-then.mjs");
  })
  .then(function () { return globalThis.evaluationSelfThen; })
  .then(function (value) { note("self-then " + value); })
  .then(function () {
    for (var i = 0; i < log.length; i++) {
      __n++;
      print(__n + " " + log[i]);
    }
  });
