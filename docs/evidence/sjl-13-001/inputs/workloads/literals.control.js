// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// LITERALS (CONTROL) - the workload in literals.js as one block comment, of the same goal (script).
// The same characters reach the tokenizer, and no construct reaches the parser; the difference between the two
// is the cost of the constructs.

/*
function l0() {
    var text = "line 0\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 0 \'quoted\'';
    var tpl = `head 0 ${text.length} middle ${single.length + 0} tail`;
    var numbers = [0, 0, 0.5, 1e0, 0x0, 0o07, 0b101, 1_000_000, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 0, name: "item0", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l1() {
    var text = "line 1\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 1 \'quoted\'';
    var tpl = `head 1 ${text.length} middle ${single.length + 1} tail`;
    var numbers = [0, 1, 1.5, 1e1, 0x1, 0o17, 0b101, 1_000_001, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 1, name: "item1", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l2() {
    var text = "line 2\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 2 \'quoted\'';
    var tpl = `head 2 ${text.length} middle ${single.length + 2} tail`;
    var numbers = [0, 2, 2.5, 1e2, 0x2, 0o27, 0b101, 1_000_002, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 2, name: "item2", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l3() {
    var text = "line 3\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 3 \'quoted\'';
    var tpl = `head 3 ${text.length} middle ${single.length + 3} tail`;
    var numbers = [0, 3, 3.5, 1e3, 0x3, 0o37, 0b101, 1_000_003, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 3, name: "item3", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l4() {
    var text = "line 4\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 4 \'quoted\'';
    var tpl = `head 4 ${text.length} middle ${single.length + 4} tail`;
    var numbers = [0, 4, 4.5, 1e4, 0x4, 0o47, 0b101, 1_000_004, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 4, name: "item4", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l5() {
    var text = "line 5\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 5 \'quoted\'';
    var tpl = `head 5 ${text.length} middle ${single.length + 5} tail`;
    var numbers = [0, 5, 5.5, 1e5, 0x5, 0o57, 0b101, 1_000_005, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 5, name: "item5", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l6() {
    var text = "line 6\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 6 \'quoted\'';
    var tpl = `head 6 ${text.length} middle ${single.length + 6} tail`;
    var numbers = [0, 6, 6.5, 1e6, 0x6, 0o67, 0b101, 1_000_006, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 6, name: "item6", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l7() {
    var text = "line 7\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 7 \'quoted\'';
    var tpl = `head 7 ${text.length} middle ${single.length + 7} tail`;
    var numbers = [0, 7, 7.5, 1e7, 0x7, 0o77, 0b101, 1_000_007, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 7, name: "item7", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l8() {
    var text = "line 8\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 8 \'quoted\'';
    var tpl = `head 8 ${text.length} middle ${single.length + 8} tail`;
    var numbers = [0, 8, 8.5, 1e8, 0x8, 0o07, 0b101, 1_000_008, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 8, name: "item8", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l9() {
    var text = "line 9\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 9 \'quoted\'';
    var tpl = `head 9 ${text.length} middle ${single.length + 9} tail`;
    var numbers = [0, 9, 9.5, 1e9, 0x9, 0o17, 0b101, 1_000_009, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 9, name: "item9", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l10() {
    var text = "line 10\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 10 \'quoted\'';
    var tpl = `head 10 ${text.length} middle ${single.length + 10} tail`;
    var numbers = [0, 10, 10.5, 1e10, 0xA, 0o27, 0b101, 1_000_010, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 10, name: "item10", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l11() {
    var text = "line 11\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 11 \'quoted\'';
    var tpl = `head 11 ${text.length} middle ${single.length + 11} tail`;
    var numbers = [0, 11, 11.5, 1e11, 0xB, 0o37, 0b101, 1_000_011, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 11, name: "item11", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l12() {
    var text = "line 12\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 12 \'quoted\'';
    var tpl = `head 12 ${text.length} middle ${single.length + 12} tail`;
    var numbers = [0, 12, 12.5, 1e12, 0xC, 0o47, 0b101, 1_000_012, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 12, name: "item12", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l13() {
    var text = "line 13\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 13 \'quoted\'';
    var tpl = `head 13 ${text.length} middle ${single.length + 13} tail`;
    var numbers = [0, 13, 13.5, 1e13, 0xD, 0o57, 0b101, 1_000_013, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 13, name: "item13", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l14() {
    var text = "line 14\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 14 \'quoted\'';
    var tpl = `head 14 ${text.length} middle ${single.length + 14} tail`;
    var numbers = [0, 14, 14.5, 1e14, 0xE, 0o67, 0b101, 1_000_014, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 14, name: "item14", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l15() {
    var text = "line 15\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 15 \'quoted\'';
    var tpl = `head 15 ${text.length} middle ${single.length + 15} tail`;
    var numbers = [0, 15, 15.5, 1e15, 0xF, 0o77, 0b101, 1_000_015, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 15, name: "item15", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l16() {
    var text = "line 16\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 16 \'quoted\'';
    var tpl = `head 16 ${text.length} middle ${single.length + 16} tail`;
    var numbers = [0, 16, 16.5, 1e16, 0x10, 0o07, 0b101, 1_000_016, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 16, name: "item16", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l17() {
    var text = "line 17\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 17 \'quoted\'';
    var tpl = `head 17 ${text.length} middle ${single.length + 17} tail`;
    var numbers = [0, 17, 17.5, 1e17, 0x11, 0o17, 0b101, 1_000_017, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 17, name: "item17", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l18() {
    var text = "line 18\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 18 \'quoted\'';
    var tpl = `head 18 ${text.length} middle ${single.length + 18} tail`;
    var numbers = [0, 18, 18.5, 1e18, 0x12, 0o27, 0b101, 1_000_018, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 18, name: "item18", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l19() {
    var text = "line 19\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 19 \'quoted\'';
    var tpl = `head 19 ${text.length} middle ${single.length + 19} tail`;
    var numbers = [0, 19, 19.5, 1e19, 0x13, 0o37, 0b101, 1_000_019, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 19, name: "item19", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l20() {
    var text = "line 20\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 20 \'quoted\'';
    var tpl = `head 20 ${text.length} middle ${single.length + 20} tail`;
    var numbers = [0, 20, 20.5, 1e0, 0x14, 0o47, 0b101, 1_000_020, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 20, name: "item20", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l21() {
    var text = "line 21\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 21 \'quoted\'';
    var tpl = `head 21 ${text.length} middle ${single.length + 21} tail`;
    var numbers = [0, 21, 21.5, 1e1, 0x15, 0o57, 0b101, 1_000_021, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 21, name: "item21", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l22() {
    var text = "line 22\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 22 \'quoted\'';
    var tpl = `head 22 ${text.length} middle ${single.length + 22} tail`;
    var numbers = [0, 22, 22.5, 1e2, 0x16, 0o67, 0b101, 1_000_022, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 22, name: "item22", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l23() {
    var text = "line 23\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 23 \'quoted\'';
    var tpl = `head 23 ${text.length} middle ${single.length + 23} tail`;
    var numbers = [0, 23, 23.5, 1e3, 0x17, 0o77, 0b101, 1_000_023, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 23, name: "item23", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l24() {
    var text = "line 24\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 24 \'quoted\'';
    var tpl = `head 24 ${text.length} middle ${single.length + 24} tail`;
    var numbers = [0, 24, 24.5, 1e4, 0x18, 0o07, 0b101, 1_000_024, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 24, name: "item24", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l25() {
    var text = "line 25\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 25 \'quoted\'';
    var tpl = `head 25 ${text.length} middle ${single.length + 25} tail`;
    var numbers = [0, 25, 25.5, 1e5, 0x19, 0o17, 0b101, 1_000_025, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 25, name: "item25", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l26() {
    var text = "line 26\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 26 \'quoted\'';
    var tpl = `head 26 ${text.length} middle ${single.length + 26} tail`;
    var numbers = [0, 26, 26.5, 1e6, 0x1A, 0o27, 0b101, 1_000_026, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 26, name: "item26", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l27() {
    var text = "line 27\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 27 \'quoted\'';
    var tpl = `head 27 ${text.length} middle ${single.length + 27} tail`;
    var numbers = [0, 27, 27.5, 1e7, 0x1B, 0o37, 0b101, 1_000_027, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 27, name: "item27", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l28() {
    var text = "line 28\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 28 \'quoted\'';
    var tpl = `head 28 ${text.length} middle ${single.length + 28} tail`;
    var numbers = [0, 28, 28.5, 1e8, 0x1C, 0o47, 0b101, 1_000_028, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 28, name: "item28", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l29() {
    var text = "line 29\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 29 \'quoted\'';
    var tpl = `head 29 ${text.length} middle ${single.length + 29} tail`;
    var numbers = [0, 29, 29.5, 1e9, 0x1D, 0o57, 0b101, 1_000_029, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 29, name: "item29", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l30() {
    var text = "line 30\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 30 \'quoted\'';
    var tpl = `head 30 ${text.length} middle ${single.length + 30} tail`;
    var numbers = [0, 30, 30.5, 1e10, 0x1E, 0o67, 0b101, 1_000_030, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 30, name: "item30", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l31() {
    var text = "line 31\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 31 \'quoted\'';
    var tpl = `head 31 ${text.length} middle ${single.length + 31} tail`;
    var numbers = [0, 31, 31.5, 1e11, 0x1F, 0o77, 0b101, 1_000_031, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 31, name: "item31", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l32() {
    var text = "line 32\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 32 \'quoted\'';
    var tpl = `head 32 ${text.length} middle ${single.length + 32} tail`;
    var numbers = [0, 32, 32.5, 1e12, 0x20, 0o07, 0b101, 1_000_032, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 32, name: "item32", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l33() {
    var text = "line 33\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 33 \'quoted\'';
    var tpl = `head 33 ${text.length} middle ${single.length + 33} tail`;
    var numbers = [0, 33, 33.5, 1e13, 0x21, 0o17, 0b101, 1_000_033, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 33, name: "item33", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l34() {
    var text = "line 34\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 34 \'quoted\'';
    var tpl = `head 34 ${text.length} middle ${single.length + 34} tail`;
    var numbers = [0, 34, 34.5, 1e14, 0x22, 0o27, 0b101, 1_000_034, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 34, name: "item34", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l35() {
    var text = "line 35\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 35 \'quoted\'';
    var tpl = `head 35 ${text.length} middle ${single.length + 35} tail`;
    var numbers = [0, 35, 35.5, 1e15, 0x23, 0o37, 0b101, 1_000_035, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 35, name: "item35", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l36() {
    var text = "line 36\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 36 \'quoted\'';
    var tpl = `head 36 ${text.length} middle ${single.length + 36} tail`;
    var numbers = [0, 36, 36.5, 1e16, 0x24, 0o47, 0b101, 1_000_036, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 36, name: "item36", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l37() {
    var text = "line 37\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 37 \'quoted\'';
    var tpl = `head 37 ${text.length} middle ${single.length + 37} tail`;
    var numbers = [0, 37, 37.5, 1e17, 0x25, 0o57, 0b101, 1_000_037, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 37, name: "item37", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l38() {
    var text = "line 38\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 38 \'quoted\'';
    var tpl = `head 38 ${text.length} middle ${single.length + 38} tail`;
    var numbers = [0, 38, 38.5, 1e18, 0x26, 0o67, 0b101, 1_000_038, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 38, name: "item38", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l39() {
    var text = "line 39\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 39 \'quoted\'';
    var tpl = `head 39 ${text.length} middle ${single.length + 39} tail`;
    var numbers = [0, 39, 39.5, 1e19, 0x27, 0o77, 0b101, 1_000_039, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 39, name: "item39", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l40() {
    var text = "line 40\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 40 \'quoted\'';
    var tpl = `head 40 ${text.length} middle ${single.length + 40} tail`;
    var numbers = [0, 40, 40.5, 1e0, 0x28, 0o07, 0b101, 1_000_040, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 40, name: "item40", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l41() {
    var text = "line 41\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 41 \'quoted\'';
    var tpl = `head 41 ${text.length} middle ${single.length + 41} tail`;
    var numbers = [0, 41, 41.5, 1e1, 0x29, 0o17, 0b101, 1_000_041, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 41, name: "item41", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l42() {
    var text = "line 42\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 42 \'quoted\'';
    var tpl = `head 42 ${text.length} middle ${single.length + 42} tail`;
    var numbers = [0, 42, 42.5, 1e2, 0x2A, 0o27, 0b101, 1_000_042, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 42, name: "item42", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l43() {
    var text = "line 43\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 43 \'quoted\'';
    var tpl = `head 43 ${text.length} middle ${single.length + 43} tail`;
    var numbers = [0, 43, 43.5, 1e3, 0x2B, 0o37, 0b101, 1_000_043, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 43, name: "item43", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l44() {
    var text = "line 44\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 44 \'quoted\'';
    var tpl = `head 44 ${text.length} middle ${single.length + 44} tail`;
    var numbers = [0, 44, 44.5, 1e4, 0x2C, 0o47, 0b101, 1_000_044, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 44, name: "item44", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l45() {
    var text = "line 45\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 45 \'quoted\'';
    var tpl = `head 45 ${text.length} middle ${single.length + 45} tail`;
    var numbers = [0, 45, 45.5, 1e5, 0x2D, 0o57, 0b101, 1_000_045, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 45, name: "item45", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l46() {
    var text = "line 46\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 46 \'quoted\'';
    var tpl = `head 46 ${text.length} middle ${single.length + 46} tail`;
    var numbers = [0, 46, 46.5, 1e6, 0x2E, 0o67, 0b101, 1_000_046, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 46, name: "item46", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l47() {
    var text = "line 47\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 47 \'quoted\'';
    var tpl = `head 47 ${text.length} middle ${single.length + 47} tail`;
    var numbers = [0, 47, 47.5, 1e7, 0x2F, 0o77, 0b101, 1_000_047, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 47, name: "item47", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l48() {
    var text = "line 48\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 48 \'quoted\'';
    var tpl = `head 48 ${text.length} middle ${single.length + 48} tail`;
    var numbers = [0, 48, 48.5, 1e8, 0x30, 0o07, 0b101, 1_000_048, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 48, name: "item48", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l49() {
    var text = "line 49\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 49 \'quoted\'';
    var tpl = `head 49 ${text.length} middle ${single.length + 49} tail`;
    var numbers = [0, 49, 49.5, 1e9, 0x31, 0o17, 0b101, 1_000_049, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 49, name: "item49", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l50() {
    var text = "line 50\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 50 \'quoted\'';
    var tpl = `head 50 ${text.length} middle ${single.length + 50} tail`;
    var numbers = [0, 50, 50.5, 1e10, 0x32, 0o27, 0b101, 1_000_050, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 50, name: "item50", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l51() {
    var text = "line 51\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 51 \'quoted\'';
    var tpl = `head 51 ${text.length} middle ${single.length + 51} tail`;
    var numbers = [0, 51, 51.5, 1e11, 0x33, 0o37, 0b101, 1_000_051, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 51, name: "item51", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l52() {
    var text = "line 52\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 52 \'quoted\'';
    var tpl = `head 52 ${text.length} middle ${single.length + 52} tail`;
    var numbers = [0, 52, 52.5, 1e12, 0x34, 0o47, 0b101, 1_000_052, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 52, name: "item52", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l53() {
    var text = "line 53\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 53 \'quoted\'';
    var tpl = `head 53 ${text.length} middle ${single.length + 53} tail`;
    var numbers = [0, 53, 53.5, 1e13, 0x35, 0o57, 0b101, 1_000_053, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 53, name: "item53", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l54() {
    var text = "line 54\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 54 \'quoted\'';
    var tpl = `head 54 ${text.length} middle ${single.length + 54} tail`;
    var numbers = [0, 54, 54.5, 1e14, 0x36, 0o67, 0b101, 1_000_054, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 54, name: "item54", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l55() {
    var text = "line 55\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 55 \'quoted\'';
    var tpl = `head 55 ${text.length} middle ${single.length + 55} tail`;
    var numbers = [0, 55, 55.5, 1e15, 0x37, 0o77, 0b101, 1_000_055, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 55, name: "item55", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l56() {
    var text = "line 56\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 56 \'quoted\'';
    var tpl = `head 56 ${text.length} middle ${single.length + 56} tail`;
    var numbers = [0, 56, 56.5, 1e16, 0x38, 0o07, 0b101, 1_000_056, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 56, name: "item56", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l57() {
    var text = "line 57\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 57 \'quoted\'';
    var tpl = `head 57 ${text.length} middle ${single.length + 57} tail`;
    var numbers = [0, 57, 57.5, 1e17, 0x39, 0o17, 0b101, 1_000_057, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 57, name: "item57", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l58() {
    var text = "line 58\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 58 \'quoted\'';
    var tpl = `head 58 ${text.length} middle ${single.length + 58} tail`;
    var numbers = [0, 58, 58.5, 1e18, 0x3A, 0o27, 0b101, 1_000_058, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 58, name: "item58", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l59() {
    var text = "line 59\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 59 \'quoted\'';
    var tpl = `head 59 ${text.length} middle ${single.length + 59} tail`;
    var numbers = [0, 59, 59.5, 1e19, 0x3B, 0o37, 0b101, 1_000_059, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 59, name: "item59", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l60() {
    var text = "line 60\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 60 \'quoted\'';
    var tpl = `head 60 ${text.length} middle ${single.length + 60} tail`;
    var numbers = [0, 60, 60.5, 1e0, 0x3C, 0o47, 0b101, 1_000_060, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 60, name: "item60", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l61() {
    var text = "line 61\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 61 \'quoted\'';
    var tpl = `head 61 ${text.length} middle ${single.length + 61} tail`;
    var numbers = [0, 61, 61.5, 1e1, 0x3D, 0o57, 0b101, 1_000_061, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 61, name: "item61", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l62() {
    var text = "line 62\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 62 \'quoted\'';
    var tpl = `head 62 ${text.length} middle ${single.length + 62} tail`;
    var numbers = [0, 62, 62.5, 1e2, 0x3E, 0o67, 0b101, 1_000_062, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 62, name: "item62", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l63() {
    var text = "line 63\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 63 \'quoted\'';
    var tpl = `head 63 ${text.length} middle ${single.length + 63} tail`;
    var numbers = [0, 63, 63.5, 1e3, 0x3F, 0o77, 0b101, 1_000_063, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 63, name: "item63", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l64() {
    var text = "line 64\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 64 \'quoted\'';
    var tpl = `head 64 ${text.length} middle ${single.length + 64} tail`;
    var numbers = [0, 64, 64.5, 1e4, 0x40, 0o07, 0b101, 1_000_064, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 64, name: "item64", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l65() {
    var text = "line 65\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 65 \'quoted\'';
    var tpl = `head 65 ${text.length} middle ${single.length + 65} tail`;
    var numbers = [0, 65, 65.5, 1e5, 0x41, 0o17, 0b101, 1_000_065, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 65, name: "item65", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l66() {
    var text = "line 66\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 66 \'quoted\'';
    var tpl = `head 66 ${text.length} middle ${single.length + 66} tail`;
    var numbers = [0, 66, 66.5, 1e6, 0x42, 0o27, 0b101, 1_000_066, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 66, name: "item66", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l67() {
    var text = "line 67\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 67 \'quoted\'';
    var tpl = `head 67 ${text.length} middle ${single.length + 67} tail`;
    var numbers = [0, 67, 67.5, 1e7, 0x43, 0o37, 0b101, 1_000_067, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 67, name: "item67", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l68() {
    var text = "line 68\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 68 \'quoted\'';
    var tpl = `head 68 ${text.length} middle ${single.length + 68} tail`;
    var numbers = [0, 68, 68.5, 1e8, 0x44, 0o47, 0b101, 1_000_068, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 68, name: "item68", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l69() {
    var text = "line 69\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 69 \'quoted\'';
    var tpl = `head 69 ${text.length} middle ${single.length + 69} tail`;
    var numbers = [0, 69, 69.5, 1e9, 0x45, 0o57, 0b101, 1_000_069, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 69, name: "item69", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l70() {
    var text = "line 70\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 70 \'quoted\'';
    var tpl = `head 70 ${text.length} middle ${single.length + 70} tail`;
    var numbers = [0, 70, 70.5, 1e10, 0x46, 0o67, 0b101, 1_000_070, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 70, name: "item70", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l71() {
    var text = "line 71\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 71 \'quoted\'';
    var tpl = `head 71 ${text.length} middle ${single.length + 71} tail`;
    var numbers = [0, 71, 71.5, 1e11, 0x47, 0o77, 0b101, 1_000_071, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 71, name: "item71", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l72() {
    var text = "line 72\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 72 \'quoted\'';
    var tpl = `head 72 ${text.length} middle ${single.length + 72} tail`;
    var numbers = [0, 72, 72.5, 1e12, 0x48, 0o07, 0b101, 1_000_072, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 72, name: "item72", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l73() {
    var text = "line 73\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 73 \'quoted\'';
    var tpl = `head 73 ${text.length} middle ${single.length + 73} tail`;
    var numbers = [0, 73, 73.5, 1e13, 0x49, 0o17, 0b101, 1_000_073, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 73, name: "item73", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l74() {
    var text = "line 74\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 74 \'quoted\'';
    var tpl = `head 74 ${text.length} middle ${single.length + 74} tail`;
    var numbers = [0, 74, 74.5, 1e14, 0x4A, 0o27, 0b101, 1_000_074, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 74, name: "item74", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l75() {
    var text = "line 75\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 75 \'quoted\'';
    var tpl = `head 75 ${text.length} middle ${single.length + 75} tail`;
    var numbers = [0, 75, 75.5, 1e15, 0x4B, 0o37, 0b101, 1_000_075, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 75, name: "item75", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l76() {
    var text = "line 76\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 76 \'quoted\'';
    var tpl = `head 76 ${text.length} middle ${single.length + 76} tail`;
    var numbers = [0, 76, 76.5, 1e16, 0x4C, 0o47, 0b101, 1_000_076, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 76, name: "item76", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l77() {
    var text = "line 77\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 77 \'quoted\'';
    var tpl = `head 77 ${text.length} middle ${single.length + 77} tail`;
    var numbers = [0, 77, 77.5, 1e17, 0x4D, 0o57, 0b101, 1_000_077, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 77, name: "item77", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l78() {
    var text = "line 78\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 78 \'quoted\'';
    var tpl = `head 78 ${text.length} middle ${single.length + 78} tail`;
    var numbers = [0, 78, 78.5, 1e18, 0x4E, 0o67, 0b101, 1_000_078, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 78, name: "item78", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l79() {
    var text = "line 79\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 79 \'quoted\'';
    var tpl = `head 79 ${text.length} middle ${single.length + 79} tail`;
    var numbers = [0, 79, 79.5, 1e19, 0x4F, 0o77, 0b101, 1_000_079, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 79, name: "item79", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l80() {
    var text = "line 80\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 80 \'quoted\'';
    var tpl = `head 80 ${text.length} middle ${single.length + 80} tail`;
    var numbers = [0, 80, 80.5, 1e0, 0x50, 0o07, 0b101, 1_000_080, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 80, name: "item80", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l81() {
    var text = "line 81\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 81 \'quoted\'';
    var tpl = `head 81 ${text.length} middle ${single.length + 81} tail`;
    var numbers = [0, 81, 81.5, 1e1, 0x51, 0o17, 0b101, 1_000_081, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 81, name: "item81", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l82() {
    var text = "line 82\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 82 \'quoted\'';
    var tpl = `head 82 ${text.length} middle ${single.length + 82} tail`;
    var numbers = [0, 82, 82.5, 1e2, 0x52, 0o27, 0b101, 1_000_082, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 82, name: "item82", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l83() {
    var text = "line 83\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 83 \'quoted\'';
    var tpl = `head 83 ${text.length} middle ${single.length + 83} tail`;
    var numbers = [0, 83, 83.5, 1e3, 0x53, 0o37, 0b101, 1_000_083, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 83, name: "item83", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l84() {
    var text = "line 84\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 84 \'quoted\'';
    var tpl = `head 84 ${text.length} middle ${single.length + 84} tail`;
    var numbers = [0, 84, 84.5, 1e4, 0x54, 0o47, 0b101, 1_000_084, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 84, name: "item84", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l85() {
    var text = "line 85\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 85 \'quoted\'';
    var tpl = `head 85 ${text.length} middle ${single.length + 85} tail`;
    var numbers = [0, 85, 85.5, 1e5, 0x55, 0o57, 0b101, 1_000_085, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 85, name: "item85", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l86() {
    var text = "line 86\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 86 \'quoted\'';
    var tpl = `head 86 ${text.length} middle ${single.length + 86} tail`;
    var numbers = [0, 86, 86.5, 1e6, 0x56, 0o67, 0b101, 1_000_086, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 86, name: "item86", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l87() {
    var text = "line 87\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 87 \'quoted\'';
    var tpl = `head 87 ${text.length} middle ${single.length + 87} tail`;
    var numbers = [0, 87, 87.5, 1e7, 0x57, 0o77, 0b101, 1_000_087, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 87, name: "item87", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l88() {
    var text = "line 88\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 88 \'quoted\'';
    var tpl = `head 88 ${text.length} middle ${single.length + 88} tail`;
    var numbers = [0, 88, 88.5, 1e8, 0x58, 0o07, 0b101, 1_000_088, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 88, name: "item88", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l89() {
    var text = "line 89\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 89 \'quoted\'';
    var tpl = `head 89 ${text.length} middle ${single.length + 89} tail`;
    var numbers = [0, 89, 89.5, 1e9, 0x59, 0o17, 0b101, 1_000_089, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 89, name: "item89", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l90() {
    var text = "line 90\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 90 \'quoted\'';
    var tpl = `head 90 ${text.length} middle ${single.length + 90} tail`;
    var numbers = [0, 90, 90.5, 1e10, 0x5A, 0o27, 0b101, 1_000_090, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 90, name: "item90", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l91() {
    var text = "line 91\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 91 \'quoted\'';
    var tpl = `head 91 ${text.length} middle ${single.length + 91} tail`;
    var numbers = [0, 91, 91.5, 1e11, 0x5B, 0o37, 0b101, 1_000_091, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 91, name: "item91", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l92() {
    var text = "line 92\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 92 \'quoted\'';
    var tpl = `head 92 ${text.length} middle ${single.length + 92} tail`;
    var numbers = [0, 92, 92.5, 1e12, 0x5C, 0o47, 0b101, 1_000_092, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 92, name: "item92", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l93() {
    var text = "line 93\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 93 \'quoted\'';
    var tpl = `head 93 ${text.length} middle ${single.length + 93} tail`;
    var numbers = [0, 93, 93.5, 1e13, 0x5D, 0o57, 0b101, 1_000_093, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 93, name: "item93", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l94() {
    var text = "line 94\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 94 \'quoted\'';
    var tpl = `head 94 ${text.length} middle ${single.length + 94} tail`;
    var numbers = [0, 94, 94.5, 1e14, 0x5E, 0o67, 0b101, 1_000_094, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 94, name: "item94", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l95() {
    var text = "line 95\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 95 \'quoted\'';
    var tpl = `head 95 ${text.length} middle ${single.length + 95} tail`;
    var numbers = [0, 95, 95.5, 1e15, 0x5F, 0o77, 0b101, 1_000_095, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 95, name: "item95", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l96() {
    var text = "line 96\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 96 \'quoted\'';
    var tpl = `head 96 ${text.length} middle ${single.length + 96} tail`;
    var numbers = [0, 96, 96.5, 1e16, 0x60, 0o07, 0b101, 1_000_096, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 96, name: "item96", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l97() {
    var text = "line 97\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 97 \'quoted\'';
    var tpl = `head 97 ${text.length} middle ${single.length + 97} tail`;
    var numbers = [0, 97, 97.5, 1e17, 0x61, 0o17, 0b101, 1_000_097, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 97, name: "item97", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l98() {
    var text = "line 98\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 98 \'quoted\'';
    var tpl = `head 98 ${text.length} middle ${single.length + 98} tail`;
    var numbers = [0, 98, 98.5, 1e18, 0x62, 0o27, 0b101, 1_000_098, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 98, name: "item98", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l99() {
    var text = "line 99\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 99 \'quoted\'';
    var tpl = `head 99 ${text.length} middle ${single.length + 99} tail`;
    var numbers = [0, 99, 99.5, 1e19, 0x63, 0o37, 0b101, 1_000_099, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 99, name: "item99", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l100() {
    var text = "line 100\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 100 \'quoted\'';
    var tpl = `head 100 ${text.length} middle ${single.length + 100} tail`;
    var numbers = [0, 100, 100.5, 1e0, 0x64, 0o47, 0b101, 1_000_100, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 100, name: "item100", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l101() {
    var text = "line 101\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 101 \'quoted\'';
    var tpl = `head 101 ${text.length} middle ${single.length + 101} tail`;
    var numbers = [0, 101, 101.5, 1e1, 0x65, 0o57, 0b101, 1_000_101, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 101, name: "item101", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l102() {
    var text = "line 102\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 102 \'quoted\'';
    var tpl = `head 102 ${text.length} middle ${single.length + 102} tail`;
    var numbers = [0, 102, 102.5, 1e2, 0x66, 0o67, 0b101, 1_000_102, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 102, name: "item102", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l103() {
    var text = "line 103\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 103 \'quoted\'';
    var tpl = `head 103 ${text.length} middle ${single.length + 103} tail`;
    var numbers = [0, 103, 103.5, 1e3, 0x67, 0o77, 0b101, 1_000_103, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 103, name: "item103", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l104() {
    var text = "line 104\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 104 \'quoted\'';
    var tpl = `head 104 ${text.length} middle ${single.length + 104} tail`;
    var numbers = [0, 104, 104.5, 1e4, 0x68, 0o07, 0b101, 1_000_104, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 104, name: "item104", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l105() {
    var text = "line 105\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 105 \'quoted\'';
    var tpl = `head 105 ${text.length} middle ${single.length + 105} tail`;
    var numbers = [0, 105, 105.5, 1e5, 0x69, 0o17, 0b101, 1_000_105, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 105, name: "item105", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l106() {
    var text = "line 106\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 106 \'quoted\'';
    var tpl = `head 106 ${text.length} middle ${single.length + 106} tail`;
    var numbers = [0, 106, 106.5, 1e6, 0x6A, 0o27, 0b101, 1_000_106, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 106, name: "item106", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l107() {
    var text = "line 107\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 107 \'quoted\'';
    var tpl = `head 107 ${text.length} middle ${single.length + 107} tail`;
    var numbers = [0, 107, 107.5, 1e7, 0x6B, 0o37, 0b101, 1_000_107, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 107, name: "item107", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l108() {
    var text = "line 108\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 108 \'quoted\'';
    var tpl = `head 108 ${text.length} middle ${single.length + 108} tail`;
    var numbers = [0, 108, 108.5, 1e8, 0x6C, 0o47, 0b101, 1_000_108, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 108, name: "item108", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l109() {
    var text = "line 109\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 109 \'quoted\'';
    var tpl = `head 109 ${text.length} middle ${single.length + 109} tail`;
    var numbers = [0, 109, 109.5, 1e9, 0x6D, 0o57, 0b101, 1_000_109, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 109, name: "item109", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l110() {
    var text = "line 110\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 110 \'quoted\'';
    var tpl = `head 110 ${text.length} middle ${single.length + 110} tail`;
    var numbers = [0, 110, 110.5, 1e10, 0x6E, 0o67, 0b101, 1_000_110, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 110, name: "item110", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l111() {
    var text = "line 111\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 111 \'quoted\'';
    var tpl = `head 111 ${text.length} middle ${single.length + 111} tail`;
    var numbers = [0, 111, 111.5, 1e11, 0x6F, 0o77, 0b101, 1_000_111, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 111, name: "item111", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l112() {
    var text = "line 112\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 112 \'quoted\'';
    var tpl = `head 112 ${text.length} middle ${single.length + 112} tail`;
    var numbers = [0, 112, 112.5, 1e12, 0x70, 0o07, 0b101, 1_000_112, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 112, name: "item112", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l113() {
    var text = "line 113\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 113 \'quoted\'';
    var tpl = `head 113 ${text.length} middle ${single.length + 113} tail`;
    var numbers = [0, 113, 113.5, 1e13, 0x71, 0o17, 0b101, 1_000_113, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 113, name: "item113", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l114() {
    var text = "line 114\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 114 \'quoted\'';
    var tpl = `head 114 ${text.length} middle ${single.length + 114} tail`;
    var numbers = [0, 114, 114.5, 1e14, 0x72, 0o27, 0b101, 1_000_114, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 114, name: "item114", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l115() {
    var text = "line 115\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 115 \'quoted\'';
    var tpl = `head 115 ${text.length} middle ${single.length + 115} tail`;
    var numbers = [0, 115, 115.5, 1e15, 0x73, 0o37, 0b101, 1_000_115, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 115, name: "item115", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l116() {
    var text = "line 116\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 116 \'quoted\'';
    var tpl = `head 116 ${text.length} middle ${single.length + 116} tail`;
    var numbers = [0, 116, 116.5, 1e16, 0x74, 0o47, 0b101, 1_000_116, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 116, name: "item116", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l117() {
    var text = "line 117\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 117 \'quoted\'';
    var tpl = `head 117 ${text.length} middle ${single.length + 117} tail`;
    var numbers = [0, 117, 117.5, 1e17, 0x75, 0o57, 0b101, 1_000_117, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 117, name: "item117", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l118() {
    var text = "line 118\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 118 \'quoted\'';
    var tpl = `head 118 ${text.length} middle ${single.length + 118} tail`;
    var numbers = [0, 118, 118.5, 1e18, 0x76, 0o67, 0b101, 1_000_118, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 118, name: "item118", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l119() {
    var text = "line 119\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 119 \'quoted\'';
    var tpl = `head 119 ${text.length} middle ${single.length + 119} tail`;
    var numbers = [0, 119, 119.5, 1e19, 0x77, 0o77, 0b101, 1_000_119, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 119, name: "item119", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l120() {
    var text = "line 120\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 120 \'quoted\'';
    var tpl = `head 120 ${text.length} middle ${single.length + 120} tail`;
    var numbers = [0, 120, 120.5, 1e0, 0x78, 0o07, 0b101, 1_000_120, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 120, name: "item120", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l121() {
    var text = "line 121\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 121 \'quoted\'';
    var tpl = `head 121 ${text.length} middle ${single.length + 121} tail`;
    var numbers = [0, 121, 121.5, 1e1, 0x79, 0o17, 0b101, 1_000_121, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 121, name: "item121", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l122() {
    var text = "line 122\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 122 \'quoted\'';
    var tpl = `head 122 ${text.length} middle ${single.length + 122} tail`;
    var numbers = [0, 122, 122.5, 1e2, 0x7A, 0o27, 0b101, 1_000_122, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 122, name: "item122", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l123() {
    var text = "line 123\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 123 \'quoted\'';
    var tpl = `head 123 ${text.length} middle ${single.length + 123} tail`;
    var numbers = [0, 123, 123.5, 1e3, 0x7B, 0o37, 0b101, 1_000_123, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 123, name: "item123", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l124() {
    var text = "line 124\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 124 \'quoted\'';
    var tpl = `head 124 ${text.length} middle ${single.length + 124} tail`;
    var numbers = [0, 124, 124.5, 1e4, 0x7C, 0o47, 0b101, 1_000_124, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 124, name: "item124", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l125() {
    var text = "line 125\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 125 \'quoted\'';
    var tpl = `head 125 ${text.length} middle ${single.length + 125} tail`;
    var numbers = [0, 125, 125.5, 1e5, 0x7D, 0o57, 0b101, 1_000_125, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 125, name: "item125", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l126() {
    var text = "line 126\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 126 \'quoted\'';
    var tpl = `head 126 ${text.length} middle ${single.length + 126} tail`;
    var numbers = [0, 126, 126.5, 1e6, 0x7E, 0o67, 0b101, 1_000_126, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 126, name: "item126", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l127() {
    var text = "line 127\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 127 \'quoted\'';
    var tpl = `head 127 ${text.length} middle ${single.length + 127} tail`;
    var numbers = [0, 127, 127.5, 1e7, 0x7F, 0o77, 0b101, 1_000_127, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 127, name: "item127", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l128() {
    var text = "line 128\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 128 \'quoted\'';
    var tpl = `head 128 ${text.length} middle ${single.length + 128} tail`;
    var numbers = [0, 128, 128.5, 1e8, 0x80, 0o07, 0b101, 1_000_128, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 128, name: "item128", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l129() {
    var text = "line 129\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 129 \'quoted\'';
    var tpl = `head 129 ${text.length} middle ${single.length + 129} tail`;
    var numbers = [0, 129, 129.5, 1e9, 0x81, 0o17, 0b101, 1_000_129, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 129, name: "item129", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l130() {
    var text = "line 130\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 130 \'quoted\'';
    var tpl = `head 130 ${text.length} middle ${single.length + 130} tail`;
    var numbers = [0, 130, 130.5, 1e10, 0x82, 0o27, 0b101, 1_000_130, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 130, name: "item130", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l131() {
    var text = "line 131\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 131 \'quoted\'';
    var tpl = `head 131 ${text.length} middle ${single.length + 131} tail`;
    var numbers = [0, 131, 131.5, 1e11, 0x83, 0o37, 0b101, 1_000_131, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 131, name: "item131", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l132() {
    var text = "line 132\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 132 \'quoted\'';
    var tpl = `head 132 ${text.length} middle ${single.length + 132} tail`;
    var numbers = [0, 132, 132.5, 1e12, 0x84, 0o47, 0b101, 1_000_132, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 132, name: "item132", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l133() {
    var text = "line 133\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 133 \'quoted\'';
    var tpl = `head 133 ${text.length} middle ${single.length + 133} tail`;
    var numbers = [0, 133, 133.5, 1e13, 0x85, 0o57, 0b101, 1_000_133, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 133, name: "item133", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l134() {
    var text = "line 134\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 134 \'quoted\'';
    var tpl = `head 134 ${text.length} middle ${single.length + 134} tail`;
    var numbers = [0, 134, 134.5, 1e14, 0x86, 0o67, 0b101, 1_000_134, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 134, name: "item134", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l135() {
    var text = "line 135\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 135 \'quoted\'';
    var tpl = `head 135 ${text.length} middle ${single.length + 135} tail`;
    var numbers = [0, 135, 135.5, 1e15, 0x87, 0o77, 0b101, 1_000_135, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 135, name: "item135", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l136() {
    var text = "line 136\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 136 \'quoted\'';
    var tpl = `head 136 ${text.length} middle ${single.length + 136} tail`;
    var numbers = [0, 136, 136.5, 1e16, 0x88, 0o07, 0b101, 1_000_136, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 136, name: "item136", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l137() {
    var text = "line 137\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 137 \'quoted\'';
    var tpl = `head 137 ${text.length} middle ${single.length + 137} tail`;
    var numbers = [0, 137, 137.5, 1e17, 0x89, 0o17, 0b101, 1_000_137, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 137, name: "item137", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l138() {
    var text = "line 138\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 138 \'quoted\'';
    var tpl = `head 138 ${text.length} middle ${single.length + 138} tail`;
    var numbers = [0, 138, 138.5, 1e18, 0x8A, 0o27, 0b101, 1_000_138, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 138, name: "item138", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l139() {
    var text = "line 139\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 139 \'quoted\'';
    var tpl = `head 139 ${text.length} middle ${single.length + 139} tail`;
    var numbers = [0, 139, 139.5, 1e19, 0x8B, 0o37, 0b101, 1_000_139, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 139, name: "item139", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l140() {
    var text = "line 140\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 140 \'quoted\'';
    var tpl = `head 140 ${text.length} middle ${single.length + 140} tail`;
    var numbers = [0, 140, 140.5, 1e0, 0x8C, 0o47, 0b101, 1_000_140, .25, 6.02e23];
    var pattern = /ab+c[0-9]?/gi;
    var data = { id: 140, name: "item140", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l141() {
    var text = "line 141\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 141 \'quoted\'';
    var tpl = `head 141 ${text.length} middle ${single.length + 141} tail`;
    var numbers = [0, 141, 141.5, 1e1, 0x8D, 0o57, 0b101, 1_000_141, .25, 6.02e23];
    var pattern = /ab+c[1-9]?/gi;
    var data = { id: 141, name: "item141", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l142() {
    var text = "line 142\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 142 \'quoted\'';
    var tpl = `head 142 ${text.length} middle ${single.length + 142} tail`;
    var numbers = [0, 142, 142.5, 1e2, 0x8E, 0o67, 0b101, 1_000_142, .25, 6.02e23];
    var pattern = /ab+c[2-9]?/gi;
    var data = { id: 142, name: "item142", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l143() {
    var text = "line 143\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 143 \'quoted\'';
    var tpl = `head 143 ${text.length} middle ${single.length + 143} tail`;
    var numbers = [0, 143, 143.5, 1e3, 0x8F, 0o77, 0b101, 1_000_143, .25, 6.02e23];
    var pattern = /ab+c[3-9]?/gi;
    var data = { id: 143, name: "item143", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l144() {
    var text = "line 144\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 144 \'quoted\'';
    var tpl = `head 144 ${text.length} middle ${single.length + 144} tail`;
    var numbers = [0, 144, 144.5, 1e4, 0x90, 0o07, 0b101, 1_000_144, .25, 6.02e23];
    var pattern = /ab+c[4-9]?/gi;
    var data = { id: 144, name: "item144", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l145() {
    var text = "line 145\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 145 \'quoted\'';
    var tpl = `head 145 ${text.length} middle ${single.length + 145} tail`;
    var numbers = [0, 145, 145.5, 1e5, 0x91, 0o17, 0b101, 1_000_145, .25, 6.02e23];
    var pattern = /ab+c[5-9]?/gi;
    var data = { id: 145, name: "item145", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l146() {
    var text = "line 146\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 146 \'quoted\'';
    var tpl = `head 146 ${text.length} middle ${single.length + 146} tail`;
    var numbers = [0, 146, 146.5, 1e6, 0x92, 0o27, 0b101, 1_000_146, .25, 6.02e23];
    var pattern = /ab+c[6-9]?/gi;
    var data = { id: 146, name: "item146", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l147() {
    var text = "line 147\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 147 \'quoted\'';
    var tpl = `head 147 ${text.length} middle ${single.length + 147} tail`;
    var numbers = [0, 147, 147.5, 1e7, 0x93, 0o37, 0b101, 1_000_147, .25, 6.02e23];
    var pattern = /ab+c[7-9]?/gi;
    var data = { id: 147, name: "item147", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l148() {
    var text = "line 148\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 148 \'quoted\'';
    var tpl = `head 148 ${text.length} middle ${single.length + 148} tail`;
    var numbers = [0, 148, 148.5, 1e8, 0x94, 0o47, 0b101, 1_000_148, .25, 6.02e23];
    var pattern = /ab+c[8-9]?/gi;
    var data = { id: 148, name: "item148", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
function l149() {
    var text = "line 149\tcolumn\n\"quoted\" \u0041\u{1F600} \x41";
    var single = 'single 149 \'quoted\'';
    var tpl = `head 149 ${text.length} middle ${single.length + 149} tail`;
    var numbers = [0, 149, 149.5, 1e9, 0x95, 0o57, 0b101, 1_000_149, .25, 6.02e23];
    var pattern = /ab+c[9-9]?/gi;
    var data = { id: 149, name: "item149", tags: ["a", "b", "c"], nested: { ok: true, none: null, list: [1, 2, [3, 4]] } };
    return [text, single, tpl, numbers, pattern, data].length;
}
l1();
*/
