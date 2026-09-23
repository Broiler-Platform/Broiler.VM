// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The root of an awaiting cycle.

import "./siblings-cycle-b.mjs";

globalThis.siblingsLog.push("A start");
await new Promise(function (resolve) { globalThis.siblingsRelease = resolve; });
globalThis.siblingsLog.push("A end");
