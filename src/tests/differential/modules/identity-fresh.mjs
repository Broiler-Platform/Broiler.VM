// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the module-identity probe reached ONLY by dynamic imports, two of them started
// in the same turn: one module, one evaluation, one namespace.

globalThis.identityFreshRuns = (globalThis.identityFreshRuns || 0) + 1;

export const fresh = "fresh";
