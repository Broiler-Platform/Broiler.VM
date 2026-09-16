// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// FIB (SMALL TWIN) - recursion, where the call IS the work.
//
// The same program with a smaller literal. It is the twin the fuel-parity condition bisects: the
// smallest allowance that completes it must be ONE figure across both arms and both forms, and one
// less must exhaust all four. Nothing here is timed.

function fib(n) {
  return n < 2 ? n : fib(n - 1) + fib(n - 2);
}

fib(12);
