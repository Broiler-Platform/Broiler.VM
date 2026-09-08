// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// BRANCH-CHAIN - a loop whose body is mostly deciding which arm to take.
//
// The manifest admits no `&&`, no `||` and no conditional expression, so a chain of `if`/`else`
// is the only shape a multi-way decision has here; that is a constraint on the kernel and it is
// also what makes it useful, because every arm is a real branch rather than an operator that
// might be folded. The counter cycles through eight values, so the arms are taken in a fixed
// repeating pattern that neither form can be accused of having got lucky with.
//
// The answer is 750000 - ten per eight-iteration cycle, seventy-five thousand cycles.

function branchy(n) {
  let acc = 0;
  let i = 0;
  let t = 0;

  while (i < n) {
    t = t + 1;

    if (t > 7) {
      t = 0;
    }

    if (t < 2) {
      acc = acc + 1;
    } else {
      if (t < 4) {
        acc = acc + 2;
      } else {
        if (t < 6) {
          acc = acc - 1;
        } else {
          acc = acc + 3;
        }
      }
    }

    i = i + 1;
  }

  return acc;
}

branchy(600000);
