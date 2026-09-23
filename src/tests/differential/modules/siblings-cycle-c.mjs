// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A module outside the cycle that depends on its member.

import "./siblings-cycle-b.mjs";

globalThis.siblingsLog.push("C");
