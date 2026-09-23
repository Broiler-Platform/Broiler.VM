// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// An async parent: it starts once its dependency is done, and suspends again.

import "./siblings-mixed-t.mjs";

globalThis.siblingsLog.push("m1 start");
await 0;
globalThis.siblingsLog.push("m1 end");
