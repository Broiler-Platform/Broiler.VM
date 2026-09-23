// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A module whose body throws synchronously. It counts its evaluations, so a body that runs again
// on a second import is a visible number.

globalThis.identityThrowsRuns = (globalThis.identityThrowsRuns || 0) + 1;

throw new Error("sync-boom");
