// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// CALL-RECURSIVE - what a call costs when the call is the work.
//
// Naive Fibonacci is here for one property and not for its arithmetic: fib(25) makes 242,785
// calls and does almost nothing between them, so nearly all of what either output form spends is
// entering a frame, comparing one argument, and returning a double. A form that changed how a
// call is made shows here and nowhere else in this set.
//
// The answer is 75025, and both forms must print it before either is timed.

function fib(n) {
  if (n < 2) {
    return n;
  }

  return fib(n - 1) + fib(n - 2);
}

fib(25);
