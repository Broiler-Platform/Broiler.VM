// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// An async sibling that resumes after two turns.

globalThis.siblingsLog.push("b start");
await 0;
await 0;
globalThis.siblingsLog.push("b end");
