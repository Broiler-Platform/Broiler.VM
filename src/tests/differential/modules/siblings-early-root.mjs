// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// An async module that throws before its first await, beside an async sibling.

import "./siblings-early-tla.mjs";
import "./siblings-early-bad.mjs";

globalThis.siblingsLog.push("root");
