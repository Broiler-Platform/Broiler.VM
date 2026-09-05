// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The three re-export forms, so the probe can read a name it never imported directly: a star, a
// renamed indirect export, and a default published under a name of its own.

export * from "./counter.mjs";
export { counter as tally } from "./counter.mjs";
export { default as forty } from "./counter.mjs";
