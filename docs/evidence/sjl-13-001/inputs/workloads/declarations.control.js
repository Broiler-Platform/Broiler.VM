// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// DECLARATIONS (CONTROL) - the workload in declarations.js as one block comment, of the same goal (script).
// The same characters reach the tokenizer, and no construct reaches the parser; the difference between the two
// is the cost of the constructs.

/*
function d0(first, second = 0, ...rest) {
    var [p0, q0 = 1, ...r0] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p0, gamma: q0 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r0, size: pair.length };
}
function d1(first, second = 1, ...rest) {
    var [p1, q1 = 1, ...r1] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p1, gamma: q1 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r1, size: pair.length };
}
function d2(first, second = 2, ...rest) {
    var [p2, q2 = 1, ...r2] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p2, gamma: q2 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r2, size: pair.length };
}
function d3(first, second = 3, ...rest) {
    var [p3, q3 = 1, ...r3] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p3, gamma: q3 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r3, size: pair.length };
}
function d4(first, second = 4, ...rest) {
    var [p4, q4 = 1, ...r4] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p4, gamma: q4 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r4, size: pair.length };
}
function d5(first, second = 5, ...rest) {
    var [p5, q5 = 1, ...r5] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p5, gamma: q5 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r5, size: pair.length };
}
function d6(first, second = 6, ...rest) {
    var [p6, q6 = 1, ...r6] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p6, gamma: q6 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r6, size: pair.length };
}
function d7(first, second = 7, ...rest) {
    var [p7, q7 = 1, ...r7] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p7, gamma: q7 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r7, size: pair.length };
}
function d8(first, second = 8, ...rest) {
    var [p8, q8 = 1, ...r8] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p8, gamma: q8 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r8, size: pair.length };
}
function d9(first, second = 9, ...rest) {
    var [p9, q9 = 1, ...r9] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p9, gamma: q9 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r9, size: pair.length };
}
function d10(first, second = 10, ...rest) {
    var [p10, q10 = 1, ...r10] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p10, gamma: q10 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r10, size: pair.length };
}
function d11(first, second = 11, ...rest) {
    var [p11, q11 = 1, ...r11] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p11, gamma: q11 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r11, size: pair.length };
}
function d12(first, second = 12, ...rest) {
    var [p12, q12 = 1, ...r12] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p12, gamma: q12 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r12, size: pair.length };
}
function d13(first, second = 13, ...rest) {
    var [p13, q13 = 1, ...r13] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p13, gamma: q13 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r13, size: pair.length };
}
function d14(first, second = 14, ...rest) {
    var [p14, q14 = 1, ...r14] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p14, gamma: q14 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r14, size: pair.length };
}
function d15(first, second = 15, ...rest) {
    var [p15, q15 = 1, ...r15] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p15, gamma: q15 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r15, size: pair.length };
}
function d16(first, second = 16, ...rest) {
    var [p16, q16 = 1, ...r16] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p16, gamma: q16 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r16, size: pair.length };
}
function d17(first, second = 17, ...rest) {
    var [p17, q17 = 1, ...r17] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p17, gamma: q17 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r17, size: pair.length };
}
function d18(first, second = 18, ...rest) {
    var [p18, q18 = 1, ...r18] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p18, gamma: q18 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r18, size: pair.length };
}
function d19(first, second = 19, ...rest) {
    var [p19, q19 = 1, ...r19] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p19, gamma: q19 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r19, size: pair.length };
}
function d20(first, second = 20, ...rest) {
    var [p20, q20 = 1, ...r20] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p20, gamma: q20 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r20, size: pair.length };
}
function d21(first, second = 21, ...rest) {
    var [p21, q21 = 1, ...r21] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p21, gamma: q21 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r21, size: pair.length };
}
function d22(first, second = 22, ...rest) {
    var [p22, q22 = 1, ...r22] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p22, gamma: q22 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r22, size: pair.length };
}
function d23(first, second = 23, ...rest) {
    var [p23, q23 = 1, ...r23] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p23, gamma: q23 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r23, size: pair.length };
}
function d24(first, second = 24, ...rest) {
    var [p24, q24 = 1, ...r24] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p24, gamma: q24 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r24, size: pair.length };
}
function d25(first, second = 25, ...rest) {
    var [p25, q25 = 1, ...r25] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p25, gamma: q25 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r25, size: pair.length };
}
function d26(first, second = 26, ...rest) {
    var [p26, q26 = 1, ...r26] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p26, gamma: q26 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r26, size: pair.length };
}
function d27(first, second = 27, ...rest) {
    var [p27, q27 = 1, ...r27] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p27, gamma: q27 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r27, size: pair.length };
}
function d28(first, second = 28, ...rest) {
    var [p28, q28 = 1, ...r28] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p28, gamma: q28 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r28, size: pair.length };
}
function d29(first, second = 29, ...rest) {
    var [p29, q29 = 1, ...r29] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p29, gamma: q29 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r29, size: pair.length };
}
function d30(first, second = 30, ...rest) {
    var [p30, q30 = 1, ...r30] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p30, gamma: q30 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r30, size: pair.length };
}
function d31(first, second = 31, ...rest) {
    var [p31, q31 = 1, ...r31] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p31, gamma: q31 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r31, size: pair.length };
}
function d32(first, second = 32, ...rest) {
    var [p32, q32 = 1, ...r32] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p32, gamma: q32 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r32, size: pair.length };
}
function d33(first, second = 33, ...rest) {
    var [p33, q33 = 1, ...r33] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p33, gamma: q33 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r33, size: pair.length };
}
function d34(first, second = 34, ...rest) {
    var [p34, q34 = 1, ...r34] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p34, gamma: q34 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r34, size: pair.length };
}
function d35(first, second = 35, ...rest) {
    var [p35, q35 = 1, ...r35] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p35, gamma: q35 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r35, size: pair.length };
}
function d36(first, second = 36, ...rest) {
    var [p36, q36 = 1, ...r36] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p36, gamma: q36 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r36, size: pair.length };
}
function d37(first, second = 37, ...rest) {
    var [p37, q37 = 1, ...r37] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p37, gamma: q37 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r37, size: pair.length };
}
function d38(first, second = 38, ...rest) {
    var [p38, q38 = 1, ...r38] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p38, gamma: q38 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r38, size: pair.length };
}
function d39(first, second = 39, ...rest) {
    var [p39, q39 = 1, ...r39] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p39, gamma: q39 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r39, size: pair.length };
}
function d40(first, second = 40, ...rest) {
    var [p40, q40 = 1, ...r40] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p40, gamma: q40 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r40, size: pair.length };
}
function d41(first, second = 41, ...rest) {
    var [p41, q41 = 1, ...r41] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p41, gamma: q41 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r41, size: pair.length };
}
function d42(first, second = 42, ...rest) {
    var [p42, q42 = 1, ...r42] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p42, gamma: q42 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r42, size: pair.length };
}
function d43(first, second = 43, ...rest) {
    var [p43, q43 = 1, ...r43] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p43, gamma: q43 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r43, size: pair.length };
}
function d44(first, second = 44, ...rest) {
    var [p44, q44 = 1, ...r44] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p44, gamma: q44 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r44, size: pair.length };
}
function d45(first, second = 45, ...rest) {
    var [p45, q45 = 1, ...r45] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p45, gamma: q45 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r45, size: pair.length };
}
function d46(first, second = 46, ...rest) {
    var [p46, q46 = 1, ...r46] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p46, gamma: q46 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r46, size: pair.length };
}
function d47(first, second = 47, ...rest) {
    var [p47, q47 = 1, ...r47] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p47, gamma: q47 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r47, size: pair.length };
}
function d48(first, second = 48, ...rest) {
    var [p48, q48 = 1, ...r48] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p48, gamma: q48 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r48, size: pair.length };
}
function d49(first, second = 49, ...rest) {
    var [p49, q49 = 1, ...r49] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p49, gamma: q49 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r49, size: pair.length };
}
function d50(first, second = 50, ...rest) {
    var [p50, q50 = 1, ...r50] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p50, gamma: q50 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r50, size: pair.length };
}
function d51(first, second = 51, ...rest) {
    var [p51, q51 = 1, ...r51] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p51, gamma: q51 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r51, size: pair.length };
}
function d52(first, second = 52, ...rest) {
    var [p52, q52 = 1, ...r52] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p52, gamma: q52 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r52, size: pair.length };
}
function d53(first, second = 53, ...rest) {
    var [p53, q53 = 1, ...r53] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p53, gamma: q53 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r53, size: pair.length };
}
function d54(first, second = 54, ...rest) {
    var [p54, q54 = 1, ...r54] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p54, gamma: q54 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r54, size: pair.length };
}
function d55(first, second = 55, ...rest) {
    var [p55, q55 = 1, ...r55] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p55, gamma: q55 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r55, size: pair.length };
}
function d56(first, second = 56, ...rest) {
    var [p56, q56 = 1, ...r56] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p56, gamma: q56 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r56, size: pair.length };
}
function d57(first, second = 57, ...rest) {
    var [p57, q57 = 1, ...r57] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p57, gamma: q57 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r57, size: pair.length };
}
function d58(first, second = 58, ...rest) {
    var [p58, q58 = 1, ...r58] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p58, gamma: q58 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r58, size: pair.length };
}
function d59(first, second = 59, ...rest) {
    var [p59, q59 = 1, ...r59] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p59, gamma: q59 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r59, size: pair.length };
}
function d60(first, second = 60, ...rest) {
    var [p60, q60 = 1, ...r60] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p60, gamma: q60 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r60, size: pair.length };
}
function d61(first, second = 61, ...rest) {
    var [p61, q61 = 1, ...r61] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p61, gamma: q61 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r61, size: pair.length };
}
function d62(first, second = 62, ...rest) {
    var [p62, q62 = 1, ...r62] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p62, gamma: q62 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r62, size: pair.length };
}
function d63(first, second = 63, ...rest) {
    var [p63, q63 = 1, ...r63] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p63, gamma: q63 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r63, size: pair.length };
}
function d64(first, second = 64, ...rest) {
    var [p64, q64 = 1, ...r64] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p64, gamma: q64 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r64, size: pair.length };
}
function d65(first, second = 65, ...rest) {
    var [p65, q65 = 1, ...r65] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p65, gamma: q65 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r65, size: pair.length };
}
function d66(first, second = 66, ...rest) {
    var [p66, q66 = 1, ...r66] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p66, gamma: q66 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r66, size: pair.length };
}
function d67(first, second = 67, ...rest) {
    var [p67, q67 = 1, ...r67] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p67, gamma: q67 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r67, size: pair.length };
}
function d68(first, second = 68, ...rest) {
    var [p68, q68 = 1, ...r68] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p68, gamma: q68 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r68, size: pair.length };
}
function d69(first, second = 69, ...rest) {
    var [p69, q69 = 1, ...r69] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p69, gamma: q69 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r69, size: pair.length };
}
function d70(first, second = 70, ...rest) {
    var [p70, q70 = 1, ...r70] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p70, gamma: q70 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r70, size: pair.length };
}
function d71(first, second = 71, ...rest) {
    var [p71, q71 = 1, ...r71] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p71, gamma: q71 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r71, size: pair.length };
}
function d72(first, second = 72, ...rest) {
    var [p72, q72 = 1, ...r72] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p72, gamma: q72 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r72, size: pair.length };
}
function d73(first, second = 73, ...rest) {
    var [p73, q73 = 1, ...r73] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p73, gamma: q73 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r73, size: pair.length };
}
function d74(first, second = 74, ...rest) {
    var [p74, q74 = 1, ...r74] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p74, gamma: q74 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r74, size: pair.length };
}
function d75(first, second = 75, ...rest) {
    var [p75, q75 = 1, ...r75] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p75, gamma: q75 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r75, size: pair.length };
}
function d76(first, second = 76, ...rest) {
    var [p76, q76 = 1, ...r76] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p76, gamma: q76 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r76, size: pair.length };
}
function d77(first, second = 77, ...rest) {
    var [p77, q77 = 1, ...r77] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p77, gamma: q77 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r77, size: pair.length };
}
function d78(first, second = 78, ...rest) {
    var [p78, q78 = 1, ...r78] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p78, gamma: q78 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r78, size: pair.length };
}
function d79(first, second = 79, ...rest) {
    var [p79, q79 = 1, ...r79] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p79, gamma: q79 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r79, size: pair.length };
}
function d80(first, second = 80, ...rest) {
    var [p80, q80 = 1, ...r80] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p80, gamma: q80 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r80, size: pair.length };
}
function d81(first, second = 81, ...rest) {
    var [p81, q81 = 1, ...r81] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p81, gamma: q81 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r81, size: pair.length };
}
function d82(first, second = 82, ...rest) {
    var [p82, q82 = 1, ...r82] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p82, gamma: q82 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r82, size: pair.length };
}
function d83(first, second = 83, ...rest) {
    var [p83, q83 = 1, ...r83] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p83, gamma: q83 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r83, size: pair.length };
}
function d84(first, second = 84, ...rest) {
    var [p84, q84 = 1, ...r84] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p84, gamma: q84 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r84, size: pair.length };
}
function d85(first, second = 85, ...rest) {
    var [p85, q85 = 1, ...r85] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p85, gamma: q85 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r85, size: pair.length };
}
function d86(first, second = 86, ...rest) {
    var [p86, q86 = 1, ...r86] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p86, gamma: q86 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r86, size: pair.length };
}
function d87(first, second = 87, ...rest) {
    var [p87, q87 = 1, ...r87] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p87, gamma: q87 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r87, size: pair.length };
}
function d88(first, second = 88, ...rest) {
    var [p88, q88 = 1, ...r88] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p88, gamma: q88 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r88, size: pair.length };
}
function d89(first, second = 89, ...rest) {
    var [p89, q89 = 1, ...r89] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p89, gamma: q89 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r89, size: pair.length };
}
function d90(first, second = 90, ...rest) {
    var [p90, q90 = 1, ...r90] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p90, gamma: q90 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r90, size: pair.length };
}
function d91(first, second = 91, ...rest) {
    var [p91, q91 = 1, ...r91] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p91, gamma: q91 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r91, size: pair.length };
}
function d92(first, second = 92, ...rest) {
    var [p92, q92 = 1, ...r92] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p92, gamma: q92 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r92, size: pair.length };
}
function d93(first, second = 93, ...rest) {
    var [p93, q93 = 1, ...r93] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p93, gamma: q93 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r93, size: pair.length };
}
function d94(first, second = 94, ...rest) {
    var [p94, q94 = 1, ...r94] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p94, gamma: q94 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r94, size: pair.length };
}
function d95(first, second = 95, ...rest) {
    var [p95, q95 = 1, ...r95] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p95, gamma: q95 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r95, size: pair.length };
}
function d96(first, second = 96, ...rest) {
    var [p96, q96 = 1, ...r96] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p96, gamma: q96 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r96, size: pair.length };
}
function d97(first, second = 97, ...rest) {
    var [p97, q97 = 1, ...r97] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p97, gamma: q97 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r97, size: pair.length };
}
function d98(first, second = 98, ...rest) {
    var [p98, q98 = 1, ...r98] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p98, gamma: q98 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r98, size: pair.length };
}
function d99(first, second = 99, ...rest) {
    var [p99, q99 = 1, ...r99] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p99, gamma: q99 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r99, size: pair.length };
}
function d100(first, second = 100, ...rest) {
    var [p100, q100 = 1, ...r100] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p100, gamma: q100 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r100, size: pair.length };
}
function d101(first, second = 101, ...rest) {
    var [p101, q101 = 1, ...r101] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p101, gamma: q101 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r101, size: pair.length };
}
function d102(first, second = 102, ...rest) {
    var [p102, q102 = 1, ...r102] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p102, gamma: q102 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r102, size: pair.length };
}
function d103(first, second = 103, ...rest) {
    var [p103, q103 = 1, ...r103] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p103, gamma: q103 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r103, size: pair.length };
}
function d104(first, second = 104, ...rest) {
    var [p104, q104 = 1, ...r104] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p104, gamma: q104 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r104, size: pair.length };
}
function d105(first, second = 105, ...rest) {
    var [p105, q105 = 1, ...r105] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p105, gamma: q105 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r105, size: pair.length };
}
function d106(first, second = 106, ...rest) {
    var [p106, q106 = 1, ...r106] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p106, gamma: q106 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r106, size: pair.length };
}
function d107(first, second = 107, ...rest) {
    var [p107, q107 = 1, ...r107] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p107, gamma: q107 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r107, size: pair.length };
}
function d108(first, second = 108, ...rest) {
    var [p108, q108 = 1, ...r108] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p108, gamma: q108 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r108, size: pair.length };
}
function d109(first, second = 109, ...rest) {
    var [p109, q109 = 1, ...r109] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p109, gamma: q109 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r109, size: pair.length };
}
function d110(first, second = 110, ...rest) {
    var [p110, q110 = 1, ...r110] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p110, gamma: q110 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r110, size: pair.length };
}
function d111(first, second = 111, ...rest) {
    var [p111, q111 = 1, ...r111] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p111, gamma: q111 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r111, size: pair.length };
}
function d112(first, second = 112, ...rest) {
    var [p112, q112 = 1, ...r112] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p112, gamma: q112 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r112, size: pair.length };
}
function d113(first, second = 113, ...rest) {
    var [p113, q113 = 1, ...r113] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p113, gamma: q113 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r113, size: pair.length };
}
function d114(first, second = 114, ...rest) {
    var [p114, q114 = 1, ...r114] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p114, gamma: q114 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r114, size: pair.length };
}
function d115(first, second = 115, ...rest) {
    var [p115, q115 = 1, ...r115] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p115, gamma: q115 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r115, size: pair.length };
}
function d116(first, second = 116, ...rest) {
    var [p116, q116 = 1, ...r116] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p116, gamma: q116 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r116, size: pair.length };
}
function d117(first, second = 117, ...rest) {
    var [p117, q117 = 1, ...r117] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p117, gamma: q117 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r117, size: pair.length };
}
function d118(first, second = 118, ...rest) {
    var [p118, q118 = 1, ...r118] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p118, gamma: q118 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r118, size: pair.length };
}
function d119(first, second = 119, ...rest) {
    var [p119, q119 = 1, ...r119] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p119, gamma: q119 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r119, size: pair.length };
}
function d120(first, second = 120, ...rest) {
    var [p120, q120 = 1, ...r120] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p120, gamma: q120 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r120, size: pair.length };
}
function d121(first, second = 121, ...rest) {
    var [p121, q121 = 1, ...r121] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p121, gamma: q121 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r121, size: pair.length };
}
function d122(first, second = 122, ...rest) {
    var [p122, q122 = 1, ...r122] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p122, gamma: q122 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r122, size: pair.length };
}
function d123(first, second = 123, ...rest) {
    var [p123, q123 = 1, ...r123] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p123, gamma: q123 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r123, size: pair.length };
}
function d124(first, second = 124, ...rest) {
    var [p124, q124 = 1, ...r124] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p124, gamma: q124 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r124, size: pair.length };
}
function d125(first, second = 125, ...rest) {
    var [p125, q125 = 1, ...r125] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p125, gamma: q125 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r125, size: pair.length };
}
function d126(first, second = 126, ...rest) {
    var [p126, q126 = 1, ...r126] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p126, gamma: q126 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r126, size: pair.length };
}
function d127(first, second = 127, ...rest) {
    var [p127, q127 = 1, ...r127] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p127, gamma: q127 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r127, size: pair.length };
}
function d128(first, second = 128, ...rest) {
    var [p128, q128 = 1, ...r128] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p128, gamma: q128 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r128, size: pair.length };
}
function d129(first, second = 129, ...rest) {
    var [p129, q129 = 1, ...r129] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p129, gamma: q129 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r129, size: pair.length };
}
function d130(first, second = 130, ...rest) {
    var [p130, q130 = 1, ...r130] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p130, gamma: q130 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r130, size: pair.length };
}
function d131(first, second = 131, ...rest) {
    var [p131, q131 = 1, ...r131] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p131, gamma: q131 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r131, size: pair.length };
}
function d132(first, second = 132, ...rest) {
    var [p132, q132 = 1, ...r132] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p132, gamma: q132 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r132, size: pair.length };
}
function d133(first, second = 133, ...rest) {
    var [p133, q133 = 1, ...r133] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p133, gamma: q133 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r133, size: pair.length };
}
function d134(first, second = 134, ...rest) {
    var [p134, q134 = 1, ...r134] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p134, gamma: q134 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r134, size: pair.length };
}
function d135(first, second = 135, ...rest) {
    var [p135, q135 = 1, ...r135] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p135, gamma: q135 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r135, size: pair.length };
}
function d136(first, second = 136, ...rest) {
    var [p136, q136 = 1, ...r136] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p136, gamma: q136 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r136, size: pair.length };
}
function d137(first, second = 137, ...rest) {
    var [p137, q137 = 1, ...r137] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p137, gamma: q137 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r137, size: pair.length };
}
function d138(first, second = 138, ...rest) {
    var [p138, q138 = 1, ...r138] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p138, gamma: q138 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r138, size: pair.length };
}
function d139(first, second = 139, ...rest) {
    var [p139, q139 = 1, ...r139] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p139, gamma: q139 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r139, size: pair.length };
}
function d140(first, second = 140, ...rest) {
    var [p140, q140 = 1, ...r140] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p140, gamma: q140 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r140, size: pair.length };
}
function d141(first, second = 141, ...rest) {
    var [p141, q141 = 1, ...r141] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p141, gamma: q141 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 7;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r141, size: pair.length };
}
function d142(first, second = 142, ...rest) {
    var [p142, q142 = 1, ...r142] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p142, gamma: q142 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 8;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r142, size: pair.length };
}
function d143(first, second = 143, ...rest) {
    var [p143, q143 = 1, ...r143] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p143, gamma: q143 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 9;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r143, size: pair.length };
}
function d144(first, second = 144, ...rest) {
    var [p144, q144 = 1, ...r144] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p144, gamma: q144 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 1;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r144, size: pair.length };
}
function d145(first, second = 145, ...rest) {
    var [p145, q145 = 1, ...r145] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p145, gamma: q145 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 2;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r145, size: pair.length };
}
function d146(first, second = 146, ...rest) {
    var [p146, q146 = 1, ...r146] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p146, gamma: q146 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 3;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r146, size: pair.length };
}
function d147(first, second = 147, ...rest) {
    var [p147, q147 = 1, ...r147] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p147, gamma: q147 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 4;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r147, size: pair.length };
}
function d148(first, second = 148, ...rest) {
    var [p148, q148 = 1, ...r148] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p148, gamma: q148 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 5;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r148, size: pair.length };
}
function d149(first, second = 149, ...rest) {
    var [p149, q149 = 1, ...r149] = [first, second].concat(rest);
    let { alpha, beta = 2, gamma: delta } = { alpha: p149, gamma: q149 };
    const pair = [alpha, beta, delta];
    const add = (u, v) => u + v;
    const scale = u => u * 6;
    function* steps(limit) { for (let k = 0; k < limit; k++) { yield k; } }
    async function later(value) { return value; }
    let total = 0;
    for (const step of steps(3)) { total = add(total, scale(step)); }
    return { pair, total, later, r: r149, size: pair.length };
}
d1(1, 2, 3);
*/
