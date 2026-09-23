// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The first parent of the module that rejects; it never runs.

import "./siblings-reject-bad.mjs";

globalThis.siblingsLog.push("r1");
