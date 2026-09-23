// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER A DIRECT EVAL AT THE TOP LEVEL OF A STRICT SCRIPT (JSeal slice V15,
// JSD-0026 step 8).
//
// The evaluation inherits the caller's strictness, so its `var` and function declarations stay in
// its own variable environment and a `with` statement is a SyntaxError; an indirect eval from the
// same script is sloppy global code. Each case prints its own number so a divergence names a case.
"use strict";
var __n = 0;
function show(v) { return typeof v === "string" ? JSON.stringify(v) : String(v); }
function top(r) { __n++; print(__n + " " + r); }

var r;
try { eval("var sv = 1"); r = typeof sv; } catch (e) { r = e.name; } top(r);
try { eval("with ({}) { }"); r = "no error"; } catch (e) { r = e.name; } top(r);
try { eval("function sf() { }"); r = typeof sf; } catch (e) { r = e.name; } top(r);
try { r = show(eval("let sl = 1; sl")); } catch (e) { r = e.name; } top(r);
try { (0, eval)("var sind = 1"); r = typeof sind; } catch (e) { r = e.name; } top(r);
try { eval("undeclaredInStrictEval = 1"); r = "no error"; } catch (e) { r = e.name; } top(r);
try { r = show(eval("var inner = 2; eval('inner + 1')")); } catch (e) { r = e.name; } top(r);
try { eval("var arguments"); r = "no error"; } catch (e) { r = e.name; } top(r);
