// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A parent whose dependency is faster runs first, whatever the import order.

import "./siblings-race-p.mjs";
import "./siblings-race-q.mjs";

globalThis.siblingsLog.push("root");
