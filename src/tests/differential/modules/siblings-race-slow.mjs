// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// An async module that resumes after three turns.

globalThis.siblingsLog.push("slow start");
await 0;
await 0;
await 0;
globalThis.siblingsLog.push("slow end");
