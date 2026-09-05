// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A module that awaits at its top level. Everything that imports it, directly or through another
// module, is evaluated after this has settled - which is the ordering top-level `await` exists to
// give and the one a probe can observe.

export let stage = "before";

stage = await Promise.resolve("after");

export const settled = stage;
