// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the-module-evaluation-errors.mjs: a body that throws after a top-level await.
globalThis.evaluationLateRuns = (globalThis.evaluationLateRuns || 0) + 1;
await 0;
throw new RangeError("late");
