// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// An unrelated async sibling.

globalThis.siblingsLog.push("good start");
await 0;
await 0;
await 0;
globalThis.siblingsLog.push("good end");
