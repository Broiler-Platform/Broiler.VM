// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the-module-evaluation-errors.mjs: imported twice at once, it binds after two awaits.
await 0;
await 0;
export let value = 1;
