// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the module-scoped eval probe that imports from the probe itself, so it runs
// first and meets the probe's export in its dead zone through a direct eval; the function it
// exports reads the same import once the probe has run.
import { later } from "../the-module-scoped-eval.mjs";

let seen;
try { seen = eval("typeof later"); } catch (e) { seen = e.name; }
export const early = seen;

export function readLater() {
  return eval("later");
}
