// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the-module-evaluation-errors.mjs: imported twice at once, it rejects after
// awaiting. Both imports must reject with the one value, and the body must run once.
globalThis.evaluationConcurrentRuns = (globalThis.evaluationConcurrentRuns || 0) + 1;
await 0;
throw new Error("concurrent");
