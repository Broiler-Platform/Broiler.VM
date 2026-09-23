// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the-module-evaluation-errors.mjs: depends on a module another import is still evaluating.
import { awaited } from "./evaluation-awaited.mjs";
export const dependent = awaited + 1;
