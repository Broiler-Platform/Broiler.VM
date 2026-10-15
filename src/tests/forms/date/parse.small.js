// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// PARSE - Date.parse and the Date constructor over strings: the Date Time String Format in each of its
// shapes - a year, a month, a date, a time with minutes, seconds and exactly three fraction digits,
// with Z, with an offset and with none - and expanded years of both signs; and this family's own
// toString, toUTCString and toISOString of the same instants read back. Every string is a valid
// instance of the format or one of those renderings, and every day exists.
// A fuel-parity probe of milestone SJL-6 (README beside it). Nothing here is timed.

var n = 0;
var pad = function (v, w) { var s = String(v); while (s.length < w) s = "0" + s; return s; };
var offsets = ["Z", "+00:00", "+05:30", "-08:00", "+14:00", "-12:00", ""];

for (var i = 0; i < 70; i++) {
  var y = 1900 + i * 3;
  var mo = pad(i % 12 + 1, 2);
  var da = pad(i % 28 + 1, 2);
  var hh = pad(i % 24, 2);
  var mi = pad(i * 7 % 60, 2);
  var ss = pad(i * 11 % 60, 2);
  var ms = pad(i * 13 % 1000, 3);
  var date = y + "-" + mo + "-" + da;
  n += Date.parse(String(y)) / 1e8 + Date.parse(y + "-" + mo) / 1e8 + Date.parse(date) / 1e8;
  var off = offsets[i % offsets.length];
  n += Date.parse(date + "T" + hh + ":" + mi + off) / 1e7;
  n += Date.parse(date + "T" + hh + ":" + mi + ":" + ss + off) / 1e7;
  n += new Date(date + "T" + hh + ":" + mi + ":" + ss + "." + ms + off).getTime() / 1e7;
  var sign = i % 2 ? "+" : "-";
  n += Date.parse(sign + pad(i * 1234 % 270000 + 1, 6) + "-" + mo + "-" + da + "T" + hh + ":" + mi + "Z") / 1e9;

  var d = new Date(Date.UTC(y * 3 - 4000, i % 12, i % 28 + 1, i % 24, i % 60, i % 60, i));
  n += Date.parse(d.toString()) / 1e9 + Date.parse(d.toUTCString()) / 1e9 + Date.parse(d.toISOString()) / 1e9;
  n += new Date(d.toString()).getUTCDay() + new Date(d.toISOString()).getUTCMilliseconds();
}

Math.round(n);
