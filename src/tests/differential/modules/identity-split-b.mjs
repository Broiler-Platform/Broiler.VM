// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The synchronous member of the split cycle. Its body runs before its root's other dependency
// has finished.

import "./identity-split-a.mjs";

globalThis.identitySplitLog.push("B run");
