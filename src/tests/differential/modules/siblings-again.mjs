// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// An async module imported twice while it is suspended: its body runs once.

globalThis.siblingsLog.push("again start");
await 0;
await 0;
globalThis.siblingsLog.push("again end");
