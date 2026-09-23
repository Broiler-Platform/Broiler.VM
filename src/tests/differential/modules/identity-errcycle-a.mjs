// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The root of a synchronous cycle whose body throws after the other member has run. Both members
// are errored: an import() of either rejects with the same value.

import "./identity-errcycle-b.mjs";

throw new TypeError("cycle-boom");
