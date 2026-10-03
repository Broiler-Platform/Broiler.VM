--> ANNEX B.1'S HTML-LIKE COMMENTS ARE COMMENTS IN A SCRIPT (JSP-7, JSC-239): `<!--` anywhere and
--> `-->` where only whitespace and comments precede it on its line, each to the end of the line. This
--> file opens with three of them. `1 <!-- 2` was read as `1 < !(--2)`, a program the file does not
--> contain, and a line opening with `-->` was a decrement and a comparison.
var x = 1 <!-- the rest of this line is a comment
;
var y = [5]
--> [0]
/*
*/ --> after a block comment that crossed a line
var made = Function('a <!-- not a parameter', 'return a\n--> not returned');
[x, y[0], made(7)].join(' / ');
