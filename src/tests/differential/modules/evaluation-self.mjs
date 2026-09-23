// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the-module-evaluation-errors.mjs: imports itself while its body is running.
export const early = 1;
globalThis.evaluationSelf = import("./evaluation-self.mjs").then(function (ns) { return ns.late; });
export const late = 2;
