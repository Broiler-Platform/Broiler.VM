// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// FLOAT-MIX - multiplication, division and accumulation over values that are not integers.
//
// This kernel exists to be the one that could disagree. Every other kernel here computes integers
// that a double holds exactly, so a backend that reassociated or contracted an expression would
// still answer the same number; this one accumulates 700,000 inexact terms, where reordering two
// additions changes the last bits and reordering enough of them changes a printed digit. The
// equivalence check in `eng/compare-forms.py` compares the two forms' printed answers exactly, so
// a backend that took an unsound liberty is caught here before any timing is reported.
//
// The answer is 181125.0087535767 and the two forms have agreed on every digit of it.

function mix(n) {
  let x = 1.0000001;
  let acc = 0.0;
  let i = 0;

  while (i < n) {
    acc = acc + x * 0.5 - x / 4.0;
    x = x + 0.0000001;
    i = i + 1;
  }

  return acc;
}

mix(700000);
