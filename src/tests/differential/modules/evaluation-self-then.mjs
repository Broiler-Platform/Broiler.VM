// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the-module-evaluation-errors.mjs: an async body whose last statement's value is a
// promise that waits on an import of the module itself. The evaluation must still finish.
await 0;
globalThis.evaluationSelfThen = import("./evaluation-self-then.mjs").then(function () { return "settled"; });
