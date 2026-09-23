// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the module-scoped eval probe. It exports a mutable binding and the function that
// changes it, a constant, and a default function that reports its receiver.

export let live = 1;

export function bumpLive() {
  live = live + 1;
}

export const fixed = "f";

export default function () {
  return this === undefined;
}
