// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The root of a cycle that also needs a module another import is still evaluating. Its cycle
// member runs first; this body waits for the slow module, so an import() of the member started in
// between must wait for this module (the cycle's root) too.

import "./identity-split-b.mjs";
import "./identity-split-slow.mjs";

globalThis.identitySplitLog.push("A run");
