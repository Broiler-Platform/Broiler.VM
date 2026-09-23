// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A module outside the cycle that depends on its member. Its body must never run.

import "./identity-rootfail-b.mjs";

globalThis.identityRootFailLog.push("C run");
