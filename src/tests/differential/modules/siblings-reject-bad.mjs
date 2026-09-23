// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// An async module that rejects after its first await.

globalThis.siblingsLog.push("bad start");
await 0;
throw new RangeError("bad");
