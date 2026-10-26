// SPDX-FileCopyrightText: 2026 Broiler Platform contributors
// SPDX-License-Identifier: Apache-2.0
//
// CLASSES (CONTROL) - the workload in classes.js as one block comment, of the same goal (script).
// The same characters reach the tokenizer, and no construct reaches the parser; the difference between the two
// is the cost of the constructs.

/*
class Base0 {
    static count = 0;
    #secret = 0;
    label = "base0";
    static { Base0.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 0]() { return this.label; }
    static make(seed) { return new Base0(seed); }
}
class Derived0 extends Base0 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base1 {
    static count = 0;
    #secret = 1;
    label = "base1";
    static { Base1.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 1]() { return this.label; }
    static make(seed) { return new Base1(seed); }
}
class Derived1 extends Base1 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base2 {
    static count = 0;
    #secret = 2;
    label = "base2";
    static { Base2.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 2]() { return this.label; }
    static make(seed) { return new Base2(seed); }
}
class Derived2 extends Base2 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base3 {
    static count = 0;
    #secret = 3;
    label = "base3";
    static { Base3.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 3]() { return this.label; }
    static make(seed) { return new Base3(seed); }
}
class Derived3 extends Base3 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base4 {
    static count = 0;
    #secret = 4;
    label = "base4";
    static { Base4.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 4]() { return this.label; }
    static make(seed) { return new Base4(seed); }
}
class Derived4 extends Base4 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base5 {
    static count = 0;
    #secret = 5;
    label = "base5";
    static { Base5.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 5]() { return this.label; }
    static make(seed) { return new Base5(seed); }
}
class Derived5 extends Base5 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base6 {
    static count = 0;
    #secret = 6;
    label = "base6";
    static { Base6.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 6]() { return this.label; }
    static make(seed) { return new Base6(seed); }
}
class Derived6 extends Base6 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base7 {
    static count = 0;
    #secret = 7;
    label = "base7";
    static { Base7.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 7]() { return this.label; }
    static make(seed) { return new Base7(seed); }
}
class Derived7 extends Base7 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base8 {
    static count = 0;
    #secret = 8;
    label = "base8";
    static { Base8.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 8]() { return this.label; }
    static make(seed) { return new Base8(seed); }
}
class Derived8 extends Base8 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base9 {
    static count = 0;
    #secret = 9;
    label = "base9";
    static { Base9.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 9]() { return this.label; }
    static make(seed) { return new Base9(seed); }
}
class Derived9 extends Base9 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base10 {
    static count = 0;
    #secret = 10;
    label = "base10";
    static { Base10.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 10]() { return this.label; }
    static make(seed) { return new Base10(seed); }
}
class Derived10 extends Base10 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base11 {
    static count = 0;
    #secret = 11;
    label = "base11";
    static { Base11.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 11]() { return this.label; }
    static make(seed) { return new Base11(seed); }
}
class Derived11 extends Base11 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base12 {
    static count = 0;
    #secret = 12;
    label = "base12";
    static { Base12.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 12]() { return this.label; }
    static make(seed) { return new Base12(seed); }
}
class Derived12 extends Base12 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base13 {
    static count = 0;
    #secret = 13;
    label = "base13";
    static { Base13.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 13]() { return this.label; }
    static make(seed) { return new Base13(seed); }
}
class Derived13 extends Base13 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base14 {
    static count = 0;
    #secret = 14;
    label = "base14";
    static { Base14.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 14]() { return this.label; }
    static make(seed) { return new Base14(seed); }
}
class Derived14 extends Base14 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base15 {
    static count = 0;
    #secret = 15;
    label = "base15";
    static { Base15.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 15]() { return this.label; }
    static make(seed) { return new Base15(seed); }
}
class Derived15 extends Base15 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base16 {
    static count = 0;
    #secret = 16;
    label = "base16";
    static { Base16.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 16]() { return this.label; }
    static make(seed) { return new Base16(seed); }
}
class Derived16 extends Base16 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base17 {
    static count = 0;
    #secret = 17;
    label = "base17";
    static { Base17.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 17]() { return this.label; }
    static make(seed) { return new Base17(seed); }
}
class Derived17 extends Base17 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base18 {
    static count = 0;
    #secret = 18;
    label = "base18";
    static { Base18.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 18]() { return this.label; }
    static make(seed) { return new Base18(seed); }
}
class Derived18 extends Base18 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base19 {
    static count = 0;
    #secret = 19;
    label = "base19";
    static { Base19.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 19]() { return this.label; }
    static make(seed) { return new Base19(seed); }
}
class Derived19 extends Base19 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base20 {
    static count = 0;
    #secret = 20;
    label = "base20";
    static { Base20.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 20]() { return this.label; }
    static make(seed) { return new Base20(seed); }
}
class Derived20 extends Base20 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base21 {
    static count = 0;
    #secret = 21;
    label = "base21";
    static { Base21.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 21]() { return this.label; }
    static make(seed) { return new Base21(seed); }
}
class Derived21 extends Base21 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base22 {
    static count = 0;
    #secret = 22;
    label = "base22";
    static { Base22.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 22]() { return this.label; }
    static make(seed) { return new Base22(seed); }
}
class Derived22 extends Base22 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base23 {
    static count = 0;
    #secret = 23;
    label = "base23";
    static { Base23.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 23]() { return this.label; }
    static make(seed) { return new Base23(seed); }
}
class Derived23 extends Base23 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base24 {
    static count = 0;
    #secret = 24;
    label = "base24";
    static { Base24.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 24]() { return this.label; }
    static make(seed) { return new Base24(seed); }
}
class Derived24 extends Base24 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base25 {
    static count = 0;
    #secret = 25;
    label = "base25";
    static { Base25.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 25]() { return this.label; }
    static make(seed) { return new Base25(seed); }
}
class Derived25 extends Base25 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base26 {
    static count = 0;
    #secret = 26;
    label = "base26";
    static { Base26.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 26]() { return this.label; }
    static make(seed) { return new Base26(seed); }
}
class Derived26 extends Base26 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base27 {
    static count = 0;
    #secret = 27;
    label = "base27";
    static { Base27.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 27]() { return this.label; }
    static make(seed) { return new Base27(seed); }
}
class Derived27 extends Base27 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base28 {
    static count = 0;
    #secret = 28;
    label = "base28";
    static { Base28.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 28]() { return this.label; }
    static make(seed) { return new Base28(seed); }
}
class Derived28 extends Base28 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base29 {
    static count = 0;
    #secret = 29;
    label = "base29";
    static { Base29.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 29]() { return this.label; }
    static make(seed) { return new Base29(seed); }
}
class Derived29 extends Base29 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base30 {
    static count = 0;
    #secret = 30;
    label = "base30";
    static { Base30.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 30]() { return this.label; }
    static make(seed) { return new Base30(seed); }
}
class Derived30 extends Base30 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base31 {
    static count = 0;
    #secret = 31;
    label = "base31";
    static { Base31.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 31]() { return this.label; }
    static make(seed) { return new Base31(seed); }
}
class Derived31 extends Base31 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base32 {
    static count = 0;
    #secret = 32;
    label = "base32";
    static { Base32.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 32]() { return this.label; }
    static make(seed) { return new Base32(seed); }
}
class Derived32 extends Base32 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base33 {
    static count = 0;
    #secret = 33;
    label = "base33";
    static { Base33.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 33]() { return this.label; }
    static make(seed) { return new Base33(seed); }
}
class Derived33 extends Base33 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base34 {
    static count = 0;
    #secret = 34;
    label = "base34";
    static { Base34.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 34]() { return this.label; }
    static make(seed) { return new Base34(seed); }
}
class Derived34 extends Base34 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base35 {
    static count = 0;
    #secret = 35;
    label = "base35";
    static { Base35.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 35]() { return this.label; }
    static make(seed) { return new Base35(seed); }
}
class Derived35 extends Base35 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base36 {
    static count = 0;
    #secret = 36;
    label = "base36";
    static { Base36.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 36]() { return this.label; }
    static make(seed) { return new Base36(seed); }
}
class Derived36 extends Base36 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base37 {
    static count = 0;
    #secret = 37;
    label = "base37";
    static { Base37.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 37]() { return this.label; }
    static make(seed) { return new Base37(seed); }
}
class Derived37 extends Base37 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base38 {
    static count = 0;
    #secret = 38;
    label = "base38";
    static { Base38.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 38]() { return this.label; }
    static make(seed) { return new Base38(seed); }
}
class Derived38 extends Base38 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base39 {
    static count = 0;
    #secret = 39;
    label = "base39";
    static { Base39.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 39]() { return this.label; }
    static make(seed) { return new Base39(seed); }
}
class Derived39 extends Base39 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base40 {
    static count = 0;
    #secret = 40;
    label = "base40";
    static { Base40.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 40]() { return this.label; }
    static make(seed) { return new Base40(seed); }
}
class Derived40 extends Base40 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base41 {
    static count = 0;
    #secret = 41;
    label = "base41";
    static { Base41.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 41]() { return this.label; }
    static make(seed) { return new Base41(seed); }
}
class Derived41 extends Base41 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base42 {
    static count = 0;
    #secret = 42;
    label = "base42";
    static { Base42.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 42]() { return this.label; }
    static make(seed) { return new Base42(seed); }
}
class Derived42 extends Base42 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base43 {
    static count = 0;
    #secret = 43;
    label = "base43";
    static { Base43.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 43]() { return this.label; }
    static make(seed) { return new Base43(seed); }
}
class Derived43 extends Base43 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base44 {
    static count = 0;
    #secret = 44;
    label = "base44";
    static { Base44.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 44]() { return this.label; }
    static make(seed) { return new Base44(seed); }
}
class Derived44 extends Base44 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base45 {
    static count = 0;
    #secret = 45;
    label = "base45";
    static { Base45.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 45]() { return this.label; }
    static make(seed) { return new Base45(seed); }
}
class Derived45 extends Base45 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base46 {
    static count = 0;
    #secret = 46;
    label = "base46";
    static { Base46.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 46]() { return this.label; }
    static make(seed) { return new Base46(seed); }
}
class Derived46 extends Base46 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base47 {
    static count = 0;
    #secret = 47;
    label = "base47";
    static { Base47.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 47]() { return this.label; }
    static make(seed) { return new Base47(seed); }
}
class Derived47 extends Base47 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base48 {
    static count = 0;
    #secret = 48;
    label = "base48";
    static { Base48.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 48]() { return this.label; }
    static make(seed) { return new Base48(seed); }
}
class Derived48 extends Base48 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base49 {
    static count = 0;
    #secret = 49;
    label = "base49";
    static { Base49.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 49]() { return this.label; }
    static make(seed) { return new Base49(seed); }
}
class Derived49 extends Base49 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base50 {
    static count = 0;
    #secret = 50;
    label = "base50";
    static { Base50.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 50]() { return this.label; }
    static make(seed) { return new Base50(seed); }
}
class Derived50 extends Base50 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base51 {
    static count = 0;
    #secret = 51;
    label = "base51";
    static { Base51.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 51]() { return this.label; }
    static make(seed) { return new Base51(seed); }
}
class Derived51 extends Base51 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base52 {
    static count = 0;
    #secret = 52;
    label = "base52";
    static { Base52.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 52]() { return this.label; }
    static make(seed) { return new Base52(seed); }
}
class Derived52 extends Base52 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base53 {
    static count = 0;
    #secret = 53;
    label = "base53";
    static { Base53.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 53]() { return this.label; }
    static make(seed) { return new Base53(seed); }
}
class Derived53 extends Base53 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base54 {
    static count = 0;
    #secret = 54;
    label = "base54";
    static { Base54.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 54]() { return this.label; }
    static make(seed) { return new Base54(seed); }
}
class Derived54 extends Base54 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base55 {
    static count = 0;
    #secret = 55;
    label = "base55";
    static { Base55.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 55]() { return this.label; }
    static make(seed) { return new Base55(seed); }
}
class Derived55 extends Base55 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base56 {
    static count = 0;
    #secret = 56;
    label = "base56";
    static { Base56.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 56]() { return this.label; }
    static make(seed) { return new Base56(seed); }
}
class Derived56 extends Base56 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base57 {
    static count = 0;
    #secret = 57;
    label = "base57";
    static { Base57.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 57]() { return this.label; }
    static make(seed) { return new Base57(seed); }
}
class Derived57 extends Base57 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base58 {
    static count = 0;
    #secret = 58;
    label = "base58";
    static { Base58.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 58]() { return this.label; }
    static make(seed) { return new Base58(seed); }
}
class Derived58 extends Base58 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base59 {
    static count = 0;
    #secret = 59;
    label = "base59";
    static { Base59.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 59]() { return this.label; }
    static make(seed) { return new Base59(seed); }
}
class Derived59 extends Base59 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base60 {
    static count = 0;
    #secret = 60;
    label = "base60";
    static { Base60.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 60]() { return this.label; }
    static make(seed) { return new Base60(seed); }
}
class Derived60 extends Base60 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base61 {
    static count = 0;
    #secret = 61;
    label = "base61";
    static { Base61.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 61]() { return this.label; }
    static make(seed) { return new Base61(seed); }
}
class Derived61 extends Base61 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base62 {
    static count = 0;
    #secret = 62;
    label = "base62";
    static { Base62.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 62]() { return this.label; }
    static make(seed) { return new Base62(seed); }
}
class Derived62 extends Base62 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base63 {
    static count = 0;
    #secret = 63;
    label = "base63";
    static { Base63.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 63]() { return this.label; }
    static make(seed) { return new Base63(seed); }
}
class Derived63 extends Base63 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base64 {
    static count = 0;
    #secret = 64;
    label = "base64";
    static { Base64.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 64]() { return this.label; }
    static make(seed) { return new Base64(seed); }
}
class Derived64 extends Base64 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base65 {
    static count = 0;
    #secret = 65;
    label = "base65";
    static { Base65.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 65]() { return this.label; }
    static make(seed) { return new Base65(seed); }
}
class Derived65 extends Base65 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base66 {
    static count = 0;
    #secret = 66;
    label = "base66";
    static { Base66.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 66]() { return this.label; }
    static make(seed) { return new Base66(seed); }
}
class Derived66 extends Base66 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base67 {
    static count = 0;
    #secret = 67;
    label = "base67";
    static { Base67.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 67]() { return this.label; }
    static make(seed) { return new Base67(seed); }
}
class Derived67 extends Base67 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base68 {
    static count = 0;
    #secret = 68;
    label = "base68";
    static { Base68.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 68]() { return this.label; }
    static make(seed) { return new Base68(seed); }
}
class Derived68 extends Base68 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base69 {
    static count = 0;
    #secret = 69;
    label = "base69";
    static { Base69.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 69]() { return this.label; }
    static make(seed) { return new Base69(seed); }
}
class Derived69 extends Base69 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base70 {
    static count = 0;
    #secret = 70;
    label = "base70";
    static { Base70.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 70]() { return this.label; }
    static make(seed) { return new Base70(seed); }
}
class Derived70 extends Base70 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base71 {
    static count = 0;
    #secret = 71;
    label = "base71";
    static { Base71.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 71]() { return this.label; }
    static make(seed) { return new Base71(seed); }
}
class Derived71 extends Base71 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base72 {
    static count = 0;
    #secret = 72;
    label = "base72";
    static { Base72.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 72]() { return this.label; }
    static make(seed) { return new Base72(seed); }
}
class Derived72 extends Base72 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base73 {
    static count = 0;
    #secret = 73;
    label = "base73";
    static { Base73.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 73]() { return this.label; }
    static make(seed) { return new Base73(seed); }
}
class Derived73 extends Base73 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base74 {
    static count = 0;
    #secret = 74;
    label = "base74";
    static { Base74.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 74]() { return this.label; }
    static make(seed) { return new Base74(seed); }
}
class Derived74 extends Base74 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base75 {
    static count = 0;
    #secret = 75;
    label = "base75";
    static { Base75.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 75]() { return this.label; }
    static make(seed) { return new Base75(seed); }
}
class Derived75 extends Base75 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base76 {
    static count = 0;
    #secret = 76;
    label = "base76";
    static { Base76.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 76]() { return this.label; }
    static make(seed) { return new Base76(seed); }
}
class Derived76 extends Base76 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base77 {
    static count = 0;
    #secret = 77;
    label = "base77";
    static { Base77.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 77]() { return this.label; }
    static make(seed) { return new Base77(seed); }
}
class Derived77 extends Base77 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base78 {
    static count = 0;
    #secret = 78;
    label = "base78";
    static { Base78.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 78]() { return this.label; }
    static make(seed) { return new Base78(seed); }
}
class Derived78 extends Base78 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base79 {
    static count = 0;
    #secret = 79;
    label = "base79";
    static { Base79.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 79]() { return this.label; }
    static make(seed) { return new Base79(seed); }
}
class Derived79 extends Base79 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base80 {
    static count = 0;
    #secret = 80;
    label = "base80";
    static { Base80.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 80]() { return this.label; }
    static make(seed) { return new Base80(seed); }
}
class Derived80 extends Base80 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base81 {
    static count = 0;
    #secret = 81;
    label = "base81";
    static { Base81.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 81]() { return this.label; }
    static make(seed) { return new Base81(seed); }
}
class Derived81 extends Base81 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base82 {
    static count = 0;
    #secret = 82;
    label = "base82";
    static { Base82.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 82]() { return this.label; }
    static make(seed) { return new Base82(seed); }
}
class Derived82 extends Base82 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base83 {
    static count = 0;
    #secret = 83;
    label = "base83";
    static { Base83.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 83]() { return this.label; }
    static make(seed) { return new Base83(seed); }
}
class Derived83 extends Base83 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base84 {
    static count = 0;
    #secret = 84;
    label = "base84";
    static { Base84.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 84]() { return this.label; }
    static make(seed) { return new Base84(seed); }
}
class Derived84 extends Base84 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base85 {
    static count = 0;
    #secret = 85;
    label = "base85";
    static { Base85.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 85]() { return this.label; }
    static make(seed) { return new Base85(seed); }
}
class Derived85 extends Base85 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base86 {
    static count = 0;
    #secret = 86;
    label = "base86";
    static { Base86.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 86]() { return this.label; }
    static make(seed) { return new Base86(seed); }
}
class Derived86 extends Base86 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base87 {
    static count = 0;
    #secret = 87;
    label = "base87";
    static { Base87.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 87]() { return this.label; }
    static make(seed) { return new Base87(seed); }
}
class Derived87 extends Base87 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base88 {
    static count = 0;
    #secret = 88;
    label = "base88";
    static { Base88.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 88]() { return this.label; }
    static make(seed) { return new Base88(seed); }
}
class Derived88 extends Base88 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base89 {
    static count = 0;
    #secret = 89;
    label = "base89";
    static { Base89.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 89]() { return this.label; }
    static make(seed) { return new Base89(seed); }
}
class Derived89 extends Base89 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base90 {
    static count = 0;
    #secret = 90;
    label = "base90";
    static { Base90.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 90]() { return this.label; }
    static make(seed) { return new Base90(seed); }
}
class Derived90 extends Base90 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base91 {
    static count = 0;
    #secret = 91;
    label = "base91";
    static { Base91.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 91]() { return this.label; }
    static make(seed) { return new Base91(seed); }
}
class Derived91 extends Base91 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base92 {
    static count = 0;
    #secret = 92;
    label = "base92";
    static { Base92.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 92]() { return this.label; }
    static make(seed) { return new Base92(seed); }
}
class Derived92 extends Base92 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base93 {
    static count = 0;
    #secret = 93;
    label = "base93";
    static { Base93.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 93]() { return this.label; }
    static make(seed) { return new Base93(seed); }
}
class Derived93 extends Base93 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base94 {
    static count = 0;
    #secret = 94;
    label = "base94";
    static { Base94.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 94]() { return this.label; }
    static make(seed) { return new Base94(seed); }
}
class Derived94 extends Base94 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base95 {
    static count = 0;
    #secret = 95;
    label = "base95";
    static { Base95.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 95]() { return this.label; }
    static make(seed) { return new Base95(seed); }
}
class Derived95 extends Base95 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base96 {
    static count = 0;
    #secret = 96;
    label = "base96";
    static { Base96.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 96]() { return this.label; }
    static make(seed) { return new Base96(seed); }
}
class Derived96 extends Base96 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base97 {
    static count = 0;
    #secret = 97;
    label = "base97";
    static { Base97.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 97]() { return this.label; }
    static make(seed) { return new Base97(seed); }
}
class Derived97 extends Base97 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base98 {
    static count = 0;
    #secret = 98;
    label = "base98";
    static { Base98.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 98]() { return this.label; }
    static make(seed) { return new Base98(seed); }
}
class Derived98 extends Base98 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
class Base99 {
    static count = 0;
    #secret = 99;
    label = "base99";
    static { Base99.count = 1; }
    constructor(seed) { this.seed = seed; }
    get secret() { return this.#secret; }
    set secret(value) { this.#secret = value; }
    #twice(n) { return n * 2; }
    double() { return this.#twice(this.#secret); }
    ["computed" + 99]() { return this.label; }
    static make(seed) { return new Base99(seed); }
}
class Derived99 extends Base99 {
    extra = [1, 2, 3];
    constructor(seed) { super(seed); this.more = seed + 1; }
    double() { return super.double() + this.extra.length; }
}
Derived1.make(1);
*/
