// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The member of the failing cycle. Its body runs before its root.

import "./identity-rootfail-a.mjs";

globalThis.identityRootFailLog.push("B run");
