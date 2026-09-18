function add(a, b) { return a + b; }
function main(n) { var s = 0; for (var i = 0; i < n; i++) { s = add(s, i) % 1000003; } return s; }
main(1000000);
