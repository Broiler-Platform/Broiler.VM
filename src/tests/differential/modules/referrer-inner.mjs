// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Eval code and Function bodies created by THIS module, one directory below the probe: their
// import() resolves against this module, so './referrer-target.mjs' names its sibling. A host that
// offered the empty referrer instead would resolve it against the probe's directory and find
// nothing, which is what makes every case below able to fail.
import * as target from "./referrer-target.mjs";
function same(promise) {
  return promise.then(function (ns) { return String(ns === target); }, function (e) { return e.name; });
}
function direct(s) { return eval("import(s)"); }

export const answers = [];

// --- indirect eval at this module's own level
answers.push(await same((0, eval)('import("./referrer-target.mjs")')));

// --- direct eval inside one of this module's functions
answers.push(await same(direct("./referrer-target.mjs")));

// --- a Function body made here
answers.push(await same(Function("s", "return import(s)")("./referrer-target.mjs")));

// --- an arrow function eval code made
answers.push(await same((0, eval)("(s => import(s))")("./referrer-target.mjs")));

// --- a Function body eval code made
answers.push(await same((0, eval)('Function("s", "return import(s)")')("./referrer-target.mjs")));

export function viaEval(s) { return (0, eval)("import(" + JSON.stringify(s) + ")"); }
export const made = Function("s", "return import(s)");

// A promise job calls eval, and the Function constructor, directly: no frame of this module is on
// the stack when they run, but this module queued the jobs.
export function viaJobEval() {
  return Promise.resolve('import("./referrer-target.mjs")').then(eval);
}
export function viaJobFunction() {
  return Promise.resolve('return import("./referrer-target.mjs")').then(Function).then(f => f());
}
