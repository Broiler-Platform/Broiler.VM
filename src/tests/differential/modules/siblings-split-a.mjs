// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The root of a cycle that waits on the slow module.

import "./siblings-split-b.mjs";
import "./siblings-split-slow.mjs";

globalThis.siblingsLog.push("A run");
