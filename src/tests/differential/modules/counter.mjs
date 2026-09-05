// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the module-goal probe. It exports a mutable binding, a function that changes it,
// a default, and a name published under a string that is not an identifier.

export let counter = 0;

export function bump() {
  counter = counter + 1;
}

export const frozen = "held";

const punctuated = "string-name";
export { punctuated as "a name with spaces" };

export default 40;
