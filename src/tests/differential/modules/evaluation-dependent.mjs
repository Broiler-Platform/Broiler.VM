// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the-module-evaluation-errors.mjs: a module whose dependency throws.
import "./evaluation-throws.mjs";
globalThis.evaluationDependentRan = true;
export const value = 1;
