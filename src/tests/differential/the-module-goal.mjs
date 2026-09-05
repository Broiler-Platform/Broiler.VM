// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE MODULE GOAL: THE IMPORT AND EXPORT FORMS, LIVE BINDINGS, THE
// NAMESPACE EXOTIC OBJECT, A CYCLE, AND TOP-LEVEL `await`.
//
// Retained from the bring-up of the family it covers: every case was compared against the
// comparison engine before it was written down, and each prints its own number so a divergence
// names a case rather than a line.
//
// IT IS A `.mjs` AND THE EXTENSION IS THE POINT. A module is a module because of how it is
// PRESENTED, not because of a flag; both engines decide the goal from the file name, so a probe
// over the module goal cannot be written as a `.js` at all. Its dependencies are one directory
// down, under `modules/`, so the runner does not mistake one of them for a probe.
import forty, { counter, bump, frozen } from "./modules/counter.mjs";
import * as everything from "./modules/counter.mjs";
import { tally, forty as fortyAgain, frozen as heldAgain } from "./modules/re-exports.mjs";
import * as reExported from "./modules/re-exports.mjs";
import { observed, fromFirst } from "./modules/cycle-first.mjs";
import { deadZone, fromSecond } from "./modules/cycle-second.mjs";
import { settled } from "./modules/awaits.mjs";

var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }

// --- the import forms
p(function () { return forty; });
p(function () { return frozen; });
p(function () { return typeof bump; });
p(function () { return typeof everything; });
p(function () { return everything.frozen; });
p(function () { return everything.default; });

// --- a live binding is read through, not copied
p(function () { return counter; });
p(function () { bump(); return counter; });
p(function () { return everything.counter; });
p(function () { bump(); return counter + "/" + everything.counter; });

// --- an import binding is immutable, at run time
p(function () { counter = 99; return "assigned"; });
p(function () { return counter; });
p(function () { everything.counter = 99; return "assigned"; });

// --- the namespace exotic object
p(function () { return Object.getPrototypeOf(everything); });
p(function () { return Object.isExtensible(everything); });
p(function () { return everything[Symbol.toStringTag]; });
p(function () { return Object.prototype.toString.call(everything); });
p(function () { return Object.keys(everything).join(","); });
p(function () { return Object.getOwnPropertyNames(everything).join(","); });
p(function () { return "counter" in everything; });
p(function () { return "nowhere" in everything; });
p(function () { return Symbol.toStringTag in everything; });
p(function () { return everything.nowhere; });
p(function () { var d = Object.getOwnPropertyDescriptor(everything, "frozen"); return d.writable + "/" + d.enumerable + "/" + d.configurable; });
p(function () { return Reflect.set(everything, "counter", 1); });
p(function () { return Reflect.set(everything, "nowhere", 1); });
p(function () { return Reflect.defineProperty(everything, "nowhere", {value: 1}); });
p(function () { return Reflect.setPrototypeOf(everything, {}); });
p(function () { return Reflect.setPrototypeOf(everything, null); });
p(function () { return Reflect.deleteProperty(everything, "nowhere"); });
p(function () { return delete everything.nowhere; });
p(function () { return Object.isFrozen(everything); });

// --- a name that is not an identifier is still an export
p(function () { return everything["a name with spaces"]; });
p(function () { return Object.keys(everything).indexOf("a name with spaces") >= 0; });

// --- the re-export forms
p(function () { return tally >= 0; });
p(function () { return fortyAgain; });
p(function () { return heldAgain; });
p(function () { return reExported.counter === everything.counter; });
p(function () { return Object.keys(reExported).join(","); });

// --- a cycle in the graph is ordinary, and the dead zone crosses the boundary
p(function () { return observed; });
p(function () { return deadZone; });
p(function () { return fromFirst() + "/" + fromSecond(); });

// --- top-level `await` orders the graph
//
// The two below are written outside `p` on purpose: `await` is an operator at a MODULE's top level
// and an identifier inside the ordinary function `p` takes, so writing them in a callback would be
// a syntax error rather than a case.
p(function () { return settled; });

var awaited = await Promise.resolve(21);
p(function () { return awaited * 2; });

var order = "before";
Promise.resolve().then(function () { order = "job-ran"; });
await null;
p(function () { return order; });
