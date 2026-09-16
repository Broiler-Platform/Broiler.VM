// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// FIB - recursion, where the call IS the work.
//
// Naive Fibonacci of 32 makes 7,049,155 calls and does almost nothing between them, so what is
// measured is entering a frame, comparing one argument and returning a number. It is the shape
// `src/tests/forms/call-recursive.js` uses for the numeric form, and it is here for the same
// reason: a change in how a call is made shows here and nowhere else in this set. The call count is
// exact rather than observed - a naive fib(n) makes 2*Fib(n+1)-1 calls - and it is a property of the
// program rather than a measurement of anything.
//
// The literal is fixed here and is NOT chosen by a pilot run: the README beside this file states
// the charged fuel one iteration costs, the count, and the argument for the count.

function fib(n) {
  return n < 2 ? n : fib(n - 1) + fib(n - 2);
}

fib(32);
