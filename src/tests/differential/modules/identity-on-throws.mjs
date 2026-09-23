// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// Depends on identity-throws.mjs after that module has already failed. Its body must not run, and
// an import of it rejects with the dependency's own error.

import "./identity-throws.mjs";

globalThis.identityOnThrowsRan = true;

export const ok = "ran";
