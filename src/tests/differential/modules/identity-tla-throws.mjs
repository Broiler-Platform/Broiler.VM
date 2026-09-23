// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// A module whose body throws after an await: the import that reaches it rejects with that value.

await null;

throw new RangeError("late");
