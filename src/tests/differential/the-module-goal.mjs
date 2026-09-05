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
import defaultAsyncGenerator, { named as namedAsyncGenerator } from "./modules/async-generators.mjs";

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

// --- `import.meta`, which is the module's own object and nobody else's
//
// The two engines populate it differently, and that is the HOST's business rather than the
// language's. What the language fixes is that it is an ordinary extensible object with no
// prototype, that both evaluations of it in one module answer the same object, and that a guest
// may add to it and take away again. The cases below ask only those; what a host puts in it is
// asked once and declared.
p(function () { return typeof import.meta; });
p(function () { return Object.getPrototypeOf(import.meta); });
p(function () { return import.meta === import.meta; });
p(function () { return Object.isExtensible(import.meta); });
p(function () { import.meta.probe = "mine"; return import.meta.probe; });
p(function () { return delete import.meta.probe; });
p(function () { return Object.keys(import.meta).length; });

// --- a dynamic import, which answers a PROMISE of the module a specifier names
p(function () { return import("./modules/counter.mjs") instanceof Promise; });
p(function () { return import("./modules/counter.mjs") === import("./modules/counter.mjs"); });

var dynamic = await import("./modules/counter.mjs");
p(function () { return dynamic.frozen; });
p(function () { return dynamic.default; });
p(function () { return dynamic === everything; });
p(function () { return Object.prototype.toString.call(dynamic); });

// A SECOND CALL IS THE SAME MODULE AND NOT A SECOND ONE, which is the whole of what a module
// registry is for: the namespace is the same object the static import got, and the counter it
// reads is the one the static import has been incrementing.
var again = await import("./modules/counter.mjs");
p(function () { return again === dynamic; });
p(function () { return again.counter === everything.counter; });

// A COMPUTED SPECIFIER IS THE ORDINARY CASE AND NOT A CURIOSITY. Nothing resolved these before the
// program ran, so they are the ones no bundler could have answered in advance.
//
// EVERY CASE BELOW AWAITS OUTSIDE ITS CALLBACK, and that is a rule of the probe rather than a
// preference: `p` takes an ordinary function and prints what it RETURNS, so a callback that
// answered a promise would have every one of these read `[object Promise]` in both engines and
// compare equal while measuring nothing.
var half = "./modules/";
var rest = "counter.mjs";
var joined = await import(half + rest);
p(function () { return joined === dynamic; });

var coerced = await import({ toString: function () { return "./modules/counter.mjs"; } });
p(function () { return coerced === dynamic; });

// A module nothing can find is a REJECTION and not a throw, which is what makes a dynamic import
// catchable in the ordinary way.
var missing = await import("./modules/nowhere.mjs").then(
  function () { return "resolved"; }, function () { return "rejected"; });
p(function () { return missing; });
p(function () { var caught = "not-yet"; import("./modules/nowhere.mjs").catch(function () { caught = "caught"; }); return caught; });

// And a specifier that will not coerce rejects with what the coercion threw.
var uncoercible = await import({ toString: function () { throw new RangeError("no"); } }).then(
  function () { return "resolved"; }, function (e) { return e.name; });
p(function () { return uncoercible; });

// An empty attributes clause asks nothing of a host, so it loads; an attribute asks for a KIND of
// module, and neither engine here has a loader for the one asked.
var noOptions = await import("./modules/counter.mjs", {});
p(function () { return noOptions === dynamic; });

var emptyClause = await import("./modules/counter.mjs", { with: {} });
p(function () { return emptyClause === dynamic; });

var jsonType = await import("./modules/counter.mjs", { with: { type: "json" } }).then(
  function () { return "resolved"; }, function () { return "rejected"; });
p(function () { return jsonType; });

// The second argument's SHAPE is the language's rule and is settled before any attribute is: these
// three reject for three different reasons and none of them is "no loader for that type".
var optionsNull = await import("./modules/counter.mjs", null).then(
  function () { return "resolved"; }, function (e) { return e.name; });
p(function () { return optionsNull; });

var withNotAnObject = await import("./modules/counter.mjs", { with: 1 }).then(
  function () { return "resolved"; }, function (e) { return e.name; });
p(function () { return withNotAnObject; });

var valueNotAString = await import("./modules/counter.mjs", { with: { type: 1 } }).then(
  function () { return "resolved"; }, function (e) { return e.name; });
p(function () { return valueNotAString; });

// A module that will not LINK and a module that will not PARSE reject with the same error, and it
// is a `SyntaxError` rather than a `TypeError`: the language calls a name nothing exports part of
// the module's own syntax, and a host that answered the two halves differently would be telling the
// guest which of its own passes refused.
var unlinkable = await import("./modules/no-such-export.mjs").then(
  function () { return "resolved"; }, function (e) { return e.name; });
p(function () { return unlinkable; });

var unparsable = await import("./modules/unparsable.mjs").then(
  function () { return "resolved"; }, function (e) { return e.name; });
p(function () { return unparsable; });

// A module with a top-level `await` finishes a turn after it starts, and the promise settles when
// it has FINISHED: the namespace the guest is handed is one whose body has run, so the binding it
// reads is the one the body wrote rather than the dead zone the body has not reached.
var late = await import("./modules/late.mjs");
p(function () { return late.ready; });

// --- an async generator as a module's default export, which is the one `export default` form that
// stayed refused after the family itself was admitted.
var fromDefault = await defaultAsyncGenerator().next();
p(function () { return fromDefault.value + "," + fromDefault.done; });

var fromNamed = await namedAsyncGenerator().next();
p(function () { return fromNamed.value + "," + fromNamed.done; });
