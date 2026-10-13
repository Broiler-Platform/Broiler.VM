// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// INTEGER-CONVERSIONS - every typed array's element conversion (ToInt8 to ToUint32, the clamp, the
// 64-bit BigInt bit patterns, Float16), DataView's, Atomics' operands, and ToIntegerOrInfinity,
// ToLength and ToIndex through the built-ins that apply them.
// A fuel-parity probe of milestone SJL-4 (README beside it). Nothing here is timed.

var i8 = new Int8Array(64), u8 = new Uint8Array(64), c8 = new Uint8ClampedArray(64);
var i16 = new Int16Array(64), u16 = new Uint16Array(64), i32 = new Int32Array(64);
var u32 = new Uint32Array(64), f16 = new Float16Array(64);
var b64 = new BigInt64Array(16), ub64 = new BigUint64Array(16);
var dv = new DataView(new ArrayBuffer(256));
var t = 0;

for (var k = 0; k < 64; k++) {
  var v = (k - 32) * 1e9 / 7 + k / 4;
  i8[k] = v; u8[k] = v; c8[k] = v / 1e7; i16[k] = v; u16[k] = v; i32[k] = v; u32[k] = v;
  f16[k] = v / 1e8;
  dv.setInt16(k * 2, v);
  dv.setUint32((k % 60) * 4, v);
  t += i8[k] + u8[k] + c8[k] + i16[k] + u16[k] + (i32[k] >>> 0) % 1000 + u32[k] % 1000 + f16[k];
  t += dv.getInt16(k * 2) + dv.getUint32((k % 60) * 4) % 1000;
  t += Atomics.add(i32, k, 2 ** 31 + k) % 1000;
}

for (var m = 0; m < 16; m++) {
  b64[m] = (2n ** 70n) * BigInt(m) - 5n;
  ub64[m] = -BigInt(m);
  t += Number(b64[m] % 1000n) + Number(ub64[m] % 1000n);
}

var a = [];
a.length = 3;
t += a.length + [1, 2, 3].indexOf(3, -Infinity) + "abcdef".slice(-2.5, Infinity).length;
t += new ArrayBuffer(7.9).byteLength + new DataView(new ArrayBuffer(16), 4.5).byteOffset;

t;
