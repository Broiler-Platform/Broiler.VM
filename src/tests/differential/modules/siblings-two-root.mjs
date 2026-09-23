// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Two async siblings and a synchronous one: all three start before either async one resumes.

import "./siblings-two-a.mjs";
import "./siblings-two-b.mjs";
import "./siblings-two-s.mjs";

globalThis.siblingsLog.push("root");
