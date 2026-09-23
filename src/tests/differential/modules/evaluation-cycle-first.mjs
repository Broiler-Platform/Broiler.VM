// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the-module-evaluation-errors.mjs: one half of a cycle, and the half that throws.
import "./evaluation-cycle-second.mjs";
throw new Error("cycle");
