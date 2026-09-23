// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// The member of the throwing cycle whose own body finishes.

import "./identity-errcycle-a.mjs";

globalThis.identityErrCycleBRuns = (globalThis.identityErrCycleBRuns || 0) + 1;
