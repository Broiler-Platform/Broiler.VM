// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// TIME-ARITHMETIC - the time-value arithmetic: Date.UTC and the constructor from components across
// the representable range, before 1970, around leap days and at the range's ends; every getter, local
// and UTC; and the setters with integral arguments, which recompose a time value through MakeTime,
// MakeDay, MakeDate and TimeClip. No argument is fractional, non-finite or out of range, no year falls
// from 0 to 99, which MakeFullYear reads as a year of the 1900s, and every time value is valid.
// A fuel-parity probe of milestone SJL-6 (README beside it). Nothing here is timed.

var n = 0;
var years = [-271820, -100000, -1001, -1, 100, 1582, 1600, 1700, 1900, 1969, 1970, 1972, 2000, 2024,
  2100, 9999, 10000, 275759];

for (var i = 0; i < years.length; i++) {
  for (var m = 0; m < 12; m++) {
    var day = (i * 7 + m * 3) % 28 + 1;
    var t = Date.UTC(years[i], m, day, (i + m) % 24, (i * m) % 60, (i + 2 * m) % 60, (i * 37 + m) % 1000);
    var d = new Date(t);
    n += d.getUTCFullYear() * 3 + d.getUTCMonth() * 5 + d.getUTCDate() * 7 + d.getUTCDay() * 11;
    n += d.getUTCHours() + d.getUTCMinutes() * 13 + d.getUTCSeconds() * 17 + d.getUTCMilliseconds();
    n += d.getFullYear() + d.getMonth() + d.getDate() + d.getDay() + d.getHours() + d.getMinutes();
    n += d.getSeconds() + d.getMilliseconds() + d.getTimezoneOffset();
    n += new Date(years[i], m, day, 12, 30, 15, 250).getTime() % 1000003;

    var e = new Date(t);
    e.setUTCDate((day + 13) % 28 + 1);
    e.setUTCHours(23, 59, 58, 999);
    e.setUTCMonth((m + 5) % 12, 1);
    e.setUTCMinutes(1, 2, 3);
    e.setUTCSeconds(4, 5);
    e.setUTCMilliseconds(6);
    n += e.getTime() % 999983;
    e.setFullYear(years[(i + 1) % years.length] < 275759 ? years[(i + 1) % years.length] : 2024, 1, 29);
    e.setHours(1, 2);
    e.setMonth(6);
    e.setDate(31);
    e.setMinutes(7);
    e.setSeconds(8);
    e.setMilliseconds(9);
    n += e.getTime() % 999979 + e.getDay();
    e.setTime(t + 86400000 * (m - 6));
    n += e.getUTCDate();
  }
}

n += new Date(8.64e15).getUTCFullYear() + new Date(-8.64e15).getUTCDay() + Date.UTC(1970, 0, 1);
n;
