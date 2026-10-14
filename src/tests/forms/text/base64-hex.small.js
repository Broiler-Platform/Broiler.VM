// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// BASE64-HEX - Uint8Array's base64 and hex codecs: both alphabets, padding kept and omitted, every
// last-chunk mode on input that decodes under it, white space in the input, and setFromBase64 and
// setFromHex into destinations shorter than the input, which write part of it and report what
// they read and wrote. No input here is refused.
// A fuel-parity probe of milestone SJL-5 (README beside it). Nothing here is timed.

var n = 0;
var modes = ["loose", "strict", "stop-before-partial"];

for (var i = 0; i < 60; i++) {
  var bytes = new Uint8Array(i % 17 + 1);
  for (var k = 0; k < bytes.length; k++) {
    bytes[k] = (i * 31 + k * 7) & 255;
  }

  var b64 = bytes.toBase64();
  var url = bytes.toBase64({ alphabet: "base64url", omitPadding: true });
  var hex = bytes.toHex();
  n += b64.length + url.length * 3 + hex.length * 5;

  var back = Uint8Array.fromBase64(" " + b64 + "\n", { lastChunkHandling: modes[i % 2] });
  var back2 = Uint8Array.fromBase64(url, { alphabet: "base64url", lastChunkHandling: modes[(i % 2) * 2] });
  var back3 = Uint8Array.fromHex(hex);
  n += back.length + back2.length * 7 + back3[0];

  var shortBase64 = new Uint8Array(i % 5 + 1);
  var r = shortBase64.setFromBase64(b64);
  n += r.read * 11 + r.written * 13 + shortBase64[0];

  var shortHex = new Uint8Array(i % 3 + 1);
  var s = shortHex.setFromHex(hex + "ZZ".repeat(i % 2 && bytes.length > shortHex.length ? 1 : 0));
  n += s.read * 17 + s.written * 19;
}

n;
