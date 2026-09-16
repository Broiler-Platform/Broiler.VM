function main(n) { var o = { x: 0, y: 1 }; for (var i = 0; i < n; i++) { o.x = (o.x + o.y) % 1000003; o.y = i; } return o.x; }
main(1500000);
