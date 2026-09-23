// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// An async module that throws before it awaits.

globalThis.siblingsLog.push("bad");
if (globalThis.siblingsLog) { throw new SyntaxError("early"); }
await 0;
