// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the module-identity probe, imported statically AND dynamically. It counts its
// own evaluations on the global object, so a second evaluation is a visible number.

globalThis.identityDepRuns = (globalThis.identityDepRuns || 0) + 1;

export let count = 0;

export function bump() {
  count++;
}
