// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The synchronous member of the awaiting cycle. Its body runs first and finishes at once.

import "./identity-await-a.mjs";

globalThis.identityAwaitLog.push("B run");
