// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// FRESH-SMALL - a program short enough that starting the process is most of what running it costs.
//
// IT IS NOT IN THE MEASURED POPULATION, and no figure from it is judged by the granularity decision
// rule. It exists for the fresh-process comparison, which asks a different question from the rest of
// this folder: not what a form costs per unit of work, but what it costs to start at all. The
// subtraction the other shapes rely on - a run lane minus a check lane - removes almost everything
// here, which is exactly the point of asking separately.

var t = 0;

for (var i = 0; i < 1000; i++) {
  t = (t + i) % 1000003;
}

t;
