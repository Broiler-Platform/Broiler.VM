// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The other half of the cycle, and the half that runs FIRST. A hoisted function of the other half
// is callable from here even though that half's body has not run; a `let` of it is not, and asking
// for one is the temporal dead zone crossing a module boundary.

import { fromFirst, late } from "./cycle-first.mjs";

export function fromSecond() {
  return fromFirst() + "-then-second";
}

export const soon = "second-ready";

export const deadZone = (function () {
  try {
    return String(late);
  } catch (error) {
    return error.name;
  }
})();
