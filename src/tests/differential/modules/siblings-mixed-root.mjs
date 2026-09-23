// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// One async module with an async parent and a synchronous parent.

import "./siblings-mixed-m1.mjs";
import "./siblings-mixed-m2.mjs";

globalThis.siblingsLog.push("root");
