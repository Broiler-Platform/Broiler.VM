// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A rejected async module fails both of its parents; an unrelated async sibling still finishes.

import "./siblings-reject-good.mjs";
import "./siblings-reject-r1.mjs";
import "./siblings-reject-r2.mjs";

globalThis.siblingsLog.push("root");
