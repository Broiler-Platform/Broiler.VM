// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// One half of a cycle. Its body runs SECOND - the other half is requested first and evaluated
// first - which is what makes `late` readable here and `soon` of the other half not yet readable
// when the other half asks for this one's.

import { fromSecond, soon } from "./cycle-second.mjs";

export function fromFirst() {
  return "first";
}

export let late = "assigned";

export const observed = fromSecond() + "/" + soon;
