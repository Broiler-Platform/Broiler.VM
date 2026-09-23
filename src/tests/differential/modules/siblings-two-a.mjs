// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// An async sibling that resumes after one turn.

globalThis.siblingsLog.push("a start");
await 0;
globalThis.siblingsLog.push("a end");
