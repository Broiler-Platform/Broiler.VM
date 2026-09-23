// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// One half of a cycle reached only by a dynamic import. The other half runs first and meets
// this module's `let` in its dead zone; a function it exports reads the live value later.

import { b, readA } from "./identity-cycle-b.mjs";

export let a = "a0";
a = "a1";

export const fromB = b;
export const late = readA();
