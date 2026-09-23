// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE REFERRER OF import() IN EVAL CODE AND FUNCTION BODIES (JSeal
// I12-upstream, JSD-0024 section 20).
//
// The language resolves such an import against GetActiveScriptOrModule(): eval code runs as the
// module that evaluated it, a function the Function constructor makes keeps the module that was
// running when it was made, however it is later called, and a promise job runs with the module
// that queued it (HostEnqueuePromiseJob). Every case's code is created by modules/referrer-inner.mjs,
// one directory below this file, and imports './referrer-target.mjs' relative to THAT module, so a
// referrer of nothing - resolved against this file's directory - finds no module. Each case prints
// whether the namespace is the one the static import below reached, after its own number.
import * as target from "./modules/referrer-target.mjs";
import { answers, viaEval, made, viaJobEval, viaJobFunction } from "./modules/referrer-inner.mjs";
let n = 0;
function out(r) { n++; print(n + " " + r); }
function same(promise) {
  return promise.then(function (ns) { return String(ns === target); }, function (e) { return e.name; });
}

// --- cases 1-5 ran at that module's own level: indirect eval, direct eval in a function, a
// Function body, an arrow function eval code made, and a Function body eval code made
for (const answer of answers) out(answer);

// --- eval code another module evaluated resolves against that module
out(await same(viaEval("./referrer-target.mjs")));

// --- a Function body another module made, called from here, keeps that module
out(await same(made("./referrer-target.mjs")));

// --- eval called directly by a promise job that module queued
out(await same(viaJobEval()));

// --- a Function made directly by a promise job that module queued
out(await same(viaJobFunction()));
