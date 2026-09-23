// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A DIFFERENTIAL PROBE OVER THE BIGINT LITERAL BOUNDARY OF THE WIDE MANIFEST, retained from JSeal
// slices B01-B02 (decision JSD-0033). The wide manifest still refuses a BigInt literal by name - the
// value kind is internal and gated until card B05 - so what this probe can ask of the end-user host
// is what that boundary must hold: every malformed spelling is the language's SyntaxError whatever
// the manifest admits, a BigInt used as a property key is its exact decimal spelling, and a
// well-formed literal is refused rather than evaluated as a Number. The gated path itself is judged
// by the slice compiler's `--checks` rows `bigint/...`, which the end-user host cannot open.
// (Amended 2026-09-21, JSeal B05: the wide manifest admits BigInt through `broiler.javascript.bigint`
// now, so a well-formed literal is evaluated exactly - case 16 answers "bigint" as the comparison
// engine does - and the malformed spellings stay SyntaxErrors.)
var __n = 0;
function t(f) { try { var v = f(); return typeof v === "string" ? JSON.stringify(v) : String(v); } catch (e) { return e.name; } }
function p(f) { __n++; print(__n + " " + t(f)); }

// 1-10: malformed spellings are syntax errors, with or without BigInt. These are regression guards
// only: the base build already answered SyntaxError for all ten, because eval reports the old
// 2104 refusal as a SyntaxError too. The move from 2104 to 2003 at the literal is judged by the
// slice compiler's `bigint/b02/gated/...` and `bigint/b02/ungated/...` rows and by Test262, not here.
p(function () { return eval("1.5n"); });
p(function () { return eval("1e3n"); });
p(function () { return eval("01n"); });
p(function () { return eval("08n"); });
p(function () { return eval(".5n"); });
p(function () { return eval("1_n"); });
p(function () { return eval("0x_1n"); });
p(function () { return eval("1nn"); });
p(function () { return eval("1n0"); });
p(function () { return eval("0b2n"); });

// 11-15: a BigInt property key names the property its exact decimal spelling names.
p(function () { return Object.keys({ 9007199254740993n: 1 })[0]; });
p(function () { return Object.keys({ 0x10n: 1, 0o7n: 2, 0b11n: 3 }).join("|"); });
p(function () { return Object.keys({ 1_000_000_000_000_000_000_001n: 1 })[0]; });
p(function () { var { 18446744073709551617n: v } = { "18446744073709551617": "exact" }; return v; });
p(function () { return Object.getOwnPropertyNames(class { static 0x20n() {} }).indexOf("32") >= 0; });

// 16: a well-formed literal is refused by the wide manifest, never evaluated as a Number.
p(function () { return typeof eval("9007199254740993n"); });
