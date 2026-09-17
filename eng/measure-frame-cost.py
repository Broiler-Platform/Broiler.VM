#!/usr/bin/env python3
# SPDX-FileCopyrightText: 2026 Broiler Platform contributors
# SPDX-License-Identifier: Apache-2.0
#
# WHAT THIS MEASURES, AND WHY IT IS A SCRIPT RATHER THAN A TEST.
#
# Roadmap section 8 says the call-depth bound is MEASURED and not chosen, and the workload roadmap's
# JSW-9 says the same thing in the other direction: the per-frame cost of this interpreter is to be
# measured rather than estimated, and the depth maximum derived from that measurement and recorded
# with it.
#
# The quantity is how much NATIVE stack one JavaScript call costs. This interpreter recurses on the
# CLR stack - one JavaScript call is `Call`, then `Invoke`, then `Execute` - so a frame is those
# three CLR frames plus whatever each holds. Nothing inside the process can read that number: a
# stack overflow is the one failure the CLR cannot turn into an exception, so a probe that asked
# "would one more frame fit" could only answer by dying.
#
# So the measurement is made from OUTSIDE, by bisection over the published binary, one child process
# per trial, with the question "did it answer, or did it die".
#
# THERE ARE TWO DEPTHS AND THEY ARE NOT THE SAME NUMBER. That is the finding this script exists to
# keep visible:
#
#   * how deep a recursion can go and RETURN; and
#   * how deep a recursion can go and THROW, with the exception unwinding to a handler above it.
#
# The second was an eighth of the first until 2026-09-04. A frame with a `catch` that rethrows is
# entered during the runtime's second pass, so a throw crossing a thousand interpreter frames
# accumulated a thousand funclets and their dispatchers and the process died - on a stack that holds
# eight thousand ordinary calls. The executor catches by FILTER now, which runs in the first pass and
# does not unwind per frame, and the two depths agree *(JSC-97)*. A build where they diverge again
# has the same defect back, and this script is how that is noticed.
#
# WHAT THE NUMBERS ARE NOT. They are bounded ABOVE by the engine's own `MaximumCallDepth`, which
# answers with a catchable `RangeError` rather than dying, and by the profile's declared call-depth
# maximum, which the core holds a caller to. So what this reports is the smaller of the real capacity
# and the declared bound - which is the right thing to report for a released build, and is NOT a
# measurement of the stack. Measuring the raw capacity means lifting both bounds in a build of your
# own; the figures that arrangement produced on 2026-09-04 are recorded in `JsEngine.MaximumCallDepth`
# beside the bound derived from them.
#
# AND THERE IS A FOURTH OUTCOME, WHICH THIS SCRIPT REPORTED AS A DEATH. The runtime's own stack
# probe is the designed backstop: a call whose probe refuses has nowhere left to build an error
# object, so the operation ENDS - cleanly, with the process alive and an answer of its own. That is
# the stack announcing itself, which is the quantity this whole script exists to find, and the
# classifier below had no name for it. It fell through to the last resort and printed A RECURSION
# TERMINATED THE PROCESS about a process that did not terminate. A measurement harness that reports
# a refusal as a death is the failure mode roadmap section 17 rule 5 exists against, and on
# `linux-x64` it fired on the first machine that met the backstop before it met a declared bound.
#
# THE BACKSTOP AND A GRANTED CEILING NOW ANSWER ALIKE, and the script tells them apart from the one
# thing it knows that the message does not carry: the ceiling it granted. A recursion asked to go
# `d` deep under a granted ceiling of `c > d` cannot have reached that ceiling, so a
# `CeilingReached on CallDepth` at such a depth is the stack. That inference is why the ceiling is
# passed rather than left at the profile's default.
#
# THE OUTPUT FORM IS AN INPUT, BECAUSE THE NATIVE FORM'S CALL IS NOT THE INTERPRETER'S. `--form
# native` hands the host `--native <backend>`, so the program is compiled in the baseline form over
# the wide manifest and one JavaScript call also crosses an emitted unit's frame, the transition into
# a handler and the handler's own frames before it reaches the call path the bytecode form takes -
# and, where the call is re-entered from an instruction inside a block rather than from a call
# instruction, the block step's frame, which is sized like the interpreter's; the shape families
# exist to measure that. The two forms therefore have two per-frame costs, and each is printed with
# the form it was taken in so neither is read as the other. The backend defaults to the x86-64
# convention this host uses, which is the only one the host arms; an artifact emitted for any other
# refuses to instantiate and would measure nothing.
#
# AND THE SHAPE IS AN INPUT, BECAUSE THE ROUTE A LEVEL NESTS THROUGH DECIDES WHAT IT COSTS. `plain`
# is the recursion this script began with: a call instruction, which the baseline form gives a step
# of its own. Most other families recurse through the object model instead - an accessor, an indexed
# or global or `super` read, a setter or a `super` write, a private accessor, a coercion hook, a Proxy
# trap, the native `__proto__` getter and setter, `Symbol.hasInstance`, `with`, `for-in`, object
# spread, the rendering of a thrown object. Those routes re-enter guest code from an instruction that
# the form may be running inside a wider step, so what one level costs there is not what `plain` costs
# and cannot be read off it. The rest keep a step of their own as `plain` does, but nest through a
# different helper on the way to the callee - a built-in, a bound function or a callable Proxy, a
# field initialiser under `super()`, a static element, or the iterator protocol's open, next, close
# and delegation - so they are not `plain` either. Each family is measured on its own.
#
# EACH FAMILY IS A (RETURNING, THROWING) PAIR, and the pair is the point: the two depths must agree,
# and a route where they do not has a throw costing stack that a call does not. Every throwing form
# tells its own exception from the bound's the way `THROWING` does, by the message it threw.
#
# WHICH BUILD A LOG WAS TAKEN FROM IS PART OF THE LOG. `--source-tree` names the tree the binary was
# built from, and the header then carries its commit, whether that tree was dirty (the lifted-bounds
# patch makes it so), and the setting of `PerOpcodeSteps` - the constant deciding whether a run-alone
# entry point runs its own per-opcode step or the shared one. Without the option all three print
# `unknown`, which is a statement and not an omission: a log that does not show the shipped setting
# is a log no depth clause may read.
#
#   python3 eng/measure-frame-cost.py [--binary-directory <dir>] [--stack-bytes <n>] [--ceiling <n>]
#                                     [--form bytecode|native] [--backend <name>]
#                                     [--shape <name>] [--source-tree <dir>]

import argparse
import os
import pathlib
import platform
import subprocess
import sys
import tempfile

ROOT = pathlib.Path(__file__).resolve().parent.parent
DEFAULT_BINARY_DIRECTORY = (
    ROOT / "src/compositions/Broiler.VM.Composition.JavaScript.Cli/bin/Release/net10.0"
)

# The stack `JsExecution.GuestStackBytes` declares for one guest invocation. It is stated here
# rather than read, because a script that read it from the source would agree with the source by
# construction and would stop being able to notice the two disagreeing.
DEFAULT_STACK_BYTES = 96 * 1024 * 1024

# Where `PerOpcodeSteps` is stated, and how its line reads, for `--source-tree`. It is read from the
# source of the tree the binary was built from because nothing the binary prints carries it.
HANDLERS = "src/Broiler.VM.Profile.JavaScript/JsBaselineHandlers.cs"
PER_OPCODE_STEPS = "const bool PerOpcodeSteps"

RETURNING = """function down(n) { return n === 0 ? 0 : down(n - 1); }
print("answered " + down(%d));
"""

# THE THROWING SHAPE HAS TO TELL ITS OWN EXCEPTION FROM THE BOUND'S. Both arrive at the same
# `catch`, and a fixture that printed either as an answer would report that every depth completes -
# which is what the engine's bound firing looks like from inside the program.
THROWING = """function down(n) { if (n === 0) { throw new Error("here"); } down(n - 1); }
try { down(%d); print("bounded no-throw"); }
catch (failure) {
  print(failure.message === "here" ? "answered " + failure.name : "bounded " + failure.name);
}
"""

# THE FAMILIES WRITTEN OUT. Each recurses through one route and through nothing else on its common
# path, so what it measures is that route's per-level cost and not a mixture. `delegate` and `forof`
# nest through the iterator protocol; the rest through the object model.
SHAPES = {
    "plain": (RETURNING, THROWING),
    "getter": (
        'var n = %d; var o = { get down() { if (n === 0) { return 0; } n = n - 1; return this.down; } };\n'
        'print("answered " + o.down);\n',
        'var n = %d; var o = { get down() { if (n === 0) { throw new Error("here"); } n = n - 1; '
        'return this.down; } };\n'
        'try { o.down; print("bounded no-throw"); }\n'
        'catch (failure) { print(failure.message === "here" ? "answered " + failure.name '
        ': "bounded " + failure.name); }\n'),
    "valueof": (
        'var n = %d; var o = { valueOf: function () { if (n === 0) { return 0; } n = n - 1; return +o; } };\n'
        'print("answered " + (+o));\n',
        'var n = %d; var o = { valueOf: function () { if (n === 0) { throw new Error("here"); } n = n - 1; '
        'return +o; } };\n'
        'try { +o; print("bounded no-throw"); }\n'
        'catch (failure) { print(failure.message === "here" ? "answered " + failure.name '
        ': "bounded " + failure.name); }\n'),
    "tostring": (
        'var n = %d; var o = { toString: function () { if (n === 0) { return "a"; } n = n - 1; '
        'return "" + o; } };\n'
        'print("answered " + ("" + o));\n',
        'var n = %d; var o = { toString: function () { if (n === 0) { throw new Error("here"); } n = n - 1; '
        'return "" + o; } };\n'
        'try { "" + o; print("bounded no-throw"); }\n'
        'catch (failure) { print(failure.message === "here" ? "answered " + failure.name '
        ': "bounded " + failure.name); }\n'),
    "proxy": (
        'var n = %d; var p = new Proxy({}, { get: function () { if (n === 0) { return 0; } n = n - 1; '
        'return p.down; } });\n'
        'print("answered " + p.down);\n',
        'var n = %d; var p = new Proxy({}, { get: function () { if (n === 0) { throw new Error("here"); } '
        'n = n - 1; return p.down; } });\n'
        'try { p.down; print("bounded no-throw"); }\n'
        'catch (failure) { print(failure.message === "here" ? "answered " + failure.name '
        ': "bounded " + failure.name); }\n'),
    "delegate": (
        'function* g(n) { if (n === 0) { yield 0; return; } yield* g(n - 1); }\n'
        'var step = g(%d).next();\n'
        'print("answered " + step.value);\n',
        'function* g(n) { if (n === 0) { throw new Error("here"); } yield* g(n - 1); }\n'
        'try { g(%d).next(); print("bounded no-throw"); }\n'
        'catch (failure) { print(failure.message === "here" ? "answered " + failure.name '
        ': "bounded " + failure.name); }\n'),
    "forof": (
        'function* g(n) { if (n > 0) { for (var v of g(n - 1)) { yield v; } } yield n; }\n'
        'var first = g(%d).next();\n'
        'print("answered " + first.value);\n',
        'function* g(n) { if (n === 0) { throw new Error("here"); } for (var v of g(n - 1)) { yield v; } }\n'
        'try { g(%d).next(); print("bounded no-throw"); }\n'
        'catch (failure) { print(failure.message === "here" ? "answered " + failure.name '
        ': "bounded " + failure.name); }\n'),
}

# THE REST, WRITTEN AS ONE TEMPLATE EACH, BECAUSE THE TWO FORMS OF A FAMILY DIFFER IN TWO PLACES AND
# NOWHERE ELSE: the base case, and what the top level does with the answer. Writing each pair out
# would repeat the recursion twice per route and let the two copies drift, and a throwing form that
# drifted from its returning form would compare two shapes rather than two depths of one.
# `@BASE@` is the base case; the third field is the expression that starts the recursion.
TEMPLATES = {
    "global": (
        'var n = %d; Object.defineProperty(globalThis, "down", { get: function () { if (n === 0) { @BASE@ } '
        'n = n - 1; return down; } });', "0", "down"),
    "index": (
        'var n = %d; var k = "down"; var o = { get down() { if (n === 0) { @BASE@ } n = n - 1; '
        'return this[k]; } };', "0", "o[k]"),
    "super": (
        'var n = %d; var base = { get down() { if (n === 0) { @BASE@ } n = n - 1; return this.down; } }; '
        'var o = { __proto__: base, get down() { return super.down; } };', "0", "o.down"),
    "setter": (
        'var n = %d; var o = { set down(v) { if (n === 0) { @BASE@ } n = n - 1; this.down = v; } };',
        "", "(function () { o.down = 1; return n; })()"),
    "toprimitive": (
        'var n = %d; var o = { [Symbol.toPrimitive]: function () { if (n === 0) { @BASE@ } n = n - 1; '
        'return +o; } };', "0", "+o"),
    "in": (
        'var n = %d; var p = new Proxy({}, { has: function () { if (n === 0) { @BASE@ } n = n - 1; '
        'return "down" in p; } });', "false", '"down" in p'),
    "with": (
        'var n = %d; var p = new Proxy({}, { has: function (t, k) { if (k !== "down") { return false; } '
        'if (n === 0) { @BASE@ } n = n - 1; with (p) { typeof down; } return false; } });',
        "false", "(function () { with (p) { typeof down; } return n; })()"),
    "forinproxy": (
        'var n = %d; var p = new Proxy({}, { ownKeys: function () { if (n === 0) { @BASE@ } n = n - 1; '
        'for (var key in p) { } return []; } });',
        "[]", "(function () { for (var key in p) { } return n; })()"),
    "hasinstance": (
        'var n = %d; var C = { [Symbol.hasInstance]: function (v) { if (n === 0) { @BASE@ } n = n - 1; '
        'return v instanceof C; } };', "false", "({} instanceof C)"),
    "spread": (
        'var n = %d; var o = { get down() { if (n === 0) { @BASE@ } n = n - 1; var copy = { ...o }; '
        'return 0; } };', "0", "(function () { var copy = { ...o }; return n; })()"),
    "render": (
        'var n = %d; var e = { get message() { if (n === 0) { @BASE@ } n = n - 1; try { throw e; } '
        'catch (x) { if (x !== e) { throw x; } } return "m"; } };',
        '"m"', '(function () { try { throw e; } catch (x) { if (x !== e) { throw x; } } return n; })()'),

    # THE BLOCK ROUTES THE FAMILIES ABOVE DO NOT REACH: six more Proxy traps, the `__proto__` getter and
    # setter, the private accessors and the `super` write. WHERE SEVERAL INSTRUCTIONS REACH ONE TRAP,
    # THE FAMILY RECURSES THROUGH THE ONE WHOSE ARM NESTS THE MOST NATIVE STACK PER LEVEL, and that was
    # decided by reading, not by measuring: by counting the helper frames that stand between the arm in
    # `ExecuteCore` and the `JsEngine.Call` of the trap, since every frame the helpers leave open
    # stays open under the level above. A tie is named where it falls. The indexed write wins most
    # of them, because `SetIndexed` stands in front of `SetProperty`, and a Proxy with no `set` trap
    # is how a write reaches the receiver's own traps: `ProxySet` forwards to `SetWithReceiver`, which
    # lands the write through `LandOnReceiver`.

    # PROXY `set`: `SetIndex` -> `SetIndexed` -> `SetProperty` -> `JsProxy.ProxySet` -> the trap.
    # `SetProperty` and `StoreGlobal` call `ProxySet` from `SetProperty` itself, one frame fewer.
    "proxyset": (
        'var n = %d; var k = "down"; var p = new Proxy({}, { set: function () { if (n === 0) { @BASE@ } '
        'n = n - 1; p[k] = 1; return true; } });', "true", "(function () { p[k] = 1; return n; })()"),

    # PROXY `deleteProperty`: `DeleteProperty` -> `JsProxy.DeleteOwnProperty` -> `ProxyDelete` -> the
    # trap. `DeleteIndex` reaches the same override with the same two frames, because its
    # `ToPropertyKey` returns before the override is called, so the two tie and the named form stands
    # for both.
    "proxydelete": (
        'var n = %d; var p = new Proxy({}, { deleteProperty: function () { if (n === 0) { @BASE@ } '
        'n = n - 1; delete p.down; return true; } });',
        "true", "(function () { delete p.down; return n; })()"),

    # PROXY `getOwnPropertyDescriptor`: `SetIndex` -> `SetIndexed` -> `SetProperty` -> `ProxySet` ->
    # `SetWithReceiver` -> `LandOnReceiver` -> `JsProxy.TryGetOwnProperty` -> `ProxyGetOwnProperty` ->
    # the trap. `SetProperty` is one frame fewer. `ForInStart` (`JsRealm.CreateEnumerator`),
    # `SpreadObject` (`CopyDataProperties`) and `StoreSuperProperty` (`SetSuper`) call
    # `TryGetOwnProperty` from their first helper, four frames fewer.
    "proxydescriptor": (
        'var n = %d; var k = "down"; var p = new Proxy({}, { getOwnPropertyDescriptor: function () { '
        'if (n === 0) { @BASE@ } n = n - 1; p[k] = 1; return undefined; } });',
        "undefined", "(function () { p[k] = 1; return n; })()"),

    # PROXY `defineProperty`: the same write, which `LandOnReceiver` ends in
    # `JsProxy.SetOwnProperty` -> `DefineOrThrow` -> `ProxyDefineOwnProperty` -> the trap.
    # `SetProperty` is one frame fewer, and `StoreSuperProperty` calls `SetOwnProperty` from `SetSuper`,
    # four frames fewer.
    "proxydefine": (
        'var n = %d; var k = "down"; var p = new Proxy({}, { defineProperty: function () { '
        'if (n === 0) { @BASE@ } n = n - 1; p[k] = 1; return true; } });',
        "true", "(function () { p[k] = 1; return n; })()"),

    # PROXY `getPrototypeOf`: `InstanceOf` -> `JsEngine.InstanceOf` -> `JsProxy.Prototype` ->
    # `ProxyGetPrototypeOf` -> the trap. `ForInStart` (`JsRealm.CreateEnumerator`) and
    # `StoreSuperProperty` (`SetSuper`) read `Prototype` from their first helper too, so the three tie
    # at the same three frames and `instanceof` stands for them.
    "proxyproto": (
        'var n = %d; function F() { } var p = new Proxy({}, { getPrototypeOf: function () { '
        'if (n === 0) { @BASE@ } n = n - 1; p instanceof F; return null; } });',
        "null", "(function () { p instanceof F; return n; })()"),

    # PROXY `getPrototypeOf` THROUGH THE NATIVE `__proto__` GETTER, a family of its own beside
    # `proxyproto`, which keeps the direct route. A read of `__proto__` from a Proxy with no `get` trap
    # is forwarded by `JsProxy.ProxyGet` to `GetWithReceiver`, whose `Lookup` finds the accessor on
    # `Object.prototype` and calls its getter with the Proxy as the receiver, and the getter reads
    # `Prototype` -> `ProxyGetPrototypeOf` -> the trap, two calls a level. The family leaves the getter
    # where the realm installs it, so the arms are the instructions that read the key `__proto__`. The
    # one chosen is `ResolveName`, asking a `with` object's `Symbol.unscopables`, which is the Proxy,
    # whether it hides the name:
    # `ResolveName` -> `Unscopable` -> `GetProperty` -> `Lookup` -> `ProxyGet` -> `GetWithReceiver` ->
    # `Lookup` -> the getter's call, seven frames; from that call to the trap's, every arm takes the
    # same frames. `GetIndex` (`GetIndexed`) is one frame fewer and `GetProperty` two. `SpreadObject`
    # (`CopyDataProperties`) ties `GetIndex`, but reads the key only when the Proxy's `ownKeys` and
    # `getOwnPropertyDescriptor` traps report a `__proto__` its target does not hold, so its level would
    # enter those two traps as well. `LoadSuperProperty` (`Lookup`) is three frames fewer. The trap
    # answers `null`, so the name is not hidden, and the `with` object's own read of `__proto__` then
    # meets only its ordinary prototype.
    "proxyprotoget": (
        'var n = %d; var p = new Proxy({}, { getPrototypeOf: function () { if (n === 0) { @BASE@ } '
        'n = n - 1; with (o) { __proto__; } return null; } }); var o = { [Symbol.unscopables]: p };',
        "null", "(function () { with (o) { __proto__; } return n; })()"),

    # PROXY `isExtensible`: the same write, which `LandOnReceiver` sends through `JsProxy.Extensible`
    # -> `ProxyIsExtensible` -> the trap before it defines. `SetProperty` is one frame fewer and
    # `StoreSuperProperty` (`SetSuper`) four fewer. The key is new at every level, because
    # `LandOnReceiver` asks only for a key the receiver does not already hold, and each level's write
    # defines its own.
    "proxyextensible": (
        'var n = %d; var p = new Proxy({}, { isExtensible: function () { if (n === 0) { @BASE@ } '
        'n = n - 1; p["k" + n] = 1; return true; } });',
        "true", '(function () { p["k" + n] = 1; return n; })()'),

    # PROXY `setPrototypeOf`, WHICH A BLOCK INSTRUCTION OF LOWERED CODE REACHES ONLY THROUGH THE NATIVE
    # `__proto__` SETTER:
    # `SetIndex` -> `SetIndexed` -> `SetProperty` -> `ProxySet` -> `SetWithReceiver` -> the setter's call
    # -> `ObjectSetPrototype` -> `ProxySetPrototypeOf` -> the trap, two calls a level. `SetProperty`
    # reaches the setter one frame sooner.
    "protoset": (
        'var n = %d; var k = "__proto__"; var p = new Proxy({}, { setPrototypeOf: function () { '
        'if (n === 0) { @BASE@ } n = n - 1; p[k] = null; return true; } });',
        "true", "(function () { p[k] = null; return n; })()"),

    # A PRIVATE ACCESSOR: `LoadPrivate` -> `ReadPrivate` -> the getter, and `StorePrivate` ->
    # `WritePrivate` -> the setter. Each helper is reached by that one instruction alone.
    "privateget": (
        'var n = %d; class P { get #down() { if (n === 0) { @BASE@ } n = n - 1; return this.#down; } '
        'read() { return this.#down; } } var o = new P();', "0", "o.read()"),
    "privateset": (
        'var n = %d; class P { set #down(v) { if (n === 0) { @BASE@ } n = n - 1; this.#down = v; } '
        'write() { this.#down = 1; return n; } } var o = new P();', "", "o.write()"),

    # A `super` WRITE: `StoreSuperProperty` -> `SetSuper` -> the setter found above the home object,
    # and no other instruction reaches that call. A level is two setters, as `super`'s is two getters:
    # the base's setter recurses by writing to the instance, whose own setter writes through `super`.
    "superset": (
        'var n = %d; var base = { set down(v) { if (n === 0) { @BASE@ } n = n - 1; this.down = v; } }; '
        'var o = { __proto__: base, set down(v) { super.down = v; } };',
        "", "(function () { o.down = 1; return n; })()"),

    # THE RUN-ALONE ROUTES `plain`, `forof` AND `delegate` DO NOT REACH. Each instruction keeps its own
    # step, and the level nests through a helper `plain` never enters. `for await` and an async
    # `yield*` have no family: they settle through the job queue, and `family()` cannot write them.

    # A FIELD INITIALISER UNDER `super()`: `SuperConstruct` constructs the base, then
    # `InitialiseInstanceElements` -> `ApplyClassElements` calls the initialiser, which recurses.
    # Three calls a level: the `new`, the initialiser and the recursion.
    "superfield": (
        'var n = %d; class B { } function make() { if (n === 0) { @BASE@ } n = n - 1; '
        'class D extends B { x = make(); } return new D().x; }', "0", "make()"),

    # A CALL THROUGH A BUILT-IN: `Array.prototype.map` calls the recursion back, so a level is the call
    # of the built-in and the built-in's call of the guest.
    "callback": (
        'var n = %d; function down() { if (n === 0) { @BASE@ } n = n - 1; return [0].map(down)[0]; }',
        "0", "down()"),

    # A CALL THROUGH A BOUND FUNCTION: `JsEngine.Call` meets the `JsBoundFunction` and calls its target,
    # two calls a level.
    "bound": (
        'var n = %d; var down = function () { if (n === 0) { @BASE@ } n = n - 1; return bound(); }; '
        'var bound = down.bind(null);', "0", "bound()"),

    # A CALL THROUGH A CALLABLE PROXY: `JsEngine.Call` -> `JsProxy.ProxyCall` -> the `apply` trap, two
    # calls a level.
    "proxyapply": (
        'var n = %d; var p = new Proxy(function () { }, { apply: function () { if (n === 0) { @BASE@ } '
        'n = n - 1; return p(); } });', "0", "p()"),

    # ITERATOR OPEN: `IterateStart` -> `GetIterator` -> a guest `Symbol.iterator` method, which recurses.
    "iterable": (
        'var n = %d; var o = { [Symbol.iterator]: function () { if (n === 0) { @BASE@ } n = n - 1; '
        'for (var x of o) { } return [][Symbol.iterator](); } };',
        "[][Symbol.iterator]()", "(function () { for (var x of o) { } return n; })()"),

    # ITERATOR CLOSE: a `break` -> `IterateClose` -> `CloseIterator` -> a guest `return`, which recurses.
    "iterclose": (
        'var n = %d; var o = { [Symbol.iterator]: function () { return this; }, '
        'next: function () { return { done: false, value: 0 }; }, '
        'return: function () { if (n === 0) { @BASE@ } n = n - 1; for (var x of o) { break; } '
        'return {}; } };',
        "{}", "(function () { for (var x of o) { break; } return n; })()"),

    # A STATIC ELEMENT: `RunStaticElements` -> `ApplyClassElements` -> a static block, which recurses.
    # A static field's initialiser is called from the same place.
    "staticblock": (
        'var n = %d; function f() { if (n === 0) { @BASE@ } n = n - 1; '
        'class C { static { this.x = f(); } } return C.x; }', "0", "f()"),
}


def family(setup, value, top):
    """The (returning, throwing) pair of one template.

    `render` is why the throwing form rethrows what it did not throw: its recursion is a throw, so a
    level's `catch` sees the bound's `RangeError` and the base case's `Error` alike, and only a level
    that passes on what is not its own leaves the top-level catch able to tell them apart.
    """
    returning = setup.replace("@BASE@", f"return {value};" if value else "return;")
    throwing = setup.replace("@BASE@", 'throw new Error("here");')

    return (
        returning + f'\nprint("answered " + ({top}));\n',
        throwing
        + f"\ntry {{ {top}; print(\"bounded no-throw\"); }}\n"
        + "catch (failure) {\n"
        + '  print(failure.message === "here" ? "answered " + failure.name : "bounded " + failure.name);\n'
        + "}\n")


SHAPES.update({name: family(*parts) for name, parts in TEMPLATES.items()})


# THE THREE OUTCOMES, AND WHY THEY ARE THREE RATHER THAN TWO. A run that COMPLETED reached the
# depth it was asked for; a run the engine's bound or the host's ceiling REFUSED did not reach it and
# is not a failure; a run that DIED is the defect this whole exercise exists to keep out. Folding
# the middle one into either of the others is what made an earlier version of this script report a
# per-frame cost derived from a bound rather than from the stack.
COMPLETED = "completed"
BOUNDED = "bounded"
BACKSTOP = "backstop"
DIED = "died"


def host_backend():
    """The x86-64 convention this host arms, or None where it arms none."""
    if platform.machine().lower() not in ("amd64", "x86_64", "x64"):
        return None

    return "x86-64-win64" if os.name == "nt" else "x86-64-sysv"


def git(tree, *arguments):
    """One git answer from that tree, or None where git could not answer."""
    try:
        done = subprocess.run(
            ["git", "-C", str(tree)] + list(arguments), capture_output=True, text=True, timeout=60)
    except (OSError, subprocess.TimeoutExpired):
        return None

    return done.stdout if done.returncode == 0 else None


def per_opcode_steps(tree):
    """The setting of `PerOpcodeSteps` in that tree, as `true`, `false` or `unknown`."""
    try:
        text = (tree / HANDLERS).read_text(encoding="utf-8")
    except OSError:
        return "unknown"

    for line in text.splitlines():
        if PER_OPCODE_STEPS not in line or "=" not in line:
            continue

        setting = line.split("=", 1)[1].strip().rstrip(";").strip()

        return setting if setting in ("true", "false") else "unknown"

    return "unknown"


def build_identity(named):
    """Which build the binary under measurement came from: its commit, its dirtiness, its constant.

    All three print `unknown` without `--source-tree`, rather than not printing at all: an absent
    line reads as an oversight in the script and a stated `unknown` reads as what it is, a log that
    cannot say which build it measured and that therefore no depth clause may be read against.
    """
    if named is None:
        return "unknown", "unknown", "unknown"

    tree = pathlib.Path(named)
    commit = git(tree, "rev-parse", "HEAD")
    status = git(tree, "status", "--porcelain")

    return (
        commit.strip() if commit else "unknown",
        ("yes" if status.strip() else "no") if status is not None else "unknown",
        per_opcode_steps(tree))


def outcome(binary, scratch, shape, depth, ceiling, timeout, form):
    """What the host did at this recursion depth."""
    source = scratch / "depth.js"
    source.write_text(shape % depth, encoding="utf-8")

    try:
        done = subprocess.run(
            [
                str(binary), str(source), "--quiet",
                "--call-depth", str(ceiling),
                "--fuel", "100000000000",
                "--wall", "600000",
            ] + form,
            capture_output=True, text=True, timeout=timeout,
        )
    except subprocess.TimeoutExpired:
        return DIED, "timed out"

    both = done.stdout + done.stderr

    if both.startswith("answered"):
        return COMPLETED, "answered"

    if both.startswith("bounded"):
        return BOUNDED, both.strip().splitlines()[0]

    if "Maximum call stack size exceeded" in both:
        return BOUNDED, "the engine's own bound"

    if "CeilingReached on CallDepth" in both:
        # THE GRANTED CEILING, OR THE STACK REFUSING IN ITS NAME. Both answer alike, and a
        # recursion shallower than the ceiling this run granted cannot have reached that ceiling,
        # so what refused was the stack.
        return (
            (BOUNDED, "the budget ceiling") if depth >= ceiling
            else (BACKSTOP, "the runtime's stack probe"))

    # A DIMENSION THIS RUN SPENT, WHICH IS A REFUSAL AND NOT A DEATH. The process is alive, it
    # answered, and it named what it ran out of. Reading either as a death is what printed a
    # termination notice about a process that was still running.
    if "AllowanceExhausted" in both:
        return BOUNDED, "an allowance this run spent"

    if "Stack overflow" in both or done.returncode < 0:
        return DIED, "the process terminated"

    return DIED, f"exit {done.returncode}: {both.strip().splitlines()[-1] if both.strip() else ''}"


def deepest(binary, scratch, shape, ceiling, timeout, label, form):
    """The deepest recursion of this shape that COMPLETES, and what stopped it going deeper."""
    low, high = 1, ceiling
    verdict, why = outcome(binary, scratch, shape, low, ceiling, timeout, form)

    if verdict != COMPLETED:
        print(f"# {label}: the shallowest recursion did not complete: {why}", file=sys.stderr)
        return None, verdict

    verdict, why = outcome(binary, scratch, shape, high, ceiling, timeout, form)

    if verdict == COMPLETED:
        print(f"# {label}: every depth up to {high} completed")
        return high, COMPLETED

    # INVARIANT: `low` completed and `high` did not. Every step keeps it, so the loop ends with
    # `low` the deepest recursion that completes and `high` the shallowest that does not.
    stopped = verdict

    while high - low > 1:
        middle = (low + high) // 2
        verdict, why = outcome(binary, scratch, shape, middle, ceiling, timeout, form)
        print(f"#   {label} {middle}: {verdict} ({why})")

        if verdict == COMPLETED:
            low = middle
        else:
            high = middle
            stopped = verdict

    return low, stopped


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--binary-directory", default=str(DEFAULT_BINARY_DIRECTORY))
    parser.add_argument("--stack-bytes", type=int, default=DEFAULT_STACK_BYTES)
    parser.add_argument("--ceiling", type=int, default=100000)
    parser.add_argument("--timeout", type=int, default=300)
    parser.add_argument("--form", choices=("bytecode", "native"), default="bytecode")
    parser.add_argument(
        "--backend", default=None, help="a native run's backend; defaults to this host's x86-64 convention")
    parser.add_argument(
        "--shape", choices=tuple(SHAPES), default="plain",
        help="the re-entry route to measure, one per route a chain of JavaScript levels can nest through")
    parser.add_argument(
        "--source-tree", default=None,
        help="the tree the binary was built from; without it the build identity lines print unknown")
    arguments = parser.parse_args()

    if arguments.backend and arguments.form != "native":
        parser.error("--backend names a native backend, and this run's form is bytecode")

    if arguments.source_tree and not pathlib.Path(arguments.source_tree).is_dir():
        parser.error(f"--source-tree names {arguments.source_tree}, which is not a directory")

    form = []
    backend = ""

    if arguments.form == "native":
        backend = arguments.backend or host_backend() or ""

        if not backend:
            print(
                "# this host's architecture arms no backend, so a native run must name one with --backend",
                file=sys.stderr)
            return 2

        form = ["--native", backend]

    binary = pathlib.Path(arguments.binary_directory) / "Broiler.VM.Composition.JavaScript.Cli"

    # The published image carries a suffix on Windows, appended rather than substituted for the
    # reason eng/run-test262.py records beside the same check.
    if not binary.exists() and binary.with_name(binary.name + ".exe").exists():
        binary = binary.with_name(binary.name + ".exe")

    if not binary.exists():
        print(f"# no binary at {binary}", file=sys.stderr)
        return 2

    commit, dirty, steps = build_identity(arguments.source_tree)
    returning_source, throwing_source = SHAPES[arguments.shape]

    print(f"# measuring against {binary}")
    print(f"# form {arguments.form}" + (f" ({backend})" if backend else ""))
    print(f"# shape {arguments.shape}")
    print(f"# source-commit {commit}")
    print(f"# source-dirty {dirty}")
    print(f"# per-opcode-steps {steps}")
    print(f"# declared guest stack {arguments.stack_bytes} bytes")

    with tempfile.TemporaryDirectory(prefix="broiler-depth-") as directory:
        scratch = pathlib.Path(directory)

        returning, why_returning = deepest(
            binary, scratch, returning_source, arguments.ceiling, arguments.timeout, "returning", form)

        throwing, why_throwing = deepest(
            binary, scratch, throwing_source, arguments.ceiling, arguments.timeout, "throwing", form)

    if returning is None or throwing is None:
        return 1

    print(f"form {arguments.form}" + (f" {backend}" if backend else ""))
    print(f"shape {arguments.shape}")
    print(f"deepest-returning-recursion {returning}")
    print(f"stopped-by-returning {why_returning}")
    print(f"deepest-throwing-recursion {throwing}")
    print(f"stopped-by-throwing {why_throwing}")
    print(f"declared-guest-stack-bytes {arguments.stack_bytes}")

    if DIED in (why_returning, why_throwing):
        print("# A RECURSION TERMINATED THE PROCESS, which is the outcome this bound exists against")
        return 1

    if why_returning == BOUNDED and why_throwing == BOUNDED:
        print(
            "# both were stopped by a declared bound and not by the stack, so this run reports what\n"
            "# the build PROMISES rather than what the stack holds. Lift `MaximumCallDepth` and the\n"
            "# profile's declared call-depth maximum in a build of your own to measure the capacity.")

        return 0

    if BOUNDED in (why_returning, why_throwing):
        print(
            "# ONE SHAPE MET A DECLARED BOUND AND THE OTHER MET THE STACK, so the two figures below\n"
            "# are not a comparison. Lift the bounds and run again before reading them as one.")

        return 1

    print(f"bytes-per-frame {arguments.stack_bytes / returning:.0f}")

    # THE TWO MUST AGREE, or a throw is costing stack a call is not. Reporting it rather than
    # asserting it is deliberate: this script measures and the acceptance table judges.
    if abs(returning - throwing) > max(64, returning // 20):
        print("# THE TWO DEPTHS DISAGREE, so an exception is costing stack a call is not")
        return 1

    return 0


if __name__ == "__main__":
    raise SystemExit(main())
