// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A module with a top-level await whose LAST statement is a promise that never settles. Its
// evaluation still finishes: a module body's completion value is not its result.

export let done = "no";

await null;

done = "yes";

new Promise(function () {});
