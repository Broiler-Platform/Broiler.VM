// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// An async module that resumes after one turn.

globalThis.siblingsLog.push("fast start");
await 0;
globalThis.siblingsLog.push("fast end");
