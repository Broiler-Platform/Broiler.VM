// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Depends on identity-slow.mjs, and is imported while that module is still awaiting.

import { v } from "./identity-slow.mjs";

export const seen = v;
