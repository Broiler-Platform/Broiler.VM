// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the-module-evaluation-errors.mjs: a module that throws nothing.
globalThis.evaluationSiblingRuns = (globalThis.evaluationSiblingRuns || 0) + 1;
export const sibling = 2;
