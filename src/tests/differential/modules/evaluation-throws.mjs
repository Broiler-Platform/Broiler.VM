// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the-module-evaluation-errors.mjs: a body that counts its runs and then throws.
globalThis.evaluationThrowsRuns = (globalThis.evaluationThrowsRuns || 0) + 1;
throw { tag: "boom" };
