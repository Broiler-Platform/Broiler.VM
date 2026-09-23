// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Two synchronous parents of one async module run in the order they were reached.

import "./siblings-diamond-x.mjs";
import "./siblings-diamond-y.mjs";

globalThis.siblingsLog.push("root");
