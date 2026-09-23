// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The other half of the dynamic cycle. Its body runs first.

import { a } from "./identity-cycle-a.mjs";

export let b;

try {
  b = "saw:" + a;
} catch (e) {
  b = "tdz:" + e.name;
}

export function readA() {
  return a;
}
