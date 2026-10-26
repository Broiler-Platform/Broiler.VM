/*---
description: SJL-13.1 R value probes
flags: [raw]
---*/
var __vmTest = true;
// SJL-13.1 task 1.1, sub-area R (copied from sub-area T): the value probes' shared prelude (SJL-11.1's form,
// Broiler.VM docs/evidence/sjl-11-001/correspondence-runs/vm/probes/prelude.js, cut to what these
// probes use). Broiler.JS's shell has print; Broiler.VM's test262 realm has none, so there the lines
// are collected and handed back in one uncaught Error the conformance root prints (epilogue.js).
var __lines = [];
var __host = typeof __vmTest === 'undefined' && typeof print === 'function';
function out(s) { s = String(s); if (__host) print(s); else __lines.push(s); }
// A string is printed quoted, every code unit outside printable ASCII (and " and \) as \uXXXX.
function q(s) {
  var r = '';
  for (var i = 0; i < s.length; i++) {
    var c = s.charCodeAt(i);
    if (c < 0x20 || c > 0x7e || c === 0x22 || c === 0x5c) r += '\\u' + ('0000' + c.toString(16)).slice(-4);
    else r += s.charAt(i);
  }
  return '"' + r + '"';
}
function plain(s) {
  var r = '';
  for (var i = 0; i < s.length; i++) {
    var c = s.charCodeAt(i);
    r += c < 0x20 || c > 0x7e ? '\\u' + ('0000' + c.toString(16)).slice(-4) : s.charAt(i);
  }
  return r;
}
function show(v) {
  if (v === null) return 'null';
  if (v === undefined) return 'undefined';
  if (typeof v === 'string') return q(v);
  if (typeof v === 'number') return Object.is(v, -0) ? '-0' : String(v);
  return typeof v + ' ' + String(v);
}
function errName(e) { return (e && e.constructor && e.constructor.name) || typeof e; }
function t(label, fn) {
  var r;
  try { r = show(fn()); }
  catch (e) { r = 'THROWS ' + errName(e) + ': ' + plain(String(e && e.message)); }
  out(label + ' => ' + r);
}
// Every source under test is compiled by an indirect eval (script goal, sloppy, global), so that a
// source one engine refuses cannot stop the probe file itself from compiling.
function ev(src) { return (0, eval)(src); }
// SJL-13.1 task 1.1, sub-area R: value probes of the inherited context of eval code (rows R03, R22 to R28, R30, R36).
// Run as one program prelude.js + r-values.js + epilogue.js (SJL-11.1's form, as sub-area T's value probes are).
// Each line prints `<label> => <value>` or `<label> => THROWS <name>: <message>`. Every eval below is a DIRECT
// eval unless the label says indirect, so that the caller's context is what the engine passes to the parse.

// ---- R03: strictness inherited from the caller --------------------------------------------------------------
t('RE01 strict caller: var public', function () { return (function () { 'use strict'; return eval('var public = 1; public'); })(); });
t('RE02 strict caller: legacy octal 010', function () { return (function () { 'use strict'; return eval('010'); })(); });
t('RE03 strict caller: with', function () { return (function () { 'use strict'; return eval('with ({}) {} 1'); })(); });
t('RE04 sloppy caller control', function () { return (function () { return eval('var public = 1; with ({}) {} public + 010'); })(); });
t('RE05 strict caller: function public() {}', function () { return (function () { 'use strict'; return eval('function public() {} 1'); })(); });

// ---- R22: the function context (new.target) -------------------------------------------------------------------
var __r;
try { __r = show(eval('new.target')); } catch (e) { __r = 'THROWS ' + errName(e) + ': ' + plain(String(e && e.message)); }
out('RE06 global code: eval new.target => ' + __r);
try { __r = show((() => eval('new.target'))()); } catch (e) { __r = 'THROWS ' + errName(e) + ': ' + plain(String(e && e.message)); }
out('RE07 global code: arrow, eval new.target => ' + __r);
t('RE08 function: eval new.target', function () { return (function () { return eval('new.target'); })(); });
t('RE09 constructed function: eval new.target === F', function () { function F() { this.v = eval('new.target === F'); } return new F().v; });
t('RE10 arrow in a function: eval new.target', function () { return (function () { return (() => eval('new.target'))(); })(); });
t('RE11 function: indirect eval new.target', function () { return (function () { return (0, eval)('new.target'); })(); });
t('RE12 function: nested direct eval new.target', function () { return (function () { return eval("eval('new.target')"); })(); });

// ---- R23: the method context (super property) -----------------------------------------------------------------
t('RE13 method: eval super property', function () { return ({ m() { return eval('super.toString === Object.prototype.toString'); } }).m(); });
t('RE14 plain function: eval super property', function () { return (function () { return eval('super.x'); })(); });
t('RE15 arrow in a method: eval super property', function () { return ({ m() { return (() => eval('typeof super.hasOwnProperty'))(); } }).m(); });
t('RE16 getter: eval super property', function () { return ({ get g() { return eval('typeof super.hasOwnProperty'); } }).g; });

// ---- R24: the derived-constructor context (super call) --------------------------------------------------------
t('RE17 derived constructor: eval super()', function () { class B { constructor() { this.b = 1; } } class D extends B { constructor() { eval('super()'); } } return new D().b; });
t('RE18 arrow in a derived constructor: eval super()', function () { class B { constructor() { this.b = 1; } } class D extends B { constructor() { (() => eval('super()'))(); } } return new D().b; });
t('RE19 base-class constructor: eval super()', function () { class C { constructor() { eval('super()'); } } return new C(); });
t('RE20 derived class field initialiser: eval super()', function () { class B {} class D extends B { x = eval('super()'); } return new D(); });
t('RE21 object method: eval super()', function () { return ({ m() { return eval('super()'); } }).m(); });

// ---- R25: the field-initialiser context (arguments) -----------------------------------------------------------
t('RE22 field initialiser: eval arguments', function () { class C { x = eval('arguments'); } return new C().x; });
t('RE23 field initialiser: eval an arrow reading arguments', function () { class C { x = eval('() => arguments'); } return typeof new C().x; });
t('RE24 field initialiser: eval a function reading its own arguments', function () { class C { x = eval('(function () { return arguments.length; })(1, 2)'); } return new C().x; });
t('RE25 method: eval arguments', function () { return ({ m(a) { return eval('arguments.length'); } }).m(1, 2, 3); });
t('RE26 static field initialiser: eval arguments', function () { class C { static x = eval('arguments'); } return C.x; });

// ---- R26: the class static block (no flag of its own in either engine; a method body for PerformEval) ---------
t('RE27 static block: eval new.target', function () { class C { static { this.v = eval('new.target'); } } return C.v; });
t('RE28 static block: eval super property', function () { class C { static { this.v = eval('typeof super.constructor'); } } return C.v; });
t('RE29 static block: eval arguments', function () { class C { static { this.v = eval('typeof arguments'); } } return C.v; });

// ---- R27: the visible private names --------------------------------------------------------------------------
t('RE30 method: eval this.#p (declared)', function () { class C { #p = 7; m() { return eval('this.#p'); } } return new C().m(); });
t('RE31 method: eval this.#q (not declared)', function () { class C { #p = 7; m() { return eval('this.#q'); } } return new C().m(); });
t('RE32 no class: eval this.#p', function () { return (function () { return eval('this.#p'); })(); });
t('RE33 static method: eval #p in o', function () { class C { #p; static t(o) { return eval('#p in o'); } } return C.t(new C()); });
t('RE34 nested class: eval reads the outer class private name', function () { class O { #o = 1; m() { class I { n(x) { return eval('x.#o'); } } return new I().n(this); } } return new O().m(); });

// ---- R28: the caller's lexical names (EvalDeclarationInstantiation) ------------------------------------------
t('RE35 function let x, sloppy eval var x', function () { return (function () { let x = 1; eval('var x = 2'); return x; })(); });
t('RE36 outer block let y, inner block sloppy eval var y', function () { return (function () { { let y = 1; { eval('var y = 2'); } } return 1; })(); });
t('RE37 strict caller: let x, eval var x', function () { return (function () { 'use strict'; let x = 1; eval('var x = 2'); return x; })(); });
t('RE38 catch parameter e, sloppy eval var e', function () { return (function () { try { throw 1; } catch (e) { eval('var e = 2'); return e; } })(); });

// ---- R30: the dynamic function inherits nothing --------------------------------------------------------------
t('RE39 strict caller: Function body with and 010', function () { return (function () { 'use strict'; return new Function('with ({}) {} return 010;')(); })(); });
t('RE40 Function body: new.target', function () { return new Function('return new.target')(); });
t('RE41 Function body: super property', function () { return typeof new Function('return super.x'); });
t('RE42 Function body in a class method: o.#p', function () { class C { #p = 1; m() { return new Function('o', 'return o.#p'); } } return typeof new C().m(); });

// ---- R36: a strict caller's eval and Broiler.JS's strict-reserved-word token re-scan (GAP step, critic C13) ----
t('RE43 strict caller: eval a class field named static', function () { return (function () { 'use strict'; return eval('class C { static = 1 }; new C().static'); })(); });
t('RE44 strict caller: eval a class field named public', function () { return (function () { 'use strict'; return eval('class C { public = 2 }; new C().public'); })(); });
t('RE45 sloppy caller control: eval a class field named static', function () { return (function () { return eval('class C { static = 1 }; new C().static'); })(); });
t('RE46 strict caller control: eval o.public = 3 (after a dot)', function () { return (function () { 'use strict'; return eval('var o = {}; o.public = 3; o.public'); })(); });
t('RE47 strict caller: eval a static field named public', function () { return (function () { 'use strict'; return eval('class C { static public = 4 }; C.public'); })(); });
t('RE48 strict caller: eval assigns to the name public', function () { return (function () { 'use strict'; return eval('public = 5'); })(); });
if (!__host) throw new Error('OUT<<' + __lines.join('\n') + '>>OUT');
