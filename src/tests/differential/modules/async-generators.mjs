// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A dependency of the module-goal probe, and the whole of what it is for is the DEFAULT export.
// An async generator was admitted everywhere else in this front end while `export default async
// function* () {}` was still refused by name - a gap in `export default` rather than in the family
// - so the anonymous default form is the one written here, with the named declaration form beside
// it because the same superseded case refused that too.

export default async function* () {
  yield "default";
}

export async function* named() {
  yield "named";
}
