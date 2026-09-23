// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the-module-evaluation-errors.mjs: an async body that throws before its await.
globalThis.evaluationEarlyRuns = (globalThis.evaluationEarlyRuns || 0) + 1;
if (globalThis.evaluationEarlyRuns > 0) {
  throw new SyntaxError("early");
}
await 0;
