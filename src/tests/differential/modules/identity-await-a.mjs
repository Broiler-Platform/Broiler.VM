// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The root of a cycle that awaits: an import() of the other member, started while this module is
// suspended, must wait for this module (the cycle's root) to finish.

import "./identity-await-b.mjs";

globalThis.identityAwaitLog.push("A start");

await new Promise(function (resolve) { globalThis.identityReleaseA = resolve; });

globalThis.identityAwaitLog.push("A end");
