// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The synchronous sibling that throws.

globalThis.siblingsLog.push("bad");
throw new TypeError("sync");
