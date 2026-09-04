// A pattern with no initialiser COUNTS towards `length` and one with an initialiser stops the
// count, so this function reports 1 rather than 2.
function place({ name, size = 2 }, [x, y] = [0, 0]) {
  return name + ":" + size + ":" + x + "," + y;
}

place({ name: "n" }) + " " + place({ name: "n", size: 5 }, [1, 2]) + " " + place.length;
