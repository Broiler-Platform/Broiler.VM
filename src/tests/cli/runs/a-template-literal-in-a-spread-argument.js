// THE ARGUMENT OF A SPREAD IS A NEW EXPRESSION POSITION, AND A TEMPLATE LITERAL IS NOW ADMITTED
// IN IT. This row also asserted a refusal until the two families met. What it checks now is that
// the spread of a String goes through the iteration protocol rather than over its indices, which
// is the difference an astral character makes visible: one argument per CODE POINT.
function takes(a, b, c) {
  return [a, b, c].join("-");
}

takes(...`ab`, "c") + " " + [...`a\u{1F600}`].length;
