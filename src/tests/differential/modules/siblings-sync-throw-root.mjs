// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A synchronous sibling throws while an async sibling is suspended.

import "./siblings-sync-throw-tla.mjs";
import "./siblings-sync-throw-bad.mjs";

globalThis.siblingsLog.push("root");
