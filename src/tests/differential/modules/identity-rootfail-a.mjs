// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The root of a cycle that waits on another import's module and then throws. Its member has
// already run; every import of the member, or of a module that depends on the member, fails with
// this module's error.

import "./identity-rootfail-b.mjs";
import "./identity-rootfail-slow.mjs";

globalThis.identityRootFailLog.push("A run");
throw new Error("root boom");
