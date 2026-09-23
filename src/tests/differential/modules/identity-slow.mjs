// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A module that awaits three times at its top level. A second graph that depends on it and is
// started while it is still awaiting must wait for it rather than read its first value.

export let v = "start";

await null;
await null;
await null;

v = "end";
