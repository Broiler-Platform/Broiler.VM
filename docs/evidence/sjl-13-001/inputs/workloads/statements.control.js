// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// STATEMENTS (CONTROL) - the workload in statements.js as one block comment, of the same goal (script).
// The same characters reach the tokenizer, and no construct reaches the parser; the difference between the two
// is the cost of the constructs.

/*
function s0(items, limit) {
    var total = 0;
    outer0: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 5) { break outer0; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 0);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s1(items, limit) {
    var total = 0;
    outer1: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 6) { break outer1; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 3);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s2(items, limit) {
    var total = 0;
    outer2: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 7) { break outer2; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 6);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s3(items, limit) {
    var total = 0;
    outer3: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 8) { break outer3; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 9);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s4(items, limit) {
    var total = 0;
    outer4: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 9) { break outer4; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 12);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s5(items, limit) {
    var total = 0;
    outer5: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 10) { break outer5; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 15);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s6(items, limit) {
    var total = 0;
    outer6: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 11) { break outer6; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 18);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s7(items, limit) {
    var total = 0;
    outer7: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 12) { break outer7; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 21);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s8(items, limit) {
    var total = 0;
    outer8: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 13) { break outer8; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 24);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s9(items, limit) {
    var total = 0;
    outer9: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 14) { break outer9; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 27);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s10(items, limit) {
    var total = 0;
    outer10: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 15) { break outer10; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 30);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s11(items, limit) {
    var total = 0;
    outer11: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 5) { break outer11; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 33);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s12(items, limit) {
    var total = 0;
    outer12: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 6) { break outer12; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 36);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s13(items, limit) {
    var total = 0;
    outer13: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 7) { break outer13; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 39);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s14(items, limit) {
    var total = 0;
    outer14: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 8) { break outer14; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 42);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s15(items, limit) {
    var total = 0;
    outer15: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 9) { break outer15; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 45);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s16(items, limit) {
    var total = 0;
    outer16: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 10) { break outer16; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 48);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s17(items, limit) {
    var total = 0;
    outer17: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 11) { break outer17; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 51);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s18(items, limit) {
    var total = 0;
    outer18: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 12) { break outer18; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 54);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s19(items, limit) {
    var total = 0;
    outer19: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 13) { break outer19; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 57);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s20(items, limit) {
    var total = 0;
    outer20: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 14) { break outer20; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 60);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s21(items, limit) {
    var total = 0;
    outer21: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 15) { break outer21; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 63);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s22(items, limit) {
    var total = 0;
    outer22: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 5) { break outer22; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 66);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s23(items, limit) {
    var total = 0;
    outer23: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 6) { break outer23; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 69);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s24(items, limit) {
    var total = 0;
    outer24: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 7) { break outer24; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 72);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s25(items, limit) {
    var total = 0;
    outer25: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 8) { break outer25; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 75);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s26(items, limit) {
    var total = 0;
    outer26: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 9) { break outer26; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 78);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s27(items, limit) {
    var total = 0;
    outer27: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 10) { break outer27; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 81);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s28(items, limit) {
    var total = 0;
    outer28: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 11) { break outer28; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 84);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s29(items, limit) {
    var total = 0;
    outer29: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 12) { break outer29; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 87);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s30(items, limit) {
    var total = 0;
    outer30: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 13) { break outer30; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 90);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s31(items, limit) {
    var total = 0;
    outer31: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 14) { break outer31; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 93);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s32(items, limit) {
    var total = 0;
    outer32: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 15) { break outer32; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 96);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s33(items, limit) {
    var total = 0;
    outer33: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 5) { break outer33; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 99);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s34(items, limit) {
    var total = 0;
    outer34: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 6) { break outer34; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 102);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s35(items, limit) {
    var total = 0;
    outer35: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 7) { break outer35; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 105);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s36(items, limit) {
    var total = 0;
    outer36: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 8) { break outer36; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 108);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s37(items, limit) {
    var total = 0;
    outer37: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 9) { break outer37; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 111);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s38(items, limit) {
    var total = 0;
    outer38: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 10) { break outer38; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 114);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s39(items, limit) {
    var total = 0;
    outer39: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 11) { break outer39; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 117);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s40(items, limit) {
    var total = 0;
    outer40: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 12) { break outer40; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 120);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s41(items, limit) {
    var total = 0;
    outer41: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 13) { break outer41; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 123);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s42(items, limit) {
    var total = 0;
    outer42: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 14) { break outer42; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 126);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s43(items, limit) {
    var total = 0;
    outer43: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 15) { break outer43; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 129);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s44(items, limit) {
    var total = 0;
    outer44: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 5) { break outer44; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 132);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s45(items, limit) {
    var total = 0;
    outer45: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 6) { break outer45; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 135);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s46(items, limit) {
    var total = 0;
    outer46: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 7) { break outer46; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 138);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s47(items, limit) {
    var total = 0;
    outer47: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 8) { break outer47; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 141);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s48(items, limit) {
    var total = 0;
    outer48: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 9) { break outer48; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 144);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s49(items, limit) {
    var total = 0;
    outer49: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 10) { break outer49; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 147);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s50(items, limit) {
    var total = 0;
    outer50: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 11) { break outer50; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 150);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s51(items, limit) {
    var total = 0;
    outer51: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 12) { break outer51; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 153);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s52(items, limit) {
    var total = 0;
    outer52: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 13) { break outer52; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 156);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s53(items, limit) {
    var total = 0;
    outer53: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 14) { break outer53; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 159);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s54(items, limit) {
    var total = 0;
    outer54: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 15) { break outer54; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 162);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s55(items, limit) {
    var total = 0;
    outer55: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 5) { break outer55; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 165);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s56(items, limit) {
    var total = 0;
    outer56: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 6) { break outer56; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 168);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s57(items, limit) {
    var total = 0;
    outer57: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 7) { break outer57; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 171);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s58(items, limit) {
    var total = 0;
    outer58: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 8) { break outer58; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 174);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s59(items, limit) {
    var total = 0;
    outer59: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 9) { break outer59; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 177);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s60(items, limit) {
    var total = 0;
    outer60: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 10) { break outer60; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 180);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s61(items, limit) {
    var total = 0;
    outer61: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 11) { break outer61; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 183);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s62(items, limit) {
    var total = 0;
    outer62: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 12) { break outer62; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 186);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s63(items, limit) {
    var total = 0;
    outer63: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 13) { break outer63; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 189);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s64(items, limit) {
    var total = 0;
    outer64: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 14) { break outer64; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 192);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s65(items, limit) {
    var total = 0;
    outer65: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 15) { break outer65; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 195);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s66(items, limit) {
    var total = 0;
    outer66: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 5) { break outer66; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 198);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s67(items, limit) {
    var total = 0;
    outer67: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 6) { break outer67; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 201);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s68(items, limit) {
    var total = 0;
    outer68: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 7) { break outer68; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 204);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s69(items, limit) {
    var total = 0;
    outer69: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 8) { break outer69; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 207);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s70(items, limit) {
    var total = 0;
    outer70: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 9) { break outer70; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 210);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s71(items, limit) {
    var total = 0;
    outer71: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 10) { break outer71; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 213);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s72(items, limit) {
    var total = 0;
    outer72: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 11) { break outer72; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 216);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s73(items, limit) {
    var total = 0;
    outer73: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 12) { break outer73; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 219);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s74(items, limit) {
    var total = 0;
    outer74: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 13) { break outer74; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 222);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s75(items, limit) {
    var total = 0;
    outer75: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 14) { break outer75; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 225);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s76(items, limit) {
    var total = 0;
    outer76: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 15) { break outer76; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 228);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s77(items, limit) {
    var total = 0;
    outer77: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 5) { break outer77; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 231);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s78(items, limit) {
    var total = 0;
    outer78: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 6) { break outer78; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 234);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s79(items, limit) {
    var total = 0;
    outer79: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 7) { break outer79; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 237);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s80(items, limit) {
    var total = 0;
    outer80: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 8) { break outer80; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 240);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s81(items, limit) {
    var total = 0;
    outer81: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 9) { break outer81; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 243);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s82(items, limit) {
    var total = 0;
    outer82: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 10) { break outer82; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 246);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s83(items, limit) {
    var total = 0;
    outer83: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 11) { break outer83; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 249);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s84(items, limit) {
    var total = 0;
    outer84: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 12) { break outer84; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 252);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s85(items, limit) {
    var total = 0;
    outer85: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 13) { break outer85; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 255);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s86(items, limit) {
    var total = 0;
    outer86: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 14) { break outer86; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 258);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s87(items, limit) {
    var total = 0;
    outer87: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 15) { break outer87; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 261);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s88(items, limit) {
    var total = 0;
    outer88: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 5) { break outer88; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 264);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s89(items, limit) {
    var total = 0;
    outer89: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 6) { break outer89; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 267);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s90(items, limit) {
    var total = 0;
    outer90: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 7) { break outer90; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 270);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s91(items, limit) {
    var total = 0;
    outer91: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 8) { break outer91; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 273);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s92(items, limit) {
    var total = 0;
    outer92: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 9) { break outer92; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 276);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s93(items, limit) {
    var total = 0;
    outer93: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 10) { break outer93; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 279);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s94(items, limit) {
    var total = 0;
    outer94: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 11) { break outer94; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 282);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s95(items, limit) {
    var total = 0;
    outer95: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 12) { break outer95; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 285);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s96(items, limit) {
    var total = 0;
    outer96: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 13) { break outer96; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 288);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s97(items, limit) {
    var total = 0;
    outer97: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 14) { break outer97; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 291);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s98(items, limit) {
    var total = 0;
    outer98: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 15) { break outer98; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 294);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s99(items, limit) {
    var total = 0;
    outer99: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 5) { break outer99; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 297);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s100(items, limit) {
    var total = 0;
    outer100: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 6) { break outer100; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 300);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s101(items, limit) {
    var total = 0;
    outer101: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 7) { break outer101; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 303);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s102(items, limit) {
    var total = 0;
    outer102: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 8) { break outer102; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 306);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s103(items, limit) {
    var total = 0;
    outer103: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 9) { break outer103; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 309);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s104(items, limit) {
    var total = 0;
    outer104: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 10) { break outer104; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 312);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s105(items, limit) {
    var total = 0;
    outer105: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 11) { break outer105; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 315);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s106(items, limit) {
    var total = 0;
    outer106: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 12) { break outer106; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 318);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s107(items, limit) {
    var total = 0;
    outer107: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 13) { break outer107; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 321);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s108(items, limit) {
    var total = 0;
    outer108: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 14) { break outer108; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 324);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s109(items, limit) {
    var total = 0;
    outer109: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 15) { break outer109; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 327);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s110(items, limit) {
    var total = 0;
    outer110: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 5) { break outer110; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 330);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s111(items, limit) {
    var total = 0;
    outer111: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 6) { break outer111; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 333);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s112(items, limit) {
    var total = 0;
    outer112: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 7) { break outer112; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 336);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s113(items, limit) {
    var total = 0;
    outer113: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 8) { break outer113; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 339);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s114(items, limit) {
    var total = 0;
    outer114: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 9) { break outer114; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 342);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s115(items, limit) {
    var total = 0;
    outer115: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 10) { break outer115; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 345);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s116(items, limit) {
    var total = 0;
    outer116: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 11) { break outer116; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 348);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s117(items, limit) {
    var total = 0;
    outer117: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 12) { break outer117; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 351);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s118(items, limit) {
    var total = 0;
    outer118: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 13) { break outer118; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 354);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
function s119(items, limit) {
    var total = 0;
    outer119: for (var k = 0; k < limit; k++) {
        if (k % 2 === 0) { continue; } else if (k > 14) { break outer119; }
        total += k;
    }
    for (var key in items) { if (key === "skip") continue; total += 1; }
    for (const item of items) { total += item ? 1 : 0; }
    var n = limit;
    while (n > 0) { n -= 1; total += n; }
    do { total -= 1; } while (total > 357);
    switch (total % 4) {
        case 0: total += 4; break;
        case 1: { let local = total; total = local * 2; break; }
        default: total -= 1;
    }
    try { if (total < 0) { throw new Error("negative"); } } catch (error) { total = 0; } finally { total += 1; }
    { let a = total; const b = a + 1; total = b; }
    return total;
}
s1([1, 0, 1], 9);
*/
